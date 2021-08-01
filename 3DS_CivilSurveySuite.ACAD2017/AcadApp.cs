// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.ACAD2017.Extensions;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// Provides access to several "active" objects and helper methods
    /// in the AutoCAD runtime environment.
    /// </summary>
    public static class AcadApp
    {
        /// <summary>
        /// Gets the <see cref="DocumentManager"/>.
        /// </summary>
        public static DocumentCollection DocumentManager => Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;

        /// <summary>
        /// Gets the active <see cref="Document"/> object.
        /// </summary>
        public static Document ActiveDocument => DocumentManager.MdiActiveDocument;

        /// <summary>
        /// Gets the active <see cref="Database"/> object.
        /// </summary>
        public static Database ActiveDatabase => ActiveDocument.Database;

        /// <summary>
        /// Gets the active <see cref="Editor"/> object.
        /// </summary>
        public static Editor Editor => ActiveDocument.Editor;

        /// <summary>
        /// Starts a transaction.
        /// </summary>
        /// <returns>Transaction.</returns>
        public static Transaction StartTransaction() => ActiveDocument.TransactionManager.StartTransaction();

        /// <summary>
        /// Starts a locked transaction.
        /// </summary>
        /// <returns>Transaction.</returns>
        public static Transaction StartLockedTransaction() => ActiveDocument.TransactionManager.StartLockedTransaction();

        public static void UsingTransaction(Action<Transaction> action)
        {
            using (var tr = StartTransaction())
            {
                action(tr);
                tr.Commit();
            }
        }

        public static void UsingLockedTransaction(Action<Transaction> action)
        {
            using (var tr = StartLockedTransaction())
            {
                action(tr);
                tr.Commit();
            }
        }

        public static void UsingModelSpace(Action<Transaction, BlockTableRecord> action)
        {
            using (var tr = StartTransaction())
            {
                // Get BlockTable and ModelSpace
                var blockTable = (BlockTable) tr.GetObject(ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                var modelSpace = (BlockTableRecord) tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                // Execute the action.
                action(tr, modelSpace);

                tr.Commit();
            }
        }

        public static void ForEach<T>(this Database database, Action<T> action) where T : Entity
        {
            using (var tr = StartTransaction())
            {
                // Get BlockTable and ModelSpace.
                var blockTable = (BlockTable) tr.GetObject(ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                var modelSpace = (BlockTableRecord) tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                // Get the entity type.
                RXClass entityType = RXObject.GetClass(typeof(T));

                foreach (ObjectId objectId in modelSpace)
                {
                    // Match correct type
                    if (objectId.ObjectClass.IsDerivedFrom(entityType))
                    {
                        var entity = (T) tr.GetObject(objectId, OpenMode.ForRead);
                        action(entity);
                    }
                }
                tr.Commit();
            }
        }
    }
}