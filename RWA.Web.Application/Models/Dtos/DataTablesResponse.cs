using System.Collections.Generic;

namespace RWA.Web.Application.Models.Dtos
{
    public class DataTablesResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
    }
}
