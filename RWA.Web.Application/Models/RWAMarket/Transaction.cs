namespace RWA.Web.Application.Models.RWAMarket
{
    public class Transaction
    {
        public string? Type { get; set; } // Achat ou Vente
        public double Montant { get; set; } // Montant de la transaction
        public DateTime Date { get; set; } // Date de la transaction
        public string? Actif { get; set; } // Nom de l'actif
    }
}
