using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AEC.AI.Core.Agents;
using AEC.AI.Core.Tools.Core;
using AEC.AI.Core.Tools.Implementations;

namespace AEC.AI.Core.Tools;

// ── DI registration — call from Program.cs ────────────────────────────────
//
//   services.AddTools(config);
//
// Registers all built-in tools. Comment out any you don't want.
// Add your own with .AddTool<MyCustomTool>() on the returned builder.

public static class ToolsServiceExtensions
{
    public static ToolRegistryBuilder AddTools(
        this IServiceCollection services,
        IConfiguration config)
    {
        // HttpClient for tools that make outbound HTTP calls
        services.AddHttpClient<WeatherTool>();
        services.AddHttpClient<WebSearchTool>();
        services.AddHttpClient<HttpApiTool>();

        return services
            .AddToolRegistry()
            .AddTool<WeatherTool>()
            .AddTool<WebSearchTool>()
            .AddTool<DatabaseTool>()
            .AddTool<HttpApiTool>()
            .AddTool<DateTimeTool>()
            .AddTool<CalculatorTool>();
    }
}

// ── AgentBuilder extensions for ToolRegistry ─────────────────────────────

public static class AgentBuilderToolExtensions
{
    /// Give the agent ALL registered tools
    public static AgentBuilder WithTools(this AgentBuilder builder, ToolRegistry registry)
        => builder.WithTools(registry.GetAll());

    /// Give the agent only tools matching a tag — e.g. "search", "data", "utility"
    public static AgentBuilder WithToolsByTag(
        this AgentBuilder builder,
        ToolRegistry registry,
        string tag)
        => builder.WithTools(registry.GetByTag(tag));

    /// Give the agent only specific named tools
    public static AgentBuilder WithToolsByName(
        this AgentBuilder builder,
        ToolRegistry registry,
        params string[] names)
        => builder.WithTools(registry.GetByName(names));
}