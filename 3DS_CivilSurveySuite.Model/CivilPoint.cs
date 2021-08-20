// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

namespace _3DS_CivilSurveySuite.Model
{
    /// <summary>
    /// Abstraction class for Civil 3D's CogoPoints.
    /// </summary>
    public class CivilPoint
    {
        public uint PointNumber { get; set; }

        public double Easting { get; set; }

        public double Northing { get; set; }

        public double Elevation { get; set;  }

        public string RawDescription { get; set; }

        public string FullDescription { get; set; }

        public string ObjectIdHandle { get; }

        public string Tag { get; }

        public string Name { get; }

        public CivilPoint(uint pointNumber, double easting, double northing, double elevation, string rawDescription, string fullDescription, string objectIdHandle, string tag, string name)
        {
            PointNumber = pointNumber;
            Easting = easting;
            Northing = northing;
            Elevation = elevation;
            RawDescription = rawDescription;
            FullDescription = fullDescription;
            ObjectIdHandle = objectIdHandle;
            Tag = tag;
            Name = name;
        }
    }

   
}
