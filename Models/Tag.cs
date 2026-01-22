namespace imageboard.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Tag
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
        
    // Navigation property
    public List<ItemTag> ItemTags { get; set; } = new();
}