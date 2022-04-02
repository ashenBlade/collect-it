using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Account.Restrictions;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;

namespace CollectIt.Database.Infrastructure;

public class PostgresqlCollectItDbContext : IdentityDbContext<User, Role, int>
{
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserSubscription> UsersSubscriptions { get; set; }
    public DbSet<ActiveUserSubscription> ActiveUsersSubscriptions { get; set; }
    
    public DbSet<AuthorRestriction> AuthorRestrictions { get; set; }
    public DbSet<DaysToRestriction> DateToRestrictions { get; set; }
    public DbSet<DaysAfterRestriction> DateFromRestrictions { get; set; }
    public DbSet<TagRestriction> TagRestrictions { get; set; }
    public DbSet<SizeRestriction> SizeRestrictions { get; set; }
    
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

    private static User GetDefaultUser()
    {
        return new User()
               {
                   Id = 1,
                   Email = "asdf@mail.ru",
                   NormalizedEmail = "ASDF@MAIL.RU",
                   UserName = "BestPhotoshoper",
                   NormalizedUserName = "BestPhotoshoper",
                   EmailConfirmed = false,
                   PhoneNumberConfirmed = false,
                   PasswordHash = "AQAAAAEAACcQAAAAEAO/K1C4Jn77AXrULgaNn6rkHlrkXbk9jOqHqe+HK+CvDgmBEEFahFadKE8H7x4Olw==",
                   SecurityStamp = "MSCN3JBQERUJBPLR4XIXZH3TQGICF6O3",
                   ConcurrencyStamp = "3e0213e9-8d80-48df-b9df-18fc7debd84e",
                   TwoFactorEnabled = false,
                   LockoutEnabled = false,
                   AccessFailedCount = 0
               };
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
        builder.Entity<Restriction>()
               .HasDiscriminator<RestrictionType>("RestrictionType")
               .HasValue<AuthorRestriction>(RestrictionType.Author)
               .HasValue<DaysToRestriction>(RestrictionType.DaysTo)
               .HasValue<DaysAfterRestriction>(RestrictionType.DaysAfter)
               .HasValue<TagRestriction>(RestrictionType.Tags)
               .HasValue<SizeRestriction>(RestrictionType.Size);
        
        builder.Entity<Subscription>()
               .HasOne(s => s.Restriction)
               .WithOne(r => r.Subscription)
               .IsRequired(false);
        
        builder.Entity<Subscription>()
               .HasData(new Subscription()
                        {
                            Id = 1,
                            Name = "Бронзовая",
                            AppliedResourceType = ResourceType.Image,
                            Description = "Обычная подписка",
                            MaxResourcesCount = 50,
                            MonthDuration = 1,
                            Price = 200,
                            RestrictionId = null
                        },
                        new Subscription()
                        {
                            Id = 2,
                            Name = "Серебрянная",
                            AppliedResourceType = ResourceType.Image,
                            Description = "Подписка для любителей качать",
                            MaxResourcesCount = 100,
                            MonthDuration = 1,
                            Price = 350,
                            RestrictionId = null
                        },
                        new Subscription()
                        {
                            Id = 3,
                            Name = "Золотая",
                            AppliedResourceType = ResourceType.Image,
                            Description = "Не для пиратов",
                            MaxResourcesCount = 200,
                            MonthDuration = 1,
                            Price = 500,
                            RestrictionId = null
                        });
        
        builder.Entity<User>()
               .HasData(GetDefaultUser());
    }

    private void OnModelCreatingResources(ModelBuilder builder)
    {
        builder.Entity<Resource>()
               .HasGeneratedTsVectorColumn(r => r.NameSearchVector,
                                           "russian",
                                           r => new { r.Name })
               .HasIndex(r => r.NameSearchVector)
               .HasMethod("GIN");

        var ownerId = GetDefaultUser().Id;
        builder.Entity<Image>()
               .HasData(new Image
                        {
                            Id = 1,
                            Path = "/imagesFromDb/abstract-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Мониторы с аниме"
                        },
                        new Image
                        {
                            Id = 2,
                            Path = "/imagesFromDb/bird-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Птица зимородок"
                        },
                        new Image
                        {
                            Id = 3,
                            Path = "/imagesFromDb/car-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Машина на дороге"
                        },
                        new Image
                        {
                            Id = 4,
                            Path = "/imagesFromDb/cat-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Котенок на одеяле"
                        },
                        new Image
                        {
                            Id = 5,
                            Path = "/imagesFromDb/house-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Стандартный американский дом"
                        },
                        new Image
                        {
                            Id = 6,
                            Path = "/imagesFromDb/nature-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Осенний лес в природе"
                        },
                        new Image
                        {
                            Id = 7,
                            Path = "/imagesFromDb/school-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Дети за партами в школе перед учителем"
                        },
                        new Image
                        {
                            Id = 8,
                            Path = "/imagesFromDb/cat-img-2.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Кот смотрит в камеру на зеленом фоне"
                        },
                        new Image
                        {
                            Id = 9,
                            Path = "/imagesFromDb/cat-img-3.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Крутой кот в очках"
                        },
                        new Image
                        {
                            Id = 10,
                            Path = "/imagesFromDb/cat-img-4.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Белоснежный кот застыл в мяукающей позе"
                        },
                        new Image
                        {
                            Id = 11,
                            Path = "/imagesFromDb/cat-img-5.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Рыжий кот заснул на полу"
                        },
                        new Image
                        {
                            Id = 12,
                            Path = "/imagesFromDb/cat-img-6.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Спящий кот прикрывается лапой от солнца"
                        },
                        new Image
                        {
                            Id = 13,
                            Path = "/imagesFromDb/cat-img-7.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "На стуле лежит кот"
                        },
                        new Image
                        {
                            Id = 14,
                            Path = "/imagesFromDb/cat-img-8.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Идущий по забору кот у причала"
                        }, 
                        new Image
                        {
                            Id = 15,
                            Path = "/imagesFromDb/cat-img-9.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Кот у елки сморит на лес"
                        });
    }
}