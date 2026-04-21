using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NameGen.Core.Interfaces;
using NameGen.Infrastructure.Data;
using NameGen.Infrastructure.Data.Seed;
using NameGen.Infrastructure.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IHumanNameService, HumanNameService>();
builder.Services.AddScoped<IFictionalNameService, FictionalNameService>();
builder.Services.AddScoped<IUsernameService, UsernameService>();
builder.Services.AddScoped<IFavoritesService, FavoritesService>();
builder.Services.AddScoped<IGenerationHistoryService, GenerationHistoryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NameGen API",
        Version = "v1",
        Description = "A hybrid name generator API for human names, fictional characters, and gaming usernames."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Apply migrations and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var csvPath = builder.Configuration["SeedDataPath"]
        ?? Path.Combine(Directory.GetCurrentDirectory(),
            "..", "NameGen.Infrastructure", "Data", "Seed", "csv");

    await NameSeeder.SeedAsync(db, csvPath);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NameGen API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();