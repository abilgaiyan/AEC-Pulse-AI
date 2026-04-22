using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AECPulse.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}