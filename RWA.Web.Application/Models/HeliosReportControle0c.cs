using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosReportControle0c
{
    public string PeriodeCloture { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string IdentifiantUniqueRetenu { get; set; } = null!;

    public string? RefCategorieRwa { get; set; }

    public string? Raf { get; set; }

    public string? LibelleOrigine { get; set; }

    public DateOnly? DateFinContrat { get; set; }
}
