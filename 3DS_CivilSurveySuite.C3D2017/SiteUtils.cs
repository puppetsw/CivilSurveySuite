// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class SiteUtils
    {

        public static Site GetSite(string siteName)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the site.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <returns>Site.</returns>
        /// <exception cref="ArgumentNullException">tr</exception>
        /// <exception cref="ArgumentNullException">objectId</exception>
        public static Site GetSite(Transaction tr, ObjectId objectId)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (objectId.IsNull)
                throw new ArgumentNullException(nameof(objectId));

            return tr.GetObject(objectId, OpenMode.ForRead) as Site;
        }


        /// <summary>
        /// Converts to civilsite.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>CivilSite.</returns>
        /// <exception cref="ArgumentNullException">site</exception>
        public static CivilSite ToCivilSite(this Site site)
        {
            if (site == null)
                throw new ArgumentNullException(nameof(site));

            return new CivilSite
            {
                Name = site.Name,
                Description = site.Description,
                ObjectId = site.ObjectId.Handle.ToString()
            };
        }

        /// <summary>
        /// Gets the civil sites.
        /// </summary>
        /// <returns>IEnumerable&lt;CivilSite&gt;.</returns>
        public static IEnumerable<CivilSite> GetCivilSites()
        {
            var sites = new List<CivilSite>();

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in C3DApp.ActiveDocument.GetSiteIds())
                {
                    var site = tr.GetObject(objectId, OpenMode.ForRead) as Site;
                 
                    if (site == null)
                        continue;
                    
                    var civilSite = new CivilSite
                    {
                        Name = site.Name,
                        Description = site.Description,
                        ObjectId = objectId.Handle.ToString()
                    };

                    sites.Add(civilSite);
                }

                tr.Commit();
            }
            return sites;
        }


    }
}