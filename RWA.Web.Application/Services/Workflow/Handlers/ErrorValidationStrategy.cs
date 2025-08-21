using RWA.Web.Application.Services.Workflow.Dtos;
using RWA.Web.Application.Services.Validation;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Strategy for handling validation errors - stops processing, renders to client
    /// </summary>
    public class ErrorValidationStrategy : IValidationStrategy
    {
        public bool CanHandle(ValidationStatus status) => status == ValidationStatus.Error;

        public async Task<bool> ProcessAsync(ValidationResult validation, UploadContext context)
        {
            context.Logger.LogError("Validation failed with errors, stopping database operations. Rendering results to client.");
            
            await Task.CompletedTask; // No database operations for errors
            
            return false; // Stop processing
        }
    }
}
