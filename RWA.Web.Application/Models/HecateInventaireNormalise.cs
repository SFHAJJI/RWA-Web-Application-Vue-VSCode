using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RWA.Web.Application.Models.Dtos;
using System.Text.Json;

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

    private AdditionalInformation _additionalInformation = new AdditionalInformation();

    [NotMapped]
    public AdditionalInformation AdditionalInformation
    {
        get
        {
            if (!string.IsNullOrEmpty(Commentaires))
            {
                try
                {
                    var deserialized = JsonSerializer.Deserialize<AdditionalInformation>(Commentaires);
                    if (deserialized != null)
                    {
                        _additionalInformation = deserialized;
                    }
                }
                catch
                {
                    // Fail silently as requested
                }
            }
            return _additionalInformation;
        }
        set
        {
            _additionalInformation = value;
            try
            {
                Commentaires = JsonSerializer.Serialize(value);
            }
            catch
            {
                // Fail silently as requested
            }
        }
    }

    public string? Commentaires { get; set; }

    public string? Bloomberg { get; set; }

    public int RefTypeDepot { get; set; }

    public int RefTypeResultat { get; set; }

    public int? CodeResultat { get; set; }

    public virtual HecateTypeDepot RefTypeDepotNavigation { get; set; } = null!;

    public virtual HecateTypeResultat RefTypeResultatNavigation { get; set; } = null!;

    public HecateInterneHistoriqueDto ToBddHistoDto()
    {
        return new HecateInterneHistoriqueDto
        {
            Source = this.Source,
            IdentifiantOrigine = this.IdentifiantOrigine,
            RefCategorieRwa = this.RefCategorieRwa,
            IdentifiantUniqueRetenu = this.IdentifiantUniqueRetenu,
            Raf = this.Raf,
            LibelleOrigine = this.LibelleOrigine,
            DateEcheance = this.DateFinContrat?.ToString("dd/MM/yyyy") ?? string.Empty
        };
    }

    public HecateInventaireNormaliseDto ToDto()
    {
        return new HecateInventaireNormaliseDto
        {
            NumLigne = this.NumLigne.ToString(),
            PeriodeCloture = this.PeriodeCloture,
            Source = this.Source,
            RefCategorieRwa = this.RefCategorieRwa ?? string.Empty,
            IdentifiantUniqueRetenu = this.IdentifiantUniqueRetenu ?? string.Empty,
            Raf = this.Raf ?? string.Empty,
            LibelleOrigine = this.LibelleOrigine ?? string.Empty,
            DateFinContrat = this.DateFinContrat?.ToString("dd/MM/yyyy") ?? string.Empty,
            IdentifiantOrigine = this.IdentifiantOrigine,
            ValeurDeMarche = this.ValeurDeMarche.ToString(),
            Categorie1 = this.Categorie1,
            Categorie2 = this.Categorie2 ?? string.Empty,
            DeviseDeCotation = this.DeviseDeCotation,
            TauxObligation = (this.TauxObligation ?? 0).ToString(),
            DateMaturite = this.DateMaturite?.ToString("dd/MM/yyyy") ?? string.Empty,
            DateExpiration = this.DateExpiration?.ToString("dd/MM/yyyy") ?? string.Empty,
            Tiers = this.Tiers ?? string.Empty,
            BoaSj = this.BoaSj ?? string.Empty,
            BoaContrepartie = this.BoaContrepartie ?? string.Empty,
            BoaDefaut = this.BoaDefaut ?? string.Empty,
            Bloomberg = this.Bloomberg ?? string.Empty
        };
    }
}
