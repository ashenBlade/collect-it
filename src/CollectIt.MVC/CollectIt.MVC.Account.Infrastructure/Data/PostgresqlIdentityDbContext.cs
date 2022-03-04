using CollectIt.MVC.Account.IdentityEntities;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.MVC.Account.Infrastructure.Data;

public class PostgresqlIdentityDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserSubscription> UsersSubscriptions { get; set; }

    public PostgresqlIdentityDbContext(DbContextOptions<PostgresqlIdentityDbContext> options)
        : base(options)
    { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
               .HasMany(u => u.Subscriptions)
               .WithMany(s => s.Subscribers)
               .UsingEntity<UserSubscription>();
    }
}