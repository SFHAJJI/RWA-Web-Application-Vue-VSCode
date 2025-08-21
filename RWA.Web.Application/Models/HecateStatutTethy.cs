using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateStatutTethy
{
    public string Counterparty { get; set; } = null!;

    public string Source { get; set; } = null!;

    public int NumLigne { get; set; }

    public string? Raf { get; set; }

    public string StatutTethys { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
