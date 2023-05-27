using System.Collections.Generic;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;

namespace CivilSurveySuite.ACAD.Services
{
    public class BlockService : IBlockService
    {
        public List<AcadBlock> GetBlocks()
        {
            return new List<AcadBlock>(BlockUtils.GetBlocks());
        }
    }
}
