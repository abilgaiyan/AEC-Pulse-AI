using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AECPulse.Domain.Common;

namespace AECPulse.Domain.Entities
{
   public class ConversationLog : BaseEntity
    {
        public string UserIdentity { get; set; } = "Anonymous_Dev";
        public string UserPrompt { get; set; } = string.Empty;
        public string AgentResponse { get; set; } = string.Empty;
        public string ModelUsed { get; set; } = string.Empty; // e.g. "gpt-4o"

        public long?  InputTokens { get; set; }
        public long? OutputTokens { get; set; }
    }
}