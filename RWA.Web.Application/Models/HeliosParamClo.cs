using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamClo
{
    public string ReferenceContratPivi { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string AssetId { get; set; } = null!;

    public string AssetDescription { get; set; } = null!;

    public string DateMaturite { get; set; } = null!;

    public string Raf { get; set; } = null!;

    public string DateCloture { get; set; } = null!;

    public string RatingCloture { get; set; } = null!;

    public string Moodys { get; set; } = null!;

    public string SandP { get; set; } = null!;

    public string AgenceCloture { get; set; } = null!;

    public string DateOctroi { get; set; } = null!;

    public string RatingOctroi { get; set; } = null!;

    public string AgenceOctroi { get; set; } = null!;

    public string LastUpdate { get; set; } = null!;
}
