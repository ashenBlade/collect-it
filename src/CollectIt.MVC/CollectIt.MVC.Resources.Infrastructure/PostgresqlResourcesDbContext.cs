using CollectIt.MVC.Resources.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.MVC.Resources.Infrastructure;

public class PostgresqlResourcesDbContext : DbContext
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Music> Musics { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Resource> Resources { get; set; }
    
    
    public PostgresqlResourcesDbContext(DbContextOptions<PostgresqlResourcesDbContext> options)
        : base(options)
    {}
}