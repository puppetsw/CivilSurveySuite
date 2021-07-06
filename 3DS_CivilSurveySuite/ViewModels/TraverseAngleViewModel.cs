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
            var point = EditorUtils.GetBasePoint2d();

            if (point != null)
            {
                _basePoint = point.Value;
                _basePointFlag = true;
            }
            else
            {
                return;
            }

            AutoCADApplicationManager.Editor.WriteMessage("Base point set: X:" + _basePoint.X + " Y:" + _basePoint.Y + "\n");

            if (TraverseAngles.Count < 1)
            {
                return;
            }

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

            var pko = new PromptKeywordOptions("\n3DS> Accept traverse and draw linework? ")
            {
                AppendKeywordsToMessage = true
            };
            pko.Keywords.Add("Accept");
            pko.Keywords.Add("Cancel");
            pko.Keywords.Add("Redraw");

            //lock acad document and start transaction
            using (Transaction tr = AutoCADApplicationManager.ActiveDocument.TransactionManager.StartLockedTransaction())
            {
                //draw first transient traverse
                TransientGraphics.ClearTransientGraphics();
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
                                Lines.DrawLines(tr, coordinates);
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