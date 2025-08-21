using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Data;

namespace RWA.Web.Application.Services.ExcelManagementService.Import.CatRWA
{
    public class CatDepoRWAExcelImportManagementServiceNew
    {
        private readonly string process = "IMPORT CAT DEPO RWA";
        private readonly string Date = DateTime.Now.ToString("dd-MM-yyyy_Hmmss");
        private readonly string NoCategorieRWAWorkbook = "L'onglet EquivalenceCatRWA est vide";
        private readonly string ErrorSQL = "ERREUR FICHIER EXCEL: ";
        private readonly string SuccessfulImport = "Import des Catégories Depositaires avec succès";


        protected RwaContext _context { get; set; }
        public CatDepoRWAExcelImportManagementServiceNew(RwaContext context)
        {
            _context = context;
        }
        public async Task<bool> TryImportOnglet(DataTable dt, Action<List<ImportResult>> value)
        {

            var importResults = new List<ImportResult>();

            if (dt == null || dt.Rows.Count<1)
            {

                value(GetImportResults(false, NoCategorieRWAWorkbook));
                return false;
            }
            if (_context.HecateCatDepositaire1s.Any())
            {
                await _context.HecateCatDepositaire1s.ExecuteDeleteAsync();
            }
            if (_context.HecateCatDepositaire2s.Any())
            {
                await _context.HecateCatDepositaire2s.ExecuteDeleteAsync();
            }
            try
            {
                IEnumerable<HecateCatDepositaire1> hecateCatDepositaire1s = dt.AsEnumerable().Select(m => new HecateCatDepositaire1()
                {
                    LibelleDepositaire1 = m.Field<string>("Code catégorie 1")
                }).DistinctBy(p => p.LibelleDepositaire1);
                _context.HecateCatDepositaire1s.AddRange(hecateCatDepositaire1s);
                IEnumerable<HecateCatDepositaire2> hecateCatDepositaire2s = dt.AsEnumerable().Select(m => new HecateCatDepositaire2()
                {
                    LibelleDepositaire2 = m.Field<string>("Code catégorie 2")
                }).DistinctBy(p => p.LibelleDepositaire2).Select(s =>
                {
                    if (s.LibelleDepositaire2 == null)
                    {
                        s.LibelleDepositaire2 = string.Empty;
                    }
                    return s;
                });
                _context.HecateCatDepositaire2s.AddRange(hecateCatDepositaire2s);

            }
            catch (Exception ex)
            {
                value(GetImportResults(false, $"{ErrorSQL}{ex.Message}{ex.InnerException?.ToString()}"));
                return false;
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
                    value(GetImportResults(false, $"{ErrorSQL}{ex.Message}{ex.InnerException?.ToString()}"));
                    return false;
                    // Optionally, log the error.
                }
            }
            value(GetImportResults(true, SuccessfulImport));
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
    }
}
