using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Forms;
using Atdi.Platform.Logging;
using System.Windows.Forms.Integration;
using System.Configuration;

namespace Atdi.Icsm.Plugins.Core
{
    public partial class ViewForm : Form
	{
		public readonly ElementHost _wpfElementHost;
		private readonly ViewBase _viewObject;
	    private readonly ViewStarter _starter;
	    private readonly ILogger _logger;

	    public ViewForm(string xamlFilePath, ViewBase viewObject, string caption, ViewStarter starter, ILogger logger)
	    {
		    _viewObject = viewObject;
		    _starter = starter;
		    _logger = logger;

		    InitializeComponent();

		    if (null == System.Windows.Application.Current)
		    {
			    new System.Windows.Application();
		    }

		    _wpfElementHost = new ElementHost
		    {
			    Dock = DockStyle.Fill
		    };
		    this.Controls.Add(_wpfElementHost);

		    // не очень уверен в этом коде, нужно будет разобраться
		    // Тоха им инициализировал формы, но его достатчоно выполнить один раз
		    var appSettings = ConfigurationManager.AppSettings;
		    string currentUICulture = appSettings["UICulture"];
		    if (!string.IsNullOrEmpty(currentUICulture))
			    System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(currentUICulture);

			// загрузка XAML
			var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
		    var fileName = Path.Combine(appFolder, xamlFilePath);
		    base.Text = caption;
		    using (var fileStream = new FileStream(fileName, FileMode.Open))
		    {
			    this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
			    (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = viewObject;
		    }

		    //this.FormClosing += ThisFormClosing;
		    //this.Load += ThisFormLoad;
		}


		protected override void OnFormClosed(FormClosedEventArgs e)
	    {
		    _starter?.Close(this);
		    var disposable = _viewObject as IDisposable;
		    disposable?.Dispose();
	    }

		//void ThisFormClosing(object sender, FormClosingEventArgs e)
		//{
		//    foreach (var spl in FindVisualChildren<CTR.GridSplitter>(_wpfElementHost.Child))
		//    {
		//        var key = this.Name + "/" + GetSplitterKey(spl);

		//        var grd = VisualTreeHelper.GetParent(spl);
		//        Int32.TryParse(spl.GetValue(CTR.Grid.RowProperty).ToString(), out int splRow);
		//        Int32.TryParse(spl.GetValue(CTR.Grid.ColumnProperty).ToString(), out int splCol);

		//        if (grd is CTR.Grid)
		//        {
		//            var grid = grd as CTR.Grid;

		//            if (splRow > 0 && grid.RowDefinitions.Count > splRow && grid.RowDefinitions[splRow - 1].Height.Value > 0 && grid.RowDefinitions[splRow + 1].Height.Value > 0)
		//            {
		//                SaveSetting(key + "/R1/Type", grid.RowDefinitions[splRow - 1].Height.GridUnitType.ToString());
		//                SaveSetting(key + "/R1/Value", grid.RowDefinitions[splRow - 1].Height.Value.ToString());
		//                SaveSetting(key + "/R2/Type", grid.RowDefinitions[splRow + 1].Height.GridUnitType.ToString());
		//                SaveSetting(key + "/R2/Value", grid.RowDefinitions[splRow + 1].Height.Value.ToString());
		//            }
		//            if (splCol > 0 && grid.ColumnDefinitions.Count > splCol && grid.ColumnDefinitions[splCol - 1].Width.Value > 0 && grid.ColumnDefinitions[splCol + 1].Width.Value > 0)
		//            {
		//                SaveSetting(key + "/C1/Type", grid.ColumnDefinitions[splCol - 1].Width.GridUnitType.ToString());
		//                SaveSetting(key + "/C1/Value", grid.ColumnDefinitions[splCol - 1].Width.Value.ToString());
		//                SaveSetting(key + "/C2/Type", grid.ColumnDefinitions[splCol + 1].Width.GridUnitType.ToString());
		//                SaveSetting(key + "/C2/Value", grid.ColumnDefinitions[splCol + 1].Width.Value.ToString());
		//            }
		//        }
		//    }
		//    Properties.Settings.Default.Save();
		//}
		//void ThisFormLoad(object sender, EventArgs e)
		//{
		//    this.InitializeSplitters();
		//}
		//void InitializeSplitters()
		//{
		//    foreach (var spl in FindVisualChildren<CTR.GridSplitter>(_wpfElementHost.Child))
		//    {
		//        var key = this.Name + "/" + GetSplitterKey(spl);
		//        var grd = VisualTreeHelper.GetParent(spl);
		//        Int32.TryParse(spl.GetValue(CTR.Grid.RowProperty).ToString(), out int splRow);
		//        Int32.TryParse(spl.GetValue(CTR.Grid.ColumnProperty).ToString(), out int splCol);

		//        if (grd is CTR.Grid)
		//        {
		//            var grid = grd as CTR.Grid;
		//            if (splRow > 0)
		//            {
		//                var stringType = LoadSetting(key + "/R1/Type");
		//                var stringVal = LoadSetting(key + "/R1/Value");
		//                if (!string.IsNullOrEmpty(stringType) && !string.IsNullOrEmpty(stringVal))
		//                {
		//                    Enum.TryParse<GridUnitType>(stringType, out GridUnitType type);
		//                    double.TryParse(stringVal, out double val);
		//                    if (val > 0)
		//                        grid.RowDefinitions[splRow - 1].Height = new GridLength(val, type);
		//                }
		//            }
		//            if (splRow > 0 && grid.RowDefinitions.Count > splRow)
		//            {
		//                var stringType = LoadSetting(key + "/R2/Type");
		//                var stringVal = LoadSetting(key + "/R2/Value");
		//                if (!string.IsNullOrEmpty(stringType) && !string.IsNullOrEmpty(stringVal))
		//                {
		//                    Enum.TryParse<GridUnitType>(stringType, out GridUnitType type);
		//                    double.TryParse(stringVal, out double val);
		//                    if (val > 0)
		//                        grid.RowDefinitions[splRow + 1].Height = new GridLength(val, type);
		//                }
		//            }
		//            if (splCol > 0)
		//            {
		//                var stringType = LoadSetting(key + "/C1/Type");
		//                var stringVal = LoadSetting(key + "/C1/Value");
		//                if (!string.IsNullOrEmpty(stringType) && !string.IsNullOrEmpty(stringVal))
		//                {
		//                    Enum.TryParse<GridUnitType>(stringType, out GridUnitType type);
		//                    double.TryParse(stringVal, out double val);
		//                    if (val > 0)
		//                        grid.ColumnDefinitions[splCol - 1].Width = new GridLength(val, type);
		//                }
		//            }
		//            if (splCol > 0 && grid.ColumnDefinitions.Count > splCol)
		//            {
		//                var stringType = LoadSetting(key + "/C2/Type");
		//                var stringVal = LoadSetting(key + "/C2/Value");
		//                if (!string.IsNullOrEmpty(stringType) && !string.IsNullOrEmpty(stringVal))
		//                {
		//                    Enum.TryParse<GridUnitType>(stringType, out GridUnitType type);
		//                    double.TryParse(stringVal, out double val);
		//                    if (val > 0)
		//                        grid.ColumnDefinitions[splCol + 1].Width = new GridLength(val, type);
		//                }
		//            }
		//        }
		//    }
		//}
		//void SaveSetting(string key, string val)
		//{
		//    var property = new SettingsProperty(Properties.Settings.Default.Properties["baseSetting"]);
		//    property.Name = key;
		//    try
		//    {
		//        Properties.Settings.Default.Properties.Add(property);
		//    }
		//    catch (Exception)
		//    {
		//    }
		//    Properties.Settings.Default[key] = val;
		//}
		//string LoadSetting(string key)
		//{
		//    var property = new SettingsProperty(Properties.Settings.Default.Properties["baseSetting"]);
		//    property.Name = key;
		//    try
		//    {
		//        Properties.Settings.Default.Properties.Add(property);
		//    }
		//    catch (Exception)
		//    {
		//    }
		//    return Properties.Settings.Default[key].ToString();
		//}
		//string GetSplitterKey(CTR.GridSplitter splitter)
		//{
		//    return GetTreeKey(splitter) + "/GridSplitter/" + splitter.HorizontalAlignment.ToString() + "/" + splitter.VerticalAlignment.ToString() + "/" + splitter.GetValue(CTR.Grid.RowProperty) + "/" + splitter.GetValue(CTR.Grid.ColumnProperty);
		//}

		//#region VisualTree
		//static string GetTreeKey(DependencyObject depObj)
		//{
		//    string key = "";
		//    if (depObj != null)
		//    {
		//        var obj = VisualTreeHelper.GetParent(depObj);
		//        if (obj != null)
		//            key = GetTreeKey(obj) + "/" + obj.DependencyObjectType.Name;
		//    }
		//    return key;
		//}
		//public List<T> GetVisualChildCollection<T>(object parent) where T : Visual
		//{
		//    List<T> visualCollection = new List<T>();
		//    GetVisualChildCollection(parent as DependencyObject, visualCollection);
		//    return visualCollection;
		//}
		//private void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
		//{
		//    int count = VisualTreeHelper.GetChildrenCount(parent);
		//    for (int i = 0; i < count; i++)
		//    {
		//        DependencyObject child = VisualTreeHelper.GetChild(parent, i);
		//        if (child is T)
		//            visualCollection.Add(child as T);
		//        else if (child != null)
		//            GetVisualChildCollection(child, visualCollection);
		//    }
		//}
		//public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		//{
		//    if (depObj != null)
		//    {
		//        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
		//        {
		//            DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
		//            if (child != null && child is T)
		//                yield return (T)child;

		//            foreach (T childOfChild in FindVisualChildren<T>(child))
		//                yield return childOfChild;
		//        }
		//    }
		//}
		//#endregion
	}
}
