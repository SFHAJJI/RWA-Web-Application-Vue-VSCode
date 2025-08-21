using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HeliosLogImportExport
{
    public string? Message { get; set; }

    public DateTime LogTime { get; set; }

    public long RefTypeLog { get; set; }

    public string? PeriodeCloture { get; set; }

    public string? Source { get; set; }

    public string Process { get; set; } = null!;

    public int CodeErreur { get; set; }

    public virtual HeliosTypeLog RefTypeLogNavigation { get; set; } = null!;
}
