using System.Windows;
using System.Windows.Controls;

namespace _3DS_CivilSurveySuite.UI.Controls
{
    public class CardControl : Button
    {
        /// <summary>
        /// Property for <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(CardControl),
            new PropertyMetadata(""));

        /// <summary>
        /// Property for <see cref="Subtitle"/>.
        /// </summary>
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
            nameof(Subtitle),
            typeof(string),
            typeof(CardControl),
            new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets text displayed on the left side of the card.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets text displayed under main <see cref="Title"/>.
        /// </summary>
        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }
    }
}