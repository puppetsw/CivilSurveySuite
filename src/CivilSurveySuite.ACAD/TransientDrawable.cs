using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.GraphicsInterface;

namespace CivilSurveySuite.ACAD
{
    public sealed class TransientDrawable : List<Drawable>, IDisposable
    {
        public void Dispose()
        {
            foreach (Drawable drawable in this)
            {
                drawable.Dispose();
            }
        }
    }
}
