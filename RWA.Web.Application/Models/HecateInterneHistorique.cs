using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateInterneHistorique
{
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
}
