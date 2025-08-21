using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateEquivalenceCatRwa
{
    public string Source { get; set; } = null!;

    public long RefCatDepositaire1 { get; set; }

    public long RefCatDepositaire2 { get; set; }

    public string RefCategorieRwa { get; set; } = null!;

    public string? RefTypeBloomberg { get; set; }

    public virtual HecateCatDepositaire1 RefCatDepositaire1Navigation { get; set; } = null!;

    public virtual HecateCatDepositaire2 RefCatDepositaire2Navigation { get; set; } = null!;

    public virtual HecateCategorieRwa RefCategorieRwaNavigation { get; set; } = null!;

    public virtual HecateTypeBloomberg? RefTypeBloombergNavigation { get; set; }
}
