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

    public DbSet<AcquiredUserResource> AcquiredUserResources { get; set; }
    
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
               .ToView("ActiveUsersSubscriptions");
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
               .HasIndex(s => s.Active);
        
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
                            RestrictionId = null,
                            Active = true
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
                            RestrictionId = null,
                            Active = true
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
                            RestrictionId = null,
                            Active = true
                        });
        
        builder.Entity<User>()
               .HasData(GetDefaultUser());
    }

    private void OnModelCreatingResources(ModelBuilder builder)
    {
        builder.Entity<Resource>()
               .HasMany(x => x.AcquiredBy)
               .WithMany(u => u.AcquiredResources)
               .UsingEntity<AcquiredUserResource>();
        builder.Entity<Resource>()
               .HasOne(r => r.Owner)
               .WithMany(u => u.ResourcesAuthorOf);
        builder.Entity<AcquiredUserResource>()
               .HasAlternateKey(aur => new {aur.UserId, aur.ResourceId});
        builder.Entity<AcquiredUserResource>()
               .HasIndex(aur => new {aur.UserId});
        builder.Entity<Resource>()
               .HasGeneratedTsVectorColumn(r => r.NameSearchVector,
                                           "russian",
                                           r => new { Name = r.Name })
               .HasIndex(r => r.NameSearchVector)
               .HasMethod("GIN");

        var ownerId = GetDefaultUser().Id;
        builder.Entity<Image>()
               .HasData(new Image
                        {
                            Id = 1,
                            Address = "/imagesFromDb/abstract-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Мониторы с аниме",
                            Extension = "jpg",
                            FileName = "abstract-img.jpg",
                            Tags = new []{"anime","fallout"}
                        },
                        new Image
                        {
                            Id = 2,
                            Address = "/imagesFromDb/bird-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Птица зимородок",
                            Extension = "jpg",
                            FileName = "bird-img.jpg",
                            Tags = new []{"bird","nature"}
                        },
                        new Image
                        {
                            Id = 3,
                            Address = "/imagesFromDb/car-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Машина на дороге",
                            Extension = "jpg",
                            FileName = "car-img.jpg",
                            Tags = new []{"car"}
                        },
                        new Image
                        {
                            Id = 4,
                            Address = "/imagesFromDb/cat-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Котенок на одеяле",
                            Extension = "jpg",
                            FileName = "cat-img.jpg",
                            Tags = new []{"cat","animal","pet"}
                        },
                        new Image
                        {
                            Id = 5,
                            Address = "/imagesFromDb/house-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Стандартный американский дом",
                            Extension = "jpg",
                            FileName = "house-img.jpg",
                            Tags = new []{"house"}
                        },
                        new Image
                        {
                            Id = 6,
                            Address = "/imagesFromDb/nature-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Осенний лес в природе",
                            Extension = "jpg",
                            FileName = "nature-img.jpg",
                            Tags = new []{"nature"}
                        },
                        new Image
                        {
                            Id = 7,
                            Address = "/imagesFromDb/school-img.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Дети за партами в школе перед учителем",
                            Extension = "jpg",
                            FileName = "school-img.jpg",
                            Tags = new []{"school","kids"}
                        },
                        new Image
                        {
                            Id = 8,
                            Address = "/imagesFromDb/cat-img-2.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Кот смотрит в камеру на зеленом фоне",
                            Extension = "jpg",
                            FileName = "cat-img-2.jpg",
                            Tags = new []{"cat","pet","animal"}
                        },
                        new Image
                        {
                            Id = 9,
                            Address = "/imagesFromDb/cat-img-3.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Крутой кот в очках",
                            Extension = "jpg",
                            FileName = "cat-img-3.jpg",
                            Tags = new []{"cat","pet","animal","sunglasses"}
                        },
                        new Image
                        {
                            Id = 10,
                            Address = "/imagesFromDb/cat-img-4.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Белоснежный кот застыл в мяукающей позе",
                            Extension = "jpg",
                            FileName = "cat-img-4.jpg",
                            Tags = new []{"cat","pet","animal"}
                        },
                        new Image
                        {
                            Id = 11,
                            Address = "/imagesFromDb/cat-img-5.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Рыжий кот заснул на полу",
                            Extension = "jpg",
                            FileName = "cat-img-5.jpg",
                            Tags = new []{"cat","pet","animal"}
                        },
                        new Image
                        {
                            Id = 12,
                            Address = "/imagesFromDb/cat-img-6.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Спящий кот прикрывается лапой от солнца",
                            Extension = "jpg",
                            FileName = "cat-img-6.jpg",
                            Tags = new []{"cat","pet","animal"}
                        },
                        new Image
                        {
                            Id = 13,
                            Address = "/imagesFromDb/cat-img-7.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "На стуле лежит кот",
                            Extension = "jpg",
                            FileName = "cat-img-7.jpg",
                            Tags = new []{"cat","pet","animal","chair","furniture"}
                        },
                        new Image
                        {
                            Id = 14,
                            Address = "/imagesFromDb/cat-img-8.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Идущий по забору кот у причала",
                            Extension = "jpg",
                            FileName = "cat-img-8.jpg",
                            Tags = new []{"cat","pet","animal","yacht","see"}
                        }, 
                        new Image
                        {
                            Id = 15,
                            Address = "/imagesFromDb/cat-img-9.jpg",
                            OwnerId = ownerId,
                            UploadDate = new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                            Name = "Кот у елки сморит на лес",
                            Extension = "jpg",
                            FileName = "cat-img-9.jpg",
                            Tags = new []{"cat","pet","animal","nature"}
                        });
    }
}