using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosControlesCoherenceMontantsEncour
{
    public string? Compte { get; set; }

    public string? Source { get; set; }

    public int? MontantEncours { get; set; }

    public int? MontantReferenceMagnitude { get; set; }

    public int? Ecart { get; set; }

    public string? Commentaires { get; set; }
}
