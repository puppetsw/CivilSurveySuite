// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Globalization;
using Autodesk.AutoCAD.DatabaseServices;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class ObjectIdExtensions
    {
        public static ObjectId[] ToArray(this ObjectIdCollection objectIdCollection)
        {
            var arrLength = objectIdCollection.Count - 1;
            var objectIds = new ObjectId[arrLength];

            for (int i = 0; i < arrLength; i++)
            {
                objectIds[i] = objectIdCollection[i];
            }

            return objectIds;
        }


        public static ObjectId ToObjectId(this string objectId)
        {
            Handle handle = new Handle(long.Parse(objectId, NumberStyles.AllowHexSpecifier));
            AcadApp.ActiveDatabase.TryGetObjectId(handle, out ObjectId id);
            return id;
        }


    }
}