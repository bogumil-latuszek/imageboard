using Microsoft.AspNetCore.Mvc.RazorPages;
using imageboard.Data;
using Microsoft.EntityFrameworkCore;

namespace imageboard.Pages;

public class TestDbModel : PageModel
{
    private readonly AppDbContext _context; // Injected by ASP.NET Core
    private readonly ILogger<TestDbModel> _logger;
    
    public string DatabasePath { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TagCount { get; set; }
    public List<string> Tables { get; set; } = new();
    
    public TestDbModel(AppDbContext context, ILogger<TestDbModel> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task OnGetAsync()
    {
        try
        {
            // Get database info
            DatabasePath = _context.Database.GetDbConnection().DataSource;
            
            // Check if we can connect
            var canConnect = await _context.Database.CanConnectAsync();
            System.Diagnostics.Debug.WriteLine("canConnect = "+canConnect);
            
            if (canConnect)
            {
                // open connection with database
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                
                // retrieve all Tables besides sqlite_sequence and EF migrations:
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name NOT LIKE '__EF%'";
                
                // retrieve all Tables:
                //command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Tables.Add(reader.GetString(0));
                }
                
                await connection.CloseAsync();
                
                // Count records
                ItemCount = await _context.Items.CountAsync();
                TagCount = await _context.Tags.CountAsync();
                
                // this is logged in debug output console
                _logger.LogInformation("Database test successful: {Tables} tables, {Items} items", 
                    Tables.Count, ItemCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database test failed");
        }
    }
}