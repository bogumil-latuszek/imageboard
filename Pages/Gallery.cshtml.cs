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
        
    // This runs when the page loads (GET request)
    public void OnGet(string? search)
    {
        SearchTerm = search;
            
        if (string.IsNullOrEmpty(search))
        {
            // Get all images
            items = _itemService.GetItems();
            _logger.LogInformation("Displaying all {Count} images", items.Count);
        }
        else
        {
            // Search for images
            items = _itemService.SearchItems(search);
            _logger.LogInformation("Found {Count} images for search: '{Search}'", 
                items.Count, search);
        }
    }
    
}