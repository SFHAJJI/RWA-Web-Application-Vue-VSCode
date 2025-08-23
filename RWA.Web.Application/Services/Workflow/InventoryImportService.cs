using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RWA.Web.Application.Services.ExcelManagementService.Import;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Text.Json;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Workflow
{
    public class InventoryImportService : IInventoryImportService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IWorkflowDbProvider _dbProvider;

        public InventoryImportService(IWebHostEnvironment env, IWorkflowDbProvider dbProvider)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
        }

        public async Task<InventoryImportResult> ImportAsync(InventoryImportResult parsedFile)
        {
            if (parsedFile == null)
            {
                throw new ArgumentNullException(nameof(parsedFile));
            }

            var rows = parsedFile.ParsedTable.AsEnumerable()
                .Select((r, i) =>
                {
                    var ent = new HecateInventaireNormalise();
                    // column-aware mapping: tolerate various column names coming from Excel/CSV
                    string? Col(string[] candidates)
                    {
                        foreach (var c in candidates)
                            if (r.Table.Columns.Contains(c)) return c;
                        return null;
                    }

                    var colIdent = Col(new[] { "Asset ID", "Identifiant", "Identifiant Origine", "IdentifiantOrigine" });
                    var colNom = Col(new[] { "Asset Description", "Nom", "Asset Description" });
                    var colVm = Col(new[] { "Market Value", "ValeurDeMarche", "MarketValue" });
                    var colCat1 = Col(new[] { "Asset Type 1", "Categorie1", "Category1" });
                    var colCat2 = Col(new[] { "Asset Type 2", "Categorie2", "Category2" });
                    var colDev = Col(new[] { "Local Currency", "DeviseDeCotation", "Devise" });
                    var colTaux = Col(new[] { "Obligation Rate", "TauxObligation" });
                    var colMat = Col(new[] { "Maturity Date", "DateMaturite" });
                    var colExp = Col(new[] { "Expiration Date", "DateExpiration" });
                    var colTiers = Col(new[] { "Counterparty", "Tiers" });
                    var colRaf = Col(new[] { "RAF", "RAF", "Raf" });
                    var colSource = Col(new[] { "Source", "Source" });
                    var colDateFinContrat = Col(new[] { "DateFinContrat" });
                    var colBoaSj = Col(new[] { "BOA_SJ" });
                    var colBoaCont = Col(new[] { "BOA_Contrepartie" });
                    var colBoaDef = Col(new[] { "BOA_DEFAUT" });

                    ent.IdentifiantOrigine = colIdent != null && !(r[colIdent] is DBNull) ? r[colIdent].ToString()?.Trim() ?? string.Empty : string.Empty;
                    ent.Identifiant = string.Empty;
                    ent.Nom = colNom != null && !(r[colNom] is DBNull) ? r[colNom].ToString()?.Trim() ?? string.Empty : string.Empty;
                    if (colVm != null && double.TryParse(r[colVm]?.ToString(), out var vm)) ent.ValeurDeMarche = vm;
                    ent.Categorie1 = colCat1 != null ? Convert.ToString(r[colCat1])?.Trim() : null;
                    ent.Categorie2 = colCat2 != null ? Convert.ToString(r[colCat2])?.Trim() : null;

                    var devise = colDev != null && !(r[colDev] is DBNull) ? r[colDev].ToString()?.Trim() : "EUR";
                    ent.DeviseDeCotation = devise?.Length == 3 ? devise : "EUR";

                    var tauxStr = r[colTaux]?.ToString()?.Trim();
                    if (tauxStr == "-") ent.TauxObligation = 0;
                    else if (decimal.TryParse(tauxStr, out var taux)) ent.TauxObligation = taux;

                    var matStr = r[colMat]?.ToString();
                    if (DateOnly.TryParse(matStr, out var dm) && dm.Year != 1900) ent.DateMaturite = dm;

                    var expStr = r[colExp]?.ToString();
                    if (DateOnly.TryParse(expStr, out var de) && de.Year != 1900) ent.DateExpiration = de;

                    ent.Tiers = colTiers != null && !(r[colTiers] is DBNull) ? r[colTiers].ToString()?.Trim() : null;

                    var rafStr = colRaf != null && !(r[colRaf] is DBNull) ? r[colRaf].ToString()?.Trim() : null;
                    if (!string.IsNullOrEmpty(rafStr))
                    {
                        ent.Raf = rafStr.PadLeft(7, '0');
                    }

                    ent.BoaSj = colBoaSj != null && !(r[colBoaSj] is DBNull) ? r[colBoaSj].ToString()?.Trim() : null;
                    ent.BoaContrepartie = colBoaCont != null && !(r[colBoaCont] is DBNull) ? r[colBoaCont].ToString()?.Trim() : null;
                    ent.BoaDefaut = colBoaDef != null && !(r[colBoaDef] is DBNull) ? r[colBoaDef].ToString()?.Trim() : null;

                    ent.PeriodeCloture = colDateFinContrat != null && !(r[colDateFinContrat] is DBNull) ? r[colDateFinContrat].ToString() : null;
                    ent.Source = colSource != null && !(r[colSource] is DBNull) ? r[colSource].ToString() : null;
                    ent.Identifiant = ent.IdentifiantOrigine ?? string.Empty;
                    ent.RefTypeDepot = 0;
                    ent.RefTypeResultat = 0;
                    return ent;
                })
                .Where(ent => ent.ValeurDeMarche < -0.1 || ent.ValeurDeMarche > 0.1)
                .ToList();

            

            var savedCount = await _dbProvider.PersistInventoryRowsAsync(rows);

            return new InventoryImportResult
            {
                Success = true,
                MissingColumns = parsedFile.MissingColumns,
                RowsParsed = parsedFile.RowsParsed,
                RowsSaved = savedCount,
                SavedFilePath = parsedFile.SavedFilePath,
                ParsedTable = parsedFile.ParsedTable,
                ParsedRowsJson = parsedFile.ParsedRowsJson
            };
        }

        public async Task<InventoryImportResult> ParseOnlyAsync(string fileName, byte[] fileContents)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("fileName");
            if (fileContents == null || fileContents.Length == 0) throw new ArgumentException("fileContents");

            var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

            var safeName = Path.GetFileName(fileName);
            var savedPath = Path.Combine(uploadsDir, $"{Guid.NewGuid():N}_{safeName}");
            await File.WriteAllBytesAsync(savedPath, fileContents);

            var sheetNames = new List<string>();
            var tableCollection = ExcelDataContext.ReadFromExcel(savedPath, ref sheetNames);
            if (tableCollection == null || tableCollection.Count == 0)
            {
                return new InventoryImportResult { Success = false, MissingColumns = Array.Empty<string>(), RowsParsed = 0, RowsSaved = 0, SavedFilePath = savedPath };
            }

            var table = tableCollection[0];

            // If the uploaded file name contains metadata (pattern RWA_Report_{3digits}_{MMyyyy}),
            // inject helper columns into the DataTable so downstream mappers can persist them.
            try
            {
                // reuse previously computed safeName
                var m = System.Text.RegularExpressions.Regex.Match(safeName ?? string.Empty, "^RWA_Report_(\\d{3})_(\\d{2})(\\d{4})");
                if (m.Success)
                {
                    var sourceDigits = m.Groups[1].Value;
                    var mm = m.Groups[2].Value;
                    var yyyy = m.Groups[3].Value;
                    var mmYYYY = mm + yyyy;

                    // Ensure Source column exists
                    if (!table.Columns.Contains("Source")) table.Columns.Add("Source", typeof(string));
                    if (!table.Columns.Contains("DateFinContrat")) table.Columns.Add("DateFinContrat", typeof(string));

                    foreach (DataRow r in table.Rows)
                    {
                        r["Source"] = sourceDigits;
                        r["DateFinContrat"] = mmYYYY;
                    }
                }
            }
            catch
            {
                // ignore failures - this is best-effort metadata enrichment
            }

            // Parse-only mode: just create JSON without database persistence
            string parsedJson = "";
            var rows = new List<Dictionary<string, object>>();
            foreach (DataRow r in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn c in table.Columns)
                {
                    dict[c.ColumnName] = r[c] is DBNull ? null : r[c];
                }
                rows.Add(dict);
            }
            parsedJson = JsonSerializer.Serialize(rows);

            return new InventoryImportResult
            {
                Success = true,
                MissingColumns = Array.Empty<string>(),
                RowsParsed = table.Rows.Count,
                RowsSaved = 0, // No persistence in parse-only mode
                SavedFilePath = savedPath,
                ParsedTable = table,
                ParsedRowsJson = parsedJson
            };
        }
    }
}
