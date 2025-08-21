using RWA.Web.Application.Services.Workflow.Dtos;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RWA.Web.Application.Services.Workflow.Handlers
{
    /// <summary>
    /// Processes file annotations using Strategy pattern - third in chain
    /// </summary>
    public class FileAnnotationHandler : BaseUploadHandler
    {
        protected override async Task<TriggerUploadResultDto?> ProcessAsync(UploadContext context)
        {
            await Task.CompletedTask; // Keep async signature for consistency

            if (context.ImportResult == null || context.UploadStep == null)
                return null; // Continue to next handler

            var annotatedPayload = AnnotateFileData(
                context.ImportResult.ParsedRowsJson,
                context.ImportResult.SavedFilePath,
                context.FileName);

            context.UploadStep.DataPayload = annotatedPayload ?? "{}";
            context.UploadStep.UpdatedAt = DateTime.UtcNow;
            await context.DbProvider.SaveChangesAsync();

            return null; // Continue to next handler (success processing)
        }

        private static string? AnnotateFileData(string? parsedJson, string? savedFilePath, string originalFileName)
        {
            if (string.IsNullOrEmpty(parsedJson) || string.IsNullOrEmpty(savedFilePath))
                return parsedJson;

            try
            {
                var safeFileName = Path.GetFileName(savedFilePath);
                var rows = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(parsedJson);
                
                if (rows == null) return parsedJson;

                var fileMetadata = ExtractFileMetadata(safeFileName);
                
                foreach (var row in rows)
                {
                    AddMetadataToRow(row, safeFileName, originalFileName, fileMetadata);
                }

                return JsonSerializer.Serialize(rows);
            }
            catch
            {
                return parsedJson; // Fallback to original on annotation failure
            }
        }

        private static FileMetadata ExtractFileMetadata(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return new FileMetadata();

            // Pattern: RWA_Report_{3digits}_{MMyyyy}
            var match = Regex.Match(fileName, @"^RWA_Report_(\d{3})_(\d{2})(\d{4})");
            if (!match.Success)
                return new FileMetadata();

            return new FileMetadata
            {
                SourceDigits = match.Groups[1].Value,
                MonthYear = match.Groups[2].Value + match.Groups[3].Value // MMyyyy
            };
        }

        private static void AddMetadataToRow(Dictionary<string, object> row, string? safeFileName, string originalFileName, FileMetadata metadata)
        {
            row["__SavedFileName"] = safeFileName ?? string.Empty;
            row["__OriginalFileName"] = originalFileName;
            
            if (!string.IsNullOrEmpty(metadata.SourceDigits))
                row["Source"] = metadata.SourceDigits;
                
            if (!string.IsNullOrEmpty(metadata.MonthYear))
                row["DateFinContrat"] = metadata.MonthYear;
        }

        private class FileMetadata
        {
            public string? SourceDigits { get; set; }
            public string? MonthYear { get; set; }
        }
    }
}
