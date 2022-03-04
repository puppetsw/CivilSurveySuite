// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using System.IO;
using _3DS_CivilSurveySuite.UI.Helpers;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;

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

        public RelayCommand AddRowCommand => new RelayCommand(AddRow, () => true);

        public RelayCommand RemoveRowCommand => new RelayCommand(RemoveRow, () => true);

        public RelayCommand ConnectCommand => new RelayCommand(ConnectLinework, () => true);

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

        private void ConnectLinework()
        {
            _connectLineworkService.ConnectCogoPoints(DescriptionKeys);
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