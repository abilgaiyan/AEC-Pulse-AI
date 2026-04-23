using AEC.AI.Core.Services;
using AECPulse.Infrastructure.Data;
using AECPulse.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AECPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(
    IAgentService agentService,
    AppDbContext  db) : ControllerBase
{
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request, CancellationToken ct)
    {
        var result = await agentService.RunAsync(request.Message, ct);

        db.ConversationLogs.Add(new ConversationLog
        {
            UserPrompt    = request.Message,
            AgentResponse = result.Output,
            InputTokens   = result.InputTokens,
            OutputTokens  = result.OutputTokens,
            ModelUsed     = result.ModelId ?? "unknown"
        });
        await db.SaveChangesAsync(ct);

        return Ok(result);
    }
}

public record ChatRequest(string Message);
