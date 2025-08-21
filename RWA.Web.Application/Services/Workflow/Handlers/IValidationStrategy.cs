using RWA.Web.Application.Services.Workflow.Dtos;
using RWA.Web.Application.Services.Validation;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Strategy for processing validation results
    /// </summary>
    public interface IValidationStrategy
    {
        bool CanHandle(ValidationStatus status);
        Task<bool> ProcessAsync(ValidationResult validation, UploadContext context);
    }
}
