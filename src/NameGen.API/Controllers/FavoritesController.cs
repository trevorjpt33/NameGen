using Microsoft.AspNetCore.Mvc;
using NameGen.Core.Interfaces;
using NameGen.Core.Models;

namespace NameGen.API.Controllers;

/// <summary>
/// Handles saving, retrieving, and deleting favorite generated names.
/// </summary>
[ApiController]
[Route("api/v1/favorites")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoritesService _favoritesService;

    /// <summary>
    /// Initializes a new instance of <see cref="FavoritesController"/>.
    /// </summary>
    /// <param name="favoritesService">The favorites persistence service.</param>
    public FavoritesController(IFavoritesService favoritesService)
    {
        _favoritesService = favoritesService;
    }

    /// <summary>
    /// Retrieves all saved favorites in reverse chronological order.
    /// </summary>
    /// <returns>A list of all saved favorites.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _favoritesService.GetAllAsync();
        return Ok(response);
    }

    /// <summary>
    /// Saves a new favorite name.
    /// </summary>
    /// <param name="request">The name and metadata to save.</param>
    /// <returns>The saved favorite with its assigned ID.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] FavoriteRequest request)
    {
        var result = await _favoritesService.AddAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    /// <summary>
    /// Removes a favorite by ID.
    /// </summary>
    /// <param name="id">The ID of the favorite to remove.</param>
    /// <returns>204 No Content on success, 404 Not Found if the ID does not exist.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _favoritesService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}