using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamEntreesPerimetre
{
    public string ReportingUnit { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string? Libelle { get; set; }

    public int? MethodeStandard { get; set; }

    public string LastUpdate { get; set; } = null!;
}
