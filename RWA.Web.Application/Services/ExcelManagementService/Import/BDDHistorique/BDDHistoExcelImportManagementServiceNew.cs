using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Data;
using System.Text.RegularExpressions;

namespace RWA.Web.Application.Services.ExcelManagementService.Import.BDDHistorique
{
    public class BDDHistoExcelImportManagementServiceNew : IExcelImportManagementService
    {
        private readonly string process = "IMPORT BDD HISTORIQUE";
        private readonly string Date = DateTime.Now.ToString("dd-MM-yyyy_Hmmss");
        private readonly string GENERAL_ERROR_SQL = "ERREUR FICHIER EXCEL: ";

        private readonly string IMPORT_PATH = "RWA Data\\Templates\\HECATE\\Import";
        private readonly string NULL_IMPORT_FILE = "Fichier non trouvé";
        private readonly string NoBDDHistoWorkbook = "L'onglet BDD est vide";
        private readonly string SUCESSFULL_IMPORT = "Fichier importé avec succès";
        private List<ImportResult> ImportResults = new List<ImportResult>();
        protected RwaContext _context { get; set; }
        private readonly ExcelColumnMappings _columnMappings;

        public BDDHistoExcelImportManagementServiceNew(RwaContext context, IOptions<ExcelColumnMappings> columnMappings)
        {
            _context = context;
            _columnMappings = columnMappings.Value;
        }
        public async Task<HECATESettingViewModel> ImportExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return GetHECATESettingViewModel(false, NULL_IMPORT_FILE);
                }
                string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(IMPORT_PATH, fileName);
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                var sheetnames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetnames);
                var importResults = await ImportBDDHistoFromDatatableCollection(tables);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return new HECATESettingViewModel() { ImportResults = importResults };
            }
            catch (Exception ex)
            {
                return GetHECATESettingViewModel(false, $"{GENERAL_ERROR_SQL}{ex.Message} {ex.InnerException?.ToString()}");

            }
           

        }
        private async Task<List<ImportResult>> ImportBDDHistoFromDatatableCollection(DataTableCollection tables)
        {


            var bddDt = tables["BDD"];
            try
            {
                if (bddDt == null || bddDt.Rows.Count < 1)
                {

                    return GetImportResults(false, NoBDDHistoWorkbook);
                }
                if (_context.HecateInterneHistoriques.Any())
                {
                    await _context.HecateInterneHistoriques.ExecuteDeleteAsync();
                }

                if (TryGetInterneHistoriques(bddDt, out List<HecateInterneHistorique> hecateInterneHistorique))
                {
                    _context.HecateInterneHistoriques.AddRange(hecateInterneHistorique);
                }

            }
            catch (Exception ex)
            {
                ImportResults.AddRange(GetImportResults(false, $"{GENERAL_ERROR_SQL}{ex.Message} {ex.InnerException?.ToString()}"));
                return ImportResults;
            }


            // Use an explicit transaction to commit all changes as a batch.
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ImportResults.AddRange(GetImportResults(false, $"{GENERAL_ERROR_SQL}{ex.Message} {ex.InnerException?.ToString()}"));
                    return ImportResults;
                }
            }
            ImportResults.AddRange(GetImportResults(true, SUCESSFULL_IMPORT));
            return ImportResults;

        }
        private bool TryGetInterneHistoriques(DataTable? bddHistoDt, out List<HecateInterneHistorique> value)
        {
            try
            {
                var columnMap = CreateColumnMap(bddHistoDt, _columnMappings.BDDHistorique);
                // ⚡ ULTRA-FAST: AsParallel() optimization for row processing
                List<HecateInterneHistorique> bddHisto = bddHistoDt.AsEnumerable()
                    .AsParallel()
                    .Select(row => new HecateInterneHistorique
                    {
                        Source = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.Source)]) ?? string.Empty,
                        RefCategorieRwa = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.CategorieRWA)]) ?? string.Empty,
                        IdentifiantUniqueRetenu = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.IdentifiantUniqueRetenu)]) ?? string.Empty,
                        Raf = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.RAF)]) ?? string.Empty,
                        LibelleOrigine = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.LibelleOrigine)]) ?? string.Empty,
                        DateEcheance = TryParseDateOnly(row.Field<object>(columnMap[nameof(_columnMappings.BDDHistorique.DateEcheance)]) ?? null),
                        IdentifiantOrigine = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.IdentifiantOrigine)]) ?? string.Empty,
                        Bbgticker = string.Empty, // Not in worksheet columns provided
                        LibelleTypeDette = "SUBORDONNE", // Always set to 'SUBORDONNE' as requested
                        LastUpdate = DateTime.Now.ToString("dd-MM-yyyy_Hmmss")
                    }).ToList();
                value = bddHisto;
            }
            catch (Exception ex)
            {
                value = Enumerable.Empty<HecateInterneHistorique>().ToList();
                throw;

            }
            return true;

        }
        private static DateOnly? TryParseDateOnly(object? dateValue)
        {
            if (dateValue == null) return null;

            if (DateTime.TryParse(dateValue.ToString(), out DateTime result))
                return DateOnly.FromDateTime(result);

            return null;
        }
        private Dictionary<string, string> CreateColumnMap<T>(DataTable dt, T mapping)
        {
            var map = new Dictionary<string, string>();
            var properties = typeof(T).GetProperties();
            var dtColumns = dt.Columns.Cast<DataColumn>().ToList();

            foreach (var prop in properties)
            {
                var expectedName = (string)prop.GetValue(mapping)!;
                var normalizedExpectedName = NormalizeString(expectedName);

                var matchedColumn = dtColumns.FirstOrDefault(c => NormalizeString(c.ColumnName) == normalizedExpectedName);

                if (matchedColumn != null)
                {
                    map[prop.Name] = matchedColumn.ColumnName;
                }
                else
                {
                    Console.WriteLine($"⚠️  Column mapping not found for '{expectedName}'");
                }
            }
            return map;
        }

        private string NormalizeString(string input)
        {
            // Remove all non-alphanumeric characters and convert to lower case
            return Regex.Replace(input, "[^a-zA-Z0-9]", "").ToLower();
        }
        private List<ImportResult> GetImportResults(bool IsSuccessful = false, params string[] args)
        {
            var importResults = new List<ImportResult>();
            foreach (var arg in args)
            {
                var importResult = new ImportResult()
                {
                    Date = Date,
                    Success = IsSuccessful,
                    Process = process,
                    Message = arg.ToString(),
                };
                importResults.Add(importResult);

            }
            return importResults;
        }
        private HECATESettingViewModel GetHECATESettingViewModel(bool IsSuccessful = false, params string[] args)
        {
            var resultViewModel = new HECATESettingViewModel();
            var importResults = new List<ImportResult>();
            foreach (var arg in args)
            {
                var importResult = new ImportResult()
                {
                    Date = Date,
                    Success = IsSuccessful,
                    Process = process,
                    Message = arg.ToString(),
                };
                importResults.Add(importResult);

            }
            resultViewModel.ImportResults = importResults;
            return resultViewModel;
        }
    }
}
