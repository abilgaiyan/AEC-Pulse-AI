using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AECPulse.Domain.Common;
using AECPulse.Domain.Enums;

namespace AECPulse.Domain.Entities;

public class FinancialTransaction : BaseEntity
{
    public Guid ProjectId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public DateTime TransactionDate { get; set; }
}

