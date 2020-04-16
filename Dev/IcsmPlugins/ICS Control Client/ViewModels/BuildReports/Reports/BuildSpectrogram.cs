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
        private double? GetLeveldBm(PointF[] points, double? val)
        {
            double? level_dBm = null;
            for (int i = 0; i < points.Length; i++)
            {
                if (val == points[i].X)
                {
                    level_dBm = points[i].Y;
                }
                else
                {
                    if ((i + 1) < points.Length)
                    {
                        if ((val > points[i].X) && (val < points[i + 1].X))
                        {
                            level_dBm = (points[i].Y + points[i + 1].Y) / 2;
                        }
                    }
                }
            }
            return level_dBm;
        }
        private ChartOption GetChartOption(DataSynchronizationProcessProtocolsViewModel emit)
        {
            var option = new ChartOption
            {
                Title = "Ref level",
                YLabel = "дБм",
                XLabel = "Частота (MHz)",
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
                    double? freqMiddle_MHz = null;
                    double constStep = 0;
                    var count = emit.ProtocolsLinkedWithEmittings.Levels_dBm.Count();

                    if (emit.ProtocolsLinkedWithEmittings.Bandwidth_kHz != null)
                    {
                        freqMiddle_MHz = (double)(emit.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz.Value + emit.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * ((emit.ProtocolsLinkedWithEmittings.T1.Value + emit.ProtocolsLinkedWithEmittings.T2.Value) / 2000.0));
                    }
                    else
                    {
                        freqMiddle_MHz = (double)((emit.ProtocolsLinkedWithEmittings.StopFrequency_MHz.Value + emit.ProtocolsLinkedWithEmittings.StartFrequency_MHz.Value) / 2);
                    }


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

                    pointsList.Add(new ChartPoints() { Points = points.ToArray(), LineColor = new SolidBrush(Color.Red) });
                    double? level_dBm = null;
                    if (emit.ProtocolsLinkedWithEmittings.T1 != 0)
                    {
                        var val = (emit.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz * 1000000 + emit.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * 1000 * emit.ProtocolsLinkedWithEmittings.T1) / 1000000;
                        level_dBm = GetLeveldBm(points.ToArray(), val);
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.Black), IsHorizontal = false, IsVertical = true, Name = Math.Round(val.Value, 6).ToString(), Num = "M"+ (linesList.Count+1).ToString(), Freq_Mhz = val.Value, level_dBm = level_dBm.Value, LabelLeft = 5, freqMiddle_MHz = freqMiddle_MHz.Value, LabelTop = -25 });
                    }
                    if (emit.ProtocolsLinkedWithEmittings.T2 != 0)
                    {
                        var val = (emit.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz * 1000000 + emit.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * 1000 * emit.ProtocolsLinkedWithEmittings.T2) / 1000000;
                        level_dBm = GetLeveldBm(points.ToArray(), val);
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.Black), IsHorizontal = false, IsVertical = true, Name = Math.Round(val.Value, 6).ToString(), Num = "M" + (linesList.Count + 1).ToString(), Freq_Mhz = val.Value, level_dBm = level_dBm.Value, LabelLeft = 5, freqMiddle_MHz = freqMiddle_MHz.Value, LabelTop = -45 });
                    }
                    if (emit.ProtocolsLinkedWithEmittings.MarkerIndex != 0)
                    {
                        var val = (emit.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz * 1000000 + emit.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz * 1000 * emit.ProtocolsLinkedWithEmittings.MarkerIndex) / 1000000;
                        level_dBm = GetLeveldBm(points.ToArray(), val);
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.Black), IsHorizontal = false, IsVertical = true, Name = Math.Round(val.Value, 6).ToString(), Num = "M" + (linesList.Count+1).ToString(), Freq_Mhz = val.Value, level_dBm = level_dBm.Value, LabelLeft = 5, freqMiddle_MHz = freqMiddle_MHz.Value, LabelTop = -35 });
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

        private PointF DeNormalizePoint(PointF pt, double Width, double Height, ChartOption _option)
        {
            PointF result = new PointF()
            {
                X = (float)(pt.X * (_option.XMax - _option.XMin) / Width) + _option.XMin,
                Y = (float)(((Height - pt.Y) * (_option.YMax - _option.YMin) + _option.YMin * Height) / Height)
            };
            return result;
        }

        public void CreateBitmapSpectrogram(DataSynchronizationProcessProtocolsViewModel emit, Bitmap bm,  int ActualWidth, int ActualHeight)
        {
            var deltaY = -50;
            var _option = GetChartOption(emit);
            var Width = ActualWidth;
            var Height = ActualHeight;
            var cnt = 40;
            var pt = new PointF();
            float dx, dy;
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.Clear(Color.White);
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var stringFont = new Font("Times New Roman", 8);
                Height = Height - cnt;
                var chartRect = new Rectangle() { Size = new Size(Width - cnt - 1, Height), X = cnt, Y = -deltaY, Location = new Point(cnt, -deltaY) };
                gr.DrawRectangle(new Pen(Color.Black, 1), chartRect);
                // Create vertical gridlines:

                var stepX = (float)(_option.XTick / _option.XInnerTickCount);

                List<float> XVal = new List<float>();

                foreach (var points in _option.PointsArray)
                {
                    var line = new List<PointF>();
                    for (int i = 0; i < points.Points.Length; i++)
                    {
                        var point = NormalizePoint(points.Points[i], Width + cnt, Height, _option);
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

                var coordValues = new List<double>();
                foreach (var lines in _option.LinesArray)
                {
                    if (lines.IsVertical)
                    {
                        SizeF size = gr.MeasureString(lines.Name, stringFont);
                        var pointDest = DeNormalizePoint(new PointF(lines.Point.X+size.Width, lines.Point.Y + size.Height), Width, Height, _option);
                        var pointStart = DeNormalizePoint(new PointF(lines.Point.X, lines.Point.Y), Width, Height, _option);
                        coordValues.Add(_option.XMax + (pointDest.X-pointStart.X + stepX*3));
                    }
                }
                if (coordValues.Count > 0)
                {
                    var maxVal = coordValues.Max();
                    if (maxVal > _option.XMax)
                    {
                        _option.XMax = (float)maxVal;
                    }
                }

                for (dx = _option.XMin + stepX*3; dx < _option.XMax; dx += stepX*3)
                {
                    var X1 = NormalizePoint(new PointF(dx, _option.YMin), Width, Height, _option).X;
                    var Y1 = NormalizePoint(new PointF(dx, _option.YMin), Width, Height, _option).Y;
                    var X2 = NormalizePoint(new PointF(dx, _option.YMax), Width, Height, _option).X;
                    var Y2 = NormalizePoint(new PointF(dx, _option.YMax), Width, Height, _option).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1- deltaY, X2 + cnt, Y2- deltaY);
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
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1 - deltaY, X2 + cnt, Y2 - deltaY);
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
                        gr.DrawLine(thick_pen, X1 + cnt, Y1 - deltaY, X2 + cnt, Y2 - deltaY);
                    }
                }



                // Create x-axis tick marks:
                for (dx = _option.XMin; LessOrEqual(dx, _option.XMax); dx += stepX*3/*_option.XTick*/)
                {
                    pt = NormalizePoint(new PointF(dx, _option.YMin), Width, Height, _option);
                    using (Pen thick_pen = new Pen(Color.Black, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        gr.DrawLine(thick_pen, pt.X + cnt, pt.Y + 5 - deltaY, pt.X + cnt, pt.Y - 5 - deltaY);
                    }


                    if (dx == _option.XMin)
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt - 15, pt.Y+5 - deltaY));
                    }
                    else if (LessOrEqual((dx + stepX*3/*_option.XTick*/), _option.XMax))
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt - 15, pt.Y + 5 - deltaY));
                    }
                    else
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt - 15, pt.Y + 5 - deltaY));
                    }

                }



                // Create y-axis tick marks:
                for (dy = _option.YMin; LessOrEqual(dy, _option.YMax); dy += stepY/*_option.YTick*/)
                {
                    pt = NormalizePoint(new PointF(_option.XMin, dy), Width, Height, _option);
                    using (Pen thick_pen = new Pen(Color.Black, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        gr.DrawLine(thick_pen, pt.X + cnt - 5, pt.Y - deltaY, pt.X + cnt + 5, pt.Y - deltaY);
                    }

                    if (dy == _option.YMin)
                    {
                        gr.DrawString(dy.ToString(), stringFont, Brushes.Black, new PointF(pt.X+10, pt.Y - 5 - deltaY));
                    }
                    else if (LessOrEqual((dy + stepY/*_option.YTick*/), _option.YMax))
                    {
                        gr.DrawString(dy.ToString(), stringFont, Brushes.Black, new PointF(pt.X + 10, pt.Y - deltaY));
                    }
                    else
                    {
                        gr.DrawString(dy.ToString(), stringFont, Brushes.Black, new PointF(pt.X + 10, pt.Y + 5 - deltaY));
                    }
                }



                foreach (var points in _option.PointsArray)
                {
                    var line = new List<PointF>();
                    for (int i = 0; i < points.Points.Length; i++)
                    {
                        var point = NormalizePoint(points.Points[i], Width, Height, _option);
                        point.X = point.X + cnt;
                        point.Y = point.Y - deltaY;
                        line.Add(point);
                    }
                    using (Pen thick_pen = new Pen(points.LineColor, 1))
                    {
                        gr.DrawLines(thick_pen, line.ToArray());
                    }
                }

                int NumLine = 1;
                foreach (var lines in _option.LinesArray)
                {
                    gr.DrawString($"{lines.Num}: {lines.Name} МГц", stringFont, lines.LineColor, NumLine*(Width/3)-300, 10);
                    gr.DrawString($"{Math.Round(lines.level_dBm, 6).ToString()} дБм", stringFont, lines.LineColor, NumLine * (Width / 3) - 300, 30);
                    NumLine++;
                }


                foreach (var lines in _option.LinesArray)
                {
                    if (lines.IsHorizontal)
                    {
                        var line = new List<PointF>();

                        var point = this.NormalizePoint(lines.Point, Width, Height, _option);
                        line.Add(new PointF() { X = 0, Y = point.Y - deltaY });
                        line.Add(new PointF() { X = Width, Y = point.Y - deltaY });

                        using (Pen thick_pen = new Pen(lines.LineColor, 3))
                        {
                            gr.DrawLines(thick_pen, line.ToArray());
                        }


                        if (!string.IsNullOrEmpty(lines.Num))
                        {
                            gr.DrawString(lines.Num, stringFont, lines.LineColor, new PointF((float)(lines.LabelLeft), 10- deltaY));
                        }

                    }
                    if (lines.IsVertical)
                    {
                        var line = new List<PointF>();

                        var point = this.NormalizePoint(lines.Point, Width, Height, _option);
                        line.Add(new PointF() { X = point.X + cnt, Y = -deltaY });
                        line.Add(new PointF() { X = point.X + cnt, Y = Height - deltaY });

                        using (Pen thick_pen = new Pen(lines.LineColor, 3))
                        {
                            gr.DrawLines(thick_pen, line.ToArray());
                        }

                        if (!string.IsNullOrEmpty(lines.Num))
                        {
                            gr.DrawString(lines.Num, stringFont, lines.LineColor, new PointF((float)(point.X + cnt + lines.LabelLeft), 10- deltaY));
                        }
                    }
                }
                //System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                //drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                gr.DrawString(_option.XLabel, stringFont, Brushes.Black, new PointF((float)Width / 2, Height - deltaY + 25));
                gr.DrawString(_option.YLabel, stringFont, Brushes.Black, new PointF(15, 10));
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
