using System.Collections.Generic;
using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface IBlockService
    {
        List<AcadBlock> GetBlocks();
    }
}
