using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CraftHouse.Web.ViewComponents;

public class MenuViewComponent : ViewComponent
{
    private readonly IAuthService _authService;

    public MenuViewComponent(IAuthService authService)
    {
        _authService = authService;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
    {
        var user = await _authService.GetLoggedInUserAsync(cancellationToken);
        var model = new MenuViewComponentModel()
        {
            User = user
        };
        return View("Default", model);
    }
}

public class MenuViewComponentModel
{
    public User? User { get; set; }
}