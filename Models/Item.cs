using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace imageboard.Models;

// TODO: Rename to Item? Item Container?
public class Item
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(50)]
    public string Uploader { get; set; } = string.Empty;
    
    // public long FileSize { get; set; }
    public List<string> Tags { get; set; } = new();
        
    // Helper property for display
    
    [NotMapped]
    public string ThumbnailUrl => $"/thumbnails/{FileName}";
    
    [NotMapped]
    public string FullUrl => $"/uploads/{FileName}";
    
    
    // Navigation property for tags (many-to-many)
    public List<ItemTag> ItemTags { get; set; } = new();
    
}