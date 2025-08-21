using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosImportMagnitudeRetraite
{
    public string PeriodeCloture { get; set; } = null!;

    public string? Source { get; set; }

    public string? CompteMagnitude { get; set; }

    public string? PartnerId { get; set; }

    public string? LibellePartnerId { get; set; }

    public string? RefCategorieRwa { get; set; }

    public string? IdentifiantUniqueRetenu { get; set; }

    public string? Raf { get; set; }

    public double? Montant { get; set; }

    public DateOnly? DateFinContrat { get; set; }
}
