// Copyright Scott Whitney. All Rights Reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;
using _3DS_CivilSurveySuite_C3DBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// ViewModel for ConnectLineworkView.xaml
    /// </summary>
    public class ConnectLineworkViewModel : ViewModelBase
    {
        private ObservableCollection<DescriptionKey> _descriptionKeys;

        public ObservableCollection<DescriptionKey> DescriptionKeys
        {
            get => _descriptionKeys;
            set
            {
                _descriptionKeys = value;
                NotifyPropertyChanged();
            }
        }

        public DescriptionKey SelectedKey { get; set; }

        public RelayCommand AddRowCommand => new RelayCommand((_) => AddRow(), (_) => true);
        public RelayCommand RemoveRowCommand => new RelayCommand((_) => RemoveRow(), (_) => true);
        public RelayCommand ConnectCommand => new RelayCommand((_) => ConnectLinework(), (_) => true);

        public ConnectLineworkViewModel()
        {
            LoadSettings();
        }

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
            using (Transaction tr = AutoCADApplicationManager.ActiveDocument.TransactionManager.StartLockedTransaction())
            {
                Dictionary<string, DescriptionKeyMatch> desMapping = new Dictionary<string, DescriptionKeyMatch>();

                foreach (ObjectId pointId in CivilApplicationManager.ActiveCivilDocument.CogoPoints)
                {
                    //BUG: Seems like there could be a problem with the keys and stuff if they don't match up
                    //TODO: add way to check for special codes e.g. SL or RECT
                    CogoPoint cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;
                    /* in each DescriptionKeyMatch add a collection of join-able points separated by line number.
                       using dictionaries with the key being the raw description and then the line number.?
                     */

                    if (cogoPoint == null)
                    {
                        continue;
                    }

                    foreach (DescriptionKey descriptionKey in DescriptionKeys)
                    {
                        if (DescriptionKeyMatch.IsMatch(cogoPoint.RawDescription, descriptionKey))
                        {
                            string description = DescriptionKeyMatch.Description(cogoPoint.RawDescription, descriptionKey);
                            string lineNumber = DescriptionKeyMatch.LineNumber(cogoPoint.RawDescription, descriptionKey);

                            DescriptionKeyMatch deskeyMatch;
                            if (desMapping.ContainsKey(description))
                            {
                                deskeyMatch = desMapping[description];
                            }
                            else
                            {
                                deskeyMatch = new DescriptionKeyMatch(descriptionKey);
                                desMapping.Add(description, deskeyMatch);
                            }

                            deskeyMatch.AddCogoPoint(cogoPoint, lineNumber);
                        }
                    }
                }

                BlockTable bt = (BlockTable) tr.GetObject(AutoCADApplicationManager.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr =
                    (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                //TODO: add special code checks in here?
                foreach (KeyValuePair<string, DescriptionKeyMatch> desKey in desMapping)
                {
                    DescriptionKeyMatch deskeyMatch = desKey.Value;

                    foreach (KeyValuePair<string, List<CogoPoint>> joinablePoints in deskeyMatch.JoinablePoints)
                    {
                        Point3dCollection points = new Point3dCollection();
                        foreach (CogoPoint point in joinablePoints.Value)
                        {
                            points.Add(point.Location);
                        }

                        string layerName = deskeyMatch.DescriptionKey.Layer;

                        //Check if the layer exists, if not create it.
                        if (!Layers.HasLayer(layerName, tr))
                        {
                            Layers.CreateLayer(layerName, tr);
                        }

                        if (deskeyMatch.DescriptionKey.Draw2D)
                        {
                            Draw2DPolyline(tr, btr, points, layerName);
                        }

                        if (deskeyMatch.DescriptionKey.Draw3D)
                        {
                            Draw3DPolyline(tr, btr, points, layerName);
                        }
                    }
                }

                tr.Commit();
            }
        }

        private static void Draw3DPolyline(Transaction tr, BlockTableRecord btr, Point3dCollection points, string layerName)
        {
            var pLine3d = new Polyline3d(Poly3dType.SimplePoly, points, false);
            pLine3d.Layer = layerName;
            btr.AppendEntity(pLine3d);
            tr.AddNewlyCreatedDBObject(pLine3d, true);
        }

        private static void Draw2DPolyline(Transaction tr, BlockTableRecord btr, Point3dCollection points, string layerName)
        {
            var pLine2d = new Polyline2d(Poly2dType.SimplePoly, points, 0, false, 0, 0, null);
            var pLine = new Polyline();
            pLine.ConvertFrom(pLine2d, false);
            pLine.Layer = layerName;
            btr.AppendEntity(pLine);
            tr.AddNewlyCreatedDBObject(pLine, true);
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
    }
}