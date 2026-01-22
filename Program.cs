using imageboard.Data;
using imageboard.Interfaces;
using imageboard.Repositories;
using imageboard.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "imageboard.db");
    options.UseSqlite($"Data Source={dbPath}");
    
    // Enable detailed errors (remove in production)
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();  // For debugging
});

// Register repository/services
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ItemService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
