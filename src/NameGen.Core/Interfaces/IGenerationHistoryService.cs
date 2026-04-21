using NameGen.Core.Models;

namespace NameGen.Core.Interfaces;

public interface IGenerationHistoryService
{
    Task LogAsync(GenerationType type, object parameters, int resultCount);
    Task<GenerationHistoryListResponse> GetAllAsync(string? type, int limit);
}