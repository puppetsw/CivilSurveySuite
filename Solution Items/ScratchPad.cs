


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







public class PointListCreator : ICloneable
  {
    private SortedDictionary<int, int> a;
    private string b;
    private double c = double.PositiveInfinity;

    public double IncludeNumbersAbove
    {
      get
      {
            return this.c;

        }
      }
      set
      {

        SortedDictionary<int, int> sortedDictionary;
        switch (num2 == num3 ? 1 : 0)
        {
          case 0:
          case 2:
label_17:
            this.a = sortedDictionary;
            break;
          case 1:
            num4 = (short) 0;
            if (num4 == (short) 0)
              ;
            this.c = value;
            sortedDictionary = new SortedDictionary<int, int>();
            using (SortedDictionary<int, int>.KeyCollection.Enumerator enumerator = this.a.Keys.GetEnumerator())
            {
              num4 = (short) 4;
              int num5 = (int) num4;
              int current;
              while (true)
              {
                switch (num5)
                {
                  case 0:
                    goto label_17;
                  case 1:
                    sortedDictionary.Add(current, 0);
                    num4 = (short) 2;
                    num5 = (int) num4;
                    continue;
                  case 2:
                    num4 = (short) 6;
                    num5 = (int) num4;
                    continue;
                  case 3:
                    if ((double) current <= this.c)
                    {
                      num4 = (short) 1;
                      num5 = (int) num4;
                      continue;
                    }
                    goto case 2;
                  case 4:
                    switch (0)
                    {
                      case 0:
                        goto label_7;
                      default:
                        continue;
                    }
                  case 5:
                    num4 = (short) 0;
                    num5 = (int) num4;
                    continue;
                  case 6:
                    if (enumerator.MoveNext())
                    {
                      current = enumerator.Current;
                      num4 = (short) 3;
                      num5 = (int) num4;
                      continue;
                    }
                    num4 = (short) 5;
                    num5 = (int) num4;
                    continue;
                  default:
label_7:
                    num4 = (short) 1;
                    if (num4 == (short) 0)
                      goto case 2;
                    else
                      goto case 2;
                }
              }
            }
          default:
            num4 = (short) 0;
            goto case 1;
        }
      }
    }

    public PointListCreator()
      : this("No points used.")
    {
    }

    public PointListCreator(string stringForEmptyList)
    {
      this.b = stringForEmptyList;
      this.a = new SortedDictionary<int, int>();
    }

    public bool ContainsPoint(int number)
    {
      int num1;
      bool flag;
      short num2;
      switch (0)
      {
        case 0:
label_2:
          flag = this.a.ContainsKey(number);
          num2 = (short) 0;
          num1 = (int) num2;
          goto default;
        default:
          while (true)
          {
            switch (num1)
            {
              case 0:
label_3:
                if (!flag)
                {
                  num2 = (short) 2;
                  num1 = (int) num2;
                  continue;
                }
                goto label_9;
              case 1:
                goto label_9;
              case 2:
                flag = (double) number > this.IncludeNumbersAbove;
                num2 = (short) 0;
                num2 = (short) -9588;
                int num3 = (int) num2;
                num2 = (short) -9588;
                int num4 = (int) num2;
                switch (num3 == num4 ? 1 : 0)
                {
                  case 0:
                  case 2:
                    goto label_3;
                  default:
                    num2 = (short) 1;
                    if (num2 == (short) 0)
                      ;
                    num2 = (short) 0;
                    if (num2 == (short) 0)
                      ;
                    num2 = (short) 1;
                    num1 = (int) num2;
                    continue;
                }
              default:
                goto label_2;
            }
          }
label_9:
          return flag;
      }
    }

    public bool ContainsPoints
    {
      get
      {
        short num1;
        while (this.a.Count <= 0)
        {
          num1 = (short) 0;
          num1 = (short) 10890;
          int num2 = (int) num1;
          num1 = (short) 10890;
          int num3 = (int) num1;
          switch (num2 == num3 ? 1 : 0)
          {
            case 0:
            case 2:
              continue;
            default:
              num1 = (short) 0;
              if (num1 == (short) 0)
                ;
              return !double.IsPositiveInfinity(this.IncludeNumbersAbove);
          }
        }
        num1 = (short) 1;
        if (num1 == (short) 0)
          ;
        return true;
      }
    }

    public void Remove(int number)
    {
      int num1 = 6;
      while (true)
      {
        short num2;
        int number1;
        switch (num1)
        {
          case 0:
            if ((double) number <= this.IncludeNumbersAbove)
              goto label_18;
            else
              break;
          case 1:
          case 5:
            num2 = (short) 7;
            num1 = (int) num2;
            continue;
          case 2:
            num2 = (short) 1;
            if (num2 == (short) 0)
              ;
            num2 = (short) 0;
            double includeNumbersAbove = this.IncludeNumbersAbove;
            this.IncludeNumbersAbove = (double) number;
            number1 = (int) includeNumbersAbove + 1;
            num2 = (short) 1;
            num1 = (int) num2;
            continue;
          case 3:
            this.a.Remove(number);
            num2 = (short) 8;
            num1 = (int) num2;
            continue;
          case 4:
            goto label_13;
          case 6:
            switch (0)
            {
              case 0:
                goto label_3;
              default:
                continue;
            }
          case 7:
            if (number1 < number)
            {
              num2 = (short) -21618;
              int num3 = (int) num2;
              num2 = (short) -21618;
              int num4 = (int) num2;
              switch (num3 == num4 ? 1 : 0)
              {
                case 0:
                case 2:
                  break;
                default:
                  num2 = (short) 0;
                  if (num2 == (short) 0)
                    ;
                  this.Add(number1);
                  ++number1;
                  num2 = (short) 5;
                  num1 = (int) num2;
                  continue;
              }
            }
            else
            {
              num2 = (short) 4;
              num1 = (int) num2;
              continue;
            }
            break;
          case 8:
            num2 = (short) 0;
            num1 = (int) num2;
            continue;
          default:
label_3:
            if (this.a.ContainsKey(number))
            {
              num2 = (short) 3;
              num1 = (int) num2;
              continue;
            }
            goto case 8;
        }
        num2 = (short) 2;
        num1 = (int) num2;
      }
label_13:
      return;
label_18:;
    }

    public void Add(int number)
    {
      int num1 = 2;
      while (true)
      {
        short num2;
        switch (num1)
        {
          case 0:
            num2 = (short) 1;
            num1 = (int) num2;
            continue;
          case 1:
            if ((double) number > this.IncludeNumbersAbove)
              goto label_10;
            else
              break;
          case 2:
            switch (0)
            {
              case 0:
                goto label_3;
              default:
                continue;
            }
          case 3:
            this.a.Add(number, 0);
            num2 = (short) 4;
            num1 = (int) num2;
            continue;
          case 4:
            goto label_8;
          default:
label_3:
            num2 = (short) 1;
            if (num2 == (short) 0)
              ;
            num2 = (short) 0;
            num2 = (short) -6472;
            int num3 = (int) num2;
            num2 = (short) -6472;
            int num4 = (int) num2;
            switch (num3 == num4 ? 1 : 0)
            {
              case 0:
              case 2:
                break;
              default:
                num2 = (short) 0;
                if (num2 == (short) 0)
                  ;
                if (!this.a.ContainsKey(number))
                {
                  num2 = (short) 0;
                  num1 = (int) num2;
                  continue;
                }
                goto label_14;
            }
            break;
        }
        num2 = (short) 3;
        num1 = (int) num2;
      }
label_8:
      return;
label_14:
      return;
label_10:;
    }

    public void Add(int[] numberArray)
    {
      int num1;
      int[] numArray;
      int index;
      short num2;
      switch (0)
      {
        case 0:
label_2:
          numArray = numberArray;
          index = 0;
          num2 = (short) 1;
          num1 = (int) num2;
          goto default;
        default:
          while (true)
          {
            switch (num1)
            {
              case 0:
                if (index < numArray.Length)
                  goto label_7;
label_6:
                num2 = (short) 3;
                num1 = (int) num2;
                continue;
label_7:
                this.Add(numArray[index]);
                ++index;
                num2 = (short) 0;
                num2 = (short) -17499;
                int num3 = (int) num2;
                num2 = (short) -17499;
                int num4 = (int) num2;
                switch (num3 == num4 ? 1 : 0)
                {
                  case 0:
                  case 2:
                    goto label_6;
                  default:
                    num2 = (short) 0;
                    if (num2 == (short) 0)
                      ;
                    num2 = (short) 2;
                    num1 = (int) num2;
                    continue;
                }
              case 1:
                num2 = (short) 1;
                if (num2 == (short) 0)
                  goto case 2;
                else
                  goto case 2;
              case 2:
                num2 = (short) 0;
                num1 = (int) num2;
                continue;
              case 3:
                goto label_10;
              default:
                goto label_2;
            }
          }
label_10:
          break;
      }
    }

    public void Add(uint[] numberArray)
    {
      int num1;
      uint[] numArray;
      int index;
      short num2;
      switch (0)
      {
        case 0:
label_2:
          numArray = numberArray;
          index = 0;
          num2 = (short) 0;
          num2 = (short) 2;
          num1 = (int) num2;
          goto default;
        default:
          while (true)
          {
            switch (num1)
            {
              case 0:
                num2 = (short) 5787;
                int num3 = (int) num2;
                num2 = (short) 5787;
                int num4 = (int) num2;
                switch (num3 == num4 ? 1 : 0)
                {
                  case 0:
                  case 2:
                    goto label_3;
                  default:
                    goto label_10;
                }
              case 1:
              case 2:
label_3:
                num2 = (short) 3;
                num1 = (int) num2;
                continue;
              case 3:
                if (index >= numArray.Length)
                {
                  num2 = (short) 0;
                  num1 = (int) num2;
                  continue;
                }
                this.Add((int) numArray[index]);
                ++index;
                num2 = (short) 1;
                num1 = (int) num2;
                continue;
              default:
                goto label_2;
            }
          }
label_10:
          num2 = (short) 1;
          if (num2 == (short) 0)
            ;
          num2 = (short) 0;
          if (num2 == (short) 0)
            break;
          break;
      }
    }

    public void Add(string pointList)
    {
      int num1 = 0;
      switch (num1)
      {
        default:
          short num2;
          char[] separator1;
          string[] strArray1;
          int index;
          switch (0)
          {
            case 0:
label_3:
              char[] separator2 = new char[1]{ ',' };
              separator1 = new char[1]{ '-' };
              pointList = new Regex("[^0-9,<>+-]").Replace(pointList, "");
              strArray1 = pointList.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
              index = 0;
              num2 = (short) 1;
              num1 = (int) num2;
              goto default;
            default:
              while (true)
              {
                num2 = (short) 0;
                int num3;
                string str1;
                string[] strArray2;
                string str2;
                int int32_1;
                int int32_2;
                int num4;
                switch (num1)
                {
                  case 0:
                    if (index >= strArray1.Length)
                    {
                      num2 = (short) 7;
                      num1 = (int) num2;
                      continue;
                    }
                    str2 = strArray1[index];
                    num2 = (short) 19;
                    num1 = (int) num2;
                    continue;
                  case 1:
                  case 4:
                    num2 = (short) 0;
                    num1 = (int) num2;
                    continue;
                  case 2:
                    num2 = (short) 1;
                    if (num2 == (short) 0)
                      ;
                    this.IncludeNumbersAbove = (double) num4;
                    break;
                  case 3:
                    if (str2.StartsWith("<"))
                    {
                      num2 = (short) 22;
                      num1 = (int) num2;
                      continue;
                    }
                    str1 = str2;
                    num2 = (short) 8;
                    num1 = (int) num2;
                    continue;
                  case 5:
                    if (str2.EndsWith("+"))
                    {
                      num2 = (short) 16;
                      num1 = (int) num2;
                      continue;
                    }
                    num2 = (short) 3;
                    num1 = (int) num2;
                    continue;
                  case 6:
                    if (strArray2.GetLength(0) > 1)
                    {
                      num2 = (short) 10;
                      num1 = (int) num2;
                      continue;
                    }
                    this.Add(Convert.ToInt32(strArray2[0]));
                    num2 = (short) 15;
                    num1 = (int) num2;
                    continue;
                  case 7:
                    goto label_41;
                  case 8:
                  case 9:
                    strArray2 = str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                    num2 = (short) 6;
                    num1 = (int) num2;
                    continue;
                  case 10:
                    int int32_3 = Convert.ToInt32(strArray2[0]);
                    int32_2 = Convert.ToInt32(strArray2[1]);
                    num3 = int32_3;
                    num2 = (short) 18;
                    num1 = (int) num2;
                    continue;
                  case 11:
                  case 12:
                  case 13:
                  case 15:
                    ++index;
                    num2 = (short) 4;
                    num1 = (int) num2;
                    continue;
                  case 14:
                    if ((double) int32_1 < this.IncludeNumbersAbove)
                    {
                      num2 = (short) 20;
                      num1 = (int) num2;
                      continue;
                    }
                    goto case 11;
                  case 16:
                    str1 = str2.Substring(0, str2.Length - 1);
                    num4 = Convert.ToInt32(str1) - 1;
                    num2 = (short) 23;
                    num1 = (int) num2;
                    continue;
                  case 17:
                    num2 = (short) 12;
                    num1 = (int) num2;
                    continue;
                  case 18:
                  case 24:
                    num2 = (short) 25;
                    num1 = (int) num2;
                    continue;
                  case 19:
                    if (!str2.StartsWith(">"))
                    {
                      num2 = (short) 5;
                      num1 = (int) num2;
                      continue;
                    }
                    num2 = (short) 21;
                    num1 = (int) num2;
                    continue;
                  case 20:
                    num2 = (short) -2001;
                    int num5 = (int) num2;
                    num2 = (short) -2001;
                    int num6 = (int) num2;
                    switch (num5 == num6 ? 1 : 0)
                    {
                      case 0:
                      case 2:
                        break;
                      default:
                        num2 = (short) 0;
                        if (num2 == (short) 0)
                          ;
                        this.IncludeNumbersAbove = (double) int32_1;
                        num2 = (short) 13;
                        num1 = (int) num2;
                        continue;
                    }
                    break;
                  case 21:
                    str1 = str2.Substring(1);
                    int32_1 = Convert.ToInt32(str1);
                    num2 = (short) 14;
                    num1 = (int) num2;
                    continue;
                  case 22:
                    str1 = "1-" + str2.Substring(1);
                    num2 = (short) 9;
                    num1 = (int) num2;
                    continue;
                  case 23:
                    if ((double) num4 < this.IncludeNumbersAbove)
                    {
                      num2 = (short) 2;
                      num1 = (int) num2;
                      continue;
                    }
                    goto case 11;
                  case 25:
                    if (num3 <= int32_2)
                    {
                      this.Add(num3++);
                      num2 = (short) 24;
                      num1 = (int) num2;
                      continue;
                    }
                    num2 = (short) 17;
                    num1 = (int) num2;
                    continue;
                  default:
                    goto label_3;
                }
                num2 = (short) 11;
                num1 = (int) num2;
              }
label_41:
              return;
          }
      }
    }

    public string PointsInCollection
    {
      get
      {
        short num1 = 1;
        if (num1 == (short) 0)
          ;
        num1 = (short) 26291;
        int num2 = (int) num1;
        num1 = (short) 26291;
        int num3 = (int) num1;
        switch (num2 == num3)
        {
          case true:
            num1 = (short) 0;
            num1 = (short) 0;
            if (num1 == (short) 0)
              ;
            return this.a(false);
          default:
            goto case 1;
        }
      }
    }

    public string QueryBuilderPointList
    {
      get
      {
        short num1 = 26111;
        int num2 = (int) num1;
        num1 = (short) 26111;
        int num3 = (int) num1;
        switch (num2 == num3)
        {
          case true:
            short num4 = 1;
            if (num4 == (short) 0)
              ;
            num4 = (short) 0;
            num4 = (short) 0;
            if (num4 == (short) 0)
              ;
            return this.a(true);
          default:
            goto case 1;
        }
      }
    }

    private string a(bool A_0)
    {
      int num1 = 0;
      switch (num1)
      {
        default:
          short num2;
          int[] pointNumberArray;
          string str;
          switch (0)
          {
            case 0:
label_3:
              num2 = (short) -10031;
              int num3 = (int) num2;
              num2 = (short) -10031;
              int num4 = (int) num2;
              switch (num3 == num4 ? 1 : 0)
              {
                case 0:
                case 2:
                  break;
                default:
                  num2 = (short) 0;
                  if (num2 == (short) 0)
                    ;
                  pointNumberArray = this.PointNumberArray;
                  str = "";
                  num2 = (short) 6;
                  num1 = (int) num2;
                  goto label_2;
              }
              break;
            default:
label_2:
              while (true)
              {
                int index;
                int A_0_1;
                int num5;
                switch (num1)
                {
                  case 0:
                    if (double.IsPositiveInfinity(this.IncludeNumbersAbove))
                    {
                      num2 = (short) 15;
                      num1 = (int) num2;
                      continue;
                    }
                    num2 = (short) 24;
                    num1 = (int) num2;
                    continue;
                  case 1:
                    if (A_0)
                    {
                      num2 = (short) 9;
                      num1 = (int) num2;
                      continue;
                    }
                    str = str + (object) ((int) this.IncludeNumbersAbove + 1) + "+";
                    num2 = (short) 28;
                    num1 = (int) num2;
                    continue;
                  case 2:
                    if (pointNumberArray[index + 1] - pointNumberArray[index] == 1)
                    {
                      ++index;
                      num2 = (short) 22;
                      num1 = (int) num2;
                      continue;
                    }
                    num2 = (short) 3;
                    num1 = (int) num2;
                    continue;
                  case 3:
                    str += this.a(A_0_1, pointNumberArray[index]);
                    num2 = (short) 26;
                    num1 = (int) num2;
                    continue;
                  case 4:
                    num2 = (short) 0;
                    num1 = (int) num2;
                    continue;
                  case 5:
                    goto label_13;
                  case 6:
                    if (pointNumberArray.GetLength(0) == 1)
                    {
                      num2 = (short) 13;
                      num1 = (int) num2;
                      continue;
                    }
                    goto case 14;
                  case 7:
                    index = 0;
                    num5 = pointNumberArray.GetLength(0) - 1;
                    num2 = (short) 21;
                    num1 = (int) num2;
                    continue;
                  case 8:
                    num2 = (short) 20;
                    num1 = (int) num2;
                    continue;
                  case 9:
                    str = str + ">" + (object) (int) this.IncludeNumbersAbove;
                    num2 = (short) 25;
                    num1 = (int) num2;
                    continue;
                  case 10:
                    num2 = (short) 2;
                    num1 = (int) num2;
                    continue;
                  case 11:
                  case 22:
                    num2 = (short) 17;
                    num1 = (int) num2;
                    continue;
                  case 12:
                    num2 = (short) 0;
                    break;
                  case 13:
                    str = pointNumberArray[0].ToString();
                    num2 = (short) 14;
                    num1 = (int) num2;
                    continue;
                  case 14:
                    num2 = (short) 16;
                    num1 = (int) num2;
                    continue;
                  case 15:
                    num2 = (short) 27;
                    num1 = (int) num2;
                    continue;
                  case 16:
                    if (pointNumberArray.GetLength(0) > 1)
                    {
                      num2 = (short) 7;
                      num1 = (int) num2;
                      continue;
                    }
                    goto case 4;
                  case 17:
                    if (index < num5)
                    {
                      num2 = (short) 10;
                      num1 = (int) num2;
                      continue;
                    }
                    goto case 3;
                  case 18:
                    str += ",";
                    num2 = (short) 23;
                    num1 = (int) num2;
                    continue;
                  case 19:
                    str = this.b;
                    num2 = (short) 29;
                    num1 = (int) num2;
                    continue;
                  case 20:
                    if (!A_0)
                    {
                      num2 = (short) 19;
                      num1 = (int) num2;
                      continue;
                    }
                    goto label_50;
                  case 21:
                    num2 = (short) 30;
                    num1 = (int) num2;
                    continue;
                  case 23:
                    num2 = (short) 1;
                    if (num2 == (short) 0)
                      goto case 21;
                    else
                      goto case 21;
                  case 24:
                    if (str.Length > 0)
                    {
                      num2 = (short) 5;
                      num1 = (int) num2;
                      continue;
                    }
                    break;
                  case 25:
                  case 28:
                  case 29:
                    goto label_50;
                  case 26:
                    if (index++ < num5)
                    {
                      num2 = (short) 18;
                      num1 = (int) num2;
                      continue;
                    }
                    goto case 21;
                  case 27:
                    if (pointNumberArray.GetLength(0) < 1)
                    {
                      num2 = (short) 8;
                      num1 = (int) num2;
                      continue;
                    }
                    goto label_50;
                  case 30:
                    if (index <= num5)
                    {
                      A_0_1 = pointNumberArray[index];
                      num2 = (short) 11;
                      num1 = (int) num2;
                      continue;
                    }
                    num2 = (short) 4;
                    num1 = (int) num2;
                    continue;
                  default:
                    goto label_3;
                }
                num2 = (short) 1;
                num1 = (int) num2;
              }
label_13:
              str += ",";
              break;
label_50:
              return str;
          }
          num2 = (short) 12;
          num1 = (int) num2;
          goto label_2;
      }
    }

    public string PointsNotInCollection
    {
      get
      {
        int num1 = 0;
        switch (num1)
        {
          default:
            int[] pointNumberArray;
            string str;
            short num2;
            switch (0)
            {
              case 0:
label_3:
                pointNumberArray = this.PointNumberArray;
                str = "";
                num2 = (short) 7;
                num1 = (int) num2;
                goto default;
              default:
                while (true)
                {
                  int index;
                  switch (num1)
                  {
                    case 0:
                      if (index >= pointNumberArray.GetLength(0) - 1)
                      {
                        num2 = (short) 10;
                        num1 = (int) num2;
                        continue;
                      }
                      goto case 8;
                    case 1:
                      num2 = (short) 17;
                      num1 = (int) num2;
                      continue;
                    case 2:
                      if (pointNumberArray.GetLength(0) > 1)
                      {
                        num2 = (short) 22;
                        num1 = (int) num2;
                        continue;
                      }
                      goto case 14;
                    case 3:
                      if (index < pointNumberArray.GetLength(0) - 1)
                      {
                        num2 = (short) 0;
                        num2 = (short) 5;
                        num1 = (int) num2;
                        continue;
                      }
                      goto case 1;
                    case 4:
                      str += ",";
                      num2 = (short) 19;
                      num1 = (int) num2;
                      continue;
                    case 5:
                      num2 = (short) 6;
                      num1 = (int) num2;
                      continue;
                    case 6:
                      if (pointNumberArray[index + 1] - pointNumberArray[index] != 1)
                      {
                        num2 = (short) 1;
                        num1 = (int) num2;
                        continue;
                      }
                      ++index;
                      num2 = (short) 8;
                      num1 = (int) num2;
                      continue;
                    case 7:
                      if (pointNumberArray.GetLength(0) == 1)
                      {
                        num2 = (short) 23;
                        num1 = (int) num2;
                        continue;
                      }
                      break;
                    case 8:
                      num2 = (short) 3;
                      num1 = (int) num2;
                      continue;
                    case 9:
                    case 21:
                      num2 = (short) 0;
                      num1 = (int) num2;
                      continue;
                    case 10:
                      num2 = (short) 1;
                      if (num2 == (short) 0)
                        ;
                      str = str + (pointNumberArray[index] + 1).ToString() + this.endingForExcludeList;
                      num2 = (short) 14;
                      num1 = (int) num2;
                      continue;
                    case 11:
                      str = "1" + this.endingForExcludeList;
                      num2 = (short) 16;
                      num1 = (int) num2;
                      continue;
                    case 12:
                      if (!str.Equals(""))
                      {
                        num2 = (short) 4;
                        num1 = (int) num2;
                        continue;
                      }
                      goto case 19;
                    case 13:
                      if (pointNumberArray.GetLength(0) < 1)
                      {
                        num2 = (short) 11;
                        num1 = (int) num2;
                        continue;
                      }
                      goto label_40;
                    case 14:
                      num2 = (short) 13;
                      num1 = (int) num2;
                      continue;
                    case 15:
                      num2 = (short) -26087;
                      int num3 = (int) num2;
                      num2 = (short) -26087;
                      int num4 = (int) num2;
                      switch (num3 == num4 ? 1 : 0)
                      {
                        case 0:
                        case 2:
                          continue;
                        default:
                          num2 = (short) 0;
                          if (num2 == (short) 0)
                            break;
                          break;
                      }
                      break;
                    case 16:
                      goto label_40;
                    case 17:
                      if (index < pointNumberArray.GetLength(0) - 1)
                      {
                        num2 = (short) 24;
                        num1 = (int) num2;
                        continue;
                      }
                      goto case 9;
                    case 18:
                      if (!str.Equals(""))
                      {
                        num2 = (short) 20;
                        num1 = (int) num2;
                        continue;
                      }
                      goto case 9;
                    case 19:
                      str = str + (pointNumberArray[0] + 1).ToString() + this.endingForExcludeList;
                      num2 = (short) 15;
                      num1 = (int) num2;
                      continue;
                    case 20:
                      str += ",";
                      num2 = (short) 21;
                      num1 = (int) num2;
                      continue;
                    case 22:
                      index = 0;
                      str = this.a(1, pointNumberArray[0] - 1);
                      num2 = (short) 18;
                      num1 = (int) num2;
                      continue;
                    case 23:
                      str += this.a(1, pointNumberArray[0] - 1);
                      num2 = (short) 12;
                      num1 = (int) num2;
                      continue;
                    case 24:
                      str = str + this.a(pointNumberArray[index] + 1, pointNumberArray[++index] - 1) + ",";
                      num2 = (short) 9;
                      num1 = (int) num2;
                      continue;
                    default:
                      goto label_3;
                  }
                  num2 = (short) 2;
                  num1 = (int) num2;
                }
label_40:
                return str;
            }
        }
      }
    }

    private string endingForExcludeList
    {
      get
      {
        if (double.IsPositiveInfinity(this.IncludeNumbersAbove))
        {
          short num1 = 1;
          if (num1 == (short) 0)
            ;
          num1 = (short) -21602;
          int num2 = (int) num1;
          num1 = (short) -21602;
          int num3 = (int) num1;
          switch (num2 == num3 ? 1 : 0)
          {
            case 0:
            case 2:
              break;
            default:
              num1 = (short) 0;
              if (num1 == (short) 0)
                ;
              num1 = (short) 0;
              return "+";
          }
        }
        return "-" + (object) (int) this.IncludeNumbersAbove;
      }
    }

    private string a(int A_0, int A_1)
    {
      int num1;
      int num2;
      short num3;
      switch (0)
      {
        case 0:
label_2:
          num2 = A_1 - A_0;
          num3 = (short) 2;
          num1 = (int) num3;
          goto default;
        default:
          while (true)
          {
            switch (num1)
            {
              case 0:
                if (num2 > 0)
                {
                  num3 = (short) 1;
                  num1 = (int) num3;
                  continue;
                }
                goto label_13;
              case 1:
                goto label_5;
              case 2:
                num3 = (short) 0;
                if (num2 == 0)
                {
                  num3 = (short) 3;
                  num1 = (int) num3;
                  continue;
                }
                num3 = (short) 0;
                num1 = (int) num3;
                continue;
              case 3:
                goto label_9;
              default:
                goto label_2;
            }
          }
label_5:
          return A_0.ToString() + "-" + A_1.ToString();
label_9:
          num3 = (short) 1;
          if (num3 == (short) 0)
            ;
          num3 = (short) -18009;
          int num4 = (int) num3;
          num3 = (short) -18009;
          int num5 = (int) num3;
          switch (num4 == num5 ? 1 : 0)
          {
            case 0:
            case 2:
              goto label_2;
            default:
              num3 = (short) 0;
              if (num3 == (short) 0)
                ;
              return A_0.ToString();
          }
label_13:
          return "";
      }
    }

    public int[] PointNumberArray
    {
      get
      {
        short num1 = -18299;
        int num2 = (int) num1;
        num1 = (short) -18299;
        int num3 = (int) num1;
        switch (num2 == num3)
        {
          case true:
            short num4 = 1;
            if (num4 == (short) 0)
              ;
            num4 = (short) 0;
            num4 = (short) 0;
            if (num4 == (short) 0)
              ;
            SortedDictionary<int, int>.KeyCollection keys = this.a.Keys;
            int[] array = new int[this.a.Count];
            this.a.Keys.CopyTo(array, 0);
            return array;
          default:
            goto case 1;
        }
      }
    }

    public IList<CogoPoint> GetIncludedCogoPoints(CivilAppConnection aeccConn)
    {
      short num1 = 0;
      num1 = (short) 0;
      switch (num1)
      {
        default:
          List<CogoPoint> cogoPointList = new List<CogoPoint>();
          Transaction transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction();
          try
          {
            num1 = (short) 25633;
            int num2 = (int) num1;
            num1 = (short) 25633;
            int num3 = (int) num1;
            switch (num2 == num3 ? 1 : 0)
            {
              case 0:
              case 2:
label_23:
                num1 = (short) 1;
                if (num1 == (short) 0)
                  ;
                transaction.Commit();
                break;
              default:
                num1 = (short) 0;
                if (num1 == (short) 0)
                  ;
                IEnumerator<ObjectId> enumerator = aeccConn.AeccDb.CogoPoints.GetEnumerator();
                try
                {
                  num1 = (short) 3;
                  int num4 = (int) num1;
                  while (true)
                  {
                    CogoPoint cogoPoint;
                    switch (num4)
                    {
                      case 0:
                        if (this.ContainsPoint((int) cogoPoint.PointNumber))
                        {
                          num1 = (short) 1;
                          num4 = (int) num1;
                          continue;
                        }
                        break;
                      case 1:
                        cogoPointList.Add(cogoPoint);
                        num1 = (short) 6;
                        num4 = (int) num1;
                        continue;
                      case 2:
                        goto label_23;
                      case 3:
                        switch (0)
                        {
                          case 0:
                            break;
                          default:
                            continue;
                        }
                        break;
                      case 4:
                        if (((IEnumerator) enumerator).MoveNext())
                        {
                          ObjectId current = enumerator.Current;
                          cogoPoint = (CogoPoint) ((ObjectId) ref current).GetObject((OpenMode) 0);
                          num1 = (short) 0;
                          num4 = (int) num1;
                          continue;
                        }
                        num1 = (short) 5;
                        num4 = (int) num1;
                        continue;
                      case 5:
                        num1 = (short) 2;
                        num4 = (int) num1;
                        continue;
                    }
                    num1 = (short) 4;
                    num4 = (int) num1;
                  }
                }
                finally
                {
                  short num5 = 0;
                  int num6 = (int) num5;
                  while (true)
                  {
                    switch (num6)
                    {
                      case 0:
                        switch (0)
                        {
                          case 0:
                            break;
                          default:
                            continue;
                        }
                        break;
                      case 1:
                        ((IDisposable) enumerator).Dispose();
                        num5 = (short) 2;
                        num6 = (int) num5;
                        continue;
                      case 2:
                        goto label_22;
                    }
                    if (enumerator != null)
                    {
                      num5 = (short) 1;
                      num6 = (int) num5;
                    }
                    else
                      break;
                  }
label_22:;
                }
            }
          }
          finally
          {
            int num7 = 0;
            while (true)
            {
              switch (num7)
              {
                case 0:
                  switch (0)
                  {
                    case 0:
                      break;
                    default:
                      continue;
                  }
                  break;
                case 1:
                  ((IDisposable) transaction).Dispose();
                  num7 = 2;
                  continue;
                case 2:
                  goto label_31;
              }
              if (transaction != null)
                num7 = 1;
              else
                break;
            }
label_31:;
          }
          return (IList<CogoPoint>) cogoPointList;
      }
    }

    public object Clone()
    {
      short num1 = 13691;
      int num2 = (int) num1;
      num1 = (short) 13691;
      int num3 = (int) num1;
      short num4;
      switch (num2 == num3)
      {
        case true:
          num4 = (short) 1;
          if (num4 == (short) 0)
            ;
          num4 = (short) 0;
          if (num4 == (short) 0)
            ;
          PointListCreator pointListCreator = new PointListCreator();
          pointListCreator.Add(this.PointNumberArray);
          return (object) pointListCreator;
        default:
          num4 = (short) 0;
          goto case 1;
      }
    }
  }