// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.UI.Strings;

namespace _3DS_CivilSurveySuite.UI.Helpers
{
    public static class ResourceHelpers
    {
        public static string GetLocalisedString(string name)
        {
            var resourceMgr = ResourceStrings.ResourceManager;
            var str = resourceMgr.GetString(name);
            return str;
        }
    }
}
