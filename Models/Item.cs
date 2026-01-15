namespace imageboard.Models;

// TODO: Rename to Item? Item Container?
public class Item
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string Uploader { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
        
    // Helper property for display
    public string ThumbnailUrl => $"/thumbnails/{FileName}";
    public string FullImageUrl => $"/uploads/{FileName}";
}