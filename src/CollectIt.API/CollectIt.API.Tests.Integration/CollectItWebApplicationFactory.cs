using System;
using System.Linq;
using System.Net.Http;
using CollectIt.API.WebAPI;
using CollectIt.Database.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace CollectIt.API.Tests.Integration;

public class CollectItWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Development");
        builder.ConfigureServices((ctx, services) =>
        {
            services.RemoveAll<DbContextOptions<PostgresqlCollectItDbContext>>();
            services.RemoveAll<PostgresqlCollectItDbContext>();
            services.AddDbContext<PostgresqlCollectItDbContext>(config =>
            {
                // I don't know how to provide connection string in user-secrets
                // So, put it directly here
                // Or find out how to do it 
                config.UseNpgsql("Server=localhost;Database=collect_it_integration_tests;User Id=ashblade;Password=12345678;Port=5432", options =>
                {
                    options.UseNodaTime();
                    options.MigrationsAssembly("CollectIt.Database.Infrastructure");
                });
                config.UseOpenIddict<int>();
            });
            
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<PostgresqlCollectItDbContext>();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
            context.Add(new OpenIddictEntityFrameworkCoreToken<int>()
                        {
                            Id = 1,
                            Subject = "2",
                            ConcurrencyToken = "b6f28987-35c6-4645-9289-3644ffde17ac",
                            CreationDate = new DateTime(2022, 4, 13, 7, 28, 26, DateTimeKind.Utc),
                            ExpirationDate = new DateTime(2023, 4, 13, 7, 28, 26, DateTimeKind.Utc),
                            Status = "valid",
                            Type = "access_token",
                        });
            // context.Database.EnsureCreated();
        });
    }
}