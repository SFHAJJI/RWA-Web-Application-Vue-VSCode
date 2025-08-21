using RWA.Web.Application.Services.Workflow.Dtos;
using RWA.Web.Application.Models.Dtos;
using RWA.Web.Application.Services.Validation;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Factory for creating error responses using Factory Pattern
    /// </summary>
    public static class UploadResultFactory
    {
        public static TriggerUploadResultDto CreateError(string message, string[]? errorData = null, string? savedFile = null)
        {
            return new TriggerUploadResultDto
            {
                Steps = Array.Empty<WorkflowStepDto>(),
                Validation = new[] { 
                    new ValidationMessageDto 
                    { 
                        Status = ValidationStatus.Error.ToString(), 
                        Message = message,
                        ErrorData = errorData
                    } 
                },
                SavedFile = savedFile
            };
        }

        public static TriggerUploadResultDto CreateSuccess(
            WorkflowStepDto[] steps, 
            ValidationMessageDto[] validation, 
            string? savedFile = null)
        {
            return new TriggerUploadResultDto
            {
                Steps = steps,
                Validation = validation,
                SavedFile = savedFile
            };
        }
    }
}
