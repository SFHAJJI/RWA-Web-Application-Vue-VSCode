using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamComptesMagnitudeAExtraire
{
    public string CompteMagnitude { get; set; } = null!;

    public string DetailsParPartnerId { get; set; } = null!;

    public string? Raf { get; set; }

    public string? PartnerId { get; set; }

    public string? LibellePartnerId { get; set; }

    public string? CompteMagnitudeRemplacement { get; set; }

    public string? CompteAnetter { get; set; }

    public string LastUpdate { get; set; } = null!;
}
