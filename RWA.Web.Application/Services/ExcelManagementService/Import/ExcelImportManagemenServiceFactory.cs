using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using RWA.Web.Application.Services.ExcelManagementService.Import.CatRWA;
using RWA.Web.Application.Services.ExcelManagementService.Import.BDDHistorique;
using Microsoft.Extensions.DependencyInjection;

namespace RWA.Web.Application.Services.ExcelManagementService.Import
{
    public class ExcelImportManagemenServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public ExcelImportManagemenServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IExcelImportManagementService GetExcelManagementService(ImportExportType importExportType)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RwaContext>();
                switch (importExportType)
                {
                    case ImportExportType.BDDHistorique:
                        return new BDDHistoExcelImportManagementServiceNew(context);
                    case ImportExportType.MappingCatRWA:
                        return new EqCatRWAExcelImportManagementServiceNew(context);
                    default:
                        return null;
                }
            }
        }
    }
}
