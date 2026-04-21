using Microsoft.EntityFrameworkCore;
using NameGen.Core.Interfaces;
using NameGen.Core.Models;
using NameGen.Infrastructure.Data;

namespace NameGen.Infrastructure.Services;

public class FavoritesService : IFavoritesService
{
    private readonly AppDbContext _context;

    public FavoritesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FavoriteListResponse> GetAllAsync()
    {
        var favorites = await _context.Favorites
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return new FavoriteListResponse
        {
            Count = favorites.Count,
            Results = favorites.Select(MapToResult).ToList()
        };
    }

    public async Task<FavoriteResult> AddAsync(FavoriteRequest request)
    {
        var favorite = new Favorite
        {
            Name      = request.Name.Trim(),
            Type      = ParseType(request.Type),
            Gender    = ParseGender(request.Gender),
            Style     = request.Style?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return MapToResult(favorite);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var favorite = await _context.Favorites.FindAsync(id);
        if (favorite is null) return false;

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    private static FavoriteResult MapToResult(Favorite f) => new()
    {
        Id        = f.Id,
        Name      = f.Name,
        Type      = f.Type.ToString().ToLower(),
        Gender    = f.Gender?.ToString().ToLower(),
        Style     = f.Style,
        CreatedAt = f.CreatedAt
    };

    private static GenerationType ParseType(string type) =>
        type.ToLower() switch
        {
            "fictional" => GenerationType.Fictional,
            "username"  => GenerationType.Username,
            _           => GenerationType.Human
        };

    private static Gender? ParseGender(string? gender) =>
        gender?.ToLower() switch
        {
            "male"   => Gender.Male,
            "female" => Gender.Female,
            "neutral" => Gender.Neutral,
            _        => null
        };
}