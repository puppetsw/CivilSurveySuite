using Autodesk.AutoCAD.ApplicationServices.Core;
using CivilSurveySuite.Shared.Services.Interfaces;

namespace CivilSurveySuite.ACAD.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public void ShowAlert(string message)
        {
            Application.ShowAlertDialog(message);
        }
    }
}
