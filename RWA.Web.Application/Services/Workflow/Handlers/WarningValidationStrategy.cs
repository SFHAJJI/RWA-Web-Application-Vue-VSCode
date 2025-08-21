using RWA.Web.Application.Services.Workflow.Dtos;
using RWA.Web.Application.Services.Validation;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Strategy for handling validation warnings - stops processing, renders to client
    /// </summary>
    public class WarningValidationStrategy : IValidationStrategy
    {
        public bool CanHandle(ValidationStatus status) => status == ValidationStatus.Warning;

        public async Task<bool> ProcessAsync(ValidationResult validation, UploadContext context)
        {
            context.Logger.LogWarning("Validation returned warnings, stopping database operations. Rendering results to client.");
            
            await Task.CompletedTask; // No database operations for warnings
            
            return false; // Stop processing
        }
    }
}
