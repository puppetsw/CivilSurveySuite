// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Globalization;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class ObjectIdExtensions
    {
        public static ObjectId[] ToArray(this ObjectIdCollection objectIdCollection)
        {
            int arrLength = objectIdCollection.Count - 1;
            var objectIds = new ObjectId[arrLength];

            for (var i = 0; i < arrLength; i++)
            {
                objectIds[i] = objectIdCollection[i];
            }

            return objectIds;
        }

        public static ObjectId ToObjectId(this string objectId)
        {
            var handle = new Handle(long.Parse(objectId, NumberStyles.AllowHexSpecifier));
            AcadApp.ActiveDatabase.TryGetObjectId(handle, out var id);
            return id;
        }

        /// <summary>
        /// Removes objects from the collection if they are in the index list.
        /// </summary>
        /// <param name="objectIdCollection">The ObjectIdCollection.</param>
        /// <param name="indexList">The list of indexes that are to be removed.</param>
        public static void RemoveAtIndexes(this ObjectIdCollection objectIdCollection, IEnumerable<int> indexList)
        {
            foreach (int index in indexList)
            {
                objectIdCollection.RemoveAt(index);
            }
        }
    }
}