// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

//TODO: Add a button to select the bearing from an existing line, pline segment.
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

        public ObservableCollection<TraverseObject> TraverseItems { get; set; } = new ObservableCollection<TraverseObject>();

        public TraverseObject SelectedTraverseItem { get; set; }

        public string CloseDistance
        {
            get => _closeDistance;
            set
            {
                _closeDistance = value;
                NotifyPropertyChanged();
            }
        }

        public string CloseBearing
        {
            get => _closeBearing;
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

        public TraverseViewModel(ITraverseService traverseService, IProcessService processService)
        {
            _traverseService = traverseService;
            _processService = processService;
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

            //hack: add index property and update method
            UpdateIndex();
        }

        private void RemoveRow()
        {
            if (SelectedTraverseItem == null) return;

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
            if (SelectedTraverseItem == null) return;

            int index = TraverseItems.IndexOf(SelectedTraverseItem);

            double distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = MathHelpers.ConvertFeetToMeters(distance);
            CloseTraverse();
        }

        private void LinksToMeters()
        {
            if (SelectedTraverseItem == null) return;

            int index = TraverseItems.IndexOf(SelectedTraverseItem);

            double distance = TraverseItems[index].Distance;
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
            int i = 0;
            foreach (TraverseObject item in TraverseItems)
            {
                item.Index = i;
                i++;
            }
        }
    }
}