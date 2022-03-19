using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

public class AccountController : Controller
{
    // GET
    public IActionResult Login()
    {
        return View();
    }
    
    public IActionResult Register()
    {
        return View();
    }
}