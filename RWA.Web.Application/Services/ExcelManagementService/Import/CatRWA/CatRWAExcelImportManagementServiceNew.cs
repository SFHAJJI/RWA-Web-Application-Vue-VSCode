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
    public class CatRWAExcelImportManagementServiceNew
    {
        private readonly string process = "IMPORT CAT RWA";
        private readonly string Date = DateTime.Now.ToString("dd-MM-yyyy_Hmmss");
        private readonly string NoCategorieRWAWorkbook = "L'onglet CategorieRWA est vide";
        private readonly string ErrorSQL = "ERREUR FICHIER EXCEL: ";
        private readonly string SuccessfulImport = "Import onglet (CategorieRWA) avec succès";


        protected RwaContext _context { get; set; }
        public CatRWAExcelImportManagementServiceNew(RwaContext context)
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
            if (_context.HecateCategorieRwas.Any())
            {
                await _context.HecateCategorieRwas.ExecuteDeleteAsync();
            }
            try
            {
                // ⚡ ULTRA-FAST: AsParallel() optimization for row processing
                IEnumerable<HecateCategorieRwa> hecateCategorieRwas = dt.AsEnumerable()
                    .AsParallel()
                    .Select(m => new HecateCategorieRwa()
                    {
                        IdCatRwa = m.Field<string>("Code RWA"),
                        Libelle = m.Field<string>("Libelle Categorie RWA"),
                        ValeurMobiliere = m.Field<string>("Valeur Mobiliere"),
                    }).Where(m=>!string.IsNullOrEmpty(m.IdCatRwa));
                _context.HecateCategorieRwas.AddRange(hecateCategorieRwas);

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
                    value(GetImportResults(false, $"{ErrorSQL}{ex.Message} {ex.InnerException?.ToString()}"));
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
