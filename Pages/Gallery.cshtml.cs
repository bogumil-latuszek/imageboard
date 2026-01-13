using imageboard.Models;
using imageboard.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;

public class Gallery : PageModel
{
    private readonly ImageService _imageService;
    private readonly ILogger<Gallery> _logger;
        
    // Properties that will be available in the .cshtml file
    public List<Image> Images { get; set; } = new();
    public string? SearchTerm { get; set; }
    public int TotalImages => Images.Count;
        
    // Constructor with dependency injection
    public Gallery(ImageService imageService, ILogger<Gallery> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }
        
    // This runs when the page loads (GET request)
    public void OnGet(string? search)
    {
        SearchTerm = search;
            
        if (string.IsNullOrEmpty(search))
        {
            // Get all images
            Images = _imageService.GetImages();
            _logger.LogInformation("Displaying all {Count} images", Images.Count);
        }
        else
        {
            // Search for images
            Images = _imageService.SearchImages(search);
            _logger.LogInformation("Found {Count} images for search: '{Search}'", 
                Images.Count, search);
        }
    }
    
}