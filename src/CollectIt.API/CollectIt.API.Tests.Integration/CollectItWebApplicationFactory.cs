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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CollectIt.API.Tests.Integration;

public class CollectItWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices((ctx, services) =>
        {
            services.RemoveAll<DbContextOptions<PostgresqlCollectItDbContext>>();
            services.RemoveAll<PostgresqlCollectItDbContext>();
            services.AddDbContext<PostgresqlCollectItDbContext>(config =>
            {
                config.UseNpgsql("Server=localhost;Database=collect_it_integration_tests;User Id=ashblade;Password=12345678;Port=5432", options =>
                {
                    options.UseNodaTime();
                    options.MigrationsAssembly("CollectIt.Database.Infrastructure");
                });

            });
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<PostgresqlCollectItDbContext>();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
            context.Database.EnsureCreated();
        });
    }
}