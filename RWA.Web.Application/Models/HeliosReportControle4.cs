using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosReportControle4
{
    public string Source { get; set; } = null!;

    public double? MontantSeedMoney { get; set; }

    public double? SommeMontantProratise { get; set; }

    public double? Ecart { get; set; }
}
