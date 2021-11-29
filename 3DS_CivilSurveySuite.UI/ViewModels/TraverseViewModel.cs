// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for TraverseView
    /// </summary>
    public class TraverseViewModel : ViewModelBase
    {
        private string _closeBearing;
        private string _closeDistance;
        private readonly ITraverseService _traverseService;
        private readonly IProcessService _processService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IOpenFileDialogService _openFileDialogService;
        private readonly ISaveFileDialogService _saveFileDialogService;

        public ObservableCollection<TraverseObject> TraverseItems
        {
            [DebuggerStepThrough]
            get;
        } = new ObservableCollection<TraverseObject>();

        public TraverseObject SelectedTraverseItem
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            set;
        }

        public string CloseDistance
        {
            [DebuggerStepThrough]
            get => _closeDistance;
            [DebuggerStepThrough]
            set
            {
                _closeDistance = value;
                NotifyPropertyChanged();
            }
        }

        public string CloseBearing
        {
            [DebuggerStepThrough]
            get => _closeBearing;
            [DebuggerStepThrough]
            set
            {

                _closeBearing = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand AddRowCommand => new RelayCommand(AddRow, () => true);

        public RelayCommand RemoveRowCommand => new RelayCommand(RemoveRow, () => true);

        public RelayCommand ClearCommand => new RelayCommand(ClearTraverse, () => true);

        public RelayCommand DrawCommand => new RelayCommand(DrawTraverse, () => true);

        public RelayCommand FeetToMetersCommand => new RelayCommand(FeetToMeters, () => true);

        public RelayCommand LinksToMetersCommand => new RelayCommand(LinksToMeters, () => true);

        public RelayCommand FlipBearingCommand => new RelayCommand(FlipBearing, () => true);

        public RelayCommand ShowHelpCommand => new RelayCommand(ShowHelp, () => true);

        public RelayCommand SetBasePointCommand => new RelayCommand(SetBasePoint, () => true);

        public RelayCommand GridUpdatedCommand => new RelayCommand(GridUpdated, () => true);

        public RelayCommand CloseWindowCommand => new RelayCommand(CloseWindow, () => true);

        public RelayCommand SelectLineCommand => new RelayCommand(SelectLine, () => true);

        public RelayCommand ZoomExtentsCommand => new RelayCommand(Zoom, () => true);

        public RelayCommand OpenFileCommand => new RelayCommand(OpenFile, () => true);

        public RelayCommand SaveFileCommand => new RelayCommand(SaveFile, () => true);

        public TraverseViewModel(ITraverseService traverseService, IProcessService processService,
            IMessageBoxService messageBoxService, IOpenFileDialogService openFileDialogService,
            ISaveFileDialogService saveFileDialogService)
        {
            _traverseService = traverseService;
            _processService = processService;
            _messageBoxService = messageBoxService;
            _openFileDialogService = openFileDialogService;
            _saveFileDialogService = saveFileDialogService;
        }

        private void OpenFile()
        {
            _openFileDialogService.DefaultExt = ".csv";
            _openFileDialogService.Filter = "CSV Files (*.csv)|*.csv";

            if (_openFileDialogService?.ShowDialog() != true)
                return;

            // Do the loading.
            var fileName = _openFileDialogService.FileName;
            var values = File.ReadAllLines(fileName).Select(v => TraverseObject.FromCsv(v)).ToList();

            TraverseItems.Clear();
            foreach (var traverseObject in values)
            {
                TraverseItems.Add(traverseObject);
            }
        }

        private void SaveFile()
        {
            _saveFileDialogService.DefaultExt = ".csv";
            _saveFileDialogService.Filter = "CSV Files (*.csv)|*.csv";

            if (_saveFileDialogService.ShowDialog() != true)
                return;

            // Do the saving.
            var fileName = _saveFileDialogService.FileName;
            File.WriteAllLines(fileName, TraverseItems.Select(t => t.ToCsv()));
        }

        private void Zoom()
        {
            _traverseService?.ZoomTo(TraverseItems);
        }

        private void SelectLine()
        {
            var trav = _traverseService?.SelectLines();

            if (trav == null)
                return;

            TraverseItems.Clear();
            foreach (var traverseObject in trav)
                TraverseItems.Add(traverseObject);

            UpdateIndex();
            NotifyPropertyChanged(nameof(TraverseItems));
            CloseTraverse();
        }

        private void SetBasePoint()
        {
            _traverseService?.SetBasePoint();

            if (TraverseItems.Count > 0)
            {
                _traverseService?.DrawTransientLines(TraverseItems);
            }
        }

        private void CloseWindow()
        {
            _traverseService?.ClearGraphics();
        }

        private void GridUpdated()
        {
            CloseTraverse();
            _traverseService?.DrawTransientLines(TraverseItems);
        }

        private void AddRow()
        {
            var ti = new TraverseObject();
            TraverseItems.Add(ti);

            UpdateIndex();
        }

        private void RemoveRow()
        {
            if (SelectedTraverseItem == null)
                return;

            _ = TraverseItems.Remove(SelectedTraverseItem);
            UpdateIndex();
        }

        private void ClearTraverse()
        {
            TraverseItems.Clear();
        }

        private void CloseTraverse()
        {
            if (TraverseItems.Count < 2)
            {
                return;
            }

            var coordinates = PointHelpers.TraverseObjectsToCoordinates(TraverseItems, new Point(0,0));
            var distance = PointHelpers.GetDistanceBetweenPoints(coordinates[coordinates.Count - 1], coordinates[0]);
            var angle = AngleHelpers.GetAngleBetweenPoints(coordinates[coordinates.Count - 1], coordinates[0]);

            CloseDistance = $"{distance:0.000}";
            CloseBearing = angle.ToString();
        }

        private void DrawTraverse()
        {
            _traverseService.DrawLines(TraverseItems);
        }

        private void FeetToMeters()
        {
            if (SelectedTraverseItem == null)
                return;

            var index = TraverseItems.IndexOf(SelectedTraverseItem);

            var distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = MathHelpers.ConvertFeetToMeters(distance);
            CloseTraverse();
        }

        private void LinksToMeters()
        {
            if (SelectedTraverseItem == null)
                return;

            var index = TraverseItems.IndexOf(SelectedTraverseItem);

            var distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = MathHelpers.ConvertLinkToMeters(distance);
            CloseTraverse();
        }

        private void FlipBearing()
        {
            if (SelectedTraverseItem == null)
                return;

            Angle angle;
            if (SelectedTraverseItem.Angle.Degrees > 180)
                angle = SelectedTraverseItem.Angle - new Angle(180);
            else
                angle = SelectedTraverseItem.Angle + new Angle(180);

            SelectedTraverseItem.Bearing = angle.ToDouble();
            CloseTraverse();
        }

        private void ShowHelp()
        {
            _processService?.Start(@"Resources\3DSCivilSurveySuite.chm");
        }

        private void UpdateIndex()
        {
            var i = 0;
            foreach (var item in TraverseItems)
            {
                item.Index = i;
                i++;
            }
        }
    }
}