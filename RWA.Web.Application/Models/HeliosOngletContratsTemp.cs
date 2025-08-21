using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosOngletContratsTemp
{
    public string CodeUfonegociatrice { get; set; } = null!;

    public string CodeEtablissement { get; set; } = null!;

    public string CodeProduit { get; set; } = null!;

    public string DateFinContrat { get; set; } = null!;

    public string DateInitialeFinContrat { get; set; } = null!;

    public string DateSignatureContrat { get; set; } = null!;

    public string IdentifiantContrat { get; set; } = null!;

    public decimal? TauxLgd { get; set; }

    public decimal? Fcec { get; set; }

    public string DateEffet { get; set; } = null!;

    public string? ReferenceMontage { get; set; }

    public string PortefeuilleIas { get; set; } = null!;

    public string PtfReglementaireMcDo { get; set; } = null!;

    public string? PaysRisque { get; set; }

    public string PresenceCreditPartage { get; set; } = null!;

    public string Devise { get; set; } = null!;

    public string Raftiers { get; set; } = null!;

    public string? CodeSegmentMcDo { get; set; }

    public string? IdentifiantIsin { get; set; }

    public string Diversifiee { get; set; } = null!;

    public string IndicateurPerimetreMcDo { get; set; } = null!;

    public string? MotifExclusionMcDo { get; set; }

    public string? DateDebutFinancement { get; set; }

    public decimal NbEncoursElementaires { get; set; }

    public string? StrategieFrench { get; set; }

    public string MethodeBaleIi { get; set; } = null!;

    public string? TypeParticipation { get; set; }

    public string? TypeContratNetting { get; set; }

    public string? ReferenceContratNettingReglem { get; set; }

    public string? SensOperation { get; set; }

    public string? LibelleTypeDette { get; set; }

    public string CodeActivite { get; set; } = null!;

    public string PresenceGarantie { get; set; } = null!;

    public decimal NombreGaranties { get; set; }

    public string? ProduitAs400 { get; set; }

    public string? TypeOption { get; set; }

    public decimal? PrixExerciceOption { get; set; }

    public string? FlagComptesSegregues { get; set; }

    public string? Quantite { get; set; }

    public decimal? PrixNegocie { get; set; }

    public decimal? PrixUnitaireContrat { get; set; }

    public string? Section { get; set; }

    public string TopTransparence { get; set; } = null!;

    public double? TauxProbabiliteDefaut { get; set; }

    public string? ClasseLgd { get; set; }

    public string? PourcentageLoanToValueActualized { get; set; }

    public string? PourcentageLoanToValueOrigination { get; set; }

    public string? PourcentageLoanToValueCrr3 { get; set; }

    public string? ManagementIntent { get; set; }

    public string? LastUpdate { get; set; }
}
