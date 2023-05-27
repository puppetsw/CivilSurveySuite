using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;

namespace CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for CogoPointViewer.
    /// </summary>
    public class CogoPointEditorViewModel : ObservableObject
    {
        private readonly ICogoPointService _cogoPointService;
        private CivilPoint _selectedCivilPoint;
        private string _filterText;

        public ObservableCollection<CivilPoint> CogoPoints
        {
            [DebuggerStepThrough]
            get;
        }

        public CivilPoint SelectedItem
        {
            [DebuggerStepThrough]
            get => _selectedCivilPoint;
            [DebuggerStepThrough]
            set => SetProperty(ref _selectedCivilPoint, value);
        }

        public ObservableCollection<CivilPoint> SelectedItems
        {
            [DebuggerStepThrough]
            get; private set;
        }

        public ICollectionView ItemsView
        {
            [DebuggerStepThrough]
            get => CollectionViewSource.GetDefaultView(CogoPoints);
        }

        public string FilterText
        {
            [DebuggerStepThrough]
            get => _filterText;
            [DebuggerStepThrough]
            set
            {
                _filterText = value;
                NotifyPropertyChanged();
                ItemsView.Refresh();
            }
        }

        public bool MultipleSelected
        {
            [DebuggerStepThrough]
            get => SelectedItems != null && SelectedItems.Count > 0;
        }

        //TODO: Add option to export filtered list of points?
        //TODO: Add more complex filtering options for easting, northing, height?
        //TODO: Add ability to load user-defined-properties.

        public RelayCommand ZoomToCommand => new RelayCommand(ZoomToPoint, () => true);

        public RelayCommand UpdateCommand => new RelayCommand(Update, () => true);

        public RelayCommand SelectCommand => new RelayCommand(Select, () => true);

        public RelayCommand<object> SelectionChangedCommand => new RelayCommand<object>(SelectionChanged, _ => true);

        public RelayCommand CopyRawDescriptionCommand => new RelayCommand(CopyRawDescription, () => MultipleSelected);

        public RelayCommand CopyDescriptionFormatCommand => new RelayCommand(CopyDescriptionFormat, () => MultipleSelected);

        public CogoPointEditorViewModel(ICogoPointService cogoPointService)
        {
            _cogoPointService = cogoPointService;
            CogoPoints = new ObservableCollection<CivilPoint>(_cogoPointService.GetPoints());

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

            _cogoPointService.UpdateSelected(SelectedItems, nameof(CivilPoint.RawDescription), SelectedItems[0].RawDescription);
        }

        private void CopyDescriptionFormat()
        {
            if (SelectedItems == null)
                return;

            _cogoPointService.UpdateSelected(SelectedItems, nameof(CivilPoint.DescriptionFormat), SelectedItems[0].DescriptionFormat);
        }

        private void Select()
        {
            if (SelectedItem != null)
                _cogoPointService.Select(SelectedItem);
        }

        private void Update()
        {
            if (SelectedItem != null)
                _cogoPointService.Update(SelectedItem);
        }

        private void ZoomToPoint()
        {
            if (SelectedItem != null)
                _cogoPointService.ZoomTo(SelectedItem);
        }

    }
}