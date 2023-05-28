using System.Collections.Generic;
using CivilSurveySuite.Common.Models;

namespace CivilSurveySuite.Common.Services.Interfaces
{
    public interface IBlockService
    {
        List<AcadBlock> GetBlocks();
    }
}
