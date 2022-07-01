using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.ApplicationServices.Core;

namespace _3DS_CivilSurveySuite.ACAD.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public void ShowAlert(string message)
        {
            Application.ShowAlertDialog(message);
        }
    }
}
