using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.Civil.DatabaseServices;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;
using CivilSurveySuite.UI.Views;

namespace CivilSurveySuite.CIVIL
{
    public static class SelectionUtils
    {
        /// <summary>
        /// Selects the surface.
        /// </summary>
        /// <returns>TinSurface.</returns>
        public static TinSurface SelectSurface()
        {
            var window = Ioc.GetRequiredView<SelectSurfaceView>();
            var dialog = window as IDialogService<CivilSurface>;
            var showDialog = Application.ShowModalWindow(window);

            if (showDialog != true)
                return null;

            if (dialog == null)
                return null;

            var civilSurface = dialog.ResultObject;
            TinSurface surface;

            using (var tr = AcadApp.StartTransaction())
            {
                surface = SurfaceUtils.GetSurfaceByName(tr, civilSurface.Name);
                tr.Commit();
            }

            return surface;
        }

        /// <summary>
        /// Selects the point group.
        /// </summary>
        /// <returns>PointGroup.</returns>
        public static PointGroup SelectPointGroup()
        {
            var window = Ioc.GetRequiredView<SelectPointGroupView>();
            var dialog = window as IDialogService<CivilPointGroup>;
            var showDialog = Application.ShowModalWindow(window);

            if (showDialog != true)
                return null;

            if (dialog == null)
                return null;

            var civilPointGroup = dialog.ResultObject;
            PointGroup pointGroup;

            using (var tr = AcadApp.StartTransaction())
            {
                pointGroup = PointGroupUtils.GetPointGroupByName(tr, civilPointGroup.Name);
                tr.Commit();
            }

            return pointGroup;
        }


        /// <summary>
        /// Selects the alignment.
        /// </summary>
        /// <returns>Alignment.</returns>
        public static Alignment SelectAlignment()
        {
            var window = Ioc.GetRequiredView<SelectAlignmentView>();
            var dialog = window as IDialogService<CivilAlignment>;
            var showDialog = Application.ShowModalWindow(window);

            if (showDialog != true)
                return null;

            if (dialog == null)
                return null;

            var civilAlignment = dialog.ResultObject;
            Alignment alignment;

            using (var tr = AcadApp.StartTransaction())
            {
                alignment = AlignmentUtils.GetAlignmentByName(tr, civilAlignment.Name);
                tr.Commit();
            }

            return alignment;
        }
    }
}
