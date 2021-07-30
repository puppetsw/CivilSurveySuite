// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.C3D2017.Commands))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    public class Commands
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
    }
}