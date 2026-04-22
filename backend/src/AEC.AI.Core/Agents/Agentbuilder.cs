using Microsoft.Extensions.AI;

namespace AEC.AI.Core.Agents;

// ── Fluent builder — makes agent definition readable in Program.cs ────────
//
// Usage:
//   var agent = new AgentBuilder("Researcher", client)
//       .WithSystemPrompt("You are a research assistant...")
//       .WithTool(AIFunctionFactory.Create(MyMethod))
//       .WithTemperature(0.3f)
//       .Build();

public class AgentBuilder(string name, IChatClient client)
{
    private string                    _systemPrompt = $"You are a helpful assistant named {name}.";
    private readonly List<AIFunction> _tools        = [];
    private float                     _temperature  = 0.7f;

    public AgentBuilder WithSystemPrompt(string prompt)
    {
        _systemPrompt = prompt;
        return this;
    }

    public AgentBuilder WithTool(AIFunction tool)
    {
        _tools.Add(tool);
        return this;
    }

    public AgentBuilder WithTools(IEnumerable<AIFunction> tools)
    {
        _tools.AddRange(tools);
        return this;
    }

    /// Manually set a specific temperature value.
    public AgentBuilder WithTemperature(float temperature)
    {
        _temperature = temperature;
        return this;
    }

    /// Let the framework pick the right temperature based on what the agent does.
    /// This is the recommended way — you describe the task, framework sets the number.
    ///
    /// Examples:
    ///   .WithTaskType(AgentTaskType.Routing)     → 0.0 (must be exact)
    ///   .WithTaskType(AgentTaskType.Research)    → 0.3 (factual + natural)
    ///   .WithTaskType(AgentTaskType.Writing)     → 0.7 (varied prose)
    ///   .WithTaskType(AgentTaskType.Brainstorm)  → 1.0 (diverse ideas)
    public AgentBuilder WithTaskType(AgentTaskType taskType)
    {
        _temperature = AgentTaskTemperature.For(taskType);
        return this;
    }

    public Agent Build() => new(
        name:         name,
        systemPrompt: _systemPrompt,
        client:       client,
        tools:        _tools.Count > 0 ? _tools : null,
        temperature:  _temperature
    );
}