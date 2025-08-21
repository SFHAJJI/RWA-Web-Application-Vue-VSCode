using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamMappingTitresTransparence
{
    public string IdentifiantIsin { get; set; } = null!;

    public string? NomActif { get; set; }

    public string? RefCategorieRwa { get; set; }

    public string? Raf { get; set; }

    public string TitreLiquide { get; set; } = null!;

    public string TitreCote { get; set; } = null!;

    public string? IndiceCotation { get; set; }

    public string? FrequenceCotation { get; set; }

    public string? PlaceCotation { get; set; }

    public string LastUpdate { get; set; } = null!;
}
