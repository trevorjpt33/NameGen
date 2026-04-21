using Microsoft.EntityFrameworkCore;
using NameGen.Core.Interfaces;
using NameGen.Core.Models;
using NameGen.Infrastructure.Data;
using System.Text.Json;

namespace NameGen.Infrastructure.Services;

public class GenerationHistoryService : IGenerationHistoryService
{
    private readonly AppDbContext _context;

    public GenerationHistoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(GenerationType type, object parameters, int resultCount)
    {
        var entry = new GenerationHistory
        {
            Type           = type,
            ParametersJson = JsonSerializer.Serialize(parameters),
            ResultsJson    = string.Empty,
            ResultCount    = resultCount,
            CreatedAt      = DateTime.UtcNow
        };

        _context.GenerationHistories.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<GenerationHistoryListResponse> GetAllAsync(string? type, int limit)
    {
        limit = Math.Min(limit, 100);

        var query = _context.GenerationHistories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
        {
            var parsed = ParseType(type);
            query = query.Where(h => h.Type == parsed);
        }

        var records = await query
            .OrderByDescending(h => h.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return new GenerationHistoryListResponse
        {
            Count   = records.Count,
            Results = records.Select(MapToResult).ToList()
        };
    }

    private static GenerationHistoryResult MapToResult(GenerationHistory h) => new()
    {
        Id          = h.Id,
        Type        = h.Type.ToString().ToLower(),
        Parameters  = JsonSerializer.Deserialize<object>(h.ParametersJson),
        ResultCount = h.ResultCount,
        CreatedAt   = h.CreatedAt
    };

    private static GenerationType ParseType(string type) =>
        type.ToLower() switch
        {
            "fictional" => GenerationType.Fictional,
            "username"  => GenerationType.Username,
            _           => GenerationType.Human
        };
}