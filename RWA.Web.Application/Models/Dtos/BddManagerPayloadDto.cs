using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class BddManagerPayload
    {
        public List<HecateInventaireNormalise> OBLValidatorPayload { get; set; }
        public List<HecateInterneHistorique> AddToBDDPayload { get; set; }
    }

    public class BddManagerPayloadDto
    {
        public List<AuditInventoryDto> AuditInventoryPayload { get; set; }
        public BddManagerPayload BDDManagerPayload { get; set; }
    }
}
