using imageboard.Models;

namespace imageboard.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(int id);
    Task<List<Item>> GetAllAsync();
    Task<List<Item>> SearchAsync(string searchTerm);
    Task<Item> CreateAsync(Item item);
    Task UpdateAsync(Item item);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
        
    // Tag management
    Task<Tag?> GetTagByNameAsync(string name);
    Task<Tag> CreateTagAsync(string name);
    void DeleteTagAsync(int Id);
    Task AddTagToItemAsync(int itemId, string tagName);
    Task RemoveTagFromItemAsync(int itemId, int tagId);
}