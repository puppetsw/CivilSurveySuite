// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

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