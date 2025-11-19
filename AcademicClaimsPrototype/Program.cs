using AcademicClaimsPrototype.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Create uploads directory if it doesn't exist
var uploadsDir = Path.Combine(app.Environment.WebRootPath, "uploads");
if (!Directory.Exists(uploadsDir))
{
    Directory.CreateDirectory(uploadsDir);
}

// Create invoices directory
var invoicesDir = Path.Combine(app.Environment.WebRootPath, "invoices");
if (!Directory.Exists(invoicesDir))
{
    Directory.CreateDirectory(invoicesDir);
}

// === DATABASE RECREATION SECTION ===
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        Console.WriteLine("Starting database initialization...");

        // Force delete and recreate to ensure new columns are added
        context.Database.EnsureDeleted();
        Console.WriteLine("Old database deleted.");

        context.Database.EnsureCreated();
        Console.WriteLine("New database created with updated schema.");

        // Verify it works by counting records
        var userCount = context.Users.Count();
        var claimCount = context.Claims.Count();
        Console.WriteLine($"Database ready with {userCount} users and {claimCount} claims.");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization error: {ex.Message}");
        Console.WriteLine("Stack trace: " + ex.StackTrace);

        // Try one more time with a simpler approach
        try
        {
            context.Database.EnsureCreated();
            Console.WriteLine("Database created using fallback method.");
        }
        catch (Exception ex2)
        {
            Console.WriteLine($"Fallback also failed: {ex2.Message}");
        }
    }
}

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();