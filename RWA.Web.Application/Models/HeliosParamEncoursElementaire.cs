using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamEncoursElementaire
{
    public string CodeProduit { get; set; } = null!;

    public string Pcec { get; set; } = null!;

    public int NombreEncours { get; set; }

    public string? NumeroEncours1 { get; set; }

    public string? Pcecencours1 { get; set; }

    public string? TypeEncours1 { get; set; }

    public float? SigneEncours1 { get; set; }

    public string? NumeroEncours2 { get; set; }

    public string? Pcecencours2 { get; set; }

    public string? TypeEncours2 { get; set; }

    public float? SigneEncours2 { get; set; }

    public string? NumeroEncours3 { get; set; }

    public string? Pcecencours3 { get; set; }

    public string? TypeEncours3 { get; set; }

    public float? SigneEncours3 { get; set; }

    public string LastUpdate { get; set; } = null!;
}
