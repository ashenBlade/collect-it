using CollectIt.MVC.Account.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.MVC.Account.Infrastructure.Data;

public class PostgresqlIdentityDbContext : IdentityDbContext<User, Role, int>
{
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserSubscription> UsersSubscriptions { get; set; }
    public PostgresqlIdentityDbContext(DbContextOptions<PostgresqlIdentityDbContext> options)
        : base(options)
    { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>()
               .HasMany(u => u.Subscriptions)
               .WithMany(s => s.Subscribers)
               .UsingEntity<UserSubscription>();
        builder.Entity<Subscription>()
               .Property(s => s.AppliedResourceType)
               .HasConversion<string>();
    }
}