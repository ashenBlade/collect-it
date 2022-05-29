using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.Entities.Account;
using CollectIt.MVC.Infrastructure;
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
    private readonly IMailSender _mailSender;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager _userManager;

    public AccountController(ILogger<AccountController> logger,
                             UserManager userManager,
                             SignInManager<User> signInManager,
                             IMailSender mailSender)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _mailSender = mailSender;
    }

    [Authorize]
    [HttpGet]
    [Route("")]
    [Route("profile")]
    public async Task<IActionResult> Profile()
    {
        // var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userManager.GetUserAsync(User);
        var subscriptions = ( await _userManager.GetSubscriptionsForUserByIdAsync(user.Id) )
           .Select(subscription =>
                       new AccountUserSubscription()
                       {
                           From = subscription.During.Start.ToDateTimeUnspecified(),
                           To = subscription.During.End.ToDateTimeUnspecified(),
                           LeftResourcesCount = subscription.LeftResourcesCount,
                           Name = subscription.Subscription.Name,
                           ResourceType = subscription.Subscription.AppliedResourceType switch
                                          {
                                              ResourceType.Image => "Изображение",
                                              ResourceType.Video => "Видео",
                                              ResourceType.Music => "Музыка",
                                              ResourceType.Any   => "Любой",
                                              _ => throw new ArgumentOutOfRangeException(nameof(subscription
                                                                                               .Subscription
                                                                                               .AppliedResourceType))
                                          }
                       });
        var resources = ( await _userManager.GetAcquiredResourcesForUserByIdAsync(user.Id) )
           .Select(resource =>
                       new AccountUserResource()
                       {
                           Id = resource.ResourceId,
                           FileName = resource.Resource.Name,
                           Extension = resource.Resource.Extension,
                           Date = resource.AcquiredDate,
                           ResourceType = resource.Resource switch
                                          {
                                              Image => ResourceType.Image,
                                              Video => ResourceType.Video,
                                              Music => ResourceType.Music,
                                              _     => ResourceType.Image
                                          }
                       });
        var myResources = ( await _userManager.GetUsersResourcesForUserByIdAsync(user.Id) )
           .Select(resource =>
                       new AccountUserResource()
                       {
                           Id = resource.Id,
                           FileName = resource.Name,
                           Extension = resource.Extension,
                           Date = resource.UploadDate,
                           ResourceType = resource switch
                                          {
                                              Image => ResourceType.Image,
                                              Video => ResourceType.Video,
                                              Music => ResourceType.Music,
                                              _     => ResourceType.Image
                                          }
                       });
        var model = new AccountViewModel()
                    {
                        UserName = User.FindFirstValue(ClaimTypes.Name),
                        Email = User.FindFirstValue(ClaimTypes.Email),
                        Subscriptions = subscriptions,
                        AcquiredResources = resources,
                        UsersResources = myResources,
                        Roles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User)),
                        EmailConfirmed = user.EmailConfirmed
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

        try
        {
            _logger.LogInformation("User (Email: {Email}) wants to register", model.Email);
            var user = new User {Email = model.Email, UserName = model.UserName};
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                try
                {
                    await _mailSender.SendMailAsync("Подтверждение почты", CreateConfirmationMailMessageBody(token),
                                                    user.Email);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while sending email confirmation");
                }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while register");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при регистрации"});
        }
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while Login");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при входе в аккаунт"});
        }
    }

    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> LogOut()
    {
        try
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while LogOut");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при выходе из аккаунта"});
        }
    }

    [HttpPost("edit")]
    [Authorize]
    public async Task<IActionResult> EditAccount([Required] ProfileAccountViewModel model)
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

        if (user.UserName != model.Username)
        {
            user.UserName = model.Username;
            user.NormalizedUserName = model.Username.ToUpper();
        }

        if (user.Email != model.Email)
        {
            user.Email = model.Email;
            user.NormalizedEmail = model.Email.ToUpper();
            user.EmailConfirmed = false;
        }

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Profile");
        }

        _logger.LogInformation(
                               $"Error while updating user credentials:\n{result.Errors.Select(e => $"- {e.Description}").Aggregate((s, n) => $"{s}\n{n}").ToArray()}");
        return View("Error", new ErrorViewModel() {Message = "Ошибка при обновлении ваших данных"});
    }


    [HttpGet("external")]
    public async Task<IActionResult> GetExternalLogins()
    {
        try
        {
            var logins = await _signInManager.GetExternalAuthenticationSchemesAsync();
            return Ok(logins.Select(l => new {l.Name, l.DisplayName}));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting External loggins");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при получении external loggins"});
        }
    }

    [Route("google-login")]
    public IActionResult GoogleLogin()
    {
        try
        {
            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme,
                                                                         Url.Action("GoogleResponse"));
            // var properties = new AuthenticationProperties() {RedirectUri = Url.Action("GoogleResponse")};
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while google login");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при попытке входа через google"});
        }
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
            _logger.LogInformation("User with existing account logged in using google");
            return RedirectToAction("Index", "Home");
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var username = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
        var user = new User() {Email = email, UserName = username};
        var identityResult = await _userManager.CreateAsync(user);
        if (identityResult.Succeeded && ( await _userManager.AddLoginAsync(user, info) ).Succeeded)
        {
            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Profile");
        }


        return View("Error",
                    new ErrorViewModel() {Message = "Could not create account with provided google credentials"});
    }

    [Authorize]
    [HttpGet("confirm")]
    public async Task<IActionResult> ConfirmEmail([FromQuery(Name = "token")] [Required] string token)
    {
        var user = await _userManager.GetUserAsync(User);
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Profile");
        }

        return View("Error", new ErrorViewModel() {Message = "Could not confirm your email. Try later"});
    }

    [Authorize]
    [HttpPost("send-email-confirmation")]
    public async Task<IActionResult> SendEmailConfirmation()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _mailSender.SendMailAsync("Подтверждение почты",
                                            CreateConfirmationMailMessageBody(token),
                                            user.Email);
            return RedirectToAction("Profile");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending confirmation email");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при подтверждении почты"});
        }
    }

    private string CreateConfirmationMailMessageBody(string? token)
    {
        return $@"
    Confirm your account: <a href=""https://collect-it-app.herokuapp.com{Url.Action("ConfirmEmail", new {token = token})}"">Click here</a>
";
    }
}