using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;

//TODO: Add a button to select the bearing from an existing line, pline segment.
//TODO: Hopefully remove all civil/acad references from this class
namespace _3DS_CivilSurveySuite.ViewModels
{
    public interface ITraverseViewModel
    {
        void DrawTraverse();
        void SetBasePoint();
        void DrawTraverseLinework(Transaction tr, IReadOnlyList<Point2d> coordinates);
    }

    /// <summary>
    /// ViewModel for TraverseView
    /// </summary>
    public class TraverseViewModel : ViewModelBase, ITraverseViewModel
    {
        private Point3d _basePoint;
        private bool _basePointFlag;
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
        public RelayCommand ClosureCommand => new RelayCommand((_) => ClosureReport(), (_) => true);
        public RelayCommand DrawCommand => new RelayCommand((_) => DrawTraverse(), (_) => true);
        public RelayCommand SetBasePointCommand => new RelayCommand((_) => SetBasePoint(), (_) => true);
        public RelayCommand FeetToMetersCommand => new RelayCommand((_) => FeetToMeters(), (_) => true);
        public RelayCommand LinksToMetersCommand => new RelayCommand((_) => LinksToMeters(), (_) => true);
        public RelayCommand FlipBearingCommand => new RelayCommand((_) => FlipBearing(), (_) => true);
        public RelayCommand RefreshTraverseCommand => new RelayCommand((_) => RefreshTransient(), (_) => true);
        public RelayCommand ShowHelpCommand => new RelayCommand((_) => ShowHelp(), (_) => true);
        public RelayCommand LostFocusEvent => new RelayCommand((_) => RefreshTransient(), (_) => true);

        public TraverseViewModel()
        {
            TraverseItems = new ObservableCollection<TraverseObject>();
            TraverseItems.CollectionChanged += TraverseItems_CollectionChanged;

            //HACK: clear graphics on drawing switch
            //AcaddocManager.DocumentToBeDeactivated += (s, e) => TransientGraphics.ClearTransientGraphics();
        }

        private void RefreshTransient()
        {
            TransientGraphics.DrawTransientPreview(MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(_basePoint.X, _basePoint.Y)));
        }

        private void TraverseItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TraverseObject item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (TraverseObject item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e) => CloseTraverse();

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

            TransientGraphics.DrawTransientPreview(MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(_basePoint.X, _basePoint.Y)));
        }

        private void ClearTraverse()
        {
            TraverseItems.Clear();
            TransientGraphics.ClearTransientGraphics();
        }

        private void CloseTraverse()
        {
            if (TraverseItems.Count < 2)
            {
                return;
            }

            var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(0, 0));

            Point2d lastCoord = coordinates[coordinates.Count - 1];
            Point2d firstCoord = coordinates[0];

            var distance = MathHelpers.DistanceBetweenPoints(firstCoord.X, lastCoord.X, firstCoord.Y, lastCoord.Y);
            var angle = MathHelpers.AngleBetweenPoints(lastCoord.X, firstCoord.X, lastCoord.Y, firstCoord.Y);

            CloseDistance = string.Format("{0:0.000}", distance);
            CloseBearing = angle.ToString();
        }

        private void ClosureReport()
        {
            //from, easting, northing, bearing, distance, to
            //2d area, misclose bearing and distance

            //set focus to acad window
            Utils.SetFocusToDwgView();

            if (!_commandRunning)
            {
                _commandRunning = true;
            }
            else
            {
                return; //exit if command already running
            }

            PromptPointOptions ppo = new PromptPointOptions("\n3DS> Select location for report text: ");
            PromptPointResult ppr = AutoCADApplicationManager.Editor.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK) // Check if we have a valid base point
            {
                return;
            }

            using (Transaction tr = AutoCADApplicationManager.ActiveDocument.TransactionManager.StartLockedTransaction())
            {
                var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(_basePoint.X, _basePoint.Y));

                const double rowHeight = 8;
                const double colWidth = 30;
                const double textHeight = 3;

                var tb = new Table();
                tb.TableStyle = AutoCADApplicationManager.ActiveDocument.Database.Tablestyle;
                tb.SetSize(1, 6);
                tb.SetColumnWidth(colWidth);
                tb.SetRowHeight(rowHeight);
                tb.Position = ppr.Value;

                tb.Cells[0, 0].TextString = "Closure Report";
                tb.Cells[0, 0].TextHeight = textHeight;

                tb.InsertRows(tb.Rows.Count, rowHeight, 1);
                int rowIndex = tb.Rows.Count - 1;
                string[] headerData = { "From", "Easting", "Northing", "Bearing", "Distance", "To" };

                //create header
                for (int i = 0; i < headerData.Length; i++)
                {
                    var cell = tb.Cells[rowIndex, i];
                    cell.TextString = headerData[i];
                    cell.TextHeight = textHeight;
                    cell.Alignment = CellAlignment.MiddleCenter;
                }

                using (var point = new Autodesk.AutoCAD.DatabaseServices.Polyline())
                {
                    for (int i = 0; i < coordinates.Count - 1; i++)
                    {
                        //add point to polyline to calculate area
                        point.AddVertexAt(i, coordinates[i], 0, 0, 0);

                        //form report text
                        string from = TraverseItems[i].Index.ToString();
                        string easting = $"{coordinates[i].X:#,0.000}";
                        string northing = $"{coordinates[i].Y:#,0.000}";
                        string bearing = TraverseItems[i].DMSBearing.ToString();
                        string distance = $"{TraverseItems[i].Distance:#,0.000}";
                        string to;

                        if (i != coordinates.Count - 2) //-2 there is always going to be 1 less bearing/distance
                        {
                            to = TraverseItems[i + 1].Index.ToString();
                        }
                        else
                        {
                            to = TraverseItems[0].Index.ToString(); //if it's last one assign starting id.
                        }

                        tb.InsertRows(tb.Rows.Count, rowHeight, 1);
                        rowIndex = tb.Rows.Count - 1;

                        string[] rowData = { from, easting, northing, bearing, distance, to };
                        for (int j = 0; j < rowData.Length; j++)
                        {
                            var cell = tb.Cells[rowIndex, j];
                            cell.TextString = rowData[j];
                            cell.TextHeight = textHeight;
                            cell.Alignment = CellAlignment.MiddleCenter;
                        }
                    }

                    var area = Math.Round(point.Area, 3);

                    tb.InsertRows(tb.Rows.Count, rowHeight, 1);
                    rowIndex = tb.Rows.Count - 1;

                    //create base of report
                    var cellRange = CellRange.Create(tb, rowIndex, 0, rowIndex, tb.Columns.Count - 1);
                    tb.MergeCells(cellRange);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Closure Distance: {CloseDistance}");
                    sb.AppendLine($"Closure Bearing: {CloseBearing}");
                    sb.AppendLine($"2D Area: {area}m²");

                    tb.Cells[rowIndex, 0].TextString = sb.ToString();
                    tb.Cells[rowIndex, 0].TextHeight = textHeight;
                    tb.Cells[rowIndex, 0].Alignment = CellAlignment.MiddleLeft;
                }

                BlockTable bt = (BlockTable) tr.GetObject(AutoCADApplicationManager.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord ms =
                    (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                ms.AppendEntity(tb);
                tr.AddNewlyCreatedDBObject(tb, true);

                // Finally we commit our transaction
                tr.Commit();
            }

            _commandRunning = false;
        }

        public void DrawTraverse()
        {
            //set focus to acad window
            Utils.SetFocusToDwgView();
            //if no basepoint set
            if (!_basePointFlag)
                SetBasePoint(); //set basepoint

            if (!_commandRunning)
                _commandRunning = true;
            else
                return; //exit if command running

            //get coordinates based on traverse data
            var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(_basePoint.X, _basePoint.Y));

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
                                coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(_basePoint.X, _basePoint.Y));
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

        private void FeetToMeters() //BUG: Doesn't update transient graphics
        {
            if (SelectedTraverseItem == null) return;

            int index = TraverseItems.IndexOf(SelectedTraverseItem);

            double distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = MathHelpers.ConvertFeetToMeters(distance);
        }

        private void LinksToMeters() //BUG: Doesn't update transient graphics
        {
            if (SelectedTraverseItem == null) return;

            int index = TraverseItems.IndexOf(SelectedTraverseItem);

            double distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = MathHelpers.ConvertLinkToMeters(distance);
        }

        private void FlipBearing() //BUG: Doesn't update transient graphics
        {
            if (SelectedTraverseItem == null) return;

            int index = TraverseItems.IndexOf(SelectedTraverseItem);
            var dms180 = new Angle(180.0000);

            var dms = dms180 - SelectedTraverseItem.DMSBearing;

            TraverseItems[index].Bearing = dms.ToDouble();
        }

        public void SetBasePoint()
        {
            var point = EditorUtils.GetBasePoint3d();

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

            if (TraverseItems.Count < 1)
            {
                return;
            }

            TransientGraphics.DrawTransientPreview(MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(_basePoint.X, _basePoint.Y)));
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

        public void DrawTraverseLinework(Transaction tr, IReadOnlyList<Point2d> coordinates)
        {
            int i = 1;
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
    }
}