// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class RasterUtils
    {
        /// <summary>
        /// Attaches a raster image to the drawing.
        /// </summary>
        /// <remarks>
        /// https://adndevblog.typepad.com/autocad/2012/05/how-to-insert-a-rasterimage-using-the-net-api.html
        /// </remarks>
        public static void AttachRasterImage(Transaction tr, string fileName, Point3d position,
            double imageWidth = 1.5, double imageHeight = 1)
        {
            const string recBase = "3DSIMG";

            RasterImageDef imageDef;

            var imageDictionaryId = RasterImageDef.GetImageDictionary(AcadApp.ActiveDatabase);

            if (imageDictionaryId.IsNull)
                imageDictionaryId = RasterImageDef.CreateImageDictionary(AcadApp.ActiveDatabase);

            // Open the image dictionary.
            var imageDictionary = (DBDictionary)tr.GetObject(imageDictionaryId, OpenMode.ForRead);

            // Check if the raster definition is already in the dictionary.
            if (imageDictionary.Contains(fileName))
            {
                // Get the raster image definition.
                var imageDefId = imageDictionary.GetAt(fileName);
                imageDef = (RasterImageDef)tr.GetObject(imageDefId, OpenMode.ForWrite);
            }
            else
            {
                // Generate name for the raster image definition.
                var i = 0;
                string recName = recBase + i;
                while (imageDictionary.Contains(recName))
                {
                    i++;
                    recName = recBase + i;
                }

                // Create a raster image definition.
                imageDef = new RasterImageDef();

                // Set its source image.
                imageDef.SourceFileName = fileName;

                // Finally load it.
                imageDef.Load();

                // Add the new image definition to the dictionary.
                imageDictionary.UpgradeOpen();
                imageDictionary.SetAt(recName, imageDef);
                tr.AddNewlyCreatedDBObject(imageDef, true);
            }

            // Now create the raster image that references the definition
            var image = new RasterImage();
            image.ImageDefId = imageDef.ObjectId;

            // Prepare the orientation
            var uCorner = new Vector3d(imageWidth, 0, 0);
            var vOnPlane = new Vector3d(0, imageHeight, 0);
            var coordinateSystem = new CoordinateSystem3d(position, uCorner, vOnPlane);

            image.Orientation = coordinateSystem;

            // And some other properties
            image.ImageTransparency = true;
            image.ShowImage = true;

            // Add the image to ModelSpace
            var bt = (BlockTable)tr.GetObject(AcadApp.ActiveDatabase.BlockTableId, OpenMode.ForRead);

            var msBtr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
            msBtr.AppendEntity(image);

            tr.AddNewlyCreatedDBObject(image, true);

            // Create a reactor between the RasterImage
            // and the RasterImageDef to avoid the "Unreferenced"
            // warning the XRef palette
            RasterImage.EnableReactors(true);
            image.AssociateRasterDef(imageDef);
        }

        /// <summary>
        /// Attaches raster images to a drawing starting at a position and incrementing by a given offsets.
        /// </summary>
        /// <param name="fileNames">List of file names to the images.</param>
        /// <param name="startingPosition">The starting position.</param>
        /// <param name="imageWidth">The width of the image in the drawing.</param>
        /// <param name="imageHeight">The height of the image in the drawing.</param>
        /// <param name="padding">The padding/spacing between images.</param>
        /// <param name="rowLimit">The limit before a new row is started.</param>
        public static void AttachRasterImages(IEnumerable<string> fileNames, Point3d startingPosition,
            double imageWidth, double imageHeight, double padding, int rowLimit = 5)
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var position = startingPosition;
                var imageCount = 1;
                foreach (string fileName in fileNames)
                {
                    double x = position.X + imageWidth + padding;
                    double y = position.Y;
                    double z = position.Z;

                    if (imageCount == rowLimit)
                    {
                        y -= imageHeight + padding;
                        x = startingPosition.X;
                        position = startingPosition; // Reset the position.
                        imageCount = 0; // Reset to 0 because we increment at the end.
                    }

                    AttachRasterImage(tr, fileName, position, imageWidth, imageHeight);
                    position = new Point3d(x, y, z);
                    imageCount++;
                }

                tr.Commit();
            }
        }
    }
}