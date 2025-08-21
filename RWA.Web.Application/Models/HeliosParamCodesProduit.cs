using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamCodesProduit
{
    public string CodeProduit { get; set; } = null!;

    public string? CodeSegmentMcDo { get; set; }

    public string ValeurMobiliere { get; set; } = null!;

    public string? TypeParticipation { get; set; }

    public string SensOperation { get; set; } = null!;

    public string? NatureTitre { get; set; }

    public string? TypeTitre { get; set; }

    public string? TitreDette { get; set; }

    public string? OpcvmcomposeTitresEligibles { get; set; }

    public string LastUpdate { get; set; } = null!;
}
