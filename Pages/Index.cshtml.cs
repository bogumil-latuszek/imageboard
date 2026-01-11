using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;

public class IndexModel : PageModel
{
    public void OnGet()
    {
        // proving execution order
        System.Diagnostics.Debug.WriteLine("index.cshtml.cs executing NOW!");
    }
}
