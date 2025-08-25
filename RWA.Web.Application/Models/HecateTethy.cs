using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RWA.Web.Application.Models;

public partial class HecateTethy
{
    public string? IdentifiantRaf { get; set; }

    public string? LibelleCourt { get; set; }

    public string? RaisonSociale { get; set; }

    public string? PaysDeResidence { get; set; }

    public string? PaysDeNationalite { get; set; }

    public string? NumeroEtNomDeRue { get; set; }

    public string? Ville { get; set; }

    public string? CategorieTethys { get; set; }

    public string? NafNace { get; set; }

    public string? CodeIsin { get; set; }

    public string? SegmentDeRisque { get; set; }

    public string? SegmentationBpce { get; set; }

    public string? CodeCusip { get; set; }

    public string? RafTeteGroupeReglementaire { get; set; }

    public string? NomTeteGroupeReglementaire { get; set; }

    public string? DateNotationInterne { get; set; }

    public string? CodeNotation { get; set; }

    public string? CodeConso { get; set; }

    public string? CodeApparentement { get; set; }

    public HecateTethy TrimProperties()
    {
        IdentifiantRaf = IdentifiantRaf?.Trim();
        LibelleCourt = LibelleCourt?.Trim();
        RaisonSociale = RaisonSociale?.Trim();
        PaysDeResidence = PaysDeResidence?.Trim();
        PaysDeNationalite = PaysDeNationalite?.Trim();
        NumeroEtNomDeRue = NumeroEtNomDeRue?.Trim();
        Ville = Ville?.Trim();
        CategorieTethys = CategorieTethys?.Trim();
        NafNace = NafNace?.Trim();
        CodeIsin = CodeIsin?.Trim();
        SegmentDeRisque = SegmentDeRisque?.Trim();
        SegmentationBpce = SegmentationBpce?.Trim();
        CodeCusip = CodeCusip?.Trim();
        RafTeteGroupeReglementaire = RafTeteGroupeReglementaire?.Trim();
        NomTeteGroupeReglementaire = NomTeteGroupeReglementaire?.Trim();
        DateNotationInterne = DateNotationInterne?.Trim();
        CodeNotation = CodeNotation?.Trim();
        CodeConso = CodeConso?.Trim();
        CodeApparentement = CodeApparentement?.Trim();
        return this;
    }
}
