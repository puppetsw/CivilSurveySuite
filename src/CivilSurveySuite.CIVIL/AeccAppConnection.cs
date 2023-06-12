namespace CivilSurveySuite.CIVIL
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

        public dynamic AeccDoc => AeccApp.ActiveDocument;

        public dynamic AeccDb => AeccApp.ActiveDocument.Database;
    }
}
