using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;
using System.Drawing;
using XICSM.ICSControlClient.ViewModels.Chart;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment;


namespace XICSM.ICSControlClient.ViewModels.Reports
{
    /// <summary>
    ///  The spectrogram building class
    /// </summary>
    public class BuildSpectrogram
    {
        private ChartOption GetChartOption(DataSynchronizationProcessProtocolsViewModel emit)
        {
            var option = new ChartOption
            {
                Title = "Ref level",
                YLabel = "Level (dBm)",
                XLabel = "Freq (MHz)",
                ChartType = ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10,
                UseZoom = true,
                IsEnableSaveToFile = true
            };

            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);

            var linesList = new List<ChartLine>();
            var pointsList = new List<ChartPoints>();



            if (emit != null)
            {
                int j = 0;
                if ((emit.ProtocolsLinkedWithEmittings!=null) && (emit.ProtocolsLinkedWithEmittings.Levels_dBm!= null))
                {
                    double constStep = 0;
                    var count = emit.ProtocolsLinkedWithEmittings.Levels_dBm.Count();
                    var points = new List<PointF>();
                    for (int i = 0; i < count; i++)
                    {
                        double valX = (double)((emit.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz * 1000000 + emit.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * 1000 * i) / 1000000);
                        double valY = (double)emit.ProtocolsLinkedWithEmittings.Levels_dBm[i] + constStep;

                        var point = new PointF
                        {
                            X = (float)valX,
                            Y = (float)valY
                        };

                        if (j == 0)
                        {
                            maxX = valX;
                            minX = valX;
                            maxY = valY;
                            minY = valY;
                        }

                        if (maxX < valX)
                            maxX = valX;
                        if (minX > valX)
                            minX = valX;
                        if (maxY < valY)
                            maxY = valY;
                        if (minY > valY)
                            minY = valY;

                        points.Add(point);
                        j++;
                    }

                    pointsList.Add(new ChartPoints() { Points = points.ToArray(), LineColor = new SolidBrush(Color.DarkRed) });
                    if (emit.ProtocolsLinkedWithEmittings.T1 != 0)
                    {
                        var val = (emit.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz * 1000000 + emit.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * 1000 * emit.ProtocolsLinkedWithEmittings.T1) / 1000000;
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.DarkRed), IsHorizontal = false, IsVertical = true, Name = Math.Round(val.Value, 6).ToString(), LabelLeft = 5, LabelTop = -25 });
                    }
                    if (emit.ProtocolsLinkedWithEmittings.T2 != 0)
                    {
                        var val = (emit.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz * 1000000 + emit.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * 1000 * emit.ProtocolsLinkedWithEmittings.T2) / 1000000;
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.DarkRed), IsHorizontal = false, IsVertical = true, Name = Math.Round(val.Value, 6).ToString(), LabelLeft = 5, LabelTop = -45 });
                    }
                    if (emit.ProtocolsLinkedWithEmittings.MarkerIndex != 0)
                    {
                        var val = (emit.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz * 1000000 + emit.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * 1000 * emit.ProtocolsLinkedWithEmittings.MarkerIndex) / 1000000;
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.DarkRed), IsHorizontal = false, IsVertical = true, Name = Math.Round(val.Value, 6).ToString(), LabelLeft = 5, LabelTop = -35 });
                    }
                }
            }

            if (pointsList.Count == 0)
                return option;

            var preparedDataY = Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 20;
            option.YMin = (float)preparedDataY.MinValue;
            option.YMax = (float)preparedDataY.MaxValue;

            var preparedDataX = Utitlity.CalcFrequencyRange(minX, maxX, 20);
            option.XTick = (float)preparedDataX.Step;
            option.XMin = (float)preparedDataX.MinValue;// + preparedDataX.Step;
            option.XMax = (float)preparedDataX.MaxValue;// - preparedDataX.Step;


            option.PointsArray = pointsList.ToArray();
            option.LinesArray = linesList.ToArray();
            return option;
        }

        private PointF NormalizePoint(PointF pt, double Width, double Height, ChartOption _option)
        {
            PointF result = new PointF()
            {
                X = (float)((pt.X - _option.XMin) * Width / (_option.XMax - _option.XMin)),
                Y = (float)(Height - (pt.Y - _option.YMin) * Height / (_option.YMax - _option.YMin))
            };
            return result;
        }

        public void CreateBitmapSpectrogram(DataSynchronizationProcessProtocolsViewModel emit, Bitmap bm,  int ActualWidth, int ActualHeight)
        {
            var _option = GetChartOption(emit);
            var Width = ActualWidth;
            var Height = ActualHeight;
            var cnt = 30;
            var pt = new PointF();
            float dx, dy;
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.Clear(Color.WhiteSmoke);
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var stringFont = new Font("Arial", 8);
                Height = Height - cnt;
                var chartRect = new Rectangle() { Size = new Size(Width - cnt - 1, Height), X = cnt, Y = 0, Location = new Point(cnt, 0) };
                gr.DrawRectangle(new Pen(Color.Black, 1), chartRect);
                // Create vertical gridlines:

                var stepX = (float)(_option.XTick / _option.XInnerTickCount);

                List<float> XVal = new List<float>();

                foreach (var points in _option.PointsArray)
                {
                    var line = new List<PointF>();
                    for (int i = 0; i < points.Points.Length; i++)
                    {
                        var point = NormalizePoint(points.Points[i], Width, Height, _option);
                        XVal.Add(points.Points[i].X);
                    }

                }

                var minX = XVal.Min();
                var maxX = XVal.Max();

                var deltaXMin = minX - _option.XMin;
                var deltaXMax = _option.XMax - maxX;
                var minVal = Math.Min(deltaXMin, deltaXMax);
                if (deltaXMin < 0)
                {
                    _option.XMin = _option.XMin + deltaXMin;
                }
                if (deltaXMax < 0)
                {
                    _option.XMax = _option.XMax - deltaXMax;
                }

                //if (stepX > minVal)
                //{
                //_option.XMax = _option.XMax + 3 *stepX;
                //}


                for (dx = _option.XMin + stepX; dx < _option.XMax; dx += stepX)
                {
                    var X1 = NormalizePoint(new PointF(dx, _option.YMin), Width, Height, _option).X;
                    var Y1 = NormalizePoint(new PointF(dx, _option.YMin), Width, Height, _option).Y;
                    var X2 = NormalizePoint(new PointF(dx, _option.YMax), Width, Height, _option).X;
                    var Y2 = NormalizePoint(new PointF(dx, _option.YMax), Width, Height, _option).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1, X2 + cnt, Y2);
                    }
                }
                for (dx = _option.XMin + _option.XTick; dx < _option.XMax; dx += _option.XTick)
                {
                    var X1 = NormalizePoint(new PointF(dx, _option.YMin), Width, Height, _option).X;
                    var Y1 = NormalizePoint(new PointF(dx, _option.YMin), Width, Height, _option).Y;
                    var X2 = NormalizePoint(new PointF(dx, _option.YMax), Width, Height, _option).X;
                    var Y2 = NormalizePoint(new PointF(dx, _option.YMax), Width, Height, _option).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1, X2 + cnt, Y2);
                    }
                }

                // Create horizontal gridlines:

                var stepY = _option.YTick / _option.YInnerTickCount;
                for (dy = _option.YMin + stepY; dy < _option.YMax; dy += stepY)
                {
                    var X1 = NormalizePoint(new PointF(_option.XMin, dy), Width, Height, _option).X;
                    var Y1 = NormalizePoint(new PointF(_option.XMin, dy), Width, Height, _option).Y;
                    var X2 = NormalizePoint(new PointF(_option.XMax, dy), Width, Height, _option).X;
                    var Y2 = NormalizePoint(new PointF(_option.XMax, dy), Width, Height, _option).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1, X2 + cnt, Y2);
                    }
                }

                for (dy = _option.YMin + _option.YTick; dy < _option.YMax; dy += _option.YTick)
                {
                    var X1 = NormalizePoint(new PointF(_option.XMin, dy), Width, Height, _option).X;
                    var Y1 = NormalizePoint(new PointF(_option.XMin, dy), Width, Height, _option).Y;
                    var X2 = NormalizePoint(new PointF(_option.XMax, dy), Width, Height, _option).X;
                    var Y2 = NormalizePoint(new PointF(_option.XMax, dy), Width, Height, _option).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1, X2 + cnt, Y2);
                    }
                }



                // Create x-axis tick marks:
                for (dx = _option.XMin; LessOrEqual(dx, _option.XMax); dx += _option.XTick)
                {
                    pt = NormalizePoint(new PointF(dx, _option.YMin), Width, Height, _option);
                    using (Pen thick_pen = new Pen(Color.Black, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        gr.DrawLine(thick_pen, pt.X + cnt, pt.Y + 5, pt.X + cnt, pt.Y - 5);
                    }


                    if (dx == _option.XMin)
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt - 15, pt.Y+5));
                    }
                    else if (LessOrEqual((dx + _option.XTick), _option.XMax))
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt - 15, pt.Y + 5));
                    }
                    else
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt - 15, pt.Y + 5));
                    }

                }



                // Create y-axis tick marks:
                for (dy = _option.YMin; LessOrEqual(dy, _option.YMax); dy += _option.YTick)
                {
                    pt = NormalizePoint(new PointF(_option.XMin, dy), Width, Height, _option);
                    using (Pen thick_pen = new Pen(Color.Black, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        gr.DrawLine(thick_pen, pt.X + cnt - 5, pt.Y, pt.X + cnt + 5, pt.Y);
                    }

                    if (dy == _option.YMin)
                    {
                        gr.DrawString(dy.ToString(), stringFont, Brushes.Black, new PointF(pt.X, pt.Y - 5));
                    }
                    else if (LessOrEqual((dy + _option.YTick), _option.YMax))
                    {
                        gr.DrawString(dy.ToString(), stringFont, Brushes.Black, new PointF(pt.X, pt.Y));
                    }
                    else
                    {
                        gr.DrawString(dy.ToString(), stringFont, Brushes.Black, new PointF(pt.X, pt.Y + 5));
                    }
                }



                foreach (var points in _option.PointsArray)
                {
                    var line = new List<PointF>();
                    for (int i = 0; i < points.Points.Length; i++)
                    {
                        var point = NormalizePoint(points.Points[i], Width, Height, _option);
                        point.X = point.X + cnt;
                        line.Add(point);
                    }
                    using (Pen thick_pen = new Pen(points.LineColor, 1))
                    {
                        gr.DrawLines(thick_pen, line.ToArray());
                    }
                }


                foreach (var lines in _option.LinesArray)
                {
                    if (lines.IsHorizontal)
                    {
                        var line = new List<PointF>();

                        var point = this.NormalizePoint(lines.Point, Width, Height, _option);
                        line.Add(new PointF() { X = 0, Y = point.Y });
                        line.Add(new PointF() { X = Width, Y = point.Y });

                        using (Pen thick_pen = new Pen(lines.LineColor, 1))
                        {
                            gr.DrawLines(thick_pen, line.ToArray());
                        }


                        if (!string.IsNullOrEmpty(lines.Name))
                        {
                            gr.DrawString(lines.Name, stringFont, lines.LineColor, new PointF((float)(lines.LabelLeft), (float)(point.Y + lines.LabelTop)));
                        }

                    }
                    if (lines.IsVertical)
                    {
                        var line = new List<PointF>();

                        var point = this.NormalizePoint(lines.Point, Width, Height, _option);
                        line.Add(new PointF() { X = point.X + cnt, Y = 0 });
                        line.Add(new PointF() { X = point.X + cnt, Y = Height });

                        using (Pen thick_pen = new Pen(lines.LineColor, 1))
                        {
                            gr.DrawLines(thick_pen, line.ToArray());
                        }

                        if (!string.IsNullOrEmpty(lines.Name))
                        {
                            gr.DrawString(lines.Name, stringFont, lines.LineColor, new PointF((float)(point.X + cnt + lines.LabelLeft), (float)(Height + lines.LabelTop)));
                        }
                    }
                }
                System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                gr.DrawString(_option.XLabel, stringFont, Brushes.Black, new PointF((float)Width / 2, Height + 15));
                gr.DrawString(_option.YLabel, stringFont, Brushes.Black, new PointF(2, ((float)Height / 2)-10), drawFormat);
            }
        }

        private bool LessOrEqual(double value, double target)
        {
            if (value <= target)
            {
                return true;
            }

            var delate = Math.Abs(value - target);

            return delate <= 0.0000001;
        }
    }
}
