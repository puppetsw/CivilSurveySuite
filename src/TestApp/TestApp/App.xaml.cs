using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GMapControl;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var vm = new AroFloProjectViewModel();
            //var view = new AroFloProjectView(vm);

            var view = new MapWindow();

            view.Show();
        }
    }
}
