using System.ComponentModel;
using Microsoft.Extensions.AI;
using AEC.AI.Core.Tools.Core;

namespace AEC.AI.Core.Tools.Implementations;

// ── Date/Time tool ────────────────────────────────────────────────────────
// LLMs don't know the current time — this gives them a reliable clock.
// No config or API key needed.

public class DateTimeTool : ToolBase
{
    public override string        Name        => "get_datetime";
    public override string        Description => "Get the current date, time, or timezone information";
    public override IList<string> Tags        => ["utility", "datetime"];

    public override AIFunction GetAIFunction() => AIFunctionFactory.Create(
        GetDateTime,
        name:        Name,
        description: Description
    );

    private static string GetDateTime(
        [Description("Timezone ID, e.g. 'UTC', 'Asia/Kolkata', 'America/New_York'")] string timezone = "UTC")
    {
        try
        {
            var tz   = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var now  = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz);
            return $"Current date/time in {timezone}: {now:dddd, dd MMMM yyyy HH:mm:ss zzz}";
        }
        catch
        {
            var now = DateTimeOffset.UtcNow;
            return $"Current UTC date/time: {now:dddd, dd MMMM yyyy HH:mm:ss} UTC";
        }
    }
}

// ── Calculator tool ───────────────────────────────────────────────────────
// Prevents hallucinated arithmetic — LLMs can confidently get maths wrong.
// No config needed.

public class CalculatorTool : ToolBase
{
    public override string        Name        => "calculate";
    public override string        Description => "Evaluate a mathematical expression and return the result";
    public override IList<string> Tags        => ["utility", "math"];

    public override AIFunction GetAIFunction() => AIFunctionFactory.Create(
        Calculate,
        name:        Name,
        description: Description
    );

    private static string Calculate(
        [Description("A math expression, e.g. '(15 * 8) / 3 + 12'")] string expression)
    {
        try
        {
            // Use DataTable for safe expression evaluation — no eval(), no code execution
            var dt     = new System.Data.DataTable();
            var result = dt.Compute(expression, null);
            return $"{expression} = {result}";
        }
        catch (Exception ex)
        {
            return $"Could not evaluate '{expression}': {ex.Message}";
        }
    }
}