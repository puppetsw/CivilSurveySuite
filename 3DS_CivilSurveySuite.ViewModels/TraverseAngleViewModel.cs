using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.ViewModels
{
    public class TraverseAngleViewModel : ViewModelBase
    {
        private string _closeBearing;
        private string _closeDistance;
        private bool _commandRunning;
        private readonly IViewerService _viewerService;
        private readonly ITraverseService _traverseService;

        // ReSharper disable UnusedMember.Global
        public ObservableCollection<TraverseAngleObject> TraverseAngles { get; } = new ObservableCollection<TraverseAngleObject>();

        public TraverseAngleObject SelectedTraverseAngle { get; set; }

        public IEnumerable<AngleReferenceDirection> ReferenceDirectionValues => Enum.GetValues(typeof(AngleReferenceDirection)).Cast<AngleReferenceDirection>();

        public IEnumerable<AngleRotationDirection> RotationDirectionValues => Enum.GetValues(typeof(AngleRotationDirection)).Cast<AngleRotationDirection>();

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

        public RelayCommand GridUpdatedCommand => new RelayCommand(GridUpdated, () => true);

        public RelayCommand ShowViewerCommand => new RelayCommand(ShowViewer, () => true);
        // ReSharper restore UnusedMember.Global

        public TraverseAngleViewModel(IViewerService viewerService, ITraverseService traverseService)
        {
            _viewerService = viewerService;
            _traverseService = traverseService;
        }

        private void GridUpdated()
        {
            CloseTraverse();
            _viewerService.AddGraphics(PointHelpers.TraverseAngleObjectsToCoordinates(TraverseAngles, new Point(0, 0)));
        }

        private void CloseTraverse()
        {
            if (TraverseAngles.Count < 2)
            {
                return;
            }

            var coordinates = PointHelpers.TraverseAngleObjectsToCoordinates(TraverseAngles, new Point(0, 0));

            double distance = PointHelpers.GetDistanceBetweenPoints(coordinates[coordinates.Count - 1], coordinates[0]);
            Angle angle = AngleHelpers.GetAngleBetweenPoints(coordinates[coordinates.Count - 1], coordinates[0]);

            CloseDistance = $"{distance:0.000}";
            CloseBearing = angle.ToString();
        }

        private void AddRow()
        {
            var ti = new TraverseAngleObject();
            TraverseAngles.Add(ti);

            //hack: add index property and update method
            UpdateIndex();
        }

        private void RemoveRow()
        {
            if (SelectedTraverseAngle == null)
            {
                return;
            }

            _ = TraverseAngles.Remove(SelectedTraverseAngle);
            UpdateIndex();
        }

        private void ClearTraverse()
        {
            TraverseAngles.Clear();
        }

        private void DrawTraverse()
        {
            // If user clicks DrawTraverse again we want to stop the command from
            // running again if it is already running.
            if (!_commandRunning)
                _commandRunning = true;
            else
                return;

            _traverseService.DrawTraverse(TraverseAngles);

            _commandRunning = false;
        }

        private void FeetToMeters()
        {
            if (SelectedTraverseAngle == null) return;

            int index = TraverseAngles.IndexOf(SelectedTraverseAngle);

            double distance = TraverseAngles[index].Distance;
            TraverseAngles[index].Distance = MathHelpers.ConvertFeetToMeters(distance);
            CloseTraverse();
        }

        private void LinksToMeters()
        {
            if (SelectedTraverseAngle == null) return;

            int index = TraverseAngles.IndexOf(SelectedTraverseAngle);

            double distance = TraverseAngles[index].Distance;
            TraverseAngles[index].Distance = MathHelpers.ConvertLinkToMeters(distance);
            CloseTraverse();
        }

        private void FlipBearing()
        {
            if (SelectedTraverseAngle == null) 
                return;

            Angle angle;
            if (SelectedTraverseAngle.Angle.Degrees > 180)
                angle = SelectedTraverseAngle.Angle - new Angle(180);
            else
                angle = SelectedTraverseAngle.Angle + new Angle(180);

            SelectedTraverseAngle.Bearing = angle.ToDouble();
            CloseTraverse();
        }

        private void ShowHelp() //TODO: Service? for testing?
        {
            _ = Process.Start(@"Resources\3DSCivilSurveySuite.chm");
        }

        private void ShowViewer()
        {
            _viewerService?.ShowWindow();
        }

        /// <summary>
        /// Updates the index property based on collection position
        /// </summary>
        private void UpdateIndex()
        {
            for (var i = 0; i < TraverseAngles.Count; i++)
            {
                TraverseAngles[i].Index = i;
            }
        }
    }
}