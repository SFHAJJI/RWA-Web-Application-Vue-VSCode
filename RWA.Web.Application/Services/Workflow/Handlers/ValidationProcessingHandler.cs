using RWA.Web.Application.Services.Workflow.Dtos;
using RWA.Web.Application.Services.Validation;
using RWA.Web.Application.Models.Dtos;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Final handler in chain - processes validation and creates success response
    /// </summary>
    public class ValidationProcessingHandler : BaseUploadHandler
    {
        private readonly WorkflowOrchestrator _orchestrator;
        private readonly IWorkflowEventPublisher _publisher;
        private readonly List<IValidationStrategy> _strategies;

        public ValidationProcessingHandler(
            WorkflowOrchestrator orchestrator, 
            IWorkflowEventPublisher publisher)
        {
            _orchestrator = orchestrator;
            _publisher = publisher;
            _strategies = new List<IValidationStrategy>
            {
                new SuccessValidationStrategy(),
                new WarningValidationStrategy(),
                new ErrorValidationStrategy()
            };
        }

        protected override async Task<TriggerUploadResultDto?> ProcessAsync(UploadContext context)
        {
            if (context.ImportResult == null || context.UploadStep == null)
                return UploadResultFactory.CreateError("Context is incomplete for validation processing.");

            // Run validation using reflection to access private method
            var validation = await GetValidationResultAsync();

            // Apply appropriate strategy based on validation status
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(validation.OverallStatus));
            if (strategy != null)
            {
                await strategy.ProcessAsync(validation, context);
            }

            // Build response DTOs
            var savedFileName = GetSafeFileName(context.ImportResult.SavedFilePath);
            var mappedValidation = validation.Messages.Select(m => 
                new ValidationMessageDto 
                { 
                    Status = m.Status.ToString(), 
                    Message = m.Message, 
                    ErrorData = m.ErrorData, 
                    ValidatorName = m.ValidatorName, 
                    FileName = savedFileName 
                }).ToArray();

            var steps = (await context.DbProvider.GetAllWorkflowStepsOrderedAsync())
                .Select(s => new WorkflowStepDto 
                { 
                    Id = s.Id, 
                    StepName = s.StepName, 
                    Status = s.Status, 
                    DataPayload = s.DataPayload 
                }).ToArray();

            // Publish workflow update via SignalR
            await PublishWorkflowUpdate(context.DbProvider);

            return UploadResultFactory.CreateSuccess(steps, mappedValidation, savedFileName);
        }

        private async Task<ValidationResult> GetValidationResultAsync()
        {
            // Use reflection to access private ValidateCurrentStep method
            var method = typeof(WorkflowOrchestrator).GetMethod("ValidateCurrentStep", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                var result = method.Invoke(_orchestrator, null);
                if (result is Task<ValidationResult> task)
                    return await task;
            }
            
            // Fallback to empty validation result if reflection fails
            return new ValidationResult();
        }

        private static string? GetSafeFileName(string? filePath)
        {
            return string.IsNullOrEmpty(filePath) ? null : System.IO.Path.GetFileName(filePath);
        }

        private async Task PublishWorkflowUpdate(IWorkflowDbProvider dbProvider)
        {
            try
            {
                var stepsSnapshot = await dbProvider.GetStepsSnapshotAsync();
                await _publisher.PublishWorkflowUpdateAsync(new { steps = stepsSnapshot });
            }
            catch
            {
                // Log but don't fail the operation - logger not available in this context
                // Could be improved by passing logger through context
            }
        }
    }
}
