using System.ComponentModel;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using AEC.AI.Core.Tools.Core;

namespace AEC.AI.Core.Tools.Implementations;

// ── Generic HTTP tool for any REST API ────────────────────────────────────
// Configure named APIs in appsettings.json:
//
// "Tools": {
//   "HttpApis": {
//     "github": {
//       "BaseUrl": "https://api.github.com",
//       "Headers": { "Authorization": "Bearer TOKEN", "User-Agent": "myAIApp" }
//     },
//     "stripe": {
//       "BaseUrl": "https://api.stripe.com/v1",
//       "Headers": { "Authorization": "Bearer sk_..." }
//     }
//   }
// }

public class HttpApiTool(IConfiguration config, HttpClient http) : ToolBase
{
    public override string        Name        => "http_api_call";
    public override string        Description => "Call any configured REST API by name. Returns JSON response.";
    public override IList<string> Tags        => ["api", "http", "rest"];

    public override AIFunction GetAIFunction() => AIFunctionFactory.Create(
        CallApiAsync,
        name:        Name,
        description: Description
    );

    private async Task<string> CallApiAsync(
        [Description("API name as configured in appsettings (e.g. 'github', 'stripe')")] string apiName,
        [Description("Path to append to base URL (e.g. '/users/octocat')")] string path,
        [Description("HTTP method: GET, POST, PUT, DELETE")] string method = "GET",
        [Description("JSON body for POST/PUT requests (optional)")] string? body = null)
    {
        var apiConfig = config.GetSection($"Tools:HttpApis:{apiName}");
        if (!apiConfig.Exists())
            return $"API '{apiName}' not found in appsettings.json Tools:HttpApis section.";

        var baseUrl = apiConfig["BaseUrl"] ?? string.Empty;
        var url     = baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');

        try
        {
            var request = new HttpRequestMessage(new HttpMethod(method.ToUpperInvariant()), url);

            // Apply configured headers (auth, user-agent, etc.)
            var headers = apiConfig.GetSection("Headers").GetChildren();
            foreach (var header in headers)
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);

            // Attach body for POST/PUT
            if (body is not null && method is "POST" or "PUT" or "PATCH")
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await http.SendAsync(request);
            var content  = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"HTTP {(int)response.StatusCode} from {apiName}: {content}";

            // Pretty-print JSON if possible
            try
            {
                using var doc = JsonDocument.Parse(content);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return content;
            }
        }
        catch (Exception ex)
        {
            return $"Request to {apiName} failed: {ex.Message}";
        }
    }
}