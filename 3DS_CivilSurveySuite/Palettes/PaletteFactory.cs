using _3DS_CivilSurveySuite.Helpers.AutoCAD;
using _3DS_CivilSurveySuite.ViewModels;
using _3DS_CivilSurveySuite.Views;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Palettes.PaletteFactory))]
namespace _3DS_CivilSurveySuite.Palettes
{
    /// <summary>
    /// PaletteFactory class for hooking up Views and ViewModels to be
    /// displayed as Palettes in AutoCAD Civil3D.
    /// </summary>
    public class PaletteFactory : CivilBase
    {
        static PaletteSet m_TraversePalSet = null;

        [CommandMethod("3DSShowTraversePalette")]
        public void ShowTraversePalette()
        {
            TraverseView view = new TraverseView();
            TraverseViewModel vm = new TraverseViewModel();
            view.DataContext = vm;

            if (m_TraversePalSet == null)
            {
                m_TraversePalSet = new PaletteSet("3DS Traverse", new Guid("39663E77-EAC7-409A-87E4-4E6E15A5D05A"));
                m_TraversePalSet.Style = PaletteSetStyles.ShowCloseButton;
                m_TraversePalSet.AddVisual("TraverseWindow", view);

                m_TraversePalSet.StateChanged += (s, e) =>
                {
                    if (e.NewState == StateEventIndex.Hide)
                    {
                        vm.ClearTransientGraphics();
                    }
                };

                m_TraversePalSet.EnableTransparency(true);
                m_TraversePalSet.KeepFocus = true;
            }

            m_TraversePalSet.Visible = true;
        }
    }
}
