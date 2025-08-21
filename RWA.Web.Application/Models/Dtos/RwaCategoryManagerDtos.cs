using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class MissingMappingRowDto
    {
        public string Identifiant { get; set; } = null!;
        public string Source { get; set; } = null!;
        public string Cat1 { get; set; } = null!;
        public string? Cat2 { get; set; }
        public string? ChosenCategorieRwa { get; set; }
        public string? ChosenTypeBloomberg { get; set; }
    }

    public class CategorieRwaOptionDto
    {
        public string IdCatRwa { get; set; } = null!;
        public string Libelle { get; set; } = null!;
    }

    public class TypeBloombergOptionDto
    {
        public string IdTypeBloomberg { get; set; } = null!;
        public string Libelle { get; set; } = null!;
    }

    public class RwaCategoryManagerPayloadDto
    {
        public List<MissingMappingRowDto> MissingMappingRows { get; set; } = new List<MissingMappingRowDto>();
        public List<CategorieRwaOptionDto> CategorieRwaOptions { get; set; } = new List<CategorieRwaOptionDto>();
        public List<TypeBloombergOptionDto> TypeBloombergOptions { get; set; } = new List<TypeBloombergOptionDto>();
    }

    public class SubmitRwaMappingsDto
    {
        public List<RwaMappingSubmissionDto> Mappings { get; set; } = new List<RwaMappingSubmissionDto>();
    }

    public class RwaMappingSubmissionDto
    {
        public string Identifiant { get; set; } = null!;
        public string Source { get; set; } = null!;
        public string Cat1 { get; set; } = null!;
        public string? Cat2 { get; set; }
        public string CategorieRwaId { get; set; } = null!;
        public string? TypeBloombergId { get; set; }
    }
}
