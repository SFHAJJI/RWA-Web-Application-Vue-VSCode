using System.Data;
using System.Threading.Tasks;

namespace RWA.Web.Application.Services.Workflow
{
    public class InventoryImportResult
    {
        public bool Success { get; set; }
        public string[] MissingColumns { get; set; }
        public int RowsParsed { get; set; }
        public int RowsSaved { get; set; }
        public string SavedFilePath { get; set; }
        public DataTable ParsedTable { get; set; }
        public string ParsedRowsJson { get; set; }
        public string FileName { get; set; }
    }

    public interface IInventoryImportService
    {
        /// <summary>
        /// Save uploaded file to wwwroot/uploads, parse using ExcelDataContext.ReadFromExcel and return a result containing parsed DataTable and any missing columns.
        /// This method does both parsing AND database persistence.
        /// </summary>
        Task<InventoryImportResult> ImportAsync(InventoryImportResult parsedFile);

        /// <summary>
        /// Parse uploaded file for validation purposes only - no database persistence.
        /// Returns parsed DataTable and JSON for validation without saving to database.
        /// </summary>
        Task<InventoryImportResult> ParseOnlyAsync(string fileName, byte[] fileContents);
    }
}
