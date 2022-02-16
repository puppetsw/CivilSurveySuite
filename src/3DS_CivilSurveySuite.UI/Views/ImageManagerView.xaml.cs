// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Windows;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    public sealed partial class ImageManagerView : Window
    {
        public ImageManagerView(ImageManagerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BtnInsertImage_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}