using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AroFloApi
{
    public class LocationService : ILocationService
    {
        public async Task<Location> GetLocationAsync(string locationId, CancellationToken cancellationToken = default)
        {
            var aroFloController = new AroFloController();
            var locations = await aroFloController.GetAroFloObjectsAsync<LocationZoneResult, Location>(cancellationToken);
            return locations.FirstOrDefault(p => p.LocationId.Equals(locationId));
        }
    }
}
