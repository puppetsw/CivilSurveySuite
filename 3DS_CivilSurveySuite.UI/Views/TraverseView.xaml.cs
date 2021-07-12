using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for TraverseView.xaml
    /// </summary>
    public partial class TraverseView : UserControl
    {
        public TraverseView()
        {
            InitializeComponent();
        }

        private void dgTraverse_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //var uiElement = e.OriginalSource as UIElement;
            //if (e.Key == Key.Enter && uiElement != null)
            //{
            //    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //    e.Handled = true;
            //} 
            //else
            //{
            //    int keyValue = (int)e.Key;
            //    if ((keyValue >= 0x30 && keyValue <= 0x39) // numbers
            //     || (keyValue >= 0x41 && keyValue <= 0x5A) // letters
            //     || (keyValue >= 0x60 && keyValue <= 0x69)) // numpad
            //    {
            //        dgTraverse.BeginEdit();
            //    }
            //}

            var dg = sender as DataGrid;

            var currentCell = dg.CurrentCell;

            // alter this condition for whatever valid keys you want - avoid arrows/tab, etc.
            //if (dg != null && !dg.IsReadOnly && e.Key == Key.Enter)
            //{
            //    var cell = dg.GetSelectedCell();
            //    if (cell != null && cell.Column is DataGridTemplateColumn)
            //    {
            //        cell.Focus();
            //        dg.BeginEdit();
            //        e.Handled = true;
            //    }
            //}
        }
    }
}

