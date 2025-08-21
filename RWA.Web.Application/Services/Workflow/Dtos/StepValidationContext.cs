using RWA.Web.Application.Services.Validation;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Workflow.Dtos
{
    /// <summary>
    /// Context for step validation state machine transitions
    /// </summary>
    public class StepValidationContext
    {
        /// <summary>
        /// The validation result from step validation
        /// </summary>
        public ValidationResult ValidationResult { get; set; } = new ValidationResult();

        /// <summary>
        /// The workflow step entity being validated
        /// </summary>
        public WorkflowStep StepEntity { get; set; } = new WorkflowStep();

        /// <summary>
        /// Whether the validation allows moving to the next step
        /// </summary>
        public bool ShouldAdvanceToNextStep => 
            ValidationResult.OverallStatus == ValidationStatus.Success;

        /// <summary>
        /// Whether there are fluent validators for this step
        /// </summary>
        public bool HasValidators { get; set; }

        /// <summary>
        /// Whether the step has validation warnings
        /// </summary>
        public bool HasWarnings => 
            ValidationResult.OverallStatus == ValidationStatus.Warning;

        /// <summary>
        /// Whether the step has validation errors
        /// </summary>
        public bool HasErrors => 
            ValidationResult.OverallStatus == ValidationStatus.Error;
    }
}
