using System.Data;
using System.Threading.Tasks;

namespace RWA.Web.Application.Services.Workflow
{
    public interface IInventoryMapper
    {
        /// <summary>
        /// Map rows from a DataTable to serializable objects for payload storage and to domain entities for persistence.
        /// </summary>
        /// <param name="table">Parsed DataTable with header row</param>
        /// <returns>Tuple of (serializable objects list as JSON string, number of mapped rows)</returns>
        Task<(string jsonRows, int mappedCount)> MapAsync(DataTable table);
    }
}
