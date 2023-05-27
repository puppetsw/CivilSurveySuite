using System.Collections.Generic;
using System.Threading.Tasks;
using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface IConnectLineworkService
    {
        string DescriptionKeyFile { get; set; }

        double MidOrdinate { get; set; }

        Task ConnectCogoPoints(IReadOnlyList<DescriptionKey> descriptionKeys);
    }
}
