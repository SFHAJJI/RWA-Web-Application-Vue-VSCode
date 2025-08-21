using System.Collections.Generic;
using System.Linq;

namespace RWA.Web.Application.Models
{
    /// <summary>
    /// Configuration record for workflow step status values
    /// Maps to WorkflowStatusMapping section in appsettings.json
    /// </summary>
    public record WorkflowStatusMappingOptions
    {
        public List<string> AdvanceOnSuccessStatuses { get; init; } = new();
        public List<string> AdvanceOnWarningStatuses { get; init; } = new();
        public List<string> ErrorStatuses { get; init; } = new();
        public List<string> WarningStatuses { get; init; } = new();
        public List<string> CurrentStatuses { get; init; } = new();
        public List<string> PendingStatuses { get; init; } = new();

        // Helper properties for easier access to single values
        public string CurrentStatus => CurrentStatuses.FirstOrDefault() ?? "Current";
        public string PendingStatus => PendingStatuses.FirstOrDefault() ?? "Open";
        public string SuccessStatus => AdvanceOnSuccessStatuses.FirstOrDefault() ?? "SuccessfulFinish";
        public string WarningStatus => AdvanceOnWarningStatuses.FirstOrDefault() ?? "FinishedWithWarning";
        public string ErrorStatus => ErrorStatuses.FirstOrDefault() ?? "CurrentWithError";
        public string CurrentWarningStatus => WarningStatuses.FirstOrDefault() ?? "CurrentWithWarning";
        
        // Combined advance statuses for client compatibility
        public List<string> AllAdvanceStatuses => 
            AdvanceOnSuccessStatuses.Concat(AdvanceOnWarningStatuses).ToList();
    }
}
