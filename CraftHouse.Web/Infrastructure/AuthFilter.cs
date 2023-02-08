using System.Reflection;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CraftHouse.Web.Infrastructure;

public class AuthFilter : IAsyncPageFilter
{
    private readonly ILogger<AuthFilter> _logger;
    private readonly IAuthService _authService;

    public AuthFilter(ILogger<AuthFilter> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        => Task.CompletedTask;

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
        PageHandlerExecutionDelegate next)
    {
        var result = context.HandlerInstance.GetType().GetCustomAttributes()
            .FirstOrDefault(x => x.GetType() == typeof(RequireAuthAttribute));

        if (result is not null)
        {
            var user = await _authService.GetLoggedInUserAsync(default);
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
                await next.Invoke();
            }
        }
        else
        {
            await next.Invoke();
        }
    }
}