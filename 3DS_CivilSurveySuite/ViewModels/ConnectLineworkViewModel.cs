using _3DS_CivilSurveySuite.Models;
using System.Collections.ObjectModel;

namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// ViewModel for ConnectLineView.xaml
    /// </summary>
    public class ConnectLineworkViewModel : ViewModelBase
    {
        public ObservableCollection<DescriptionKey> DescriptionKeys { get; set; }
        public ObservableCollection<string> Layers { get; set; }

        public ConnectLineworkViewModel()
        {
            DescriptionKeys = new ObservableCollection<DescriptionKey>
            {
                new DescriptionKey() { Key = "BB#*", Description = "Bottom of bank", Layer = "SVY-BOB", Draw2D = false, Draw3D = true },
                new DescriptionKey() { Key = "BD#*", Description = "Building", Layer = "SVY-BLD", Draw2D = true, Draw3D = false }
            };

            Layers = new ObservableCollection<string>
            {
                "SVY-BOB",
                "SVY-BLD",
                "SVY-NATURAL"
            };
        }
    }
}
