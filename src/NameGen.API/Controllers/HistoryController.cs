using Microsoft.AspNetCore.Mvc;
using NameGen.Core.Interfaces;

namespace NameGen.API.Controllers;

/// <summary>
/// Handles retrieval of name generation history.
/// </summary>
[ApiController]
[Route("api/v1/history")]
public class HistoryController : ControllerBase
{
    private readonly IGenerationHistoryService _historyService;

    /// <summary>
    /// Initializes a new instance of <see cref="HistoryController"/>.
    /// </summary>
    /// <param name="historyService">The generation history service.</param>
    public HistoryController(IGenerationHistoryService historyService)
    {
        _historyService = historyService;
    }

    /// <summary>
    /// Retrieves generation history in reverse chronological order.
    /// </summary>
    /// <param name="type">Optional filter by generation type: human, fictional, username.</param>
    /// <param name="limit">Maximum number of records to return (default 20, max 100).</param>
    /// <returns>A list of generation history records.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(
        [FromQuery] string? type = null,
        [FromQuery] int limit = 20)
    {
        var response = await _historyService.GetAllAsync(type, limit);
        return Ok(response);
    }
}