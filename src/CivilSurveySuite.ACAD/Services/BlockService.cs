using System.Collections.Generic;
using CivilSurveySuite.Common.Models;
using CivilSurveySuite.Common.Services.Interfaces;

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
