﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.CIVIL
{
    public class FeatureLineSiteCommand : IAcadCommand
    {
        public void Execute()
        {
            if (!EditorUtils.TryGetEntityOfType<FeatureLine>("", "", out var featureLineId, true))
            {
                AcadApp.Editor.WriteMessage("\nPlease select a Feature Line.");
                return;
            }

            using (var tr = AcadApp.StartTransaction())
            {
                var featureLine = (FeatureLine)tr.GetObject(featureLineId, OpenMode.ForRead);
                var siteId = featureLine.SiteId;
                var site = (Site)tr.GetObject(siteId, OpenMode.ForRead);
                var styleId = featureLine.StyleId;
                var style = (FeatureLineStyle)tr.GetObject(styleId, OpenMode.ForRead);

                AcadApp.Editor.WriteMessage($"\nSiteId: {siteId}, SiteName: {site.Name}, StyleName: {style.Name}");

                tr.Commit();
            }
        }
    }
}
