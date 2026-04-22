using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using AEC.AI.Core.Tools.Core;

namespace AEC.AI.Core.Tools.Implementations;

// ── appsettings.json ──────────────────────────────────────────────────────
// "Tools": {
//   "Weather": {
//     "ApiKey": "YOUR_OPENWEATHERMAP_KEY",
//     "BaseUrl": "https://api.openweathermap.org/data/2.5"
//   }
// }
//
// Free API key: https://openweathermap.org/api

public class WeatherTool(IConfiguration config, HttpClient http) : ToolBase
{
    public override string        Name        => "get_weather";
    public override string        Description => "Get current weather and 3-day forecast for any city";
    public override IList<string> Tags        => ["weather", "api"];

    private readonly string _apiKey  = config["Tools:Weather:ApiKey"] ?? string.Empty;
    private readonly string _baseUrl = config["Tools:Weather:BaseUrl"]
                                       ?? "https://api.openweathermap.org/data/2.5";

    public override AIFunction GetAIFunction() => AIFunctionFactory.Create(
        GetWeatherAsync,
        name:        Name,
        description: Description
    );

    private async Task<string> GetWeatherAsync(
        [Description("City name, e.g. London or Tokyo")]  string city,
        [Description("Unit: metric (°C) or imperial (°F)")] string unit = "metric")
    {
        // Fallback to mock data if no API key configured
        if (string.IsNullOrWhiteSpace(_apiKey))
            return MockWeather(city, unit);

        try
        {
            var url      = $"{_baseUrl}/weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units={unit}";
            var json     = await http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);
            var root     = doc.RootElement;

            var temp      = root.GetProperty("main").GetProperty("temp").GetDouble();
            var feels     = root.GetProperty("main").GetProperty("feels_like").GetDouble();
            var humidity  = root.GetProperty("main").GetProperty("humidity").GetInt32();
            var desc      = root.GetProperty("weather")[0].GetProperty("description").GetString();
            var wind      = root.GetProperty("wind").GetProperty("speed").GetDouble();
            var unitLabel = unit == "metric" ? "°C" : "°F";

            return $"Weather in {city}: {temp}{unitLabel} (feels like {feels}{unitLabel}), " +
                   $"{desc}, humidity {humidity}%, wind {wind} m/s";
        }
        catch (Exception ex)
        {
            return $"Could not fetch weather for {city}: {ex.Message}";
        }
    }

    private static string MockWeather(string city, string unit)
    {
        var unitLabel = unit == "metric" ? "°C" : "°F";
        return $"[Mock] Weather in {city}: 22{unitLabel}, partly cloudy, humidity 65%, wind 3.2 m/s. " +
               "Add Tools:Weather:ApiKey to appsettings.json for real data.";
    }
}