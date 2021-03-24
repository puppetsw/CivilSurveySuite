// 3DS_CivilSurveySuite References
using _3DS_CivilSurveySuite.Helpers.AutoCAD;
using _3DS_CivilSurveySuite.ViewModels;
using _3DS_CivilSurveySuite.Views;
// AutoCAD References
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
// System References
using System;
using System.Collections.Generic;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Palettes.PaletteFactory))]
namespace _3DS_CivilSurveySuite.Palettes
{
    /// <summary>
    /// PaletteFactory class for hooking up Views and ViewModels to be
    /// displayed as Palettes in AutoCAD Civil3D.
    /// </summary>
    public class PaletteFactory : CivilBase, IExtensionApplication
    {
        #region Private Members
        private bool m_PaletteVisible;
        private static PaletteSet m_CivilSurveySuitePalSet = null;
        private static readonly List<Type> m_Palettes = new List<Type>();
        #endregion

        #region IExtensionApplication
        public void Initialize()
        {
            //hookup events
            AcaddocManager.DocumentActivated += AcaddocManager_DocumentActivated;
            AcaddocManager.DocumentCreated += AcaddocManager_DocumentCreated;
            AcaddocManager.DocumentToBeDeactivated += AcaddocManager_DocumentToBeDeactivated;
            AcaddocManager.DocumentToBeDestroyed += AcaddocManager_DocumentToBeDestroyed;
        }

        public void Terminate()
        {
            //unhook events
            AcaddocManager.DocumentActivated -= AcaddocManager_DocumentActivated;
            AcaddocManager.DocumentCreated -= AcaddocManager_DocumentCreated;
            AcaddocManager.DocumentToBeDeactivated -= AcaddocManager_DocumentToBeDeactivated;
            AcaddocManager.DocumentToBeDestroyed -= AcaddocManager_DocumentToBeDestroyed;
        }
        #endregion

        #region Document Events

        private void AcaddocManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            if (m_CivilSurveySuitePalSet == null) return;
            m_PaletteVisible = m_CivilSurveySuitePalSet.Visible;
            if (AcaddocManager.Count == 1)
                m_CivilSurveySuitePalSet.Visible = false;
        }

        private void AcaddocManager_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e)
        {
            if (m_CivilSurveySuitePalSet == null) return;
            m_PaletteVisible = m_CivilSurveySuitePalSet.Visible;
        }

        private void AcaddocManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            if (m_CivilSurveySuitePalSet == null) return;
            m_CivilSurveySuitePalSet.Visible = m_PaletteVisible;
        }

        private void AcaddocManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (m_CivilSurveySuitePalSet == null) return;
            m_CivilSurveySuitePalSet.Visible = e.Document != null && m_PaletteVisible;
        }

        #endregion

        #region Show Palettes

        [CommandMethod("3DSShowTraversePalette")]
        public void ShowTraversePalette()
        {
            TraverseView view = new TraverseView();
            TraverseViewModel vm = new TraverseViewModel();
            view.DataContext = vm;

            if (m_CivilSurveySuitePalSet == null)
                CreatePaletteSet();

            if (!m_Palettes.Contains(view.GetType()))
            {
                m_CivilSurveySuitePalSet.AddVisual("Traverse", view);
                m_Palettes.Add(view.GetType());
                m_CivilSurveySuitePalSet.Activate(m_Palettes.IndexOf(view.GetType()));
                m_CivilSurveySuitePalSet.StateChanged += (s, e) =>
                {
                    if (e.NewState == StateEventIndex.Hide)
                        vm.ClearTransientGraphics();
                };
            }

            if (!m_CivilSurveySuitePalSet.Visible)
                m_CivilSurveySuitePalSet.Visible = true;
        }

        [CommandMethod("3DSShowConnectLinePalette")]
        public void ShowConnectLinePalette()
        {
            ConnectLineworkView view = new ConnectLineworkView();
            ConnectLineworkViewModel vm = new ConnectLineworkViewModel();
            view.DataContext = vm;

            if (m_CivilSurveySuitePalSet == null)
                CreatePaletteSet();

            if (!m_Palettes.Contains(view.GetType()))
            {
                m_CivilSurveySuitePalSet.AddVisual("Linework", view);
                m_Palettes.Add(view.GetType());
                m_CivilSurveySuitePalSet.Activate(m_Palettes.IndexOf(view.GetType()));
            }

            if (!m_CivilSurveySuitePalSet.Visible)
                m_CivilSurveySuitePalSet.Visible = true;
        }

        #endregion

        #region Private Methods

        private void CreatePaletteSet()
        {
            m_CivilSurveySuitePalSet = new PaletteSet("3DS Civil Survey Suite", new Guid("C55243DF-EEBB-4FA6-8651-645E018F86DE"));
            m_CivilSurveySuitePalSet.Style = PaletteSetStyles.ShowCloseButton | 
                                             PaletteSetStyles.ShowPropertiesMenu | 
                                             PaletteSetStyles.ShowAutoHideButton; 
            m_CivilSurveySuitePalSet.EnableTransparency(true);
            m_CivilSurveySuitePalSet.KeepFocus = false;
        }

        #endregion

    }
}