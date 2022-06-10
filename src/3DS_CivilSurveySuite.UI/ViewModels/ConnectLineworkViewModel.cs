// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for ConnectLineworkView.xaml
    /// </summary>
    public class ConnectLineworkViewModel : ObservableObject
    {
        private readonly IConnectLineworkService _connectLineworkService;
        private ObservableCollection<DescriptionKey> _descriptionKeys;

        public ObservableCollection<DescriptionKey> DescriptionKeys
        {
            get => _descriptionKeys;
            set
            {
                _descriptionKeys = value;
                NotifyPropertyChanged();
            }
        }

        public DescriptionKey SelectedKey { get; set; }

        public ICommand AddRowCommand => new RelayCommand(AddRow, () => true);

        public ICommand RemoveRowCommand => new RelayCommand(RemoveRow, () => true);

        public ICommand ConnectCommand => new AsyncRelayCommand(ConnectLinework);

        public ConnectLineworkViewModel(IConnectLineworkService connectLineworkService)
        {
            _connectLineworkService = connectLineworkService;
            LoadSettings(_connectLineworkService.DescriptionKeyFile);
        }

        private void AddRow()
        {
            DescriptionKeys.Add(new DescriptionKey());
        }

        private void RemoveRow()
        {
            if (SelectedKey != null)
            {
                DescriptionKeys.Remove(SelectedKey);
            }
        }

        private async Task ConnectLinework()
        {
            await _connectLineworkService.ConnectCogoPoints(DescriptionKeys);
        }

        /// <summary>
        /// Get the last xml file loaded from settings
        /// </summary>
        private void LoadSettings(string fileName)
        {
            if (File.Exists(fileName))
            {
                Load(fileName);
            }
            else
            {
                DescriptionKeys = new ObservableCollection<DescriptionKey>();
            }
        }

        /// <summary>
        /// Load XML file
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            DescriptionKeys = XmlHelper.ReadFromXmlFile<ObservableCollection<DescriptionKey>>(fileName);
        }

        /// <summary>
        /// Save XML file
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            XmlHelper.WriteToXmlFile(fileName, DescriptionKeys);
            //Properties.Settings.Default.ConnectLineworkFileName = fileName;
            //Properties.Settings.Default.Save();
        }
    }
}