using imageboard.Models;
using imageboard.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;

public class Gallery : PageModel
{
    private readonly ItemService _itemService;
    private readonly ILogger<Gallery> _logger;
        
    // Properties that will be available in the .cshtml file
    public List<Item> items { get; set; } = new();
    public string? SearchTerm { get; set; }
    public int TotalItems => items.Count;
        
    // Constructor with dependency injection
    public Gallery(ItemService itemService, ILogger<Gallery> logger)
    {
        _itemService = itemService;
        _logger = logger;
    }

    public string getItemURL(int itemId, string fileType)
    {
        string itemURL = _itemService.getItemURL(itemId,fileType);
        return itemURL;
    }

    public string getThumbnailURL(int itemId)
    {
        string itemURL = _itemService.getThumbnailURL(itemId);
        return itemURL;
    }
        
    // This runs when the page loads (GET request)
    public async Task OnGetAsync(string? search)
    {
        SearchTerm = search;

        if (string.IsNullOrEmpty(search))
        {
            // Get all images
            items = await _itemService.GetItemsAsync();
            _logger.LogInformation("Displaying all {Count} images", items.Count);
        }
        else
        {
            // Search for images
            items = await _itemService.SearchItemsAsync(search);
            _logger.LogInformation("Found {Count} images for search: '{Search}'", 
                items.Count, search);
        }
    }

    public bool IsVideoExtension(string fileExtension)
    {
        return fileExtension switch
        {
             "mp4" => true,
             "webm" => true,
             "ogg" => true,
             "avi" => true,
             "mov" => true,
             _ => false
        };
    }
    
}