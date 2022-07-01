namespace _3DS_CivilSurveySuite.C3D2017
{
    public sealed class AeccAppConnection
    {
        private dynamic _aeccApp;

        public dynamic AeccApp
        {
            get
            {
                if (_aeccApp == null)
                {
                    _aeccApp = AeccAppTools.GetAeccApp("Land");
                }
                return _aeccApp;
            }
        }

        public dynamic AeccDoc
        {
            get
            {
                return AeccApp.ActiveDocument;
            }
        }

        public dynamic AeccDb
        {
            get
            {
                return AeccApp.ActiveDocument.Database;
            }
        }
    }
}
