using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamEncoursElementairesPcc
{
    public string CodeProduit { get; set; } = null!;

    public string Pcc { get; set; } = null!;

    public int NombreEncours { get; set; }

    public string? NumeroEncours1 { get; set; }

    public string? Pccencours1 { get; set; }

    public string? TypeEncours1 { get; set; }

    public float? SigneEncours1 { get; set; }

    public string? NumeroEncours2 { get; set; }

    public string? Pccencours2 { get; set; }

    public string? TypeEncours2 { get; set; }

    public float? SigneEncours2 { get; set; }

    public string? NumeroEncours3 { get; set; }

    public string? Pccencours3 { get; set; }

    public string? TypeEncours3 { get; set; }

    public float? SigneEncours3 { get; set; }

    public string LastUpdate { get; set; } = null!;
}
