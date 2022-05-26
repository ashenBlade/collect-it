using CollectIt.Database.Infrastructure;
using OpenIddict.Abstractions;

namespace CollectIt.API.WebAPI;

public static class ProgramExtensions
{
    public static void AddCollectItOpenIddict(this IServiceCollection services)
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
                     options.SetAccessTokenLifetime(TimeSpan.FromDays(365));
                     options.AcceptAnonymousClients()
                             // .AllowAuthorizationCodeFlow()
                            .AllowPasswordFlow();
                     // .AllowRefreshTokenFlow();
                     options.AddDevelopmentSigningCertificate()
                            .AddDevelopmentEncryptionCertificate();
                     options.SetTokenEndpointUris("/connect/token");
                     // .SetAuthorizationEndpointUris("/connect/authorize")
                     // .SetUserinfoEndpointUris("/connect/userinfo");
                     options.RegisterScopes(OpenIddictConstants.Scopes.Email,
                                            OpenIddictConstants.Scopes.Profile,
                                            OpenIddictConstants.Scopes.Roles,
                                            OpenIddictConstants.Scopes.OpenId);

                     var config = options.UseAspNetCore();
                     config.DisableTransportSecurityRequirement();
                     config.EnableTokenEndpointPassthrough()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableUserinfoEndpointPassthrough()
                           .EnableStatusCodePagesIntegration();
                 })
                .AddValidation(validation =>
                 {
                     validation.UseLocalServer();
                     validation.UseAspNetCore();
                 });
    }
}