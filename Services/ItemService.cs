namespace imageboard.Services;

using imageboard.Models;

public class ItemService
    {
        // Temporary in-memory data (replace with database later)
        private readonly List<Item> _items = new()
        {
            new Item
            {
                Id = 1,
                FileName = "dragon.png",
                Title = "Blue-Eyes White Dragon",
                Description = "the best Dragon evar",
                UploadDate = DateTime.Now.AddDays(-2),
                Uploader = "kaiba",
                Tags = new List<string> { "epic", "yugioh", "dragon" }
            },
            new Item
            {
                Id = 2,
                FileName = "sunset.jpg",
                Title = "Beautiful Sunset",
                Description = "A sunset over the mountains",
                UploadDate = DateTime.Now.AddDays(-2),
                Uploader = "nature_lover",
                Tags = new List<string> { "nature", "sunset", "mountains" }
            },
            new Item
            {
                Id = 3,
                FileName = "cat.png",
                Title = "Sleepy Cat",
                Description = "A cat sleeping in a sunny spot",
                UploadDate = DateTime.Now.AddDays(-1),
                Uploader = "cat_person",
                Tags = new List<string> { "animals", "cat", "cute" }
            },
            new Item
            {
                Id = 4,
                FileName = "cityscape.gif",
                Title = "City Lights",
                Description = "Night view of city skyscrapers",
                UploadDate = DateTime.Now.AddHours(-6),
                Uploader = "urban_explorer",
                Tags = new List<string> { "city", "night", "architecture" }
            },
            new Item
            {
                Id = 5,
                FileName = "forest.png",
                Title = "Misty Forest",
                Description = "Early morning fog in the forest",
                UploadDate = DateTime.Now.AddDays(-3),
                Uploader = "hiker",
                Tags = new List<string> { "nature", "forest", "fog" }
            }
        };

        public List<Item> GetItems()
        {
            return _items.OrderByDescending(i => i.UploadDate).ToList();
        }

        public Item? GetItem(int id)
        {
            return _items.FirstOrDefault(i => i.Id == id);
        }

        public List<Item> SearchItems(string searchTerm)
        {
            return _items
                .Where(i => i.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           i.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           i.Tags.Any(t => t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(i => i.UploadDate)
                .ToList();
        }
    }