using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;

namespace RWA.Web.Application.Services.ExcelManagementService.Export
{
    public class ExcelManagementServiceFactory
    {
        private RwaContext _context { get; set; }
        public ExcelManagementServiceFactory(RwaContext context)
        {
            _context = context;
        }
        public ExcelManagementService GetExcelManagementService(ImportExportType importExportType)
        {
            switch (importExportType)
            {
                case ImportExportType.BDDHistorique:
                    return new BDDHistoExcelManagementService(_context);
                case ImportExportType.MappingCatRWA:
                    return new RWAParamExcelManagementService(_context);
                default:
                    return null;
            }
      
        }
    }
}
