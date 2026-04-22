using System;
using System.Collections.Generic;
using System.Linq;
using AECPulse.Domain.Entities;

namespace AECPulse.Application.Interfaces;

public interface IAIService
{
    // The "Reasoning" method
    Task<string> AnalyzeProjectHealthAsync(Project project, IEnumerable<FinancialTransaction> history);
    
    // The "Natural Language" method
    Task<string> GetFinancialInsightAsync(string userQuestion);
}