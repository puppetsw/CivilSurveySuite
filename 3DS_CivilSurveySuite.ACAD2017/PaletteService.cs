// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Windows;
using _3DS_CivilSurveySuite.ViewModels;
using Autodesk.AutoCAD.Windows;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class PaletteService : IPaletteService, IDisposable
    {
        private readonly List<Type> _palettes = new List<Type>();

        public PaletteSet PaletteSet { get; private set; }

        ~PaletteService()
        {
            ReleaseUnmanagedResources();
        }

        /// <summary>
        /// Generates the palette.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The ViewModel.</param>
        /// <param name="viewName">The View associated with the ViewModel.</param>
        /// <param name="hideMethod">The action to run when the palette is hidden/closed.</param>
        public void GeneratePalette(FrameworkElement view, ViewModelBase viewModel, string viewName, Action hideMethod = null)
        {
            view.DataContext = viewModel;

            if (PaletteSet == null)
            {
                CreatePaletteSet();
            }

            // ReSharper disable PossibleNullReferenceException
            if (!_palettes.Contains(view.GetType()))
            {
                PaletteSet.AddVisual(viewName, view);
                _palettes.Add(view.GetType());
                PaletteSet.Activate(_palettes.IndexOf(view.GetType()));

                if (hideMethod != null)
                {
                    PaletteSet.StateChanged += (s, e) =>
                    {
                        if (e.NewState == StateEventIndex.Hide)
                        {
                            hideMethod.Invoke();
                        }
                    };

                    //BUG: When closing AutoCAD this event tries to run. But Editor is null.
                    //view.IsVisibleChanged += (s, e) => AutoCADApplicationManager.Editor.WriteMessage("IsVisibleChanged");
                }
            }

            if (!PaletteSet.Visible)
            {
                PaletteSet.Visible = true;
            }
            // ReSharper restore PossibleNullReferenceException
        }

        private void CreatePaletteSet()
        {
            PaletteSet = new PaletteSet("3DS Civil Survey Suite", new Guid("C55243DF-EEBB-4FA6-8651-645E018F86DE"));
            PaletteSet.Style = PaletteSetStyles.ShowCloseButton |
                                             PaletteSetStyles.ShowPropertiesMenu |
                                             PaletteSetStyles.ShowAutoHideButton;
            PaletteSet.EnableTransparency(true);
            PaletteSet.KeepFocus = false;
        }

        
        private void ReleaseUnmanagedResources()
        {
            if (PaletteSet != null)
                PaletteSet.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}