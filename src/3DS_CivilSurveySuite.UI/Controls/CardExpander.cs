using System.Windows;
using System.Windows.Controls;

namespace _3DS_CivilSurveySuite.UI.Controls
{
    public class CardExpander : Expander
    {
        /// <summary>
        /// Property for <see cref="Subtitle"/>.
        /// </summary>
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(nameof(Subtitle),
            typeof(string), typeof(CardExpander), new PropertyMetadata(null));

        /// <summary>
        /// Property for <see cref="HeaderContent"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register(nameof(HeaderContent), typeof(object), typeof(CardExpander),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets text displayed under main <see cref="HeaderContent"/>.
        /// </summary>
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        /// <summary>
        /// Gets or sets additional content displayed next to the chevron.
        /// </summary>
        public object HeaderContent
        {
            get => GetValue(HeaderContentProperty);
            set => SetValue(HeaderContentProperty, value);
        }
    }
}
