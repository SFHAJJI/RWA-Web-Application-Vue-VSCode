using RWA.Web.Application.Services.Workflow.Dtos;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Base handler for Chain of Responsibility pattern
    /// </summary>
    public abstract class BaseUploadHandler : IUploadResultHandler
    {
        public IUploadResultHandler? NextHandler { get; set; }

        public async Task<TriggerUploadResultDto?> HandleAsync(UploadContext context)
        {
            var result = await ProcessAsync(context);
            if (result != null)
                return result;

            return NextHandler != null ? await NextHandler.HandleAsync(context) : null;
        }

        protected abstract Task<TriggerUploadResultDto?> ProcessAsync(UploadContext context);
    }
}
