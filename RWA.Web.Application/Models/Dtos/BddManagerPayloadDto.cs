using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class BddManagerPayloadDto
    {
        public List<HecateInventaireNormalise> SuccessfulMatches { get; set; }
        public List<HecateInventaireNormalise> FailedMatches { get; set; }
        public List<HecateInterneHistorique> ToBeAddedToBDD { get; set; }
    }
}
