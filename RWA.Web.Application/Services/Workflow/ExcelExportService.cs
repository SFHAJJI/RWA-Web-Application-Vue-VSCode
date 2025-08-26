using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Workflow
{
    public class ExcelExportService : IExcelExportService
    {
        public async Task<byte[]> ExportToExcelAsync(IEnumerable<HecateInventaireNormalise> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Inventaire");

                // Define headers
                var headers = new List<string>
                {
                    "Période", "Source", "N° de ligne", "Identifiant", "Nom", "Valeur de marché",
                    "Catégorie 1", "Catégorie 2", "Devise de cotation", "Taux obligation", "Date maturité",
                    "Date expiration", "Tiers", "RAF", "BOA SJ", "BOA Contrepartie", "BOA Défaut",
                    "Identifiant origine", "Réf. catégorie RWA", "Identifiant unique retenu", "RAF enrichi",
                    "Libellé origine", "Date fin contrat", "Bloomberg", "Réf. type dépôt",
                     "Code résultat"
                };

                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Add data
                var row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.PeriodeCloture;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2].Value = item.Source;
                    worksheet.Cells[row, 3].Value = item.NumLigne;
                    worksheet.Cells[row, 4].Value = item.Identifiant;
                    worksheet.Cells[row, 5].Value = item.Nom;
                    worksheet.Cells[row, 6].Value = item.ValeurDeMarche;
                    worksheet.Cells[row, 7].Value = item.Categorie1;
                    worksheet.Cells[row, 8].Value = item.Categorie2;
                    worksheet.Cells[row, 9].Value = item.DeviseDeCotation;
                    worksheet.Cells[row, 10].Value = item.TauxObligation;
                    worksheet.Cells[row, 11].Value = item.DateMaturite?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 12].Value = item.DateExpiration?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 13].Value = item.Tiers;
                    worksheet.Cells[row, 14].Value = item.Raf;
                    worksheet.Cells[row, 15].Value = item.BoaSj;
                    worksheet.Cells[row, 16].Value = item.BoaContrepartie;
                    worksheet.Cells[row, 17].Value = item.BoaDefaut;
                    worksheet.Cells[row, 18].Value = item.IdentifiantOrigine;
                    worksheet.Cells[row, 19].Value = item.RefCategorieRwa;
                    worksheet.Cells[row, 20].Value = item.IdentifiantUniqueRetenu;
                    worksheet.Cells[row, 21].Value = item.Rafenrichi;
                    worksheet.Cells[row, 22].Value = item.LibelleOrigine;
                    worksheet.Cells[row, 23].Value = item.DateFinContrat?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 24].Value = item.Bloomberg;
                    worksheet.Cells[row, 25].Value = item.RefTypeDepot;
                    worksheet.Cells[row, 26].Value = item.RefTypeResultat;
                    worksheet.Cells[row, 27].Value = item.CodeResultat;
                    row++;
                }

                return await package.GetAsByteArrayAsync();
            }
        }
    }
}
