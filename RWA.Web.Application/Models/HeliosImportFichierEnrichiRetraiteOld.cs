using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosImportFichierEnrichiRetraiteOld
{
    public string PeriodeCloture { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string? CompteMagnitude { get; set; }

    public string? PartnerId { get; set; }

    public string? RefCategorieRwa { get; set; }

    public string IdentifiantUniqueRetenu { get; set; } = null!;

    public string? Raf { get; set; }

    public string? LibelleOrigine { get; set; }

    public double? MontantProratise { get; set; }

    public DateOnly? DateFinContrat { get; set; }

    public double? ValeurDeMarche { get; set; }
}
