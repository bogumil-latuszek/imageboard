using imageboard.Models;
using Microsoft.EntityFrameworkCore;

namespace imageboard.Data;

public class AppDbContext: DbContext
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ItemTag> ItemTags => Set<ItemTag>();
        
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
            
        // Configure composite key
        modelBuilder.Entity<ItemTag>()
            .HasKey(it => new { it.ItemId, it.TagId });
        
        // Configure one-to-many relationship between Item-ItemTag
        modelBuilder.Entity<ItemTag>()
            .HasOne(it => it.Item)
            .WithMany(i => i.ItemTags)
            .HasForeignKey(it => it.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
                
        // Configure one-to-many relationship between Tag-ItemTag
        modelBuilder.Entity<ItemTag>()
            .HasOne(it => it.Tag)
            .WithMany(t => t.ItemTags)
            .HasForeignKey(it => it.TagId);
                
        // Add some indexes for performance
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.UploadDate);
                
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.UploaderId);
    }
}