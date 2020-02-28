using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;
using Atdi.Modules.Sdrn.Chart;
using System.Drawing;
using Atdi.Modules.Sdrn.Calculation;



namespace Atdi.Modules.Sdrn.Reports
{
    /// <summary>
    ///  The spectrogram building class
    /// </summary>
    public class BuildSpectrogram
    {
        private ChartOption _option;

        private ChartOption GetChartOption(Emitting emit, ReferenceLevels referenceLevels)
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


            if (emit != null && referenceLevels != null && referenceLevels.levels != null)
            {
                var count = referenceLevels.levels.Length;
                var points = new List<PointF>();

                int j = 0;
                for (int i = 0; i < count; i++)
                {
                    var valX = (referenceLevels.StartFrequency_Hz + referenceLevels.StepFrequency_Hz * i) / 1000000;
                    var valY = referenceLevels.levels[i];

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
                    else
                    {
                        if (maxX < valX)
                            maxX = valX;
                        if (minX > valX)
                            minX = valX;
                        if (maxY < valY)
                            maxY = valY;
                        if (minY > valY)
                            minY = valY;
                    }
                    points.Add(point);
                    j++;
                }

                pointsList.Add(new ChartPoints() { Points = points.ToArray(), LineColor = new SolidBrush(Color.DarkBlue) });
            }

            if (emit != null)
            {
                int j = 0;
                if ((emit.Spectrum!=null) && (emit.Spectrum.Levels_dBm!= null))
                {
                    
                    double constStep = 0;
                    if (referenceLevels != null && referenceLevels.levels != null && Math.Abs(referenceLevels.StepFrequency_Hz - emit.Spectrum.SpectrumSteps_kHz) > 0.01 && referenceLevels.StepFrequency_Hz != 0)
                    {
                        constStep = -10 * Math.Log10(emit.Spectrum.SpectrumSteps_kHz * 1000 / referenceLevels.StepFrequency_Hz);
                    }

                    var count = emit.Spectrum.Levels_dBm.Count();
                    var points = new List<PointF>();

                    for (int i = 0; i < count; i++)
                    {
                        double valX = (double)((emit.Spectrum.SpectrumStartFreq_MHz * 1000000 + emit.Spectrum.SpectrumSteps_kHz * 1000 * i) / 1000000);
                        double valY = (double)emit.Spectrum.Levels_dBm[i] + constStep;


                        var point = new PointF
                        {
                            X = (float)valX,
                            Y = (float)valY
                        };

                        if ((referenceLevels == null || referenceLevels.levels == null) && j == 0)
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
                    if (emit.Spectrum.T1 != 0)
                    {
                        var val = (emit.Spectrum.SpectrumStartFreq_MHz * 1000000 + emit.Spectrum.SpectrumSteps_kHz * 1000 * emit.Spectrum.T1) / 1000000;
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.DarkRed), IsHorizontal = false, IsVertical = true, Name = Math.Round(val, 6).ToString(), LabelLeft = 5, LabelTop = -25 });
                    }
                    if (emit.Spectrum.T2 != 0)
                    {
                        var val = (emit.Spectrum.SpectrumStartFreq_MHz * 1000000 + emit.Spectrum.SpectrumSteps_kHz * 1000 * emit.Spectrum.T2) / 1000000;
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.DarkRed), IsHorizontal = false, IsVertical = true, Name = Math.Round(val, 6).ToString(), LabelLeft = 5, LabelTop = -45 });
                    }
                    if (emit.Spectrum.MarkerIndex != 0)
                    {
                        var val = (emit.Spectrum.SpectrumStartFreq_MHz * 1000000 + emit.Spectrum.SpectrumSteps_kHz * 1000 * emit.Spectrum.MarkerIndex) / 1000000;
                        linesList.Add(new ChartLine() { Point = new PointF { X = (float)val, Y = 0 }, LineColor = new SolidBrush(Color.DarkRed), IsHorizontal = false, IsVertical = true, Name = Math.Round(val, 6).ToString(), LabelLeft = 5, LabelTop = -35 });
                    }
                }
            }

            if (pointsList.Count == 0)
                return option;

            var preparedDataY = Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 20;
            option.YMin = preparedDataY.MinValue;
            option.YMax = preparedDataY.MaxValue;

            var preparedDataX = Utitlity.CalcFrequencyRange(minX, maxX, 20);
            option.XTick = preparedDataX.Step;
            option.XMin = preparedDataX.MinValue;// + preparedDataX.Step;
            option.XMax = preparedDataX.MaxValue;// - preparedDataX.Step;


            option.PointsArray = pointsList.ToArray();
            option.LinesArray = linesList.ToArray();
            return option;
        }

        private PointF NormalizePoint(PointF pt, double Width, double Height)
        {
            PointF result = new PointF()
            {
                X = (float)((pt.X - this._option.XMin) * Width / (this._option.XMax - this._option.XMin)),
                Y = (float)(Height - (pt.Y - this._option.YMin) * Height / (this._option.YMax - this._option.YMin))
            };
            return result;
        }

        public System.Drawing.Image CreateBitmapSpectrogram(Emitting emit, ReferenceLevels referenceLevels, int ActualWidth, int ActualHeight)
        {
            this._option = GetChartOption(emit, referenceLevels);
            var Width = ActualWidth;
            var Height = ActualHeight;
            var cnt = 30;
            var pt = new PointF();
            float dx, dy;
            var bm = new Bitmap(Width, Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.Clear(Color.WhiteSmoke);
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var stringFont = new Font("Arial", 8);
                Height = Height - cnt;
                var chartRect = new Rectangle() { Size = new Size(Width - cnt - 1, Height), X = cnt, Y = 0, Location = new Point(cnt, 0) };
                gr.DrawRectangle(new Pen(Color.Black, 1), chartRect);
                // Create vertical gridlines:

                var stepX = (float)(this._option.XTick / this._option.XInnerTickCount);
                for (dx = this._option.XMin + stepX; dx < this._option.XMax; dx += stepX)
                {
                    var X1 = NormalizePoint(new PointF(dx, this._option.YMin), Width, Height).X;
                    var Y1 = NormalizePoint(new PointF(dx, this._option.YMin), Width, Height).Y;
                    var X2 = NormalizePoint(new PointF(dx, this._option.YMax), Width, Height).X;
                    var Y2 = NormalizePoint(new PointF(dx, this._option.YMax), Width, Height).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1, X2 + cnt, Y2);
                    }
                }
                for (dx = this._option.XMin + this._option.XTick; dx < this._option.XMax; dx += this._option.XTick)
                {
                    var X1 = NormalizePoint(new PointF(dx, this._option.YMin), Width, Height).X;
                    var Y1 = NormalizePoint(new PointF(dx, this._option.YMin), Width, Height).Y;
                    var X2 = NormalizePoint(new PointF(dx, this._option.YMax), Width, Height).X;
                    var Y2 = NormalizePoint(new PointF(dx, this._option.YMax), Width, Height).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1, X2 + cnt, Y2);
                    }
                }

                // Create horizontal gridlines:

                var stepY = this._option.YTick / this._option.YInnerTickCount;
                for (dy = this._option.YMin + stepY; dy < this._option.YMax; dy += stepY)
                {
                    var X1 = NormalizePoint(new PointF(this._option.XMin, dy), Width, Height).X;
                    var Y1 = NormalizePoint(new PointF(this._option.XMin, dy), Width, Height).Y;
                    var X2 = NormalizePoint(new PointF(this._option.XMax, dy), Width, Height).X;
                    var Y2 = NormalizePoint(new PointF(this._option.XMax, dy), Width, Height).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1, X2 + cnt, Y2);
                    }
                }

                for (dy = this._option.YMin + this._option.YTick; dy < this._option.YMax; dy += this._option.YTick)
                {
                    var X1 = NormalizePoint(new PointF(this._option.XMin, dy), Width, Height).X;
                    var Y1 = NormalizePoint(new PointF(this._option.XMin, dy), Width, Height).Y;
                    var X2 = NormalizePoint(new PointF(this._option.XMax, dy), Width, Height).X;
                    var Y2 = NormalizePoint(new PointF(this._option.XMax, dy), Width, Height).Y;
                    using (Pen thick_pen = new Pen(Color.Gray, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        gr.DrawLine(thick_pen, X1 + cnt, Y1, X2 + cnt, Y2);
                    }
                }



                // Create x-axis tick marks:
                for (dx = this._option.XMin; LessOrEqual(dx, this._option.XMax); dx += this._option.XTick)
                {
                    pt = NormalizePoint(new PointF(dx, this._option.YMin), Width, Height);
                    using (Pen thick_pen = new Pen(Color.Black, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        gr.DrawLine(thick_pen, pt.X + cnt, pt.Y + 5, pt.X + cnt, pt.Y - 5);
                    }


                    if (dx == this._option.XMin)
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt - 5, pt.Y));
                    }
                    else if (LessOrEqual((dx + this._option.XTick), this._option.XMax))
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt, pt.Y));
                    }
                    else
                    {
                        gr.DrawString(dx.ToString(), stringFont, Brushes.Black, new PointF(pt.X + cnt + 5, pt.Y));
                    }

                }



                // Create y-axis tick marks:
                for (dy = this._option.YMin; LessOrEqual(dy, this._option.YMax); dy += this._option.YTick)
                {
                    pt = NormalizePoint(new PointF(this._option.XMin, dy), Width, Height);
                    using (Pen thick_pen = new Pen(Color.Black, 0.5f))
                    {
                        thick_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        gr.DrawLine(thick_pen, pt.X + cnt - 5, pt.Y, pt.X + cnt + 5, pt.Y);
                    }

                    if (dy == this._option.YMin)
                    {
                        gr.DrawString(dy.ToString(), stringFont, Brushes.Black, new PointF(pt.X, pt.Y - 5));
                    }
                    else if (LessOrEqual((dy + this._option.YTick), this._option.YMax))
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
                        var point = NormalizePoint(points.Points[i], Width, Height);
                        point.X = point.X + cnt;
                        line.Add(point);
                    }
                    using (Pen thick_pen = new Pen(points.LineColor, 1))
                    {
                        gr.DrawLines(thick_pen, line.ToArray());
                    }
                }



                foreach (var lines in this._option.LinesArray)
                {
                    if (lines.IsHorizontal)
                    {
                        var line = new List<PointF>();

                        var point = this.NormalizePoint(lines.Point, Width, Height);
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

                        var point = this.NormalizePoint(lines.Point, Width, Height);
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
                gr.DrawString(this._option.XLabel, stringFont, Brushes.Black, new PointF((float)Width / 2, Height + 15));
                gr.DrawString(this._option.YLabel, stringFont, Brushes.Black, new PointF(2, (float)Height / 2), drawFormat);
            }
            var memoryStream = new System.IO.MemoryStream();
            bm.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return  System.Drawing.Image.FromStream(memoryStream);
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
