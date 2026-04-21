using Microsoft.EntityFrameworkCore;
using NameGen.Infrastructure.Data;
using NameGen.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var csvPath = builder.Configuration["SeedDataPath"]
        ?? Path.Combine(Directory.GetCurrentDirectory(),
            "..", "NameGen.Infrastructure", "Data", "Seed", "csv");

    await NameSeeder.SeedAsync(db, csvPath);
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();