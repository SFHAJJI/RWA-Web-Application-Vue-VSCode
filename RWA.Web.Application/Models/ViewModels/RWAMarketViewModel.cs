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
    public class RWAMarketViewModel
    {

        // Déclaration des variables liés à la lecture des transactions
        public List<Transaction> Transactions { get; set; } // Pour stocker les transactions
        public int TransactionCount { get; set; } // Pour stocker le nombre de transactions lues
        public IEnumerable<SelectListItem> AvailableAssets { get; set; } // Pour stocker les actifs disponibles
        public string? ErrorMessage { get; set; } // Pour stocker un message d'erreur

        // Déclaration des informations
        [BindProperty]
        public List<string?> SelectedAssets { get; set; }  // Pour stocker les actifs sélectionnés par l'utilisateur
        [BindProperty]
        public DateTime? StartDate { get; set; } // Pour stocker la date de début, pour filtrer les transactions sur les dates
        [BindProperty]
        public DateTime? EndDate { get; set; } // Pour stocker la date de fin

        public RWAMarketViewModel()
        {
            Transactions = new List<Transaction>(); // Initialisation de la liste
            SelectedAssets = new List<string?>(); // Initialisation de la liste des actifs sélectionnés
        }

        public void OnGet()
        {
            ReadTransactionsFromCsv(); // Lire les transactions à la demande GET
            LoadAvailableAssets(); // Charger les actifs disponibles

            // Initialiser les dates par défaut
            if (!StartDate.HasValue)
            {
                StartDate = Transactions.Min(t => t.Date); // Date minimale
            }
            if (!EndDate.HasValue)
            {
                EndDate = Transactions.Max(t => t.Date); // Date maximale
            }
        }

        public void OnPost()
        {
            ReadTransactionsFromCsv(); // Lire les transactions lors de la soumission du formulaire
            LoadAvailableAssets(); // Charger les actifs disponibles
        }


        private void ReadTransactionsFromCsv()
        {
            var path = Path.Combine("RWA Data", "transactions.csv"); // Chemin vers le fichier CSV

            try
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    Transactions = csv.GetRecords<Transaction>().ToList(); // Lire les enregistrements du CSV
                    TransactionCount = Transactions.Count; // Compter le nombre de transactions
                }
            }
            catch (Exception ex)
            {
                // Stocker le message d'erreur pour l'afficher dans l'interface web
                ErrorMessage = $"Erreur lors de la lecture du fichier CSV : {ex.Message}";
            }
        }

        private void LoadAvailableAssets()
        {
            AvailableAssets = Transactions.Select(t => new SelectListItem(){Text= t.Actif , Value = t.Actif }).Distinct().ToList(); // Obtenir la liste des actifs uniques
        }


    }
}
