export interface HecateInventaireNormaliseDto {
    numLigne: number;
    periodeCloture: string;
    source: string;
    refCategorieRwa: string;
    identifiantUniqueRetenu: string;
    raf: string;
    libelleOrigine: string;
    dateFinContrat: string;
    identifiantOrigine: string;
    valeurDeMarche: number;
    categorie1: string;
    categorie2: string;
    deviseDeCotation: string;
    tauxObligation: number | null;
    dateMaturite: string | null;
    dateExpiration: string | null;
    tiers: string;
    boaSj: string;
    boaContrepartie: string;
    boaDefaut: string;
    bloomberg: string;
    isTauxObligationInvalid: boolean;
    isDateMaturiteInvalid: boolean;
}

export interface HecateInterneHistoriqueDto {
    source: string;
    refCategorieRwa: string;
    identifiantUniqueRetenu: string;
    raf: string;
    libelleOrigine: string;
    dateEcheance: string | null;
    identifiantOrigine: string;
    bbgticker: string | null;
    libelleTypeDette: string | null;
}
