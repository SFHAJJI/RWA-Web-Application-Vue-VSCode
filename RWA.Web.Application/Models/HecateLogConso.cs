using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateLogConso
{
    public int NumLigne { get; set; }

    public DateTime LogTime { get; set; }

    public long RefTypeLog { get; set; }

    public string PeriodeCloture { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string IdentifiantOrigine { get; set; } = null!;

    public string? Message { get; set; }

    public virtual HecateTypeLog RefTypeLogNavigation { get; set; } = null!;
}
