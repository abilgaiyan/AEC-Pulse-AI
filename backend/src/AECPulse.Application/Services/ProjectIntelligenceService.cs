using Microsoft.Extensions.AI;
using AECPulse.Domain.Entities;
using AECPulse.Infrastructure.Data;
using AEC.AI.Core.Agents;
using AEC.AI.Core.Tools.Core;
using AEC.AI.Core.Tools;

namespace AECPulse.Application.Services;

public class ProjectIntelligenceService
{
    private readonly IChatClient _chatClient;
    private readonly ToolRegistry _registry;
    private readonly AppDbContext _db;

    public ProjectIntelligenceService(IChatClient chatClient, ToolRegistry registry, AppDbContext db)
    {
        _chatClient = chatClient;
        _registry = registry;
        _db = db;
    }

    public async Task<string> RunAECAnalysisAsync(string userInput)
    {
        var analyst = new AgentBuilder("AEC-Analyst", _chatClient)
            .WithSystemPrompt("Analyze project health. Use the database tool for facts.")
            .WithToolsByTag(_registry, "data")
            .Build();

        var result = await analyst.RunAsync(userInput);

        var log = new ConversationLog
        {
            UserPrompt = userInput,
            AgentResponse = result.Output,
            InputTokens = result.InputTokens,
            OutputTokens = result.OutputTokens
        };

        var modelName = GetUsedModelName(result);
        var modelProperty = typeof(ConversationLog).GetProperty("ModelUsed")
            ?? typeof(ConversationLog).GetProperty("Model")
            ?? typeof(ConversationLog).GetProperty("ModelName");

        modelProperty?.SetValue(log, modelName);

        _db.ConversationLogs.Add(log);
        await _db.SaveChangesAsync();

        return result.Output;
    }

    private static string GetUsedModelName(object result)
    {
        if (result == null)
            return "unknown";

        foreach (var propertyName in new[] { "Model", "ModelId", "ModelUsed", "ModelName", "Engine" })
        {
            var prop = result.GetType().GetProperty(propertyName);
            if (prop == null)
                continue;

            var value = prop.GetValue(result);
            if (value is string str && !string.IsNullOrWhiteSpace(str))
                return str;

            if (value != null)
                return value.ToString();
        }

        return "unknown";
    }
}