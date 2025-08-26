using Microsoft.AspNetCore.Mvc;
using RWA.Web.Application.Services.Workflow;
using System.Threading.Tasks;

namespace RWA.Web.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IWorkflowDbProvider _dbProvider;
        private readonly IExcelExportService _excelExportService;

        public ExportController(IWorkflowDbProvider dbProvider, IExcelExportService excelExportService)
        {
            _dbProvider = dbProvider;
            _excelExportService = excelExportService;
        }

        [HttpGet("inventaire")]
        public async Task<IActionResult> DownloadInventaire()
        {
            var data = await _dbProvider.GetAllInventaireNormaliseAsync();
            var excelBytes = await _excelExportService.ExportToExcelAsync(data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "inventaire.xlsx");
        }
    }
}
