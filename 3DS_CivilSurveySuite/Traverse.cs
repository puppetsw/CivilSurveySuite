using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Traverse))]
namespace _3DS_CivilSurveySuite
{
    public class Traverse : CivilBase
    {
        //[CommandMethod("3DSPointLabelRotate")]

        public struct DMS
        {
            public int Degrees;
            public int Minutes;
            public int Seconds;
        }

        public DMS ParseBearing(string bearing)
        {
            //bearing is 354.5020
            //bearing is 354.50
            //bearing is 354

            switch (bearing.Length)
            {
                case 8:
                    var degrees = Convert.ToInt32(bearing.Substring(0, 3));
                    var minutes = Convert.ToInt32(bearing.Substring(4, 2));
                    var seconds = Convert.ToInt32(bearing.Substring(6, 8));

                    return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
                case 6:

                break;

                case 3:

                break;

                default:
                    break;
            }


            return new DMS();
        }
    }
}
