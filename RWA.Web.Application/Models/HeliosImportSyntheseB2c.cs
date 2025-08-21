using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosImportSyntheseB2c
{
    public string PeriodeCloture { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string IdentifiantUniqueRetenu { get; set; } = null!;

    public string? Approche { get; set; }

    public string? Rafb2c { get; set; }

    public string? ClasseActifB2 { get; set; }

    public string? CategorieProduit { get; set; }

    public string? Rating { get; set; }

    public double? TauxPonderation { get; set; }

    public double? MontantActifPondere { get; set; }

    public double? Notionnel { get; set; }

    public string ExternalId { get; set; } = null!;
}
