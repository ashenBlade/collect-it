using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure;

public class PostgresqlCollectItDbContext : IdentityDbContext<User, Role, int>
{
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserSubscription> UsersSubscriptions { get; set; }
    public DbSet<ActiveUserSubscription> ActiveUsersSubscriptions { get; set; }
    
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Music> Musics { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Resource> Resources { get; set; }

    public PostgresqlCollectItDbContext(DbContextOptions<PostgresqlCollectItDbContext> options)
        : base(options)
    { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        OnModelCreatingAccounts(builder);
        OnModelCreatingResources(builder);
    }
    
    private static void OnModelCreatingAccounts(ModelBuilder builder)
    {
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
                            MaxResourcesCount = 50,
                            MonthDuration = 1,
                            Price = 200
                        },
                        new Subscription()
                        {
                            Id = 2,
                            Name = "Серебрянная",
                            AppliedResourceType = ResourceType.Image,
                            Description = "Подписка для любителей качать",
                            MaxResourcesCount = 100,
                            MonthDuration = 1,
                            Price = 350
                        },
                        new Subscription()
                        {
                            Id = 3,
                            Name = "Золотая",
                            AppliedResourceType = ResourceType.Image,
                            Description = "Не для пиратов",
                            MaxResourcesCount = 200,
                            MonthDuration = 1,
                            Price = 500
                        });
    }

    private void OnModelCreatingResources(ModelBuilder builder)
    { }
}