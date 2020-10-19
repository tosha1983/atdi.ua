using System;
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

namespace ControlU.Controls.Meas
{
    /// <summary>
    /// Логика взаимодействия для MeasMonData.xaml
    /// </summary>
    public partial class MeasMonData : UserControl, INotifyPropertyChanged
    {
        IList CollectionSource { get; set; }
        ObservableCollection<Filter> Filters = new ObservableCollection<Filter>();
        Type TypeCollectionSource { get; set; }
        Filter selected = new Filter() { };
        GroupFilter gf = new GroupFilter();
        ListCollectionView View;
        private readonly object _itemsLock = new object();
        private DB.MeasData _DG_DataSelectedItem = new DB.MeasData() { };
        public DB.MeasData DG_DataSelectedItem
        {
            get { return _DG_DataSelectedItem; }
            set { _DG_DataSelectedItem = value; OnPropertychanged("DG_DataSelectedItem"); }
        }
        public MeasMonData()
        {
            BindingOperations.EnableCollectionSynchronization(MainWindow.db_v2.MeasMon.Data, _itemsLock);
            CollectionSource = MainWindow.db_v2.MeasMon.Data;
            View = new ListCollectionView(CollectionSource);
            TypeCollectionSource = CollectionSource.GetType().GetGenericArguments().Single();

            Filters.Add(new Filter() { Name = "ATDI_FrequencyPermission", Method = ATDI_FrequencyPermissionFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("ATDI_FrequencyPermission") });
            Filters.Add(new Filter() { Name = "ATDI_GCID", Method = ATDI_GCIDFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("ATDI_GCID") });
            Filters.Add(new Filter() { Name = "StandartSubband", Method = StandartSubbandFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("StandartSubband") });
            Filters.Add(new Filter() { Name = "ThisToMeas", Method = ThisToMeasFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("ThisToMeas") });
            Filters.Add(new Filter() { Name = "FreqDN", Method = FreqDNFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("FreqDN") });
            Filters.Add(new Filter() { Name = "GCID", Method = GCIDFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("GCID") });
            Filters.Add(new Filter() { Name = "UCRFGCID", Method = UCRFGCIDFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("UCRFGCID") });
            Filters.Add(new Filter() { Name = "PlanFreq_ID", Method = PlanFreq_IDFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("PlanFreq_ID") });
            Filters.Add(new Filter() { Name = "TraceCount", Method = TraceCountFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("TraceCount") });
            Filters.Add(new Filter() { Name = "AllTraceCount", Method = AllTraceCountFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("AllTraceCount") });
            Filters.Add(new Filter() { Name = "AllTraceCountToMeas", Method = AllTraceCountToMeasFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("AllTraceCountToMeas") });
            Filters.Add(new Filter() { Name = "MeasTime", Method = MeasTimeFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("MeasTime") });
            Filters.Add(new Filter() { Name = "ChanelBWMeasured", Method = ChanelBWMeasuredFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("ChanelBWMeasured") });
            Filters.Add(new Filter() { Name = "LastMeasDateTime", Method = LastMeasDateTimeFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("LastMeasDateTime") });
            Filters.Add(new Filter() { Name = "DeltaFreqMeasured", Method = DeltaFreqMeasuredFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("DeltaFreqMeasured") });
            InitializeComponent();
            DataContext = this;
            AllCount.DataContext = MainWindow.db_v2.MeasMon;
            info_grid.DataContext = MainWindow.db_v2.MeasMon;
            DG_Data.ItemsSource = View;
            InitializeComponent();
            //DG_Data.DataContext = MainWindow.db_v2;
            //GSMBandMeass_DG.DataContext = MainWindow.db_v2;
        }
        #region Filter Methods
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
        bool StandartSubbandFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool ThisToMeasFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool FreqDNFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool GCIDFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool UCRFGCIDFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool PlanFreq_IDFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool TraceCountFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool AllTraceCountFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool AllTraceCountToMeasFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool MeasTimeFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool ChanelBWMeasuredFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool LastMeasDateTimeFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool DeltaFreqMeasuredFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        #endregion

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

        #region btn
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
                    foreach (object st in DG_Data.ItemsSource.OfType<DB.MeasData>().ToList())//DGData.Items) // studentView.Cast<Student>().ToList())
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
                        for (int m = 0; m < MainWindow.db_v2.MeasMon.Data.Count; m++) //(var st in IdentificationData.GSM.BTS)
                        {
                            if (find == false && fil.TypeFiltered.GetValue(MainWindow.db_v2.MeasMon.Data[m], null).Equals(fil.Value[i].Item))
                            { find = true; }
                        }
                        if (find == false)
                        { fil.Value.RemoveAt(i); i--; }
                    }
                    //добавляем новые
                    for (int m = 0; m < MainWindow.db_v2.MeasMon.Data.Count; m++)
                    {
                        bool find = false;
                        for (int f = 0; f < fil.Value.Count; f++) //(var st in IdentificationData.GSM.BTS)
                        {
                            if (find == false && fil.TypeFiltered.GetValue(MainWindow.db_v2.MeasMon.Data[m], null).Equals(fil.Value[f].Item))
                            { find = true; }
                        }
                        if (find == false)
                        {
                            fil.Value.Add(new CheckedListItem<object> { Item = fil.TypeFiltered.GetValue(MainWindow.db_v2.MeasMon.Data[m], null), IsChecked = AllSelected(fil.Value), IsEnabled = true });
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
            Filter f = Filters.First(y => y.Name.Replace(".", "") == name);
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

        private void MonDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG_Data.SelectedIndex >= 0)
            {
                DG_DataSelectedItem = (DB.MeasData)DG_Data.SelectedItem;
                //MobileMonSpectrum_uc.SpectrumFromReceiver = false;
                //MobileMonSpectrum_uc.tomeas = smd;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertychanged(string pName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pName));
            }
        }
    }
}
