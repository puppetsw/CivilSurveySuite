// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Windows;
using _3DS_CivilSurveySuite.ACAD2017.Services;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using _3DS_CivilSurveySuite.UI.Views;
using SimpleInjector;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class AcadService
    {
        private static Container Container { get; } = new Container();

        /// <summary>
        /// Registers the service dependencies.
        /// </summary>
        public static void Register()
        {
            Container.Register<IProcessService, ProcessService>();
            Container.Register<ITraverseService, TraverseService>(Lifestyle.Singleton);
            Container.Register<IMessageBoxService, MessageBoxService>();

            Container.Register<IOpenFileDialogService, OpenFileDialogService>();
            Container.Register<ISaveFileDialogService, SaveFileDialogService>();

            Container.Register<AngleCalculatorView>();
            Container.Register<AngleCalculatorViewModel>();

            Container.Register<TraverseAngleView>();
            Container.Register<TraverseAngleViewModel>();

            Container.Register<TraverseView>();
            Container.Register<TraverseViewModel>();

            Container.Verify();
        }

        [Obsolete("This method is obsolete. Use ShowDialog<TView>() instead.", false)]
        public static Window ShowWindow<TView>() where TView : Window
        {
            var view = CreateWindow<TView>();
            Application.ShowModelessWindow(view);
            return view;
        }

        public static bool? ShowDialog<TView>() where TView : Window
        {
            var view = CreateWindow<TView>();
            return Application.ShowModalWindow(view);
        }

        private static TView CreateWindow<TView>() where TView : Window
        {
            return Container.GetInstance<TView>();
        }

    }
}