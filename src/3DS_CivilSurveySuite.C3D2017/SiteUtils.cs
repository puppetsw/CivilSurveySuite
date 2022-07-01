// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Shared.Models;
using Autodesk.AECC.Interop.Land;
using Autodesk.AECC.Interop.UiLand;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
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
            var sites = new List<CivilSite>
            {
                // Add the None site.
                CivilSite.NoneSite
            };

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

        /// <summary>
        /// Tries to create a site with the given name.
        /// </summary>
        /// <param name="tr">The active <see cref="Transaction"/>. If null starts a new <see cref="Transaction"/></param>
        /// <param name="siteName">The name of the site to create.</param>
        /// <param name="siteId">The <see cref="ObjectId"/> of the created site. Null if site not created.</param>
        /// <returns><c>True</c> if the site was created successfully, otherwise <c>false</c>.</returns>
        public static bool TryCreateSite(Transaction tr, string siteName, out ObjectId siteId)
        {
            siteId = ObjectId.Null;
            if (string.IsNullOrEmpty(siteName))
            {
                return false;
            }

            bool CheckCreateSite(out ObjectId id)
            {
                id = ObjectId.Null;
                foreach (ObjectId objectId in C3DApp.ActiveDocument.GetSiteIds())
                {
                    var site = (Site)tr.GetObject(objectId, OpenMode.ForRead);
                    if (site.Name.Equals(siteName, StringComparison.InvariantCulture))
                    {
                        // Site alread exsits
                        return false;
                    }
                }

                id = Site.Create(C3DApp.ActiveDocument, siteName);
                return true;
            }

            if (tr == null)
            {
                using (tr = AcadApp.StartTransaction())
                {
                    if (!CheckCreateSite(out siteId))
                    {
                        return false;
                    }

                    tr.Commit();
                }
            }
            else
            {
                if (!CheckCreateSite(out siteId))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool TryDeleteSite(Transaction tr, string siteName)
        {
            var app = new AeccAppConnection();

            if (app.AeccApp != null)
            {
                int index = 0;
                foreach (AeccSite site in app.AeccDb.Sites)
                {
                    if (site.Name.Equals(siteName))
                    {
                        app.AeccDb.Sites.Remove(index);
                        return true;
                    }

                    index++;
                }
            }

            return false;
            // UNDONE: Can't delete site using Erase()
            // See: https://forums.autodesk.com/t5/civil-3d-customization/removing-sites/td-p/4687437
            // Also see: https://forums.autodesk.com/t5/civil-3d-customization/erase-site/m-p/4759889
            /*foreach (ObjectId objectId in C3DApp.ActiveDocument.GetSiteIds())
            {
                var site = (Site)tr.GetObject(objectId, OpenMode.ForRead);
                if (site.Name.Equals(siteName, StringComparison.InvariantCulture))
                {
                    site.UpgradeOpen();
                    site.Erase();
                    return true;
                }
            }
            return false;*/
        }
    }
}
