using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosParamPerimetreDette
{
    public string CodeProduit { get; set; } = null!;

    public string? LibelleProduit { get; set; }

    public string LastUpdate { get; set; } = null!;
}
