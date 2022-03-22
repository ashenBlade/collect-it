using CollectIt.MVC.Account.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.MVC.Account.Infrastructure.Data;

public class PostgresqlIdentityDbContext : IdentityDbContext<User, Role, int>
{
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserSubscription> UsersSubscriptions { get; set; }
    public DbSet<ActiveUserSubscription> ActiveUsersSubscriptions { get; set; }
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
        builder.Entity<ActiveUserSubscription>()
               .ToView("ActiveUsersSubscriptions")
               .HasNoKey();
        builder.Entity<Role>()
               .HasData(new Role() { Id = 1, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "DEFAULT_STAMP" },
                        new Role() { Id = 2, Name = "User", NormalizedName = "USER", ConcurrencyStamp = "DEFAULT_STAMP" },
                        new Role() { Id = 3, Name = "Technical Support", NormalizedName = "TECHNICAL SUPPORT", ConcurrencyStamp = "DEFAULT_STAMP" });
        builder.Entity<Subscription>()
               .HasData(new Subscription()
                        {
                               Id = 1,
                               Name = "Бронзовая",
                               AppliedResourceType = ResourceType.Image,
                               Description = "Обычная подписка",
                               MaxResourcesCount = 10,
                               Price = 100,
                               MonthDuration = 1
                        },
                        new Subscription()
                        {
                               Id = 2,
                               Name = "Серебрянная",
                               AppliedResourceType = ResourceType.Image,
                               Description = "Продвинутая подписка",
                               MaxResourcesCount = 50,
                               Price = 200,
                               MonthDuration = 3,
                        },
                        new Subscription()
                        {
                               Id = 3,
                               Name = "Золотая",
                               AppliedResourceType = ResourceType.Image,
                               Description = "Элитная подписка",
                               MaxResourcesCount = 100,
                               Price = 1000,
                               MonthDuration = 6
                        });
    }
}