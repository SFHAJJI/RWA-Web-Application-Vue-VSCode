using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RWA.Web.Application.Models;

public partial class HecateInventaireNormalise
{
    public string PeriodeCloture { get; set; } = null!;

    public string Source { get; set; } = null!;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NumLigne { get; set; }

    public string Identifiant { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public double ValeurDeMarche { get; set; }

    public string Categorie1 { get; set; } = null!;

    public string? Categorie2 { get; set; }

    public string DeviseDeCotation { get; set; } = null!;

    public decimal? TauxObligation { get; set; }

    public DateOnly? DateMaturite { get; set; }

    public DateOnly? DateExpiration { get; set; }

    public string? Tiers { get; set; }

    public string? Raf { get; set; }

    public string? BoaSj { get; set; }

    public string? BoaContrepartie { get; set; }

    public string? BoaDefaut { get; set; }

    public string IdentifiantOrigine { get; set; } = null!;

    public string? RefCategorieRwa { get; set; }

    public string? IdentifiantUniqueRetenu { get; set; }

    public string? Rafenrichi { get; set; }

    public string? LibelleOrigine { get; set; }

    public DateOnly? DateFinContrat { get; set; }

    public string? Commentaires { get; set; }

    public string? Bloomberg { get; set; }

    public int RefTypeDepot { get; set; }

    public int RefTypeResultat { get; set; }

    public int? CodeResultat { get; set; }

    public virtual HecateTypeDepot RefTypeDepotNavigation { get; set; } = null!;

    public virtual HecateTypeResultat RefTypeResultatNavigation { get; set; } = null!;
}
