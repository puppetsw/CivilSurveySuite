// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017.Services;
using _3DS_CivilSurveySuite.Model;
using SimpleInjector;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class ServiceLocator
    {
        public static Container Container { get; } = new Container();

        /// <summary>
        /// Registers the service dependencies.
        /// </summary>
        public static void Register()
        {
            ServiceLocator.Container.Register<ITraverseService, TraverseService>();
            ServiceLocator.Container.Register<IViewerService, ViewerService>(Lifestyle.Singleton);
            ServiceLocator.Container.Register<IPaletteService, PaletteService>(Lifestyle.Singleton);
            ServiceLocator.Container.Verify();
        }
    }
}