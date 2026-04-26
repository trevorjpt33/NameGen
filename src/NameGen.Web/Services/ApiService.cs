using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NameGen.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    // -------------------------------------------------------------------------
    // Human Names
    // -------------------------------------------------------------------------

    public async Task<HumanNameApiResponse?> GetHumanNamesAsync(
        string type = "full",
        string gender = "neutral",
        int count = 5,
        string weighted = "none")
    {
        var query = BuildQuery(new Dictionary<string, string?>
        {
            ["type"]     = type,
            ["gender"]   = gender,
            ["count"]    = count.ToString(),
            ["weighted"] = weighted
        });

        return await _http.GetFromJsonAsync<HumanNameApiResponse>(
            $"api/v1/names/human{query}", JsonOptions);
    }

    // -------------------------------------------------------------------------
    // Fictional Names
    // -------------------------------------------------------------------------

    public async Task<FictionalNameApiResponse?> GetFictionalNamesAsync(
        string style = "random",
        string type = "full",
        int count = 5,
        string weighted = "none")
    {
        var query = BuildQuery(new Dictionary<string, string?>
        {
            ["style"]    = style,
            ["type"]     = type,
            ["count"]    = count.ToString(),
            ["weighted"] = weighted
        });

        return await _http.GetFromJsonAsync<FictionalNameApiResponse>(
            $"api/v1/names/fictional{query}", JsonOptions);
    }

    // -------------------------------------------------------------------------
    // Usernames
    // -------------------------------------------------------------------------

    public async Task<UsernameApiResponse?> GetUsernamesAsync(
        string style = "random",
        int count = 5,
        string weighted = "none",
        bool allowNumbers = true,
        bool allowSymbols = false)
    {
        var query = BuildQuery(new Dictionary<string, string?>
        {
            ["style"]        = style,
            ["count"]        = count.ToString(),
            ["weighted"]     = weighted,
            ["allowNumbers"] = allowNumbers.ToString().ToLower(),
            ["allowSymbols"] = allowSymbols.ToString().ToLower()
        });

        return await _http.GetFromJsonAsync<UsernameApiResponse>(
            $"api/v1/names/username{query}", JsonOptions);
    }

    // -------------------------------------------------------------------------
    // Favorites
    // -------------------------------------------------------------------------

    public async Task<FavoriteListApiResponse?> GetFavoritesAsync()
    {
        return await _http.GetFromJsonAsync<FavoriteListApiResponse>(
            "api/v1/favorites", JsonOptions);
    }

    public async Task<bool> SaveFavoriteAsync(string name, string type,
        string? gender = null, string? style = null)
    {
        var payload = new
        {
            name,
            type,
            gender,
            style
        };

        var response = await _http.PostAsJsonAsync("api/v1/favorites", payload, JsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteFavoriteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/v1/favorites/{id}");
        return response.IsSuccessStatusCode;
    }

    // -------------------------------------------------------------------------
    // History
    // -------------------------------------------------------------------------

    public async Task<GenerationHistoryApiResponse?> GetHistoryAsync(int limit = 20)
    {
        return await _http.GetFromJsonAsync<GenerationHistoryApiResponse>(
            $"api/v1/history?limit={limit}", JsonOptions);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static string BuildQuery(Dictionary<string, string?> parameters)
    {
        var sb = new StringBuilder();
        foreach (var kvp in parameters)
        {
            if (kvp.Value is null) continue;
            sb.Append(sb.Length == 0 ? "?" : "&");
            sb.Append($"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}");
        }
        return sb.ToString();
    }
}

// =============================================================================
// Local response models — mirrors API contracts in NameGen.Core
// =============================================================================

public class HumanNameApiResponse
{
    public int Count { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public List<HumanNameApiResult> Results { get; set; } = new();
    public string? Warning { get; set; }
}

public class HumanNameApiResult
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Gender { get; set; }
}

public class FictionalNameApiResponse
{
    public int Count { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public List<FictionalNameApiResult> Results { get; set; } = new();
    public string? Warning { get; set; }
}

public class FictionalNameApiResult
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Style { get; set; }
}

public class UsernameApiResponse
{
    public int Count { get; set; }
    public string Style { get; set; } = string.Empty;
    public List<UsernameApiResult> Results { get; set; } = new();
    public string? Warning { get; set; }
}

public class UsernameApiResult
{
    public string Username { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
}

public class FavoriteListApiResponse
{
    public int Count { get; set; }
    public List<FavoriteApiResult> Results { get; set; } = new();
}

public class FavoriteApiResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? Style { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GenerationHistoryApiResponse
{
    public int Count { get; set; }
    public List<GenerationHistoryApiResult> Results { get; set; } = new();
}

public class GenerationHistoryApiResult
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int ResultCount { get; set; }
    public DateTime CreatedAt { get; set; }
}