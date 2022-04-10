using System;
using System.Threading.Tasks;
using CollectIt.Database.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.API.Tests.Integration;

public class MoqPostgresqlCollectItDbContext : PostgresqlCollectItDbContext, IAsyncDisposable
{
    public MoqPostgresqlCollectItDbContext(DbContextOptions<PostgresqlCollectItDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public override async ValueTask DisposeAsync()
    {
        await Database.EnsureDeletedAsync();
        await base.DisposeAsync();
    }
}