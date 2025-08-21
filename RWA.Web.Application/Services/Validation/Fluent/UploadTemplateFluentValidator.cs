using FluentValidation;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Validation.Fluent
{
    [RWA.Web.Application.Services.Validation.SupportedWorkflowStep("Upload inventory")]
    public class UploadTemplateFluentValidator : AbstractValidator<WorkflowStep>
    {
        public UploadTemplateFluentValidator()
        {
            RuleFor(x => x.DataPayload).NotEmpty().WithMessage("Upload payload must not be empty");
            // Additional header checks are handled in MandatoryColumns validator
        }
    }
}
