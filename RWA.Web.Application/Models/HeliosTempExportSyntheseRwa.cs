using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosTempExportSyntheseRwa
{
    public string? Source { get; set; }

    public string? CompteMagnitude { get; set; }

    public string? PartnerId { get; set; }

    public string? IdentifiantUniqueRetenu { get; set; }

    public string? Raf { get; set; }

    public string? LibelleOrigine { get; set; }

    public string? CompteCopernic { get; set; }

    public string TraitementParTransparenceT1 { get; set; } = null!;

    public string CategorieSyntheseT1 { get; set; } = null!;

    public double? MontantProratiseT1 { get; set; }

    public int? MontantMagnitudeT1 { get; set; }

    public double? MontantActifPondereT1 { get; set; }

    public double? MontantActifPondereChangeT1 { get; set; }

    public double? TauxPonderationT1 { get; set; }

    public double? NotionnelT1 { get; set; }

    public double? PoderationEffectiveT1 { get; set; }

    public string Rafb2ct1 { get; set; } = null!;

    public string ApprocheT1 { get; set; } = null!;

    public string ClasseActifB2t1 { get; set; } = null!;

    public string CategorieProduitT1 { get; set; } = null!;

    public string RatingT1 { get; set; } = null!;

    public string? TraitementParTransparenceT2 { get; set; }

    public string? CategorieSyntheseT2 { get; set; }

    public double? MontantProratiseT2 { get; set; }

    public double? MontantActifPondereT2 { get; set; }

    public double? TauxPonderationT2 { get; set; }

    public double? NotionnelT2 { get; set; }

    public double? PoderationEffectiveT2 { get; set; }

    public string? Rafb2ct2 { get; set; }

    public string? ApprocheT2 { get; set; }

    public string? ClasseActifB2t2 { get; set; }

    public string? CategorieProduitT2 { get; set; }

    public string? RatingT2 { get; set; }

    public double? VariationEffetChange { get; set; }

    public double? VariationPonderation { get; set; }

    public double? VariationBruteMagnitude { get; set; }

    public double? VariationRelativeMagnitude { get; set; }

    public double? VariationBruteRwa { get; set; }

    public double? VariationRelativeRwa { get; set; }
}
