using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosReportControle0b
{
    public string TypeRaf { get; set; } = null!;

    public string? PeriodeCloture { get; set; }

    public string? Source { get; set; }

    public string? IdentifiantUniqueRetenu { get; set; }

    public string? RefCategorieRwa { get; set; }

    public string? Raf { get; set; }

    public string? LibelleOrigine { get; set; }

    public DateOnly? DateFinContrat { get; set; }
}
