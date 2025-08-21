using System.Runtime.CompilerServices;
using OfficeOpenXml;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.ExcelManagementService.Import
{
    public interface IExcelImportManagementService
    {
        Task<HECATESettingViewModel> ImportExcel(IFormFile file);
    }
}
