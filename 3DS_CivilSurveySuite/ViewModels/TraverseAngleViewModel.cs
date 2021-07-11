using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ViewModels
{
    public class TraverseAngleViewModel : ViewModelBase
    {
        private string _closeBearing;
        private string _closeDistance;
        private bool _commandRunning;

        public ObservableCollection<TraverseAngleObject> TraverseAngles { get; set; } = new ObservableCollection<TraverseAngleObject>();

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

        public RelayCommand AddRowCommand => new RelayCommand((_) => AddRow(), (_) => true);

        public RelayCommand RemoveRowCommand => new RelayCommand((_) => RemoveRow(), (_) => true);

        public RelayCommand ClearCommand => new RelayCommand((_) => ClearTraverse(), (_) => true);

        public RelayCommand DrawCommand => new RelayCommand((_) => DrawTraverse(), (_) => true);

        //public RelayCommand FeetToMetersCommand => new RelayCommand((_) => FeetToMeters(), (_) => true);
        //public RelayCommand LinksToMetersCommand => new RelayCommand((_) => LinksToMeters(), (_) => true);
        //public RelayCommand FlipBearingCommand => new RelayCommand((_) => FlipBearing(), (_) => true);
        //public RelayCommand ShowHelpCommand => new RelayCommand((_) => ShowHelp(), (_) => true);

        public RelayCommand CellUpdatedEvent => new RelayCommand((_) => CellUpdated(), (_) => true);

        private void CellUpdated()
        {
            CloseTraverse();
        }

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

            var point = EditorUtils.GetBasePoint2d();

            if (point == null)
                return;

            AutoCADApplicationManager.Editor.WriteMessage($"\nBase point set: X:{point.Value.X} Y:{point.Value.Y}");

            //get coordinates based on traverse data
            var coordinates = MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, point.Value);

            var pko = new PromptKeywordOptions("\nAccept and draw traverse?") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Redraw);

            try
            {
                // Lock ACAD document and start transaction as we are running from Palette.
                using (Transaction tr = AutoCADApplicationManager.StartLockedTransaction())
                {
                    // Draw Transient Graphics of Traverse.
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
                                case Keywords.Redraw: //if redraw update the coordinates clear transients and redraw
                                    TransientGraphics.ClearTransientGraphics();
                                    coordinates = MathHelpers.AngleAndDistanceToCoordinates(TraverseAngles, point.Value);
                                    TransientGraphics.DrawTransientTraverse(coordinates);
                                    break;
                                case Keywords.Accept:
                                    Lines.DrawLines(tr, coordinates);
                                    cancelled = true;
                                    break;
                                case Keywords.Cancel:
                                    cancelled = true;
                                    break;
                            }
                        }
                    } while (prResult.Status != PromptStatus.Cancel && prResult.Status != PromptStatus.Error && !cancelled);

                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //TODO: Handle exception better at later date.
                throw;
            }
            finally
            {
                TransientGraphics.ClearTransientGraphics();
                _commandRunning = false;
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