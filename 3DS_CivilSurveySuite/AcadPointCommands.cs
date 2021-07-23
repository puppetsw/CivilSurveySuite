// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Commands;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.MyCommands))]
namespace _3DS_CivilSurveySuite
{
    public partial class MyCommands
    {
        [CommandMethod("3DS", "_3DSPointCreateAtAngleAndDistance", CommandFlags.Modal)]
        public void AcadPoint_Create_At_Angle_And_Distance() => PointCreateAtAngleAndDistance.RunCommand();

        [CommandMethod("3DS", "_3DSPointCreateAtOffsetTwoLines", CommandFlags.Modal)]
        public void AcadPoint_Create_At_Offset_Two_Lines() => PointCreateAtOffsetTwoLines.RunCommand();

        [CommandMethod("3DS", "_3DSPointCreateAtProduction", CommandFlags.Modal)]
        public void Point_Create_At_Production_Of_Line_And_Distance() => PointCreateOnProductionOfLine.RunCommand();
    }
}