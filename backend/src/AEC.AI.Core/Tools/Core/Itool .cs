using Microsoft.Extensions.AI;

namespace AEC.AI.Core.Tools.Core;

// ── Every tool implements this ────────────────────────────────────────────
// Name        — unique key, used for lookup and logging
// Description — shown to the LLM so it knows when to call this tool
// Tags        — lets filter tools per agent ("search", "data", "api")
// GetAIFunction() — produces the AIFunction the MEAI pipeline actually calls
public interface ITool
{
    string       Name        { get; }
    string       Description { get; }
    IList<string> Tags       { get; }

    AIFunction GetAIFunction();
}

// ── Base class — reduces boilerplate in concrete tools ────────────────────
public abstract class ToolBase : ITool
{
    public abstract string        Name        { get; }
    public abstract string        Description { get; }
    public virtual  IList<string> Tags        => [];

    public abstract AIFunction GetAIFunction();
}