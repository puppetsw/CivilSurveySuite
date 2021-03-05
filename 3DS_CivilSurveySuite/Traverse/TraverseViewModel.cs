using _3DS_CivilSurveySuite.Helpers;
using Autodesk.AutoCAD.Geometry;
using System.Collections.ObjectModel;

namespace _3DS_CivilSurveySuite.Traverse
{
    public class TraverseViewModel : CivilBase
    {
        #region Properties
        public ObservableCollection<TraverseItem> TraverseItems { get; set; }

        public TraverseItem SelectedTraverseItem { get; set; }
        #endregion

        #region Constructor
        public TraverseViewModel()
        {
            TraverseItems = new ObservableCollection<TraverseItem>();
            TraverseItems.Add(new TraverseItem()
            {
                Index = 0,
                Bearing = 354.5020,
                Distance = 34.21,
            });
            TraverseItems.Add(new TraverseItem()
            {
                Index = 1,
                Bearing = 84.5020,
                Distance = 20.81,
            });
            TraverseItems.Add(new TraverseItem()
            {
                Index = 2,
                Bearing = 174.5020,
                Distance = 20.81,
            });
        }
        #endregion

        #region Commands

        public RelayCommand AddRowCommand => new RelayCommand((_) => AddRow(), (_) => true);
        public RelayCommand RemoveRowCommand => new RelayCommand((_) => RemoveRow(), (_) => true);
        public RelayCommand ClosureCommand => new RelayCommand((_) => CloseTraverse(), (_) => true);

        public RelayCommand FeetToMetersCommand => new RelayCommand((_) => FeetToMeters(), (_) => true);
        public RelayCommand LinksToMetersCommand => new RelayCommand((_) => LinksToMeters(), (_) => true);

        #endregion

        #region Command Methods

        private void AddRow()
        {
            TraverseItems.Add(new TraverseItem());
            //hack: add index property and update method
            TraverseItem.UpdateIndex(TraverseItems);
        }

        private void RemoveRow()
        {
            if (SelectedTraverseItem == null) return;

            TraverseItems.Remove(SelectedTraverseItem);
            TraverseItem.UpdateIndex(TraverseItems);
        }

        private void CloseTraverse()
        {
            var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(0,0));

            Point2d lastCoord = coordinates[coordinates.Count - 1];
            Point2d firstCoord = coordinates[0];

            var distance = MathHelpers.DistanceBetweenPoints(firstCoord.X, lastCoord.X, firstCoord.Y, lastCoord.Y);
            var angle = MathHelpers.AngleBetweenPoints(lastCoord.X, firstCoord.X, lastCoord.Y, firstCoord.Y);

            string message = string.Format("Closure results: distance {0}, bearing {1}\n", distance, angle.ToString());

            WriteMessage(message);
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

        #endregion
    }
}