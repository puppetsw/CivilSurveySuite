// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017.Services;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using SimpleInjector;

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
            Container.Register<ITraverseService, TraverseService>();
            Container.Register<IViewerService, ViewerService>(Lifestyle.Singleton);
            Container.Register<IPaletteService, PaletteService>(Lifestyle.Singleton);
            Container.Verify();
        }

        public static ITraverseService CreateTraverseService() => Container.GetInstance<ITraverseService>();

        public static IViewerService CreateViewerService() => Container.GetInstance<IViewerService>();

        public static IPaletteService CreatePaletteService() => Container.GetInstance<IPaletteService>();
    }
}