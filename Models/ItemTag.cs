namespace imageboard.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ItemTag
{
    // composite key rules have to be set up by fulent api
    public int ItemId { get; set; }
    public int TagId { get; set; }

    //NAVIGATION
    public Item Item { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}