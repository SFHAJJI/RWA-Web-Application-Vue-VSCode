using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosOngletTitre
{
    public string IdentifiantUniqueRetenu { get; set; } = null!;

    public string IdentifiantTitre { get; set; } = null!;

    public string NatureTitre { get; set; } = null!;

    public string TypeTitre { get; set; } = null!;

    public string Rafemetteur { get; set; } = null!;

    public decimal? NominalTitre { get; set; }

    public string? CodeDeviseNominal { get; set; }

    public string? DateEcheanceTitre { get; set; }

    public string? IndexTaux { get; set; }

    public string? TypeTaux { get; set; }

    public decimal? ValeurOuMargeTauxFixe { get; set; }

    public string? DeviseCoupon { get; set; }

    public string TitreDette { get; set; } = null!;

    public decimal? CoursTitre { get; set; }

    public string? NombreDecimaleCours { get; set; }

    public string TitreGaranti { get; set; } = null!;

    public string? RafgarantTitre { get; set; }

    public string? TitreLiquide { get; set; }

    public string? TitreCote { get; set; }

    public string? IndiceCotation { get; set; }

    public string? FrequenceCotation { get; set; }

    public string? PlaceCotation { get; set; }

    public string? RangTitre { get; set; }

    public string? TitreNote { get; set; }

    public string? NotationTitre { get; set; }

    public string? Oeec { get; set; }

    public string? EligibiliteMcDo { get; set; }

    public string? OpcvmcomposeTitresEligibles { get; set; }

    public string? Granularite { get; set; }

    public string? DateNotation { get; set; }
}
