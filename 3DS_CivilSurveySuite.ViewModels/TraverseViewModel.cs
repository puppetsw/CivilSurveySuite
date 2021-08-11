using System.Collections.ObjectModel;
using System.Diagnostics;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;

//TODO: Add a button to select the bearing from an existing line, pline segment.
namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// ViewModel for TraverseView
    /// </summary>
    public class TraverseViewModel : ViewModelBase
    {
        private string _closeBearing;
        private string _closeDistance;
        private bool _commandRunning;
        private readonly IViewerService _viewerService;
        private readonly ITraverseService _traverseService;

        public ObservableCollection<TraverseObject> TraverseItems { get; set; }

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

        public RelayCommand AddRowCommand => new RelayCommand(_ => AddRow(), _ => true);
        public RelayCommand RemoveRowCommand => new RelayCommand(_ => RemoveRow(), _ => true);
        public RelayCommand ClearCommand => new RelayCommand(_ => ClearTraverse(), _ => true);
        public RelayCommand DrawCommand => new RelayCommand(_ => DrawTraverse(), _ => true);
        public RelayCommand FeetToMetersCommand => new RelayCommand(_ => FeetToMeters(), _ => true);
        public RelayCommand LinksToMetersCommand => new RelayCommand(_ => LinksToMeters(), _ => true);
        public RelayCommand FlipBearingCommand => new RelayCommand(_ => FlipBearing(), _ => true);
        public RelayCommand ShowHelpCommand => new RelayCommand(_ => ShowHelp(), _ => true);
        public RelayCommand GridUpdatedCommand => new RelayCommand(_ => GridUpdated(), _ => true);
        public RelayCommand ShowViewerCommand => new RelayCommand(_ => ShowViewer(), _ => true);

        public TraverseViewModel(IViewerService viewerService, ITraverseService traverseService)
        {
            TraverseItems = new ObservableCollection<TraverseObject>();
            _viewerService = viewerService;
            _traverseService = traverseService;
        }

        private void ShowViewer()
        {
            _viewerService?.ShowWindow();
        }

        private void GridUpdated()
        {
            CloseTraverse();
            _viewerService.AddGraphics(PointHelpers.TraverseObjectsToCoordinates(TraverseItems, new Point(0, 0)));
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
            var distance = PointHelpers.DistanceBetweenPoints(coordinates[coordinates.Count - 1], coordinates[0]);
            var angle = AngleHelpers.AngleBetweenPoints(coordinates[coordinates.Count - 1], coordinates[0]);

            CloseDistance = $"{distance:0.000}";
            CloseBearing = angle.ToString();
        }

        private void DrawTraverse()
        {
            if (!_commandRunning)
                _commandRunning = true;
            else
                return; //exit if command running

            _traverseService.DrawTraverse(TraverseItems);

            _commandRunning = false;
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
            _ = Process.Start(@"Resources\3DSCivilSurveySuite.chm");
        }

        /// <summary>
        /// Updates the index property based on collection position
        /// </summary>
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