using System;
using System.Windows.Markup;

namespace CivilSurveySuite.UI.Helpers
{
    public sealed class ResourceString : MarkupExtension
    {
        public string Name
        {
            get; set;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ResourceHelpers.GetLocalisedString(Name);
        }
    }
}