// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for CogoPointViewer.
    /// </summary>
    public class CogoPointViewerViewModel : ViewModelBase
    {
        private readonly ICogoPointViewerService _cogoPointViewerService;
        private CivilPoint _selectedCivilPoint;
        private string _filterText;

        public ObservableCollection<CivilPoint> CogoPoints { get; }

        public CivilPoint SelectedItem
        {
            get => _selectedCivilPoint;
            set => SetProperty(ref _selectedCivilPoint, value);
        }

        public ObservableCollection<CivilPoint> SelectedItems { get; private set; }

        public ICollectionView ItemsView => CollectionViewSource.GetDefaultView(CogoPoints);

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                NotifyPropertyChanged();
                ItemsView.Refresh();
            }
        }

        public bool MultipleSelected => SelectedItems != null && SelectedItems.Count > 0;

        //TODO: Add option to export filtered list of points?
        //TODO: Add more complex filtering options for easting, northing, height?
        //TODO: Add ability to load user-defined-properties.

        public UI.RelayCommand ZoomToCommand => new UI.RelayCommand(ZoomToPoint, () => true);

        public UI.RelayCommand UpdateCommand => new UI.RelayCommand(Update, () => true);

        public UI.RelayCommand SelectCommand => new UI.RelayCommand(Select, () => true);

        public UI.RelayCommand<object> SelectionChangedCommand => new UI.RelayCommand<object>(SelectionChanged, _ => true);

        public UI.RelayCommand CopyRawDescriptionCommand => new UI.RelayCommand(CopyRawDescription, () => MultipleSelected);

        public UI.RelayCommand CopyDescriptionFormatCommand => new UI.RelayCommand(CopyDescriptionFormat, () => MultipleSelected);

        public CogoPointViewerViewModel(ICogoPointViewerService cogoPointViewerService)
        {
            _cogoPointViewerService = cogoPointViewerService;
            CogoPoints = new ObservableCollection<CivilPoint>(_cogoPointViewerService.GetPoints());

            ItemsView.Filter = o => Filter(o as CivilPoint);
        }

        private bool Filter(CivilPoint civilPoint)
        {
            return FilterText == null
                   || civilPoint.PointNumber.ToString().StartsWith(FilterText, StringComparison.CurrentCultureIgnoreCase)
                   || civilPoint.RawDescription.StartsWith(FilterText, StringComparison.CurrentCultureIgnoreCase)
                   || civilPoint.PointName.StartsWith(FilterText, StringComparison.CurrentCultureIgnoreCase)
                   || civilPoint.DescriptionFormat.StartsWith(FilterText, StringComparison.CurrentCultureIgnoreCase);
        }

        private void SelectionChanged(object items)
        {
            if (items == null)
                return;

            IList itemsList = (IList)items;

            if (itemsList.Count > 1)
            {
                var collection = itemsList.Cast<CivilPoint>();
                SelectedItems = new ObservableCollection<CivilPoint>(collection);
                NotifyPropertyChanged(nameof(MultipleSelected));
            }
        }

        private void CopyRawDescription()
        {
            if (SelectedItems == null) 
                return;

            _cogoPointViewerService.UpdateSelected(SelectedItems, nameof(CivilPoint.RawDescription), SelectedItems[0].RawDescription);
        }

        private void CopyDescriptionFormat()
        {
            if (SelectedItems == null) 
                return;

            _cogoPointViewerService.UpdateSelected(SelectedItems, nameof(CivilPoint.DescriptionFormat), SelectedItems[0].DescriptionFormat);
        }

        private void Select()
        {
            if (SelectedItem != null)
                _cogoPointViewerService.Select(SelectedItem);
        }

        private void Update()
        {
            if (SelectedItem != null)
                _cogoPointViewerService.Update(SelectedItem);
        }

        private void ZoomToPoint()
        {
            if (SelectedItem != null)
                _cogoPointViewerService.ZoomTo(SelectedItem);
        }

    }
}
