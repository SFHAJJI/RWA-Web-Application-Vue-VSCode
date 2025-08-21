using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;

namespace RWA.Web.Application.Services.ExcelManagementService.Import.CatRWA
{
    public class EqCatRWAExcelImportManagementServiceNew : IExcelImportManagementService
    {
        private const string PROCESS = "IMPORT EQUIV RWA";
        private const string NULL_IMPORT_FILE = "Fichier non trouvé";
        private readonly string Date = DateTime.Now.ToString("dd-MM-yyyy_Hmmss");
        private const string NO_EQUIVALENCE_CAT_RWA_WORKBOOK = "L'onglet EquivalenceCatRWA est vide";
        private const string GENERAL_ERROR_SQL = "ERREUR FICHIER EXCEL: ";
        private const string SUCESSFULL_IMPORT = "Import onglet (EquivalenceCatRWA) avec succès";
        private List<ImportResult> ImportResults = new List<ImportResult>();
        private readonly string IMPORT_PATH = "RWA Data\\Templates\\HECATE\\Import";
        private CatRWAExcelImportManagementServiceNew CatRWAImportService;
        private TypeBloombergExcelImportManagementServiceNew TypeBloombergImportService;
        private CatDepoRWAExcelImportManagementServiceNew CatDepoRWAImportService;
        protected RwaContext _context { get; set; }
        public EqCatRWAExcelImportManagementServiceNew(RwaContext context)
        {
            _context = context;
            CatRWAImportService = new CatRWAExcelImportManagementServiceNew(_context);
            TypeBloombergImportService = new TypeBloombergExcelImportManagementServiceNew(_context);
            CatDepoRWAImportService = new CatDepoRWAExcelImportManagementServiceNew(_context);
        }
        public async Task<HECATESettingViewModel> ImportExcel(IFormFile file)
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
            var importResults = await ImportCarRWAFromDatatableCollection(tables);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return new HECATESettingViewModel() { ImportResults = importResults };

        }
        private async Task<List<ImportResult>> ImportCarRWAFromDatatableCollection(DataTableCollection tables)
        {
            if (_context.HecateEquivalenceCatRwas.Any())
            {
                await _context.HecateEquivalenceCatRwas.ExecuteDeleteAsync();
            }
            if (!await TryPreImport(tables, value => ImportResults.AddRange(value)))
            {
                return ImportResults;
            }
           
            var equivalenceCatRWADt = tables["EquivalenceCatRWA"];
            try
            {

                if (TryGetHecateEquivalenceCatRwas(equivalenceCatRWADt, out List<HecateEquivalenceCatRwa> hecateEquivalenceCatRwas))
                {
                    _context.HecateEquivalenceCatRwas.AddRange(hecateEquivalenceCatRwas);
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

        private bool TryGetHecateEquivalenceCatRwas(DataTable? equivalenceCatRWADt, out List<HecateEquivalenceCatRwa> value)
        {
            try
            {
                var HecateCategorieRwasDict = _context.HecateCategorieRwas.ToDictionary(p => p.IdCatRwa, p => p);
                var HecateTypeBloombergsDict = _context.HecateTypeBloombergs.ToDictionary(p => p.Libelle, p => p);

                var HecateCatDepositaire1sDict = _context.HecateCatDepositaire1s.ToDictionary(p => p.LibelleDepositaire1, p => p);
                var HecateCatDepositaire2sDict = _context.HecateCatDepositaire2s.ToDictionary(p => p.LibelleDepositaire2??string.Empty, p => p);

                // ⚡ ULTRA-FAST: AsParallel() optimization for row processing
                List<HecateEquivalenceCatRwa> hecateEquivalenceCatRwas = equivalenceCatRWADt.AsEnumerable()
                    .AsParallel()
                    .Where(s =>
                    {
                       
                        if (!string.IsNullOrEmpty(s.Field<string>("Source")) &&
                        HecateCategorieRwasDict.ContainsKey(s.Field<string>("Catégorie RWA")) &&
                        HecateCatDepositaire1sDict.ContainsKey(s.Field<string>("Code catégorie 1")) &&
                        HecateCatDepositaire2sDict.ContainsKey(s.Field<string>("Code catégorie 2") ?? string.Empty)
                        )
                        {
                            return true;
                        }
                        return false; 
                    }).Select(s =>
                    {
                        return GetHecateEquivalenceCatRwa(s, HecateCategorieRwasDict, HecateTypeBloombergsDict, HecateCatDepositaire1sDict, HecateCatDepositaire2sDict);
                    }).ToList();
                value = hecateEquivalenceCatRwas;
            }
            catch(Exception ex)
            {
                value = Enumerable.Empty<HecateEquivalenceCatRwa>().ToList();
                throw;
              
            }
            return true;
          
        }
        private HecateEquivalenceCatRwa GetHecateEquivalenceCatRwa(DataRow s, 
            Dictionary<string, HecateCategorieRwa> HecateCategorieRwasDict,
            Dictionary<string, HecateTypeBloomberg> HecateTypeBloombergsDict,
            Dictionary<string, HecateCatDepositaire1> HecateCatDepositaire1sDict,
            Dictionary<string, HecateCatDepositaire2> HecateCatDepositaire2sDict)
        {
            try
            {
                var refCatDepositaire2 = s.Field<string>("Code catégorie 2") ?? string.Empty;
             
                return new HecateEquivalenceCatRwa()
                {
                    Source = s.Field<string>("Source"),
                    RefCategorieRwa = s.Field<string>("Catégorie RWA"),
                    RefCategorieRwaNavigation = HecateCategorieRwasDict[s.Field<string>("Catégorie RWA")],
                    RefTypeBloomberg = s.Field<string>("Type Bloomberg"),
                    RefTypeBloombergNavigation = string.IsNullOrEmpty(s.Field<string>("Type Bloomberg")) ? null : HecateTypeBloombergsDict[s.Field<string>("Type Bloomberg")],
                    RefCatDepositaire1 = HecateCatDepositaire1sDict[s.Field<string>("Code catégorie 1")].IdDepositaire1,
                    RefCatDepositaire1Navigation = HecateCatDepositaire1sDict[s.Field<string>("Code catégorie 1")],
                    RefCatDepositaire2 = HecateCatDepositaire2sDict[refCatDepositaire2].IdDepositaire2,
                    RefCatDepositaire2Navigation = HecateCatDepositaire2sDict[refCatDepositaire2],
                };
            }
            catch (Exception e)
            {
                var test = e;
                return null;
            }
            
        }
        private async Task<bool> TryPreImport(DataTableCollection tables, Action<List<ImportResult>> value)
        {
            var equivalenceCatRWADt = tables["EquivalenceCatRWA"];
            var typeBloombergDt = tables["TypeBloomberg"]; 
            var categorieRWADt = tables["CategorieRWA"]; 



            if (equivalenceCatRWADt == null || equivalenceCatRWADt.Rows.Count<1)
            {
                value(GetImportResults(false, NO_EQUIVALENCE_CAT_RWA_WORKBOOK));
                return false;
               
            }
          
            // ⚡ ULTRA-FAST: Task.WhenAll for parallel import operations  
            var importTasks = new Task<bool>[]
            {
                CatRWAImportService.TryImportOnglet(categorieRWADt, value => ImportResults.AddRange(value)),
                TypeBloombergImportService.TryImportOnglet(typeBloombergDt, value => ImportResults.AddRange(value)),
                CatDepoRWAImportService.TryImportOnglet(equivalenceCatRWADt, value => ImportResults.AddRange(value))
            };

            var results = await Task.WhenAll(importTasks);
            
            if (!results.All(r => r))
            {
                value(ImportResults);
                return false;
            }
            return true;
           
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
                    Process = PROCESS,
                    Message = arg.ToString(),
                };
                importResults.Add(importResult);

            }
            resultViewModel.ImportResults = importResults;
            return resultViewModel;
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
                    Process = PROCESS,
                    Message = arg.ToString(),
                };
                importResults.Add(importResult);

            }
            return importResults;
        }
    }
}
