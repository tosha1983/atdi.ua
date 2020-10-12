using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace ControlU.Equipment
{
    public class PrintScreen : INotifyPropertyChanged
    {
        Settings.XMLSettings Sett = App.Sett;

        /// <summary>
        /// 1=SpectrumAnalyzer 2=RuSReceiver 3=RuSTSMx 5=SignalHound
        /// </summary>
        public int InstrType = 0;
        //public double[] inFreq;
        public Equipment.tracepoint[] Trace1;
        public Equipment.tracepoint[] Trace2;
        public Equipment.tracepoint[] Trace3;
        public bool Trace1State = false;
        public bool Trace2State = false;
        public bool Trace3State = false;
        public string Trace1Legend = "";
        public string Trace2Legend = "";
        public string Trace3Legend = "";
        public int ActualPoints = 0;

        public decimal RefLevel = 0;
        public decimal Range = 0;
        public string LevelUnit = "";
        public string Att = "";
        public decimal RBW = 0;
        public decimal VBW = 0;
        public decimal SWT = 0;
        public string Mode = "";
        public string PreAmp = "";
        public string InstrManufacrure = "";
        public string InstrModel = "";
        public System.Windows.Point Location;
        public decimal FreqCentr = 0;
        public decimal FreqSpan = 0;
        public decimal FreqStart = 0;
        public decimal FreqStop = 0;
        /// <summary>
        /// true = CentrSpan
        /// false = StartStop
        /// </summary>
        public bool Freq_CentrSpan_StartStop = false;
        public string OverLoad = "";
        public DateTime DateTime;
        public string FilePath = "";
        public ObservableCollection<Equipment.Marker> Markers = new ObservableCollection<Equipment.Marker>();
        //public ObservableCollection<Equipment.TMarker> inTMarkers = new ObservableCollection<Equipment.TMarker>();
        //public List<Equipment.TracePoint> inTraceSet = new List<Equipment.TracePoint>();
        public bool ChannelPower = false;
        public decimal ChannelPowerBW = 100000;
        public double ChannelPowerResult = 0;
        public decimal ChannelPowerTxTotal = 0;

        Helpers.Helper help = new Helpers.Helper();

        public void drawImageToPath()
        {
            string[] temp = Sett.Screen_Settings.ScreenResolution.Trim(' ').Split('x');
            int width = Int32.Parse(temp[0]);
            int height = Int32.Parse(temp[1]);
            //try
            //{
            Bitmap image = new Bitmap(width, height);
            image = drawImage(width, height);
            ImageFormat imfo = ImageFormat.Png;

            //System.Drawing.Imaging.ImageFormat imfo = GetImageFormat(App.Sett.Screen_Settings.SaveScreenImageFormat);
            //image.Save(@"d:\1123456.png", imfo);
            string str = FilePath + "." + @App.Sett.Screen_Settings.SaveScreenImageFormat.ToString();
            image.Save(str, imfo);
            image.Dispose();
            //}
            //catch (Exception exp)
            //{
            //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "PrintScreen", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            //}
        }
        private static ImageFormat GetImageFormat(string str)
        {
            switch (str.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;
                case @".gif":
                    return ImageFormat.Gif;
                case @".ico":
                    return ImageFormat.Icon;
                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;
                case @".png":
                    return ImageFormat.Png;
                case @".tiff":
                    return ImageFormat.Tiff;
                case @".wmf":
                    return ImageFormat.Wmf;
                default:
                    return ImageFormat.Png;
            }
        }
        public Bitmap drawImageToDB()
        {
            string[] temp = Sett.Screen_Settings.ScreenResolution.Trim(' ').Split('x');
            int width = Int32.Parse(temp[0]);
            int height = Int32.Parse(temp[1]);

            Bitmap image = new Bitmap(width, height);
            image = drawImage(width, height);
            return image;
        }
        public Bitmap drawImage(int width, int height)
        {
            Bitmap image = new Bitmap(width, height);
            int markersCount = 0;
            for (int i = 0; i < Markers.Count; i++)
            {
                if (Markers[i].State == true)
                {
                    markersCount++;
                    if (Markers[i].TMarkers.Count > 0) markersCount += 2;
                }
            }
            //markersCount += inTMarkers.Count / 2; //хз почему их создается 4 а не 2 (искать кто открывает экспандер 2 раза)
            string[] temp = Sett.Screen_Settings.ScreenResolution.Trim(' ').Split('x');
            int fontSize = width / 80;
            decimal margin = (decimal)(width / 100);
            decimal UPPanelY = 0; //размер верхней панели до панели Trace
            decimal TrPanelY = 0; //размер верхней панели до панели Trace
            decimal DnPowerPanelY = 0;//размер нижней панели от панели Power
            decimal DnMarkerPanelY = 0;//размер нижней панели от панели Marker
            if (markersCount > 0)
            {
                DnMarkerPanelY = 2 * margin + (margin + fontSize) * markersCount;
            }
            if (InstrType == 1 || InstrType == 5)
            {
                //размер верхней панели до панели Trace
                UPPanelY = margin * 7 + fontSize * 4;
                //размер нижней панели от панели Trace
                //DnMarkerPanelY = margin * (1 + markersCount) + fontSize * (1 + markersCount);
                if (ChannelPower)
                { DnPowerPanelY = margin * (3) + fontSize * (1); }

                TrPanelY = height - UPPanelY - DnPowerPanelY - DnMarkerPanelY;
            }
            else if (InstrType == 2)
            {
                //размер верхней панели до панели Trace
                UPPanelY = margin * 7 + fontSize * 4;
                //размер нижней панели от панели Trace
                if (ChannelPower)
                { DnPowerPanelY = margin * (3) + fontSize * (1); }
                //DnMarkerPanelY = margin * (1 + markersCount) + fontSize * (1 + markersCount);
                TrPanelY = height - UPPanelY - DnPowerPanelY - DnMarkerPanelY;
            }
            System.Drawing.Pen penLine = new System.Drawing.Pen(System.Drawing.Brushes.Gray);
            penLine.Width = 1F;

            System.Drawing.Pen penLineTrace1 = new System.Drawing.Pen(System.Drawing.Brushes.SteelBlue);
            penLineTrace1.Width = 1F;
            System.Drawing.Pen penLineTrace2 = new System.Drawing.Pen(System.Drawing.Brushes.LightGreen);
            penLineTrace2.Width = 1F;
            System.Drawing.Pen penLineTrace3 = new System.Drawing.Pen(System.Drawing.Brushes.Red);
            penLineTrace3.Width = 1F;

            System.Drawing.Pen penLineMarker = new System.Drawing.Pen(System.Drawing.Brushes.Red);
            penLineMarker.Width = 2F;

            Font fontText = new Font("Segoe UI Mono", fontSize);
            Font fontLineText = new Font("Segoe UI Mono", fontSize - 2);

            //using (image = new Bitmap(width, height))
            //{
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);

            Bitmap imageUP = new Bitmap(width, (int)UPPanelY);
            Graphics gU = Graphics.FromImage(imageUP);
            gU.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            gU = DrawUpPanel(gU, width, (int)UPPanelY, (float)margin, penLine, fontText, fontSize);
            g.DrawImage(imageUP, new Point(0, 0));

            Bitmap imageTrace = new Bitmap((int)(width - margin * 4), (int)(TrPanelY - margin * 2));
            Graphics gT = Graphics.FromImage(imageTrace);
            gT.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            gT = DrawTracePanel(gT, (int)(width - margin * 4), (int)(TrPanelY - margin * 4), margin, penLine, penLineTrace1, penLineTrace2, penLineTrace3, fontLineText, fontSize);
            g.DrawImage(imageTrace, new Point((int)margin * 2, (int)(UPPanelY + margin)));

            if (ChannelPower)
            {
                Bitmap imagePowerDN = new Bitmap(width, (int)DnPowerPanelY);
                Graphics gPD = Graphics.FromImage(imagePowerDN);
                gPD.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                gPD = DrawPowerPanel(gPD, width, (int)DnPowerPanelY, markersCount, (float)margin, penLine, fontText, fontSize);
                g.DrawImage(imagePowerDN, new Point(0, (int)(height - DnMarkerPanelY - DnPowerPanelY)));
            }
            if (DnMarkerPanelY > 0)
            {
                Bitmap imageMarkerDN = new Bitmap(width, (int)DnMarkerPanelY);
                Graphics gMD = Graphics.FromImage(imageMarkerDN);
                gMD.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                gMD = DrawMarkerPanel(gMD, width, (int)DnMarkerPanelY, markersCount, (float)margin, penLine, fontText, fontSize);
                g.DrawImage(imageMarkerDN, new Point(0, (int)(height - DnMarkerPanelY)));
            }

            Controls.FreqConverter fcdp = new Controls.FreqConverter();
            if (Freq_CentrSpan_StartStop)
            {
                string freqCenter = "Center " + (string)fcdp.Convert(FreqCentr, null, null, null);
                string freqSpan = "Span " + (string)fcdp.Convert(FreqSpan, null, null, null);
                string pts = ActualPoints.ToString() + " pts";
                float freqSpanLength = ((float)(freqSpan.Length * fontSize)) / 20f * 16.5f;
                float ptsLength = ((float)(pts.Length * fontSize)) / 20f * 16.5f;
                g.DrawString(freqCenter, fontText, System.Drawing.Brushes.Black, (float)(margin * 2.3m), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 2 - fontSize));
                g.DrawString(freqSpan, fontText, System.Drawing.Brushes.Black, (float)(width - (float)(margin * 2.5m) - freqSpanLength), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 2 - fontSize));
                g.DrawString(pts, fontText, System.Drawing.Brushes.Black, (float)(width / 2 - ptsLength / 2), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 2 - fontSize));
            }
            else
            {
                string FreqStart = "Start " + (string)fcdp.Convert(this.FreqStart, null, null, null);
                string freqStop = "Stop " + (string)fcdp.Convert(FreqStop, null, null, null);
                string pts = ActualPoints.ToString() + " pts";
                float freqStopLength = ((float)(freqStop.Length * fontSize)) / 20f * 16.5f;
                float ptsLength = ((float)(pts.Length * fontSize)) / 20f * 16.5f;
                g.DrawString(FreqStart, fontText, System.Drawing.Brushes.Black, (float)(margin * 2.3m), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 2 - fontSize));
                g.DrawString(freqStop, fontText, System.Drawing.Brushes.Black, (float)(width - (float)(margin * 2.5m) - freqStopLength), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 2 - fontSize));
                g.DrawString(pts, fontText, System.Drawing.Brushes.Black, (float)(width / 2 - ptsLength / 2), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 2 - fontSize));
            }

            g.DrawLine(penLine, (float)margin * 2, (float)UPPanelY, (float)(width - margin * 2), (float)UPPanelY);//up
            g.DrawLine(penLine, (float)margin * 2, (float)(height - DnMarkerPanelY - DnPowerPanelY - margin), (float)(width - margin * 2), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin));//down
            g.DrawLine(penLine, (float)margin, (float)(UPPanelY + margin), (float)margin, (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 2));//left
            g.DrawLine(penLine, (float)(width - margin), (float)(UPPanelY + margin), (float)(width - margin), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 2));//right

            g.DrawArc(penLine, (float)(width - margin * 3), (float)UPPanelY, (float)margin * 2, (float)margin * 2, 270, 90);
            g.DrawArc(penLine, (float)margin, (float)UPPanelY, (float)margin * 2, (float)margin * 2, 180, 90);
            g.DrawArc(penLine, (float)(width - margin * 3), (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 3), (float)margin * 2, (float)margin * 2, 0, 90);
            g.DrawArc(penLine, (float)margin, (float)(height - DnMarkerPanelY - DnPowerPanelY - margin * 3), (float)margin * 2, (float)margin * 2, 90, 90);

            //Дата и время
            string dt = DateTime.ToString("yyyy-MM-dd H.mm.ss.fff");
            decimal dtLength = ((decimal)(dt.Length * fontSize)) / 20m * 16.5m;
            g.DrawString(dt, fontText, System.Drawing.Brushes.Black, (float)(width - margin * 3 - dtLength), (float)(UPPanelY + TrPanelY - margin * 4 - fontSize));

            g.Dispose();

            //}
            return image;
        }
        public Bitmap drawImageSpectr(int width, int height)
        {
            decimal margin = (decimal)(width / 100);
            int fontSize = 12;//height / 60;
            System.Drawing.Pen penLine = new System.Drawing.Pen(System.Drawing.Brushes.Gray);
            penLine.Width = 1F;

            System.Drawing.Pen penLineTrace1 = new System.Drawing.Pen(System.Drawing.Brushes.SteelBlue);
            penLineTrace1.Width = 1F;
            System.Drawing.Pen penLineTrace2 = new System.Drawing.Pen(System.Drawing.Brushes.LightGreen);
            penLineTrace2.Width = 1F;
            System.Drawing.Pen penLineTrace3 = new System.Drawing.Pen(System.Drawing.Brushes.Red);
            penLineTrace3.Width = 1F;

            System.Drawing.Pen penLineMarker = new System.Drawing.Pen(System.Drawing.Brushes.Red);
            penLineMarker.Width = 2F;

            Font fontText = new Font("Segoe UI Mono", fontSize);
            Font fontLineText = new Font("Segoe UI Mono", fontSize - 2);
            Bitmap image = new Bitmap(width, height);


            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g = DrawTracePanel(g, width, height, margin, penLine, penLineTrace1, penLineTrace2, penLineTrace3, fontLineText, fontSize);
            g.DrawImage(image, new Point(width, height));
            g.Dispose();



            //}
            return image;
        }
        private Graphics DrawUpPanel(Graphics graphics, int width, int height, float margin, System.Drawing.Pen penLine, Font fontText, int fontSize)
        {
            Graphics g = graphics;
            //зарисовываем фон
            g.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);
            //верхняя панель 
            g.DrawLine(penLine, margin * 2, margin, width - margin * 2, margin);
            g.DrawLine(penLine, margin * 2, margin * 6 + fontSize * 4, width - margin * 2, margin * 6 + fontSize * 4);
            g.DrawLine(penLine, margin, margin * 2, margin, margin * 5 + fontSize * 4);
            g.DrawLine(penLine, width - margin, margin * 2, width - margin, margin * 5 + fontSize * 4);

            g.DrawArc(penLine, width - margin * 3, margin, margin * 2, margin * 2, 270, 90);
            g.DrawArc(penLine, margin, margin, margin * 2, margin * 2, 180, 90);
            g.DrawArc(penLine, width - margin * 3, margin * 4 + fontSize * 4, margin * 2, margin * 2, 0, 90);
            g.DrawArc(penLine, margin, margin * 4 + fontSize * 4, margin * 2, margin * 2, 90, 90);
            Controls.TimeConverter tc = new Controls.TimeConverter();
            Controls.FreqConverter fcd = new Controls.FreqConverter();
            if (InstrType == 1 || InstrType == 5)
            {
                #region
                g.DrawString("Ref Level " + help.helpLevel((float)RefLevel, LevelUnit), fontText, System.Drawing.Brushes.Black, width / 40, margin * 2 - 5);
                g.DrawString("Att " + Att, fontText, System.Drawing.Brushes.Black, width / 40, margin * 3 + fontSize - 5);

                g.DrawString("SWT " + (string)tc.Convert(SWT, null, null, null), fontText, System.Drawing.Brushes.Black, width / 4 + width / 40, margin * 2 - 5);
                g.DrawString("Preamp " + PreAmp, fontText, System.Drawing.Brushes.Black, width / 4 + width / 40, margin * 3 + fontSize - 5);

                string RBW = "RBW " + (string)fcd.Convert(this.RBW, null, null, null);
                string VBW = "VBW " + (string)fcd.Convert(this.VBW, null, null, null);
                g.DrawString(RBW, fontText, System.Drawing.Brushes.Black, width / 2 + width / 40, margin * 2 - 5);
                g.DrawString(VBW, fontText, System.Drawing.Brushes.Black, width / 2 + width / 40, margin * 3 + fontSize - 5);

                if (OverLoad != "")
                    g.DrawString(OverLoad, fontText, System.Drawing.Brushes.Red, width / 4 * 3 + width / 40, margin * 2 - 5);
                g.DrawString("Mode " + Mode, fontText, System.Drawing.Brushes.Black, width / 4 * 3 + width / 40, margin * 3 + fontSize - 5);

                if (Location.X != 0 && Location.Y != 0)
                {
                    Controls.CoorConverter cc = new Controls.CoorConverter();
                    string loc = "Lon " + (string)cc.Convert(Location.X, null, null, null) + "Lat " + (string)cc.Convert(Location.Y, null, null, null);
                    g.DrawString("Location " + loc, fontText, System.Drawing.Brushes.Black, width / 2 + width / 40, margin * 4 + fontSize * 2 - 5);
                }
                g.DrawString("Instr " + InstrManufacrure + " " + InstrModel, fontText, System.Drawing.Brushes.Black, width / 40, margin * 4 + fontSize * 2 - 5);
                #endregion
            }
            else if (InstrType == 2)
            {
                #region
                g.DrawString("Ref Level " + help.helpLevel((float)RefLevel, LevelUnit), fontText, System.Drawing.Brushes.Black, width / 40, margin * 2 - 5);
                g.DrawString("Att " + Att, fontText, System.Drawing.Brushes.Black, width / 40, margin * 3 + fontSize - 5);

                g.DrawString("SWT " + (string)tc.Convert(SWT, null, null, null), fontText, System.Drawing.Brushes.Black, width / 4 + width / 40, margin * 2 - 5);
                g.DrawString("Preamp " + PreAmp, fontText, System.Drawing.Brushes.Black, width / 4 + width / 40, margin * 3 + fontSize - 5);

                g.DrawString("RBW " + (string)fcd.Convert(this.RBW, null, null, null), fontText, System.Drawing.Brushes.Black, width / 2 + width / 40, margin * 2 - 5);


                if (OverLoad != "")
                    g.DrawString(OverLoad, fontText, System.Drawing.Brushes.Red, width / 4 * 3 + width / 40, margin * 2 - 5);
                g.DrawString("Mode " + Mode, fontText, System.Drawing.Brushes.Black, width / 4 * 3 + width / 40, margin * 3 + fontSize - 5);

                if (Location.X != 0 && Location.Y != 0)
                {
                    Controls.CoorConverter cc = new Controls.CoorConverter();
                    string loc = "Lon " + (string)cc.Convert(Location.X, null, null, null) + "Lat " + (string)cc.Convert(Location.Y, null, null, null);
                    g.DrawString("Location " + loc, fontText, System.Drawing.Brushes.Black, width / 2 + width / 40, margin * 4 + fontSize * 2 - 5);
                }
                g.DrawString("Instr " + InstrManufacrure + " " + InstrModel, fontText, System.Drawing.Brushes.Black, width / 40, margin * 4 + fontSize * 2 - 5);
                #endregion
            }
            float x = 0;
            int trcount = 0;
            if (Trace1State) trcount++;
            if (Trace2State) trcount++;
            if (Trace2State) trcount++;
            for (int i = 0; i < trcount; i++)
            {
                x = width / 3 * i + width / 40;
                if (i == 0)
                {
                    g.FillRectangle(System.Drawing.Brushes.SteelBlue, (float)x, (float)(margin * 5 + fontSize * 3.2), margin, margin);
                    g.DrawString(Trace1Legend, fontText, System.Drawing.Brushes.Black, x + 25, margin * 5 + fontSize * 3 - 5);
                }
                if (i == 1)
                {
                    g.FillRectangle(System.Drawing.Brushes.LightGreen, (float)x, (float)(margin * 5 + fontSize * 3.2), margin, margin);
                    g.DrawString(Trace2Legend, fontText, System.Drawing.Brushes.Black, x + 25, margin * 5 + fontSize * 3 - 5);
                }
                if (i == 2)
                {
                    g.FillRectangle(System.Drawing.Brushes.Red, (float)x, (float)(margin * 5 + fontSize * 3.2), margin, margin);
                    g.DrawString(Trace3Legend, fontText, System.Drawing.Brushes.Black, x + 25, margin * 5 + fontSize * 3 - 5);
                }

            }
            return g;
        }
        private Graphics DrawTracePanel(Graphics graphics, int width, int height, decimal margin, System.Drawing.Pen penLine, System.Drawing.Pen penLineTrace1, System.Drawing.Pen penLineTrace2, System.Drawing.Pen penLineTrace3, Font fontLineText, int fontSize)
        {
            Graphics g = graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            //зарисовываем фон
            g.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);


            double specTemp = (double)height / (double)Range;
            //рисуем 
            if (ChannelPower)
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(100, 179, 179, 179));
                decimal sp = FreqStop - FreqStart;
                decimal cf = (FreqStop + FreqStart) / 2;
                float xLo = (float)((width / sp) * ((cf - (ChannelPowerBW / 2) - FreqStart)));
                float xHi = (float)((width / sp) * ((ChannelPowerBW)));

                //float xMarker = (float)((width / cf) * (cf + (inChannelPowerBW / 2) - inFreqStart));
                float yLo = (float)((double)RefLevel * specTemp - (double)ChannelPowerResult * specTemp);
                float yHi = height - yLo;

                g.FillRectangle(brush, xLo, yLo, xHi, yHi);// height);
            }
            //горизонтальные линии панели Trace
            //расчет шага по вертикали

            for (int i = (int)((RefLevel) / 10) * 10; i >= (int)((RefLevel / 10) * 10 - Range); i -= (int)Range / 10)
            {
                string level = String.Concat(i, " ", LevelUnit);

                decimal shift = ((decimal)(level.Length * fontSize)) / 20m * 16.5m; // (decimal)(String.Concat(i, inRefLevel).Length * fontSize * 0.5);
                decimal y = (decimal)((double)RefLevel * specTemp - i * specTemp);
                if (y > (decimal)(fontSize * 0.8) && y < (decimal)(height - fontSize * 0.8))
                {
                    g.DrawLine(penLine, (float)shift, (float)y, width, (float)y);
                    g.DrawString(level, fontLineText, System.Drawing.Brushes.Gray, (float)(margin * 0.2m), (float)(y - ((decimal)fontSize) / 5m * 4m));
                }
                else { g.DrawLine(penLine, 0, (float)y, width, (float)y); }
            }
            g.DrawLine(penLine, 0, 0, width, 0);
            g.DrawLine(penLine, 0, height - 1, width, height - 1);

            //вертикальные линии
            for (int i = 0; i < 12; i++)
            {
                g.DrawLine(penLine, i * width / 10, 0, i * width / 10, height);
            }
            g.DrawLine(penLine, width - 1, 0, width - 1, height);
            //рисуем Trace
            double yStartTrace1Line = 0;
            double yStopTrace1Line = 0;
            double yStartTrace2Line = 0;
            double yStopTrace2Line = 0;
            double yStartTrace3Line = 0;
            double yStopTrace3Line = 0;

            if (Trace1State == true && Trace1 != null)
            {
                double xStepT1 = (double)width / (double)Trace1.Length;
                for (int i = 0; i < Trace1.Length - 1; i++)
                {
                    yStartTrace1Line = (double)RefLevel * specTemp - Trace1[i].level * specTemp;
                    yStopTrace1Line = (double)RefLevel * specTemp - Trace1[i + 1].level * specTemp;
                    g.DrawLine(penLineTrace1, (float)(xStepT1 * i), (float)yStartTrace1Line, (float)(xStepT1 * (i + 1)), (float)yStopTrace1Line);
                }
            }
            if (Trace2State == true && Trace2 != null)
            {
                double xStepT2 = (double)width / (double)Trace2.Length;

                for (int i = 0; i < Trace2.Length - 1; i++)
                {
                    yStartTrace2Line = (double)RefLevel * specTemp - Trace2[i].level * specTemp;
                    yStopTrace2Line = (double)RefLevel * specTemp - Trace2[i + 1].level * specTemp;
                    g.DrawLine(penLineTrace2, (float)xStepT2 * i, (float)yStartTrace2Line, (float)xStepT2 * (i + 1), (float)yStopTrace2Line);
                }
            }
            if (Trace3State == true && Trace3 != null)
            {
                double xStepT3 = (double)width / (double)Trace3.Length;
                for (int i = 0; i < Trace3.Length - 1; i++)
                {
                    yStartTrace3Line = (double)RefLevel * specTemp - Trace3[i].level * specTemp;
                    yStopTrace3Line = (double)RefLevel * specTemp - Trace3[i + 1].level * specTemp;
                    g.DrawLine(penLineTrace3, (float)xStepT3 * i, (float)yStartTrace3Line, (float)xStepT3 * (i + 1), (float)yStopTrace3Line);
                }
            }

            System.Drawing.Pen penLineMarker = new System.Drawing.Pen(System.Drawing.Brushes.Black);
            penLineMarker.Width = 1F;
            //маркеры 
            for (int i = 0; i < Markers.Count; i++)
            {//маркеры на трейсе
                if (Markers[i].State == true)
                {
                    float yMarker = (float)((double)RefLevel * specTemp - Markers[i].Level * specTemp);
                    float xMarker = (float)((width / (FreqStop - FreqStart)) * (Markers[i].Freq - FreqStart)); // (float)(margin * 2 + ((freqStart + inMarkers[i].Freq) * width) / span);
                    g.DrawLine(penLineMarker, xMarker, yMarker, xMarker - (float)margin / 3, yMarker - (float)margin / 2);
                    g.DrawLine(penLineMarker, xMarker, yMarker, xMarker + (float)margin / 3, yMarker - (float)margin / 2);
                    g.DrawLine(penLineMarker, xMarker - (float)margin / 3, yMarker - (float)margin / 2, xMarker + (float)margin / 3, yMarker - (float)margin / 2);

                    float shiftName = ((float)(Markers[i].Name.Length * fontSize)) / 40f * 16.5f;
                    g.DrawString(Markers[i].Name, fontLineText, System.Drawing.Brushes.Black, xMarker - shiftName, yMarker - fontSize * 2);

                    if (i == 0 && (Markers[0].MarkerType == 3 || Markers[0].MarkerType == 4))
                    {
                        for (int j = 0; j < Markers[0].TMarkers.Count; j++)
                        {
                            float yMarkert = (float)((double)RefLevel * specTemp - Markers[0].TMarkers[j].Level * specTemp);
                            float xMarkert = (float)((width / (FreqStop - FreqStart)) * (Markers[0].TMarkers[j].Freq - FreqStart)); // (float)(margin * 2 + ((freqStart + inMarkers[i].Freq) * width) / span);
                            g.DrawLine(penLineMarker, xMarkert, yMarkert, xMarkert - (float)margin / 3, yMarkert - (float)margin / 2);
                            g.DrawLine(penLineMarker, xMarkert, yMarkert, xMarkert + (float)margin / 3, yMarkert - (float)margin / 2);
                            g.DrawLine(penLineMarker, xMarkert - (float)margin / 3, yMarkert - (float)margin / 2, xMarkert + (float)margin / 3, yMarkert - (float)margin / 2);

                            float shiftNameT = ((float)(Markers[0].TMarkers[j].Name.Length * fontSize)) / 40f * 16.5f;
                            g.DrawString(Markers[0].TMarkers[j].Name, fontLineText, System.Drawing.Brushes.Black,
                               xMarkert - shiftNameT, yMarkert - fontSize * 2);
                        }
                    }
                }
            }

            return g;
        }
        private Graphics DrawPowerPanel(Graphics graphics, int width, int height, int markersCount, float margin, System.Drawing.Pen penLine, Font fontText, int fontSize)
        {
            Graphics g = graphics;
            //зарисовываем фон
            g.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);
            //верхняя панель 
            g.DrawLine(penLine, margin * 2, 0, width - margin * 2, 0);//up
            g.DrawLine(penLine, margin * 2, height - margin, width - margin * 2, height - margin);//down
            g.DrawLine(penLine, margin, margin, margin, height - margin * 2);//left
            g.DrawLine(penLine, width - margin, margin, width - margin, height - margin * 2);//right

            g.DrawArc(penLine, width - margin * 3, 0, margin * 2, margin * 2, 270, 90);

            g.DrawArc(penLine, margin, 0, margin * 2, margin * 2, 180, 90);

            g.DrawArc(penLine, width - margin * 3, height - margin * 3, margin * 2, margin * 2, 0, 90);

            g.DrawArc(penLine, margin, height - margin * 3, margin * 2, margin * 2, 90, 90);

            Controls.FreqConverter fcdp = new Controls.FreqConverter();
            string bw = "Channel Power Bandwidth " + (string)fcdp.Convert(ChannelPowerBW, null, null, null);
            g.DrawString(bw, fontText, System.Drawing.Brushes.Black, width / 20, margin / 2);// height - margin - fontSize);
            g.DrawString("Power " + ChannelPowerResult.ToString() + " " + LevelUnit, fontText, System.Drawing.Brushes.Black, width / 2 + width / 20, margin / 2);

            return g;
        }
        //запилено кроме даты и времени
        private Graphics DrawMarkerPanel(Graphics graphics, int width, int height, int markersCount, float margin, System.Drawing.Pen penLine, Font fontText, int fontSize)
        {
            Graphics g = graphics;
            //зарисовываем фон
            g.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height);

            g.DrawLine(penLine, margin * 2, 0, width - margin * 2, 0);//up
            g.DrawLine(penLine, margin * 2, height - margin, width - margin * 2, height - margin);//down
            g.DrawLine(penLine, margin, margin, margin, height - margin * 2);//left
            g.DrawLine(penLine, width - margin, margin, width - margin, height - margin * 2);//right

            g.DrawArc(penLine, width - margin * 3, 0, margin * 2, margin * 2, 270, 90);
            g.DrawArc(penLine, margin, 0, margin * 2, margin * 2, 180, 90);
            g.DrawArc(penLine, width - margin * 3, height - margin * 3, margin * 2, margin * 2, 0, 90);
            g.DrawArc(penLine, margin, height - margin * 3, margin * 2, margin * 2, 90, 90);

            float markW = (width - margin * 2) / 50;
            float markHStep = margin + fontSize;
            int pos = 0;
            for (int i = 0; i < Markers.Count; i++)
            {
                if (Markers[i].State == true)
                {
                    //name
                    g.DrawString(Markers[i].Name, fontText, System.Drawing.Brushes.Black, margin + markW, margin / 2 + markHStep * pos);

                    //ref
                    if (Markers[i].MarkerParent != null)
                        g.DrawString(Markers[i].MarkerParent.Name, fontText, System.Drawing.Brushes.Black, margin + markW * 5, margin / 2 + markHStep * pos);

                    //on trace
                    g.DrawString(Markers[i].TraceNumber.UI, fontText, System.Drawing.Brushes.Black, margin + markW * 9, margin / 2 + markHStep * pos);

                    //freq
                    Controls.FreqConverter fcdp = new Controls.FreqConverter();
                    string freq = (string)fcdp.Convert(Markers[i].Freq, null, null, null);
                    g.DrawString(freq, fontText, System.Drawing.Brushes.Black, margin + markW * 15, margin / 2 + markHStep * pos);

                    //level
                    g.DrawString(Math.Round(Markers[i].Level, 2).ToString() + " " + Markers[i].LevelUnit, fontText, System.Drawing.Brushes.Black, margin + markW * 24, margin / 2 + markHStep * pos);

                    if (Markers[i].FunctionDataType == 1)
                    {
                        g.DrawString((string)fcdp.Convert(Markers[i].Funk1, null, null, null), fontText, System.Drawing.Brushes.Black, margin + markW * 33, margin / 2 + markHStep * pos);
                        g.DrawString(Markers[i].Funk2.ToString() + " " + Markers[i].LevelUnit, fontText, System.Drawing.Brushes.Black, margin + markW * 40, margin / 2 + markHStep * pos);
                    }
                    else if (Markers[i].FunctionDataType == 2)
                    {
                        g.DrawString("N dB", fontText, System.Drawing.Brushes.Black, margin + markW * 33, margin / 2 + markHStep * pos);
                        g.DrawString(Markers[i].Funk2.ToString() + " dB", fontText, System.Drawing.Brushes.Black, margin + markW * 40, margin / 2 + markHStep * pos);
                    }
                    else if (Markers[i].FunctionDataType == 5)
                    {
                        g.DrawString("Power BW", fontText, System.Drawing.Brushes.Black, margin + markW * 33, margin / 2 + markHStep * pos);
                        g.DrawString(Markers[i].Funk2.ToString() + " %", fontText, System.Drawing.Brushes.Black, margin + markW * 40, margin / 2 + markHStep * pos);
                    }
                    pos++;
                    if (i == 0 && (Markers[0].MarkerType == 3 || Markers[0].MarkerType == 4))
                    {
                        for (int j = 0; j < Markers[0].TMarkers.Count; j++)
                        {
                            g.DrawString(Markers[0].TMarkers[j].Name, fontText, System.Drawing.Brushes.Black, margin + markW, margin / 2 + markHStep * pos);

                            g.DrawString(Markers[0].TMarkers[j].MarkerParent.Name, fontText, System.Drawing.Brushes.Black, margin + markW * 5, margin / 2 + markHStep * pos);

                            g.DrawString(Markers[0].TMarkers[j].TraceNumber.UI, fontText, System.Drawing.Brushes.Black, margin + markW * 9, margin / 2 + markHStep * pos);

                            Controls.FreqConverter fcdpt = new Controls.FreqConverter();
                            string freqt = (string)fcdpt.Convert(Markers[0].TMarkers[j].Freq, null, null, null);
                            g.DrawString(freqt, fontText, System.Drawing.Brushes.Black, margin + markW * 15, margin / 2 + markHStep * pos);

                            g.DrawString(Math.Round(Markers[0].TMarkers[j].Level, 2).ToString() + " " + Markers[0].TMarkers[j].LevelUnit, fontText, System.Drawing.Brushes.Black, margin + markW * 24, margin / 2 + markHStep * pos);

                            if (Markers[0].TMarkers[j].FunctionDataType == 3)
                            {
                                g.DrawString("ndB down", fontText, System.Drawing.Brushes.Black, margin + markW * 33, margin / 2 + markHStep * pos);
                                g.DrawString((string)fcdp.Convert(Markers[0].TMarkers[j].Funk2, null, null, null), fontText, System.Drawing.Brushes.Black, margin + markW * 40, margin / 2 + markHStep * pos);
                            }
                            else if (Markers[0].TMarkers[j].FunctionDataType == 4)
                            {
                                g.DrawString("Q factor", fontText, System.Drawing.Brushes.Black, margin + markW * 33, margin / 2 + markHStep * pos);
                                g.DrawString(Markers[0].TMarkers[j].Funk2.ToString(), fontText, System.Drawing.Brushes.Black, margin + markW * 40, margin / 2 + markHStep * pos);
                            }
                            else if (Markers[0].TMarkers[j].FunctionDataType == 6)
                            {
                                g.DrawString("OBW Result", fontText, System.Drawing.Brushes.Black, margin + markW * 33, margin / 2 + markHStep * pos);
                                g.DrawString((string)fcdp.Convert(Markers[0].TMarkers[j].Funk2, null, null, null), fontText, System.Drawing.Brushes.Black, margin + markW * 40, margin / 2 + markHStep * pos);
                            }
                            pos++;
                        }
                    }
                }
            }



            return g;
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
