using _3DS_CivilSurveySuite.ACAD2017;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.C3D2017.C3DPlugin))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    public class C3DPlugin : IExtensionApplication
    {
        private const string _3DS_CUI_FILE = "3DS_CSS_ACAD.cuix";

        public void Initialize()
        {
            C3DService.Register();
            AcadApp.LoadCuiFile(_3DS_CUI_FILE);
        }

        public void Terminate()
        {
            AcadApp.UnloadCuiFile(_3DS_CUI_FILE);
        }
    }
}