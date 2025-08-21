using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosReportControle9
{
    public string PeriodeCloture { get; set; } = null!;

    public string? CompteMagnitude { get; set; }

    public string? Source { get; set; }

    public double? MontantAgrege { get; set; }

    public double? MontantMagnitude { get; set; }

    public double? Ecart { get; set; }

    public string? Commentaires { get; set; }
}
