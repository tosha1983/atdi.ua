﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ControlU.Controls;
using ControlU.Equipment;

namespace ControlU.Controls.Identification
{
    /// <summary>
    /// Логика взаимодействия для UMTS.xaml
    /// </summary>
    public partial class UMTS : UserControl
    {
        IList CollectionSource { get; set; }
        ObservableCollection<Filter> Filters = new ObservableCollection<Filter>();
        Type TypeCollectionSource { get; set; }
        Filter selected = new Filter() { };
        GroupFilter gf = new GroupFilter();
        ListCollectionView View;// = CollectionViewSource.GetDefaultView(MainWindow.students.items) as ListCollectionView;
        private readonly object _itemsLock = new object();
        ObservableCollection<Header> Headers = new ObservableCollection<Header>() { };

        public UMTSBTSData BTSSelectedItem
        {
            get { return _BTSSelectedItem; }
            set { _BTSSelectedItem = value; OnPropertyChanged("BTSSelectedItem"); }
        }
        private UMTSBTSData _BTSSelectedItem;
        public UMTS()
        {
            BindingOperations.EnableCollectionSynchronization(IdentificationData.UMTS.BTS, _itemsLock);
            CollectionSource = IdentificationData.UMTS.BTS;
            //IdentificationData.UMTS.BTS.CollectionChanged += CollectionChanged;
            View = new ListCollectionView(CollectionSource);
            TypeCollectionSource = CollectionSource.GetType().GetGenericArguments().Single();

            Filters.Add(new Filter() { Name = "UARFCN_DN", Method = UARFCN_DNFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("UARFCN_DN") });
            Filters.Add(new Filter() { Name = "FreqDn", Method = FreqDnFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("FreqDn") });
            Filters.Add(new Filter() { Name = "StandartSubband", Method = StandartSubbandFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("StandartSubband") });// (new Student().StudentCity).GetType() });
            Filters.Add(new Filter() { Name = "SC", Method = SCFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("SC") });
            Filters.Add(new Filter() { Name = "MCC", Method = MCCFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("MCC") });
            Filters.Add(new Filter() { Name = "MNC", Method = MNCFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("MNC") });
            Filters.Add(new Filter() { Name = "LAC", Method = LACFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("LAC") });
            Filters.Add(new Filter() { Name = "CID", Method = CIDFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("CID") });
            Filters.Add(new Filter() { Name = "UCID", Method = UCIDFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("UCID") });
            Filters.Add(new Filter() { Name = "CIDToDB", Method = CIDToDBFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("CIDToDB") });
            Filters.Add(new Filter() { Name = "RNC", Method = RNCFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("RNC") });
            Filters.Add(new Filter() { Name = "GCID", Method = GCIDFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("GCID") });
            Filters.Add(new Filter() { Name = "ATDI_FrequencyPermission", Method = ATDI_FrequencyPermissionFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("ATDI_FrequencyPermission") });
            Filters.Add(new Filter() { Name = "ATDI_GCID", Method = ATDI_GCIDFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("ATDI_GCID") });
            Filters.Add(new Filter() { Name = "RS135_Freq", Method = RS135_FreqFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("RS135_Freq") });
            Filters.Add(new Filter() { Name = "RS135_GCIDFromDB", Method = RS135_GCIDFromDBFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("RS135_GCIDFromDB") });
            InitializeComponent();
            DataContext = this;
            DG_Data.ItemsSource = View;


            //UMTSData_DG.DataContext = IdentificationData.UMTS;
            UMTSHeader_gd.DataContext = IdentificationData.UMTS;
            DeleteAllFilters_btn.DataContext = this;
            GetHeaders(DG_Data);
        }
        private void DG_Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BTSSelectedItem = (UMTSBTSData)((DataGrid)sender).SelectedItem;
        }
        private void GetHeaders(DataGrid Grid)
        {
            foreach (DataGridColumn column in Grid.Columns)
            {
                if (column.Header is string) Headers.Add(new Header() { Name = column.Header.ToString(), Visible = true, col = column });
                else if (column.Header is Grid)
                {
                    Grid gr = (Grid)column.Header;
                    TextBlock tb = (TextBlock)gr.Children[0];
                    Headers.Add(new Header() { Name = tb.Text, Visible = true, col = column });
                }
            }
            object obj = Grid.FindResource("ColumnChooserMenu");
            ContextMenu m = (ContextMenu)obj;
            m.DataContext = this;
            m.ItemsSource = Headers;
            Grid.Resources["ColumnChooserMenu"] = m;
        }
        private void UMTSIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            Equipment.IdentificationData.UMTS.TechIsEnabled = !Equipment.IdentificationData.UMTS.TechIsEnabled;
            App.Sett.SaveTSMxReceiver();
        }
        #region Filter Methods
        bool UARFCN_DNFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool FreqDnFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool StandartSubbandFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool SCFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool MCCFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool MNCFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool LACFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool CIDFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool UCIDFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool CIDToDBFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool RNCFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool GCIDFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool ATDI_FrequencyPermissionFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool ATDI_GCIDFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool RS135_FreqFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool RS135_GCIDFromDBFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        #endregion
        #region btn
        private void DeleteAllFilters_Click(object sender, RoutedEventArgs e)
        {
            gf.RemoveAllFilter();
            for (int f = 0; f < Filters.Count; f++)
            {
                for (int v = 0; v < Filters[f].Value.Count; v++)
                {
                    Filters[f].Value[v].IsChecked = true;
                    Filters[f].Value[v].IsEnabled = true;
                }
            }
            ((ListCollectionView)CollectionViewSource.GetDefaultView(DG_Data.ItemsSource)).Filter = gf.Filter;
            DeleteAllFilters_btn.Visibility = Visibility.Collapsed;
        }
        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ListBox lb = FindAncestor<ListBox>(cb);
            StackPanel sp = (StackPanel)lb.Parent;
            Border br = (Border)sp.Parent;
            Popup pp = (Popup)br.Parent;

            Button b = (Button)pp.PlacementTarget;
            Grid gr = (Grid)b.Parent;
            DataGridColumnHeader dgch = FindAncestor<DataGridColumnHeader>(gr);
            Binding binding = dgch.Column.ClipboardContentBinding as System.Windows.Data.Binding;
            //StackPanel sp2 = (StackPanel)b.Parent;

            selected = Filters.First(y => y.Name == binding.Path.Path);
            if (selected != null)
            {
                View = CollectionViewSource.GetDefaultView(DG_Data.ItemsSource) as ListCollectionView;
                View.IsLiveFiltering = true;
                View.LiveFilteringProperties.Remove(selected.Name);
                View.LiveFilteringProperties.Add(selected.Name);
                selected.Value = new ObservableCollection<CheckedListItem<object>>(lb.Items.OfType<CheckedListItem<object>>().ToList());
                if (selected.Value.Count(w => w.IsChecked == false) > 0) { ((Path)b.Content).Fill = new SolidColorBrush(Color.FromArgb(255, (byte)70, (byte)150, (byte)204)); }
                else { ((Path)b.Content).Fill = null; }
                gf.RemoveFilter(selected.Method);
                gf.AddFilter(selected.Method);
                View.Filter = gf.Filter;
            }
            if (Filters.Count(w => w.AnyIsChecked == false) > 0) { DeleteAllFilters_btn.Visibility = Visibility.Visible; }
            else { DeleteAllFilters_btn.Visibility = Visibility.Collapsed; }
            #region udate IsEnabled 
            foreach (Filter fil in Filters)
            {
                foreach (CheckedListItem<object> val in fil.Value)
                {
                    bool find = false;
                    foreach (object st in DG_Data.ItemsSource.OfType<UMTSBTSData>().ToList())//DGData.Items) // studentView.Cast<Student>().ToList())
                    {
                        if (fil.TypeFiltered.GetValue(st, null).Equals(val.Item)) find = true;
                    }
                    val.IsEnabled = find;
                    //val.IsChecked = find;
                }
                fil.Value = new ObservableCollection<CheckedListItem<object>>(fil.Value.OrderByDescending(w => w.IsEnabled).ThenBy((w => w.Item)));
            }
            #endregion
        }
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Grid sp = (Grid)b.Parent;
            DataGridColumnHeader dgch = FindAncestor<DataGridColumnHeader>(sp);
            Binding binding = dgch.Column.ClipboardContentBinding as System.Windows.Data.Binding;

            foreach (Filter fil in Filters)
            {
                if (fil.Name == binding.Path.Path)
                {
                    for (int i = 0; i < fil.Value.Count; i++)
                    {
                        bool find = false;
                        for (int g = 0; g < IdentificationData.UMTS.BTS.Count; g++) //(var st in IdentificationData.UMTS.BTS)
                        {
                            if (find == false && fil.TypeFiltered.GetValue(IdentificationData.UMTS.BTS[g], null).Equals(fil.Value[i].Item))
                            { find = true; }
                        }
                        if (find == false)
                        { fil.Value.RemoveAt(i); i--; }
                    }
                    //добавляем новые
                    for (int g = 0; g < IdentificationData.UMTS.BTS.Count; g++)
                    {
                        bool find = false;
                        for (int f = 0; f < fil.Value.Count; f++) //(var st in IdentificationData.UMTS.BTS)
                        {
                            if (find == false && fil.TypeFiltered.GetValue(IdentificationData.UMTS.BTS[g], null).Equals(fil.Value[f].Item))
                            { find = true; }
                        }
                        if (find == false)
                        {
                            fil.Value.Add(new CheckedListItem<object> { Item = fil.TypeFiltered.GetValue(IdentificationData.UMTS.BTS[g], null), IsChecked = AllSelected(fil.Value), IsEnabled = true });
                        }
                    }
                    fil.Value = new ObservableCollection<CheckedListItem<object>>(fil.Value.OrderByDescending(w => w.IsEnabled).ThenBy((w => w.Item)));
                    selected = fil;
                }
            }
            ///selected = Filters.First(y => y.Name == binding.Path.Path);
            if (selected != null)
            { lstFilters.ItemsSource = selected.Value; }
            filter_popup.PlacementTarget = b;
            filter_popup.IsOpen = true;
        }
        //uni
        private void PopUpAllSelection_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            DockPanel dp = (DockPanel)b.Parent;
            StackPanel sp2 = (StackPanel)dp.Parent;
            for (int i = 0; i < sp2.Children.OfType<ListBox>().ToArray()[0].Items.Count; i++)
            {
                if (b.Tag.ToString() == "SelectAll")
                { ((CheckedListItem<object>)sp2.Children.OfType<ListBox>().ToArray()[0].Items[i]).IsChecked = true; }
                else if (b.Tag.ToString() == "UnSelectAll")
                { ((CheckedListItem<object>)sp2.Children.OfType<ListBox>().ToArray()[0].Items[i]).IsChecked = false; }
            }
            ((ListCollectionView)CollectionViewSource.GetDefaultView(DG_Data.ItemsSource)).Filter = gf.Filter;
        }
        //uni
        private void PopUpSort_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            DockPanel dp = (DockPanel)b.Parent;
            StackPanel sp = (StackPanel)dp.Parent;
            Border br = (Border)sp.Parent;
            Popup pp = (Popup)br.Parent;
            Button b2 = (Button)pp.PlacementTarget;

            Grid gr = (Grid)b2.Parent;
            DataGridColumnHeader dgch = FindAncestor<DataGridColumnHeader>(gr);
            Binding binding = dgch.Column.ClipboardContentBinding as System.Windows.Data.Binding;
            View.SortDescriptions.Clear();
            ListSortDirection lsd = ListSortDirection.Ascending;
            if (b.Tag.ToString() == "UP")
            { lsd = ListSortDirection.Ascending; }
            else if (b.Tag.ToString() == "DOWN")
            { lsd = ListSortDirection.Descending; }
            View.SortDescriptions.Add(new SortDescription(binding.Path.Path, lsd));
        }
        #endregion
        #region helpers
        /// <summary>
        /// если все фильтры выбраны(отображать все) то возвращает тру
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private bool AllSelected(ObservableCollection<CheckedListItem<object>> items)
        {
            bool res = true;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsChecked == false) res = false;
            }
            return res;
        }
        private bool Found(object d, string name)
        {
            bool result = false;
            Filter f = Filters.First(y => y.Name == name);
            var std = Convert.ChangeType(d, f.Type);
            foreach (CheckedListItem<object> val in f.Value)
            {
                if (val.IsChecked == true && f.TypeFiltered.GetValue(std, null).Equals(val.Item))
                {
                    result = true;
                }
            }
            return result;
        }
        public static T FindAncestor<T>(DependencyObject from)
            where T : class
        {
            if (from == null)
            {
                return null;
            }

            T candidate = from as T;
            if (candidate != null)
            {
                return candidate;
            }

            return FindAncestor<T>(VisualTreeHelper.GetParent(from));
        }
        #endregion

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            GUIThreadDispatcher.Instance.Invoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    }
}
