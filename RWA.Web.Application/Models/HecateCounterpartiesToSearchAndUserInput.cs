using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class HecateCounterpartiesToSearchAndUserInput
{
    public string Counterparty { get; set; } = null!;

    public string Source { get; set; } = null!;

    public string? UserSearch1 { get; set; }

    public string? UserSearch2 { get; set; }

    public string? UserSearch3 { get; set; }
}
