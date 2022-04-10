using CollectIt.Database.Infrastructure;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

namespace CollectIt.API.WebAPI;

public static class ProgramExtensions
{
    public static IServiceCollection AddCollectItOpenIddict(this IServiceCollection services, IWebHostEnvironment environment)
    {
       services.AddOpenIddict()
               .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<PostgresqlCollectItDbContext>();
                })
               .AddServer(options =>
                {
                    options.AcceptAnonymousClients()
                           .AllowAuthorizationCodeFlow()
                           .AllowPasswordFlow()
                           .AllowRefreshTokenFlow();
                    options.AddDevelopmentSigningCertificate()
                           .AddDevelopmentEncryptionCertificate();
                    options.SetTokenEndpointUris("/connect/token")
                           .SetAuthorizationEndpointUris("/connect/authorize")
                           .SetUserinfoEndpointUris("/connect/userinfo");
                    options.RegisterScopes(OpenIddictConstants.Scopes.Email,
                                           OpenIddictConstants.Scopes.Profile,
                                           OpenIddictConstants.Scopes.Roles);
                    
                    var config = options.UseAspNetCore();
                    if (environment.IsDevelopment() || environment.IsStaging())
                    {
                        config.DisableTransportSecurityRequirement();
                    }

                    config.EnableTokenEndpointPassthrough()
                          .EnableAuthorizationEndpointPassthrough()
                          .EnableUserinfoEndpointPassthrough()
                          .EnableStatusCodePagesIntegration();

                });
       return services;
    }
}