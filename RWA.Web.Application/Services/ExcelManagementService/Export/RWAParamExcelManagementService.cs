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
        protected RwaContext _context { get; set; }
        private readonly string excelPath = Path.Combine("RWA Data\\Templates\\HECATE\\Export", "Template Parametrage RWA.xlsx");
        public RWAParamExcelManagementService(RwaContext context)
        {
            _context = context;
        }
        public override ExcelPackage CreateExcel(out string fileName)
        {
            fileName = $"Parametrage RWA_{DateTime.Now.ToString("dd-MM-yyyy_Hmmss")}";

            var package = new ExcelPackage(excelPath);



            var hecateCategorieRwas = _context.HecateCategorieRwas.ToList();
            var hecateTypeBloombergs = _context.HecateTypeBloombergs.ToList();
            var hecateEquivalenceCatRwas = _context.HecateEquivalenceCatRwas.Include(s=> s.RefCatDepositaire2Navigation).Include(s=>s.RefCatDepositaire1Navigation).ToList();

            var categorieRWAWorksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "CategorieRWA");
            var typeBloombergworksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "TypeBloomberg");
            var equivalenceCatRWAworksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "EquivalenceCatRWA");


            ExcelNamedStyleXml ns = package.Workbook.Styles.CreateNamedStyle("ORANGE");
            ns.Style.Fill.PatternType = ExcelFillStyle.Solid;
            ns.Style.Fill.BackgroundColor.SetColor(Color.LightGoldenrodYellow);

            for (int i = 0; i < hecateCategorieRwas.Count(); i++)
            {
                categorieRWAWorksheet.Cells[i + 2, 1].Value = hecateCategorieRwas[i].IdCatRwa;
                categorieRWAWorksheet.Cells[i + 2, 1].StyleName = "ORANGE";
                categorieRWAWorksheet.Cells[i + 2, 2].Value = hecateCategorieRwas[i].Libelle;
                categorieRWAWorksheet.Cells[i + 2, 2].StyleName = "ORANGE";
                categorieRWAWorksheet.Cells[i + 2, 3].Value = hecateCategorieRwas[i].ValeurMobiliere;
                categorieRWAWorksheet.Cells[i + 2, 3].StyleName = "ORANGE";
            }


            for (int i = 0; i < hecateTypeBloombergs.Count(); i++)
            {
                typeBloombergworksheet.Cells[i + 2, 1].Value = hecateTypeBloombergs[i].IdTypeBloomberg;
                typeBloombergworksheet.Cells[i + 2, 1].StyleName = "ORANGE";
                typeBloombergworksheet.Cells[i + 2, 2].Value = hecateTypeBloombergs[i].Libelle;
                typeBloombergworksheet.Cells[i + 2, 2].StyleName = "ORANGE";
            }

            for (int i = 0; i < hecateEquivalenceCatRwas.Count(); i++)
            {
                equivalenceCatRWAworksheet.Cells[i + 2, 1].Value = hecateEquivalenceCatRwas[i].Source;

                equivalenceCatRWAworksheet.Cells[i + 2, 1].StyleName = "ORANGE";
                equivalenceCatRWAworksheet.Cells[i + 2, 2].Value = hecateEquivalenceCatRwas[i].RefCatDepositaire1Navigation.LibelleDepositaire1;
                equivalenceCatRWAworksheet.Cells[i + 2, 2].StyleName = "ORANGE";
                equivalenceCatRWAworksheet.Cells[i + 2, 3].Value = hecateEquivalenceCatRwas[i].RefCatDepositaire2Navigation.LibelleDepositaire2 != "NONE" ? hecateEquivalenceCatRwas[i].RefCatDepositaire2Navigation.LibelleDepositaire2 : string.Empty;
                equivalenceCatRWAworksheet.Cells[i + 2, 3].StyleName = "ORANGE";
                equivalenceCatRWAworksheet.Cells[i + 2, 4].Value = hecateEquivalenceCatRwas[i].RefCategorieRwa;
                equivalenceCatRWAworksheet.Cells[i + 2, 4].StyleName = "ORANGE";
                equivalenceCatRWAworksheet.Cells[i + 2, 5].Value = hecateEquivalenceCatRwas[i].RefTypeBloomberg;
                equivalenceCatRWAworksheet.Cells[i + 2, 5].StyleName = "ORANGE";
            }
            categorieRWAWorksheet.Cells[categorieRWAWorksheet.Dimension.Address].AutoFitColumns();
            typeBloombergworksheet.Cells[typeBloombergworksheet.Dimension.Address].AutoFitColumns();
            equivalenceCatRWAworksheet.Cells[equivalenceCatRWAworksheet.Dimension.Address].AutoFitColumns();



            return package;

        }
    }
}
