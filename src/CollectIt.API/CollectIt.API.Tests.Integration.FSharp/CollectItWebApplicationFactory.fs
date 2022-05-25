module CollectIt.API.Tests.Integration.FSharp.CollectItWebApplicationFactory

open System
open System.Reflection
open CollectIt.API.Tests.Integration.FSharp.StubImageFileManager
open CollectIt.API.Tests.Integration.FSharp.StubMusicFileManager
open CollectIt.API.Tests.Integration.FSharp.StubVideoFileManager
open CollectIt.API.WebAPI
open CollectIt.Database.Infrastructure
open CollectIt.Database.Infrastructure.Resources.FileManagers
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc.Testing
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.DependencyInjection.Extensions

type CollectItWebApplicationFactory() =
    inherit WebApplicationFactory<Program>()

    override this.ConfigureWebHost(builder: IWebHostBuilder) =
        ``base``.ConfigureWebHost(builder)
        builder.UseEnvironment "Development" |> ignore

        builder.ConfigureAppConfiguration (fun config ->
            UserSecretsConfigurationExtensions.AddUserSecrets(config, Assembly.GetExecutingAssembly())
            |> ignore)
        |> ignore

        builder.ConfigureServices (fun ctx services ->
            services
            |> ServiceCollectionDescriptorExtensions.RemoveAll<DbContextOptions<PostgresqlCollectItDbContext>>
            |> ServiceCollectionDescriptorExtensions.RemoveAll<PostgresqlCollectItDbContext>
            |> (fun services ->
                EntityFrameworkServiceCollectionExtensions.AddDbContext<PostgresqlCollectItDbContext>(
                    services,
                    (fun config ->
                        let connectionString =
                            ctx.Configuration["ConnectionStrings:Postgresql:Testing"]

                        config.UseNpgsql(
                            connectionString,
                            (fun options ->
                                options.UseNodaTime() |> ignore

                                options.MigrationsAssembly("CollectIt.Database.Infrastructure")
                                |> ignore)
                        )
                        |> ignore

                        config.UseOpenIddict<Int32>() |> ignore
                        ())
                ))
            |> ServiceCollectionDescriptorExtensions.RemoveAll<IVideoFileManager>
            |> (fun services -> services.AddTransient<IVideoFileManager, StubVideoFileManager>())
            |> (fun services -> services.AddTransient<IMusicFileManager, StubMusicFileManager>())
            |> (fun services -> services.AddTransient<IImageFileManager, StubImageFileManager>())
            |> (fun services ->
                let sp = services.BuildServiceProvider()
                use scope = sp.CreateScope()

                let context =
                    scope.ServiceProvider.GetRequiredService<PostgresqlCollectItDbContext>()

                context.Database.EnsureDeleted() |> ignore
                context.Database.Migrate()
                services)
            |> ignore)
        |> ignore
