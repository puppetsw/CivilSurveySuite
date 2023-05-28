using System.Collections.Generic;
using System.Threading.Tasks;
using CivilSurveySuite.Common.Models;

namespace CivilSurveySuite.Common.Services.Interfaces
{
    public interface IConnectLineworkService
    {
        string DescriptionKeyFile { get; set; }

        double MidOrdinate { get; set; }

        Task ConnectCogoPoints(IReadOnlyList<DescriptionKey> descriptionKeys);
    }
}
