using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class VwHeliosOngletTitresCsv
{
    public string IdentifiantUniqueRetenu { get; set; } = null!;

    public string IdentifiantTitre { get; set; } = null!;

    public string NatureTitre { get; set; } = null!;

    public string TypeTitre { get; set; } = null!;

    public string Rafemetteur { get; set; } = null!;

    public string NominalTitre { get; set; } = null!;

    public string CodeDeviseNominal { get; set; } = null!;

    public string DateEcheanceTitre { get; set; } = null!;

    public string IndexTaux { get; set; } = null!;

    public string TypeTaux { get; set; } = null!;

    public string ValeurOuMargeTauxFixe { get; set; } = null!;

    public string DeviseCoupon { get; set; } = null!;

    public string TitreDette { get; set; } = null!;

    public string CoursTitre { get; set; } = null!;

    public string NombreDecimaleCours { get; set; } = null!;

    public string TitreGaranti { get; set; } = null!;

    public string RafgarantTitre { get; set; } = null!;

    public string TitreLiquide { get; set; } = null!;

    public string TitreCote { get; set; } = null!;

    public string IndiceCotation { get; set; } = null!;

    public string FrequenceCotation { get; set; } = null!;

    public string PlaceCotation { get; set; } = null!;

    public string RangTitre { get; set; } = null!;

    public string TitreNote { get; set; } = null!;

    public string NotationTitre { get; set; } = null!;

    public string Oeec { get; set; } = null!;

    public string EligibiliteMcDo { get; set; } = null!;

    public string OpcvmcomposeTitresEligibles { get; set; } = null!;

    public string Granularite { get; set; } = null!;

    public string DateNotation { get; set; } = null!;
}
