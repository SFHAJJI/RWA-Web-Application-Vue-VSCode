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
        public EquivalenceCatRWATableMapping EquivalenceCatRWA { get; set; } = new();
    }

    public class CategorieRWAMapping
    {
        public string CodeRWA { get; set; } = string.Empty;
        public string LibelleCategorieRWA { get; set; } = string.Empty;
        public string ValeurMobiliere { get; set; } = string.Empty;
    }

    public class TypeBloombergMapping
    {
        public string LibelleBloomberg { get; set; } = string.Empty;
        public string CodeBloomberg { get; set; } = string.Empty;
    }
    public class EquivalenceCatRWATableMapping
    {
        public string Source { get; set; } = string.Empty;
        public string CodeDepositaire1 { get; set; } = string.Empty;
        public string CodeDepositaire2 { get; set; } = string.Empty;
        public string TypeBloomberg { get; set; } = string.Empty;
        public string CategorieRWA { get; set; } = string.Empty;
    }
}
