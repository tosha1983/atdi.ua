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
    /// Логика взаимодействия для DrawIQ.xaml
    /// </summary>
    public partial class DrawIQ : UserControl
    {
        #region data
        float[] BackgroundRGB = new float[3] { 0.9372f, 0.9372f, 0.949f };
        float[] ForegroundRGB = new float[3] { 0.1f, 0.1f, 0.1f };
        float[] Trace1RGB = new float[3] { 0.3529f, 0.6274f, 0.7843f };

        public float[] IQ = new float[] { -1, -1, -1, -1 };
        float[] Level = new float[] { };
        double[] Freq = new double[] { };
        #endregion
        #region Property
        public double TriggerOffset = 1;
        public double SampleTimeLength = 1;

        public double RefLevel
        {
            get { return (double)GetValue(RefLevelProperty); }
            set { SetValue(RefLevelProperty, value); }
        }
        public string TriggerOfset { get; set; } = "";
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
        public ADP.SpectrumAnalyzer.Adapter ANAdapter
        {
            get;
            set;
        }
        public ADP.SignalHound.Adapter SHAdapter
        {
            get;
            set;
        }
        public static readonly DependencyProperty RefLevelProperty = DependencyProperty.Register("RefLevel", typeof(double), typeof(DrawIQ), new PropertyMetadata(0d, null));
        public static readonly DependencyProperty LowestLevelProperty = DependencyProperty.Register("LowestLevel", typeof(double), typeof(DrawIQ), new PropertyMetadata(-100d, null));
        public static readonly DependencyProperty RangeProperty = DependencyProperty.Register("Range", typeof(double), typeof(DrawIQ), new PropertyMetadata(100d, null));
        public static readonly DependencyProperty FreqCentrProperty = DependencyProperty.Register("FreqCentr", typeof(double), typeof(DrawIQ), new PropertyMetadata(150000000d, null));
        public static readonly DependencyProperty FreqSpanProperty = DependencyProperty.Register("FreqSpan", typeof(double), typeof(DrawIQ), new PropertyMetadata(10000000d, null));
        public static readonly DependencyProperty FreqStartProperty = DependencyProperty.Register("FreqStart", typeof(double), typeof(DrawIQ), new PropertyMetadata(145000000d, null));
        public static readonly DependencyProperty FreqStopProperty = DependencyProperty.Register("FreqStop", typeof(double), typeof(DrawIQ), new PropertyMetadata(155000000d, null));
        public static readonly DependencyProperty TracePointsProperty = DependencyProperty.Register("TracePoints", typeof(int), typeof(DrawIQ), new PropertyMetadata(1601, null));
        public static readonly DependencyProperty LevelUnitProperty = DependencyProperty.Register("LevelUnit", typeof(string), typeof(DrawIQ), new PropertyMetadata("dBm", null));


        #endregion
        public DrawIQ()
        {
            #region Initialize
            FreqCentr = 100000000;
            FreqSpan = 20000000;
            FreqStart = 90000000;
            FreqStop = 110000000;

            RefLevel = 0.002;// -40;
            LowestLevel = 0;// -0.000001; //-140;
            Range = RefLevel - LowestLevel;

            #endregion
            InitializeComponent();

            glo.DataContext = this;
        }

        private void openGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;


            gl.LoadIdentity();
            double wGrid = 0.1;
            if (openGLControl.ActualWidth > 0)
                wGrid = (IQ.Length / 2 - 0) / openGLControl.ActualWidth;
            //gl.Ortho(0 - wGrid, IQ.Length / 2 + wGrid, LowestLevel - 0.01, RefLevel + 0.01, 1, -1);
            gl.Ortho(0 - wGrid, IQ.Length / 2 + wGrid, LowestLevel, RefLevel, 1, -1);
        }
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            args.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
            args.OpenGL.ClearColor(0, 0, 0, 0);

        }
        public float MAP(float x, float inMin, float inMax, float outMin, float outMax)
        {
            float d = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
            if (d > outMax) d = outMax;
            if (d < outMin) d = outMin;
            return d;
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
                if (this.Name == "ANIQ" && ANAdapter != null)
                {
                    IQ = ANAdapter.IQArr;
                    TriggerOffset = (double)ANAdapter.TriggerOffset;
                    SampleTimeLength = (double)ANAdapter.SampleTimeLength;
                }
                if (this.Name == "SHIQ" && SHAdapter != null)
                {
                    IQ = SHAdapter.IQArr;
                    //TriggerOffset = (double)SHAdapter.TriggerOffset;
                }
                //IQ = ANAdapter.IQArr;
                //Level = ANAdapter.LevelArr;// new Equipment.TracePoint[rcv.TracePoints];
                //Freq = ANAdapter.FreqArr;


                #endregion


                #endregion
            }
            catch (Exception e)
            {
            }
            try
            {
                double wGrid = (IQ.Length / 2 - 0) / w;
                //OpenGL gl = openGLControl.OpenGL;
                var gl = args.OpenGL;
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


                gl.LoadIdentity();
                gl.Ortho2D(0 - wGrid * 0.1, IQ.Length / 2 + wGrid * 0.1, LowestLevel, RefLevel);
                //gl.Ortho2D(0 - wGrid * 0.1, IQ.Length / 2 + wGrid * 0.1, LowestLevel - 0.01, RefLevel + 0.01);

                //gl.Enable(OpenGL.GL_BLEND);
                gl.ClearColor(BackgroundRGB[0], BackgroundRGB[1], BackgroundRGB[2], 1.0f);//0.89453f, 0.89453f, 0.89453f, 0.0f); //


                double gridW = IQ.Length / 20;
                gl.Begin(BeginMode.Lines);
                //float fcol = 0.3098f; //((i % 10) == 0) ? 0.3f : 0.15f;
                gl.Color(ForegroundRGB[0], ForegroundRGB[1], ForegroundRGB[2]);
                for (int i = 0; i <= 10; i++)
                {
                    gl.Vertex(0 + i * gridW, RefLevel);
                    gl.Vertex(0 + i * gridW, LowestLevel);
                }
                gl.End();

                // Горизонтальные линии сетки
                gl.Begin(BeginMode.Lines);
                gl.Color(ForegroundRGB[0], ForegroundRGB[1], ForegroundRGB[2]);
                //верхняя линия
                gl.Vertex(0, RefLevel);
                gl.Vertex(IQ.Length / 2, RefLevel);

                for (double i = (((int)RefLevel) / 10) * 10; i >= (int)LowestLevel; i -= (RefLevel - LowestLevel) / 10)
                {
                    double shift = 0 + wGrid * String.Concat((int)i, LevelUnit).Length * 7.5;
                    gl.Vertex(shift, i);
                    gl.Vertex(IQ.Length / 2, i);
                }
                //нижняя линия
                gl.Vertex(0, LowestLevel);
                gl.Vertex(IQ.Length / 2, LowestLevel);
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
                gl.DrawText((int)w - 210, (int)h - 15, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, (IQ.Length / 2).ToString());
                gl.DrawText((int)w - 210, (int)h - 50, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, TriggerOfset);
                gl.DrawText((int)w - 210, (int)h - 70, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, RefLevel.ToString());
                //gl.DrawText((int)w - 210, (int)h - 15, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, Freq.Length.ToString() + "  " +
                //    Math.Round(Adapter.RBW, 2) + "  " + Math.Round(Freq[10] - Freq[9], 2));
                gl.Flush();
                if (IQ != null && IQ.Length > 0)
                {
                    double d = 0;
                    gl.Begin(BeginMode.LineStrip);
                    gl.Color(Trace1RGB[0], Trace1RGB[1], Trace1RGB[2]);
                    for (int i = 0; i < IQ.Length / 2; i++)
                    {
                        d = Math.Sqrt(Math.Pow(IQ[0 + i * 2], 2) + Math.Pow(IQ[1 + i * 2], 2));
                        gl.Vertex(i, d);

                    }
                    gl.End();
                }
                gl.Flush();

                if (this.Name == "ANIQ" && ANAdapter != null)
                {
                    gl.Begin(BeginMode.Lines);
                    //float fcol = 0.3098f; //((i % 10) == 0) ? 0.3f : 0.15f;
                    gl.Color(1f, 0f, 0f);


                    gl.Vertex(TriggerOffset / SampleTimeLength, RefLevel);
                    gl.Vertex(TriggerOffset / SampleTimeLength, LowestLevel);
                    gl.End();
                    gl.DrawText(0, (int)h - 70, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, "12332132");


                    gl.Flush();
                }
                else if (this.Name == "SHIQ" && SHAdapter != null)
                {
                    //TriggerOffset = (double)SHAdapter.TriggerOffset;
                }
               
            }
            catch (Exception e)
            {
            }
        }
    }
}
