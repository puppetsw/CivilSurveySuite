using CivilSurveySuite.UI.Strings;

namespace CivilSurveySuite.UI.Helpers
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
