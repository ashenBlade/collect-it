using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using CollectIt.API.WebAPI;
using CollectIt.Database.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http.Logging;
using Microsoft.Extensions.Logging;
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
        builder.ConfigureLogging(logging =>
        {
            logging.AddSimpleConsole(opts =>
            {
                opts.SingleLine = false;
            });
        });
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddUserSecrets(Assembly.GetExecutingAssembly());
        });
        builder.ConfigureServices((ctx, services) =>
        {
            services.RemoveAll<DbContextOptions<PostgresqlCollectItDbContext>>();
            services.RemoveAll<PostgresqlCollectItDbContext>();
            services.AddDbContext<PostgresqlCollectItDbContext>(config =>
            {
                // I don't know how to provide connection string in user-secrets
                // So, put it directly here
                // Or find out how to do it 
                var connectionString = ctx.Configuration["ConnectionStrings:Postgresql:Testing"];
                if (connectionString is null)
                {
                    throw new ArgumentNullException(nameof(connectionString), "Connection string is still null");
                }
                config.UseNpgsql(connectionString, options =>
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
            // context.Database.EnsureCreated();
        });
    }
}