using Microsoft.AspNetCore.Mvc;
using NameGen.Core.Interfaces;
using NameGen.Core.Models;

namespace NameGen.API.Controllers;

/// <summary>
/// Handles name generation requests.
/// </summary>
[ApiController]
[Route("api/v1/names")]
public class NamesController : ControllerBase
{
    private readonly IHumanNameService _humanNameService;
    private readonly IFictionalNameService _fictionalNameService;
    private readonly IUsernameService _usernameService;
    private readonly IGenerationHistoryService _historyService;

    /// <summary>
    /// Initializes a new instance of <see cref="NamesController"/>.
    /// </summary>
    /// <param name="humanNameService">The human name generation service.</param>
    /// <param name="fictionalNameService">The fictional name generation service.</param>
    /// <param name="usernameService">The gaming username generation service.</param>
    /// <param name="historyService">The generation history logging service.</param>
    public NamesController(
        IHumanNameService humanNameService,
        IFictionalNameService fictionalNameService,
        IUsernameService usernameService,
        IGenerationHistoryService historyService)
    {
        _humanNameService = humanNameService;
        _fictionalNameService = fictionalNameService;
        _usernameService = usernameService;
        _historyService = historyService;
    }

    /// <summary>
    /// Generates human names from the SSA and Census datasets.
    /// </summary>
    /// <param name="request">Filter parameters for name generation.</param>
    /// <returns>A list of generated names matching the specified filters.</returns>
    [HttpGet("human")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHumanNames([FromQuery] HumanNameRequest request)
    {
        var response = await _humanNameService.GenerateAsync(request);
        await _historyService.LogAsync(GenerationType.Human, request, response.Count);
        return Ok(response);
    }

    /// <summary>
    /// Generates fictional character names using syllable chaining and style presets.
    /// </summary>
    /// <param name="request">Filter parameters for fictional name generation.</param>
    /// <returns>A list of generated fictional names matching the specified filters.</returns>
    [HttpGet("fictional")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFictionalNames([FromQuery] FictionalNameRequest request)
    {
        var response = await _fictionalNameService.GenerateAsync(request);
        await _historyService.LogAsync(GenerationType.Fictional, request, response.Count);
        return Ok(response);
    }

    /// <summary>
    /// Generates gaming usernames using style presets.
    /// </summary>
    /// <param name="request">Filter parameters for username generation.</param>
    /// <returns>A list of generated usernames matching the specified filters.</returns>
    [HttpGet("username")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsernames([FromQuery] UsernameRequest request)
    {
        var response = await _usernameService.GenerateAsync(request);
        await _historyService.LogAsync(GenerationType.Username, request, response.Count);
        return Ok(response);
    }
}