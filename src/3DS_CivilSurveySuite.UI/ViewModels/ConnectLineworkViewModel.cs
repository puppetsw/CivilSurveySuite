﻿// Copyright Scott Whitney. All Rights Reserved.
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
        private double _midOrdinateDistance;

        public ObservableCollection<DescriptionKey> DescriptionKeys
        {
            get => _descriptionKeys;
            set
            {
                _descriptionKeys = value;
                NotifyPropertyChanged();
            }
        }

        public double MidOrdinateDistance
        {
            get => _midOrdinateDistance;
            set => SetProperty(ref _midOrdinateDistance, value);
        }

        public DescriptionKey SelectedKey { get; set; }

        public ICommand AddRowCommand => new RelayCommand(AddRow, () => true);

        public ICommand RemoveRowCommand => new RelayCommand(RemoveRow, () => true);

        public ICommand ConnectCommand => new AsyncRelayCommand(ConnectLinework);

        public ConnectLineworkViewModel(IConnectLineworkService connectLineworkService)
        {
            MidOrdinateDistance = 0.0;
            _connectLineworkService = connectLineworkService;
            _connectLineworkService.DescriptionKeyFile = Properties.Settings.Default.DescriptionKeyFileName;
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
            _connectLineworkService.MidOrdinate = MidOrdinateDistance;
            await _connectLineworkService.ConnectCogoPoints(DescriptionKeys);
        }

        /// <summary>
        /// Get the last xml file loaded from settings
        /// </summary>
        private void LoadSettings(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                DescriptionKeys = new ObservableCollection<DescriptionKey>();
                return;
            }

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
            Properties.Settings.Default.DescriptionKeyFileName = fileName;
            Properties.Settings.Default.Save();
        }
    }
}
