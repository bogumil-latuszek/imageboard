
using imageboard.Models;
using imageboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;

public class UploadModel : PageModel
{
    private readonly ItemService _itemService;
    private readonly FileHashingUtil _fileHashingUtil;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadModel> _logger;
    
    [BindProperty]
    public string? Description { get; set; } = null;
    
    [BindProperty]
    public string TagsInput { get; set; } = string.Empty;
    
    [BindProperty]
    public IFormFile ItemFile { get; set; } = null!;
    
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    
    public UploadModel(ItemService itemService, FileHashingUtil fileHashingUtil, IWebHostEnvironment environment, 
                      ILogger<UploadModel> logger)
    {
        _itemService = itemService;
        _fileHashingUtil = fileHashingUtil;
        _environment = environment;
        _logger = logger;
    }
    
    public void OnGet()
    {
        
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
            var allowedExtensions = new[] { "jpg", "jpeg", "png", "gif", "webp", "mp4", "webm" };
            var extensionWithDot = Path.GetExtension(ItemFile.FileName).ToLowerInvariant();
            var fileType = extensionWithDot?.Substring(1);
            
            if (!allowedExtensions.Contains(fileType))
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
            
            // Create item
            var item = new Item
            {
                FileType = fileType,
                Description = Description?.Trim() ?? string.Empty,
                Hash = await _fileHashingUtil.ComputeFileHashAsync(ItemFile)
            };
            
            // // Generate unique filename
            // var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            // Directory.CreateDirectory(uploadsPath);

            // Save to database
            var createdItem = await _itemService.CreateItemAsync(ItemFile, item, TagsInput);

            if(createdItem == null)
            {
                throw new Exception("error while writing item to database");
            }

            // _logger.LogInformation("Item uploaded: {FileName} by {Uploader} with {TagCount} tags", 
            //     createdItem.Id, Uploader, tagList.Count);

            SuccessMessage = $"Item uploaded successfully! ID: {createdItem.Id}";
            
            // Clear form
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