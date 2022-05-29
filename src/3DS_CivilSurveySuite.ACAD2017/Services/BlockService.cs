using System.Collections.Generic;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;

namespace _3DS_CivilSurveySuite.ACAD2017.Services
{
    public class BlockService : IBlockService
    {
        public List<AcadBlock> GetBlocks()
        {
            return new List<AcadBlock>(BlockUtils.GetBlocks());
        }
    }
}
