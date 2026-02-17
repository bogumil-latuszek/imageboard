using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;

public class Example : PageModel
{
    // GET handler - called when page loads
    public void OnGet()
    {
        
    }
    
    // POST handler - automatically called on form submit
    // *Async suffix is optional for async methods!
    public async Task<IActionResult> OnPostAsync()
    {
        return null;
    }
    
    // here is an example of a custom OnPost handler - the naming convention is as follows:
    // OnPost + CustomName
    // It has to be called explicitly from .cshtml page
    public async Task<IActionResult> OnPostCaptcha()
    {
        return null;
    }

}