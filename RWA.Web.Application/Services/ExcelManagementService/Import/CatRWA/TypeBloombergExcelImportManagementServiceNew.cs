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
    public class TypeBloombergExcelImportManagementServiceNew
    {
        private readonly string process = "IMPORT TYPE BLOOM";
        private readonly string Date = DateTime.Now.ToString("dd-MM-yyyy_Hmmss");
        private readonly string NoTypeBloombergWorkbook = "L'onglet TypeBloomberg est vide";
        private readonly string ErrorSQL = "ERREUR FICHIER EXCEL: ";
        private readonly string SuccessfulImport = "Import onglet (TypeBloomberg) avec succès";


        protected RwaContext _context { get; set; }
        public TypeBloombergExcelImportManagementServiceNew(RwaContext context)
        {
            _context = context;
        }
        public async Task<bool> TryImportOnglet(DataTable dt, Action<List<ImportResult>> value)
        {
            var importResults = new List<ImportResult>();

            if (dt == null || dt.Rows.Count < 1)
            {

                value(GetImportResults(false, NoTypeBloombergWorkbook));
                return false;
            }
            if (_context.HecateTypeBloombergs.Any())
            {
                await _context.HecateTypeBloombergs.ExecuteDeleteAsync();
            }
            try
            {
                // ⚡ ULTRA-FAST: AsParallel() optimization for row processing
                IEnumerable<HecateTypeBloomberg> hecateTypeBloomberg = dt.AsEnumerable()
                    .AsParallel()
                    .Select(m => new HecateTypeBloomberg()
                    {
                        IdTypeBloomberg = m.Field<string>("Code Bloomberg"),
                        Libelle = m.Field<string>("Libelle Bloomberg")
                    }).Where(m => !string.IsNullOrEmpty(m.IdTypeBloomberg));
                _context.HecateTypeBloombergs.AddRange(hecateTypeBloomberg);

            }
            catch (Exception ex)
            {
                value(GetImportResults(false, $"{ErrorSQL}{ex.Message} {ex.InnerException?.ToString()}"));
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
