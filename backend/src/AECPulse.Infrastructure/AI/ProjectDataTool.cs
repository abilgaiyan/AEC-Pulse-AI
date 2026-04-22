
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using AECPulse.Infrastructure.Data;
using AEC.AI.Core.Tools.Core;
using System.ComponentModel;

namespace AECPulse.Infrastructure.AI;

public class ProjectDataTool(AppDbContext context) : ToolBase
{
    public override string Name => "get_project_financials";
    public override string Description => "USE THIS to fetch project financials. Accepts a Project Name, Project Number, or a GUID ID.";
    public override IList<string> Tags => ["data", "finance"];

    public override AIFunction GetAIFunction() => AIFunctionFactory.Create(
        async ([Description("The GUID or name of the project, e.g. 3fa85f64-5717-4562-b3fc-2c963f66afa6 or project name e.g Skyline Tower Expansion")]
            string identifier) =>
        {
            Domain.Entities.Project? project = null;

        // 1. Try to parse as GUID first
        if (Guid.TryParse(identifier, out var guidId))
        {
            project = await context.Projects.FirstOrDefaultAsync(p => p.Id == guidId);
        }

        // 2. If not a GUID or not found by GUID, search by Name
        if (project == null)
        {
            project = await context.Projects
                .FirstOrDefaultAsync(p => p.Name.Contains(identifier) || 
                                         p.ProjectNumber.Contains(identifier));
        }

        if (project == null) 
            return $"I couldn't find a project matching '{identifier}'. Please check the name or ID.";

            var recentTransactions = await context.Transactions
                .Where(t => t.ProjectId == project.Id)
                .OrderByDescending(t => t.CreatedAt)   // add ordering
                .Take(5)
                .ToListAsync();

            return JsonSerializer.Serialize(new
            {
                project.Id,
                project.Name,
                project.TotalBudget,
                project.SpentToDate,
                RecentTransactions = recentTransactions
            });
        },
        name:        Name,
        description: Description
    );
}