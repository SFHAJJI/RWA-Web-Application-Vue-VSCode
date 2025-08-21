using RWA.Web.Application.Services.Workflow.Dtos;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Validates upload step exists - second in chain
    /// </summary>
    public class UploadStepValidationHandler : BaseUploadHandler
    {
        protected override async Task<TriggerUploadResultDto?> ProcessAsync(UploadContext context)
        {
            context.UploadStep = await context.DbProvider.GetStepByNameAsync("Upload inventory");
            if (context.UploadStep == null)
                return UploadResultFactory.CreateError("Upload step missing.");

            return null; // Continue to next handler
        }
    }
}
