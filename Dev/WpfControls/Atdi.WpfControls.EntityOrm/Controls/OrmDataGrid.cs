using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Media;

namespace Atdi.WpfControls.EntityOrm.Controls
{
    public class OrmDataGrid : DataGrid
    {
        private long[] _selectedItemsIndexes;
        private bool _IsChangeSelected = false;

        public OrmDataGrid()
        {
            this.SelectionChanged += OrmDataGrid_SelectionChanged;
            this.Loaded += OrmDataGrid_Loaded;

            this.VerticalGridLinesBrush = new SolidColorBrush(Colors.LightGray);  //"DarkGray" 
            this.HorizontalGridLinesBrush = new SolidColorBrush(Colors.LightGray);
            this.FontFamily = new FontFamily("Segoe UI");
            this.FontSize = 12;
            this.HeadersVisibility = DataGridHeadersVisibility.Column;

        }

        void OrmDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectedIndex = -1;
        }
        void OrmDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_IsChangeSelected)
                this.SetSelectedItemsList(this.SelectedItems);
        }

        public IList GetSelectedItemsList()
        { return (IList)GetValue(SelectedItemsListProperty); }
        public void SetSelectedItemsList(IList value)
        {
            if (!_IsChangeSelected)
                SetValue(SelectedItemsListProperty, value);
        }
        public long[] SelectedItemsIndexes
        {
            get { return this._selectedItemsIndexes; }
            set
            {
                this._selectedItemsIndexes = value;
                if (this._selectedItemsIndexes != null)
                {
                    _IsChangeSelected = true;
                    this.SelectedItems.Clear();
                    long index = 0;
                    foreach (var item in this.Items)
                    {
                        if (this._selectedItemsIndexes.Contains(index))
                            this.SelectedItems.Add(item);
                        index++;
                    }
                    _IsChangeSelected = false;
                }
            }
        }

        public static DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(OrmDataGrid), new FrameworkPropertyMetadata(default(IList), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty SelectedItemsIndexesProperty = DependencyProperty.Register("SelectedItemsIndexes", typeof(long[]), typeof(OrmDataGrid), new FrameworkPropertyMetadata(default(long[]), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            OrmDataGrid grd = sender as OrmDataGrid;
            if (e.Property == SelectedItemsIndexesProperty)
                grd.SelectedItemsIndexes = (long[])e.NewValue;
        }
    }
}
