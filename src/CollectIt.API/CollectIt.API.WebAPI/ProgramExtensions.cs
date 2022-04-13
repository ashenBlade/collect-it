using CollectIt.Database.Infrastructure;
using Microsoft.IdentityModel.Tokens;
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
                           .UseDbContext<PostgresqlCollectItDbContext>()
                           .ReplaceDefaultEntities<int>();
                })
               .AddServer(options =>
                {
                    // options.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String("YWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWE="))); // 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
                    // For test
                    // options.SetAccessTokenLifetime(TimeSpan.FromDays(365));
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
                        config.DisableTransportSecurityRequirement();
                        config.EnableTokenEndpointPassthrough()
                              .EnableAuthorizationEndpointPassthrough()
                              .EnableUserinfoEndpointPassthrough()
                              .EnableStatusCodePagesIntegration();
                })
               .AddValidation();
       return services;
    }
}