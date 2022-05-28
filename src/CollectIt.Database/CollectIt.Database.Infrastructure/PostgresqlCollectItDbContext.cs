using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Account.Restrictions;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using OpenIddict.EntityFrameworkCore.Models;

namespace CollectIt.Database.Infrastructure;

public class PostgresqlCollectItDbContext : IdentityDbContext<User, Role, int>
{
    internal static string AdminAccessTokenBearer =
        "eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIiLCJraWQiOiJEN0E1NDZFNzFDRDA0MkI3MzkyMzVCOTU0MjBDOUQ1OEFGRDE4QUZCIiwidHlwIjoiYXQrand0In0.kiLie1BHwiT2-VFZd8cwoObiTDfHOUDUVuKJ9SkiyxQLi99Xc18kEo1a3xKf3Syow0ehLw9kBm7YN818bg7xaPm5vQZTOsX9ErNxan77M5chBC6cYJZxjmH8R82U2UNNQkuV6AP-g45xCvZ7bzkVRj2EzdJ9SKktxgY0gzAxlGZZvCukrl-9H-89N2GXfY_7pmwsyegi64eVOdoJj9K-cb7pdp6zCE_c9eYNgCuRySt0YjfE86e09PCF9HE1lPbxkI481Sb1gm6Gtg9WmgQIyKWd0LzZLnlGDHs_7gMZuAt2VAr-tcSl840qhZ8gJZFQ-yVMzzA6Xei2IlM_mqoXKw.sLnJoZ2PvwPIuy9iD4tPmA.0xz8a3c2vb-A5-bEwTHHoiTew_MEEa7LXQV55c8QJmpZxEejQ9PtMiuoBoaFUg9n8uPf6LD2YiKV4NFu7cGcN7LWjiYg0Nf-DyAE7UPEz8dfW000QmGr4thuNXDfrc9Mk9iaEDxJfzaOczS1ftLsfwX-05WDkbo9AW6oDUL7oC9fbWxZUYLL1xMmwunAf3sBjlZLwSHRfRD_CshjGYLC0zUsEhDnfC7MzrzYXinyj8I9GffyBZC4gu6j6-D0LP8CWyhi4Ua_wpWfJchpyN3wgTo5ZlO9AZDIT0HrrmthTjAimcgpbxIX5V036Cq7qn60RThbbcZffMiq_Qqfn11AFut6TvZttthFy1Bju9UNaxC3hZsKQvoSPWUFTlS237xrUIvevl0ft1mrrRgJMgcy51XokKRZBRrbD9FLWRNuYhWJ0OeCQ96F8CBhVeVdP-fG_ZmSHUY3gZf1F64kn_Y_bHvUzn8kS_bvYpdqYS2zTc_4sGzfW1abs373MLob_Cq97f-XhQyNzyIyFIY-aJXG0tUJ8vew37C6-rgph3RRaVVUPSl8w6v8PdVReAyFx6CXrLhubOUhYsc2WEvvFTrOsCK8c59DkQqFzKgDuz5k2BGltiDM-MifByvz6LSB1OZfuvNAC0-DSw1-U-EMmGz3fTdAYQl6boTW5I0yLpMZ4juRLST0ZAKjDkaDFOu8A4hGoLbJJYS1ETPo24RGAwTHN7N3jhKmGOFmTHZW1JgIMTBKa16opVg9DaK6Gbx1XmBZwiuhe2FA1t-bgSamKM5OwgEpblZQrpO3LGiqCsa44N6KTWu2x4W7bX3m_OjaUcVpVDGL7-AXohls_EiHlkIAmWmyllAXJo6Ep69m-rAMMHIBAGU2DusS9GmUyheu8j5snmLKEicyASc3ntN9yRemKr8EYzPq-Yj27XHt6NAClCWDV8ZKn64CdWNilEy2jzvp.wsd0CMpvV7hxnyY4MPrM1MhWLYaXiyqhPE9ZVbMh3pE";

    internal static Image DefaultImage1 =
        new()
        {
            Id = 1,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Мониторы с аниме",
            Extension = "jpg",
            FileName = "abstract-img.jpg",
            Tags = new[] {"аниме", "фоллаут"}
        };

    internal static Image DefaultImage2 =
        new()
        {
            Id = 2,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Птица зимородок",
            Extension = "jpg",
            FileName = "bird-img.jpg",
            Tags = new[] {"птица", "природа"}
        };

    internal static Image DefaultImage3 =
        new()
        {
            Id = 3,
            OwnerId = TechSupportUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Машина на дороге",
            Extension = "jpg",
            FileName = "car-img.jpg",
            Tags = new[] {"машина"}
        };

    internal static Image DefaultImage4 =
        new()
        {
            Id = 4,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Котенок на одеяле",
            Extension = "jpg",
            FileName = "cat-img.jpg",
            Tags = new[] {"кот", "животное", "питомец"}
        };

    internal static Image DefaultImage5 =
        new()
        {
            Id = 5,
            OwnerId = TechSupportUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Стандартный американский дом",
            Extension = "jpg",
            FileName = "house-img.jpg",
            Tags = new[] {"дом"}
        };

    internal static Image DefaultImage6 =
        new()
        {
            Id = 6,
            OwnerId = DefaultUserTwoId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Осенний лес в природе",
            Extension = "jpg",
            FileName = "nature-img.jpg",
            Tags = new[] {"природа"}
        };

    internal static Image DefaultImage7 =
        new()
        {
            Id = 7,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Дети за партами в школе перед учителем",
            Extension = "jpg",
            FileName = "school-img.jpg",
            Tags = new[] {"школа", "дети"}
        };

    internal static Image DefaultImage8 =
        new()
        {
            Id = 8,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Кот смотрит в камеру на зеленом фоне",
            Extension = "jpg",
            FileName = "cat-img-2.jpg",
            Tags = new[] {"кот", "питомец", "животное"}
        };

    internal static Image DefaultImage9 =
        new()
        {
            Id = 9,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Крутой кот в очках",
            Extension = "jpg",
            FileName = "cat-img-3.jpg",
            Tags = new[] {"кот", "питомец", "животное", "очки"}
        };

    internal static Image DefaultImage10 =
        new()
        {
            Id = 10,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Белоснежный кот застыл в мяукающей позе",
            Extension = "jpg",
            FileName = "cat-img-4.jpg",
            Tags = new[] {"кот", "питомец", "животное"}
        };

    internal static Image DefaultImage11 =
        new()
        {
            Id = 11,
            OwnerId = DefaultUserTwoId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Рыжий кот заснул на полу",
            Extension = "jpg",
            FileName = "cat-img-5.jpg",
            Tags = new[] {"кот", "питомец", "животное"}
        };


    internal static Image DefaultImage12 =
        new()
        {
            Id = 12,
            OwnerId = DefaultUserOneId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Спящий кот прикрывается лапой от солнца",
            Extension = "jpg",
            FileName = "cat-img-6.jpg",
            Tags = new[] {"кот", "питомец", "животное"}
        };

    internal static Image DefaultImage13 =
        new()
        {
            Id = 13,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "На стуле лежит кот",
            Extension = "jpg",
            FileName = "cat-img-7.jpg",
            Tags = new[] {"кот", "питомец", "животное", "стул", "мебель"}
        };

    internal static Image DefaultImage14 =
        new()
        {
            Id = 14,
            OwnerId = AdminUserId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Идущий по забору кот у причала",
            Extension = "jpg",
            FileName = "cat-img-8.jpg",
            Tags = new[] {"кот", "питомец", "животное", "яхта", "море"}
        };

    internal static Image DefaultImage15 =
        new()
        {
            Id = 15,
            OwnerId = DefaultUserOneId,
            UploadDate =
                new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
            Name = "Кот у елки сморит на лес",
            Extension = "jpg",
            FileName = "cat-img-9.jpg",
            Tags = new[] {"кот", "питомец", "животное", "природа"}
        };

    public PostgresqlCollectItDbContext(DbContextOptions<PostgresqlCollectItDbContext> options)
        : base(options)
    {
    }

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

    internal static Role Admin =>
        new() {Id = 1, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "DEFAULT_STAMP"};

    internal static Role User =>
        new() {Id = 2, Name = "User", NormalizedName = "USER", ConcurrencyStamp = "DEFAULT_STAMP"};

    internal static Role TechSupport =>
        new()
        {
            Id = 3, Name = "Technical Support", NormalizedName = "TECHNICAL SUPPORT", ConcurrencyStamp = "DEFAULT_STAMP"
        };

    internal static Subscription BronzeSubscription =>
        new()
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
        };

    internal static Subscription SilverSubscription =>
        new()
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
        };

    internal static Subscription GoldenSubscription =>
        new()
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
        };

    internal static Subscription DisabledSubscription =>
        new()
        {
            Id = 4,
            Name = "Отключенная подписка",
            AppliedResourceType = ResourceType.Image,
            Description = "Этот тип подписки не должен быть показан, так как его специально отключили",
            MaxResourcesCount = 1,
            MonthDuration = 1,
            Price = 1,
            RestrictionId = null,
            Active = false
        };

    internal static Subscription AllowAllSubscription =>
        new()
        {
            Id = 5,
            Name = "Кардбланш",
            AppliedResourceType = ResourceType.Any,
            Description = "Этот тип подписки только для привилегированных. Скачивай что хочешь.",
            MaxResourcesCount = int.MaxValue,
            MonthDuration = 120000, // Max month for datetime to add
            Price = int.MaxValue,
            RestrictionId = null,
            Active = false
        };

    internal static User[] DefaultUsers =>
        new[] {AdminUser, TechSupportUser, DefaultUserOne, DefaultUserTwo,};

    internal static int DefaultUserTwoId => 2;

    internal static User DefaultUserTwo =>
        new()
        {
            Id = DefaultUserTwoId,
            Email = "mail@mail.ru",
            NormalizedEmail = "MAIL@MAIL.RU",
            UserName = "Discriminator",
            NormalizedUserName = "DISCRIMINATOR",
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            PasswordHash =
                "AQAAAAEAACcQAAAAEENZCDY7KW1yCVxiLaIjILavAHSPVWMvTeb0YjDdOK74mqCBqby19ul9VfFQk6Il9A==",
            SecurityStamp = "TX26HJDK44UKB7FQTM3WSW7A5K4PRRS6",
            ConcurrencyStamp = "31ab9dd7-d86c-4640-aa97-22ff38176d94",
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };

    internal static int DefaultUserOneId => 3;

    internal static User DefaultUserOne =>
        new()
        {
            Id = DefaultUserOneId,
            Email = "andrey1999@yandex.ru",
            NormalizedEmail = "ANDREY1999@YANDEX.RU",
            UserName = "AndreyPhoto",
            NormalizedUserName = "ANDREYPHOTO",
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            PasswordHash =
                "AQAAAAEAACcQAAAAEDFG3rJjU9RopPeh1w+EePG21c/o6h2ng8hgRiQactvUbYOKSeLjxL/HAhJfDsuO0A==",
            SecurityStamp = "AG44W4JZWJVREA7HQRCKUFDSNZDYKCAW",
            ConcurrencyStamp = "f1a6e983-61f0-4fe3-b201-e8131080d312",
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };

    internal static int TechSupportUserId => 4;

    internal static User TechSupportUser =>
        new()
        {
            Id = TechSupportUserId,
            Email = "user@mail.ru",
            NormalizedEmail = "USER@MAIL.RU",
            UserName = "NineOneOne",
            NormalizedUserName = "NINEONEONE",
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            PasswordHash =
                "AQAAAAEAACcQAAAAEO63OCfJlqJdesMS4+ORyynU0r6Y/3x8u0j9ZQsd52y6ELqZG0f1X/WN49PV2NQWkA==",
            SecurityStamp = "A7NZSQXBUSPXKD4PTF5DPC3LTROWH2PH",
            ConcurrencyStamp = "fac5fa96-0453-4eaf-bebb-bc7ad73299d2",
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };

    internal static int AdminUserId => 1;

    internal static User AdminUser =>
        new()
        {
            Id = AdminUserId,
            Email = "asdf@mail.ru",
            NormalizedEmail = "ASDF@MAIL.RU",
            UserName = "BestPhotoshoper",
            NormalizedUserName = "BESTPHOTOSHOPER",
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            PasswordHash = "AQAAAAEAACcQAAAAEAO/K1C4Jn77AXrULgaNn6rkHlrkXbk9jOqHqe+HK+CvDgmBEEFahFadKE8H7x4Olw==",
            SecurityStamp = "MSCN3JBQERUJBPLR4XIXZH3TQGICF6O3",
            ConcurrencyStamp = "3e0213e9-8d80-48df-b9df-18fc7debd84e",
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };

    internal static Video DefaultVideo1 => new()
                                           {
                                               Id = 20,
                                               OwnerId = AdminUserId,
                                               UploadDate =
                                                   new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                               Name = "Диско лицо",
                                               Extension = "webm",
                                               FileName = "diman.webm",
                                               Tags = new[] {"Брекоткин", "диско лицо", "диско"},
                                               Duration = 60
                                           };

    internal static Video DefaultVideo2 => new()
                                           {
                                               Id = 21,
                                               OwnerId = AdminUserId,
                                               UploadDate =
                                                   new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                               Name =
                                                   "Сильный монолог на фоне церковных песнопений и красивой картинки",
                                               Extension = "webm",
                                               FileName = "strong_monolog.webm",
                                               Tags = new[] {"аниме", "церковь", "2д", "монолог"},
                                               Duration = 60
                                           };

    internal static Video DefaultVideo3 => new()
                                           {
                                               Id = 22,
                                               OwnerId = AdminUserId,
                                               UploadDate =
                                                   new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                               Name =
                                                   "Какое-то видео",
                                               Extension = "mp4",
                                               FileName = "polish-cow.mp4",
                                               Tags = new[] {"видео", "просто", "что"},
                                               Duration = 60
                                           };

    internal static Video[] DefaultVideos => new[] {DefaultVideo1, DefaultVideo2, DefaultVideo3};

    internal static Music DefaultMusic1 => new()
                                           {
                                               Id = 16,
                                               OwnerId = AdminUserId,
                                               UploadDate =
                                                   new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                               Name = "Тектоник - Басы",
                                               Extension = "mp3",
                                               FileName = "тектоник-басы.mp3",
                                               Tags = new[] {"качает", "2007"},
                                               Duration = 69
                                           };

    internal static Music DefaultMusic2 => new()
                                           {
                                               Id = 17,
                                               OwnerId = AdminUserId,
                                               UploadDate =
                                                   new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                               Name =
                                                   "OG BUDA, MORGENSHTERN, Mayot, blago white, SODA LUV - Cristal & МОЁТ (Remix)",
                                               Extension = "mp3",
                                               FileName = "MORGENSHTERN_JESTKO_VALIT.mp3",
                                               Tags = new[] {"качает", "морген", "сода лув", "ог буда"},
                                               Duration = 219
                                           };

    internal static Music DefaultMusic3 => new()
                                           {
                                               Id = 18,
                                               OwnerId = AdminUserId,
                                               UploadDate =
                                                   new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                               Name = "OST Naruto shippuden Ikimono-gakari - Blue Bird OP3",
                                               Extension = "mp3",
                                               FileName = "naruto_bluebird.mp3",
                                               Tags = new[] {"аниме", "наруто", "афган"},
                                               Duration = 218
                                           };

    internal static Music DefaultMusic4 => new()
                                           {
                                               Id = 19,
                                               OwnerId = AdminUserId,
                                               UploadDate =
                                                   new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                               Name = "минин - Зелёный глаз",
                                               Extension = "mp3",
                                               FileName = "minin_zeleniy_glaz.mp3",
                                               Tags = new[] {"грусть", "тикток", "рэп про тёлку"},
                                               Duration = 114
                                           };

    internal static Music[] DefaultMusics => new[] {DefaultMusic1, DefaultMusic2, DefaultMusic3, DefaultMusic4,};

    internal static Image[] DefaultImages => new[]
                                             {
                                                 DefaultImage1, DefaultImage2, DefaultImage3, DefaultImage4,
                                                 DefaultImage5, DefaultImage6, DefaultImage7, DefaultImage8,
                                                 DefaultImage9, DefaultImage10, DefaultImage11, DefaultImage12,
                                                 DefaultImage13, DefaultImage14, DefaultImage15
                                             };

    internal static UserSubscription DefaultUserOneGoldenSubscriptionUserSubscription =>
        new()
        {
            Id = 5,
            SubscriptionId = GoldenSubscription.Id,
            UserId = DefaultUserOneId,
            During = new DateInterval(new LocalDate(2022, 5, 1), new LocalDate(2022, 7, 1)),
            LeftResourcesCount = GoldenSubscription.MaxResourcesCount
        };

    internal static UserSubscription AdminAllowAllSubscriptionUserSubscription =>
        new UserSubscription()
        {
            Id = 1,
            SubscriptionId = AllowAllSubscription.Id,
            UserId = AdminUser.Id,
            During = new DateInterval(new LocalDate(2000, 1, 1), LocalDate.MaxIsoValue),
            LeftResourcesCount = AllowAllSubscription.MaxResourcesCount,
        };

    internal static UserSubscription DefaultUserOneSilverSubscriptionUserSubscription =>
        new()
        {
            Id = 2,
            SubscriptionId = SilverSubscription.Id,
            UserId = DefaultUserOne.Id,
            During = new DateInterval(new LocalDate(2021, 3, 1), new LocalDate(2021, 5, 9)),
            LeftResourcesCount = 0
        };

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
               .ToView("ActiveUsersSubscriptions");
        builder.Entity<Role>()
               .HasData(Admin, User, TechSupport);
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
               .HasData(BronzeSubscription,
                        SilverSubscription,
                        GoldenSubscription,
                        AllowAllSubscription);
        builder.Entity<TagRestriction>()
               .HasData(new TagRestriction() {Id = 1, Tags = new[] {"видео"}});
        builder.Entity<AuthorRestriction>()
               .HasData(new AuthorRestriction() {Id = 2, AuthorId = 1});
        builder.Entity<Subscription>()
               .HasData(new Subscription()
                        {
                            Id = 6,
                            Active = true,
                            Description = "Только видео с тегом \"видео\"",
                            Name = "Хочю видео",
                            Price = 600,
                            RestrictionId = 1,
                            AppliedResourceType = ResourceType.Video,
                            MonthDuration = 10,
                            MaxResourcesCount = 100
                        },
                        new Subscription()
                        {
                            Id = 7,
                            Active = true,
                            Description = "Только музыка Админа \"BestPhotoshoper\"",
                            Name = "Люблю админа",
                            Price = 228,
                            RestrictionId = 2,
                            MonthDuration = 3,
                            AppliedResourceType = ResourceType.Music,
                            MaxResourcesCount = 1000
                        });
        builder.Entity<User>()
               .HasMany(u => u.Roles)
               .WithMany(r => r.Users)
               .UsingEntity<IdentityUserRole<int>>();

        builder.Entity<User>()
               .HasData(DefaultUsers);

        builder.Entity<IdentityUserRole<int>>()
               .HasData(new IdentityUserRole<int>() {RoleId = Admin.Id, UserId = AdminUserId},
                        new IdentityUserRole<int>() {RoleId = TechSupport.Id, UserId = TechSupportUserId});
        builder.Entity<OpenIddictEntityFrameworkCoreToken<int>>()
               .HasData(new OpenIddictEntityFrameworkCoreToken<int>()
                        {
                            Id = 1,
                            ConcurrencyToken = "05fa1fe4-a237-4abc-a242-fa56c18c08ee",
                            CreationDate = new DateTime(2022, 4, 13, 14, 13, 30, DateTimeKind.Utc),
                            ExpirationDate = new DateTime(2025, 4, 12, 14, 13, 30, DateTimeKind.Utc),
                            Status = "valid",
                            Subject = "1",
                            Type = "access_token"
                        });
        builder.Entity<UserSubscription>()
               .HasData(AdminAllowAllSubscriptionUserSubscription,
                        DefaultUserOneSilverSubscriptionUserSubscription,
                        new UserSubscription()
                        {
                            Id = 3,
                            SubscriptionId = GoldenSubscription.Id,
                            UserId = DefaultUserOne.Id,
                            During = new DateInterval(new LocalDate(2021, 5, 10), new LocalDate(2022, 1, 10)),
                            LeftResourcesCount = 2
                        },
                        new UserSubscription()
                        {
                            Id = 4,
                            SubscriptionId = BronzeSubscription.Id,
                            UserId = DefaultUserOne.Id,
                            During = new DateInterval(new LocalDate(2022, 2, 20), new LocalDate(2022, 5, 20)),
                            LeftResourcesCount = BronzeSubscription.MaxResourcesCount
                        },
                        DefaultUserOneGoldenSubscriptionUserSubscription);
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
        // builder.Entity<Resource>()
        //        .Property(r => r.NameSearchVector)
        //        .IsGeneratedTsVectorColumn("russian", "Name");
        // builder.Entity<Resource>()
        //        .Property(r => r.TagsSearchVector)
        //        .IsGeneratedTsVectorColumn("russian", "Tags", "Name");
        var aurId = 1;
        builder.Entity<AcquiredUserResource>()
               .HasData(DefaultVideos.Select(v => new AcquiredUserResource()
                                                  {
                                                      Id = aurId++,
                                                      UserId = AdminUserId,
                                                      AcquiredDate =
                                                          new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                                      ResourceId = v.Id
                                                  }));
        builder.Entity<AcquiredUserResource>()
               .HasData(DefaultMusics.Select(m => new AcquiredUserResource()
                                                  {
                                                      Id = aurId++,
                                                      UserId = AdminUserId,
                                                      AcquiredDate =
                                                          new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                                      ResourceId = m.Id
                                                  }));
        builder.Entity<AcquiredUserResource>()
               .HasData(DefaultImages.Select(i => new AcquiredUserResource()
                                                  {
                                                      Id = aurId++,
                                                      UserId = AdminUserId,
                                                      AcquiredDate =
                                                          new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc),
                                                      ResourceId = i.Id
                                                  }));
        builder.Entity<AcquiredUserResource>()
               .HasAlternateKey(aur => new {aur.UserId, aur.ResourceId});
        builder.Entity<AcquiredUserResource>()
               .HasIndex(aur => new {aur.UserId});
        builder.Entity<Resource>()
               .HasGeneratedTsVectorColumn(r => r.NameSearchVector,
                                           "russian",
                                           r => new {Name = r.Name})
               .HasIndex(r => r.NameSearchVector)
               .HasMethod("GIN");
        builder.Entity<Resource>()
               .HasGeneratedTsVectorColumn(r => r.TagsSearchVector,
                                           "russian",
                                           r => new {r.Tags})
               .HasIndex(r => r.TagsSearchVector, "IX_Resources_TagsSearchVector")
               .HasMethod("GIN");

        builder.Entity<Image>()
               .HasData(DefaultImages);
        builder.Entity<Music>()
               .HasData(DefaultMusics);
        builder.Entity<Video>()
               .HasData(DefaultVideos);
    }
}