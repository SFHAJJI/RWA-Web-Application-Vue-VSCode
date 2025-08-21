using System;
using System.Collections.Generic;

namespace RWA.Web.Application.Models;

public partial class CommonUser
{
    public int Userid { get; set; }

    public string? Username { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Emailaddress { get; set; }

    public bool Isactive { get; set; }
}
