// 3DS_CivilSurveySuite References
using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Helpers.Wpf;
using _3DS_CivilSurveySuite.Models;
// AutoCAD References

// System References
using System.Collections.ObjectModel;
using System.IO;

namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// ViewModel for ConnectLineworkView.xaml
    /// </summary>
    public class ConnectLineworkViewModel : ViewModelBase
    {
        #region Private Members
        private ObservableCollection<DescriptionKey> descriptionKeys;


        #endregion

        #region Properties

        public ObservableCollection<DescriptionKey> DescriptionKeys { get => descriptionKeys; set { descriptionKeys = value; NotifyPropertyChanged(); } }

        public DescriptionKey SelectedKey { get; set; }

        #endregion

        #region Constructor

        public ConnectLineworkViewModel()
        {
            LoadSettings();
        }

        #endregion

        #region Commands

        public RelayCommand AddRowCommand => new RelayCommand((_) => AddRow(), (_) => true);
        public RelayCommand RemoveRowCommand => new RelayCommand((_) => RemoveRow(), (_) => true);
        public RelayCommand ConnectCommand => new RelayCommand((_) => ConnectLinework(), (_) => true);

        #endregion

        #region Command Methods

        private void AddRow()
        {
            DescriptionKeys.Add(new DescriptionKey());
        }

        private void RemoveRow()
        {
            if (SelectedKey != null)
                DescriptionKeys.Remove(SelectedKey);
        }

        private void ConnectLinework()
        {

        }

        #endregion

        #region Private Methods



        /// <summary>
        /// Get the last xml file loaded from settings
        /// </summary>
        private void LoadSettings()
        {
            string fileName = Properties.Settings.Default.ConnectLineworkFileName;

            if (File.Exists(fileName))
            {
                Load(fileName);
            }
            else
            {
                DescriptionKeys = new ObservableCollection<DescriptionKey>();
            }
        }

        public void Load(string fileName)
        {
            DescriptionKeys = XmlHelper.ReadFromXmlFile<ObservableCollection<DescriptionKey>>(fileName);
        }

        public void Save(string fileName)
        {
            XmlHelper.WriteToXmlFile(fileName, DescriptionKeys);
            Properties.Settings.Default.ConnectLineworkFileName = fileName;
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}
