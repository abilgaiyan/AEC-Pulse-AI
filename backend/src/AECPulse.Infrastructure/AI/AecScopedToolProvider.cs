using AEC.AI.Core.Tools.Core;
using AECPulse.Infrastructure.AI;
using AECPulse.Infrastructure.Data;
using Microsoft.Extensions.AI;
 
namespace AECPulse.Infrastructure.AI;
 
// ── Lives in Infrastructure — the only place that knows about AppDbContext ─
// Registered as Scoped so it gets a fresh DbContext each request.
 
public class AecScopedToolProvider(AppDbContext context) : IScopedToolProvider
{
    public IEnumerable<AIFunction> GetScopedTools()
    {
        // Add more scoped tools here as your app grows
        yield return new ProjectDataTool(context).GetAIFunction();
    }
}
 