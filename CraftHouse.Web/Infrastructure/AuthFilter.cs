using System.Reflection;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Infrastructure;

public class AuthFilter : IPageFilter
{
    private readonly ILogger<AuthFilter> _logger;
    private readonly IAuthService _authService;

    public AuthFilter(ILogger<AuthFilter> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    public void OnPageHandlerSelected(PageHandlerSelectedContext context)
    {
    }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var result = context.HandlerInstance.GetType().GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(RequireAuthAttribute));

        if (result is not null)
        {
            var user = _authService.GetLoggedInUser();
            if (user is null)
            {
                context.Result = new RedirectToPageResult("/login",
                    new { redirectUrl = context.HttpContext.Request.Path.Value });
            }
            else
            {
                var allowedUserTypes = (result as RequireAuthAttribute)!.AllowedTypes;
                if (!allowedUserTypes.Contains(user.UserType))
                {
                    context.Result = new NotFoundResult();
                }
                _logger.LogInformation("Allowed user types: {@userTypes}", allowedUserTypes);
            }
        }
    }

    public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
    }
}