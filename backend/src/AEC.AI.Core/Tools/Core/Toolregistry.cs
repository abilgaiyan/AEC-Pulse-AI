using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AEC.AI.Core.Tools.Core;

// ── Central registry — inject this anywhere you need tools ────────────────
//
//   // Register all tools → DI
//   services.AddToolRegistry()
//           .AddTool<WeatherTool>()
//           .AddTool<DatabaseTool>()
//           .AddTool<WebSearchTool>();
//
//   // In an agent:
//   var agent = new AgentBuilder("Assistant", client)
//       .WithTools(registry.GetAll())         // all tools
//       .WithTools(registry.GetByTag("data")) // only data tools
//       .Build();

public class ToolRegistry(ILogger<ToolRegistry> logger)
{
    private readonly Dictionary<string, ITool> _tools = new(StringComparer.OrdinalIgnoreCase);

    // ── Registration ──────────────────────────────────────────────────────

    public ToolRegistry Register(ITool tool)
    {
        if (_tools.ContainsKey(tool.Name))
            logger.LogWarning("Tool '{Name}' already registered — overwriting.", tool.Name);

        _tools[tool.Name] = tool;
        logger.LogInformation("Tool registered: {Name} [{Tags}]",
            tool.Name, string.Join(", ", tool.Tags));
        return this;
    }

    // ── Retrieval ─────────────────────────────────────────────────────────

    /// All tools as AIFunction[] — pass directly to AgentBuilder.WithTools()
    public IEnumerable<AIFunction> GetAll()
        => _tools.Values.Select(t => t.GetAIFunction());

    /// Filter by tag — e.g. GetByTag("search"), GetByTag("data")
    public IEnumerable<AIFunction> GetByTag(string tag)
        => _tools.Values
                 .Where(t => t.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                 .Select(t => t.GetAIFunction());

    /// Filter by name — e.g. GetByName("weather", "web_search")
    public IEnumerable<AIFunction> GetByName(params string[] names)
        => names.Where(_tools.ContainsKey)
                .Select(n => _tools[n].GetAIFunction());

    /// All registered tool names — useful for debugging
    public IEnumerable<string> GetRegisteredNames() => _tools.Keys;

    public int Count => _tools.Count;
}

// ── DI extension ──────────────────────────────────────────────────────────
public static class ToolRegistryExtensions
{
    /// Registers the ToolRegistry as a singleton and returns a builder
    /// so you can chain .AddTool<T>() calls fluently.
    public static ToolRegistryBuilder AddToolRegistry(this IServiceCollection services)
    {
        services.AddSingleton<ToolRegistry>();
        return new ToolRegistryBuilder(services);
    }
}

public class ToolRegistryBuilder(IServiceCollection services)
{
    /// Registers a tool as both its concrete type and ITool (for auto-discovery)
    public ToolRegistryBuilder AddTool<T>() where T : class, ITool
    {
        services.AddSingleton<T>();
        services.AddSingleton<ITool>(sp => sp.GetRequiredService<T>());
        return this;
    }
}

// ── Auto-population extension on ToolRegistry itself ─────────────────────
// Call registry.PopulateFromDI(serviceProvider) after building the container
// to auto-register every ITool that was added via AddTool<T>()
public static class ToolRegistryPopulationExtensions
{
    public static ToolRegistry PopulateFromDI(
        this ToolRegistry registry, IServiceProvider sp)
    {
        foreach (var tool in sp.GetServices<ITool>())
            registry.Register(tool);
        return registry;
    }
}