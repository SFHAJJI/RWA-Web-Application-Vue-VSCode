using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamMappingTitresMagnitude
{
    public string PartnerId { get; set; } = null!;

    public string LibellePartnerId { get; set; } = null!;

    public string? Raf { get; set; }

    public string? RefCategorieRwa { get; set; }

    public string? IdentifiantIsin { get; set; }

    public string? TitreLiquide { get; set; }

    public string? TitreCote { get; set; }

    public string? IndiceCotation { get; set; }

    public string? FrequenceCotation { get; set; }

    public string? PlaceCotation { get; set; }

    public string? CategorieSynthese { get; set; }

    public string? TraitementStandard { get; set; }

    public string? LibelleTypeDette { get; set; }

    public string? ManagementIntent { get; set; }

    public string? LastUpdate { get; set; }
}
