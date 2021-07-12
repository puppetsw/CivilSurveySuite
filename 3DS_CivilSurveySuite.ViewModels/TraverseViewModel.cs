using System.Collections.ObjectModel;
using System.Diagnostics;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.ViewModels.Helpers;
using _3DS_CivilSurveySuite_ACADBase21;

//TODO: Add a button to select the bearing from an existing line, pline segment.
namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// ViewModel for TraverseView
    /// </summary>
    public class TraverseViewModel : ViewModelBase
    {
        private string _closeBearing = "0°00'00\"";
        private string _closeDistance = "0.000";
        private bool _commandRunning;

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

        public RelayCommand AddRowCommand => new RelayCommand((_) => AddRow(), (_) => true);
        public RelayCommand RemoveRowCommand => new RelayCommand((_) => RemoveRow(), (_) => true);
        public RelayCommand ClearCommand => new RelayCommand((_) => ClearTraverse(), (_) => true);
        public RelayCommand DrawCommand => new RelayCommand((_) => DrawTraverse(), (_) => true);
        public RelayCommand FeetToMetersCommand => new RelayCommand((_) => FeetToMeters(), (_) => true);
        public RelayCommand LinksToMetersCommand => new RelayCommand((_) => LinksToMeters(), (_) => true);
        public RelayCommand FlipBearingCommand => new RelayCommand((_) => FlipBearing(), (_) => true);
        public RelayCommand ShowHelpCommand => new RelayCommand((_) => ShowHelp(), (_) => true);
        public RelayCommand CellUpdatedEvent => new RelayCommand((_) => CloseTraverse(), (_) => true);

        public TraverseViewModel()
        {
            TraverseItems = new ObservableCollection<TraverseObject>();
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

            var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point(0,0));

            var distance = MathHelpers.DistanceBetweenPoints(coordinates[0], coordinates[coordinates.Count - 1]);
            var angle = MathHelpers.AngleBetweenPoints(coordinates[0], coordinates[coordinates.Count - 1]);

            CloseDistance = $"{distance:0.000}";
            CloseBearing = angle.ToString();
        }

        private void DrawTraverse()
        {
            if (!_commandRunning)
                _commandRunning = true;
            else
                return; //exit if command running

            Traverse.DrawTraverse(TraverseItems);

            _commandRunning = false;
        }

        private void FeetToMeters()
        {
            if (SelectedTraverseItem == null) return;

            int index = TraverseItems.IndexOf(SelectedTraverseItem);

            double distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = MathHelpers.ConvertFeetToMeters(distance);
        }

        private void LinksToMeters()
        {
            if (SelectedTraverseItem == null) return;

            int index = TraverseItems.IndexOf(SelectedTraverseItem);

            double distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = MathHelpers.ConvertLinkToMeters(distance);
        }

        private void FlipBearing()
        {
            if (SelectedTraverseItem == null) return;

            int index = TraverseItems.IndexOf(SelectedTraverseItem);
            var dms180 = new Angle(180.0000);

            var dms = dms180 - SelectedTraverseItem.Angle;

            TraverseItems[index].Bearing = dms.ToDouble();
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