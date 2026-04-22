using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using AECPulse.Infrastructure.Data;
using AECPulse.Infrastructure.AI;
using AEC.AI.Core.Tools.Core;
using AEC.AI.Core.Tools;
using AEC.AI.Core.Agents;
using AECPulse.Domain.Entities;

namespace AECPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatClient client, ToolRegistry registry, AppDbContext db) : ControllerBase
{
    [HttpPost("ask")]
public async Task<IActionResult> Ask([FromBody] ChatRequest request)
{
    // ProjectDataTool created here — same scope as AppDbContext (db)
    var projectTool = new ProjectDataTool(db).GetAIFunction();

    var agent = new AgentBuilder("AEC-Analyst", client)
        .WithSystemPrompt(@"You are the AEC Pulse AI.
            - Use get_project_financials whenever you see a GUID or project ID.
            - Even if the user says 'product ID' or 'reference ID', treat it as a Project ID.
            - Do not ask for schema or table names.")
        .WithTools(registry.GetAll())   // CalculatorTool, DateTimeTool, MCP tools etc
        .WithTool(projectTool)          // DB tool added per-request
        .WithTaskType(AgentTaskType.QAndA)
        .Build();

    var result = await agent.RunAsync(request.Message);

    db.ConversationLogs.Add(new ConversationLog {
        UserPrompt    = request.Message,
        AgentResponse = result.Output,
        InputTokens   = result.InputTokens,
        OutputTokens  = result.OutputTokens,
        ModelUsed     = result.ModelId ?? "unknown"
    });
    await db.SaveChangesAsync();

    return Ok(result);
}
}

public record ChatRequest(string Message);