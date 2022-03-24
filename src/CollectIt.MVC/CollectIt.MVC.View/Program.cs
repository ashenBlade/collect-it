using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = "/account/login";
    options.DefaultSignOutScheme = "/account/logout";
}).AddCookie(options =>
{
    options.Cookie.Name = "Cookie";
});
builder.Services.AddAuthorization();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<PostgresqlCollectItDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration["ConnectionStrings:Postgresql:Development"],
                      config =>
                      {
                          config.MigrationsAssembly("CollectIt.MVC.View");
                          config.UseNodaTime();
                      });
});
builder.Services.AddIdentity<User, Role>(config =>
        {
            config.User = new UserOptions
                          {
                              RequireUniqueEmail = true,
                          };
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
       .AddDefaultTokenProviders();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();