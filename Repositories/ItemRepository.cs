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

    public async Task<List<Item>> SearchAsync(List<string> tagNames)
    {
        try
        {
            // for empty tag name list => return all items
            if (!tagNames.Any())
            {
                return await GetAllAsync();
            }
            
            // Remove duplicates to avoid counting issues
            var distinctTagNames = tagNames.Distinct().ToList();

            List<int> tagIds = await _context.Tags
                .Where(t => distinctTagNames.Contains(t.Name))
                .Select(t => t.Id)
                .ToListAsync();

            // If any tags couldn't be found => return empty list
            if(tagIds.Count != distinctTagNames.Count)
            {
                return new List<Item>();
            }

            // List<Item> items = await _context.Items
            //     .Include(i => i.ItemTags)
            //     .ThenInclude(it => it.Tag)
            //     .Where(i => i.ItemTags.All(it => tagIds.Contains(it.Tag.Id)))
            //     .OrderByDescending(i => i.UploadDate)
            //     .ToListAsync();

            // find all items that have ALL searched tags (but can have more)
            List<Item> items = await _context.Items
                .Include(i => i.ItemTags)
                .ThenInclude(it => it.Tag)
                .Where(i => tagIds.All(tagId => i.ItemTags.Any(it => it.Tag.Id == tagId)))
                .OrderByDescending(i => i.UploadDate)
                .ToListAsync();
            
            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Items with term '{SearchTerm}'", tagNames.ToString());
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
                // Update tag use counts
                foreach (var itemTag in item.ItemTags.ToList())
                {
                    // var tag = itemTag.Tag;
                    // tag.UseCount--;
                    
                    // if (tag.UseCount <= 0)
                    // {
                    //     // Remove orphaned tag
                    //     DeleteTagAsync(tag.Id);
                    // }
                    // // Note: The ItemTag itself will be cascade deleted when we remove the Item
                    await RemoveTagFromItemAsync(itemTag.ItemId, itemTag.TagId);
                }

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
            itemTag.Tag.UseCount -= 1;
            if(itemTag.Tag.UseCount <= 0)
            {
                _context.Tags.Remove(itemTag.Tag);
            }
            _context.ItemTags.Remove(itemTag);
            await _context.SaveChangesAsync();
        }
    }
}