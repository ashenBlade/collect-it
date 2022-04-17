using System.Security.Claims;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.Entities.Account;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace CollectIt.MVC.View.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager _userManager;
    private readonly SignInManager<User> _signInManager;

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
                    ResourceType = subscription.Subscription.AppliedResourceType == ResourceType.Image ? "Изображение" : "Другое"
                });
        var resources = (await _userManager.GetAcquiredResourcesForUserByIdAsync(userId))
            .Select(resource =>
                new AccountUserResource()
                {
                    Id = resource.ResourceId,
                    FileName = resource.Resource.Name,
                    Address = resource.Resource.Address,
                    Extension = resource.Resource.Extension,
                    Date = resource.AcquiredDate
                });
        var myResources = (await _userManager.GetUsersResourcesForUserByIdAsync(userId))
            .Select(resource =>
                new AccountUserResource()
                {
                    Id = resource.Id,
                    FileName = resource.Name,
                    Address = resource.Address,
                    Extension = resource.Extension,
                    Date = resource.UploadDate
                });
       var model = new AccountViewModel()
                    {
                        UserName = User.FindFirstValue(ClaimTypes.Name),
                        Email = User.FindFirstValue(ClaimTypes.Email),
                        Subscriptions = subscriptions,
                        AcquiredResources = resources,
                        UsersResources = myResources
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

    [HttpPost]
    [Authorize]
    [Route("edit")]
    public async Task<IActionResult> EditAccount(ProfileAccountViewModel model)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return BadRequest();
        }

        user.UserName = model.Username;
        user.NormalizedUserName = model.Username.ToUpper();
        user.Email = model.Email;
        user.NormalizedEmail = model.Email.ToUpper();
        var result = await _userManager.UpdateAsync(user);
        var subs = await _userManager.GetSubscriptionsForUserByIdAsync(user.Id);

       var accountModel = new AccountViewModel()
                           {
                               Email = user.UserName, 
                               UserName = user.UserName, 
                               Subscriptions = subs.Select(s => new AccountUserSubscription()
                                                                {
                                                                    From = s.During.Start.ToDateTimeUnspecified(),
                                                                    To = s.During.End.ToDateTimeUnspecified(),
                                                                    Name = s.Subscription.Name,
                                                                    ResourceType = s.Subscription.AppliedResourceType == ResourceType.Image ? "Изображение" : "Другое",
                                                                    LeftResourcesCount = s.LeftResourcesCount
                                                                })
                                                   .ToList()
                           };
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            return View("Profile", accountModel);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("Profile", accountModel);
    }

    [HttpPost]
    [Authorize]
    [Route("upload")]
    public void UploadImage(ImageViewModel model)
    {
        
    }
}