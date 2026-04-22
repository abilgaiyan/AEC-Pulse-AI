using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AECPulse.Domain.Common;
using AECPulse.Domain.Enums;

namespace AECPulse.Domain.Entities;

public class Project : BaseEntity
{
    public string ProjectNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal TotalBudget { get; set; }
    public decimal LaborBudget { get; set; }
    public decimal SpentToDate { get; set; }
    public ProjectStatus Status { get; set; }

    // Logic to calculate health - AI will use this later
    public decimal MarginPercentage => TotalBudget > 0 
        ? ((TotalBudget - SpentToDate) / TotalBudget) * 100 
        : 0;
}

