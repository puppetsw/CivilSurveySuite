// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.ViewModels
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

        //TODO: Add option to export filtered list of points?
        //TODO: Add more complex filtering options for easting, northing, height?
        //TODO: Select all points and change description format, raw description or pointname etc.
        //TODO: Add ability to load user-defined-properties.

        public RelayCommand ZoomToCommand => new RelayCommand(ZoomToPoint, () => true);

        public RelayCommand UpdateCommand => new RelayCommand(Update, () => true);

        public RelayCommand SelectCommand => new RelayCommand(Select, () => true);

        public RelayCommand<object> SelectionChangedCommand => new RelayCommand<object>(SelectionChanged, _ => true);

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
            var itemList = (items as ObservableCollection<CivilPoint>).ToList();
            SelectedItems = new ObservableCollection<CivilPoint>(itemList);
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
