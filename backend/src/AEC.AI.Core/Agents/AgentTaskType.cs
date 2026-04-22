using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AEC.AI.Core.Agents;

    // ── Task types our agents typically fall into ────────────────────────────
// The framework picks the right temperature automatically based on this.
// We can still override with .WithTemperature() if you need fine control.

public enum AgentTaskType
{
    // Precision tasks — must be deterministic and exact
    Routing,        // 0.0  picks agent names, classifies intent
    SqlQuery,       // 0.0  generates SQL — wrong token = broken query
    Extraction,     // 0.1  extracts structured data from text
    Classification, // 0.1  labels, categories, yes/no decisions

    // Balanced tasks — factual but needs natural phrasing
    Research,       // 0.3  finds and summarises facts
    Analysis,       // 0.3  evaluates options, compares data
    QAndA,          // 0.4  answers questions conversationally
    Support,        // 0.5  customer support, helpful tone

    // Creative tasks — variety and originality valued
    Writing,        // 0.7  blog posts, reports, documentation
    Summarisation,  // 0.6  condenses content naturally
    Brainstorm,     // 1.0  generates diverse ideas
    Creative,       // 1.1  stories, marketing copy, poetry
}

// ── Maps each task type to its recommended temperature ────────────────────
public static class AgentTaskTemperature
{
    private static readonly Dictionary<AgentTaskType, float> _map = new()
    {
        [AgentTaskType.Routing]        = 0.0f,
        [AgentTaskType.SqlQuery]       = 0.0f,
        [AgentTaskType.Extraction]     = 0.1f,
        [AgentTaskType.Classification] = 0.1f,
        [AgentTaskType.Research]       = 0.3f,
        [AgentTaskType.Analysis]       = 0.3f,
        [AgentTaskType.QAndA]          = 0.4f,
        [AgentTaskType.Support]        = 0.5f,
        [AgentTaskType.Summarisation]  = 0.6f,
        [AgentTaskType.Writing]        = 0.7f,
        [AgentTaskType.Brainstorm]     = 1.0f,
        [AgentTaskType.Creative]       = 1.1f,
    };

    public static float For(AgentTaskType type) => _map[type];
}