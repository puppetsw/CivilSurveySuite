// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Model;
using SimpleInjector;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class C3DServiceFactory
    {
        private static Container Container { get; } = new Container();

        /// <summary>
        /// Registers the service dependencies.
        /// </summary>
        public static void Register()
        {
            Container.Register<ISurfaceSelectService, SurfaceSelectService>();
            Container.Register<IConnectLineworkService, ConnectLineworkService>();
            Container.Register<ICogoPointViewerService, CogoPointViewerService>();
            Container.Verify();
        }

        public static ISurfaceSelectService GetSurfaceSelectService() => Container.GetInstance<ISurfaceSelectService>();

        public static IConnectLineworkService GetConnectLineworkService() => Container.GetInstance<IConnectLineworkService>();

        public static ICogoPointViewerService GetCogoPointViewerService() => Container.GetInstance<ICogoPointViewerService>();
    }
}