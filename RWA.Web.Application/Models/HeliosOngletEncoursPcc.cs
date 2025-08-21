using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosOngletEncoursPcc
{
    public string CodeUfonegociatrice { get; set; } = null!;

    public string CodeEtablissement { get; set; } = null!;

    public string IdentifiantContrat { get; set; } = null!;

    public string NumeroEncours { get; set; } = null!;

    public string Raf { get; set; } = null!;

    public string? Pccencours { get; set; }

    public decimal? ValeurTauxFixe { get; set; }

    public string TypeEncours { get; set; } = null!;

    public decimal MontantEncours { get; set; }

    public string DeviseEncours { get; set; } = null!;

    public string? DateDebutImpact { get; set; }

    public string? DateFinImpact { get; set; }

    public string FlagEncoursMcDo { get; set; } = null!;

    public string? PeriodiciteInterets { get; set; }

    public string? FixiteTauxContrat { get; set; }

    public decimal? Elcredit { get; set; }

    public decimal? Eldilution { get; set; }

    public string NumTirage { get; set; } = null!;
}
