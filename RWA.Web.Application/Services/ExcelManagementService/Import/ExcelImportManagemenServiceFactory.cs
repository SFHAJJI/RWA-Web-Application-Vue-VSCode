using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using RWA.Web.Application.Services.ExcelManagementService.Import.CatRWA;
using RWA.Web.Application.Services.ExcelManagementService.Import.BDDHistorique;

namespace RWA.Web.Application.Services.ExcelManagementService.Import
{
    public class ExcelImportManagemenServiceFactory
    {
        private RwaContext _context { get; set; }
        public ExcelImportManagemenServiceFactory(RwaContext context)
        {
            _context = context;
        }
        public IExcelImportManagementService GetExcelManagementService(ImportExportType importExportType)
        {
            switch (importExportType)
            {
                case ImportExportType.BDDHistorique:
                    return new BDDHistoExcelImportManagementServiceNew(_context);
                case ImportExportType.MappingCatRWA:
                    return new EqCatRWAExcelImportManagementServiceNew(_context);
                default:
                    return null;
            }
      
        }
    }
}
