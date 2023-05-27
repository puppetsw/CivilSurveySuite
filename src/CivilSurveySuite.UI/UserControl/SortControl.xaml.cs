using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.UI.UserControl
{
    /// <summary>
    /// Interaction logic for SortControl.xaml
    /// </summary>
    public partial class SortControl
    {
        public static readonly DependencyProperty HeadersProperty = DependencyProperty.Register(
            "Headers", typeof(ObservableCollection<ColumnHeader>), typeof(SortControl),
            new PropertyMetadata(default(ObservableCollection<ColumnHeader>)));

        public ObservableCollection<ColumnHeader> Headers
        {
            get => (ObservableCollection<ColumnHeader>)GetValue(HeadersProperty);
            set => SetValue(HeadersProperty, value);
        }

        // public static readonly DependencyProperty SelectedHeadersProperty = DependencyProperty.Register(
        //     "SelectedHeaders", typeof(IList<ColumnHeader>), typeof(SortControl),
        //     new PropertyMetadata(default(IList<ColumnHeader>)));
        //
        // public IList<ColumnHeader> SelectedHeaders
        // {
        //     get => (IList<ColumnHeader>)GetValue(SelectedHeadersProperty);
        //     set => SetValue(SelectedHeadersProperty, value);
        // }

        private IList<ColumnHeader> SelectedHeaders { get; } = new List<ColumnHeader>();

        public static readonly DependencyProperty SortColumnHeadersProperty = DependencyProperty.Register(
            "SortColumnHeaders", typeof(IList<SortColumnHeader>), typeof(SortControl),
            new PropertyMetadata(default(IList<SortColumnHeader>)));

        public IList<SortColumnHeader> SortColumnHeaders
        {
            get => (IList<SortColumnHeader>)GetValue(SortColumnHeadersProperty);
            set => SetValue(SortColumnHeadersProperty, value);
        }

        public SortControl()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddSelectedColumn();
        }

        // BUG: Somehow a null column was added in between two valid sort columns.
        // Steps to reproduce.
        // Add new sorting column
        // Change column name of first sorting column.
        private void AddSelectedColumn()
        {
            foreach (UIElement element in LayoutParent.Children)
            {
                if (element is SortOptionControl sortOptionControl)
                {
                    if (!SelectedHeaders.Contains(sortOptionControl.SelectedHeader))
                    {
                        if (sortOptionControl.SelectedHeader == null)
                            throw new ArgumentNullException("Error");

                        SelectedHeaders.Add(sortOptionControl.SelectedHeader);
                        SortColumnHeaders.Add(sortOptionControl.SortColumnHeader);
                    }
                }
            }
        }

        private void RemoveSelectedColumn(SortOptionControl control)
        {
            SelectedHeaders.Remove(control.SelectedHeader);
            SortColumnHeaders.Remove(control.SortColumnHeader);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var sortOption = new SortOptionControl
            {
                Headers = new BindingList<ColumnHeader>(Headers.ToList())
            };

            sortOption.ComboBox.SelectionChanged += SelectionChanged;

            LayoutParent.Children.Add(sortOption);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (LayoutParent.Children.Count > 0)
            {
                if (LayoutParent.Children[LayoutParent.Children.Count - 1] is SortOptionControl lastSortOption)
                {
                    lastSortOption.ComboBox.SelectionChanged -= SelectionChanged;
                    RemoveSelectedColumn(lastSortOption);

                    LayoutParent.Children.Remove(lastSortOption);
                }
            }
        }
    }
}
