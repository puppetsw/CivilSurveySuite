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
// System References
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

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
            using (Transaction tr = Acaddoc.TransactionManager.StartLockedTransaction())
            {
                Dictionary<string, DescriptionKeyMatch> desMapping = new Dictionary<string, DescriptionKeyMatch>();

                foreach (ObjectId pointId in Civildoc.CogoPoints)
                {
                    //BUG: Seems like there could be a problem with the keys and stuff if they don't match up
                    //TODO: add way to check for special codes e.g. SL or RECT
                    CogoPoint cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;
                    /* in each descriptionkeymatch add a collection of joinable points seperated by line number.
                       using dictionaries with the key being the raw description and then the line number.?
                     */
                    foreach (DescriptionKey descriptionKey in DescriptionKeys)
                    {
                        if (DescriptionKeyMatch.IsMatch(cogoPoint.RawDescription, descriptionKey))
                        {
                            string description = DescriptionKeyMatch.Description(cogoPoint.RawDescription, descriptionKey);
                            string lineNumber = DescriptionKeyMatch.LineNumber(cogoPoint.RawDescription, descriptionKey);

                            DescriptionKeyMatch deskeyMatch = null;
                            if (desMapping.ContainsKey(description))
                                deskeyMatch = desMapping[description];
                            else
                            {
                                deskeyMatch = new DescriptionKeyMatch(descriptionKey);
                                desMapping.Add(description, deskeyMatch);
                            }

                            deskeyMatch.AddCogoPoint(cogoPoint, lineNumber);
                        }
                    }
                }

                BlockTable bt = (BlockTable)tr.GetObject(Acaddoc.Database.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                
                //TODO: add special code checks in here?
                foreach (KeyValuePair<string, DescriptionKeyMatch> desKey in desMapping)
                {
                    DescriptionKeyMatch deskeyMatch = desKey.Value;

                    foreach (KeyValuePair<string, List<CogoPoint>> joinablePoints in deskeyMatch.JoinablePoints)
                    {
                        Point3dCollection points = new Point3dCollection();
                        foreach (CogoPoint point in joinablePoints.Value)
                            points.Add(point.Location);

                        string layerName = deskeyMatch.DescriptionKey.Layer;

                        //Check if the layer exists, if not create it.
                        if (!HasLayer(layerName, tr))
                            CreateLayer(layerName, tr);

                        if (deskeyMatch.DescriptionKey.Draw2D)
                            Draw2DPolyline(tr, btr, points, layerName);
                        if (deskeyMatch.DescriptionKey.Draw3D)
                            Draw3DPolyline(tr, btr, points, layerName);
                    }
                }
                tr.Commit();
            }
        }

        #endregion

        #region Private Methods

        private static void Draw3DPolyline(Transaction tr, BlockTableRecord btr, Point3dCollection points, string layerName)
        {
            Polyline3d pline3d = new Polyline3d(Poly3dType.SimplePoly, points, false);
            pline3d.Layer = layerName;
            btr.AppendEntity(pline3d);
            tr.AddNewlyCreatedDBObject(pline3d, true);
        }

        private static void Draw2DPolyline(Transaction tr, BlockTableRecord btr, Point3dCollection points, string layerName)
        {
            Polyline2d pline2d = new Polyline2d(Poly2dType.SimplePoly, points, 0, false, 0, 0, null);
            Polyline pline = new Polyline();
            pline.ConvertFrom(pline2d, false);
            pline.Layer = layerName;
            btr.AppendEntity(pline);
            tr.AddNewlyCreatedDBObject(pline, true);
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
