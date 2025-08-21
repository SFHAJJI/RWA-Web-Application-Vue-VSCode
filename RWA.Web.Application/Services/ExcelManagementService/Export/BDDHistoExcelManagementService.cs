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


            var hecateInterneHistoriques = _context.HecateInterneHistoriques.ToList();


            ExcelNamedStyleXml ns = package.Workbook.Styles.CreateNamedStyle("BDD");
            ns.Style.Fill.PatternType = ExcelFillStyle.Solid;
            ns.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);


            ExcelNamedStyleXml nsDate = package.Workbook.Styles.CreateNamedStyle("BDDDate");
            nsDate.Style.Fill.PatternType = ExcelFillStyle.Solid;
            nsDate.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            nsDate.Style.Numberformat.Format = "dd/mm/yyyy";
            for (int i = 0; i < hecateInterneHistoriques.Count(); i++)
            {
                bddWorksheet.Cells[i + 2, 1].Value = hecateInterneHistoriques[i].Source;
                bddWorksheet.Cells[i + 2, 1].StyleName = "BDD";
                bddWorksheet.Cells[i + 2, 2].Value = hecateInterneHistoriques[i].RefCategorieRwa;
                bddWorksheet.Cells[i + 2, 2].StyleName = "BDD";
                bddWorksheet.Cells[i + 2, 3].Value = hecateInterneHistoriques[i].IdentifiantUniqueRetenu;
                bddWorksheet.Cells[i + 2, 3].StyleName = "BDD";
                bddWorksheet.Cells[i + 2, 4].Value = hecateInterneHistoriques[i].Raf;
                bddWorksheet.Cells[i + 2, 4].StyleName = "BDD";
                bddWorksheet.Cells[i + 2, 5].Value = hecateInterneHistoriques[i].LibelleOrigine;
                bddWorksheet.Cells[i + 2, 5].StyleName = "BDD";
                bddWorksheet.Cells[i + 2, 6].Value = hecateInterneHistoriques[i].DateEcheance;
                bddWorksheet.Cells[i + 2, 6].StyleName = "BDDDate";
                bddWorksheet.Cells[i + 2, 7].Value = hecateInterneHistoriques[i].IdentifiantOrigine;
                bddWorksheet.Cells[i + 2, 7].StyleName = "BDD";
                bddWorksheet.Cells[i + 2, 8].Value = hecateInterneHistoriques[i].Bbgticker;
                bddWorksheet.Cells[i + 2, 8].StyleName = "BDD";
                bddWorksheet.Cells[i + 2, 9].Value = hecateInterneHistoriques[i].LibelleTypeDette;
                bddWorksheet.Cells[i + 2, 9].StyleName = "BDD";



            }

            bddWorksheet.Cells["A1:I1"].AutoFilter = true;
            bddWorksheet.Column(4).AutoFit();
            return package;

        }
    }
}
