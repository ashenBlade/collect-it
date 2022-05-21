using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.Database.Infrastructure.Resources.FileManagers;
using CollectIt.Database.Infrastructure.Resources.Managers;
using CollectIt.Database.Infrastructure.Resources.Repositories;
using CollectIt.MVC.Abstractions.TechSupport;
using CollectIt.MVC.Infrastructure.Account;
using CollectIt.MVC.Infrastructure.Resources;
using CollectIt.MVC.View.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction())
{
    builder.WebHost.ConfigureKestrel(kestrel =>
    {
        // Configure Heroku port
        kestrel.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("PORT")!));
    });
}

var services = builder.Services;

services.AddControllersWithViews();
services.AddAuthentication(options =>
         {
             options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
             options.DefaultSignInScheme = "/account/login";
             options.DefaultSignOutScheme = "/account/logout";
         })
        .AddCookie(options =>
         {
             options.Cookie.Name = "Cookie";
         })
        .AddGoogle(g =>
         {
             g.CorrelationCookie = new CookieBuilder()
                                   {
                                       SameSite = SameSiteMode.None,
                                       HttpOnly = false,
                                       SecurePolicy = CookieSecurePolicy.None
                                   };
             g.ClientId = builder.Configuration["Google:ClientId"];
             g.ClientSecret = builder.Configuration["Google:ClientSecret"];
             g.SignInScheme = IdentityConstants.ExternalScheme;
         });
services.AddAuthorization();
services.AddDbContext<PostgresqlCollectItDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgresql:Development"],
                      config =>
                      {
                          config.MigrationsAssembly("CollectIt.Database.Infrastructure");
                          config.UseNodaTime();
                      });
    options.UseOpenIddict<int>();
});

services.AddScoped<ISubscriptionService, PostgresqlSubscriptionService>();
services.AddScoped<ISubscriptionManager, SubscriptionManager>();

var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "content", "Videos");
var musicPath = Path.Combine(Directory.GetCurrentDirectory(), "content", "Musics");
var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "content", "images");
services.AddTransient<IVideoFileManager>(_ => new GenericPhysicalFileManager(videoPath));
services.AddTransient<IMusicFileManager>(_ => new GenericPhysicalFileManager(musicPath));
services.AddTransient<IImageFileManager>(_ => new GenericPhysicalFileManager(imagePath));
Directory.CreateDirectory(videoPath);
Directory.CreateDirectory(musicPath);
Directory.CreateDirectory(imagePath);

services.AddSignalR();
services.AddIdentity<User, Role>(config =>
         {
             config.User = new UserOptions {RequireUniqueEmail = true,};
             config.Password = new PasswordOptions
                               {
                                   RequireDigit = true,
                                   RequiredLength = 6,
                                   RequireLowercase = false,
                                   RequireUppercase = false,
                                   RequiredUniqueChars = 1,
                                   RequireNonAlphanumeric = false,
                               };
             config.SignIn = new SignInOptions
                             {
                                 RequireConfirmedEmail = false,
                                 RequireConfirmedAccount = false,
                                 RequireConfirmedPhoneNumber = false,
                             };
         })
        .AddEntityFrameworkStores<PostgresqlCollectItDbContext>()
        .AddUserManager<UserManager>()
        .AddDefaultTokenProviders()
        .AddErrorDescriber<RussianLanguageIdentityErrorDescriber>();

services.AddScoped<IImageManager, PostgresqlImageManager>();
services.AddScoped<IMusicManager, PostgresqlMusicManager>();
services.AddScoped<IVideoManager, PostgresqlVideoManager>();

services.AddScoped<IResourceAcquisitionService, ResourceAcquisitionService>();
services.AddScoped<ICommentManager, CommentManager>();
services.AddSingleton<ITechSupportChatManager, TechSupportChatManager>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<TechSupportChatHub>("/tech-support/chat");
app.MapControllerRoute(
                       name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();