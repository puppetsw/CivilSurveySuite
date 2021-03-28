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
                //HACK: this is so bad, need to fix.
                //BUG: Doesn't add single line number codes eg. all TK1, or leaves off FP7 in example.
                List<CogoPoint> sortedCogoPoints = drawingCogoPoints.OrderBy(x => x.RawDescription).ThenBy(x => x.PointNumber).ToList();
                List<DescriptionKeyMatch> matchKeys = new List<DescriptionKeyMatch>();
                Dictionary<string, DescriptionKeyMatch> mapKeys = new Dictionary<string, DescriptionKeyMatch>();

                /*
                 * in each descriptionkeymatch add a collection of joinable points seperated by line number.
                 * using dictionaries with the key being the raw description and then the line number.?
                */
                foreach (CogoPoint cogoPoint in drawingCogoPoints)
                {
                    foreach (DescriptionKey descriptionKey in DescriptionKeys)
                    {
                        string pattern = "^(" + descriptionKey.Key.Replace("#", ")(\\d\\d?\\d?)").Replace("*", ".*?");
                        Match regMatch = Regex.Match(cogoPoint.RawDescription, pattern);

                        if (regMatch.Success)
                        {
                            string currentDescription = regMatch.Groups[1].Value;
                            string currentLineNumber = regMatch.Groups[2].Value;

                            DescriptionKeyMatch match = null;
                            bool noMatch = false;

                            if (!mapKeys.ContainsKey(currentDescription))
                            {
                                match = new DescriptionKeyMatch();
                                match.DescriptionKey = descriptionKey;
                                noMatch = true;
                            }
                            else
                                match = mapKeys[currentDescription];

                            if (match.MatchCollection.ContainsKey(currentLineNumber))
                                match.MatchCollection[currentLineNumber].Add(cogoPoint);
                            else
                            {
                                List<CogoPoint> cogoPoints = new List<CogoPoint>();
                                cogoPoints.Add(cogoPoint);
                                match.MatchCollection.Add(currentLineNumber, cogoPoints);
                            }

                            if (noMatch)
                                mapKeys.Add(currentDescription, match);

                            break;
                        }
                    }


                }


                //foreach (DescriptionKey deskey in DescriptionKeys)
                //{
                //    DescriptionKeyMatch match = new DescriptionKeyMatch();
                //    match.DescriptionKey = deskey;

                //    string pattern = "^(" + deskey.Key.Replace("#", ")(\\d\\d?\\d?)").Replace("*", ".*?");
                //    Match regexMatch = null; //initalise regexmatch

                //    string nextDescription = string.Empty;
                //    string nextLineNumber = string.Empty;

                //    for (int i = 0; i < sortedCogoPoints.Count; i++)
                //    {
                //        CogoPoint point = sortedCogoPoints[i];
                //        CogoPoint nextPoint = null;

                //        if (i + 1 < sortedCogoPoints.Count)
                //            nextPoint = sortedCogoPoints[i + 1];

                //        //create regex matching pattern
                //        // \A anchor start of string, \d digit, \d? optional digital x2, .* wildcard
                //        regexMatch = Regex.Match(point.RawDescription, pattern);

                //        if (regexMatch.Success)
                //        {
                //            string currentDescription = regexMatch.Groups[1].Value;
                //            string currentLineNumber = regexMatch.Groups[2].Value;

                //            if (match.DescriptionKey == null)
                //                match.DescriptionKey = deskey;

                //            if (match.LineNumber == string.Empty)
                //                match.LineNumber = currentLineNumber;
                              
                //            //succesful, add point
                //            match.CogoPoints.Add(point);

                //            if (nextPoint != null)
                //            {
                //                string patternNext = "^(\\w\\w\\w?)(\\d\\d?\\d?).*?";
                //                Match regexMatchNext = Regex.Match(nextPoint.RawDescription, patternNext);
                //                if (regexMatch.Success)
                //                {
                //                    nextDescription = regexMatchNext.Groups[1].Value;
                //                    nextLineNumber = regexMatchNext.Groups[2].Value;

                //                    if (nextLineNumber != currentLineNumber || nextDescription != currentDescription)
                //                    {
                //                        matchKeys.Add(match);
                //                        match = new DescriptionKeyMatch();
                //                    }
                //                }
                //            }

                //            if (i + 1 == sortedCogoPoints.Count)
                //                matchKeys.Add(match);
                //        }
                //    }
                //}

                BlockTable bt = (BlockTable)tr.GetObject(Acaddoc.Database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (KeyValuePair<string, DescriptionKeyMatch> keyValue in mapKeys)
                {
                    foreach (KeyValuePair<string, List<CogoPoint>> cogoValue in keyValue.Value.MatchCollection)
                    {
                        Point3dCollection points = new Point3dCollection();
                        foreach (CogoPoint point in cogoValue.Value)
                            points.Add(point.Location);

                        if (keyValue.Value.DescriptionKey.Draw2D)
                        {
                            Polyline2d pline2d = new Polyline2d(Poly2dType.SimplePoly, points, 0, false, 0, 0, null);
                            pline2d.Layer = keyValue.Value.DescriptionKey.Layer;
                            btr.AppendEntity(pline2d);
                            tr.AddNewlyCreatedDBObject(pline2d, true);
                        }
                        //BUG: if layer doesn't exist throws error
                        if (keyValue.Value.DescriptionKey.Draw3D)
                        {
                            Polyline3d pline3d = new Polyline3d(Poly3dType.SimplePoly, points, false);
                            pline3d.Layer = keyValue.Value.DescriptionKey.Layer;
                            btr.AppendEntity(pline3d);
                            tr.AddNewlyCreatedDBObject(pline3d, true);
                        }
                    }
                }

                //foreach (DescriptionKeyMatch key in matchKeys)
                //{
                //    Point3dCollection pointCollection = new Point3dCollection();
                //    foreach (CogoPoint point in key.CogoPoints)
                //        pointCollection.Add(point.Location);

                //    //BUG: if layer doesn't exist throws error
                //    if (key.DescriptionKey.Draw2D)
                //    {
                //        Polyline2d pline2d = new Polyline2d(Poly2dType.SimplePoly, pointCollection, 0, false, 0, 0, null);
                //        pline2d.Layer = key.DescriptionKey.Layer;
                //        btr.AppendEntity(pline2d);
                //        tr.AddNewlyCreatedDBObject(pline2d, true);
                //    }
                //    //BUG: if layer doesn't exist throws error
                //    if (key.DescriptionKey.Draw3D)
                //    {
                //        Polyline3d pline3d = new Polyline3d(Poly3dType.SimplePoly, pointCollection, false);
                //        pline3d.Layer = key.DescriptionKey.Layer;
                //        btr.AppendEntity(pline3d);
                //        tr.AddNewlyCreatedDBObject(pline3d, true);
                //    }
                //}

                tr.Commit();
            }
        }

        #endregion

        #region Private Methods

        private void Draw2DPolyline()
        { }

        private void Draw3DPolyline()
        { }

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
