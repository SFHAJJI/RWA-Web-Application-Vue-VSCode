using System;
using System.ComponentModel.DataAnnotations;

namespace RWA.Web.Application.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Utilisateur")]
    public string Username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; }

    public string? ErrorMessage { get; set; }
}
