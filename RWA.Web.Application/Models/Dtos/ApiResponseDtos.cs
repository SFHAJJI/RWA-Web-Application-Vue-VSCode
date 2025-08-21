namespace RWA.Web.Application.Models.Dtos
{
    public class ApplyMappingsResponseDto
    {
        public WorkflowStepDto[] Steps { get; set; } = System.Array.Empty<WorkflowStepDto>();
        public ValidationMessageDto[] Validation { get; set; } = System.Array.Empty<ValidationMessageDto>();
        public int Updated { get; set; }
        public EquivalenceApplyResultDto? EquivalenceResult { get; set; }
    }

    public class EquivalenceApplyResultDto
    {
    public (long Id, string Libelle)[] CreatedDepositaire1 { get; set; } = System.Array.Empty<(long, string)>();
    public (long Id, string Libelle)[] CreatedDepositaire2 { get; set; } = System.Array.Empty<(long, string)>();
    public (string Id, string Libelle)[] CreatedCategorie { get; set; } = System.Array.Empty<(string, string)>();
    public (string Id, string Libelle)[] CreatedTypeBloomberg { get; set; } = System.Array.Empty<(string, string)>();
    public int CreatedOrUpdatedCount { get; set; }
    }

    public class MissingRowDto
    {
        public string Identifiant { get; set; } = string.Empty;
    // hidden id for the inventory row to allow posts to reference the exact row
    public string IdentifiantOrigine { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string? Categorie1 { get; set; }
        public string? Categorie2 { get; set; }
        public int NumLigne { get; set; }
    public long? Depositaire1Id { get; set; }
    public bool Depositaire1Created { get; set; }
    public long? Depositaire2Id { get; set; }
    public bool Depositaire2Created { get; set; }
        public string? SuggestedRefCategorieRwa { get; set; }
        public double Confidence { get; set; }
    }

    public class EquivalenceCandidateDto
    {
        public string Source { get; set; } = string.Empty;
    public string Depositaire1 { get; set; } = string.Empty; // LibelleDepositaire1
    public long? Depositaire1Id { get; set; }
    public bool Depositaire1Created { get; set; }
    public string? Depositaire2 { get; set; }
    public long? Depositaire2Id { get; set; }
    public bool Depositaire2Created { get; set; }
        public string? RefCategorieRwaLibelle { get; set; }
    public string? RefCategorieRwaId { get; set; }
        public string? RefTypeBloombergLibelle { get; set; }
    public string? RefTypeBloombergId { get; set; }
        public string IdentifiantOrigine { get; set; } = string.Empty;
    }

    public class EquivalenceMappingDto
    {
        public string Source { get; set; } = string.Empty;
    // client may post ids (preferred) or libelles; server will accept both and create missing refs as needed
    public long? Depositaire1Id { get; set; }
    public string Depositaire1 { get; set; } = string.Empty; // Libelle fallback
    public long? Depositaire2Id { get; set; }
    public string? Depositaire2 { get; set; }
    public string? RefCategorieRwaId { get; set; }
    public string RefCategorieRwaLibelle { get; set; } = string.Empty; // chosen by user from dropdown
    public string? RefTypeBloombergId { get; set; }
    public string? RefTypeBloombergLibelle { get; set; }
    public string IdentifiantOrigine { get; set; } = string.Empty; // fuel row identifier
    }
}
