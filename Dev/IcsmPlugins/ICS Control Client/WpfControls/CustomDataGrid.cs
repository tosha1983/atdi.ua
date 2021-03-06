﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;

namespace XICSM.ICSControlClient.WpfControls
{
    public class CustomDataGrid : DataGrid
    {
        private long[] _selectedItemsIndexes;
        private bool _IsChangeSelected = false;

        public CustomDataGrid()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }
        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGrid), new FrameworkPropertyMetadata(default(IList), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPropertyChanged)));
        public static DependencyProperty SelectedItemsIndexesProperty = DependencyProperty.Register("SelectedItemsIndexes", typeof(long[]), typeof(CustomDataGrid), new FrameworkPropertyMetadata(default(long[]), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomDataGrid grd = sender as CustomDataGrid;
            if (e.Property == SelectedItemsIndexesProperty)
                grd.SelectedItemsIndexes = (long[])e.NewValue;
        }
    }
}
