using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Atdi.WpfControls.Charts
{
    public struct TextDescriptor
    {
        public Brush Forecolor { get; set; }

        public string Text { get; set; }
    }

    public interface IFastChartData
    {
        TextDescriptor? Title { get; set; }

        TextDescriptor? LeftTitle { get; set; }

        TextDescriptor? RightTitle { get; set; }

        TextDescriptor? TopLegenda { get; set; }

        TextDescriptor? BottomLegenda { get; set; }

        TextDescriptor? LeftLegenda { get; set; }

        TextDescriptor? RightLegenda { get; set; }

        int LeftLabelSize { get; set; }

        int RightLabelSize { get; set; }

        int TopLabelSize { get; set; }

        int BottomLabelSize { get; set; }
    }

    public class FastChartData : IFastChartData
    {
        public TextDescriptor? Title { get; set; }

        public TextDescriptor? LeftTitle { get; set; }

        public TextDescriptor? RightTitle { get; set; }

        public TextDescriptor? TopLegenda { get; set; }

        public TextDescriptor? BottomLegenda { get; set; }

        public TextDescriptor? LeftLegenda { get; set; }

        public TextDescriptor? RightLegenda { get; set; }

        public int LeftLabelSize { get; set; }

        public int RightLabelSize { get; set; }

        public int TopLabelSize { get; set; }

        public int BottomLabelSize { get; set; }
    }

    public class FastChartData<TData> : FastChartData, IFastChartData<TData>
    {
        public FastChartData(TData container)
        {
            this.Contaier = container;
        }
        public TData Contaier { get; set; }
    }

    public interface IFastChartData<TData> : IFastChartData
    {
        TData Contaier { get; }
    }

    public interface IFastChartContext
    {
        int Width { get;  }

        int Height { get; }

        void PushPixel(int x, int y, Color color);

        void PushLine(int x1, int y1, int x2, int y2, Color color);

        void PushPolyline(int[] points, Color color);
    }

    public struct GridLine
    {
        public int Space { get; set; }

        public int Length { get; set; }

        public int Thickness { get; set; }

        public Color Color { get; set; }

        // смещение точки относительно координат
        public int Offset { get; set; }
    }

    /// <summary>
    /// Штрих/засечка на оси
    /// </summary>
    public struct GridDash
    {
        // Цвет триха
        public Color Color { get; set; }

        // текст напротив отсечки, если нужен
        public TextDescriptor? Label { get; set; }

        // толщина отсечки
        public int Thickness { get; set; }

        // размер отсечки
        public int Size { get; set; }
        // смещение точки относительно координат
        public int Offset { get; set; }
    }

    public struct GridPoint
    {

        

        // Описание линии отсечки
        public GridLine? Line { get; set; }

        public GridDash? LeftTopDash { get; set; }

        public GridDash? BottomRightDash { get; set; }
    }

    public class FastChartGripOptions
    {
        public GridPoint[] VerticalPoints { get; set; }

        public GridPoint[] HorizontalPoints { get; set; }

        public GridLine BorderLine { get; set; }
    }

    public interface IFastChartDataAdapter
    {
        FastChartGripOptions DefineGrid(IFastChartData staticData, IFastChartContext context);

        void DrawImage(IFastChartData staticData, IFastChartData dynamicData, IFastChartContext context);
    }

    public abstract class FastChartDataAdapterBase<TStaticData, TDynamicData> : IFastChartDataAdapter
    {
        public FastChartGripOptions DefineGrid(IFastChartData staticData, IFastChartContext context)
        {
            var typedStaticData = staticData as IFastChartData<TStaticData>;
            return this.DefineGrid(typedStaticData, context);
        }

        public void DrawImage(IFastChartData staticData, IFastChartData dynamicData, IFastChartContext context)
        {
            var typedStaticData = staticData as IFastChartData<TStaticData>;
            var typedDynamicData = dynamicData as IFastChartData<TDynamicData>;

            this.DrawImage(typedStaticData, typedDynamicData, context);
        }

        public abstract FastChartGripOptions DefineGrid(IFastChartData<TStaticData> staticData, IFastChartContext context);

        public abstract void DrawImage(IFastChartData<TStaticData> staticData, IFastChartData<TDynamicData> dynamicData, IFastChartContext context);

    }

    /// <summary>
    /// Interaction logic for FastChart.xaml
    /// </summary>
    public partial class FastChart : UserControl
    {
        private class FastChartContext : IFastChartContext
        {
            public WriteableBitmap Bitmap { get; set; }

            public int Width { get => Bitmap.PixelWidth; }

            public int Height { get => Bitmap.PixelHeight; }

            public void PushPixel(int x, int y, Color color)
            {
                y = Bitmap.PixelHeight - 1 - y ;
                Bitmap.SetPixel(x, y, color);
            }

            public void PushLine(int x1, int y1, int x2, int y2, Color color)
            {
                y1 = Bitmap.PixelHeight - 1 - y1;
                y2 = Bitmap.PixelHeight - 1 - y2;

                Bitmap.DrawLine(x1, y1, x2, y2, color);
            }

            public void PushPolyline(int[] points, Color color)
            {
                for (int i = 1; i < points.Length; i+=2)
                {
                    points[i] = Bitmap.PixelHeight - 1 - points[i];
                }
                Bitmap.DrawPolyline(points, color);
            }
        }

        private class DefaultAdapter : IFastChartDataAdapter
        {
            public FastChartGripOptions DefineGrid(IFastChartData staticData, IFastChartContext context)
            {
                var count = 100;
                var horStep = 50;
                var verStep = horStep * 2;
                var options = new FastChartGripOptions
                {
                    BorderLine = new GridLine { Color = Colors.Gray, Thickness = 3, Length = 0, Space = 0, Offset = 5 },
                    HorizontalPoints = new GridPoint[count],
                    VerticalPoints = new GridPoint[count]
                };

                for (int i = 0; i < count; i++)
                {
                    options.HorizontalPoints[i] = new GridPoint
                    {
                        Line = new GridLine
                        {
                            Color = Colors.DarkSlateGray,
                            Length = 6,
                            Space = 2,
                            Thickness = 2,
                            Offset = horStep + i * horStep
                        },
                        LeftTopDash = new GridDash
                        {
                            Color = Colors.DarkGray,
                            Size = 10,
                            Thickness = 4,
                            Offset = horStep - 1 + i * horStep,
                            Label = new TextDescriptor
                            {
                                Text = $"L: {i:D3}",
                                Forecolor = Brushes.DarkGreen
                            }
                        },
                        BottomRightDash = new GridDash
                        {
                            Color = Colors.DarkGray,
                            Size = 10,
                            Thickness = 4,
                            Offset = horStep - 1 + i * horStep,
                            Label = new TextDescriptor
                            {
                                Text = $"R: {i:D3}",
                                Forecolor = Brushes.DarkGreen
                            }
                        }
                    };

                    options.VerticalPoints[i] = new GridPoint
                    {
                        Line = new GridLine
                        {
                            Color = Colors.DarkSlateGray,
                            Length = 6,
                            Space = 2,
                            Thickness = 2,
                            Offset = verStep + i * verStep
                        },
                        LeftTopDash = new GridDash
                        {
                            Color = Colors.DarkGray,
                            Size = 10,
                            Thickness = 4,
                            Offset = verStep - 1 + i * verStep,
                            Label = new TextDescriptor
                            {
                                Text = $"L: {i:D3}",
                                Forecolor = Brushes.DarkGreen
                            }
                        },
                        BottomRightDash = new GridDash
                        {
                            Color = Colors.DarkGray,
                            Size = 10,
                            Thickness = 4,
                            Offset = verStep - 1 + i * verStep,
                            Label = new TextDescriptor
                            {
                                Text = $"R: {i:D3}",
                                Forecolor = Brushes.DarkGreen
                            }
                        }
                    };
                }
                return options;
            }

            public void DrawImage(IFastChartData staticData, IFastChartData dynamicData, IFastChartContext context)
            {
                for (int i = 50 ; i < context.Width - 50; i+=5)
                {
                    for (int j = 10; j < context.Height/3; j+=3)
                    {
                        context.PushPixel(i, j, Colors.Red);
                    }
                    
                }
                return;
            }
        }

        private WriteableBitmap _gridBitmap;
        private WriteableBitmap _workBitmap;

        private IFastChartData _staticData;
        private IFastChartData _dynamicData;
        private IFastChartDataAdapter _adapter;

        private FastChartContext _gridContext;
        private FastChartContext _workContext;

        private bool _notDrawOnResize;

        public FastChart()
        {
            this._gridContext = new FastChartContext();
            this._workContext = new FastChartContext();

            this._staticData = new FastChartData
            {
                Title = new TextDescriptor {  Text = "ATDI Fast Chart", Forecolor = Brushes.Gray},
                LeftTitle = new TextDescriptor { Text = "Left title", Forecolor = Brushes.Green },
                RightTitle = new TextDescriptor { Text = "Right title", Forecolor = Brushes.Red },
                TopLegenda = new TextDescriptor { Text = "Top Legenda", Forecolor = Brushes.DarkMagenta },
                BottomLegenda = new TextDescriptor { Text = "Bottom Legenda", Forecolor = Brushes.DarkMagenta },
                LeftLegenda = new TextDescriptor { Text = "Left Legenda", Forecolor = Brushes.Blue },
                RightLegenda = new TextDescriptor { Text = "Right Legenda", Forecolor = Brushes.Blue },
                LeftLabelSize = 20,
                BottomLabelSize = 20,
                RightLabelSize = 20,
                TopLabelSize = 20 
            };

            this._adapter = new DefaultAdapter();

            InitializeComponent();

            this.ApplyStaticData();
            this.ShowHideLabelCanvase();
        }

        public IFastChartData StaticData
        {
            get
            {
                return _staticData;
            }
            set
            {
                _staticData = value;
                // нужно обновить ширины отключив ресайзинг навремя
                this._notDrawOnResize = true;
                try
                {
                    // тут нужно обновить все метки 
                    this.ApplyStaticData();

                    // и показать/скрыть канвасы подписей
                    this.ShowHideLabelCanvase();
                }
                catch (Exception e)
                {
                    Debug.Fail(e.Message, e.ToString());
                }
                finally
                {
                    this._notDrawOnResize = false;
                }

                // перерисовать сетку
                this.RebuildBitmaps();
            }
        }

        private void ApplyStaticData()
        {
            if (_staticData.Title.HasValue)
            {
                var title = _staticData.Title.Value;
                this.TitleTextBlock.Text = title.Text;
                this.TitleTextBlock.Foreground = title.Forecolor ?? Brushes.Black;
            }
            if (_staticData.LeftTitle.HasValue)
            {
                var leftTitle = _staticData.LeftTitle.Value;
                this.LeftTitleTextBlock.Text = leftTitle.Text;
                this.LeftTitleTextBlock.Foreground = leftTitle.Forecolor ?? Brushes.Black;
            }
            if (_staticData.RightTitle.HasValue)
            {
                var rightTitle = _staticData.RightTitle.Value;
                this.RightTitleTextBlock.Text = rightTitle.Text;
                this.RightTitleTextBlock.Foreground = rightTitle.Forecolor ?? Brushes.Black;
            }
            if (_staticData.TopLegenda.HasValue)
            {
                var topLegenda = _staticData.TopLegenda.Value;
                this.TopLegendaTextBlock.Text = topLegenda.Text;
                this.TopLegendaTextBlock.Foreground = topLegenda.Forecolor ?? Brushes.Black;
            }
            if (_staticData.BottomLegenda.HasValue)
            {
                var bottomLegenda = _staticData.BottomLegenda.Value;
                this.BottomLegendaTextBlock.Text = bottomLegenda.Text;
                this.BottomLegendaTextBlock.Foreground = bottomLegenda.Forecolor ?? Brushes.Black;
            }
            if (_staticData.LeftLegenda.HasValue)
            {
                var leftLegenda = _staticData.LeftLegenda.Value;
                this.LeftLegendaTextBlock.Text = leftLegenda.Text;
                this.LeftLegendaTextBlock.Foreground = leftLegenda.Forecolor ?? Brushes.Black;
            }
            if (_staticData.RightLegenda.HasValue)
            {
                var rightLegenda = _staticData.RightLegenda.Value;
                this.RightLegendaTextBlock.Text = rightLegenda.Text;
                this.RightLegendaTextBlock.Foreground = rightLegenda.Forecolor ?? Brushes.Black;
            }
        }

        private void ShowHideLabelCanvase()
        {
            LeftDashPanel.Width = this._staticData.LeftLabelSize;
            RightDashPanel.Width = this._staticData.RightLabelSize;
            TopDashPanel.Height = this._staticData.TopLabelSize;
            BottomDashPanel.Height = this._staticData.BottomLabelSize;
        }

        //private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    FastChart lcc = sender as FastChart;

        //    if (e.Property == DynamicDataProperty)
        //        lcc.DynamicData = (IFastChartData)e.NewValue;
        //}

        //public static DependencyProperty DynamicDataProperty = DependencyProperty.Register("DynamicData", typeof(IFastChartData), typeof(FastChart),
        //   new FrameworkPropertyMetadata(default(IFastChartData), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPropertyChanged)));

        public IFastChartData DynamicData
        {
            get
            {
                return _dynamicData;
            }
            set
            {
                _dynamicData = value;
                this.ApplyStaticData();
                this.Draw();
                //ViewportImage.Source = null;
                //ViewportImage.Source = _workBitmap;
                //ViewportImage.InvalidateVisual();
            }
        }

        public IFastChartDataAdapter Adapter
        {
            get
            {
                return _adapter;
            }
            set
            {
                _adapter = value;
            }
        }

        private void RebuildBitmaps()
        {
            var width = (int)ViewPortContainer.ActualWidth;
            var height = (int)ViewPortContainer.ActualHeight;

            _workBitmap = BitmapFactory.New(width, height);
            _gridBitmap = BitmapFactory.New(width, height);

            _gridContext.Bitmap = _gridBitmap;
            _workContext.Bitmap = _workBitmap;

            DrawGrid();
            Draw();

            ViewportImage.Source = _workBitmap;
        }

        private void Draw()
        {
            using (_workBitmap.GetBitmapContext())
            {
                _workBitmap.FromByteArray(_gridBitmap.ToByteArray());
                this.DrawImage();
            }
        }

        /// <summary>
        /// Draws the different types of shapes.
        /// </summary>
        private void DrawGrid()
        {
            
            // Wrap updates in a GetContext call, to prevent invalidation and nested locking/unlocking during this block
            using (_gridBitmap.GetBitmapContext())
            {
                // Init some size vars
                int w = (int)this._gridBitmap.PixelWidth;
                int h = (int)this._gridBitmap.PixelHeight;


                // Clear 
                _gridBitmap.Clear();

                if (_adapter == null)
                {
                    return;
                }

                var options = _adapter.DefineGrid(_staticData, _gridContext);

                if (options == null)
                {
                    return;
                }

                var borderSize = options.BorderLine.Thickness;
                var borderOffset = options.BorderLine.Offset;
                for (int i = 0; i < borderSize; i++)
                {
                    _gridBitmap.DrawRectangle(i + options.BorderLine.Offset, i + options.BorderLine.Offset, w - 1 - i - options.BorderLine.Offset, h - 1 - i - options.BorderLine.Offset, options.BorderLine.Color);
                }

                // рисуем линии и штрихи/черту/отсечки по горизонтальные
                if (options.HorizontalPoints != null && options.HorizontalPoints.Length > 0)
                {
                    var points = options.HorizontalPoints;
                    for (int i = 0; i < points.Length; i++)
                    {
                        var point = points[i];
                        // рисуем линию
                        if (point.Line.HasValue)
                        {
                            var line = point.Line.Value;

                            var x1 = borderSize + borderOffset + 1;
                            var x2 = x1 - 1 + w - 2 * (borderSize + borderOffset);
                            var y1 = h - line.Offset;
                            for (int j = 0; j < line.Thickness; j++)
                            {
                                var y2 = y1 - j;
                                if (x1 >= 0 && x2 >= 0 && y1 >= 0 && y2 >= 0)
                                {
                                    _gridBitmap.DrawLineDotted(x1, y2, x2, y2, line.Space, line.Length, line.Color);
                                }
                                
                            }
                        }
                        // черта с лева
                        if (point.LeftTopDash.HasValue)
                        {
                            var dash = point.LeftTopDash.Value;
                            var x1 = 0;
                            var x2 = x1 + dash.Size;
                            var y1 = h - dash.Offset;
                            var y2 = y1 - dash.Thickness;
                            _gridBitmap.FillRectangle(x1, y1, x2, y2, dash.Color);
                        }
                        // черта с справа
                        if (point.BottomRightDash.HasValue)
                        {
                            var dash = point.BottomRightDash.Value;
                            var x1 = w - dash.Size;
                            var x2 = x1 + dash.Size;
                            var y1 = h - dash.Offset;
                            var y2 = y1 - dash.Thickness;
                            _gridBitmap.FillRectangle(x1, y1, x2, y2, dash.Color);
                        }
                    }
                }

                // рисуем линии и штрихи/черту/отсечки по горизонтальные
                if (options.VerticalPoints != null && options.VerticalPoints.Length > 0)
                {
                    var points = options.VerticalPoints;
                    for (int i = 0; i < points.Length; i++)
                    {
                        var point = points[i];
                        // рисуем линию
                        if (point.Line.HasValue)
                        {
                            var line = point.Line.Value;

                            var y1 = borderSize + 1;
                            var y2 = y1 - 1 + h - 2 * borderSize;
                            var x1 = line.Offset + 1;
                            for (int j = 0; j < line.Thickness; j++)
                            {
                                var x2 = x1 + j;
                                if (x1 >= 0 && x2 >= 0 && y1 >= 0 && y2 >= 0)
                                {
                                    _gridBitmap.DrawLineDotted(x2, y1, x2, y2, line.Space, line.Length, line.Color);
                                }

                            }
                        }
                        // черта в верху
                        if (point.LeftTopDash.HasValue)
                        {
                            var dash = point.LeftTopDash.Value;
                            var y1 = 0;
                            var y2 = y1 + dash.Size;
                            var x1 = dash.Offset;
                            var x2 = x1 + dash.Thickness;
                            _gridBitmap.FillRectangle(x1, y1, x2, y2, dash.Color);
                        }
                        // черта в низу
                        if (point.BottomRightDash.HasValue)
                        {
                            var dash = point.BottomRightDash.Value;
                            var y1 = h - dash.Size;
                            var y2 = y1 + dash.Size;
                            var x1 = dash.Offset;
                            var x2 = x1 + dash.Thickness;
                            _gridBitmap.FillRectangle(x1, y1, x2, y2, dash.Color);
                        }
                    }
                }
            }


            
        }

        private void DrawImage()
        {
            if (_adapter == null)
            {
                return;
            }

            _adapter.DrawImage(_staticData, _dynamicData, _workContext);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.RebuildBitmaps();
        }

        private void ViewPortContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this._notDrawOnResize)
            {
                return;
            }

            try
            {
                this.RebuildBitmaps();
            }
            catch (Exception)
            {
            }
        }



    }
}
