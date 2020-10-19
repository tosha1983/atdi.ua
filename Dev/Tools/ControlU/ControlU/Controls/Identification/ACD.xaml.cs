using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ControlU.Equipment;
using System.Reflection;
using System.Windows.Controls.Primitives;

namespace ControlU.Controls.Identification
{
    /// <summary>
    /// Логика взаимодействия для ACD.xaml
    /// </summary>
    public partial class ACD : UserControl, INotifyPropertyChanged
    {
        IList CollectionSource { get; set; }
        ObservableCollection<Filter> Filters = new ObservableCollection<Filter>();
        Type TypeCollectionSource { get; set; }
        Filter selected = new Filter() { };
        GroupFilter gf = new GroupFilter();
        ListCollectionView View;// = CollectionViewSource.GetDefaultView(MainWindow.students.items) as ListCollectionView;
        private readonly object _itemsLock = new object();
        ObservableCollection<Header> Headers = new ObservableCollection<Header>() { };


        public ACD()
        {
            BindingOperations.EnableCollectionSynchronization(IdentificationData.ACD.ACDData, _itemsLock);
            CollectionSource = IdentificationData.ACD.ACDData;
            //IdentificationData.CDMA.BTS.CollectionChanged += CollectionChanged;
            View = new ListCollectionView(CollectionSource);
            TypeCollectionSource = CollectionSource.GetType().GetGenericArguments().Single();

            Filters.Add(new Filter() { Name = "Freq", Method = FreqFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("Freq") });
            Filters.Add(new Filter() { Name = "BW", Method = BWFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("BW") });
            Filters.Add(new Filter() { Name = "RSSI", Method = RSSIFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("RSSI") });// (new Student().StudentCity).GetType() });
            Filters.Add(new Filter() { Name = "MCC", Method = MCCFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("MCC") });
            Filters.Add(new Filter() { Name = "MNC", Method = MNCFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("MNC") });
            Filters.Add(new Filter() { Name = "TechStr", Method = TechStrFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("TechStr") });
            Filters.Add(new Filter() { Name = "Band", Method = BandFound, Type = TypeCollectionSource, TypeFiltered = TypeCollectionSource.GetProperty("Band") });

            InitializeComponent();
            DataContext = this;
            DG_Data.ItemsSource = View;


            //CDMAData_DG.DataContext = IdentificationData.CDMA;
            ACDHeader_gd.DataContext = IdentificationData.ACD;
            DeleteAllFilters_btn.DataContext = this;
            GetHeaders(DG_Data);
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
        private void ACDIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            Equipment.IdentificationData.ACD.TechIsEnabled = !Equipment.IdentificationData.ACD.TechIsEnabled;
            App.Sett.SaveTSMxReceiver();
        }
        #region Filter Methods
        bool FreqFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool BWFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool RSSIFound(object d)
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
        bool TechStrFound(object d)
        {
            string s = MethodInfo.GetCurrentMethod().Name.Replace("Found", "");
            return Found(d, s);
        }
        bool BandFound(object d)
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
                    foreach (object st in DG_Data.ItemsSource.OfType<ACD_Data>().ToList())//DGData.Items) // studentView.Cast<Student>().ToList())
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
                        for (int g = 0; g < IdentificationData.ACD.ACDData.Count(); g++) //(var st in IdentificationData.CDMA.BTS)
                        {
                            if (find == false && fil.TypeFiltered.GetValue(IdentificationData.ACD.ACDData[g], null).Equals(fil.Value[i].Item))
                            { find = true; }
                        }
                        if (find == false)
                        { fil.Value.RemoveAt(i); i--; }
                    }
                    //добавляем новые
                    for (int g = 0; g < IdentificationData.ACD.ACDData.Count(); g++)
                    {
                        bool find = false;
                        for (int f = 0; f < fil.Value.Count; f++) //(var st in IdentificationData.CDMA.BTS)
                        {
                            if (find == false && fil.TypeFiltered.GetValue(IdentificationData.ACD.ACDData[g], null).Equals(fil.Value[f].Item))
                            { find = true; }
                        }
                        if (find == false)
                        {
                            fil.Value.Add(new CheckedListItem<object> { Item = fil.TypeFiltered.GetValue(IdentificationData.ACD.ACDData[g], null), IsChecked = AllSelected(fil.Value), IsEnabled = true });
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
