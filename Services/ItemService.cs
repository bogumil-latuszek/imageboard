using imageboard.Interfaces;
using imageboard.Models;
using imageboard.Repositories;

namespace imageboard.Services;

public class ItemService
{
    private readonly IItemRepository _repository;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IItemRepository repository, ILogger<ItemService> logger)
    {
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

    public async Task<List<Item>> SearchItemsAsync(string searchTerm)
    {
        return await _repository.SearchAsync(searchTerm);
    }


    public async Task<Item> CreateItemAsync(Item item, IEnumerable<string> tags)
    {
        try
        {
            // Save item to database
            var createdItem = await _repository.CreateAsync(item);
        
            // Add tags
            foreach (var tagName in tags)
            {
                await _repository.AddTagToItemAsync(createdItem.Id, tagName);
            }
        
            // Reload with tags
            return (await _repository.GetByIdAsync(createdItem.Id))!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item with tags");
            throw;
        }
    }

    public async void DeleteItemAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task SeedSampleDataAsync()
    {
        var existingItems = await _repository.GetAllAsync();
        if (existingItems.Any()) return;

        _logger.LogInformation("Seeding sample data...");

        var sampleItems = new List<Item>
        {
            new Item
            {
                FileName = "sunset.jpg",
                Title = "Beautiful Sunset",
                Description = "A sunset over the mountains",
                Uploader = "nature_lover",
                UploadDate = DateTime.UtcNow.AddDays(-2)
            },
            new Item
            {
                FileName = "cat.png",
                Title = "Sleepy Cat",
                Description = "A cat sleeping in a sunny spot",
                Uploader = "cat_person",
                UploadDate = DateTime.UtcNow.AddDays(-1)
            }
        };

        foreach (var item in sampleItems)
        {
            var tags = item.Title.ToLower().Split(' ')
                .Where(w => w.Length > 3)
                .ToList();
                
            await CreateItemAsync(item, tags);
        }

        _logger.LogInformation("Sample data seeded successfully");
    }
}