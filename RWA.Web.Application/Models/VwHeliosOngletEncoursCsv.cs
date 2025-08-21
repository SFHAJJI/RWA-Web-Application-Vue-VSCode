using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class VwHeliosOngletEncoursCsv
{
    public string CodeUfonegociatrice { get; set; } = null!;

    public string CodeEtablissement { get; set; } = null!;

    public string IdentifiantContrat { get; set; } = null!;

    public string NumeroEncours { get; set; } = null!;

    public string Raf { get; set; } = null!;

    public string Pcecencours { get; set; } = null!;

    public string ValeurTauxFixe { get; set; } = null!;

    public string TypeEncours { get; set; } = null!;

    public decimal MontantEncours { get; set; }

    public string DeviseEncours { get; set; } = null!;

    public string DateDebutImpact { get; set; } = null!;

    public string DateFinImpact { get; set; } = null!;

    public string FlagEncoursMcDo { get; set; } = null!;

    public string PeriodiciteInterets { get; set; } = null!;

    public string FixiteTauxContrat { get; set; } = null!;

    public string Elcredit { get; set; } = null!;

    public string Eldilution { get; set; } = null!;

    public string NumTirage { get; set; } = null!;
}
