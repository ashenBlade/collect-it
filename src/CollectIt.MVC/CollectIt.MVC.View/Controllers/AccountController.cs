using CollectIt.MVC.Account.IdentityEntities;
using CollectIt.MVC.Account.Infrastructure.Data;
using CollectIt.MVC.View.DTO.Account;
using CollectIt.MVC.View.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        var user = new User {Email = model.Email, UserName = model.Email};
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User (Email: {Email}) successfully registered", model.Email);
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Resource", "Home");
        }
        _logger.LogInformation("User (Email: {Email}) has already registered", model.Email);
        ModelState.AddModelError("", "Пользователь с такой почтой уже зарегистрирован");
        return View(model);
        
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        _logger.LogInformation("User with email: {Email} wants to login", dto.Email);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            ModelState.AddModelError("", $"No user with such email: {dto.Email}");
            return View();
        }
        
        if (await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError("", "Invalid password");
        return View();

    }
}