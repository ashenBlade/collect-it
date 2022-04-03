using CollectIt.API.DTO.Account;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Set up jwt
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddIdentity<User, Role>(config => 
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
       .AddUserManager<UserManager>()
       .AddEntityFrameworkStores<PostgresqlCollectItDbContext>()
       .AddDefaultTokenProviders();

builder.Services.AddDbContext<PostgresqlCollectItDbContext>(config =>
{
    config.UseNpgsql(builder.Configuration["ConnectionStrings:Postgresql:Development"], options =>
    {
        options.UseNodaTime();
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();