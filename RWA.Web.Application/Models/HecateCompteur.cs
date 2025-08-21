using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateCompteur
{
    public string Source { get; set; } = null!;

    public string RefCategorieRwa { get; set; } = null!;

    public string PeriodeCloture { get; set; } = null!;

    public int? Compteur { get; set; }
}
