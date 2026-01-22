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
        item = await _itemService.GetItemAsync(itemId);
    }
}