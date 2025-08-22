namespace RWA.Web.Application.Models
{
    public class ExcelColumnMappings
    {
        public BDDHistoriqueMapping BDDHistorique { get; set; } = new();
        public EquivalenceCatRWAMapping EquivalenceCatRWA { get; set; } = new();
    }

    public class BDDHistoriqueMapping
    {
        public string Source { get; set; } = string.Empty;
        public string CategorieRWA { get; set; } = string.Empty;
        public string IdentifiantUniqueRetenu { get; set; } = string.Empty;
        public string RAF { get; set; } = string.Empty;
        public string LibelleOrigine { get; set; } = string.Empty;
        public string DateEcheance { get; set; } = string.Empty;
        public string IdentifiantOrigine { get; set; } = string.Empty;
    }

    public class EquivalenceCatRWAMapping
    {
        public CategorieRWAMapping CategorieRWA { get; set; } = new();
        public TypeBloombergMapping TypeBloomberg { get; set; } = new();
        public CatDepositaireMapping CatDepositaire1 { get; set; } = new();
        public CatDepositaireMapping CatDepositaire2 { get; set; } = new();
        public EquivalenceCatRWATableMapping EquivalenceCatRWA { get; set; } = new();
    }

    public class CategorieRWAMapping
    {
        public string CategorieRWA { get; set; } = string.Empty;
        public string DescriptionCategorieRWA { get; set; } = string.Empty;
    }

    public class TypeBloombergMapping
    {
        public string TypeBloomberg { get; set; } = string.Empty;
        public string DescriptionTypeBloomberg { get; set; } = string.Empty;
    }

    public class CatDepositaireMapping
    {
        public string CatDepositaire { get; set; } = string.Empty;
        public string DescriptionCatDepositaire { get; set; } = string.Empty;
    }

    public class EquivalenceCatRWATableMapping
    {
        public string CatDepositaire1 { get; set; } = string.Empty;
        public string CatDepositaire2 { get; set; } = string.Empty;
        public string TypeBloomberg { get; set; } = string.Empty;
        public string CategorieRWA { get; set; } = string.Empty;
    }
}
