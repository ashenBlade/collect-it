using CollectIt.MVC.Account.IdentityEntities;
using CollectIt.MVC.Account.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PostgresqlIdentityDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration["ConnectionStrings:Accounts:PostgresqlDevelopment"],
                      config =>
                      {
                          config.MigrationsAssembly("CollectIt.MVC.View");
                      });
});
builder.Services.AddIdentity<User, Role>()
       .AddEntityFrameworkStores<PostgresqlIdentityDbContext>()
       .AddUserManager<UserManager>()
       .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();