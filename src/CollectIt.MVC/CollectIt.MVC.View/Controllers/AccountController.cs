using System.Security.Claims;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.Entities.Account;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager _userManager;

    public AccountController(ILogger<AccountController> logger,
                             UserManager userManager,
                             SignInManager<User> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Authorize]
    [HttpGet]
    [Route("")]
    [Route("profile")]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var subscriptions = ( await _userManager.GetSubscriptionsForUserByIdAsync(userId) )
           .Select(subscription =>
                       new AccountUserSubscription()
                       {
                           From = subscription.During.Start.ToDateTimeUnspecified(),
                           To = subscription.During.End.ToDateTimeUnspecified(),
                           LeftResourcesCount = subscription.LeftResourcesCount,
                           Name = subscription.Subscription.Name,
                           ResourceType = subscription.Subscription.AppliedResourceType == ResourceType.Image
                                              ? "Изображение"
                                              : "Другое"
                       });
        var resources = ( await _userManager.GetAcquiredResourcesForUserByIdAsync(userId) )
           .Select(resource =>
                       new AccountUserResource()
                       {
                           Id = resource.ResourceId,
                           FileName = resource.Resource.Name,
                           // Address = resource.Resource.Address,
                           Extension = resource.Resource.Extension,
                           Date = resource.AcquiredDate
                       });
        var myResources = ( await _userManager.GetUsersResourcesForUserByIdAsync(userId) )
           .Select(resource =>
                       new AccountUserResource()
                       {
                           Id = resource.Id,
                           FileName = resource.Name,
                           //Address = resource.Address,
                           Extension = resource.Extension,
                           Date = resource.UploadDate
                       });
        var model = new AccountViewModel()
                    {
                        UserName = User.FindFirstValue(ClaimTypes.Name),
                        Email = User.FindFirstValue(ClaimTypes.Email),
                        Subscriptions = subscriptions,
                        AcquiredResources = resources,
                        UsersResources = myResources,
                        Roles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User))
                    };
        return View(model);
    }

    [HttpGet]
    [Route("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    [Route("register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _logger.LogInformation("User (Email: {Email}) wants to register", model.Email);
        var user = new User {Email = model.Email, UserName = model.UserName};
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User (Email: {Email}) successfully registered", model.Email);
            return RedirectToAction("Login");
        }

        _logger.LogInformation("User (Email: {Email}) has already registered", model.Email);
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _logger.LogInformation("User with email: {Email} wants to login", model.Email);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            ModelState.AddModelError("", "Пользователя с такой почтой не существует");
            return View();
        }

        if (!( await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false) ).Succeeded)
        {
            ModelState.AddModelError("", "Неправильный пароль");
            return View(model);
        }

        await _signInManager.SignInAsync(user, model.RememberMe);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction("Login");
    }

    [HttpPost("edit")]
    [Authorize]
    public async Task<IActionResult> EditAccount(ProfileAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Profile");
        }

        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return View("Error", new ErrorViewModel() {Message = "User not found"});
        }

        user.UserName = model.Username;
        user.NormalizedUserName = model.Username.ToUpper();
        user.Email = model.Email;
        user.NormalizedEmail = model.Email.ToUpper();
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Profile");
        }

        _logger.LogInformation($"Error while updating user credentials:\n{result.Errors.Select(e => $"- {e.Description}").Aggregate((s, n) => $"{s}\n{n}").ToArray()}");
        return View("Error", new ErrorViewModel() {Message = "Ошибка при обновлении ваших данных"});
    }


    [HttpGet("external")]
    public async Task<IActionResult> GetExternalLogins()
    {
        var logins = await _signInManager.GetExternalAuthenticationSchemesAsync();
        return Ok(logins.Select(l => new {l.Name, l.DisplayName}));
    }

    [Route("google-login")]
    public IActionResult GoogleLogin()
    {
        // var properties = new AuthenticationProperties() {RedirectUri = Url.Action("GoogleResponse")};
        // return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        var properties =
            _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme,
                                                                     Url.Action("GoogleResponse"));
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [Route("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            _logger.LogError("Could not get google external login info");
            return View("Error", new ErrorViewModel() {Message = "User claims were not provided"});
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                                                                   info.ProviderKey,
                                                                   true);
        if (result.Succeeded)
        {
            _logger.LogTrace("User with existing account logged in using google");
            return RedirectToAction("Index", "Home");
        }


        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var username = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
        var user = new User() {Email = email, UserName = username};
        var identityResult = await _userManager.CreateAsync(user);
        if (identityResult.Succeeded && ( await _userManager.AddLoginAsync(user, info) ).Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Profile");
        }

        return View("Error",
                    new ErrorViewModel() {Message = "Could not create account with provided google credentials"});
    }
}