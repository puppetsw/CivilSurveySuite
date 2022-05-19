using System.Threading;
using System.Threading.Tasks;
using AroFloApi.Enums;
using AroFloApi.Models;
using AroFloApi.Responses;

namespace AroFloApi
{
    public static class LocationController
    {
        public static async Task<Location> GetLocationAsync(string locationId, CancellationToken cancellationToken = default)
        {
            var aroFloController = new AroFloController();
            return await aroFloController.GetAroFloObject<LocationZoneResponse, Location>(Field.LocationId, locationId, cancellationToken);
        }
    }
}
