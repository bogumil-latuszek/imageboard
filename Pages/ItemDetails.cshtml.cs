using imageboard.Models;
using imageboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;


public class ItemDetails : PageModel
{
    public Item? item;
    
    public IActionResult OnGet(int imageId)
    {
        ItemService service = new ItemService();
        item = service.GetItem(imageId);
        if (item == null)
        {
            return NotFound();  // Returns 404 status code
        }

        return Page();
    }
    
    // public void OnGet(int imageId)
    // {
    //     ImageService service = new ImageService();
    //     item = service.GetImage(imageId);
    // }
}