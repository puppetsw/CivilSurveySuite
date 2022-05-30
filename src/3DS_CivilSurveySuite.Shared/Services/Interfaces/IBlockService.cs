using System.Collections.Generic;
using _3DS_CivilSurveySuite.Shared.Models;

namespace _3DS_CivilSurveySuite.Shared.Services.Interfaces
{
    public interface IBlockService
    {
        List<AcadBlock> GetBlocks();
    }
}
