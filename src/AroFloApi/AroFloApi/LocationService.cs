using System.Threading;
using System.Threading.Tasks;

namespace AroFloApi
{
    public class LocationService : ILocationService
    {
        public async Task<Location> GetLocationAsync(string locationId, CancellationToken cancellationToken = default)
        {
            var aroFloController = new AroFloController();
            return await aroFloController.GetAroFloObject<LocationZoneResult, Location>(Fields.LocationId, locationId, cancellationToken);
        }
    }
}
