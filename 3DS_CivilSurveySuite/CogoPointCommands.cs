// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.Runtime;
using _3DS_CivilSurveySuite.Commands;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.MyCommands))]
namespace _3DS_CivilSurveySuite
{
    public partial class MyCommands
    {
        [CommandMethod("3DS", "_3DSCogoPointLabelRotate", CommandFlags.Modal)]
        public void CogoPoint_Label_Rotate() => CogoPointLabelRotate.RunCommand();

        [CommandMethod("3DS", "_3DSCogoPointRawDescriptionToUpperCase", CommandFlags.Modal)]
        public void CogoPoint_RawDescription_ToUpper() => CogoPointRawDescriptionToUpperCase.RunCommand();

        [CommandMethod("3DS", "_3DSCogoPointFullDescriptionToUpperCase", CommandFlags.Modal)]
        public void CogoPoint_FullDescription_ToUpper() => CogoPointFullDescriptionToUpperCase.RunCommand();

        [CommandMethod("3DS", "_3DSCogoPointCreateTrunkPointAtTrees", CommandFlags.Modal)]
        public void CogoPoint_CreateTrunksAtTrees() => CogoPointCreateTrunkAtTree.RunCommand();

        [CommandMethod("3DS", "_3DSCogoPointCreateAtAngleAndDistance", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Angle_And_Distance() => CogoPointCreateAtAngleAndDistance.RunCommand();

        [CommandMethod("3DS", "_3DSCogoPointCreateAtOffsetTwoLines", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Offset_Two_Lines() => CogoPointCreateAtOffsetTwoLines.RunCommand();

        [CommandMethod("3DS", "_3DSCogoPointCreateAtProduction", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Production_Of_Line_And_Distance() => CogoPointCreateOnProductionOfLine.RunCommand();
    }
}