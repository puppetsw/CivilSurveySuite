


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
