using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CraftHouse.Web.ViewComponents;

public class MenuViewComponent : ViewComponent
{
    private readonly IAuthService _authService;
    private readonly ICartService _cartService;

    public MenuViewComponent(IAuthService authService, ICartService cartService)
    {
        _authService = authService;
        _cartService = cartService;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
    {
        var user = await _authService.GetLoggedInUserAsync(cancellationToken);
        var cartItemCount = _cartService.GetCartEntries().Count();
        var model = new MenuViewComponentModel()
        {
            User = user,
            CartItemCount = cartItemCount
        };
        return View("Default", model);
    }
}

public class MenuViewComponentModel
{
    public User? User { get; set; }
    public int CartItemCount { get; set; }
}