using RWA.Web.Application.Services.Workflow.Dtos;
using RWA.Web.Application.Services.Workflow;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Handles upload result processing using Chain of Responsibility pattern
    /// </summary>
    public interface IUploadResultHandler
    {
        IUploadResultHandler? NextHandler { get; set; }
        Task<TriggerUploadResultDto?> HandleAsync(UploadContext context);
    }
}
