using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class DataTableRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; }
        public bool SortDesc { get; set; }
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
