using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class TethysStatusPage
    {
        public IEnumerable<HecateTethysDto> Items { get; set; }
        public string NextCursor { get; set; }
        public int Total { get; set; }
    }
}
