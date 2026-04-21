using NameGen.Core.Models;

namespace NameGen.Core.Interfaces;

public interface IFavoritesService
{
    Task<FavoriteListResponse> GetAllAsync();
    Task<FavoriteResult> AddAsync(FavoriteRequest request);
    Task<bool> DeleteAsync(int id);
}