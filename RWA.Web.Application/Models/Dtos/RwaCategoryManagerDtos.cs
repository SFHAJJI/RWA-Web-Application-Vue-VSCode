using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class RwaMappingRowDto
    {
        public List<int> NumLignes { get; set; } = new List<int>();
        public string Source { get; set; } = null!;
        public string Cat1 { get; set; } = null!;
        public string? Cat2 { get; set; }
        public string? CategorieRwaId { get; set; }
        public string? TypeBloombergId { get; set; }
    }

    public class CategorieRwaOptionDto
    {
        public string IdCatRwa { get; set; } = null!;
        public string Libelle { get; set; } = null!;
        public string ValeurMobiliere { get; set; } = null!;
    }

    public class TypeBloombergOptionDto
    {
        public string IdTypeBloomberg { get; set; } = null!;
        public string Libelle { get; set; } = null!;
    }

    public class RwaCategoryManagerPayloadDto
    {
        public List<RwaMappingRowDto> MissingMappingRows { get; set; } = new List<RwaMappingRowDto>();
        public List<CategorieRwaOptionDto> CategorieRwaOptions { get; set; } = new List<CategorieRwaOptionDto>();
        public List<TypeBloombergOptionDto> TypeBloombergOptions { get; set; } = new List<TypeBloombergOptionDto>();
    }

    public class SubmitRwaMappingsDto
    {
        public List<RwaMappingRowDto> Mappings { get; set; } = new List<RwaMappingRowDto>();
    }

    public class EnrichedInventaireDto
    {
        public int NumLigne { get; set; }
        public bool IsValeurMobiliere { get; set; }
        public string Libelle { get; set; } = null!;
    }
}
