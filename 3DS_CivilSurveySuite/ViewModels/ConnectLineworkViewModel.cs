// 3DS_CivilSurveySuite References
using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Helpers.AutoCAD;
using _3DS_CivilSurveySuite.Helpers.Wpf;
using _3DS_CivilSurveySuite.Models;
// AutoCAD References
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
// Civil3D References
using Autodesk.Civil.DatabaseServices;
using System;
// System References
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// ViewModel for ConnectLineworkView.xaml
    /// </summary>
    public class ConnectLineworkViewModel : ViewModelBase
    {
        #region Private Members
        private ObservableCollection<DescriptionKey> descriptionKeys;


        #endregion

        #region Properties

        public ObservableCollection<DescriptionKey> DescriptionKeys { get => descriptionKeys; set { descriptionKeys = value; NotifyPropertyChanged(); } }

        public DescriptionKey SelectedKey { get; set; }

        #endregion

        #region Constructor

        public ConnectLineworkViewModel()
        {
            LoadSettings();
        }

        #endregion

        #region Commands

        public RelayCommand AddRowCommand => new RelayCommand((_) => AddRow(), (_) => true);
        public RelayCommand RemoveRowCommand => new RelayCommand((_) => RemoveRow(), (_) => true);
        public RelayCommand ConnectCommand => new RelayCommand((_) => ConnectLinework(), (_) => true);

        #endregion

        #region Command Methods

        private void AddRow()
        {
            DescriptionKeys.Add(new DescriptionKey());
        }

        private void RemoveRow()
        {
            if (SelectedKey != null)
                DescriptionKeys.Remove(SelectedKey);
        }

        private void ConnectLinework()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            using (Transaction tr = doc.TransactionManager.StartLockedTransaction())
            {
                var drawingCogoPoints = new List<CogoPoint>();

                foreach (ObjectId pointId in Civildoc.CogoPoints)
                {
                    CogoPoint cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;
                    drawingCogoPoints.Add(cogoPoint);
                }

                //TODO: Clean-up, seems a bit hacky.
                //TODO: Move point3dcollection into own class with layer and line number
                List<CogoPoint> sortedCogoPoints = drawingCogoPoints.OrderBy(x => x.RawDescription).ThenBy(x => x.PointNumber).ToList();

                List<List<DescriptionKeyMatch>> joinableCogoPoints = new List<List<DescriptionKeyMatch>>();

                foreach (DescriptionKey deskey in DescriptionKeys)
                {
                    var matchCogoPoints = new List<DescriptionKeyMatch>();
                    var removeCogoPoints = new List<CogoPoint>();

                    foreach (CogoPoint point in sortedCogoPoints)
                    {
                        //create regex matching pattern
                        // \A anchor start of string, \d digit, \d? optional digital x2, .* wildcard
                        string pattern = "\\A" + deskey.Key.Replace("#", "(\\d\\d?\\d?)").Replace("*", ".*");
                        Match match = Regex.Match(point.RawDescription, pattern);

                        if (match.Success)
                        {
                            int lineNumber = Convert.ToInt32(match.Groups[1].Value);
                            var desKeyMatch = new DescriptionKeyMatch() { LineNumber = lineNumber, CogoPoint = point, DescriptionKey = deskey };

                            matchCogoPoints.Add(desKeyMatch);
                            //if match remove from sortedList
                            //HACK: add point to a new list so we can remove later.
                            removeCogoPoints.Add(point);
                        }
                    }

                    if (matchCogoPoints.Count != 0)
                        joinableCogoPoints.Add(matchCogoPoints);

                    foreach (CogoPoint point in removeCogoPoints)
                        sortedCogoPoints.Remove(point);
                }

                BlockTable bt = (BlockTable)tr.GetObject(Acaddoc.Database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                //Process linework
                foreach (List<DescriptionKeyMatch> matchList in joinableCogoPoints)
                {
                    List<Point3dCollection> pointCollection = new List<Point3dCollection>();
                    List<Point3d> points = new List<Point3d>();

                    for (int i = 0; i < matchList.Count; i++)
                    {
                        points.Add(matchList[i].CogoPoint.Location);

                        if (i == matchList.Count - 1)
                        {
                            var ptCol = new Point3dCollection(points.ToArray());
                            pointCollection.Add(ptCol);
                            break;
                        }

                        if (matchList[i].LineNumber != matchList[i+1].LineNumber)
                        {
                            var ptCol = new Point3dCollection(points.ToArray());
                            pointCollection.Add(ptCol);
                            points = new List<Point3d>();
                        }
                    }

                    foreach (Point3dCollection ptCol in pointCollection)
                    {
                        Polyline3d polyline = new Polyline3d(Poly3dType.SimplePoly, ptCol, false);
                        btr.AppendEntity(polyline);
                        tr.AddNewlyCreatedDBObject(polyline, true);
                    }
                }
                tr.Commit();
            }
        }

        #endregion

        #region Private Methods

        private void Draw2DPolyline()
        { }

        private void Draw3DPolyline(Point3dCollection pointCollection)
        {

        }

        /// <summary>
        /// Get the last xml file loaded from settings
        /// </summary>
        private void LoadSettings()
        {
            string fileName = Properties.Settings.Default.ConnectLineworkFileName;

            if (File.Exists(fileName))
            {
                Load(fileName);
            }
            else
            {
                DescriptionKeys = new ObservableCollection<DescriptionKey>();
            }
        }

        /// <summary>
        /// Load XML file
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            DescriptionKeys = XmlHelper.ReadFromXmlFile<ObservableCollection<DescriptionKey>>(fileName);
        }

        /// <summary>
        /// Save XML file
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            XmlHelper.WriteToXmlFile(fileName, DescriptionKeys);
            Properties.Settings.Default.ConnectLineworkFileName = fileName;
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}
