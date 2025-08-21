using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosReportControle2
{
    public string Source { get; set; } = null!;

    public string IdentifiantUniqueRetenu { get; set; } = null!;

    public string? CompteMagnitude { get; set; }

    public string? PartnerId { get; set; }

    public double? MontantProratise { get; set; }
}
