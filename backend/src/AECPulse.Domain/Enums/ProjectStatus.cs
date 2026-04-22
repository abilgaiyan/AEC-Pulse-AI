using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AECPulse.Domain.Enums
{
    public enum ProjectStatus
    {
        Active,
        OnHold,
        Completed,
        AtRisk // The AI will help move projects into this status
    }
}