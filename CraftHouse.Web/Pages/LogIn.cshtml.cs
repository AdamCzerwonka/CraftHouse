using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class LogIn : PageModel
{
    [BindProperty]
    public string Mail { get; set; } = null!;

    [BindProperty]
    public string Password { get; set; } = null!;
    
    public void OnGet()
    {
        
    }
    
    public void OnPost()
    {
        
    }
}