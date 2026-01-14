using imageboard.Models;
using imageboard.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;


public class ItemDetails : PageModel
{
    public Image? item;
    
    public void OnGet(int imageId)
    {
        ImageService service = new ImageService();
        item = service.GetImage(imageId);
    }
}