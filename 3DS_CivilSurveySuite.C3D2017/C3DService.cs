﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Windows;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.C3D2017.Services;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using _3DS_CivilSurveySuite.UI.Views;
using Autodesk.Civil.DatabaseServices;
using SimpleInjector;
//using SimpleInjector.Diagnostics;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class C3DService
    {
        private static Container Container { get; } = new Container();

        /// <summary>
        /// Registers the service dependencies.
        /// </summary>
        public static void Register()
        {
            Container.Register<ISelectSurfaceService, SelectSurfaceService>();
            Container.Register<ISelectPointGroupService, SelectPointGroupService>();
            Container.Register<ISelectAlignmentService, SelectAlignmentService>();

            Container.Register<ICogoPointMoveLabelService, CogoPointMoveLabelService>();
            Container.Register<IConnectLineworkService, ConnectLineworkService>();
            Container.Register<ICogoPointEditorService, CogoPointEditorService>();
            Container.Register<ICogoPointSurfaceReportService, CogoPointSurfaceReportService>();
            Container.Register<ICogoPointReplaceDuplicateService, CogoPointReplaceDuplicateService>();

            Container.Register<SelectSurfaceView>();
            Container.Register<SelectSurfaceViewModel>();

            Container.Register<SelectPointGroupView>();
            Container.Register<SelectPointGroupViewModel>();

            Container.Register<SelectAlignmentView>();
            Container.Register<SelectAlignmentViewModel>();

            Container.Register<CogoPointMoveLabelView>();
            Container.Register<CogoPointMoveLabelViewModel>();

            Container.Register<ConnectLineworkView>();
            Container.Register<ConnectLineworkViewModel>();

            Container.Register<CogoPointEditorView>();
            Container.Register<CogoPointEditorViewModel>();

            Container.Register<CogoPointSurfaceReportView>();
            Container.Register<CogoPointSurfaceReportViewModel>();

            Container.Register<CogoPointReplaceDuplicateView>();
            Container.Register<CogoPointReplaceDuplicateViewModel>();

            Container.Verify(VerificationOption.VerifyAndDiagnose);
            //var debug = Analyzer.Analyze(Container);
        }

        [Obsolete("This method is obsolete. Use ShowDialog<TView>() instead.", false)]
        public static void ShowWindow<TView>() where TView : Window
        {
            var view = CreateWindow<TView>();
            Application.ShowModelessWindow(view);
        }

        public static bool? ShowDialog<TView>() where TView : Window
        {
            var view = CreateWindow<TView>();
            return Application.ShowModalWindow(view);
        }

        private static Window CreateWindow<TView>() where TView : Window
        {
            return Container.GetInstance<TView>();
        }

        /// <summary>
        /// Selects the surface.
        /// </summary>
        /// <returns>TinSurface.</returns>
        public static TinSurface SelectSurface()
        {
            var window = CreateWindow<SelectSurfaceView>();
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
            var window = CreateWindow<SelectPointGroupView>();
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
            var window = CreateWindow<SelectAlignmentView>();
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