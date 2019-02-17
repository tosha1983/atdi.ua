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
using System.Windows.Shapes;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Shaders;
using SharpGL.SceneGraph;
using ADP = Atdi.AppUnits.Sdrn.DeviceServer.Adapters;

namespace Atdi.Test.Sdrn.DeviceServer.Adapters.WPF
{
    /// <summary>
    /// Логика взаимодействия для DrawSpectrum.xaml
    /// </summary>
    public partial class DrawSpectrum : UserControl
    {
        #region data
        float[] BackgroundRGB = new float[3] { 0.9372f, 0.9372f, 0.949f };
        float[] ForegroundRGB = new float[3] { 0.1f, 0.1f, 0.1f };
        float[] Trace1RGB = new float[3] { 0.3529f, 0.6274f, 0.7843f };
        float[] Trace2RGB = new float[3] { 0.1375f, 0.6274f, 0.1375f };
        float[] Trace3RGB = new float[3] { 0.7843f, 0.2352f, 0.2352f };
        
        float[] Level = new float[] { };
        double[] Freq = new double[] { };
        #endregion
        #region Property
        public double RefLevel
        {
            get { return (double)GetValue(RefLevelProperty); }
            set { SetValue(RefLevelProperty, value); }
        }
        public double LowestLevel
        {
            get { return (double)GetValue(LowestLevelProperty); }
            set { SetValue(LowestLevelProperty, value); }
        }
        public double Range
        {
            get { return (double)GetValue(RangeProperty); }
            set { SetValue(RangeProperty, value); }
        }
        public double FreqCentr
        {
            get { return (double)GetValue(FreqCentrProperty); }
            set { SetValue(FreqCentrProperty, value); }
        }
        public double FreqSpan
        {
            get { return (double)GetValue(FreqSpanProperty); }
            set { SetValue(FreqSpanProperty, value); }
        }
        public double FreqStart
        {
            get { return (double)GetValue(FreqStartProperty); }
            set { SetValue(FreqStartProperty, value); }
        }
        public double FreqStop
        {
            get { return (double)GetValue(FreqStopProperty); }
            set { SetValue(FreqStopProperty, value); }
        }
        public int TracePoints
        {
            get { return (int)GetValue(TracePointsProperty); }
            set { SetValue(TracePointsProperty, value); }
        }
        public string LevelUnit
        {
            get { return (string)GetValue(LevelUnitProperty); }
            set { SetValue(LevelUnitProperty, value); }
        }
        public ADP.SignalHound.Adapter Adapter
        {
            get { return (ADP.SignalHound.Adapter)GetValue(AdapterProperty); }
            set { SetValue(AdapterProperty, value); }
        }
        public static readonly DependencyProperty RefLevelProperty = DependencyProperty.Register("RefLevel", typeof(double), typeof(DrawSpectrum), new PropertyMetadata(0d, null));
        public static readonly DependencyProperty LowestLevelProperty = DependencyProperty.Register("LowestLevel", typeof(double), typeof(DrawSpectrum), new PropertyMetadata(-100d, null));
        public static readonly DependencyProperty RangeProperty = DependencyProperty.Register("Range", typeof(double), typeof(DrawSpectrum), new PropertyMetadata(100d, null));
        public static readonly DependencyProperty FreqCentrProperty = DependencyProperty.Register("FreqCentr", typeof(double), typeof(DrawSpectrum), new PropertyMetadata(150000000d, null));
        public static readonly DependencyProperty FreqSpanProperty = DependencyProperty.Register("FreqSpan", typeof(double), typeof(DrawSpectrum), new PropertyMetadata(10000000d, null));
        public static readonly DependencyProperty FreqStartProperty = DependencyProperty.Register("FreqStart", typeof(double), typeof(DrawSpectrum), new PropertyMetadata(145000000d, null));
        public static readonly DependencyProperty FreqStopProperty = DependencyProperty.Register("FreqStop", typeof(double), typeof(DrawSpectrum), new PropertyMetadata(155000000d, null));
        public static readonly DependencyProperty TracePointsProperty = DependencyProperty.Register("TracePoints", typeof(int), typeof(DrawSpectrum), new PropertyMetadata(1601, null));
        public static readonly DependencyProperty LevelUnitProperty = DependencyProperty.Register("LevelUnit", typeof(string), typeof(DrawSpectrum), new PropertyMetadata("dBm", null));
        public static readonly DependencyProperty AdapterProperty = DependencyProperty.Register("Adapter", typeof(ADP.SignalHound.Adapter), typeof(DrawSpectrum), null);


        #endregion
        public DrawSpectrum()
        {
            #region Initialize
            FreqCentr = 100000000;
            FreqSpan = 20000000;
            FreqStart = 90000000;
            FreqStop = 110000000;

            RefLevel = -40;
            LowestLevel = -140;
            Range = 100;

            #endregion
            InitializeComponent();

            glo.DataContext = this;
        }
        public float MAP(float x, float inMin, float inMax, float outMin, float outMax)
        {
            float d = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
            if (d > outMax) d = outMax;
            if (d < outMin) d = outMin;
            return d;
        }
        private void openGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;


            gl.LoadIdentity();
            double wGrid = 0.1;
            if (openGLControl.ActualWidth > 0)
                wGrid = (FreqStop - FreqStart) / openGLControl.ActualWidth;
            gl.Ortho(FreqStart - wGrid, FreqStop + wGrid, LowestLevel - 0.01, RefLevel + 0.01, 1, -1);
        }
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            args.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
            args.OpenGL.ClearColor(0, 0, 0, 0);

        }

        private void openGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            double w = 10, h = 10;
            try
            {
                //Color bg = (Color)this.FindResource("Background_NormalColor");
                //BackgroundRGB[0] = ((float)bg.R) / 256;
                //BackgroundRGB[1] = ((float)bg.G) / 256;
                //BackgroundRGB[2] = ((float)bg.B) / 256;
                //Color fg = (Color)this.FindResource("Foreground_NormalColor");
                //ForegroundRGB[0] = ((float)fg.R) / 256;
                //ForegroundRGB[1] = ((float)fg.G) / 256;
                //ForegroundRGB[2] = ((float)fg.B) / 256;

                if (openGLControl.ActualWidth > 0)
                    w = openGLControl.ActualWidth;
                if (openGLControl.ActualHeight > 0)
                    h = openGLControl.ActualHeight;
            }
            catch { }
            try
            {
                #region Device Drawing
                #region


                Level = Adapter.LevelArr;// new Equipment.TracePoint[rcv.TracePoints];
                Freq = Adapter.FreqArr;
                FreqCentr = (double)Adapter.FreqCentr;
                FreqSpan = (double)Adapter.FreqSpan;
                FreqStop = (double)Adapter.FreqStop;
                FreqStart = (double)Adapter.FreqStart;
                LevelUnit = Adapter.LevelUnit.ToString();
                if (Adapter.LevelUnit == DataModels.Sdrn.DeviceServer.Adapters.Enums.LevelUnit.dBm)
                {
                    RefLevel = (double)Adapter.RefLevel;
                    LowestLevel = (double)Adapter.LowestLevel;
                    Range = (double)Adapter.Range;
                }
                else if (Adapter.LevelUnit == DataModels.Sdrn.DeviceServer.Adapters.Enums.LevelUnit.dBµV)
                {
                    RefLevel = (double)Adapter.RefLevel + 107;
                    LowestLevel = (double)Adapter.LowestLevel + 107;
                    Range = (double)Adapter.Range;
                }

                #endregion


                #endregion
            }
            catch { }
            try
            {
                double wGrid = (FreqStop - FreqStart) / w;
                //OpenGL gl = openGLControl.OpenGL;
                var gl = args.OpenGL;
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
                 
                
                gl.LoadIdentity();
                gl.Ortho2D(FreqStart - wGrid * 0.1, FreqStop + wGrid * 0.1, LowestLevel - 0.01, RefLevel + 0.01);

                //gl.Enable(OpenGL.GL_BLEND);
                gl.ClearColor(BackgroundRGB[0], BackgroundRGB[1], BackgroundRGB[2], 1.0f);//0.89453f, 0.89453f, 0.89453f, 0.0f); //


                double gridW = FreqSpan / 10;
                gl.Begin(BeginMode.Lines);
                //float fcol = 0.3098f; //((i % 10) == 0) ? 0.3f : 0.15f;
                gl.Color(ForegroundRGB[0], ForegroundRGB[1], ForegroundRGB[2]);
                for (int i = 0; i <= 10; i++)
                {
                    gl.Vertex(FreqStart + i * gridW, RefLevel);
                    gl.Vertex(FreqStart + i * gridW, LowestLevel);
                }
                gl.End();

                // Горизонтальные линии сетки
                gl.Begin(BeginMode.Lines);
                gl.Color(ForegroundRGB[0], ForegroundRGB[1], ForegroundRGB[2]);
                //верхняя линия
                gl.Vertex(FreqStart, RefLevel);
                gl.Vertex(FreqStop, RefLevel);

                for (double i = (((int)RefLevel) / 10) * 10; i >= (int)LowestLevel; i -= (RefLevel - LowestLevel) / 10)
                {
                    double shift = FreqStart + wGrid * String.Concat((int)i, LevelUnit).Length * 7.5;
                    gl.Vertex(shift, i);
                    gl.Vertex(FreqStop, i);
                }
                //нижняя линия
                gl.Vertex(FreqStart, LowestLevel);
                gl.Vertex(FreqStop, LowestLevel);
                gl.End();

                //подписи шкалы
                for (int i = 0; i < 11; i++)
                {
                    int y = (int)(((int)RefLevel / 10) * 10 - (Range / 10) * i);
                    if (y < RefLevel - Range / 50 && y > RefLevel - Range + Range / 50)
                    {
                        double y1 = MAP(y, (float)LowestLevel, (float)RefLevel, 0, (float)h);
                        gl.DrawText(2, (int)y1 - 3, ForegroundRGB[0], ForegroundRGB[1], ForegroundRGB[2], "Segoe UI", 10.0f, string.Format(String.Concat((int)(RefLevel / 10) * 10 - (Range / 10) * i, LevelUnit), 10, 8));
                        //gl.End();
                        gl.Flush();

                    }
                }
                //gl.DrawText(10, (int)13, 1.0f, 0.0f, 0.0f, "Segoe UI Mono", 10.0f, string.Format(String.Concat((int)(RefLevel / 10) * 10 - (Range / 10) *1, LevelUnit), 10, 8));
                //gl.Flush();
                if (Freq != null && Freq.Length > 0)
                {
                    gl.Begin(BeginMode.LineStrip);
                    gl.Color(Trace1RGB[0], Trace1RGB[1], Trace1RGB[2]);
                    for (int i = 0; i < Freq.Length; i++)
                    {
                        gl.Vertex(Freq[i], Level[i]);

                    }
                    gl.End();
                    //double[] data1 = new double[Trace1.Length * 2];
                    //for (int i = 0; i < Trace1.Length; i++)
                    //{


                    //    data1[i * 2] = (double)Trace1[i].Freq;
                    //    data1[i * 2 + 1] = (double)Trace1[i].Level;

                    //}
                    //gl.PushMatrix();

                    ////gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
                    ////gl.VertexPointer(2, 0, data1);
                    ////gl.DrawArrays(OpenGL.GL_POINTS, 0, sh.RealTimeFrame.Length);
                    ////gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
                    ////gl.End();
                    //gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
                    //gl.VertexPointer(2, 0, data1);
                    //gl.DrawArrays(OpenGL.GL_LINE_STRIP, 0, Trace1.Length);
                    //gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
                    //gl.End();
                    //gl.PopMatrix();
                }



                //if (RFOverload != 0)//&& !ShowUpPanel)
                //{
                //    if (RFOverload == 1) gl.DrawText((int)w - 110, (int)h - 13, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, "RF Overload");
                //    else if (RFOverload == 1) gl.DrawText((int)w - 110, (int)h - 13, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, "IF Overload");
                //    gl.Flush();
                //}
                gl.Flush();
            }
            catch { }
        }
    }
}
