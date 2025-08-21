using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosSeedMoney
{
    public string Source { get; set; } = null!;

    public string? Libelle { get; set; }

    public string PartnerId { get; set; } = null!;

    public string CompteMagnitude { get; set; } = null!;

    public string Raf { get; set; } = null!;

    public string? PeriodeCloture { get; set; }

    public double? MontantSeedMoney { get; set; }

    public double? TotalActifSousGestion { get; set; }
}
