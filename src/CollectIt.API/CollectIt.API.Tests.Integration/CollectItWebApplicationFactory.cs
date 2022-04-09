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
    // protected override IHost CreateHost(IHostBuilder builder)
    // {
    //     builder.ConfigureServices(services =>
    //     {
    //         var dbContextOptionsDescriptor =
    //             services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PostgresqlCollectItDbContext>));
    //         if (dbContextOptionsDescriptor is not null)
    //         {
    //             services.Remove(dbContextOptionsDescriptor);
    //         }
    //         
    //         // services.RemoveAll<PostgresqlCollectItDbContext>();
    //         services.AddDbContext<PostgresqlCollectItDbContext>(config =>
    //         {
    //             config.UseInMemoryDatabase("CollectItDB.Tests");
    //         });
    //     });
    //     return base.CreateHost(builder);
    // }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PostgresqlCollectItDbContext>>();
            services.RemoveAll<PostgresqlCollectItDbContext>();
            services.AddDbContext<PostgresqlCollectItDbContext>(config =>
            {
                config.UseNpgsql("Server=localhost;Port=5432;Database=collect_it_integration_tests;User Id=ashblade;Password=12345678", options =>
                {
                    options.UseNodaTime();
                });

            });
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<PostgresqlCollectItDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });
    }
}