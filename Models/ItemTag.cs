namespace imageboard.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ItemTag
{
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
        
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}