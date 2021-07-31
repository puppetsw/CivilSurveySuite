using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public class C3DPlugin : IExtensionApplication
    {
        private C3DPalettes _palettes;

        public void Initialize()
        {
            _palettes = new C3DPalettes();
            _palettes.HookupEvents();
        }

        public void Terminate()
        {
            _palettes.UnhookEvents();
        }
    }
}
