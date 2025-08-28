using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateInterneHistorique
{
    public HecateInterneHistorique() { }

    public HecateInterneHistorique(HecateInventaireNormalise item)
    {
        Source = item.Source;
        IdentifiantOrigine = item.IdentifiantOrigine;
        RefCategorieRwa = item.RefCategorieRwa ?? string.Empty;
        IdentifiantUniqueRetenu = item.IdentifiantUniqueRetenu ?? string.Empty;
        Raf = item.Raf ?? string.Empty;
        LibelleOrigine = item.Nom ?? string.Empty;
        DateEcheance = item.DateFinContrat;
        LastUpdate = DateTime.UtcNow.ToString("o");
    }

    public string Source { get; set; } = null!;

    public string IdentifiantOrigine { get; set; } = null!;

    public string RefCategorieRwa { get; set; } = null!;

    public string IdentifiantUniqueRetenu { get; set; } = null!;

    public string Raf { get; set; } = null!;

    public string LibelleOrigine { get; set; } = null!;

    public DateOnly? DateEcheance { get; set; }

    public string? Bbgticker { get; set; }

    public string? LibelleTypeDette { get; set; }

    public string? LastUpdate { get; set; }

    public Dtos.HecateInterneHistoriqueDto ToDto()
    {
        return new Dtos.HecateInterneHistoriqueDto
        {
            Source = this.Source,
            IdentifiantOrigine = this.IdentifiantOrigine,
            RefCategorieRwa = this.RefCategorieRwa ?? string.Empty,
            IdentifiantUniqueRetenu = this.IdentifiantUniqueRetenu ?? string.Empty,
            Raf = this.Raf ?? string.Empty,
            LibelleOrigine = this.LibelleOrigine ?? string.Empty,
            DateEcheance = this.DateEcheance?.ToString("dd/MM/yyyy") ?? string.Empty,
            Bbgticker = this.Bbgticker ?? string.Empty,
            LibelleTypeDette = this.LibelleTypeDette ?? string.Empty
        };
    }
}
