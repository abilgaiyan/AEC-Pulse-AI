using Microsoft.Extensions.AI;

namespace AEC.AI.Core.Tools.Core;

// ── Abstraction for tools that need scoped dependencies (e.g. DbContext) ─
// Implement this in infrastructure layer (AECPulse.Infrastructure).
// The AI core layer never references AppDbContext or any EF types directly.
//
// Register in DI as Scoped:
//   builder.Services.AddScoped<IScopedToolProvider, AecScopedToolProvider>();

public interface IScopedToolProvider
{
    /// Returns AIFunctions that are safe to use within the current request scope.
    /// Called once per request by AgentService before building the agent.
    IEnumerable<AIFunction> GetScopedTools();
}
