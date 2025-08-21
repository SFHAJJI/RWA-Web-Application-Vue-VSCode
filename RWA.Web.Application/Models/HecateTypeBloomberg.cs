using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateTypeBloomberg
{
    public string IdTypeBloomberg { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public virtual ICollection<HecateEquivalenceCatRwa> HecateEquivalenceCatRwas { get; set; } = new List<HecateEquivalenceCatRwa>();
}
