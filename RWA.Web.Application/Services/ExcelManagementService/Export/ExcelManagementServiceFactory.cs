using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace RWA.Web.Application.Services.ExcelManagementService.Export
{
    public class ExcelManagementServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public ExcelManagementServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ExcelManagementService GetExcelManagementService(ImportExportType importExportType)
        {
            switch (importExportType)
            {
                case ImportExportType.BDDHistorique:
                    return new BDDHistoExcelManagementService(_serviceProvider.GetRequiredService<RwaContext>());
                case ImportExportType.MappingCatRWA:
                    return new RWAParamExcelManagementService(_serviceProvider.GetRequiredService<IDbContextFactory<RwaContext>>());
                default:
                    return null;
            }
      
        }
    }
}
