using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public sealed class AssignRafBatchRequest
    {
        public List<int> NumLignes { get; set; } = new();
        public string Raf { get; set; } = string.Empty;
        public string? CptTethys { get; set; }
    }
}

