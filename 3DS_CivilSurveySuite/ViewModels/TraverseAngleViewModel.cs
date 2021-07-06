using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;

namespace _3DS_CivilSurveySuite.ViewModels
{
    public class TraverseAngleViewModel : ViewModelBase
    {
        private Point2d _basePoint;
        private bool _basePointFlag;
        private string _closeBearing;
        private string _closeDistance;
        private bool _commandRunning;

        public ObservableCollection<TraverseAngleObject> TraverseAngles { get; set; }

        public TraverseAngleObject SelectedTraverseAngle { get; set; }

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

        //public RelayCommand ClosureCommand => new RelayCommand((_) => ClosureReport(), (_) => true);
        public RelayCommand DrawCommand => new RelayCommand((_) => DrawTraverse(), (_) => true);

        public RelayCommand SetBasePointCommand => new RelayCommand((_) => SetBasePoint(), (_) => true);

        //public RelayCommand FeetToMetersCommand => new RelayCommand((_) => FeetToMeters(), (_) => true);
        //public RelayCommand LinksToMetersCommand => new RelayCommand((_) => LinksToMeters(), (_) => true);
        //public RelayCommand FlipBearingCommand => new RelayCommand((_) => FlipBearing(), (_) => true);
        public RelayCommand RefreshTraverseCommand => new RelayCommand((_) => TransientGraphics.DrawTransientPreview(MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, _basePoint)), (_) => true);
        //public RelayCommand ShowHelpCommand => new RelayCommand((_) => ShowHelp(), (_) => true);

        public RelayCommand LostFocusEvent => new RelayCommand((_) => TransientGraphics.DrawTransientPreview(MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, _basePoint)), (_) => true);

        public RelayCommand ClearGraphicsEvent => new RelayCommand((_) => TransientGraphics.ClearTransientGraphics(), (_) => true);

        public TraverseAngleViewModel()
        {
            TraverseAngles = new ObservableCollection<TraverseAngleObject>();
            TraverseAngles.CollectionChanged += TraverseItems_CollectionChanged;

            //HACK: clear graphics on drawing switch
            //AcaddocManager.DocumentToBeDeactivated += (s, e) => TransientGraphics.ClearTransientGraphics();
        }

        private void TraverseItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TraverseAngleObject item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (TraverseAngleObject item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e) => CloseTraverse();

        private void CloseTraverse()
        {
            if (TraverseAngles.Count < 2)
            {
                return;
            }

            var coordinates = MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, new Point2d(0, 0));

            Point2d lastCoord = coordinates[coordinates.Count - 1];
            Point2d firstCoord = coordinates[0];

            double distance = MathHelpers.DistanceBetweenPoints(firstCoord.X, lastCoord.X, firstCoord.Y, lastCoord.Y);
            Angle angle = MathHelpers.AngleBetweenPoints(lastCoord.X, firstCoord.X, lastCoord.Y, firstCoord.Y);

            CloseDistance = string.Format("{0:0.000}", distance);
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

            //DrawTransientPreview();
            TransientGraphics.DrawTransientPreview(MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, _basePoint));
        }

        private void ClearTraverse()
        {
            TraverseAngles.Clear();
            TransientGraphics.ClearTransientGraphics();
        }

        private void SetBasePoint()
        {
            Utils.SetFocusToDwgView();
            PromptPointOptions ppo = new PromptPointOptions("\n3DS> Select a base point: ");
            PromptPointResult ppr = AutoCADApplicationManager.Editor.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK) return; //if we have a valid point

            //_basePoint = ppr.Value;
            _basePoint = new Point2d(ppr.Value.X, ppr.Value.Y);
            _basePointFlag = true;

            AutoCADApplicationManager.Editor.WriteMessage("Base point set: X:" + _basePoint.X + " Y:" + _basePoint.Y + "\n");

            if (TraverseAngles.Count < 1)
            {
                return;
            }

            //DrawTransientPreview();
            TransientGraphics.DrawTransientPreview(MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, _basePoint));
        }

        private void DrawTraverse()
        {
            //set focus to acad window
            Utils.SetFocusToDwgView();
            //if no basepoint set
            if (!_basePointFlag)
            {
                SetBasePoint(); //set basepoint
            }

            if (!_commandRunning)
            {
                _commandRunning = true;
            }
            else
            {
                return; //exit if command running
            }

            //get coordinates based on traverse data
            var coordinates = MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, new Point2d(_basePoint.X, _basePoint.Y));

            PromptKeywordOptions pko = new PromptKeywordOptions("\n3DS> Accept traverse and draw linework? ")
            {
                AppendKeywordsToMessage = true
            };
            pko.Keywords.Add("Accept");
            pko.Keywords.Add("Cancel");
            pko.Keywords.Add("Redraw");

            //lock acad document and start transaction
            using (Transaction tr = AutoCADApplicationManager.ActiveDocument.TransactionManager.StartLockedTransaction())
            {
                TransientGraphics.ClearTransientGraphics();
                //draw first transient traverse
                //DrawTransientTraverse(coordinates);
                TransientGraphics.DrawTransientTraverse(coordinates);
                var cancelled = false;
                PromptResult prResult;
                do
                {
                    prResult = AutoCADApplicationManager.Editor.GetKeywords(pko);
                    if (prResult.Status == PromptStatus.Keyword || prResult.Status == PromptStatus.OK)
                    {
                        switch (prResult.StringResult)
                        {
                            case "Redraw": //if redraw update the coordinates clear transients and redraw
                                TransientGraphics.ClearTransientGraphics();
                                coordinates = MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, new Point2d(_basePoint.X, _basePoint.Y));
                                TransientGraphics.DrawTransientTraverse(coordinates);
                                break;
                            case "Accept":
                                DrawTraverseLinework(tr, coordinates);
                                cancelled = true;
                                break;
                            case "Cancel":
                                cancelled = true;
                                break;
                        }
                    }
                } while (prResult.Status != PromptStatus.Cancel && prResult.Status != PromptStatus.Error && !cancelled);

                tr.Commit();
            }

            TransientGraphics.ClearTransientGraphics();
            _commandRunning = false;
        }

        private void DrawTraverseLinework(Transaction tr, IReadOnlyList<Point2d> coordinates)
        {
            var i = 1;
            foreach (Point2d point in coordinates)
            {
                var bt = (BlockTable) tr.GetObject(AutoCADApplicationManager.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                if (coordinates.Count == i)
                {
                    break;
                }

                var ln = new Line(new Point3d(point.X, point.Y, 0), new Point3d(coordinates[i].X, coordinates[i].Y, 0));

                btr.AppendEntity(ln);
                tr.AddNewlyCreatedDBObject(ln, true);
                i++;
            }
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