using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class RwaTrace
{
    public string? Login { get; set; }

    public string? Section { get; set; }

    public string? Step { get; set; }

    public string? Info { get; set; }

    public DateTime Dt { get; set; }
}
