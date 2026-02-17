using System.Runtime.CompilerServices;
using imageboard.Models;
using imageboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace imageboard.Pages;


public class ItemDetails : PageModel
{
    private readonly ItemService _itemService;
    private readonly ILogger<ItemDetails> _logger;
    
    public Item? item;

    public ItemDetails(ItemService itemService, ILogger<ItemDetails> logger)
    {
        _itemService = itemService;
        _logger = logger;
    }

    public async Task OnGet(int itemId)
    {
        Item? itemFound = await _itemService.GetItemAsync(itemId);
        item = itemFound;
    }

    public string GetFileExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return string.Empty;
            
        int lastDotIndex = fileName.LastIndexOf('.');
        if (lastDotIndex == -1 || lastDotIndex == fileName.Length - 1)
            return string.Empty; // No extension or ends with dot
            
        return fileName.Substring(lastDotIndex + 1).ToLowerInvariant();
    }

    public bool IsVideoExtension(string fileExtension)
    {
        return fileExtension switch
        {
             "mp4" => true,
             "webm" => true,
             "ogg" => true,
             "avi" => true,
             "mov" => true,
             _ => false
        };
    }
}