using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollectIt.MVC.View.Pages;

public class Index : PageModel
{
    private readonly ILogger<Index> _logger;

    public Index(ILogger<Index> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
    }
}