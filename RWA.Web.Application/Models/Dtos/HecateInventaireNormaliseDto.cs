using System;

namespace RWA.Web.Application.Models.Dtos
{
    public class HecateInventaireNormaliseDto
    {
        public int NumLigne { get; set; }
        public string PeriodeCloture { get; set; }
        public string Source { get; set; }
        public string RefCategorieRwa { get; set; }
        public string IdentifiantUniqueRetenu { get; set; }
        public string Raf { get; set; }
        public string LibelleOrigine { get; set; }
        public string DateFinContrat { get; set; }
        public string IdentifiantOrigine { get; set; }
        public double ValeurDeMarche { get; set; }
        public string Categorie1 { get; set; }
        public string Categorie2 { get; set; }
        public string DeviseDeCotation { get; set; }
        public decimal? TauxObligation { get; set; }
        public string DateMaturite { get; set; }
        public string DateExpiration { get; set; }
        public string Tiers { get; set; }
        public string BoaSj { get; set; }
        public string BoaContrepartie { get; set; }
        public string BoaDefaut { get; set; }
        public string Bloomberg { get; set; }
        public bool IsTauxObligationInvalid { get; set; }
        public bool IsDateMaturiteInvalid { get; set; }
    }
}
