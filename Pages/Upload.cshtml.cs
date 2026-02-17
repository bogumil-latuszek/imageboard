
using imageboard.Models;
using imageboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;

public class UploadModel : PageModel
{
    private readonly ItemService _itemService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadModel> _logger;
    
    [BindProperty]
    public string Title { get; set; } = string.Empty;
    
    [BindProperty]
    public string Description { get; set; } = string.Empty;
    
    [BindProperty]
    public string Uploader { get; set; } = string.Empty;
    
    [BindProperty]
    public string TagsInput { get; set; } = string.Empty;
    
    [BindProperty]
    public IFormFile ItemFile { get; set; } = null!;
    
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    
    public UploadModel(ItemService itemService, IWebHostEnvironment environment, 
                      ILogger<UploadModel> logger)
    {
        _itemService = itemService;
        _environment = environment;
        _logger = logger;
    }
    
    public void OnGet()
    {
        // Pre-fill uploader if possible (future: from logged-in user)
        Uploader = User.Identity?.Name ?? "Anonymous";
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please check the form for errors.";
            return Page();
        }
        
        if (ItemFile == null || ItemFile.Length == 0)
        {
            ErrorMessage = "Please select a file to upload.";
            return Page();
        }
        
        try
        {
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".webm" };
            var extension = Path.GetExtension(ItemFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                ErrorMessage = $"File type not allowed. Allowed: {string.Join(", ", allowedExtensions)}";
                return Page();
            }
            
            // Validate file size (10MB max)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (ItemFile.Length > maxFileSize)
            {
                ErrorMessage = $"File too large. Maximum size is {maxFileSize / (1024 * 1024)}MB.";
                return Page();
            }
            
            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);
            
            var filePath = Path.Combine(uploadsPath, fileName);
            
            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ItemFile.CopyToAsync(stream);
            }
            
            // Parse tags (comma or space separated)
            var tagList = TagsInput.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLower())
                .Where(t => t.Length > 0)
                .Distinct()
                .ToList();
            
            // Create item
            var item = new Item
            {
                FileName = fileName,
                Title = Title.Trim(),
                Description = Description.Trim(),
                Uploader = string.IsNullOrWhiteSpace(Uploader) ? "Anonymous" : Uploader.Trim(),
                UploadDate = DateTime.UtcNow
            };
            
            // Save to database
            var createdItem = await _itemService.CreateItemAsync(item, tagList);
            
            _logger.LogInformation("Item uploaded: {FileName} by {Uploader} with {TagCount} tags", 
                fileName, Uploader, tagList.Count);
            
            SuccessMessage = $"Item uploaded successfully! ID: {createdItem.Id}";
            
            // Clear form
            Title = string.Empty;
            Description = string.Empty;
            TagsInput = string.Empty;
            ModelState.Clear();
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading item");
            ErrorMessage = $"An error occurred: {ex.Message}";
            return Page();
        }
    }
}