using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NameGen.Core.Interfaces;
using NameGen.Core.Validators;
using NameGen.Infrastructure.Data;
using NameGen.Infrastructure.Data.Seed;
using NameGen.Infrastructure.Services;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<HumanNameRequestValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorDev", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5201",
            "https://d3ghy3wsrcikfp.cloudfront.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = 
    Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

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

app.UseSerilogRequestLogging();
app.UseCors("BlazorDev");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();