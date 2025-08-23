using RWA.Web.Application.Services.Validation;
using System.Collections.Generic;

namespace RWA.Web.Application.Services.Workflow.Dtos
{
    /// <summary>
    /// Context for validation results to be passed between state machine transitions
    /// </summary>
    public class ValidationResultContext
    {
        public ValidationResult ValidationResult { get; set; } = null!;
        public string StepName { get; set; } = string.Empty;
        public List<InventoryImportResult> ParsedFiles { get; set; } = new List<InventoryImportResult>();
        public bool ShouldAdvance => ValidationResult.OverallStatus == ValidationStatus.Success;
        public bool HasWarnings => ValidationResult.OverallStatus == ValidationStatus.Warning;
        public bool HasErrors => ValidationResult.OverallStatus == ValidationStatus.Error;
    }
}
