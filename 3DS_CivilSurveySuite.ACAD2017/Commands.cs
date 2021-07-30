// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.ACAD2017.Commands))]
namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class Commands
    {
        [CommandMethod("3DS", "_3DSPointCreateAtAngleAndDistance", CommandFlags.Modal)]
        public void AcadPoint_Create_At_Angle_And_Distance() => PointCreateAtAngleAndDistance.RunCommand();

        [CommandMethod("3DS", "_3DSPointCreateAtOffsetTwoLines", CommandFlags.Modal)]
        public void AcadPoint_Create_At_Offset_Two_Lines() => PointCreateAtOffsetTwoLines.RunCommand();

        [CommandMethod("3DS", "_3DSPointCreateAtProduction", CommandFlags.Modal)]
        public void Point_Create_At_Production_Of_Line_And_Distance() => PointCreateOnProductionOfLine.RunCommand();
    }
}