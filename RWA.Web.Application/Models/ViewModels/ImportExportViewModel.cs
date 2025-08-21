
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace RWA.Web.Application.Models
{
    public class ImportExportViewModel
    {
        public static Dictionary<string, ImportExportType> Dict = new Dictionary<string, ImportExportType>();

        public ImportExportViewModel()
        {

        }
        public ImportExportViewModel(string title, ImportExportType importExportType, string tooltipMessage)
        {
            Title = title;
            ImportExportType = importExportType;
            TooltipMessage = tooltipMessage;
            if (!Dict.ContainsKey(tooltipMessage))
            {
                Dict.Add(tooltipMessage, ImportExportType);
            }
        }

        public string Title { get; set; }
        // The uploaded Excel file (.xlsx)
        public IFormFile FileUpload { get; set; }
        // Message for overall import status
        public string TooltipMessage { get; set; }
        // Detailed results of processing each product row

        public ImportExportType ImportExportType;

    }
}
