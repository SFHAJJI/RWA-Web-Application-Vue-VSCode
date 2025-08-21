using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosReportControle10
{
    public string PeriodeCloture { get; set; } = null!;

    public string? Source { get; set; }

    public string? CompteMagnitude { get; set; }

    public string? PartnerId { get; set; }

    public string? RefCategorieRwa { get; set; }

    public string? IdentifiantUniqueRetenu { get; set; }

    public string? Raf { get; set; }

    public string? LibelleOrigineOuPartnerId { get; set; }

    public double? MontantProratise { get; set; }

    public DateOnly? DateFinContrat { get; set; }

    public string? CompteCopernic { get; set; }

    public string? CodeProduit { get; set; }

    public string? Pcec { get; set; }
}
