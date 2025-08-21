using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamIntentionGestionParCompte
{
    public string CompteMagnitude { get; set; } = null!;

    public string? PortefeuilleIas { get; set; }

    public string LastUpdate { get; set; } = null!;
}
