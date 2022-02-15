using System;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteUITests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void AcadLaunchTest()
        {
            using (var app = Application.Launch(@"C:\Program Files\Autodesk\AutoCAD 2017\acad.exe"))
            {
                using (var automation = new UIA3Automation())
                {
                    var window = app.GetMainWindow(automation);

                    Thread.Sleep(10000);


                    window.FindFirstChild(cf => cf.)

                    var mainWindow = FindElement(window, "Autodesk AutoCAD Civil 3D 2017 - [Drawing1.dwg]");

                    Assert.That(window, Is.Not.Null);
                    Assert.That(window.Title, Is.Not.Null);
                }
            }
        }

        private AutomationElement FindElement(AutomationElement mainElement, string text)
        {
            var element = mainElement.FindFirstDescendant(cf => cf.ByText(text));
            return element;
        }
    }
}