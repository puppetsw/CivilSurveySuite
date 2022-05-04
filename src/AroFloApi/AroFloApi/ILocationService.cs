using System.Threading;
using System.Threading.Tasks;

namespace AroFloApi
{
    public interface ILocationService
    {
        Task<Location> GetLocationAsync(string locationId, CancellationToken cancellationToken);
    }
}
