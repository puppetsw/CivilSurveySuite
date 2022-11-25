using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.GraphicsInterface;

namespace _3DS_CivilSurveySuite.ACAD
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
