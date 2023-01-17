using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class LogIn : PageModel
{
    [BindProperty]
    public string Mail { get; set; }
    [BindProperty]
    public string Password { get; set; }
    
    public void OnGet()
    {
        
    }
    
    public void OnPost()
    {
        
    }
}