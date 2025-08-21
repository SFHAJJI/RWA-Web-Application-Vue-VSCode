using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosOngletTitresTocsv
{
    public string? IdentifiantUniqueRetenu { get; set; }

    public string? IdentifiantTitre { get; set; }

    public string? NatureTitre { get; set; }

    public string? TypeTitre { get; set; }

    public string? Rafemetteur { get; set; }

    public decimal? NominalTitre { get; set; }

    public string? CodeDeviseNominal { get; set; }

    public string? DateEcheanceTitre { get; set; }

    public string? IndexTaux { get; set; }

    public string? TypeTaux { get; set; }

    public decimal? ValeurOuMargeTauxFixe { get; set; }

    public string? DeviseCoupon { get; set; }

    public string? TitreDette { get; set; }

    public decimal? CoursTitre { get; set; }

    public string? NombreDecimaleCours { get; set; }

    public string? TitreGaranti { get; set; }

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
