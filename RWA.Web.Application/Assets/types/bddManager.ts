export interface HecateInventaireNormaliseDto {
    numLigne: string;
    periodeCloture: string;
    source: string;
    refCategorieRwa: string;
    identifiantUniqueRetenu: string;
    raf: string;
    libelleOrigine: string;
    dateFinContrat: string;
    identifiantOrigine: string;
    valeurDeMarche: string;
    categorie1: string;
    categorie2: string;
    deviseDeCotation: string;
    tauxObligation: string;
    dateMaturite: string;
    dateExpiration: string;
    tiers: string;
    boaSj: string;
    boaContrepartie: string;
    boaDefaut: string;
    bloomberg: string;
}

export interface HecateInterneHistoriqueDto {
    source: string;
    refCategorieRwa: string;
    identifiantUniqueRetenu: string;
    raf: string;
    libelleOrigine: string;
    dateEcheance: string;
    identifiantOrigine: string;
    bbgticker: string;
    libelleTypeDette: string;
}
