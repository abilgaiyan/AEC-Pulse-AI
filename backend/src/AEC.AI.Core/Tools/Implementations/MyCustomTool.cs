using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using AEC.AI.Core.Tools.Core;

namespace AEC.AI.Core.Tools.Implementations;
public class MyCustomTool(IConfiguration config) : ToolBase
{
    public override string        Name        => "my_tool";
    public override string        Description => "What this tool does — LLM reads this";
    public override IList<string> Tags        => ["custom"];

    public override AIFunction GetAIFunction() => AIFunctionFactory.Create(
        DoSomethingAsync,
        name: Name, description: Description
    );

    private async Task<string> DoSomethingAsync(
        [Description("Input parameter")] string input)
    {
        // your logic here
        return $"Result for {input}";
    }
}