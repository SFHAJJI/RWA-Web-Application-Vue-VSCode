using System;

namespace RWA.Web.Application.Models.Dtos
{
    public class HecateInterneHistoriqueDto
    {
        public string Source { get; set; }
        public string IdentifiantOrigine { get; set; }
        public string RefCategorieRwa { get; set; }
        public string IdentifiantUniqueRetenu { get; set; }
        public string Raf { get; set; }
        public string LibelleOrigine { get; set; }
        public DateOnly? DateEcheance { get; set; }

        public HecateInterneHistorique ToHecateInterneHistorique()
        {
            return new HecateInterneHistorique
            {
                Source = this.Source,
                IdentifiantOrigine = this.IdentifiantOrigine,
                RefCategorieRwa = this.RefCategorieRwa,
                IdentifiantUniqueRetenu = this.IdentifiantUniqueRetenu,
                Raf = this.Raf,
                LibelleOrigine = this.LibelleOrigine,
                DateEcheance = this.DateEcheance,
                LastUpdate = DateTime.UtcNow.ToString("o")
            };
        }
    }
}
