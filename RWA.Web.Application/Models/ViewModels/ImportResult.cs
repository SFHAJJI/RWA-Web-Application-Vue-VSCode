using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO;
using CsvHelper;
using RWA.Web.Application.Models.RWAMarket; // N'oubliez pas d'inclure ce namespace
using System.Collections.Generic; // Assurez-vous d'inclure ce namespace pour List<>
using System.Linq; // Pour l'utilisation de ToList()
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RWA.Web.Application.Models
{
    public class ImportResult
    {

        public string Date { get; set; }
        public bool Success { get; set; }
        public string Process { get; set; }
        // If not processed, this message will hold the validation errors or exception details
        public string Message { get; set; }
    }
}
