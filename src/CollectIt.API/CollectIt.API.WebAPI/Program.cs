using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

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
                throw new NotImplementedException("Cors for production is not setup");
            }

            options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
        });

        builder.Services
               .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                    config.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                    config.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                    config.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
                    config.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
                })
               .AddUserManager<UserManager>()
               .AddRoleManager<RoleManager>()
               .AddEntityFrameworkStores<PostgresqlCollectItDbContext>()
               .AddDefaultTokenProviders();
        builder.Services.AddCollectItOpenIddict(builder.Environment);
        
        builder.Services.AddScoped<ISubscriptionManager, SubscriptionManager>();
        builder.Services.AddDbContext<PostgresqlCollectItDbContext>(config =>
        {
            config.UseNpgsql(builder.Configuration["ConnectionStrings:Postgresql:Development"], 
                             options =>
                             {
                                 options.MigrationsAssembly("CollectIt.Database.Infrastructure");
                                 options.UseNodaTime();
                             });
            config.UseOpenIddict<int>();
        });

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}