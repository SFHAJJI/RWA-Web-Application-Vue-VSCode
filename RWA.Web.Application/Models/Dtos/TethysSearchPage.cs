using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class TethysSearchPage
    {
        public IEnumerable<TethysSearchRowDto> Items { get; set; } = new List<TethysSearchRowDto>();
        public string? NextCursor { get; set; }
        public int Total { get; set; }
    }
}

