using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.Windows.Shapes;
using XICSM.ICSControlClient;

namespace XICSM.ICSControlClient.WpfControls.Charts
{
    public enum LinePatternEnum
    {
        Solid = 1,
        Dash = 2,
        Dot = 3,
        DashDot = 4,
        None = 5
    }

    public enum GridlinePatternEnum
    {
        Solid = 1,
        Dash = 2,
        Dot = 3,
        DashDot = 4
    }

    public partial class LineChart : UserControl
    {
        private Point startPoint;
        private Rectangle rect;

        private ChartOption _option;
        private ChartLineStyle _lineStyle;
        private ChartColumnsStyle _columnsStyle;
        private ChartGridStyle _gridStyle;

        private double[] _selectedRangeX;

        private static ChartOption GetDefaultChartOption()
        {
            return new ChartOption
            {
                ChartType = ChartType.Line,
                Points = new Point[] { },
                XMin = 0,
                XMax = 10,
                XTick = 1,
                XInnerTickCount = 5,
                XLabel = "X - Label",
                Title = "Some Chart",
                YMin = 0,
                YMax = 10,
                YTick = 1,
                YInnerTickCount = 5,
                YLabel = "Y - Label",
                UseZoom = false
            };
        }
        public LineChart()
        {
            InitializeComponent();

            this._option = LineChart.GetDefaultChartOption();

            this._gridStyle = new ChartGridStyle
            {
                BottomOffset = 15,
                LeftOffset = 20,
                RightOffset = 10,
                InnerLineColor = Brushes.Gray,
                LineColor = Brushes.Gray,
                InnerLinePattern = GridlinePatternEnum.Dot,
                LinePattern = GridlinePatternEnum.DashDot,
                IsXGrid = true,
                IsXInnerGrid = true,
                IsYGrid = true,
                IsYInnerGrid = true,
                InnerLineThickness = 1,
                LineThickness = 1
            };

            this._lineStyle = new ChartLineStyle
            {
                Color = Brushes.Black,
                Thickness = 1,
                Pattern = LinePatternEnum.Solid
            };

            this._columnsStyle = new ChartColumnsStyle
            {
                FillColor = Brushes.RoyalBlue,
                StrokeColor = Brushes.DarkBlue,
                Thickness = 1,
                Pattern = LinePatternEnum.Solid
            };
            
        }
        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawChart();
        }

        private void DrawChart()
        {
            //if (this._option == null)
            //{
            //    this._option = this.GetDefaultChartOption();
            //}

            textCanvas.Width = chartGrid.ActualWidth;
            textCanvas.Height = chartGrid.ActualHeight;
            chartCanvas.Children.RemoveRange(1, chartCanvas.Children.Count - 1);
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);

            rect = new Rectangle()
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(50, 0, 100, 0))
            };
            chartCanvas.Children.Add(rect);

            this.DrawGridlines();

            if(this._option.Points == null && this._option.PointsArray == null)
            {
                return;
            }

            if (this._option.ChartType == ChartType.Line)
            {
                if (this._option.PointsArray == null)
                    this.DrawLine();
                else
                    this.DrawLines();
            }
            else if (this._option.ChartType == ChartType.Columns)
            {
                this.DrawColumns();
            }

        }

        public void DrawGridlines()
        {
            Point pt = new Point();
            Line tick = new Line();
            double offset = 0;
            double dx, dy;

            TextBlock tb = new TextBlock();

            //  determine right offset:
            tb.Text = this._option.XMax.ToString();
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size size = tb.DesiredSize;
            var rightOffset = size.Width / 2 + 2;

            // Determine left offset:
            for (dy = this._option.YMin; dy <= this._option.YMax; dy += this._option.YTick)
            {
                pt = NormalizePoint(new Point(this._option.XMin, dy));
                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.TextAlignment = TextAlignment.Right;
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                if (offset < size.Width)
                    offset = size.Width;
            }
            var leftOffset = offset + 5;
            var bottomOffset = this._gridStyle.BottomOffset;

            Canvas.SetLeft(chartCanvas, leftOffset);
            Canvas.SetBottom(chartCanvas, bottomOffset);
            chartCanvas.Width = Math.Abs(textCanvas.Width - leftOffset - rightOffset);
            chartCanvas.Height = Math.Abs(textCanvas.Height - bottomOffset - size.Height / 2);
            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = chartCanvas.Width;
            chartRect.Height = chartCanvas.Height;
            chartCanvas.Children.Add(chartRect);

            // Create vertical gridlines:
            if (this._gridStyle.IsYGrid == true)
            {
                if (this._gridStyle.IsYInnerGrid)
                {
                    var step = this._option.XTick / this._option.XInnerTickCount;
                    for (dx = this._option.XMin + step; dx < this._option.XMax; dx += step)
                    {
                        var gridline = new Line();
                        this.ApplyStyleToInnerGridLine(gridline);
                        gridline.X1 = NormalizePoint(new Point(dx, this._option.YMin)).X;
                        gridline.Y1 = NormalizePoint(new Point(dx, this._option.YMin)).Y;
                        gridline.X2 = NormalizePoint(new Point(dx, this._option.YMax)).X;
                        gridline.Y2 = NormalizePoint(new Point(dx, this._option.YMax)).Y;
                        chartCanvas.Children.Add(gridline);
                    }

                }
                for (dx = this._option.XMin + this._option.XTick; dx < this._option.XMax; dx += this._option.XTick)
                {
                    var gridline = new Line();
                    this.ApplyStyleToGridLine(gridline);
                    gridline.X1 = NormalizePoint(new Point(dx, this._option.YMin)).X;
                    gridline.Y1 = NormalizePoint(new Point(dx, this._option.YMin)).Y;
                    gridline.X2 = NormalizePoint(new Point(dx, this._option.YMax)).X;
                    gridline.Y2 = NormalizePoint(new Point(dx, this._option.YMax)).Y;
                    chartCanvas.Children.Add(gridline);
                }
            }

            // Create horizontal gridlines:
            if (this._gridStyle.IsXGrid == true)
            {
                if (this._gridStyle.IsXInnerGrid)
                {
                    var step = this._option.YTick / this._option.YInnerTickCount;
                    for (dy = this._option.YMin + step; dy < this._option.YMax; dy += step)
                    {
                        var gridline = new Line();
                        this.ApplyStyleToInnerGridLine(gridline);
                        gridline.X1 = NormalizePoint(new Point(this._option.XMin, dy)).X;
                        gridline.Y1 = NormalizePoint(new Point(this._option.XMin, dy)).Y;
                        gridline.X2 = NormalizePoint(new Point(this._option.XMax, dy)).X;
                        gridline.Y2 = NormalizePoint(new Point(this._option.XMax, dy)).Y;
                        chartCanvas.Children.Add(gridline);
                    }

                }
                for (dy = this._option.YMin + this._option.YTick; dy < this._option.YMax; dy += this._option.YTick)
                {
                    var gridline = new Line();
                    this.ApplyStyleToGridLine(gridline);
                    gridline.X1 = NormalizePoint(new Point(this._option.XMin, dy)).X;
                    gridline.Y1 = NormalizePoint(new Point(this._option.XMin, dy)).Y;
                    gridline.X2 = NormalizePoint(new Point(this._option.XMax, dy)).X;
                    gridline.Y2 = NormalizePoint(new Point(this._option.XMax, dy)).Y;
                    chartCanvas.Children.Add(gridline);
                }
            }

            // Create x-axis tick marks:
            for (dx = this._option.XMin; dx.LessOrEqual(this._option.XMax); dx += this._option.XTick)
            {
                pt = NormalizePoint(new Point(dx, this._option.YMin));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y + 5;
                tick.X2 = pt.X;
                tick.Y2 = pt.Y - 5;
                chartCanvas.Children.Add(tick);

                tb = new TextBlock();
                tb.Text = dx.ToString();
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                textCanvas.Children.Add(tb);
                Canvas.SetLeft(tb, leftOffset + pt.X - size.Width / 2);
                Canvas.SetTop(tb, pt.Y + 2 + size.Height / 2);

                if (dx == this._option.XMin)
                {
                    Canvas.SetLeft(tb, size.Width / 2 + leftOffset + pt.X - size.Width / 2);
                }
                else if ((dx + this._option.XTick).LessOrEqual(this._option.XMax))
                {
                    Canvas.SetLeft(tb, leftOffset + pt.X - size.Width / 2);
                }
                else
                {
                    Canvas.SetLeft(tb, (leftOffset + pt.X - size.Width / 2) - size.Width / 2);
                }
            }

            // Create y-axis tick marks:
            for (dy = this._option.YMin; dy.LessOrEqual(this._option.YMax); dy += this._option.YTick)
            {
                pt = NormalizePoint(new Point(this._option.XMin, dy));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X - 5;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X + 5;
                tick.Y2 = pt.Y;
                chartCanvas.Children.Add(tick);

                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                textCanvas.Children.Add(tb);
                Canvas.SetRight(tb, chartCanvas.Width + rightOffset + 3);

                if (dy == this._option.YMin)
                {
                    Canvas.SetTop(tb, pt.Y - 5);
                }
                else if ((dy + this._option.YTick).LessOrEqual(this._option.YMax))
                {
                    Canvas.SetTop(tb, pt.Y);
                }
                else
                {
                    Canvas.SetTop(tb, pt.Y + 5);
                }

            }

            // Add title and labels:
            tbTitle.Text = this._option.Title;
            tbXLabel.Text = this._option.XLabel;
            tbYLabel.Text = this._option.YLabel;
            tbXLabel.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
            tbTitle.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
        }
        private void ApplyStyleToGridLine(Line gridLine)
        {
            gridLine.Stroke = this._gridStyle.LineColor;
            gridLine.StrokeThickness = this._gridStyle.LineThickness;

            switch (this._gridStyle.LinePattern)
            {
                case GridlinePatternEnum.Dash:
                    gridLine.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case GridlinePatternEnum.Dot:
                    gridLine.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case GridlinePatternEnum.DashDot:
                    gridLine.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }

        private void ApplyStyleToInnerGridLine(Line gridLine)
        {
            gridLine.Stroke = this._gridStyle.InnerLineColor;
            gridLine.StrokeThickness = this._gridStyle.InnerLineThickness;

            switch (this._gridStyle.InnerLinePattern)
            {
                case GridlinePatternEnum.Dash:
                    gridLine.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case GridlinePatternEnum.Dot:
                    gridLine.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case GridlinePatternEnum.DashDot:
                    gridLine.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }

        private void DrawLine()
        {
            var line = new Polyline();
            this.ApplyStyleToLine(line);
            
            for (int i = 0; i < _option.Points.Length; i++)
            {
                var point = this.NormalizePoint(_option.Points[i]);
                line.Points.Add(point);
            }
            chartCanvas.Children.Add(line);
        }
        private void DrawLines()
        {
            foreach (var points in _option.PointsArray)
            {
                var line = new Polyline();
                this.ApplyStyleToLine(line, points.LineColor);

                for (int i = 0; i < points.Points.Length; i++)
                {
                    var point = this.NormalizePoint(points.Points[i]);
                    line.Points.Add(point);
                }
                chartCanvas.Children.Add(line);
            }
        }
        private void ApplyStyleToLine(Polyline line, Brush lineColor)
        {
            line.Stroke = lineColor;
            line.StrokeThickness = this._lineStyle.Thickness;

            switch (this._lineStyle.Pattern)
            {
                case LinePatternEnum.Dash:
                    line.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case LinePatternEnum.Dot:
                    line.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case LinePatternEnum.DashDot:
                    line.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
                case LinePatternEnum.None:
                    line.Stroke = Brushes.Transparent;
                    break;
            }
        }
        private void ApplyStyleToLine(Polyline line)
        {
            this.ApplyStyleToLine(line, this._lineStyle.Color);
        }

        private void DrawColumns()
        {
            var points = this._option.Points;
            var normolizedPoints = points.Select(p => this.NormalizePoint(p)).ToArray();

            if (normolizedPoints.Length > 2)
            {
                for (int i = 0; i < points.Length; i++)

                {
                    var rect = new Rectangle();
                    this.ApplyStyleToColumnRectangle(rect);

                    var currentPoint = normolizedPoints[i];
                    var posX = currentPoint.X;
                    var posY = currentPoint.Y;

                    rect.Height = chartCanvas.Height - Math.Abs(currentPoint.Y);

                    if (i == 0)
                    {
                        var nextPoint = normolizedPoints[i + 1];
                        rect.Width = 0.9 * ((nextPoint.X - currentPoint.X) / 2);
                    }
                    else if (i == normolizedPoints.Length - 1)
                    {
                        var prevPoint = normolizedPoints[i - 1];
                        rect.Width = 0.9 * ((currentPoint.X - prevPoint.X) / 2);
                        posX = posX - (0.9 * ((currentPoint.X - prevPoint.X) / 2));
                    }
                    else
                    {
                        var nextPoint = normolizedPoints[i + 1];
                        var prevPoint = normolizedPoints[i - 1];
                        rect.Width = 0.9 * ((nextPoint.X - currentPoint.X) / 2 + (currentPoint.X - prevPoint.X) / 2);
                        posX = posX - (0.9 * ((currentPoint.X - prevPoint.X) / 2));
                    }

                    chartCanvas.Children.Add(rect);
                    Canvas.SetLeft(rect, posX);
                    Canvas.SetTop(rect, posY);
                }

            }
            else if (normolizedPoints.Length == 1)
            {
                var rect = new Rectangle();
                this.ApplyStyleToColumnRectangle(rect);

                var currentPoint = normolizedPoints[0];

                var posX = currentPoint.X - 5;
                var posY = currentPoint.Y;
                rect.Height = Math.Abs(currentPoint.Y);
                rect.Width = 10;
                chartCanvas.Children.Add(rect);
                Canvas.SetLeft(rect, posX);
                Canvas.SetTop(rect, posY);
            }
        }

        private Point NormalizePoint(Point pt)
        {
            if (chartCanvas.Width.ToString() == "NaN")
                chartCanvas.Width = 270;
            if (chartCanvas.Height.ToString() == "NaN")
                chartCanvas.Height = 250;
            Point result = new Point();
            result.X = (pt.X - this._option.XMin) * chartCanvas.Width / (this._option.XMax - this._option.XMin);
            result.Y = chartCanvas.Height - (pt.Y - this._option.YMin) * chartCanvas.Height / (this._option.YMax - this._option.YMin);
            return result;
        }
        private Point DeNormalizePoint(Point pt)
        {
            if (chartCanvas.Width.ToString() == "NaN")
                chartCanvas.Width = 270;
            if (chartCanvas.Height.ToString() == "NaN")
                chartCanvas.Height = 250;
            Point result = new Point();
            result.X = (pt.X * (this._option.XMax - this._option.XMin) / chartCanvas.Width) + this._option.XMin;
            result.Y = ((chartCanvas.Height - pt.Y) * (this._option.YMax - this._option.YMin) + this._option.YMin * chartCanvas.Height) / chartCanvas.Height;
            return result;
        }

        private void ApplyStyleToColumnRectangle(Rectangle rect)
        {
            rect.Fill = this._columnsStyle.FillColor;
            rect.Stroke = this._columnsStyle.StrokeColor;
        }



        public static DependencyProperty OptionProperty = DependencyProperty.Register("Option", typeof(ChartOption), typeof(LineChart), 
           new FrameworkPropertyMetadata(default(ChartOption), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPropertyChanged)));

        public ChartOption Option
        {
            get { return this._option; } // (ChartOption)GetValue(OptionProperty); }
            set
            {
                //SetValue(OptionProperty, value);
                this._option = value;
                DrawChart();
            }
        }

        public static DependencyProperty IsXGridProperty = DependencyProperty.Register("IsXGrid", typeof(bool), typeof(LineChart),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));

        public bool IsXGrid
        {
            get { return (bool)GetValue(IsXGridProperty); }
            set
            {
                SetValue(IsXGridProperty, value);
                this._gridStyle.IsXGrid = value;
            }
        }

        public static DependencyProperty IsYGridProperty = DependencyProperty.Register("IsYGrid", typeof(bool), typeof(LineChart),
           new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));

        public bool IsYGrid
        {
            get { return (bool)GetValue(IsYGridProperty); }
            set
            {
                SetValue(IsYGridProperty, value);
                this._gridStyle.IsYGrid = value;
            }
        }

        public static DependencyProperty GridlineColorProperty = DependencyProperty.Register("GridlineColor", typeof(Brush), typeof(LineChart),
           new FrameworkPropertyMetadata(Brushes.Gray, new PropertyChangedCallback(OnPropertyChanged)));

        public Brush GridlineColor
        {
            get { return (Brush)GetValue(GridlineColorProperty); }
            set
            {
                SetValue(GridlineColorProperty, value);
                this._gridStyle.LineColor = value;
            }
        }

        public static DependencyProperty GridlinePatternProperty = DependencyProperty.Register("GridlinePattern",
            typeof(GridlinePatternEnum), typeof(LineChart),
            new FrameworkPropertyMetadata(GridlinePatternEnum.Solid,
            new PropertyChangedCallback(OnPropertyChanged)));

        public GridlinePatternEnum GridlinePattern
        {
            get { return (GridlinePatternEnum)GetValue(GridlinePatternProperty); }
            set
            {
                SetValue(GridlinePatternProperty, value);
                this._gridStyle.LinePattern = value;
            }
        }

        public static DependencyProperty SelectedRangeXProperty = DependencyProperty.Register("SelectedRangeX", typeof(double[]), typeof(LineChart),
           new FrameworkPropertyMetadata(default(double[]), new PropertyChangedCallback(OnPropertyChanged)));

        public double[] SelectedRangeX
        {
            get { return (double[])GetValue(SelectedRangeXProperty); } 
            set
            {
                SetValue(SelectedRangeXProperty, value);
                this._selectedRangeX = value;
            }
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            LineChart lcc = sender as LineChart;
           
            if (e.Property == GridlinePatternProperty)
                lcc.GridlinePattern = (GridlinePatternEnum)e.NewValue;
            else if (e.Property == GridlineColorProperty)
                lcc.GridlineColor = (Brush)e.NewValue;
            else if (e.Property == IsXGridProperty)
                lcc.IsXGrid = (bool)e.NewValue;
            else if (e.Property == IsYGridProperty)
                lcc.IsYGrid = (bool)e.NewValue;
            else if (e.Property == OptionProperty)
                lcc.Option = (ChartOption)e.NewValue;
            //else if (e.Property == SelectedRangeXProperty)
            //    lcc.SelectedRangeX = (double[])e.NewValue;

        }

        private void chartCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this._option.UseZoom)
                {
                    startPoint = e.GetPosition(chartCanvas);

                    Canvas.SetLeft(rect, startPoint.X);
                    Canvas.SetTop(rect, 0);
                }
            }
            catch (Exception msg)
            {
                Debug.Print(msg.Message);
            }
        }
        private void chartCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (this._option.UseZoom)
                {
                    if (e.LeftButton == MouseButtonState.Released || rect == null)
                        return;

                    var pos = e.GetPosition(chartCanvas);

                    var x = Math.Min(pos.X, startPoint.X);
                    var y = Math.Min(pos.Y, 0);
                    var w = Math.Max(pos.X, startPoint.X) - x;
                    var h = Math.Max(pos.Y, startPoint.Y) - y;

                    rect.Width = w;
                    rect.Height = chartCanvas.ActualHeight;

                    Canvas.SetLeft(rect, x);
                    Canvas.SetTop(rect, y);
                }
            }
            catch (Exception msg)
            {
                Debug.Print(msg.Message);
            }
        }

        private void chartCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this._option.UseZoom)
            {
                var pos = e.GetPosition(chartCanvas);
                Point point1 = new Point() { X = startPoint.X, Y = startPoint.Y};
                Point point2 = new Point() { X = pos.X, Y = pos.Y };

                var realPoint1 = DeNormalizePoint(point1);
                var realPoint2 = DeNormalizePoint(point2);

                double[] result;
                if (realPoint1.X > realPoint2.X)
                    result = new double[] { realPoint2.X, realPoint1.X };
                else
                    result = new double[] { realPoint1.X, realPoint2.X };

                this.SelectedRangeX = result;
            }
        }
    }
}
