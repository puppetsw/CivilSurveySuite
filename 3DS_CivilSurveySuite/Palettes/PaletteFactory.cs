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
        static PaletteSet m_ConnectLinePalSet = null;

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
                m_TraversePalSet.AddVisual("TraverseView", view);

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

        [CommandMethod("3DSShowConnectLinePalette")]
        public void ShowConnectLinePalette()
        {
            ConnectLineworkView view = new ConnectLineworkView();
            ConnectLineworkViewModel vm = new ConnectLineworkViewModel();
            view.DataContext = vm;

            if (m_ConnectLinePalSet == null)
            {
                m_ConnectLinePalSet = new PaletteSet("3DS Connect Linework", new Guid("6F0020E1-19CB-42AD-AB4F-D81D0C4731AF"));
                m_ConnectLinePalSet.Style = PaletteSetStyles.ShowCloseButton;
                m_ConnectLinePalSet.AddVisual("ConnectLineView", view);
                m_ConnectLinePalSet.EnableTransparency(true);
                m_ConnectLinePalSet.KeepFocus = true;
            }

            m_ConnectLinePalSet.Visible = true;
        }
    }
}
