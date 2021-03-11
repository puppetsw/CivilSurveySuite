using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Models;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Internal;

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// Traverse ViewModel for TraversePalette view
    /// </summary>
    public class TraverseViewModel : ViewModelBase
    {
        #region Private Members

        DBObjectCollection _markers = null;
        Point3d m_basePoint;
        bool m_basePointFlag = false;

        private string closeDistance = "0.000";
        private string closeBearing = "0°00'00\"";

        #endregion

        #region Properties
        public ObservableCollection<TraverseItem> TraverseItems { get; set; }

        public TraverseItem SelectedTraverseItem { get; set; }

        public string CloseDistance { get => closeDistance; set { closeDistance = value; NotifyPropertyChanged(); } }

        public string CloseBearing { get => closeBearing; set { closeBearing = value; NotifyPropertyChanged(); } }

        #endregion

        #region Constructor/Deconstructor
        public TraverseViewModel()
        {
            TraverseItems = new ObservableCollection<TraverseItem>();
            TraverseItems.CollectionChanged += TraverseItems_CollectionChanged;
        }

        ~TraverseViewModel()
        {
        }
        #endregion

        #region Commands

        public RelayCommand AddRowCommand => new RelayCommand((_) => AddRow(), (_) => true);
        public RelayCommand RemoveRowCommand => new RelayCommand((_) => RemoveRow(), (_) => true);
        public RelayCommand ClearCommand => new RelayCommand((_) => ClearTraverse(), (_) => true);
        public RelayCommand ClosureCommand => new RelayCommand((_) => CloseTraverse(), (_) => true);
        public RelayCommand DrawCommand => new RelayCommand((_) => DrawTraverse(), (_) => true);
        public RelayCommand SetBasePointCommand => new RelayCommand((_) => SetBasePoint(), (_) => true);
        public RelayCommand FeetToMetersCommand => new RelayCommand((_) => FeetToMeters(), (_) => true);
        public RelayCommand LinksToMetersCommand => new RelayCommand((_) => LinksToMeters(), (_) => true);
        public RelayCommand FlipBearingCommand => new RelayCommand((_) => FlipBearing(), (_) => true);

        #endregion

        #region Events

        private void TraverseItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TraverseItem item in e.NewItems)
                    item.PropertyChanged += (s, ev) => CloseTraverse();

            if (e.OldItems != null)
                foreach (TraverseItem item in e.OldItems)
                    item.PropertyChanged -= (s, ev) => CloseTraverse();
        }

        #endregion

        #region Command Methods

        private void AddRow()
        {
            var ti = new TraverseItem();
            TraverseItems.Add(ti);
            //hack: add index property and update method
            UpdateIndex();
        }

        private void RemoveRow()
        {
            if (SelectedTraverseItem == null) return;

            TraverseItems.Remove(SelectedTraverseItem);
            UpdateIndex();
        }

        private void ClearTraverse()
        {
            TraverseItems.Clear();
        }

        private void CloseTraverse()
        {
            if (TraverseItems.Count < 2)
                return;
            
            var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(0,0));

            Point2d lastCoord = coordinates[coordinates.Count - 1];
            Point2d firstCoord = coordinates[0];

            var distance = MathHelpers.DistanceBetweenPoints(firstCoord.X, lastCoord.X, firstCoord.Y, lastCoord.Y);
            var angle = MathHelpers.AngleBetweenPoints(lastCoord.X, firstCoord.X, lastCoord.Y, firstCoord.Y);

            CloseDistance = string.Format("{0:0.000}", distance);
            CloseBearing = angle.ToString();
        }

        private void DrawTraverse()
        {
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
            //get basepoints
            if (!m_basePointFlag)
                SetBasePoint();

            var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(m_basePoint.X, m_basePoint.Y));

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

        private void FlipBearing()
        {
            if (SelectedTraverseItem == null) return;
            
            int index = TraverseItems.IndexOf(SelectedTraverseItem);
            var dms180 = new DMS(180.0000);

            var dms = dms180 - SelectedTraverseItem.DMSBearing;

            TraverseItems[index].Bearing = dms.ToDouble();
        }

        private void SetBasePoint()
        {
            Utils.SetFocusToDwgView();
            PromptPointOptions ppo = new PromptPointOptions("\n3DS> Select a base point: ");
            PromptPointResult ppr = Editor.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK) return; //if we have a valid point

            m_basePoint = ppr.Value;
            m_basePointFlag = true;

            WriteMessage("Base point set: X:" + m_basePoint.X + " Y:" + m_basePoint.Y + "\n");
        }

        #endregion

        #region Private Methods

        private void UpdateIndex()
        {
            int i = 0;
            foreach (TraverseItem item in TraverseItems)
            {
                item.Index = i;
                i++;
            }
        }

        #endregion


        #region TransientGraphics

        private void DrawTransientTraverse()
        {
            if (!m_basePointFlag)
                return;

            ClearTransientGraphics();
            _markers = new DBObjectCollection();
            using (Transaction tr = startTransaction())
            {
                var coords = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(m_basePoint.X, m_basePoint.Y));
                int i = 1;
                foreach (Point2d point in coords)
                {
                    //draw the coordlines
                    Line ln;
                    if (coords.Count == i)
                        break;
                    else
                        ln = new Line(new Point3d(point.X, point.Y, 0), new Point3d(coords[i].X, coords[i].Y, 0));

                    _markers.Add(ln);

                    TransientManager tm = TransientManager.CurrentTransientManager;
                    IntegerCollection intCol = new IntegerCollection();
                    tm.AddTransient(ln, TransientDrawingMode.Highlight, 128, intCol);

                    i++;
                }
                Acaddoc.TransactionManager.QueueForGraphicsFlush();
                tr.Commit();
            }
            //Editor.Regen();

        }

        /// <summary>
        /// Clear all Transient Graphics
        /// </summary>
        public void ClearTransientGraphics()
        {
            TransientManager tm = TransientManager.CurrentTransientManager;
            IntegerCollection intCol = new IntegerCollection();
            //tm.EraseTransients(TransientDrawingMode.Highlight, 128, intCol);

            if (_markers != null)
            {
                foreach (DBObject marker in _markers)
                {
                    tm.EraseTransient(marker, intCol);
                    marker.Dispose();
                }
            }
        }

        #endregion
    }
}