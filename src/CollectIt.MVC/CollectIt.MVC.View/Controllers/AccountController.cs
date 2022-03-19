using CollectIt.MVC.Account.IdentityEntities;
using CollectIt.MVC.Account.Infrastructure.Data;
using CollectIt.MVC.View.DTO.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CollectIt.MVC.View.Controllers;

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
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
         _logger.LogInformation("User with email: {Email} try register", dto.Email);
         var user = new User {Email = dto.Email, UserName = dto.Email};
         var result = await _userManager.CreateAsync(user, dto.Password);
         if (result.Succeeded)
         {
             _logger.LogInformation("User with email: {Email} successfully registered", dto.Email);
             await _signInManager.SignInAsync(user, false);
             return RedirectToAction("Resource", "Home");
         }
         ModelState.AddModelError("", result.Errors.Select(x => x.Description).Aggregate((s, n) => $"{s} {n}"));
         return View();
    }

    [HttpPost]
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