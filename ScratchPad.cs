﻿


[CommandMethod("HighlightPolySeg")]
public void SelectNestedPolyline()
{
    Document doc = Application.DocumentManager.MdiActiveDocument;
    Database db = doc.Database;
    Editor ed = doc.Editor;

    // Prompt the user to select a polyline segment.
    PromptNestedEntityOptions options = new PromptNestedEntityOptions("\nSelect a polyline segment: ");
    options.AllowNone = false;
    PromptNestedEntityResult result = ed.GetNestedEntity(options);
    if (result.Status != PromptStatus.OK)
        return;

    // If the selected entity is a polyline.
    if (result.ObjectId.ObjectClass.Name == "AcDbPolyline")
    {
        // Start a transaction to open the selected polyline.
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            // Transform the picked point from current UCS to WCS.
            Point3d wcsPickedPoint = result.PickedPoint.TransformBy(ed.CurrentUserCoordinateSystem);

            // Open the polyline.
            Polyline pline = (Polyline) tr.GetObject(result.ObjectId, OpenMode.ForRead);

            // Get the closest point to picked point on the polyline.
            // If the polyline is nested, it's needed to transform the picked point using the 
            // the transformation matrix that is applied to the polyline by its containers.
            Point3d pointOnPline = result.GetContainers().Length == 0
                ? pline.GetClosestPointTo(wcsPickedPoint, false)
                : // not nested polyline
                pline.GetClosestPointTo(wcsPickedPoint.TransformBy(result.Transform.Inverse()), false); // nested polyline

            // Get the selected segment index.
            int segmentIndex = (int) pline.GetParameterAtPoint(pointOnPline);
            ed.WriteMessage("\nSelected segment index: {0}", segmentIndex);
            tr.Commit();
        }
    }
}



[CommandMethod("AddRectangle")]
public void AddRectangle()
{
    Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
    Editor ed = doc.Editor;
    Database db = HostApplicationServices.WorkingDatabase;
    Matrix3d ucs = ed.CurrentUserCoordinateSystem;
    CoordinateSystem3d Cs = ucs.CoordinateSystem3d;
    Plane pn = new Plane(Point3d.Origin, Cs.Zaxis);
    Vector3d normal = Cs.Zaxis;

    //First point
    PromptPointResult Pres = ed.GetPoint("\nPick the first point:");
    if (Pres.Status != PromptStatus.OK)
        return;

    Point3d U1 = Pres.Value;

    //Second point
    PromptPointOptions PPO = new PromptPointOptions("\nPick the second point:");
    PPO.UseBasePoint = true;
    PPO.BasePoint = U1;
    Pres = ed.GetPoint(PPO);

    if (Pres.Status != PromptStatus.OK)
        return;

    Point3d U2 = Pres.Value;

    if (U1.Z != U2.Z)
        U2 = new Point3d(U2.X, U2.Y, U1.Z);

    //Add pline
    Polyline oPline = new Polyline(7);
    oPline.Normal = Cs.Zaxis;
    oPline.Elevation = -new Plane(Cs.Origin, Cs.Zaxis).Coefficients.D;
    if (U1.Z != 0) oPline.Elevation += U1.Z;

    Point2d P1 = U1.TransformBy(ucs).Convert2d(pn);
    Point2d P2 = U2.TransformBy(ucs).Convert2d(pn);
    oPline.AddVertexAt(0, P1, 0, 0, 0);
    oPline.AddVertexAt(1, P2, 0, 0, 0);
    Vector2d v = (P2 - P1).GetNormal().RotateBy(Math.PI * 0.5);

    using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
    {
        BlockTableRecord Cspace = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
        Cspace.AppendEntity(oPline);
        tr.AddNewlyCreatedDBObject(oPline, true);
        tr.Commit();
    }

    double CurOffset = (double)Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("OFFSETDIST");

    if (CurOffset < 0)
        CurOffset *= -1;

    PromptDistanceOptions PD = new PromptDistanceOptions("\nSpecify offset distance: ");

    PD.DefaultValue = CurOffset;

    double dOffset = ed.GetDistance(PD).Value;

    if (dOffset < 0)
        dOffset *= -1;

    if (dOffset != CurOffset)
        ed.WriteMessage("\nOffset distance=" + dOffset);

    Retry:
    Pres = ed.GetPoint("\nSpecify point on side to offset:");

    if (Pres.Status != PromptStatus.OK)
        return;

    int iOffset = MathHelpers.IsLeft(P1.ToPoint(), P2.ToPoint(), Pres.Value.TransformBy(ucs).Convert2d(pn).ToPoint());

    if (iOffset == 0)  //means all points are linear
    {
        //MessageBox.Show("The offset point must not be on the line:");
        AutoCADActive.Editor.WriteMessage("The offset point must not be on the line:");
        goto Retry;
    }

    v = v * iOffset * dOffset;
    using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
    {
        oPline = tr.GetObject(oPline.Id, OpenMode.ForWrite) as Polyline;
        oPline.AddVertexAt(2, P2.Add(v), 0, 0, 0);
        oPline.AddVertexAt(3, P1.Add(v), 0, 0, 0);
        oPline.Closed = true;
        tr.Commit();
    }

    Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("OFFSETDIST", Math.Abs(dOffset));


} //End AddRectangle



public static void ZoomTo()
    {
      SupExtEditor.ActEdt().SetCommandHelp();
      using (Transaction transaction = SupExtDatabase.ActDbs().TransactionManager.StartTransaction())
      {
        SupExtEditor.ActEdt().EchoString("DS> Collecting Points ... ", false);
        System.Collections.Generic.List<SurveyPoint> all = Points.GetAll();
        SupExtEditor.ActEdt().EchoString("DS> Collecting Points ... Done.");
        if (all.Count > 0)
        {
          bool flag = false;
          while (!flag)
          {
            PromptStringOptions promptStringOptions = new PromptStringOptions("\r\nDS> Point ID: ");
            PromptResult promptResult = SupExtEditor.ActEdt().GetString(promptStringOptions);
            if (promptResult.Status == 5100)
            {
              if (promptResult.StringResult.Length > 0)
              {
                if (Operators.CompareString(promptResult.StringResult, "*", false) == 0)
                {
                  Extents3d extents3d;
                  // ISSUE: explicit constructor call
                  ((Extents3d) ref extents3d).\u002Ector();
                  try
                  {
                    foreach (SurveyPoint surveyPoint in all)
                      ((Extents3d) ref extents3d).AddPoint(surveyPoint.Location);
                  }
                  finally
                  {
                    System.Collections.Generic.List<SurveyPoint>.Enumerator enumerator;
                    enumerator.Dispose();
                  }
                  SupExtEditor.ActEdt().ZoomWindow(((Extents3d) ref extents3d).MinPoint, ((Extents3d) ref extents3d).MaxPoint, 0.05f);
                }
                else
                {
                  SurveyPoint surveyPoint1 = new SurveyPoint();
                  try
                  {
                    foreach (SurveyPoint surveyPoint2 in all)
                    {
                      if (surveyPoint2.Point.Equals(promptResult.StringResult))
                      {
                        surveyPoint1 = surveyPoint2;
                        break;
                      }
                    }
                  }
                  finally
                  {
                    System.Collections.Generic.List<SurveyPoint>.Enumerator enumerator;
                    enumerator.Dispose();
                  }
                  if (surveyPoint1.IsValid)
                  {
                    SupExtEditor.ActEdt().ZoomCenter(surveyPoint1.Location, Conversions.ToDouble(Application.GetSystemVariable("VIEWSIZE")));
                    SupExtEditor.ActEdt().EchoString(string.Format("Point {0}  N:{1}  E:{2}  Z:{3}  D:{4}", (object) surveyPoint1.Point, (object) surveyPoint1.Northing, (object) surveyPoint1.Easting, (object) surveyPoint1.Elevation, (object) surveyPoint1.RawDescription));
                  }
                  else
                    SupExtEditor.ActEdt().EchoString("DS> Point (" + promptResult.StringResult + ") Not Found !!!");
                }
              }
              else
                flag = true;
            }
            else
              flag = true;
          }
        }
        else
          SupExtEditor.ActEdt().EchoString("DS> No Points Found in Drawing !!!");
        transaction.Commit();
      }
    }

 public static void ZoomObjects()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Transaction tr = doc.TransactionManager.StartTransaction();

            using (tr)
            {
                try
                {
                    PromptSelectionResult sres = ed.GetSelection();

                    if (sres.Status != PromptStatus.OK) return;

                    ObjectId[] ids = sres.Value.GetObjectIds();

                    if (ids.Length == 0) return;

                   int tile = Convert.ToInt32(Application.GetSystemVariable("CVPORT"));
                    
                    var ents = from id in ids
                               where id != null
                               select tr.GetObject(id, OpenMode.ForRead);

                    var minx = (from n in ents
                                 where n != null
                                 select ((Entity)n).GeometricExtents.MinPoint[0]).Min();

                    var maxx = (from n in ents
                                where n != null
                                select ((Entity)n).GeometricExtents.MinPoint[0]).Max();

                    var miny = (from n in ents
                                where n != null
                                select ((Entity)n).GeometricExtents.MinPoint[1]).Min();
                    var maxy = (from n in ents
                                where n != null
                                select ((Entity)n).GeometricExtents.MinPoint[1]).Max();


                    Autodesk.AutoCAD.GraphicsSystem.Manager graph = doc.GraphicsManager;

                    using (graph)
                    {
                        Autodesk.AutoCAD.GraphicsSystem.View view = graph.GetGsView(tile, true);

                    using (view)
                    {                        
                        view.ZoomExtents(new Point3d(minx,miny,0),new Point3d(maxx,maxy,0));
                        view.Zoom(0.8);//<--optional 
                        graph.SetViewportFromView(tile, view, true, true, false);
                       
                    }                   
                }
                  tr.Commit();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    ed.WriteMessage("Error: {0}\nTrace: {1}",ex.Message,ex.StackTrace);
                }
            }
        }



public static int PntIntDirDir(
    Point3d BasPt1,
    Point3d PrjPt1,
    Point3d BasPt2,
    Point3d PrjPt2,
    ref Point3d RetPnt)
{
    int num1 = 0;
    double x1 = ((Point3d) ref BasPt1 ).X;
    double y1 = ((Point3d) ref BasPt1 ).Y;
    double x2 = ((Point3d) ref PrjPt1 ).X;
    double y2 = ((Point3d) ref PrjPt1 ).Y;
    double x3 = ((Point3d) ref BasPt2 ).X;
    double y3 = ((Point3d) ref BasPt2 ).Y;
    double x4 = ((Point3d) ref PrjPt2 ).X;
    double y4 = ((Point3d) ref PrjPt2 ).Y;
    double num2 = (y1 - y3) * (x4 - x3) - (x1 - x3) * (y4 - y3);
    double num3 = (y1 - y3) * (x2 - x1) - (x1 - x3) * (y2 - y1);
    double num4 = (x2 - x1) * (y4 - y3) - (y2 - y1) * (x4 - x3);
    if (num4 != 0.0)
    {
        double num5 = num2 / num4;
        double num6 = num3 / num4;
        double num7 = x1 + num5 * (x2 - x1);
        double num8 = y1 + num5 * (y2 - y1);
        // ISSUE: explicit constructor call
        //((Point3d) ref RetPnt).\u002Ector(num7, num8, 0.0);
        var RetPnt = new Point3d(num7, num8, 0.0);

        if (num5 > 0.0 && num5 < 1.0 && num6 > 0.0 && num6 < 1.0)
            num1 = 1;
        else if (num5 < 0.0 || num5 > 1.0 || num6 < 0.0 || num6 > 1.0)
            num1 = 2;
        else if (((Point3d) ref RetPnt).X == ((Point3d) ref BasPt2).X && ((Point3d) ref RetPnt).Y == ((Point3d) ref BasPt2).Y)
            num1 = 3;
        else if (((Point3d) ref RetPnt).X == ((Point3d) ref PrjPt2).X && ((Point3d) ref RetPnt).Y == ((Point3d) ref PrjPt2).Y)
            num1 = 4;
    }

    return num1;
}


public static int PntIntDirDis(
    Point3d LinPt1,
    Point3d LinPt2,
    Point3d CenPnt,
    double Radius,
    ref Point3d SolPt1,
    ref Point3d SolPt2)
{
    double num1 = ((Point3d) ref LinPt2).X - ((Point3d) ref LinPt1).X;
    double num2 = ((Point3d) ref LinPt2).Y - ((Point3d) ref LinPt1).Y;
    double num3 = num1 * num1 + num2 * num2;
    double num4 = 2.0 * (num1 * (((Point3d) ref LinPt1).X - ((Point3d) ref CenPnt).X) + num2 * (((Point3d) ref LinPt1).Y - ((Point3d) ref CenPnt).Y));
    double num5 = (((Point3d) ref LinPt1).X - ((Point3d) ref CenPnt).X) * (((Point3d) ref LinPt1).X - ((Point3d) ref CenPnt).X) + (((Point3d) ref LinPt1).Y - ((Point3d) ref CenPnt).Y) * (((Point3d) ref LinPt1).Y - ((Point3d) ref CenPnt).Y) - Radius * Radius;
    double d = num4 * num4 - 4.0 * num3 * num5;
    if (num3 <= 1E-07 | d < 0.0)
        return 0;
    if (d == 0.0)
    {
        double num6 = -num4 / (2.0 * num3);
        // ISSUE: explicit constructor call
        ((Point3d) ref SolPt1).\u002Ector(((Point3d) ref LinPt1).X + num6 * num1, ((Point3d) ref LinPt1).Y + num6 * num2, 0.0);
        // ISSUE: explicit constructor call
        ((Point3d) ref SolPt2).\u002Ector(0.0, 0.0, 0.0);
        return 1;
    }
    double num7 = (-num4 + Math.Sqrt(d)) / (2.0 * num3);
    // ISSUE: explicit constructor call
    ((Point3d) ref SolPt1).\u002Ector(((Point3d) ref LinPt1).X + num7 * num1, ((Point3d) ref LinPt1).Y + num7 * num2, 0.0);
    double num8 = (-num4 - Math.Sqrt(d)) / (2.0 * num3);
    // ISSUE: explicit constructor call
    ((Point3d) ref SolPt2).\u002Ector(((Point3d) ref LinPt1).X + num8 * num1, ((Point3d) ref LinPt1).Y + num8 * num2, 0.0);
    return 2;
}


public static Point2d GetPolarPoint(this Point2d ptBase, double angle, double distance)
{
    short num1 = 0;
    num1 = (short) 1;
    if (num1 == (short) 0)
        ;
    num1 = (short) 5534;
    int num2 = (int) num1;
    num1 = (short) 5534;
    int num3 = (int) num1;
    switch (num2 == num3)
    {
        case true:
            num1 = (short) 0;
            if (num1 == (short) 0)
                ;
            return new Point2d(((Point2d) ref ptBase).X + distance * Math.Cos(angle), ((Point2d) ref ptBase).Y + distance * Math.Sin(angle));
        default:
            goto case 1;
    }
}