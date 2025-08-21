using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosControlesOngletContrat
{
    public string? PeriodeCloture { get; set; }

    public string IdentifiantContrat { get; set; } = null!;

    public string? ControleRaf { get; set; }

    public string? ControleCodeSegmentMcDonough { get; set; }

    public string? ControlePerimetreMcDonough { get; set; }

    public string? ControleCodeActivite { get; set; }

    public string? ControleDateInitialeFinContrat { get; set; }

    public string ControleDevise { get; set; } = null!;

    public string ControlePresenceEncours { get; set; } = null!;

    public string ControlePresenceTitres { get; set; } = null!;

    public string ControleDoublon { get; set; } = null!;
}
