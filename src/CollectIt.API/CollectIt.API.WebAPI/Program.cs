using System.Reflection;
using CollectIt.API.WebAPI.ModelBinders;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.Database.Infrastructure.Resources.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace CollectIt.API.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(opts =>
        {
            opts.ModelBinderProviders.Insert(0, new RestrictionModelBinderProvider());
        }).AddNewtonsoftJson();

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
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(gen =>
        {

            var file = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var path = Path.Combine(AppContext.BaseDirectory, file);
            gen.IncludeXmlComments(path);
        });
        
        builder.Services
               .AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                })
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
                    config.ClaimsIdentity.UserNameClaimType = Claims.Name;
                    config.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                    config.ClaimsIdentity.RoleClaimType = Claims.Role;
                    config.ClaimsIdentity.EmailClaimType = Claims.Email;
                })
               .AddUserManager<UserManager>()
               .AddRoleManager<RoleManager>()
               .AddEntityFrameworkStores<PostgresqlCollectItDbContext>()
               .AddDefaultTokenProviders();
        
        builder.Services.AddCollectItOpenIddict(builder.Environment);
        
        builder.Services.AddScoped<ISubscriptionManager, SubscriptionManager>();
        builder.Services.AddScoped<ISubscriptionService, PostgresqlSubscriptionService>();
        builder.Services.AddScoped<IImageManager, PostgresqlImageManager>();
        builder.Services.AddScoped<IResourceAcquisitionService, ResourceAcquisitionService>();
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
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}