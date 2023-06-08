using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CivilSurveySuite.Common.Models;

namespace CivilSurveySuite.ACAD
{
    public static class BlockUtils
    {
        public static IEnumerable<BlockReference> GetBlockReferences(this BlockTableRecord btr, OpenMode mode = OpenMode.ForRead, bool directOnly = true)
        {
            if (btr == null)
            {
                throw new ArgumentNullException(nameof(btr));
            }

            var tr = btr.Database.TransactionManager.TopTransaction;
            if (tr == null)
            {
                throw new InvalidOperationException("No transaction");
            }

            var ids = btr.GetBlockReferenceIds(directOnly, true);
            int cnt = ids.Count;
            for (int i = 0; i < cnt; i++)
            {
                yield return (BlockReference)tr.GetObject(ids[i], mode, false, false);
            }

            if (btr.IsDynamicBlock)
            {
                var blockIds = btr.GetAnonymousBlockIds();
                cnt = blockIds.Count;
                for (int i = 0; i < cnt; i++)
                {
                    var btr2 = (BlockTableRecord)tr.GetObject(blockIds[i], OpenMode.ForRead, false, false);
                    ids = btr2.GetBlockReferenceIds(directOnly, true);
                    int cnt2 = ids.Count;
                    for (int j = 0; j < cnt2; j++)
                    {
                        yield return (BlockReference)tr.GetObject(ids[j], mode, false, false);
                    }
                }
            }
        }

        public static IEnumerable<AcadBlock> GetBlocks()
        {
            var list = new List<AcadBlock>();

            using (var tr = AcadApp.StartTransaction())
            {
                var bt = (BlockTable)tr.GetObject(AcadApp.ActiveDatabase.BlockTableId, OpenMode.ForRead);

                foreach (ObjectId objectId in bt)
                {
                    var btr = (BlockTableRecord)objectId.GetObject(OpenMode.ForRead);

                    if (btr.IsLayout)
                    {
                        continue;
                    }

                    var attributes = GetBlockAttributeTags(btr.Name);
                    list.Add(new AcadBlock
                    {
                        ObjectId = objectId.ToString(),
                        Name = btr.Name,
                        Attributes = new ObservableCollection<AcadBlockAttribute>(attributes)
                    });
                }


                tr.Commit();
            }

            return list;
        }

        public static AcadBlock GetBlockByName(string blockName)
        {
            throw new NotSupportedException();
        }

        public static string GetBlockName(Transaction tr, ObjectId blockId)
        {
            BlockReference blockRef = (BlockReference)tr.GetObject(blockId, OpenMode.ForRead);
            BlockTableRecord block;

            if (blockRef.IsDynamicBlock)
            {
                //get the real dynamic block name.
                block = tr.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            }
            else
            {
                block = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            }

            return block != null ? block.Name : string.Empty;
        }

        public static string GetBlockName(Transaction tr, BlockReference blockRef)
        {
            BlockTableRecord block;

            if (blockRef.IsDynamicBlock)
            {
                //get the real dynamic block name.
                block = tr.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            }
            else
            {
                block = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
            }

            return block != null ? block.Name : string.Empty;
        }

        public static bool TryUpdateBlockAttribute(Transaction tr, ObjectId blockId, string attributeName, string attributeValue)
        {
            BlockReference blockRef = (BlockReference)tr.GetObject(blockId, OpenMode.ForRead);
            BlockTableRecord block = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead);

            if (!block.HasAttributeDefinitions)
            {
                return false;
            }

            foreach (ObjectId id in blockRef.AttributeCollection)
            {
                DBObject obj = tr.GetObject(id, OpenMode.ForRead);
                var ar = obj as AttributeReference;

                if (ar == null)
                {
                    continue;
                }

                if (ar.Tag != attributeName)
                {
                    continue;
                }

                ar.UpgradeOpen();
                ar.TextString = attributeValue;
                ar.DowngradeOpen();
                return true;
            }

            return false;
        }
        private static IEnumerable<AcadBlockAttribute> GetBlockAttributeTags(string blockName)
        {
            var result = new List<AcadBlockAttribute>();
            var db = HostApplicationServices.WorkingDatabase;
            var attDefClass = RXObject.GetClass(typeof(AttributeDefinition));

            using (var tr = new OpenCloseTransaction())
            {
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord)tr.GetObject(bt[blockName], OpenMode.ForRead);

                if (btr.HasAttributeDefinitions)
                {
                    foreach (ObjectId id in btr)
                    {
                        if (id.ObjectClass == attDefClass)
                        {
                            var attDef = (AttributeDefinition)tr.GetObject(id, OpenMode.ForRead);
                            result.Add(new AcadBlockAttribute { Tag = attDef.Tag });
                        }
                    }
                }
            }
            return result;
        }
    }
}
