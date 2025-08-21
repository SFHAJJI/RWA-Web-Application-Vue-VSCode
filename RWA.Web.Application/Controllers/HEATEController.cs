using System.ComponentModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RWA.Web.Application.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using OfficeOpenXml.Style;
using RWA.Web.Application.Services.ExcelManagementService;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using RWA.Web.Application.Services.ExcelManagementService.Export;
using RWA.Web.Application.Services.ExcelManagementService.Import;

namespace RWA.Web.Application.Controllers
{
    public class HECATEController : Controller
    {
        private readonly RwaContext _context;
        private HECATESettingViewModel _settingviewModel;

        public HECATEController(RwaContext context)
        {
            _context = context;
            //var test = _context.HecateCategorieRwas.ToList();
            //var test2 = test;
        }
        [HttpGet]
        [Authorize]
        public IActionResult MenuPrincipalHecate()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public IActionResult ImportParams()
        {
            _settingviewModel =  new HECATESettingViewModel();
            // Logique pour le paramétrage
            return View(_settingviewModel);
        }
        [HttpGet]
        [Authorize]
        public IActionResult MenuInventairesNormalises()
        {
            // Logique pour inventaires normalisés
            return View();
        }
    
        [HttpGet]
        [Authorize]
        public IActionResult Tethys()
        {
            // Logique pour TETHYS
            return View();
        }
        [HttpGet]
        [Authorize]
        public IActionResult HecateReport()
        {
            // Logique pour rapports
            return View();
        }

        // GET: Products/DownloadTemplate
        // Allows the user to download the Excel template for product import.
        public IActionResult DownloadTemplate(ImportExportType importExportType)
        {
            try
            {
                var ExcelManagementService = new ExcelManagementServiceFactory(_context).GetExcelManagementService(importExportType);

                using (var package = ExcelManagementService.CreateExcel(out string fileName))
                {
                    var fileBytes = package.GetAsByteArray();
                    return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{fileName}.xlsx"
                    );
                }

            }
            catch (Exception ex)
            {
                var importResults = new List<ImportResult>();
                importResults.Add(new ImportResult()
                {
                    Date = string.Empty,
                    Success = false,
                    Process = "Export",
                    Message = $"{ex.Message}{ex.InnerException?.ToString() ??string.Empty}",
                });
                var hecateSettingViewModel= new HECATESettingViewModel() { ImportResults = importResults };

                return View("ImportParams", hecateSettingViewModel);
            }
            
        }

        // POST: Products/Import
        // Processes the uploaded Excel file, validates each product, and displays an import summary.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import([FromForm] ImportExportViewModel model)
        {
            var ExcelImportManagementService = new ExcelImportManagemenServiceFactory(_context).GetExcelManagementService(ImportExportViewModel.Dict[model.TooltipMessage]);
            var hecateSettingViewModel = await ExcelImportManagementService.ImportExcel(model.FileUpload);
            return View("ImportParams", hecateSettingViewModel);
         
        }
    }
}