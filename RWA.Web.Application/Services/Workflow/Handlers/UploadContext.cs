using RWA.Web.Application.Services.Workflow;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Context object for upload operations
    /// </summary>
    public class UploadContext
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public InventoryImportResult? ImportResult { get; set; }
        public Models.WorkflowStep? UploadStep { get; set; }
        public IWorkflowDbProvider DbProvider { get; set; } = null!;
        public ILogger Logger { get; set; } = null!;
    }
}
