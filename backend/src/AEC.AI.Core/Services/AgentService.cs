using AEC.AI.Core.Agents;
using AEC.AI.Core.Tools;
using AEC.AI.Core.Tools.Core;
using Microsoft.Extensions.AI;

namespace AEC.AI.Core.Services;

// ── Scoped service — builds and runs agents ───────────────────────────────
// Controller only knows this interface. All agent/tool wiring lives here.
// Register as Scoped in DI.

public interface IAgentService
{
    Task<AgentResult> RunAsync(string userMessage, CancellationToken ct = default);
}

public class AgentService(
    IChatClient          client,
    ToolRegistry         registry,
    IScopedToolProvider  scopedTools) : IAgentService
{
    private const string SystemPrompt = """
        You are the AEC Pulse AI assistant.
        - Your primary data source is the Projects database.
        - If a user provides a 36-character GUID, it is always a Project ID.
        - Even if they call it a 'Product ID' or 'Reference ID', treat it as a Project ID.
        - Use get_project_financials immediately when you see any GUID.
        - Do not ask the user for table names or schema details.
        """;

    public async Task<AgentResult> RunAsync(string userMessage, CancellationToken ct = default)
    {
        var agent = new AgentBuilder("AEC-Analyst", client)
            .WithSystemPrompt(SystemPrompt)
            .WithTools(registry.GetAll())            // singleton tools: Calculator, DateTime, MCP
            .WithTools(scopedTools.GetScopedTools()) // scoped tools: ProjectDataTool
            .WithTaskType(AgentTaskType.QAndA)
            .Build();

        return await agent.RunAsync(userMessage, ct);
    }
}