using System;
using System.Globalization;

namespace RWA.Web.Application.Models.Dtos
{
    public class HecateInterneHistoriqueDto
    {
        public string Source { get; set; }
        public string RefCategorieRwa { get; set; }
        public string IdentifiantUniqueRetenu { get; set; }
        public string Raf { get; set; }
        public string LibelleOrigine { get; set; }
        public string DateEcheance { get; set; }
        public string IdentifiantOrigine { get; set; }
        public string Bbgticker { get; set; }
        public string LibelleTypeDette { get; set; }

        public HecateInterneHistorique ToHecateInterneHistorique()
        {
            DateOnly? dateEcheance = null;
            if (DateOnly.TryParseExact(this.DateEcheance, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                dateEcheance = parsedDate;
            }

            return new HecateInterneHistorique
            {
                Source = this.Source,
                IdentifiantOrigine = this.IdentifiantOrigine,
                RefCategorieRwa = this.RefCategorieRwa,
                IdentifiantUniqueRetenu = this.IdentifiantUniqueRetenu,
                Raf = this.Raf,
                LibelleOrigine = this.LibelleOrigine,
                DateEcheance = dateEcheance,
                Bbgticker = this.Bbgticker,
                LibelleTypeDette = this.LibelleTypeDette,
                LastUpdate = DateTime.UtcNow.ToString("o")
            };
        }
    }
}
