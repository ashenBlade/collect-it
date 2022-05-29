using CollectIt.Database.Infrastructure;
using OpenIddict.Abstractions;

namespace CollectIt.API.WebAPI;

public static class ProgramExtensions
{
    public static void AddCollectItOpenIddict(this IServiceCollection services)
    {
        services.AddOpenIddict()
                .AddCore(options =>
                             options.UseEntityFrameworkCore()
                                    .UseDbContext<PostgresqlCollectItDbContext>()
                                    .ReplaceDefaultEntities<int>())
                .AddServer(options =>
                 {
                     options.SetAccessTokenLifetime(TimeSpan.FromMinutes(60));
                     options.AcceptAnonymousClients()
                            .AllowPasswordFlow();
                     options.AddDevelopmentSigningCertificate()
                            .AddDevelopmentEncryptionCertificate();
                     options.SetTokenEndpointUris("/connect/token");
                     options.RegisterScopes(OpenIddictConstants.Scopes.Email,
                                            OpenIddictConstants.Scopes.Profile,
                                            OpenIddictConstants.Scopes.Roles,
                                            OpenIddictConstants.Scopes.OpenId);

                     options.UseAspNetCore()
                            .DisableTransportSecurityRequirement()
                            .EnableTokenEndpointPassthrough()
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