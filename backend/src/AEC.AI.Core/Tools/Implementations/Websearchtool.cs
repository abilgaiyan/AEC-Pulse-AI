using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using AEC.AI.Core.Tools.Core;

namespace AEC.AI.Core.Tools.Implementations;

// ── appsettings.json ──────────────────────────────────────────────────────
// "Tools": {
//   "WebSearch": {
//     "ApiKey":   "YOUR_BING_SEARCH_KEY",
//     "Endpoint": "https://api.bing.microsoft.com/v7.0/search",
//     "MaxResults": 5
//   }
// }
//
// Free tier: https://www.microsoft.com/en-us/bing/apis/bing-web-search-api

public class WebSearchTool(IConfiguration config, HttpClient http) : ToolBase
{
    public override string        Name        => "web_search";
    public override string        Description => "Search the web for current information, news, or facts";
    public override IList<string> Tags        => ["search", "web", "api"];

    private readonly string _apiKey     = config["Tools:WebSearch:ApiKey"]   ?? string.Empty;
    private readonly string _endpoint   = config["Tools:WebSearch:Endpoint"]
                                          ?? "https://api.bing.microsoft.com/v7.0/search";
    private readonly int    _maxResults = int.TryParse(config["Tools:WebSearch:MaxResults"], out var n) ? n : 5;

    public override AIFunction GetAIFunction() => AIFunctionFactory.Create(
        SearchAsync,
        name:        Name,
        description: Description
    );

    private async Task<string> SearchAsync(
        [Description("Search query")] string query,
        [Description("Max results to return (1-10)")] int count = 3)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return MockSearch(query);

        try
        {
            count = Math.Clamp(count, 1, _maxResults);
            var url = $"{_endpoint}?q={Uri.EscapeDataString(query)}&count={count}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);

            var response  = await http.SendAsync(request);
            var json      = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var results = doc.RootElement
                             .GetProperty("webPages")
                             .GetProperty("value")
                             .EnumerateArray()
                             .Take(count)
                             .Select(r => new
                             {
                                 Title   = r.GetProperty("name").GetString(),
                                 Url     = r.GetProperty("url").GetString(),
                                 Snippet = r.GetProperty("snippet").GetString()
                             });

            var lines = results.Select(r => $"- {r.Title}\n  {r.Snippet}\n  {r.Url}");
            return $"Search results for \"{query}\":\n\n{string.Join("\n\n", lines)}";
        }
        catch (Exception ex)
        {
            return $"Search failed for '{query}': {ex.Message}";
        }
    }

    private static string MockSearch(string query) =>
        $"[Mock] Search results for \"{query}\":\n" +
        $"- Result 1: Overview of {query} — example.com/1\n" +
        $"- Result 2: Latest news on {query} — example.com/2\n" +
        "Add Tools:WebSearch:ApiKey to appsettings.json for real Bing search.";
}