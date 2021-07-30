using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.Runtime;
using SimpleInjector;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.ACAD2017.AcadPlugin))]
namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class AcadPlugin : IExtensionApplication
    {
        private AcadPalettes _palettes;

        public void Initialize()
        {
            // 2. Configure the container (register)
            ServiceLoader.Container.Register<IViewerService, ViewerService>(Lifestyle.Singleton);
            ServiceLoader.Container.Register<IPaletteService, PaletteService>(Lifestyle.Singleton);
            ServiceLoader.Container.Register<ITraverseService, TraverseService>();
            
            // 3. Verify your configuration
            ServiceLoader.Container.Verify();

            _palettes = new AcadPalettes();
            _palettes.HookupEvents();
        }

        public void Terminate()
        {
            _palettes.UnhookEvents();
        }
    }
}
