// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.Runtime;
using _3DS_CivilSurveySuite.Commands;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.MyCommands))]
namespace _3DS_CivilSurveySuite
{
    public partial class MyCommands
    {
        [CommandMethod("3DS", "_3DSCPLabelRotate", CommandFlags.Modal)]
        public void CogoPoint_Label_Rotate() => CogoPointLabelRotate.RunCommand();

        [CommandMethod("3DS", "_3DSCPRawDescriptionToUpper", CommandFlags.Modal)]
        public void CogoPoint_RawDescription_ToUpper() => CogoPointRawDescriptionToUpperCase.RunCommand();

        [CommandMethod("3DS", "_3DSCPFullDescriptionToUpper", CommandFlags.Modal)]
        public void CogoPoint_FullDescription_ToUpper() => CogoPointFullDescriptionToUpperCase.RunCommand();

        [CommandMethod("3DS", "_3DSCPTrunkPointAtTrees", CommandFlags.Modal)]
        public void CogoPoint_CreateTrunksAtTrees() => CogoPointCreateTrunkAtTree.RunCommand();

        [CommandMethod("3DS", "_3DSCPAngleAndDistance", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Angle_And_Distance() => CogoPointCreateAtAngleAndDistance.RunCommand();

        [CommandMethod("3DS", "_3DSCPOffsetTwoLines", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Offset_Two_Lines() => CogoPointCreateAtOffsetTwoLines.RunCommand();

        [CommandMethod("3DS", "_3DSCPProduction", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Production_Of_Line_And_Distance() => CogoPointCreateOnProductionOfLine.RunCommand();



        [CommandMethod("3DSTEST")]
        public void Test()
        {
            //var a = TransientGraphics.PixelUnits(8);

            var result = AutoCADActive.Editor.GetPoint("Pick point");

            using (var graphics = new TransientGraphics())
            {
                //graphics.DrawDot(result.Value, 8);
                //graphics.DrawPointer(result.Value, 0, 8);
                //graphics.DrawPlus(result.Value, 8);
                //graphics.DrawX(result.Value, 8);
                //graphics.DrawBox(result.Value, 8, true);
                //graphics.DrawArrow(result.Value, new Angle(0), 8);
                graphics.DrawArrow(result.Value, 45, 8);
                graphics.DrawText(result.Value, new Angle(45).ToString(), 3, new Angle(45));
                var waitforKey = AutoCADActive.Editor.GetDistance("waiting...");
            }
        }
    }
}