using System.Runtime.CompilerServices;
using OfficeOpenXml;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.ExcelManagementService.Export
{
    public abstract class ExcelManagementService
    {
       
        public abstract ExcelPackage CreateExcel(out string fileName);
    }
}
