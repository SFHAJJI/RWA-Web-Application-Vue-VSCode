using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamRatingTitrisation
{
    public string IdentifiantIsin { get; set; } = null!;

    public string? Rating { get; set; }

    public string? Oeec { get; set; }

    public string LastUpdate { get; set; } = null!;
}
