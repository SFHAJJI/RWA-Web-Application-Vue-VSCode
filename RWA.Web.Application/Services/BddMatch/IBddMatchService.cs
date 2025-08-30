using System.Threading;
using System.Threading.Tasks;

namespace RWA.Web.Application.Services.BddMatch
{
    public interface IBddMatchService
    {
        Task ComputeAndPersistAsync(string version, CancellationToken ct = default);
    }
}

