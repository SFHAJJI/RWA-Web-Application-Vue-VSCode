using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style.XmlAccess;

namespace RWA.Web.Application.Services.ExcelManagementService.Export
{
    public class RWAParamExcelManagementService : ExcelManagementService
    {
        private readonly IDbContextFactory<RwaContext> _contextFactory;
        private readonly string excelPath = Path.Combine("RWA Data\\Templates\\HECATE\\Export", "Template Parametrage RWA.xlsx");
        public RWAParamExcelManagementService(IDbContextFactory<RwaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public override ExcelPackage CreateExcel(out string fileName)
        {
            fileName = $"Parametrage RWA_{DateTime.Now.ToString("dd-MM-yyyy_Hmmss")}";

            var package = new ExcelPackage(excelPath);



            var categorieRWAWorksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "CategorieRWA");
            var typeBloombergworksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "TypeBloomberg");
            var equivalenceCatRWAworksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "EquivalenceCatRWA");

            ExcelNamedStyleXml ns = package.Workbook.Styles.CreateNamedStyle("ORANGE");
            ns.Style.Fill.PatternType = ExcelFillStyle.Solid;
            ns.Style.Fill.BackgroundColor.SetColor(Color.LightGoldenrodYellow);

            Parallel.Invoke(
                () => {
                    using var context = _contextFactory.CreateDbContext();
                    var hecateCategorieRwas = context.HecateCategorieRwas.AsNoTracking().ToList();
                    if (categorieRWAWorksheet != null)
                    {
                        categorieRWAWorksheet.Cells["A2"].LoadFromCollection(hecateCategorieRwas, false);
                        var dataRange = categorieRWAWorksheet.Cells[2, 1, hecateCategorieRwas.Count + 1, 3];
                        dataRange.StyleName = "ORANGE";
                        categorieRWAWorksheet.Cells[categorieRWAWorksheet.Dimension.Address].AutoFitColumns();
                    }
                },
                () => {
                    using var context = _contextFactory.CreateDbContext();
                    var hecateTypeBloombergs = context.HecateTypeBloombergs.AsNoTracking().ToList();
                    if (typeBloombergworksheet != null)
                    {
                        typeBloombergworksheet.Cells["A2"].LoadFromCollection(hecateTypeBloombergs, false);
                        var dataRange = typeBloombergworksheet.Cells[2, 1, hecateTypeBloombergs.Count + 1, 2];
                        dataRange.StyleName = "ORANGE";
                        typeBloombergworksheet.Cells[typeBloombergworksheet.Dimension.Address].AutoFitColumns();
                    }
                },
                () => {
                    using var context = _contextFactory.CreateDbContext();
                    var hecateEquivalenceCatRwas = context.HecateEquivalenceCatRwas.AsNoTracking().Include(s => s.RefCatDepositaire2Navigation).Include(s => s.RefCatDepositaire1Navigation).ToList();
                    if (equivalenceCatRWAworksheet != null)
                    {
                        var exportData = hecateEquivalenceCatRwas.Select(e => new
                        {
                            e.Source,
                            LibelleDepositaire1 = e.RefCatDepositaire1Navigation.LibelleDepositaire1,
                            LibelleDepositaire2 = e.RefCatDepositaire2Navigation.LibelleDepositaire2 != "NONE" ? e.RefCatDepositaire2Navigation.LibelleDepositaire2 : string.Empty,
                            e.RefCategorieRwa,
                            e.RefTypeBloomberg
                        }).ToList();

                        equivalenceCatRWAworksheet.Cells["A2"].LoadFromCollection(exportData, false);
                        var dataRange = equivalenceCatRWAworksheet.Cells[2, 1, hecateEquivalenceCatRwas.Count + 1, 5];
                        dataRange.StyleName = "ORANGE";
                        equivalenceCatRWAworksheet.Cells[equivalenceCatRWAworksheet.Dimension.Address].AutoFitColumns();
                    }
                }
            );



            return package;

        }
    }
}
