using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateCatDepositaire2
{
    public long IdDepositaire2 { get; set; }

    public string LibelleDepositaire2 { get; set; } = null!;

    public virtual ICollection<HecateEquivalenceCatRwa> HecateEquivalenceCatRwas { get; set; } = new List<HecateEquivalenceCatRwa>();
}
