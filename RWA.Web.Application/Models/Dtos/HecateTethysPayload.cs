using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class HecateTethysPayload
    {
        public List<HecateTethysDto> Dtos { get; set; }

        public HecateTethysPayload()
        {
            Dtos = new List<HecateTethysDto>();
        }
    }
}
