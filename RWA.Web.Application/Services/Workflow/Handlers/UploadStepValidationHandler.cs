using Microsoft.Extensions.Options;
using RWA.Web.Application.Models;
using RWA.Web.Application.Services.Workflow.Dtos;
using System.Threading.Tasks;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Validates upload step exists - second in chain
    /// </summary>
    public class UploadStepValidationHandler : BaseUploadHandler
    {
        private readonly WorkflowStepNamesMapping _workflowStepNames;

        public UploadStepValidationHandler(IOptions<WorkflowStepNamesMapping> workflowStepNames)
        {
            _workflowStepNames = workflowStepNames.Value;
        }

        protected override async Task<TriggerUploadResultDto?> ProcessAsync(UploadContext context)
        {
            context.UploadStep = await context.DbProvider.GetStepByNameAsync(_workflowStepNames.UploadInventory);
            if (context.UploadStep == null)
                return UploadResultFactory.CreateError("Upload step missing.");

            return null; // Continue to next handler
        }
    }
}
