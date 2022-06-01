using System.Windows;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class InputDialogView : IInputDialogService
    {
        public InputDialogView()
        {
            InitializeComponent();

            InputText.Focus();
        }

        private void PrimaryButton_OnClick(object sender, RoutedEventArgs e)
        {
            ResultString = InputText.Text;
            DialogResult = true;
            Close();
        }

        private void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            ResultString = string.Empty;
            DialogResult = false;
            Close();
        }

        public string ResultString { get; set; }
        public void AssignOptions(InputServiceOptions options)
        {
            Title = options.Title;
            InputMessageText.Text = options.Message;
            PrimaryButton.Content = options.PrimaryButtonText;
        }
    }
}
