using RWA.Web.Application.Services.Validation;

namespace RWA.Web.Application.Services.Workflow.Dtos
{
    /// <summary>
    /// Context for upload operation results to be passed between state machine transitions
    /// </summary>
    public class UploadResultContext
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public InventoryImportResult? ImportResult { get; set; }
        public ValidationResult? ValidationResult { get; set; }
        public string? ErrorMessage { get; set; }
        public string[]? ErrorData { get; set; }
        public bool IsSuccess => ImportResult?.Success == true && string.IsNullOrEmpty(ErrorMessage);
    }
}
