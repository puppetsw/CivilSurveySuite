// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Linq;
using _3DS_CivilSurveySuite.Model;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class CogoPointExtensions
    {
        public static CivilPoint ToCivilPoint(this CogoPoint cogoPoint)
        {
            return new CivilPoint
            {
                PointNumber = cogoPoint.PointNumber,
                Easting = cogoPoint.Easting,
                Northing = cogoPoint.Northing,
                Elevation = cogoPoint.Elevation,
                RawDescription = cogoPoint.RawDescription,
                DescriptionFormat = cogoPoint.DescriptionFormat,
                ObjectId = cogoPoint.ObjectId.Handle.ToString(),
                PointName = cogoPoint.PointName
            };
        }

        public static IEnumerable<CivilPoint> ToListOfCivilPoints(this IEnumerable<CogoPoint> cogoPoints)
        {
            return cogoPoints.Select(cogoPoint => cogoPoint.ToCivilPoint()).ToList();
        }
    }
}