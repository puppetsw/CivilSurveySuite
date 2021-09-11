using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.C3D2017.C3DPlugin))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    public class C3DPlugin : IExtensionApplication
    {
        public void Initialize()
        {
            C3DService.Register();
        }

        public void Terminate()
        {
        }
    }
}
