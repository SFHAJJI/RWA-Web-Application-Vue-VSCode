using System.Collections.Generic;
using System.Threading.Tasks;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Workflow
{
    public interface IExcelExportService
    {
        Task<byte[]> ExportToExcelAsync(IEnumerable<HecateInventaireNormalise> data);
    }
}
