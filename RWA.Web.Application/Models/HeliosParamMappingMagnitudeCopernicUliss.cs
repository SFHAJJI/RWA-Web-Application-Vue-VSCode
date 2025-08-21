using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamMappingMagnitudeCopernicUliss
{
    public string CompteMagnitude { get; set; } = null!;

    public string Signe { get; set; } = null!;

    public string RefCategorieRwa { get; set; } = null!;

    public string? CompteCopernic { get; set; }

    public string? CodeProduit { get; set; }

    public string? Pcec { get; set; }

    public string? AjoutDeLignes { get; set; }

    public string? Signe2 { get; set; }

    public string? RefCategorieRwa2 { get; set; }

    public string? CompteCopernic2 { get; set; }

    public string? CodeProduit2 { get; set; }

    public string? Pcec2 { get; set; }

    public string? Signe3 { get; set; }

    public string? RefCategorieRwa3 { get; set; }

    public string? CompteCopernic3 { get; set; }

    public string? CodeProduit3 { get; set; }

    public string? Pcec3 { get; set; }

    public string LastUpdate { get; set; } = null!;
}
