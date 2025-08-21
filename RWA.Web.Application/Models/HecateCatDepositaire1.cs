using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateCatDepositaire1
{
    public long IdDepositaire1 { get; set; }

    public string LibelleDepositaire1 { get; set; } = null!;

    public virtual ICollection<HecateEquivalenceCatRwa> HecateEquivalenceCatRwas { get; set; } = new List<HecateEquivalenceCatRwa>();
}
