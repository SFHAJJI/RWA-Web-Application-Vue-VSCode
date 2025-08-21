using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Data;

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
        public BDDHistoExcelImportManagementServiceNew(RwaContext context)
        {
            _context = context;
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

                // ⚡ ULTRA-FAST: AsParallel() optimization for row processing
                List<HecateInterneHistorique> bddHisto = bddHistoDt.AsEnumerable()
                    .AsParallel()
                    .Select(s =>
                {
                    var dateEch = s.Field<Object>("Date d’échéance");
                    DateOnly? parse;
                    if (dateEch != null)
                    {
                        if (DateTime.TryParse(dateEch.ToString(), out DateTime res))
                        {
                            parse = DateOnly.FromDateTime(res);
                        }
                        else
                        {
                            parse = null;
                        }

                    }
                    else
                    {
                        parse = null;
                    }
                    return new HecateInterneHistorique()
                    {
                        Source = s.Field<string>("Source"),
                        RefCategorieRwa = s.Field<string>("Catégorie RWA"),
                        IdentifiantUniqueRetenu = s.Field<string>("Identifiant unique retenu"),
                        Raf = s.Field<string>("RAF"),
                        LibelleOrigine = s.Field<string>("Libellé d’origine"),
                        DateEcheance = parse,
                        IdentifiantOrigine = s.Field<string>("Identifiant d'origine"),
                        Bbgticker = s.Field<string>("BBG Ticker"),
                        LibelleTypeDette = "SUBORDONNE",
                        LastUpdate = DateTime.Now.ToString("dd-MM-yyyy_Hmmss")
                    };
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
