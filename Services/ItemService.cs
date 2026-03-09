using imageboard.Interfaces;
using imageboard.Models;
using imageboard.Repositories;

namespace imageboard.Services;

public class ItemService
{
    private readonly IWebHostEnvironment _environment;

    private readonly IItemRepository _repository;

    private readonly ILogger<ItemService> _logger;

    public ItemService(IWebHostEnvironment environment, IItemRepository repository, ILogger<ItemService> logger)
    {
        _environment = environment;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Item?> GetItemAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<List<Item>> SearchItemsAsync(string tagNamesRaw)
    {
        List<string> tagNames = extractTagNames(tagNamesRaw);
        return await _repository.SearchAsync(tagNames);
    }

    private List<string> extractTagNames(string tagNamesRaw)
    {
        // Parse tags (comma or space separated)
        var tagList = tagNamesRaw.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim().ToLower())
            .Where(t => t.Length > 0)
            .Distinct()
            .ToList();
        return tagList;
    }


    public async Task<Item> CreateItemAsync(IFormFile itemFile, Item item, string tagNamesRaw)
    {
        try
        {
            List<string> tagNames = extractTagNames(tagNamesRaw);

            // Save item to database
            var createdItem = await _repository.CreateAsync(item);
        
            // Add tags
            foreach (var tagName in tagNames)
            {
                await _repository.AddTagToItemAsync(createdItem.Id, tagName);
            }

            // Save itemFile in files
            saveItemFile(itemFile, createdItem.Id, createdItem.FileType);
        
            // Reload with tags
            return (await _repository.GetByIdAsync(createdItem.Id))!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item with tags");
            throw;
        }
    }

    public async void DeleteItemAsync(int ItemId)
    {
        // TODO: use transaction for File removal + Tag count update + Item Removal
        Item? itemFound = await GetItemAsync(ItemId);

        if(itemFound == null)
        {
            return;
        }

        // Delete physical file
        deleteItemFile(ItemId, itemFound.FileType);

        // Delete Item and Tag Associations using cascading
        await _repository.DeleteAsync(ItemId);
    }
    
    // ****************************
    // extract to "file manager"?

    public string getItemURL(int itemId, string fileExtension)
    {
        // var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
        // Directory.CreateDirectory(uploadsPath);
        var uploadsPath = "uploads";
        string fileName = $"{itemId}.{fileExtension}";
        return Path.Combine(uploadsPath, fileName);
        //return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
    }

    public string getItemURLForSaving(int itemId, string fileExtension)
    {
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsPath);
        string fileName = $"{itemId}.{fileExtension}";
        return Path.Combine(uploadsPath, fileName);
        //return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
    }
    public async void saveItemFile(IFormFile itemFile, int itemId, string fileExtension)
    {
        string filePath = getItemURLForSaving(itemId, fileExtension);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await itemFile.CopyToAsync(stream);
        }
    }
    public async void deleteItemFile(int itemId, string fileExtension)
    {
        string filePath = getItemURLForSaving(itemId, fileExtension);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
    // ****************************
}