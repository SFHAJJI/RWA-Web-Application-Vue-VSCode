using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosControlesOngletEncour
{
    public string? PeriodeCloture { get; set; }

    public string? IdentifiantContrat { get; set; }

    public string? ControleRaf { get; set; }

    public string? ControleEncoursProvisionnels { get; set; }

    public string ControlePresenceContrats { get; set; } = null!;
}
