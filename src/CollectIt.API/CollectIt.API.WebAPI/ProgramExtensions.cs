using CollectIt.Database.Infrastructure;

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
                           .AllowPasswordFlow()
                           .AllowRefreshTokenFlow();
                    options.SetTokenEndpointUris("/connect/token");
                    options.AddDevelopmentSigningCertificate()
                           .AddDevelopmentEncryptionCertificate();
                    
                    var config = options.UseAspNetCore();
                    if (environment.IsDevelopment() || environment.IsStaging())
                    {
                        config.DisableTransportSecurityRequirement();
                    }

                    config.EnableTokenEndpointPassthrough();

                });
       return services;
    }
}