using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosImportFichierEnrichiBrut
{
    public string PeriodeCloture { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string? RefCategorieRwa { get; set; }

    public string IdentifiantUniqueRetenu { get; set; } = null!;

    public string? Raf { get; set; }

    public string? LibelleOrigine { get; set; }

    public DateOnly? DateFinContrat { get; set; }

    public double? ValeurDeMarche { get; set; }
}
