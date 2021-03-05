using _3DS_CivilSurveySuite.Helpers;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System.Collections.ObjectModel;

namespace _3DS_CivilSurveySuite.Traverse.ViewModels
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
            AddStubItems();
        }
        #endregion

        #region Commands

        public RelayCommand AddRowCommand => new RelayCommand((_) => AddRow(), (_) => true);
        public RelayCommand RemoveRowCommand => new RelayCommand((_) => RemoveRow(), (_) => true);
        public RelayCommand ClearCommand => new RelayCommand((_) => ClearTraverse(), (_) => true);
        public RelayCommand ClosureCommand => new RelayCommand((_) => CloseTraverse(), (_) => true);
        public RelayCommand DrawCommand => new RelayCommand((_) => DrawTraverse(), (_) => true);
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

        private void ClearTraverse()
        {
            TraverseItems.Clear();
            AddStubItems();
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

        private void DrawTraverse()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
            //get basepoints
            PromptPointOptions ppo = new PromptPointOptions("\n3DS Traverse: Pick base point");
            PromptPointResult ppr = Editor.GetPoint(ppo);

            if (ppr.Value == null)
                return;

            var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(ppr.Value.X, ppr.Value.Y));

            using (Acaddoc.LockDocument())
            {
                using (Transaction tr = startTransaction())
                {
                    int i = 1;
                    foreach (Point2d point in coordinates)
                    {
                        BlockTable bt = (BlockTable)tr.GetObject(Acaddoc.Database.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                        Line ln;

                        if (coordinates.Count == i)
                            break; //ln = new Line(new Point3d(point.X, point.Y, 0), new Point3d(coordinates[0].X, coordinates[0].Y, 0));
                        else
                            ln = new Line(new Point3d(point.X, point.Y, 0), new Point3d(coordinates[i].X, coordinates[i].Y, 0));

                        btr.AppendEntity(ln);
                        tr.AddNewlyCreatedDBObject(ln, true);
                        i++;
                    }
                    tr.Commit();
                }
            }
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

        private void AddStubItems()
        {
            TraverseItems.Add(new TraverseItem()
            {
                Index = 0,
                Bearing = 0,
                Distance = 0,
            });
            TraverseItems.Add(new TraverseItem()
            {
                Index = 1,
                Bearing = 0,
                Distance = 0,
            });
            TraverseItems.Add(new TraverseItem()
            {
                Index = 2,
                Bearing = 0,
                Distance = 0,
            });
            TraverseItems.Add(new TraverseItem()
            {
                Index = 3,
                Bearing = 0,
                Distance = 0,
            });
        }
    }
}