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
        /// <summary>
        /// Gets the site by the site name.
        /// </summary>
        /// <param name="tr">Transaction.</param>
        /// <param name="siteName">The site name.</param>
        /// <returns>The <see cref="Site"/> if found, otherwise null.</returns>
        /// <exception cref="ArgumentException">Thrown if siteName is null or empty</exception>
        public static Site GetSite(Transaction tr, string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
                throw new ArgumentException(nameof(siteName));

            foreach (ObjectId siteId in C3DApp.ActiveDocument.GetSiteIds())
            {
                var site = tr.GetObject(siteId, OpenMode.ForRead) as Site;

                if (site == null)
                    continue;

                if (site.Name == siteName)
                    return site;
            }

            return null;
        }

        /// <summary>
        /// Gets the site by it's <see cref="ObjectId"/>.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <returns>Site.</returns>
        /// <exception cref="ArgumentNullException">is thrown if the <see cref="Transaction"/> is null.</exception>
        /// <exception cref="ArgumentNullException">is thrown if the <see cref="ObjectId"/> is null.</exception>
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
            // Add the None site.
            sites.Add(CivilSite.NoneSite);

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in C3DApp.ActiveDocument.GetSiteIds())
                {
                    var site = tr.GetObject(objectId, OpenMode.ForRead) as Site;

                    if (site == null)
                        continue;

                    sites.Add(site.ToCivilSite());
                }

                tr.Commit();
            }
            return sites;
        }
    }
}