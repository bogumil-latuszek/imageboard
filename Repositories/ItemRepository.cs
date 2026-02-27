using imageboard.Data;
using imageboard.Interfaces;
using imageboard.Models;
using Microsoft.EntityFrameworkCore;


namespace imageboard.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(AppDbContext context, ILogger<ItemRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Items
                .Include(i => i.ItemTags)
                .ThenInclude(it => it.Tag)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Item with ID {ItemId}", id);
            throw;
        }
    }

    public async Task<List<Item>> GetAllAsync()
    {
        try
        {
            return await _context.Items
                .Include(i => i.ItemTags)
                .ThenInclude(it => it.Tag)
                .OrderByDescending(i => i.UploadDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all Items");
            throw;
        }
    }

    public async Task<List<Item>> SearchAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.Trim().ToLower();
            
            return await _context.Items
                .Include(i => i.ItemTags)
                .ThenInclude(it => it.Tag)
                .Where(i => 
                    i.Title.ToLower().Contains(term) ||
                    i.Description.ToLower().Contains(term) ||
                    i.Uploader.ToLower().Contains(term) ||
                    i.ItemTags.Any(it => it.Tag.Name.ToLower().Contains(term)))
                .OrderByDescending(i => i.UploadDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Items with term '{SearchTerm}'", searchTerm);
            throw;
        }
    }

    public async Task<Item> CreateAsync(Item item)
    {
        try
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item");
            throw;
        }
    }

    public async Task UpdateAsync(Item item)
    {
        try
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Item with ID {ItemId}", item.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var item = await GetByIdAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item with ID {ItemId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Items.AnyAsync(i => i.Id == id);
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
    }

    public async Task<Tag> CreateTagAsync(string name)
    {
        var tag = new Tag { Name = name.Trim() };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async void DeleteTagAsync(int Id)
    {
        try
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == Id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item with ID {ItemId}", Id);
            throw;
        }
    }


    public async Task AddTagToItemAsync(int itemId, string tagName)
    {
        var item = await GetByIdAsync(itemId);
        if (item == null)
            throw new ArgumentException($"Item with ID {itemId} not found");

        var tag = await GetTagByNameAsync(tagName) ?? await CreateTagAsync(tagName);
        // Check if relationship already exists
        if (!item.ItemTags.Any(it => it.TagId == tag.Id))
        {
            tag.UseCount += 1;
            item.ItemTags.Add(new ItemTag { ItemId = itemId, TagId = tag.Id });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveTagFromItemAsync(int itemId, int tagId)
    {
        var itemTag = await _context.ItemTags
            .FirstOrDefaultAsync(it => it.ItemId == itemId && it.TagId == tagId);
            
        if (itemTag != null)
        {
            _context.ItemTags.Remove(itemTag);
            await _context.SaveChangesAsync();
        }
    }
}