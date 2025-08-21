using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RWA.Web.Application.Services.ExcelManagementService.Import.BDDHistorique
{
    public class BDDHistoExcelImportManagementService : IExcelImportManagementService
    {
        private readonly string process = "IMPORT BDD HISTORIQUE";
        private readonly string Date = DateTime.Now.ToString("dd-MM-yyyy_Hmmss");
        private readonly string NullImportFile = "Fichier non trouvé";
        private readonly string NoBDDHistoWorkbook = "L'onglet BDD est vide";
        private readonly string ErrorSQL = "ERREUR FICHIER EXCEL: ";
        private readonly string SuccessfulImport = "Fichier importé avec succès";


        protected RwaContext _context { get; set; }
        public BDDHistoExcelImportManagementService(RwaContext context)
        {
            _context = context;
        }
        public async Task<HECATESettingViewModel> ImportExcel(IFormFile file)
        {
            var ResultViewModel = new HECATESettingViewModel();
            if (file == null || file.Length == 0)
            {
                return GetHECATESettingViewModel(false, NullImportFile);
            }
            // Set EPPlus license context for non-commercial use.
          
            var importResults = new List<ImportResult>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(s=> s.Name=="BDD");
                    if (worksheet == null)
                    {

                        return GetHECATESettingViewModel(false, NoBDDHistoWorkbook);
                    }
                    int rowCount = worksheet.Dimension.Rows;
                    if (rowCount <2) 
                    {
                        return GetHECATESettingViewModel(false, NoBDDHistoWorkbook); 
                    }
                    if (_context.HecateInterneHistoriques.Any())
                    {
                        await _context.HecateInterneHistoriques.ExecuteDeleteAsync();
                    }
                    try
                    {

                        for (int row = 2; row <= rowCount; row++)
                        {
                            await _context.HecateInterneHistoriques.AddAsync(new HecateInterneHistorique()
                            {
                                Source = worksheet.Cells[row, 1].Text.Trim(),
                                RefCategorieRwa = worksheet.Cells[row, 2].Text.Trim(),
                                IdentifiantUniqueRetenu = worksheet.Cells[row, 3].Text.Trim(),
                                Raf = worksheet.Cells[row, 4].Text.Trim(),
                                LibelleOrigine = worksheet.Cells[row, 5].Text.Trim(),
                                DateEcheance = DateOnly.TryParse(worksheet.Cells[row, 6].Text.Trim(), out DateOnly date) ? date : null,
                                IdentifiantOrigine = worksheet.Cells[row, 7].Text.Trim(),
                                Bbgticker = worksheet.Cells[row, 8].Text.Trim(),
                                LibelleTypeDette = "SUBORDONNE",
                                LastUpdate = DateTime.Now.ToString("dd-MM-yyyy_Hmmss")
                            });


                        }
                        
                    }
                    catch (Exception ex)
                    {
                        return GetHECATESettingViewModel(false, $"{ErrorSQL}{ex.Message}{ex.InnerException?.ToString()}");
                    }
                }
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
                    return GetHECATESettingViewModel(false, $"{ErrorSQL}{ex.Message}{ex.InnerException?.ToString()}");
                    // Optionally, log the error.
                }
            }

            return GetHECATESettingViewModel(true, SuccessfulImport);
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
