using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MapControl;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media.Effects;

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для GlobalMap.xaml
    /// </summary>
    public partial class GlobalOSMMap : UserControl, INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private int _maxDownload = -1;
        Settings.AnyMap_Set Settings = new ControlU.Settings.AnyMap_Set();
        bool settingLoaded = false;


        public bool ShowMyLocation
        {
            get { return Settings.ShowMyLocation; }
            set { Settings.ShowMyLocation = value; OnPropertyChanged("ShowMyLocation"); }
        }
        public bool AutoPanModeMap
        {
            get { return Settings.AutoPanModeMap; }
            set { Settings.AutoPanModeMap = value; OnPropertyChanged("AutoPanModeMap"); }
        }
        private bool _GetCoorFromMap = false;
        public bool GetCoorFromMap
        {
            get { return _GetCoorFromMap; }
            set { if (_GetCoorFromMap != value) { _GetCoorFromMap = value; OnPropertyChanged("GetCoorFromMap"); } }
        }
        private Point _CoorFromMap = new Point(0, 0);
        public Point CoorFromMap
        {
            get { return _CoorFromMap; }
            set { if (_CoorFromMap != value) { _CoorFromMap = value; OnPropertyChanged("CoorFromMap"); } }
        }
        private string _CoorFromMapX = "";
        public string CoorFromMapX
        {
            get { return _CoorFromMapX; }
            set { if (_CoorFromMapX != value) { _CoorFromMapX = value; OnPropertyChanged("CoorFromMapX"); } }
        }
        private string _CoorFromMapY = "";
        public string CoorFromMapY
        {
            get { return _CoorFromMapY; }
            set { if (_CoorFromMapY != value) { _CoorFromMapY = value; OnPropertyChanged("CoorFromMapY"); } }
        }
        public bool ShowPlanPanelExpander
        {
            get { return Settings.ShowPlanPanelExpander; }
            set { Settings.ShowPlanPanelExpander = value; OnPropertyChanged("ShowPlanPanelExpander"); }
        }
        public bool ShowSelectedInfoPanelExpander
        {
            get { return Settings.ShowSelectedInfoPanelExpander; }
            set { Settings.ShowSelectedInfoPanelExpander = value; OnPropertyChanged("ShowSelectedInfoPanelExpander"); }
        }
        public double SelectedInfoPanelWidth
        {
            get { return Settings.SelectedInfoPanelWidth; }
            set { Settings.SelectedInfoPanelWidth = value; OnPropertyChanged("SelectedInfoPanelWidth"); }
        }
        private bool _ShowMapSettings = false;
        public bool ShowMapSettings
        {
            get { return _ShowMapSettings; }
            set { _ShowMapSettings = value; OnPropertyChanged("ShowMapSettings"); }
        }
        public double MapOpacity
        {
            get { return Settings.MapOpacity; }
            set { Settings.MapOpacity = value; OnPropertyChanged("MapOpacity"); }
        }
        public double PlansLayerOpacity
        {
            get { return Settings.LayerOpacity; }
            set { Settings.LayerOpacity = value; OnPropertyChanged("PlansLayerOpacity"); }
        }
        public double RadiusLayerOpacity
        {
            get { return Settings.RadiusLayerOpacity; }
            set { Settings.RadiusLayerOpacity = value; OnPropertyChanged("RadiusLayerOpacity"); }
        }
        public bool ShowSearchRadiusOnMapState
        {
            get { return Settings.ShowRadiusOnSearchState; }
            set { Settings.ShowRadiusOnSearchState = value; OnPropertyChanged("ShowSearchRadiusOnMapState"); }
        }
        /// <summary>
        /// Скрывать выполненое
        /// </summary>
        public bool HideCompleted
        {
            get { return _HideCompleted; }
            set { _HideCompleted = value; OnPropertyChanged("HideCompleted"); }
        }
        private bool _HideCompleted = false;

        public string ScreenName
        {
            get { return _ScreenName; }
            set { _ScreenName = value; OnPropertyChanged("ScreenName"); }
        }
        string _ScreenName = "";

        Point MouseLeftDownPositionPoint = new Point();

        #region Слои
        public ObservableCollection<UIElement> SelectedSectors
        {
            get { return _SelectedSectors; }
            set { _SelectedSectors = value; OnPropertyChanged("ItemsOnMap"); }
        }
        private ObservableCollection<UIElement> _SelectedSectors = new ObservableCollection<UIElement>() { };
        public Canvas SelectedStation
        {
            get { return _SelectedStation; }
            set { _SelectedStation = value; OnPropertyChanged("SelectedStation"); }
        }
        private Canvas _SelectedStation;
        public Canvas SelectedCluster
        {
            get { return _SelectedCluster; }
            set { _SelectedCluster = value; OnPropertyChanged("SelectedCluster"); }
        }
        private Canvas _SelectedCluster;// = new Canvas();

        public ObservableCollection<Canvas> ItemsOnMap
        {
            get { return _ItemsOnMap; }
            set { _ItemsOnMap = value; OnPropertyChanged("ItemsOnMap"); }
        }
        private ObservableCollection<Canvas> _ItemsOnMap = new ObservableCollection<Canvas>() { };

        #endregion
        #region для отрисовки на карте

        public ObservableCollection<ATDI_DrawTaskInfo> ATDI_TasksToDraw
        {
            get { return _ATDI_TasksToDraw; }
            set { _ATDI_TasksToDraw = value; OnPropertyChanged("ATDI_TasksToDraw"); }
        }
        private ObservableCollection<ATDI_DrawTaskInfo> _ATDI_TasksToDraw = new ObservableCollection<ATDI_DrawTaskInfo>() { };

        public ObservableCollection<ItemOnMap> ATDI_ItemsTasksOnMap
        {
            get { return _ATDI_ItemsPlanOnMap; }
            set { _ATDI_ItemsPlanOnMap = value; OnPropertyChanged("ATDI_ItemsPlanOnMap"); }
        }
        private ObservableCollection<ItemOnMap> _ATDI_ItemsPlanOnMap = new ObservableCollection<ItemOnMap>() { };

        public ObservableCollection<TableInfo> InfoWithRadius
        {
            get { return _InfoWithRadius; }
            set { _InfoWithRadius = value; OnPropertyChanged("InfoWithRadius"); }
        }
        private ObservableCollection<TableInfo> _InfoWithRadius = new ObservableCollection<TableInfo>() { };


        public StrengtItem[,] StrengtMatrix
        {
            get { return _StrengtMatrix; }
            set { _StrengtMatrix = value; OnPropertyChanged("StrengtMatrix"); }
        }
        StrengtItem[,] _StrengtMatrix = new StrengtItem[,] { };

        public double TableRadiusFind
        {
            get { return _TableRadiusFind; }
            set { _TableRadiusFind = value; OnPropertyChanged("TableRadiusFind"); }
        }
        private double _TableRadiusFind = 1000;
        Canvas GPSPositionCanvas;
        Canvas RadiusCentrPointCanvas;
        Canvas RadiusEndPointCanvas;
        Map.DrawCircle RadiusControl;
        Canvas DFBearingCanvas = null;
        #endregion

        #region переменные coor
        private double _LatDD, _LatMM, _LatSS, _LonDD, _LonMM, _LonSS;
        //private double _Lat, _Lon;
        //private System.Windows.Point _Coor;
        //public System.Windows.Point Coor
        //{
        //    get { return _Coor; }
        //    set { if (_Coor != value) { _Coor = value; OnPropertyChanged("Coor"); } }
        //}
        //public double Lat
        //{
        //    get { return _Lat; }
        //    set { if (_Lat != value) { _Lat = value; OnPropertyChanged("Lat"); } }
        //}
        //public double Lon
        //{
        //    get { return _Lon; }
        //    set { if (_Lon != value) { _Lon = value; OnPropertyChanged("Lon"); } }
        //}
        public double LatDD
        {
            get { return _LatDD; }
            set { if (_LatDD != value) { _LatDD = value; OnPropertyChanged("LatDD"); } }
        }
        public double LatMM
        {
            get { return _LatMM; }
            set { if (_LatMM != value) { _LatMM = value; OnPropertyChanged("LatMM"); } }
        }
        public double LatSS
        {
            get { return _LatSS; }
            set { if (_LatSS != value) { _LatSS = value; OnPropertyChanged("LatSS"); } }
        }
        public double LonDD
        {
            get { return _LonDD; }
            set { if (_LonDD != value) { _LonDD = value; OnPropertyChanged("LonDD"); } }
        }
        public double LonMM
        {
            get { return _LonMM; }
            set { if (_LonMM != value) { _LonMM = value; OnPropertyChanged("LonMM"); } }
        }
        public double LonSS
        {
            get { return _LonSS; }
            set { if (_LonSS != value) { _LonSS = value; OnPropertyChanged("LonSS"); } }
        }
        #endregion
        public System.Windows.Point SelectedCoor
        {
            get { return (System.Windows.Point)GetValue(SelectedCoorProperty); }
            set { SetValue(SelectedCoorProperty, value); OnPropertyChanged("SelectedCoor"); }
        }
        public static readonly DependencyProperty SelectedCoorProperty = DependencyProperty.Register("SelectedCoor", typeof(System.Windows.Point), typeof(GlobalOSMMap), new PropertyMetadata(new System.Windows.Point(0, 0), null));
        public bool ShowDFBearing
        {
            get { return (bool)GetValue(ShowDFBearingProperty); }
            set { SetValue(ShowDFBearingProperty, value); OnPropertyChanged("ShowDFBearing"); }
        }
        public static readonly DependencyProperty ShowDFBearingProperty = DependencyProperty.Register("ShowDFBearing", typeof(bool), typeof(GlobalOSMMap), new PropertyMetadata(true, null));
        public GlobalOSMMap()
        {
            TileGenerator.CacheFolder = App.Sett.Map_Settings.MapPath;// @"MAPOSM";
            TileGenerator.DownloadCountChanged += this.OnDownloadCountChanged;
            TileGenerator.DownloadError += this.OnDownloadError;
            InitializeComponent();

            this.DataContext = this;
            MainWindow.db_v2.PropertyChanged += UpdatePlanState_PropertyChanged;
            this.PropertyChanged += Changed;
            MainWindow.gps.PropertyChanged += GPSCoor_PropertyChanged;
            CommandManager.AddPreviewExecutedHandler(this, this.PreviewExecuteCommand); // We're going to do some effects when zooming.
            MainWindow.Rcvr.PropertyChanged += DFBearingChanged;
            PrintScreen.DataContext = App.Sett.Map_Settings;

            //ShowOnMapATDI_TC.DataContext = MainWindow.db_v2;
        }
        private void GlobalOSMMap_Loaded(object sender, RoutedEventArgs e)
        {
            if (settingLoaded == false)
            {
                Settings.MapsControlName = this.Name;
                GetSettings();
                AddMarkerGPSPosition();
                AddMarkerDFBearingCanvas();
                if (ShowMyLocation == true) { GPSPositionCanvas.Visibility = Visibility.Visible; }
                else { GPSPositionCanvas.Visibility = Visibility.Collapsed; }

                if (App.Sett.Map_Settings.SaveMapPosition == true)//приминение сохраненных последних позиций карт
                {
                    try
                    {
                        tileCanvas.Center(Settings.MapsInitialY, Settings.MapsInitialX, Settings.MapsInitialScale);
                    }
                    catch { }
                }
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.PropertyChanged -= MainWindow_PropertyChanged;
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.PropertyChanged += MainWindow_PropertyChanged;
                //((SplashWindow)App.Current.MainWindow).m_mainWindow.Closing += MainWindow_Closing;
                settingLoaded = true;
                GetHeaders(InfoSelectedData);
            }
        }
        private void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MainWindow_Closing")
            {
                SaveCentrPosition();
            }
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
                    for (int i = 0; i < Settings.ATDI_StationInfoColumnChooserMenu.Count; i++)
                    {
                        if (Settings.ATDI_StationInfoColumnChooserMenu[i].VariableName == binding.Path.Path)
                        {
                            if (Settings.ATDI_StationInfoColumnChooserMenu[i].Visible == true)
                            {
                                column.Visibility = Visibility.Visible;
                            }
                            else column.Visibility = Visibility.Collapsed;
                            if (find == false)
                            {
                                Settings.ATDI_StationInfoColumnChooserMenu[i].Name = column.Header.ToString();
                                Settings.ATDI_StationInfoColumnChooserMenu[i].col = column;
                                find = true;
                            }
                        }
                    }
                    if (find == false)
                    {
                        Settings.ATDI_StationInfoColumnChooserMenu.Add(new Settings.DataGridHeaderVisibility() { Name = column.Header.ToString(), VariableName = binding.Path.Path, Visible = true, col = column });
                    }
                }
                //else if (column.Header is Grid)
                //{
                //    Grid gr = (Grid)column.Header;
                //    TextBlock tb = (TextBlock)gr.Children[0];
                //    Settings.ATDI_StationInfoColumnChooserMenu.Add(new Settings.DataGridHeaderVisibility() { Name = tb.Text, Visible = true, col = column });
                //}
            }
            for (int i = 0; i < Settings.ATDI_StationInfoColumnChooserMenu.Count; i++)
            {
                if (Settings.ATDI_StationInfoColumnChooserMenu[i].col == null) { Settings.ATDI_StationInfoColumnChooserMenu.RemoveAt(i); i--; }
            }
            Settings.ATDI_StationInfoColumnChooserMenu = new ObservableCollection<Settings.DataGridHeaderVisibility>(Settings.ATDI_StationInfoColumnChooserMenu.OrderBy(i => i.Name));
            object obj = Grid.FindResource("ColumnChooserMenu");
            ContextMenu m = (ContextMenu)obj;
            m.DataContext = Settings;
            m.ItemsSource = Settings.ATDI_StationInfoColumnChooserMenu;
            Grid.Resources["ColumnChooserMenu"] = m;
        }

        #region смена размеров Station Info Selected
        bool InfoSelectedBorderLeftChange = false;
        bool InfoSelectedBorderDownChange = false;
        bool InfoSelectedBorderLeftDownChange = false;
        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (InfoSelectedBorderLeftChange == true)
            {
                SelectedInfoPanelWidth = ((Grid)sender).ActualWidth - e.GetPosition((Grid)sender).X + 2.5;

                InfoSelected_Bor.SetValue(Border.WidthProperty, SelectedInfoPanelWidth);
            }
            else if (InfoSelectedBorderDownChange == true)
            {
                double h = e.GetPosition((Grid)sender).Y - 28 + 2.5;
                if (h > 0)
                    InfoSelected_Bor.SetValue(Border.HeightProperty, h);
            }
            else if (InfoSelectedBorderLeftDownChange == true)
            {
                SelectedInfoPanelWidth = ((Grid)sender).ActualWidth - e.GetPosition((Grid)sender).X + 2.5;

                InfoSelected_Bor.SetValue(Border.WidthProperty, SelectedInfoPanelWidth);
                double h = e.GetPosition((Grid)sender).Y - 28 + 2.5;
                if (h > 0)
                    InfoSelected_Bor.SetValue(Border.HeightProperty, h);
            }
        }
        private void leftrec_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            InfoSelectedBorderLeftChange = false;
        }
        private void leftrec_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Control c = sender as Control;
            Mouse.Capture(c);
            InfoSelectedBorderLeftChange = true;
        }
        private void downrec_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            InfoSelectedBorderDownChange = false;
        }
        private void downrec_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Control c = sender as Control;
            Mouse.Capture(c);
            InfoSelectedBorderDownChange = true;
        }
        private void leftdownrec_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            InfoSelectedBorderLeftDownChange = false;
        }
        private void leftdownrec_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Control c = sender as Control;
            Mouse.Capture(c);
            InfoSelectedBorderLeftDownChange = true;
        }
        #endregion смена размеров Station Info Selected

        #region Обработчики карты
        private void GetSettings()
        {
            bool find = false;
            if (App.Sett.Map_Settings.ControlsSettings.Count > 0)
            {
                for (int i = 0; i < App.Sett.Map_Settings.ControlsSettings.Count; i++)
                {
                    if (Settings.MapsControlName == App.Sett.Map_Settings.ControlsSettings[i].MapsControlName)
                    {
                        Settings = App.Sett.Map_Settings.ControlsSettings[i];
                        OnPropertyChanged("ShowMyLocation");
                        OnPropertyChanged("AutoPanModeMap");
                        OnPropertyChanged("ShowPlanPanelExpander");
                        OnPropertyChanged("ShowSelectedInfoPanelExpander");
                        OnPropertyChanged("SelectedInfoPanelWidth");
                        OnPropertyChanged("MapOpacity");
                        OnPropertyChanged("PlansLayerOpacity");
                        OnPropertyChanged("RadiusLayerOpacity");
                        OnPropertyChanged("ShowSearchRadiusOnMapState");
                        find = true;
                    }
                }
            }
            if (find == false)
            {
                App.Sett.Map_Settings.ControlsSettings.Add(Settings);
            }
        }
        public void SaveCentrPosition()
        {
            Settings.MapsInitialScale = tileCanvas.Zoom; //_offsetX.CenterOn(TileGenerator.GetTileX(longitude, this.Zoom));
            Point p1 = GetCentrPoint();
            Settings.MapsInitialX = p1.X;
            Settings.MapsInitialY = p1.Y;
            App.Sett.SaveMap();
        }

        private void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri); // Launch the site in the user's default browser.
        }

        private void OnDownloadCountChanged(object sender, EventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new Action(() => this.OnDownloadCountChanged(sender, e)), null);
                return;
            }
            if (TileGenerator.DownloadCount == 0)
            {
                this.downloadpanel.Visibility = Visibility.Collapsed;
                _maxDownload = -1;
            }
            else
            {
                this.errorBar.Visibility = Visibility.Collapsed;

                if (_maxDownload < TileGenerator.DownloadCount)
                {
                    _maxDownload = TileGenerator.DownloadCount;
                }
                this.progress.Value = 100 - (TileGenerator.DownloadCount * 100.0 / _maxDownload);
                this.downloadpanel.Visibility = Visibility.Visible;
                this.progress.Visibility = Visibility.Visible;
                this.label.Text = string.Format(
                    CultureInfo.CurrentUICulture,
                    "Downloading {0} item{1}",
                    TileGenerator.DownloadCount,
                    TileGenerator.DownloadCount != 1 ? 's' : ' ');
                this.label.Visibility = Visibility.Visible;
            }
        }

        private void OnDownloadError(object sender, EventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new Action(() => this.OnDownloadError(sender, e)), null);
                return;
            }

            this.errorBar.Text = "Unable to contact the server to download map data.";
            this.errorBar.Visibility = Visibility.Visible;
        }


        private void OnZoomStoryboardCompleted(object sender, EventArgs e)
        {
            this.zoomGrid.Visibility = Visibility.Hidden;
            this.zoomImage.Source = null;
        }

        private void PreviewExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == NavigationCommands.DecreaseZoom)
            {
                if (this.tileCanvas.Zoom > 0) // Make sure we can actualy zoom out
                {
                    this.StartZoom("zoomOut", 1);
                    //ATDI_CalcAndDrawClustersFromTask(task);
                    ATDI_CalcAndDrawClustersFrom_ATDI_TasksToDraw();
                }
            }
            else if (e.Command == NavigationCommands.IncreaseZoom)
            {
                if (this.tileCanvas.Zoom < TileGenerator.MaxZoom)
                {
                    this.StartZoom("zoomIn", 0.5);
                    //ATDI_CalcAndDrawClustersFromTask(task);
                    ATDI_CalcAndDrawClustersFrom_ATDI_TasksToDraw();
                }
            }
        }

        private void StartZoom(string name, double scale)
        {
            this.zoomImage.Source = this.tileCanvas.CreateImage();
            this.zoomRectangle.Height = this.tileCanvas.ActualHeight * scale;
            this.zoomRectangle.Width = this.tileCanvas.ActualWidth * scale;

            this.zoomGrid.RenderTransform = new ScaleTransform(); // Clear the old transform
            this.zoomGrid.Visibility = Visibility.Visible;

            ((Storyboard)this.zoomGrid.FindResource(name)).Begin();

        }
        public Point GetCentrPoint()
        {
            Point screenLoc = new Point(this.zoomRectangle.ActualWidth / 2, this.zoomRectangle.ActualHeight / 2);//0, 0);
            Point pout = tileCanvas.GetLocation(screenLoc);

            return pout;
        }
        private void tileCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                tileCanvas._offsetY.AnimateTranslate(-256);
                AutoPanModeMap = false;
            }
            else if (e.Key == Key.Up)
            {
                tileCanvas._offsetY.AnimateTranslate(256);
                AutoPanModeMap = false;
            }
            else if (e.Key == Key.Right)
            {
                tileCanvas._offsetX.AnimateTranslate(-256);
                AutoPanModeMap = false;
            }
            else if (e.Key == Key.Left)
            {
                tileCanvas._offsetX.AnimateTranslate(256);
                AutoPanModeMap = false;
            }
            else if (e.Key == Key.OemPlus)
            {
                tileCanvas.Zoom++;
            }
            else if (e.Key == Key.OemMinus)
            {
                tileCanvas.Zoom--;
            }
        }
        #endregion

        #region грузим инфу

        private void UpdatePlanState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            if (e.PropertyName == "UpdateTaskState" || e.PropertyName == "LoadedTasks")
            {
                try
                {
                    ATDI_TasksToDraw.Clear();
                    for (int i = 0; i < MainWindow.db_v2.AtdiTasks.Count; i++)
                    {
                        if (!ATDI_TasksToDraw.Any(p => p.Task.task_id == MainWindow.db_v2.AtdiTasks[i].task_id))
                        {
                            ATDI_TasksToDraw.Add(
                                new ATDI_DrawTaskInfo()
                                {
                                    Task = MainWindow.db_v2.AtdiTasks[i],
                                    TaskToDraw = false,
                                //TaskState = MainWindow.db_v2.LocalAtdiTasks[i].TaskState,
                                TaskTechItem = new ObservableCollection<ATDI_DrawTechTaskInfo>() { }
                                });
                        }
                        //else
                        //{
                        //    ATDI_TasksToDraw[i].TaskState = MainWindow.db_v2.LocalAtdiTasks[i].TaskState;
                        //}
                        for (int j = 0; j < MainWindow.db_v2.AtdiTasks[i].data_from_tech.Count; j++)
                        {
                            if (!ATDI_TasksToDraw[i].TaskTechItem.Any(p => p.tech.tech == MainWindow.db_v2.AtdiTasks[i].data_from_tech[j].tech))
                            {
                                ATDI_TasksToDraw[i].TaskTechItem.Add(
                                    new ATDI_DrawTechTaskInfo()
                                    {
                                        tech = MainWindow.db_v2.AtdiTasks[i].data_from_tech[j],
                                        TechToDraw = false
                                    });
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    ControlU.App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ControlU.MainWindow.exp.ExceptionData = new ControlU.ExData() { ex = exp, ClassName = "AtdiDataConverter", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name};
                    }));
                }
            }
        }
        #endregion

        #region рисуем на карте
        private void DFBearingChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (DFBearingCanvas != null)
                    {
                        if (ShowDFBearing && MainWindow.Rcvr.Mode.Mode == "DF")
                        {
                            if (DFBearingCanvas.Visibility == Visibility.Collapsed) { DFBearingCanvas.Visibility = Visibility.Visible; }
                            if (e.PropertyName == "DFAzimuth")
                            {
                                UpdateDFBearing((double)MainWindow.gps.LatitudeDecimal, (double)MainWindow.gps.LongitudeDecimal, (double)MainWindow.Rcvr.DFAzimuth);
                            }
                        }
                        if (MainWindow.Rcvr.Mode.Mode != "DF")
                        {
                            DFBearingCanvas.Visibility = Visibility.Collapsed;
                        }
                    }
                }));
            }
            catch { }
        }

        private void GPSCoor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ShowMyLocation == true && e.PropertyName.Contains("LatitudeD") || e.PropertyName.Contains("LongitudeD"))
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    UpdateGPSPosition((double)MainWindow.gps.LatitudeDecimal, (double)MainWindow.gps.LongitudeDecimal, MainWindow.gps.AngleCourse);

                    UpdateDFBearing((double)MainWindow.gps.LatitudeDecimal, (double)MainWindow.gps.LongitudeDecimal, (double)MainWindow.Rcvr.DFAzimuth);

                    if (AutoPanModeMap && Mouse.LeftButton == MouseButtonState.Released)
                    {
                        double w = this.ActualWidth, h = this.ActualHeight;
                        double Left = w / 4, right = (w / 4) * 3, up = h / 4, down = (h / 4) * 3;
                        Point leftup = new Point(Left, up);
                        Point rightdown = new Point(right, down);
                        Point leftupMap = tileCanvas.GetLocation(leftup);
                        Point rightdownMap = tileCanvas.GetLocation(rightdown);
                        if ((double)MainWindow.gps.LatitudeDecimal > leftupMap.Y)//сдвинем в верх
                        { tileCanvas._offsetY.AnimateTranslate(this.ActualHeight / 3); }
                        if ((double)MainWindow.gps.LatitudeDecimal < rightdownMap.Y)//сдвинем в низ
                        { tileCanvas._offsetY.AnimateTranslate(0 - this.ActualHeight / 3); }
                        if ((double)MainWindow.gps.LongitudeDecimal < leftupMap.X)//сдвинем в лево
                        { tileCanvas._offsetX.AnimateTranslate(this.ActualHeight / 3); }
                        if ((double)MainWindow.gps.LongitudeDecimal > rightdownMap.X)//сдвинем в право
                        { tileCanvas._offsetX.AnimateTranslate(0 - this.ActualHeight / 3); }
                    }
                }));
            }
        }
        private void UpdateGPSPosition(double Latitude, double Longitude, double Azimuth)
        {
            if (GPSPositionCanvas != null)
            {
                MapCanvas.SetLongitude(GPSPositionCanvas, Longitude);
                MapCanvas.SetLatitude(GPSPositionCanvas, Latitude);
                RotateTransform rt = new RotateTransform(Azimuth, 0, 0);
                GPSPositionCanvas.Children.OfType<Path>().Where(x => x.Name == "arrow").FirstOrDefault().RenderTransform = rt;
            }
        }
        private void UpdateDFBearing(double Latitude, double Longitude, double Azimuth)
        {
            if (DFBearingCanvas != null)
            {
                MapCanvas.SetLongitude(DFBearingCanvas, Longitude);
                MapCanvas.SetLatitude(DFBearingCanvas, Latitude);
                RotateTransform rt = new RotateTransform(Azimuth, 0, 0);
                DFBearingCanvas.Children.OfType<Path>().Where(x => x.Name == "DFBearingCanvas").FirstOrDefault().RenderTransform = rt;
            }
        }

        private void AddMarkerGPSPosition()
        {
            //GPSPositionCanvas = (Canvas)this.tileCanvas.Children.OfType<Canvas>().Where(x => x.Name == "GPSPosition").FirstOrDefault(); //(Canvas)this.tileCanvas.FindName("GPSPosition");
            if (GPSPositionCanvas == null)
            {
                GPSPositionCanvas = new Canvas();
                Path path = new Path();
                path.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)51, (byte)153, (byte)255));
                path.Data = Geometry.Parse("M -10,0 A 1,1 0 1 1 10,0 M -10,0 A 1,1 0 1 0 10,0");
                path.VerticalAlignment = VerticalAlignment.Center;
                path.HorizontalAlignment = HorizontalAlignment.Center;

                Path arrow = new Path();
                arrow.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)255, (byte)255, (byte)255));
                arrow.Data = Geometry.Parse("M 0,-8 -5,6 0,4 5,6");
                arrow.VerticalAlignment = VerticalAlignment.Center;
                arrow.HorizontalAlignment = HorizontalAlignment.Center;
                RotateTransform rt = new RotateTransform((double)MainWindow.gps.AngleCourse, 0, 0);
                arrow.RenderTransform = rt;
                arrow.Name = "arrow";

                Path Circle = new Path();
                Circle.Stroke = new SolidColorBrush(Color.FromArgb(255, (byte)51, (byte)153, (byte)255));
                Circle.Data = Geometry.Parse("M -10,0 A 1,1 0 1 1 10,0 M -10,0 A 1,1 0 1 0 10,0");
                Circle.StrokeThickness = 0.4;
                Circle.VerticalAlignment = VerticalAlignment.Center;
                Circle.HorizontalAlignment = HorizontalAlignment.Center;
                ScaleTransform RenderTransform = new ScaleTransform();
                Circle.RenderTransform = RenderTransform;
                //
                //DoubleAnimation Animation = new DoubleAnimation
                //{
                //    From = 0.9,
                //    To = 3,
                //    Duration = TimeSpan.FromMilliseconds(1200),
                //    RepeatBehavior = RepeatBehavior.Forever
                //};
                //Storyboard.SetTargetProperty(Animation, new PropertyPath(Canvas.WidthProperty));
                //Storyboard.SetTarget(Animation, RenderTransform);
                //Storyboard s = new Storyboard();
                //s.Children.Add(Animation);
                //RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, Animation);
                //RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, Animation);
                //s.Begin();
                //
                GPSPositionCanvas.Name = "GPSPosition";
                Panel.SetZIndex(GPSPositionCanvas, 10);
                GPSPositionCanvas.Children.Add(Circle);

                GPSPositionCanvas.Children.Add(path);
                GPSPositionCanvas.Children.Add(arrow);
                GPSPositionCanvas.Visibility = Visibility.Collapsed;
                MapCanvas.SetLongitude(GPSPositionCanvas, (double)MainWindow.gps.LongitudeDecimal);
                MapCanvas.SetLatitude(GPSPositionCanvas, (double)MainWindow.gps.LatitudeDecimal);
                tileCanvas.Children.Add(GPSPositionCanvas);
                tileCanvas.Center((double)MainWindow.gps.LatitudeDecimal, (double)MainWindow.gps.LongitudeDecimal, this.tileCanvas.Zoom);
            }
        }
        private void AddMarkerDFBearingCanvas()
        {
            if (DFBearingCanvas == null)
            {
                DFBearingCanvas = new Canvas();
                Path path = new Path();
                path.Stroke = new SolidColorBrush(Color.FromArgb(255, (byte)255, (byte)0, (byte)0));
                path.Data = Geometry.Parse("M0,-1000 0,-1000 M0,0 0,-500");// "M 0,-80 -50,60 0,40 50,60");// M 490,500 A 1,1 0 1 1 510,500  M 490,500 A 1,1 0 1 0 510,500");
                path.StrokeThickness = 2;
                path.VerticalAlignment = VerticalAlignment.Center;
                path.HorizontalAlignment = HorizontalAlignment.Center;
                RotateTransform rt = new RotateTransform((double)MainWindow.gps.AngleCourse, 0, 0);
                path.RenderTransform = rt;
                path.Name = "DFBearingCanvas";

                Panel.SetZIndex(DFBearingCanvas, 9);
                DFBearingCanvas.Children.Add(path);
                DFBearingCanvas.Visibility = Visibility.Collapsed;

                MapCanvas.SetLongitude(DFBearingCanvas, (double)MainWindow.gps.LongitudeDecimal);
                MapCanvas.SetLatitude(DFBearingCanvas, (double)MainWindow.gps.LatitudeDecimal);
                tileCanvas.Children.Add(DFBearingCanvas);
            }
        }

        #endregion

        private void tileCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            GetCentrPoint();
            Point screenLoc = new Point(e.GetPosition(tileCanvas).X, e.GetPosition(tileCanvas).Y);
            Point mapLoc = tileCanvas.GetLocation(screenLoc);

            string str = String.Format("Lat={0:N7} Lon={1:N7}", mapLoc.Y, mapLoc.X) + "\r\n";
            str += String.Format(" X={0} Y={1}", screenLoc.X, screenLoc.Y);


            CoorFromMap = new Point(mapLoc.X, mapLoc.Y);
            CoorFromMapX = MainWindow.help.DDDtoDDMMSS(mapLoc.X);
            CoorFromMapY = MainWindow.help.DDDtoDDMMSS(mapLoc.Y);
            if (CoorFromMap != null)
                SelectedCoor = CoorFromMap;
            if (GetCoorFromMap)
            { ///притуливаем координаты контролу что их показывает а смотреть или нет то такое
                MapCanvas.SetLongitude(clickOverlayGrid, SelectedCoor.X);
                MapCanvas.SetLatitude(clickOverlayGrid, SelectedCoor.Y);
            }
            if (ShowSearchRadiusOnMapState && GetCoorFromMap)
            {
                if (RadiusCentrPointCanvas == null && RadiusEndPointCanvas == null)
                {

                    RadiusCentrPointCanvas = new Canvas();
                    MapCanvas.SetLongitude(RadiusCentrPointCanvas, SelectedCoor.X);
                    MapCanvas.SetLatitude(RadiusCentrPointCanvas, SelectedCoor.Y);
                    tileCanvas.Children.Add(RadiusCentrPointCanvas);

                    Point p = MainWindow.help.calculateEndPoint(SelectedCoor, 90, 1000 * Math.Sqrt(2));
                    RadiusEndPointCanvas = new Canvas();
                    MapCanvas.SetLongitude(RadiusEndPointCanvas, p.X);
                    MapCanvas.SetLatitude(RadiusEndPointCanvas, p.Y);
                    tileCanvas.Children.Add(RadiusEndPointCanvas);

                    RadiusControl = new Map.DrawCircle() { From = RadiusCentrPointCanvas, To = RadiusEndPointCanvas, Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF4696CC")) };

                    Binding vis = new Binding("ShowSearchRadiusOnMapState");
                    vis.Source = this;
                    vis.Converter = new LocalBooleanDirectToVisibilityConverter();
                    vis.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    vis.Mode = BindingMode.OneWay;
                    RadiusControl.SetBinding(UserControl.VisibilityProperty, vis);

                    tileCanvas.Children.Add(RadiusControl);
                }
                else
                {
                    MapCanvas.SetLongitude(RadiusCentrPointCanvas, SelectedCoor.X);
                    MapCanvas.SetLatitude(RadiusCentrPointCanvas, SelectedCoor.Y);

                    Point p = MainWindow.help.calculateEndPoint(CoorFromMap, 90, 1000 * Math.Sqrt(2));
                    MapCanvas.SetLongitude(RadiusEndPointCanvas, p.X);
                    MapCanvas.SetLatitude(RadiusEndPointCanvas, p.Y);
                }


            }
        }


        private void tileCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                MouseLeftDownPositionPoint = e.GetPosition(this);
        }
        private void tileCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                if (AutoPanModeMap &&
                    (Math.Abs(MouseLeftDownPositionPoint.X - e.GetPosition(this).X) > this.ActualWidth / 2 ||
                    Math.Abs(MouseLeftDownPositionPoint.Y - e.GetPosition(this).Y) > this.ActualHeight / 2))
                {
                    AutoPanModeMap = false;
                }
        }
        #region Кнопки
        private void ShowPanelDataOnPlan_Click(object sender, RoutedEventArgs e)
        {
            ShowPlanPanelExpander = !ShowPlanPanelExpander;
        }
        private void ShowMyLocationMapGui_Click(object sender, RoutedEventArgs e)
        {
            ShowMyLocation = !ShowMyLocation;
            if (ShowMyLocation == true) { GPSPositionCanvas.Visibility = Visibility.Visible; }
            else { AutoPanModeMap = false; GPSPositionCanvas.Visibility = Visibility.Collapsed; }

        }

        private void AutoPanModeMapGui_Click(object sender, RoutedEventArgs e)
        {
            AutoPanModeMap = !AutoPanModeMap;
            if (AutoPanModeMap)
            {
                tileCanvas.Center((double)MainWindow.gps.LatitudeDecimal, (double)MainWindow.gps.LongitudeDecimal, tileCanvas.Zoom--);
                tileCanvas.Center((double)MainWindow.gps.LatitudeDecimal, (double)MainWindow.gps.LongitudeDecimal, tileCanvas.Zoom++);
            }
        }

        private void GetCoorFromMapGui_Click(object sender, RoutedEventArgs e)
        {
            GetCoorFromMap = !GetCoorFromMap;
            if (GetCoorFromMap) { clickOverlayGrid.Visibility = Visibility.Visible; }
            else if (!GetCoorFromMap) { clickOverlayGrid.Visibility = Visibility.Hidden; }
        }


        private void ShowSelectedInfo_Click(object sender, RoutedEventArgs e)
        {
            string d = this.Name;
            ShowSelectedInfoPanelExpander = !ShowSelectedInfoPanelExpander;
        }
        private void ShowMapSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowMapSettings = !ShowMapSettings;
        }
        private void ShowSearchRadiusOnMap_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchRadiusOnMapState = !ShowSearchRadiusOnMapState;
        }
        #endregion




        private void Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Coor")
            {
                //CalcCoorDoubleToDDMMSS();
                //((MainWindow)App.Current.MainWindow).Message = CoorFromMapX + " " + CoorFromMapY;
            }
            else if (e.PropertyName == "MapOpacity")
            {
                this.tileCanvas.Opacity = MapOpacity;
            }
            //else if (e.PropertyName == "PlansLayerOpacity")
            //{ PlansLayer.Opacity = PlansLayerOpacity; }
            //else if (e.PropertyName == "RadiusLayerOpacity")
            //{ RadiusLayer.Opacity = RadiusLayerOpacity; }
            //else if (e.PropertyName == "ShowSearchRadiusOnMapState")
            //{
            //    if (ShowSearchRadiusOnMapState == true) { ShowRadius(Coor, TableRadiusFind); }
            //    else { RadiusLayer.Graphics.Clear(); }
            //}
        }


        #region



        /// <summary>
        /// Обработчик канваса который его выделяет при нажатии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ATDI_MarkerClick(object sender, RoutedEventArgs e)
        {
            SelectedStation.Effect = null;
            SelectedStation = (Canvas)sender;
            DropShadowEffect myDropShadowEffect = new DropShadowEffect();
            myDropShadowEffect.Color = Color.FromRgb(255, 0, 0); //Color.FromRgb(51, 153, 255);
            myDropShadowEffect.Direction = 0;
            myDropShadowEffect.BlurRadius = 15;
            myDropShadowEffect.ShadowDepth = 0;
            myDropShadowEffect.Opacity = 1;
            myDropShadowEffect.RenderingBias = RenderingBias.Performance;
            SelectedStation.Effect = myDropShadowEffect;
            //SelectedMarker.Background = new SolidColorBrush(Color.FromRgb(51, 153, 255));

            // Apply the bitmap effect to the Button.

        }
        //***
        public Point GetABSCoor()
        {
            Point pout = new Point();
            Point screenLoc1 = new Point(0, 0);//0, 0);
            Point pout1 = tileCanvas.GetLocation(screenLoc1);
            Point screenLoc2 = new Point(this.zoomRectangle.ActualWidth, this.zoomRectangle.ActualHeight);//0, 0);
            Point pout2 = tileCanvas.GetLocation(screenLoc2);
            pout = new Point() { X = pout2.X - pout1.X, Y = pout2.Y - pout1.Y };

            return pout;
        }
        //***
        DB.localatdi_meas_task task;
        //***
        public List<Helpers.MyAlgorithmClusterBundle.Bucket> clusters;

        //***
        private void ATDI_ShowAllOnMyTask_btn(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            /*DB.LocalAtdiTask */
            ATDI_DrawTaskInfo dti = (ATDI_DrawTaskInfo)bt.Tag;
            dti.TaskToDraw = !dti.TaskToDraw;
            for (int i = 0; i < dti.TaskTechItem.Count; i++)
            {
                dti.TaskTechItem[i].TechToDraw = dti.TaskToDraw;
            }
            task = dti.Task;
            //if (dti.TaskToDraw)
            //    ATDI_CalcAndDrawClustersFromTask(task);
            //else { }
            ATDI_CalcAndDrawClustersFrom_ATDI_TasksToDraw();
        }
        //***
        private void ATDI_AllHideCompletedOnMyTask_btn(object sender, RoutedEventArgs e)
        {
            HideCompleted = !HideCompleted;
        }
        //***
        private void ATDI_ShowAllOnMyTechTask_btn(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ATDI_DrawTechTaskInfo dtti = (ATDI_DrawTechTaskInfo)bt.Tag;
            dtti.TechToDraw = !dtti.TechToDraw;
            ATDI_CalcAndDrawClustersFrom_ATDI_TasksToDraw();
        }
        //***
        private void ATDI_CalcAndDrawClustersFrom_ATDI_TasksToDraw()
        {
            long g = DateTime.Now.Ticks;
            Helpers.MyAlgorithmClusterBundle al = new Helpers.MyAlgorithmClusterBundle();

            Point p = GetABSCoor();
            //al.setAbsSize(p.X, p.Y);
            al.setMARKERCLUSTERER_SIZE(p.X / 10);
            Point screenLoc1 = new Point(0, 0);//0, 0);
            Point pout1 = tileCanvas.GetLocation(screenLoc1);
            Point screenLoc2 = new Point(this.zoomRectangle.ActualWidth, this.zoomRectangle.ActualHeight);//0, 0);
            Point pout2 = tileCanvas.GetLocation(screenLoc2);
            al.setMinMax(pout1.X, pout1.Y, pout2.X, pout2.Y);
            al._points = new List<Helpers.MyAlgorithmClusterBundle.XY>() { };

            for (int i = 0; i < ATDI_TasksToDraw.Count; i++)
            {
                //if (ATDI_TasksToDraw[i].TaskToDraw == true)//добавляем в кластер таск
                //{
                for (int tc = 0; tc < ATDI_TasksToDraw[i].TaskTechItem.Count; tc++)
                {
                    if (ATDI_TasksToDraw[i].TaskTechItem[tc].TechToDraw == true)//добавляем в кластер
                    {
                        for (int it = 0; it < ATDI_TasksToDraw[i].TaskTechItem[tc].tech.TaskItems.Count; it++)
                        {
                            al._points.Add(new Helpers.MyAlgorithmClusterBundle.XY()
                            {
                                X = ATDI_TasksToDraw[i].TaskTechItem[tc].tech.TaskItems[it].site.location.latitude,
                                Y = ATDI_TasksToDraw[i].TaskTechItem[tc].tech.TaskItems[it].site.location.longitude,
                                Size = 1,
                                obj = ATDI_TasksToDraw[i].TaskTechItem[tc].tech.TaskItems[it],
                                st = ATDI_TasksToDraw[i].TaskTechItem[tc].tech.TaskItems[it],
                            });
                        }
                    }
                }
                //}
            }
            if (al._points.Count > 0)
            {
                al.clusterType = Helpers.MyAlgorithmClusterBundle.ClusterType.MarkerClusterer;
                al.Run(al.clusterType);
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - g);

                if (al._clustersb.Count > 0)
                {
                    clusters = al._clustersb;
                    drawClusters(clusters);
                }

                //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = ts.ToString() + "    " + (new TimeSpan(DateTime.Now.Ticks - g)).ToString() + "    " + al._clustersb.Count.ToString();

            }
            else { ATDI_DeleteItemsFromMapWithTaskNumber(); }
        }

        //***
        void drawClusters(List<Helpers.MyAlgorithmClusterBundle.Bucket> clu)
        {
            try
            {
                for (int i = 0; i < ItemsOnMap.Count; i++)
                {
                    if (ItemsOnMap[i].Name.StartsWith("cluster") || ItemsOnMap[i].Name.StartsWith("station") || ItemsOnMap[i].Name.StartsWith("sector"))
                    {
                        tileCanvas.Children.Remove(ItemsOnMap[i]);
                        ItemsOnMap.Remove(ItemsOnMap[i]);
                        i--;
                    }
                }
            }
            catch { }
            double Xmin = double.MaxValue, Ymin = double.MaxValue, Xmax = double.MinValue, Ymax = double.MinValue;
            double xavg = 0, yavg = 0;
            for (int cl = 0; cl < clu.Count; cl++)
            {
                if (clu[cl].Centroid.X < Xmin) Xmin = clu[cl].Centroid.X;
                if (clu[cl].Centroid.X > Xmax) Xmax = clu[cl].Centroid.X;
                if (clu[cl].Centroid.Y < Ymin) Ymin = clu[cl].Centroid.Y;
                if (clu[cl].Centroid.Y > Ymax) Ymax = clu[cl].Centroid.Y;
                if (clu[cl].Points.Count > 1)//малюем кластеры
                {
                    #region
                    #region считаем кластеры
                    int mval = 0;
                    int unmval = 0;
                    int ival = 0;
                    string ssss = "";
                    for (int p = 0; p < clu[cl].Points.Count; p++)
                    {
                        DB.localatdi_station item = (DB.localatdi_station)clu[cl].Points[p].obj;
                        if (item.meas_data_exist) mval++;
                        else unmval++;
                        if (item.IsIdentified) ival++;
                        ssss += item.standard + " " + item.Callsign_db_S1.ToString() + " " + item.Callsign_db_S3.ToString() + "\r\n";
                        //item.meas_data_exist
                        item.PropertyChanged += chatter_PropertyChanged;
                    }
                    clu[cl].Measured = mval;
                    clu[cl].UnMeasured = unmval;
                    clu[cl].Identified = ival;
                    #endregion
                    int diam = 50;

                    Canvas c = new Canvas();
                    c.Width = 0;
                    c.Height = 0;
                    c.Name = "cluster_" + clu[cl].Id.Replace(";", ""); //(clu[cl].Centroid.X + "_" + clu[cl].Centroid.Y).Replace(",", "");

                    Map.Cluster lcp = new Map.Cluster();
                    lcp.Name = "cluster_" + clu[cl].Id.Replace(";", ""); //(clu[cl].Centroid.X + "_" + clu[cl].Centroid.Y).Replace(",", "");
                    lcp.LeftMinimum = 0;
                    lcp.LeftMaximum = clu[cl].Points.Count;
                    lcp.RightMinimum = 0;
                    lcp.RightMaximum = clu[cl].Points.Count;
                    lcp.Width = diam - 1;
                    lcp.Height = diam - 1;
                    lcp.StrokeThickness = diam / 5;
                    lcp.VerticalAlignment = VerticalAlignment.Center;
                    lcp.HorizontalAlignment = HorizontalAlignment.Center;
                    lcp.LeftSegmentColor = new SolidColorBrush(Color.FromArgb(255, (byte)70, (byte)150, (byte)204)); //(SolidColorBrush)(new BrushConverter().ConvertFrom("#FF4696CC"));
                                                                                                                     //lcp.TextCentr = clu[cl].Points.Count.ToString();

                    DataTrigger dt = new DataTrigger();
                    Binding bhc = new Binding("HideCompleted");
                    bhc.Source = this;
                    bhc.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bhc.Mode = BindingMode.OneWay;
                    dt.Binding = bhc;
                    dt.Value = true;
                    dt.Setters.Add(new Setter() { Property = Map.Cluster.TextCentrProperty, Value = clu[cl].UnMeasured.ToString() });

                    Style tstyle = new Style();
                    tstyle.Setters.Add(new Setter() { Property = Map.Cluster.TextCentrProperty, Value = clu[cl].Points.Count.ToString() });
                    tstyle.Triggers.Add(dt);
                    lcp.Style = tstyle;





                    Binding bleftCircle = new Binding("Identified");
                    bleftCircle.Source = clu[cl];
                    bleftCircle.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bleftCircle.Mode = BindingMode.OneWay;
                    lcp.SetBinding(Map.Cluster.LeftValueProperty, bleftCircle);
                    Binding brightCircle = new Binding("Measured");
                    brightCircle.Source = clu[cl];
                    brightCircle.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    brightCircle.Mode = BindingMode.OneWay;
                    lcp.SetBinding(Map.Cluster.RightValueProperty, brightCircle);

                    MultiDataTrigger mdt = new MultiDataTrigger();
                    Binding cc1_1 = new Binding("Measured");
                    cc1_1.Source = clu[cl];
                    cc1_1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    cc1_1.Mode = BindingMode.OneWay;
                    Condition cc1_c1 = new Condition(cc1_1, clu[cl].Points.Count);
                    Binding cc1_2 = new Binding("HideCompleted");
                    cc1_2.Source = this;
                    cc1_2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    cc1_2.Mode = BindingMode.OneWay;
                    Condition cc1_c2 = new Condition(cc1_2, true);
                    mdt.Conditions.Add(cc1_c1);
                    mdt.Conditions.Add(cc1_c2);
                    mdt.Setters.Add(new Setter() { Property = Canvas.VisibilityProperty, Value = Visibility.Collapsed });

                    Style style = new Style();
                    style.Setters.Add(new Setter() { Property = Canvas.VisibilityProperty, Value = Visibility.Visible });
                    style.Triggers.Add(mdt);
                    c.Style = style;

                    lcp.Margin = new Thickness(-diam / 2, -diam / 2, 0, 0);
                    c.Children.Add(lcp);
                    c.MouseDown += ATDI_ClusterClick;


                    Map.ATDI_TooltipStation dg = new Map.ATDI_TooltipStation();
                    Binding bdg = new Binding();
                    bdg.Source = clu[cl].Points;
                    bdg.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bdg.Mode = BindingMode.OneWay;
                    dg.Data.SetBinding(DataGrid.ItemsSourceProperty, bdg);


                    ToolTip tooltip = new ToolTip() { Content = dg };
                    ToolTipService.SetShowDuration(c, 60000);
                    ToolTipService.SetToolTip(c, tooltip);
                    //c.ToolTip = new ToolTip() { Content = dg };

                    Panel.SetZIndex(c, 1);
                    MapCanvas.SetLongitude(c, clu[cl].Centroid.Y);
                    MapCanvas.SetLatitude(c, clu[cl].Centroid.X);

                    //биндим прозрачность
                    Binding Opacity = new Binding("LayerOpacity");
                    Opacity.Source = Settings;
                    Opacity.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    Opacity.Mode = BindingMode.OneWay;
                    c.SetBinding(Canvas.OpacityProperty, Opacity);


                    ItemsOnMap.Add(c);
                    tileCanvas.Children.Add(c);
                    #endregion
                }
                else//малюем станцию
                {
                    #region
                    if (clu[cl].Points.Count > 0)
                    {
                        DB.localatdi_station item = (DB.localatdi_station)clu[cl].Points[0].obj;
                        if (item != null)
                        {
                            Canvas c = new Canvas();
                            int diam = 25;

                            c.Name = "station_" + item.license.icsm_id;// item.station_id; //(clu[cl].Centroid.X + "_" + clu[cl].Centroid.Y).Replace(",", "");
                            Map.MarkerStation ms = new Map.MarkerStation();
                            ms.Width = diam - 1;
                            ms.Height = diam - 1;
                            //ms.StrokeThickness = diam / 5;
                            ms.VerticalAlignment = VerticalAlignment.Center;
                            ms.HorizontalAlignment = HorizontalAlignment.Center;
                            ms.LeftSegmentColor = new SolidColorBrush(Color.FromArgb(255, (byte)70, (byte)150, (byte)204)); //(SolidColorBrush)(new BrushConverter().ConvertFrom("#FF4696CC"));
                                                                                                                            //ms.TextCentr = clu[cl].Points.Count.ToString();
                            ms.Margin = new Thickness(-diam / 2, -diam / 2, 0, 0);

                            Map.ATDI_TooltipStation dg = new Map.ATDI_TooltipStation();
                            Binding bdg = new Binding();
                            bdg.Source = clu[cl].Points;
                            bdg.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            bdg.Mode = BindingMode.OneWay;
                            dg.Data.SetBinding(DataGrid.ItemsSourceProperty, bdg);

                            ToolTip tooltip = new ToolTip() { Content = dg };
                            ToolTipService.SetShowDuration(c, 60000);
                            ToolTipService.SetToolTip(c, tooltip);
                            //ms.ToolTip = new ToolTip() { Content = dg };
                            //ms.ToolTip = new ToolTip() { Content = item.standart + "\r\n" + item.standart + " " + item.Callsign_S1.ToString() + " " + item.Callsign_S3.ToString() };



                            Binding bleftCircle = new Binding("IsIdentified");
                            bleftCircle.Converter = new Identified_ToColor_Converter();
                            bleftCircle.Source = item;
                            bleftCircle.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            bleftCircle.Mode = BindingMode.OneWay;
                            ms.SetBinding(Map.MarkerStation.LeftSegmentColorProperty, bleftCircle);

                            Binding brightCircle = new Binding("meas_data_exist");
                            brightCircle.Converter = new Measured_ToColor_Converter();
                            brightCircle.Source = item;
                            brightCircle.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            brightCircle.Mode = BindingMode.OneWay;
                            ms.SetBinding(Map.MarkerStation.RightSegmentColorProperty, brightCircle);


                            MultiDataTrigger mdt = new MultiDataTrigger();
                            Binding cc1_1 = new Binding("meas_data_exist");
                            cc1_1.Source = item;
                            cc1_1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            cc1_1.Mode = BindingMode.OneWay;
                            Condition cc1_c1 = new Condition(cc1_1, true);
                            Binding cc1_2 = new Binding("HideCompleted");
                            cc1_2.Source = this;
                            cc1_2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            cc1_2.Mode = BindingMode.OneWay;
                            Condition cc1_c2 = new Condition(cc1_2, true);
                            mdt.Conditions.Add(cc1_c1);
                            mdt.Conditions.Add(cc1_c2);
                            mdt.Setters.Add(new Setter() { Property = Canvas.VisibilityProperty, Value = Visibility.Collapsed });

                            Style style = new Style();
                            style.Setters.Add(new Setter() { Property = Canvas.VisibilityProperty, Value = Visibility.Visible });
                            style.Triggers.Add(mdt);
                            c.Style = style;

                            c.MouseDown += ATDI_StationClick;

                            //c.Children.Add(gr);
                            c.Children.Add(ms);
                            Panel.SetZIndex(c, 3);
                            //биндим прозрачность
                            Binding Opacity = new Binding("LayerOpacity");
                            Opacity.Source = Settings;
                            Opacity.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            Opacity.Mode = BindingMode.OneWay;
                            c.SetBinding(Canvas.OpacityProperty, Opacity);

                            /////////////////////////////c.RenderTransform = new RotateTransform(0 - azstep * st);
                            //ms.MouseDown += ATDI_MarkerClick;
                            MapCanvas.SetLongitude(c, item.site.location.longitude);
                            MapCanvas.SetLatitude(c, item.site.location.latitude);
                            ItemsOnMap.Add(c);
                            tileCanvas.Children.Add(c);
                        }
                    }
                    #endregion
                }
            }
            xavg = (Xmin + Xmax) / 2;
            yavg = (Ymin + Ymax) / 2;
            tileCanvas.Center(xavg, yavg, tileCanvas.Zoom--);
            tileCanvas.Center(xavg, yavg, tileCanvas.Zoom++);
        }
        //***
        private void chatter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "meas_data_exist" || e.PropertyName == "IsIdentified")
            {
                for (int cl = 0; cl < clusters.Count; cl++)
                {
                    int meas = 0, unmeas = 0, ident = 0;

                    for (int p = 0; p < clusters[cl].Points.Count; p++)
                    {
                        DB.localatdi_station item = (DB.localatdi_station)clusters[cl].Points[p].obj;
                        if (item.meas_data_exist == true) meas++;
                        else unmeas++;
                        if (item.IsIdentified == true) ident++;
                    }
                    clusters[cl].Identified = ident;
                    clusters[cl].Measured = meas;
                    clusters[cl].UnMeasured = unmeas;
                }

            }
            //Debug.WriteLine("A property has changed: " + e.PropertyName);
        }
        //***
        /// <summary>
        /// Обработчик санваса который его выделяет при нажатии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ATDI_ClusterClick(object sender, RoutedEventArgs e)
        {
            if (SelectedCluster != null)
            {
                for (int cl = 0; cl < clusters.Count; cl++)
                {
                    if (SelectedCluster.Name == "cluster_" + clusters[cl].Id.Replace(";", ""))
                    {
                        for (int st = 0; st < clusters[cl].Points.Count; st++)
                        {
                            //SelectedCluster.Children.OfType<Grid>().ToArray()[0].Items
                            DB.localatdi_station item = (DB.localatdi_station)clusters[cl].Points[st].obj;
                            List<Grid> gr = SelectedCluster.Children.OfType<Grid>().ToList();
                            for (int i = 0; i < gr.Count; i++)
                            {
                                if (gr[i].Name == "station_" + item.license.icsm_id)
                                { SelectedCluster.Children.Remove(gr[i]); }
                            }
                            ////Grid item3 = SelectedCluster.Children.OfType<Grid>().ToList().Where(g => g.Name == "station_" + item.station_id.ToString()).Single();
                            //UIElement child = SelectedCluster.Children.FindName("station_" + item.station_id.ToString()) as UIElement;
                            //SelectedCluster.Children.Remove(child);
                        }
                    }
                }
                Panel.SetZIndex(SelectedCluster, 1);
                Map.Cluster cp1 = SelectedCluster.Children.OfType<Map.Cluster>().FirstOrDefault();//(CircularProgressBar)SelectedCluster.Parent;//Children.OfType<CircularProgressBar>().ToList()[0];
                cp1.IsSelected = false;
                cp1.Effect = null;
            }
            //MarkersOnSelectedCluster
            SelectedCluster = (Canvas)sender;
            Map.Cluster cp2 = SelectedCluster.Children.OfType<Map.Cluster>().FirstOrDefault();
            cp2.IsSelected = true;
            DropShadowEffect myDropShadowEffect = new DropShadowEffect();
            myDropShadowEffect.Color = Color.FromRgb(255, 0, 0); //Color.FromRgb(51, 153, 255);
            myDropShadowEffect.Direction = 0;
            myDropShadowEffect.BlurRadius = 15;
            myDropShadowEffect.ShadowDepth = 0;
            myDropShadowEffect.Opacity = 1;
            myDropShadowEffect.RenderingBias = RenderingBias.Performance;
            cp2.Effect = myDropShadowEffect;
            Panel.SetZIndex(SelectedCluster, 3);
            for (int cl = 0; cl < clusters.Count; cl++)
            {
                if (SelectedCluster.Name == "cluster_" + clusters[cl].Id.Replace(";", ""))//(clusters[cl].Centroid.X + "_" + clusters[cl].Centroid.Y).Replace(",", ""))//нашли кластер
                {
                    InfoSelectedData.ItemsSource = clusters[cl].Points;
                    int azstep = 360 / clusters[cl].Points.Count;
                    for (int st = 0; st < clusters[cl].Points.Count; st++)
                    {
                        DB.localatdi_station item = (DB.localatdi_station)clusters[cl].Points[st].obj;
                        Canvas c = new Canvas();
                        int diam = 25;

                        c.Name = "station_" + item.license.icsm_id;// item.station_id; //(clu[cl].Centroid.X + "_" + clu[cl].Centroid.Y).Replace(",", "");
                        Map.MarkerStation ms = new Map.MarkerStation();
                        ms.Width = diam - 1;
                        ms.Height = diam - 1;

                        ms.VerticalAlignment = VerticalAlignment.Center;
                        ms.HorizontalAlignment = HorizontalAlignment.Center;
                        ms.Margin = new Thickness(-diam / 2, -diam / 2, 0, 0);

                        Binding bleftCircle = new Binding("IsIdentified");
                        bleftCircle.Converter = new Identified_ToColor_Converter();
                        bleftCircle.Source = item;
                        bleftCircle.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        bleftCircle.Mode = BindingMode.OneWay;
                        ms.SetBinding(Map.MarkerStation.LeftSegmentColorProperty, bleftCircle);

                        Binding brightCircle = new Binding("meas_data_exist");
                        brightCircle.Converter = new Measured_ToColor_Converter();
                        brightCircle.Source = item;
                        brightCircle.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        brightCircle.Mode = BindingMode.OneWay;
                        ms.SetBinding(Map.MarkerStation.RightSegmentColorProperty, brightCircle);


                        Map.ATDI_TooltipStation dg = new Map.ATDI_TooltipStation();
                        Binding bdg = new Binding();
                        bdg.Source = new List<Helpers.MyAlgorithmClusterBundle.XY>() { clusters[cl].Points[st] };
                        bdg.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        bdg.Mode = BindingMode.OneWay;
                        dg.Data.SetBinding(DataGrid.ItemsSourceProperty, bdg);

                        ToolTip tooltip = new ToolTip() { Content = dg };
                        ToolTipService.SetShowDuration(c, 60000);
                        ToolTipService.SetToolTip(c, tooltip);
                        //ms.ToolTip = new ToolTip() { Content = dg };
                        //ms.ToolTip = new ToolTip() { Content = item.standart + "\r\n" + item.standart + " " + item.Callsign_S1.ToString() + " " + item.Callsign_S3.ToString() };

                        MultiDataTrigger mdt = new MultiDataTrigger();
                        Binding cc1_1 = new Binding("meas_data_exist");
                        cc1_1.Source = item;
                        cc1_1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        cc1_1.Mode = BindingMode.OneWay;
                        Condition cc1_c1 = new Condition(cc1_1, true);
                        Binding cc1_2 = new Binding("HideCompleted");
                        cc1_2.Source = this;
                        cc1_2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        cc1_2.Mode = BindingMode.OneWay;
                        Condition cc1_c2 = new Condition(cc1_2, true);
                        mdt.Conditions.Add(cc1_c1);
                        mdt.Conditions.Add(cc1_c2);
                        mdt.Setters.Add(new Setter() { Property = Canvas.VisibilityProperty, Value = Visibility.Collapsed });

                        Style style = new Style();
                        style.Setters.Add(new Setter() { Property = Canvas.VisibilityProperty, Value = Visibility.Visible });
                        style.Triggers.Add(mdt);
                        c.Style = style;




                        c.MouseDown += ATDI_StationClick;

                        c.Children.Add(ms);


                        c.RenderTransform = new RotateTransform(+90 - azstep * st);
                        /////////////////////////////c.RenderTransform = new RotateTransform(0 - azstep * st);
                        //ms.MouseDown += ATDI_MarkerClick;
                        Grid gr2 = new Grid();
                        gr2.Height = 100;
                        gr2.Width = 21;
                        gr2.VerticalAlignment = VerticalAlignment.Center;
                        gr2.HorizontalAlignment = HorizontalAlignment.Center;
                        gr2.Children.Add(c);

                        Grid gr3 = new Grid();
                        gr3.Height = 100;
                        gr3.Width = 100;
                        gr3.VerticalAlignment = VerticalAlignment.Center;
                        gr3.HorizontalAlignment = HorizontalAlignment.Center;
                        gr3.Children.Add(gr2);
                        Panel.SetZIndex(gr3, 3);
                        gr3.RenderTransform = new RotateTransform(-90 + azstep * st);
                        gr3.Name = "station_" + item.license.icsm_id;
                        SelectedCluster.Children.Add(gr3);
                        //////MapCanvas.SetLongitude(c, (double)item.site_lon);
                        //////MapCanvas.SetLatitude(c, (double)item.site_lat);
                        //////ItemsOnMap.Add(c);
                        //////tileCanvas.Children.Add(c);
                    }
                }
            }
        }

        private void GetImageFromMap_Click(object sender, RoutedEventArgs e)
        {
            bool fromname = false;
            if (App.Sett.Map_Settings.PrintScreenType == 0)
            {
                DialogWindow dialog = new DialogWindow(this.FindResource("EnterFileName").ToString(), "ControlU");
                dialog.Width = 250;
                dialog.ResponseText = ScreenName;
                if (dialog.ShowDialog() == true)
                {
                    fromname = true;
                    ScreenName = dialog.ResponseText;
                }
            }
            else if (App.Sett.Map_Settings.PrintScreenType == 1)
            {
                int ind = 1;
                string[] Files = MainWindow.help.GetFileNames(App.Sett.Screen_Settings.ScreenFolder, "Map_Screen_*.*");
                if (Files.Length > 0)
                {
                    for (int i = 0; i < Files.Length; i++)
                    {
                        int f = 0;
                        int.TryParse(Files[i].Replace("Map_Screen_", ""), out f);
                        ind = Math.Max(ind, f);
                    }
                    ind++;
                }
                ScreenName = "Map_Screen_" + string.Format("{0:00000}", ind);
            }
            else if (App.Sett.Map_Settings.PrintScreenType == 2)
            {
                ScreenName = "Map " + MainWindow.gps.LocalTime.ToString("yyyy-MM-dd H.mm.ss.fff");
            }
            if ((App.Sett.Map_Settings.PrintScreenType == 0 && fromname) || App.Sett.Map_Settings.PrintScreenType != 0)
            {
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)tileCanvas.ActualWidth, (int)tileCanvas.ActualHeight, 96, 96, PixelFormats.Default);
                rtb.Render(tileCanvas);

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(stream);

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);

                int fontSize = bitmap.Width / 50;
                if (fontSize > 10) fontSize = 10;
                System.Drawing.Font fontText = new System.Drawing.Font("Segoe UI Mono", fontSize);
                if (ShowMyLocation && MainWindow.gps.LongitudeDecimal != 0 && MainWindow.gps.LatitudeDecimal != 0)
                {
                    System.Drawing.Bitmap image = new System.Drawing.Bitmap(bitmap.Width, bitmap.Height + fontSize * 2);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                    g.FillRectangle(System.Drawing.Brushes.White, 0, 0, image.Width, image.Height);
                    g.DrawImage(bitmap, new System.Drawing.Point(0, 0));

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    Controls.CoorConverter cc = new Controls.CoorConverter();
                    string loc = "Lon " + (string)cc.Convert(MainWindow.gps.LongitudeDecimal, null, null, null) +
                        "Lat " + (string)cc.Convert(MainWindow.gps.LatitudeDecimal, null, null, null);
                    g.DrawString("Location " + loc, fontText, System.Drawing.Brushes.Black, 4, image.Height - fontSize * 2);
                    string osm = "© OpenStreetMap";
                    g.DrawString(osm, fontText, System.Drawing.Brushes.Black, image.Width - 10 - ((float)(osm.Length * fontSize)) / 20f * 16.5f, image.Height - fontSize * 2);
                    g.Dispose();
                    System.Drawing.Imaging.ImageFormat imfo = System.Drawing.Imaging.ImageFormat.Png;
                    image.Save(App.Sett.Screen_Settings.ScreenFolder + "\\" + ScreenName + ".Png", imfo);
                    image.Dispose();
                }
                else
                {
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    string osm = "© OpenStreetMap";
                    g.DrawString(osm, fontText, System.Drawing.Brushes.Black, bitmap.Width - 10 - ((float)(osm.Length * fontSize)) / 20f * 16.5f, bitmap.Height - fontSize * 2);
                    g.Dispose();

                    System.Drawing.Imaging.ImageFormat imfo = System.Drawing.Imaging.ImageFormat.Png;
                    bitmap.Save(App.Sett.Screen_Settings.ScreenFolder + "\\" + ScreenName + ".Png", imfo);
                    bitmap.Dispose();
                }
                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = this.FindResource("FileSaved").ToString().Replace("*NAME*", ScreenName);
            }






            //PngBitmapEncoder png = new PngBitmapEncoder();
            //png.Frames.Add(BitmapFrame.Create(rtb));
            //System.IO.MemoryStream stream = new System.IO.MemoryStream();
            //png.Save(stream);

            //System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            //image.Save(App.Sett.Screen_Settings.ScreenFolder + "\\1111111111111111111111.Png", System.Drawing.Imaging.ImageFormat.Png);
        }
        private void PrintScreenType_Click(object sender, RoutedEventArgs e)
        {
            App.Sett.Map_Settings.PrintScreenType = int.Parse((string)((MenuItem)sender).Tag);
            App.Sett.SaveEquipments();
        }
        private void GetImageFromMapTypeMenu_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = GetImageFromMap_Btn;// (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }











        //***
        private void ATDI_StationClick(object sender, RoutedEventArgs e)
        {
            if (SelectedStation != null)
            {
                SelectedStation.Effect = null;
                Map.MarkerStation ms1 = SelectedStation.Children.OfType<Map.MarkerStation>().FirstOrDefault();
                ms1.IsSelected = false;
            }

            SelectedStation = (Canvas)sender;
            Map.MarkerStation ms2 = SelectedStation.Children.OfType<Map.MarkerStation>().FirstOrDefault();
            ms2.IsSelected = true;
            DropShadowEffect myDropShadowEffect = new DropShadowEffect();
            myDropShadowEffect.Color = Color.FromRgb(255, 0, 0); //Color.FromRgb(51, 153, 255);
            myDropShadowEffect.Direction = 0;
            myDropShadowEffect.BlurRadius = 15;
            myDropShadowEffect.ShadowDepth = 0;
            myDropShadowEffect.Opacity = 1;
            myDropShadowEffect.RenderingBias = RenderingBias.Performance;
            ms2.Effect = myDropShadowEffect;
            object parent = SelectedStation.Parent;
            if (parent is Grid)//станция из кластера
            {
                #region
                Grid gr1 = (Grid)parent;
                Grid gr2 = (Grid)gr1.Parent;
                Canvas can = (Canvas)gr2.Parent;
                for (int cl = 0; cl < clusters.Count; cl++)
                {
                    if (can.Name == "cluster_" + clusters[cl].Id.Replace(";", ""))//нашли кластер
                    {
                        for (int p = 0; p < clusters[cl].Points.Count; p++)
                        {
                            DB.localatdi_station item = (DB.localatdi_station)clusters[cl].Points[p].obj;
                            if (SelectedStation.Name == "station_" + item.license.icsm_id)//нашли кластер
                            {
                                InfoSelectedData.ItemsSource = clusters[cl].Points;
                                drawSectorsFromStation(item);
                                tileCanvas.Center(item.site.location.latitude, item.site.location.longitude, tileCanvas.Zoom--);
                                tileCanvas.Center(item.site.location.latitude, item.site.location.longitude, tileCanvas.Zoom++);
                            }
                        }
                    }
                }
                #endregion
            }
            else if (parent is MapCanvas)//станция не из кластера
            {
                for (int cl = 0; cl < clusters.Count; cl++)
                {
                    DB.localatdi_station item = (DB.localatdi_station)clusters[cl].Points[0].obj;
                    if (SelectedStation.Name == "station_" + item.license.icsm_id)//нашли кластер
                    {
                        InfoSelectedData.ItemsSource = clusters[cl].Points;
                        drawSectorsFromStation(item);
                        tileCanvas.Center(item.site.location.latitude, item.site.location.longitude, tileCanvas.Zoom--);
                        tileCanvas.Center(item.site.location.latitude, item.site.location.longitude, tileCanvas.Zoom++);
                    }
                }
            }
            //SelectedMarker.Name// = "station_" + item.permission_id
        }
        //***
        public void drawSectorsFromStation(DB.localatdi_station item)
        {
            bool find = false;
            for (int ssc = 0; ssc < SelectedSectors.Count; ssc++)
            {
                for (int sc = 0; sc < item.sectors.Count; sc++)
                {
                    if (SelectedSectors[ssc] is Canvas && ((Canvas)SelectedSectors[ssc]).Name == "sector_" + item.sectors[sc].sector_id)
                    {
                        find = true;
                        tileCanvas.Children.Remove(SelectedSectors[ssc]);
                        SelectedSectors.Remove(SelectedSectors[ssc]);
                    }
                }
            }
            if (find == false)
            {
                #region
                for (int sc = 0; sc < item.sectors.Count; sc++)
                {
                    Canvas c = new Canvas();
                    c.Name = "sector_" + item.sectors[sc].sector_id;

                    Grid gr = new Grid();


                    SolidColorBrush br = new SolidColorBrush(Color.FromArgb(255, (byte)50, (byte)50, (byte)50));
                    Line l1 = new Line();
                    l1.X1 = 5;
                    l1.X2 = 5;
                    l1.Y1 = 0;
                    l1.Y2 = 100;
                    l1.Stroke = br;
                    l1.StrokeThickness = 4;
                    l1.Width = 10;
                    l1.HorizontalAlignment = HorizontalAlignment.Center;
                    
                    gr.Width = 10;
                    gr.Children.Add(l1);

                    gr.Background = new SolidColorBrush(Color.FromArgb(0, (byte)0, (byte)0, (byte)0));

                    TransformGroup group = new TransformGroup();
                    group.Children.Add(new TranslateTransform(-5, -100));
                    group.Children.Add(new RotateTransform((double)item.sectors[sc].azimuth));
                    gr.RenderTransform = group;

                    c.Background = new SolidColorBrush(Colors.Gray);
                    c.Children.Add(gr);

                    Map.ATDI_TooltipSector tsec = new Map.ATDI_TooltipSector();
                    Binding bdg = new Binding();
                    bdg.Source = new List<DB.localatdi_station_sector>() { item.sectors[sc] };
                    bdg.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bdg.Mode = BindingMode.OneWay;
                    tsec.Data.SetBinding(DataGrid.ItemsSourceProperty, bdg);

                    ToolTip tooltip = new ToolTip() { Content = tsec };
                    ToolTipService.SetShowDuration(c, 60000);
                    ToolTipService.SetToolTip(c, tooltip);


                    c.MouseDown += ATDI_MarkerClick;
                    MapCanvas.SetLongitude(c, item.site.location.longitude);
                    MapCanvas.SetLatitude(c, item.site.location.latitude);
                    Panel.SetZIndex(c, 2);
                    SelectedSectors.Add(c);
                    tileCanvas.Children.Add(c);
                }
                #endregion
            }
        }

        //***
        private void ATDI_DeleteItemsFromMapWithTaskNumber()
        {
            try
            {
                for (int i = 0; i < ItemsOnMap.Count; i++)
                {
                    if (ItemsOnMap[i].Name.StartsWith("cluster") || ItemsOnMap[i].Name.StartsWith("station") || ItemsOnMap[i].Name.StartsWith("sector"))
                    {
                        tileCanvas.Children.Remove(ItemsOnMap[i]);
                        ItemsOnMap.Remove(ItemsOnMap[i]);
                        i--;
                    }
                }
            }
            catch { }
        }

        //***


        #endregion

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ATDI_DrawTaskInfo : INotifyPropertyChanged
    {
        public DB.localatdi_meas_task Task
        {
            get { return _Task; }
            set { _Task = value; OnPropertyChanged("Task"); }
        }
        private DB.localatdi_meas_task _Task;

        //public int TaskState
        //{
        //    get { return _TaskState; }
        //    set { _TaskState = value; OnPropertyChanged("TaskState"); }
        //}
        //private int _TaskState = 0;

        /// <summary>
        /// Показать этот таск
        /// </summary>
        public bool TaskToDraw
        {
            get { return _TaskToDraw; }
            set { _TaskToDraw = value; OnPropertyChanged("TaskToDraw"); }
        }
        private bool _TaskToDraw = false;
        ///// <summary>
        ///// Скрывать выполненое
        ///// </summary>
        //public bool HideCompleted
        //{
        //    get { return _HideCompleted; }
        //    set { _HideCompleted = value; OnPropertyChanged("HideCompleted"); }
        //}
        //private bool _HideCompleted = false;

        public ObservableCollection<ATDI_DrawTechTaskInfo> TaskTechItem
        {
            get { return _TaskTechItem; }
            set { _TaskTechItem = value; OnPropertyChanged("TaskTechItem"); }
        }
        private ObservableCollection<ATDI_DrawTechTaskInfo> _TaskTechItem = new ObservableCollection<ATDI_DrawTechTaskInfo>() { };

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ATDI_DrawTechTaskInfo : INotifyPropertyChanged
    {
        public DB.localatdi_task_with_tech tech
        {
            get { return _tech; }
            set { _tech = value; OnPropertyChanged("tech"); }
        }
        private DB.localatdi_task_with_tech _tech;// = "";
                                               /// <summary>
                                               /// Показать эту технологию
                                               /// </summary>
        public bool TechToDraw
        {
            get { return _TechToDraw; }
            set { _TechToDraw = value; OnPropertyChanged("TechToDraw"); }
        }
        private bool _TechToDraw = false;

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class TableInfo : INotifyPropertyChanged
    {
        public int PlanFreq_ID
        {
            get { return _PlanFreq_ID; }
            set { _PlanFreq_ID = value; OnPropertyChanged("PlanFreq_ID"); }
        }
        private int _PlanFreq_ID = 0;

        public string FreqStart
        {
            get { return _FreqStart; }
            set { _FreqStart = value; OnPropertyChanged("FreqStart"); }
        }
        private string _FreqStart = "";

        public string FreqStop
        {
            get { return _FreqStop; }
            set { _FreqStop = value; OnPropertyChanged("FreqStop"); }
        }
        private string _FreqStop = "";

        public string Identifier
        {
            get { return _Identifier; }
            set { _Identifier = value; OnPropertyChanged("Identifier"); }
        }
        private string _Identifier = "";

        public string PermissionNumber
        {
            get { return _PermissionNumber; }
            set { _PermissionNumber = value; OnPropertyChanged("PermissionNumber"); }
        }
        private string _PermissionNumber = "";



        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ItemOnMap
    {
        public int id { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public double azimuth { get; set; }
        public int tech { get; set; }
        public List<Ids> ListIdsOnPoint = new List<Ids>() { };
        public class Ids
        {
            public int Plan_Freq_Id { get; set; }
            public int Plan_Id { get; set; }

        }
    }
    public class StrengtItem
    {
        public string id { get; set; }
        public Point coor { get; set; }
        public double Strengt { get; set; }
    }
}
