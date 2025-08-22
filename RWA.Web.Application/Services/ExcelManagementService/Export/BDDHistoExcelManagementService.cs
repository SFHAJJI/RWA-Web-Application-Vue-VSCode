using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style.XmlAccess;

namespace RWA.Web.Application.Services.ExcelManagementService.Export
{
    public class BDDHistoExcelManagementService : ExcelManagementService
    {
        protected RwaContext _context { get; set; }
        public BDDHistoExcelManagementService(RwaContext context)
        {
            _context = context;
        }
        private readonly string excelPath = Path.Combine("RWA Data\\Templates\\HECATE\\Export", "Template BDDHistorique.xlsx");

        public override ExcelPackage CreateExcel(out string fileName)
        {
            fileName = $"BDDHistorique_{DateTime.Now.ToString("dd-MM-yyyy_Hmmss")}";

            var package = new ExcelPackage(excelPath);


            var bddWorksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "BDD");

            if (bddWorksheet != null)
            {
                var hecateInterneHistoriques = _context.HecateInterneHistoriques.AsNoTracking().Select(e => new {
                    e.Source,
                    e.RefCategorieRwa,
                    e.IdentifiantUniqueRetenu,
                    e.Raf,
                    e.LibelleOrigine,
                    e.DateEcheance,
                    e.IdentifiantOrigine,
                    e.Bbgticker,
                    e.LibelleTypeDette
                }).ToList();
                
                // Use LoadFromCollection for bulk writing - much faster
                bddWorksheet.Cells["A2"].LoadFromCollection(hecateInterneHistoriques, false);

                // Apply styles after data is loaded
                var dataRange = bddWorksheet.Cells[2, 1, hecateInterneHistoriques.Count + 1, 9];
                dataRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                dataRange.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                var dateColumn = bddWorksheet.Cells[2, 6, hecateInterneHistoriques.Count + 1, 6];
                dateColumn.Style.Numberformat.Format = "dd/mm/yyyy";

                bddWorksheet.Cells["A1:I1"].AutoFilter = true;
                bddWorksheet.Cells[bddWorksheet.Dimension.Address].AutoFitColumns();
            }
            return package;

        }
    }
}
