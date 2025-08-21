using RWA.Web.Application.Services.Workflow.Dtos;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Handles import result validation - first in chain
    /// </summary>
    public class ImportValidationHandler : BaseUploadHandler
    {
        protected override async Task<TriggerUploadResultDto?> ProcessAsync(UploadContext context)
        {
            await Task.CompletedTask; // Keep async signature for consistency

            if (context.ImportResult == null)
                return UploadResultFactory.CreateError("Import service returned null result.");

            if (!context.ImportResult.Success)
                return UploadResultFactory.CreateError(
                    "Import failed or no sheets found.",
                    savedFile: GetSafeFileName(context.ImportResult.SavedFilePath));

            if (context.ImportResult.MissingColumns != null && context.ImportResult.MissingColumns.Length > 0)
                return UploadResultFactory.CreateError(
                    "Uploaded file does not match expected template.",
                    context.ImportResult.MissingColumns,
                    GetSafeFileName(context.ImportResult.SavedFilePath));

            return null; // Continue to next handler
        }

        private static string? GetSafeFileName(string? filePath)
        {
            return string.IsNullOrEmpty(filePath) ? null : System.IO.Path.GetFileName(filePath);
        }
    }
}
