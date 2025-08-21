using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosControlesOngletTitre
{
    public string? PeriodeCloture { get; set; }

    public string IdentifiantTitre { get; set; } = null!;

    public string ControleTypeTitre { get; set; } = null!;

    public string? ControleRaf { get; set; }

    public string? ControleCodeDeviseNominal { get; set; }

    public string ControleTitreCote { get; set; } = null!;

    public string ControleTitreNote { get; set; } = null!;

    public string ControleIdentifiantTitre { get; set; } = null!;

    public string ControlePresenceContrats { get; set; } = null!;

    public string ControleDoublon { get; set; } = null!;
}
