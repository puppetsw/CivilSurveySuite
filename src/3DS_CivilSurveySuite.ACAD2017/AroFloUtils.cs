using System.Collections.Generic;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using AroFloApi;
using Autodesk.AutoCAD.DatabaseServices;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class AroFloUtils
    {
        private static void ConfigureAroFlo()
        {
            AroFloConfiguration.SECRET_KEY = "RHIzTUFiUlJhSUpPenNQaFA2WHBzcGMzYXJlM1RxMCtDVW5uNkRKdnhITzI1S0krNW4vM0NZdk45SnlnNFFTaG1wcnB0WXBlRGMzNlFYeDEwVE9Wbmc9PQ==";
            AroFloConfiguration.U_ENCODE = "PjZPQjtBSEM7RihdOjI6JDJMKlwgJiohQ0AxTVw4Klg9Jzk6NDUpWiwK";
            AroFloConfiguration.P_ENCODE = "cTdod3FkODFlNnI0TGVk";
            AroFloConfiguration.ORG_ENCODE = "JSc6TyBQLFAgCg==";
        }

        private static ILogger Logger { get; } = Ioc.Default.GetInstance<ILogger>();

        public static async void ProjectDetailsTo3DSTitleBlock()
        {
            // valid title blocks
            var validTitleBlocks = new List<string>
            {
                "3DS_TitleBlock_A1",
                "3DS_TitleBlock_A2",
                "3DS_TitleBlock_A2_Portrait",
                "3DS_TitleBlock_A3",
                "3DS_TitleBlock_A3_Portrait"
            };

            using (var tr = AcadApp.StartLockedTransaction())
            {
                if (!EditorUtils.TryGetEntityOfType<BlockReference>("\n3DS> Select a title block.",
                        "\n3DS> Select a block reference.", out var blockId, true))
                {
                    return;
                }

                var blockName = BlockUtils.GetBlockName(tr, blockId);

                if (!validTitleBlocks.Contains(blockName))
                {
                    AcadApp.Editor.WriteMessage("\n3DS> Please select a valid 3D Surveys title block.");
                    Logger.Warn("User selected an invalid title block.");
                    tr.Commit();
                    return;
                }

                if (!EditorUtils.TryGetInt("\n3DS> Please enter the project number PN: ", out int projectNumber))
                {
                    return;
                }

                ConfigureAroFlo();
                var project = await ProjectController.GetProjectAsync(projectNumber);

                if (project == null)
                {
                    AcadApp.Editor.WriteMessage("\n3DS> Unable to find project number.");
                    Logger.Warn("User entered an invalid project number.");
                    tr.Commit();
                    return;
                }

                project.Location = await LocationController.GetLocationAsync(project.Location.LocationId);

                /*
                 * ADDRESS#1
                 * ADDRESS#2
                 * ADDRESS#3
                 * DRAWING_TITLE
                 * PROJECT_NO
                 * CLIENT_NAME
                 */

                BlockUtils.TryUpdateBlockAttribute(tr, blockId, "ADDRESS#2", project.Location.Address.ToUpper());
                BlockUtils.TryUpdateBlockAttribute(tr, blockId, "ADDRESS#3", $"{project.Location.Suburb.ToUpper()}, " +
                                                                             $"{project.Location.State.ToUpper()}, " +
                                                                             $"{project.Location.PostCode}");
                BlockUtils.TryUpdateBlockAttribute(tr, blockId, "PROJECT_NO", $"PN{project.ProjectNumber}");
                BlockUtils.TryUpdateBlockAttribute(tr, blockId, "CLIENT_NAME", project.Client.Name.ToUpper());

                tr.Commit();
            }
        }
    }
}
