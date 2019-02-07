using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ControlU.Settings;
using System.Collections.ObjectModel;

namespace ControlU.Controls.Map
{
    /// <summary>
    /// Логика взаимодействия для ATDI_TooltipStation.xaml
    /// </summary>
    public partial class ATDI_TooltipStation : UserControl
    {
        public ATDI_TooltipStation()
        {
            InitializeComponent();
            GetHeaders(Data);
        }
        private void GetHeaders(DataGrid Grid)
        {
            foreach (DataGridColumn column in Grid.Columns)
            {
                Binding binding = column.ClipboardContentBinding as System.Windows.Data.Binding;
                if (binding == null && column is DataGridTextColumn)
                {
                    DataGridTextColumn tc = (DataGridTextColumn)column;
                    Setter st = (Setter)tc.ElementStyle.Setters[0];
                    binding = (Binding)st.Value;
                }
                if (column.Header is string)
                {
                    bool find = false;
                    for (int i = 0; i < App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu.Count; i++)
                    {
                        if (App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu[i].VariableName == binding.Path.Path)
                        {
                            if (App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu[i].Visible == true)
                            {
                                column.Visibility = Visibility.Visible;
                            }
                            else column.Visibility = Visibility.Collapsed;
                            if (find == false)
                            {
                                App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu[i].Name = column.Header.ToString();
                                App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu[i].col = column;
                                find = true;
                            }
                        }
                    }
                    if (find == false)
                    {
                        App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu.Add(new DataGridHeaderVisibility() { Name = column.Header.ToString(), VariableName = binding.Path.Path, Visible = true, col = column });
                    }
                }
                //else if (column.Header is Grid)
                //{
                //    Grid gr = (Grid)column.Header;
                //    TextBlock tb = (TextBlock)gr.Children[0];
                //    App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu.Add(new DataGridHeaderVisibility() { Name = tb.Text, Visible = true, col = column });
                //}
            }
            for (int i = 0; i < App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu.Count; i++)
            {
                if (App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu[i].col == null) { App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu.RemoveAt(i); i--; }
            }
            App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu = new ObservableCollection<DataGridHeaderVisibility>(App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu.OrderBy(i => i.Name));
            //object obj = Grid.FindResource("ColumnChooserMenu");
            //ContextMenu m = (ContextMenu)obj;
            //m.DataContext = App.Sett.Map_Settings;
            //m.ItemsSource = App.Sett.Map_Settings.ATDI_TooltipColumnChooserMenu;
            //Grid.Resources["ColumnChooserMenu"] = m;
        }
    }
}
