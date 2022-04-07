using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CollectIt.API.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            if (builder.Environment.IsProduction())
            {
                throw new NotImplementedException("Cors for production is not set");
            }

            options.AddDefaultPolicy(policyBuilder =>
            {
                // Only for tests
                policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
                {
                    var jwtOptions = builder.Configuration.GetValue<JwtOptions>("JwtOptions");
                    options.RequireHttpsMetadata = builder.Environment.IsProduction();
                    options.TokenValidationParameters = new TokenValidationParameters()
                                                        {
                                                            ValidateIssuer = true,
                                                            ValidIssuer = jwtOptions.Issuer,
                                                            ValidateAudience = true,
                                                            ValidAudience = jwtOptions.Audience,
                                                            ValidateLifetime = true,
                                                            IssuerSigningKey = jwtOptions.SymmetricSecurityKey,
                                                            ValidateIssuerSigningKey = true
                                                        };
                });
        builder.Services.AddAuthorization();

        builder.Services.AddIdentity<User, Role>(config => 
                {
                    config.User = new UserOptions {RequireUniqueEmail = true,};
                    config.Password = new PasswordOptions
                                      {
                                          RequireDigit = true,
                                          RequiredLength = 6,
                                          RequireLowercase = false,
                                          RequireUppercase = false,
                                          RequiredUniqueChars = 1,
                                          RequireNonAlphanumeric = false,
                                      };
                    config.SignIn = new SignInOptions
                                    {
                                        RequireConfirmedEmail = false,
                                        RequireConfirmedAccount = false,
                                        RequireConfirmedPhoneNumber = false,
                                    };
                })
               .AddUserManager<UserManager>()
               .AddEntityFrameworkStores<PostgresqlCollectItDbContext>()
               .AddDefaultTokenProviders();
        builder.Services.AddScoped<ISubscriptionManager, SubscriptionManager>();
        builder.Services.AddDbContext<PostgresqlCollectItDbContext>(config =>
        {
            config.UseNpgsql(builder.Configuration["ConnectionStrings:Postgresql:Development"], options =>
            {
                options.UseNodaTime();
            });
        });

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}