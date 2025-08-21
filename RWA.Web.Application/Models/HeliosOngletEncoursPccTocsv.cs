using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosOngletEncoursPccTocsv
{
    public string? CodeUfonegociatrice { get; set; }

    public string? CodeEtablissement { get; set; }

    public string? IdentifiantContrat { get; set; }

    public string? NumeroEncours { get; set; }

    public string? Raf { get; set; }

    public string? Pccencours { get; set; }

    public decimal? ValeurTauxFixe { get; set; }

    public string? TypeEncours { get; set; }

    public decimal? MontantEncours { get; set; }

    public string? DeviseEncours { get; set; }

    public string? DateDebutImpact { get; set; }

    public string? DateFinImpact { get; set; }

    public string? FlagEncoursMcDo { get; set; }

    public string? PeriodiciteInterets { get; set; }

    public string? FixiteTauxContrat { get; set; }

    public decimal? Elcredit { get; set; }

    public decimal? Eldilution { get; set; }

    public string? NumTirage { get; set; }
}
