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
        double RefLevel2 = 0.02;
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
        public ADP.RSFPL.Adapter ANAdapter
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

        public int HMax
        {
            get { return (int)GetValue(HMaxProperty); }
            set { SetValue(HMaxProperty, value); }
        }
        public static readonly DependencyProperty HMaxProperty = DependencyProperty.Register("HMax", typeof(int), typeof(DrawIQ), new PropertyMetadata(1601, null));
        public int HMin
        {
            get { return (int)GetValue(HMinProperty); }
            set { SetValue(HMinProperty, value); }
        }
        public static readonly DependencyProperty HMinProperty = DependencyProperty.Register("HMin", typeof(int), typeof(DrawIQ), new PropertyMetadata(1601, null));

        public int HMaxDraw
        {
            get { return (int)GetValue(HMaxDrawProperty); }
            set { SetValue(HMaxDrawProperty, value); }
        }
        public static readonly DependencyProperty HMaxDrawProperty = DependencyProperty.Register("HMaxDraw", typeof(int), typeof(DrawIQ), new PropertyMetadata(1601, null));
        public int HMinDraw
        {
            get { return (int)GetValue(HMinDrawProperty); }
            set { SetValue(HMinDrawProperty, value); }
        }
        public static readonly DependencyProperty HMinDrawProperty = DependencyProperty.Register("HMinDraw", typeof(int), typeof(DrawIQ), new PropertyMetadata(1601, null));
        public int HMarkerDraw
        {
            get { return (int)GetValue(HMarkerDrawProperty); }
            set { SetValue(HMarkerDrawProperty, value); }
        }
        public static readonly DependencyProperty HMarkerDrawProperty = DependencyProperty.Register("HMarkerDraw", typeof(int), typeof(DrawIQ), new PropertyMetadata(1601, null));

        double CircleRefLevel2 = 0.02;
        public double CircleRefLevel
        {
            get { return (double)GetValue(CircleRefLevelProperty); }
            set { SetValue(CircleRefLevelProperty, value); }
        }
        public static readonly DependencyProperty CircleRefLevelProperty = DependencyProperty.Register("CircleRefLevel", typeof(double), typeof(DrawIQ), new PropertyMetadata(0.000004d, null));
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
            HMin = 0;
            HMax = 1600;
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
                wGrid = (HMaxDraw - 0) / openGLControl.ActualWidth;
            //gl.Ortho(0 - wGrid, IQ.Length / 2 + wGrid, LowestLevel - 0.01, RefLevel + 0.01, 1, -1);
            gl.Ortho(HMinDraw - wGrid, HMaxDraw + wGrid, LowestLevel, RefLevel2, 1, -1);
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
                if ((this.Name == "ANIQ"|| this.Name == "AN2IQ") && ANAdapter != null)
                {
                    IQ = ANAdapter.IQArr;
                    TriggerOffset = (double)ANAdapter.TriggerOffset;
                    SampleTimeLength = (double)ANAdapter.SampleTimeLength;
                    HMin = 0;
                    HMax = IQ.Length / 2;
                }
                if (this.Name == "SHIQ" && SHAdapter != null)
                {
                    IQ = SHAdapter.IQArr;
                    TriggerOffset = (double)SHAdapter.TriggerOffset;
                    SampleTimeLength = (double)SHAdapter.OneSampleDuration;
                    HMin = 0;
                    HMax = IQ.Length / 2;
                }
                //IQ = ANAdapter.IQArr;
                //Level = ANAdapter.LevelArr;// new Equipment.TracePoint[rcv.TracePoints];
                //Freq = ANAdapter.FreqArr;

                RefLevel2 = RefLevel * RefLevel;
                #endregion


                #endregion
            }
            catch (Exception e)
            {
            }
            try
            {
                double wGrid = (HMaxDraw - 0) / w;
                //OpenGL gl = openGLControl.OpenGL;
                var gl = args.OpenGL;
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


                gl.LoadIdentity();
                gl.Ortho2D(HMinDraw - wGrid * 0.1, HMaxDraw + wGrid * 0.1, LowestLevel, RefLevel2);
                //gl.Ortho2D(0 - wGrid * 0.1, IQ.Length / 2 + wGrid * 0.1, LowestLevel - 0.01, RefLevel + 0.01);

                //gl.Enable(OpenGL.GL_BLEND);
                gl.ClearColor(BackgroundRGB[0], BackgroundRGB[1], BackgroundRGB[2], 1.0f);//0.89453f, 0.89453f, 0.89453f, 0.0f); //


                double gridW = HMaxDraw / 20;
                gl.Begin(BeginMode.Lines);
                //float fcol = 0.3098f; //((i % 10) == 0) ? 0.3f : 0.15f;
                gl.Color(ForegroundRGB[0], ForegroundRGB[1], ForegroundRGB[2]);
                for (int i = 0; i <= 10; i++)
                {
                    gl.Vertex(HMinDraw + i * gridW, RefLevel2);
                    gl.Vertex(HMinDraw + i * gridW, LowestLevel);
                }
                gl.End();

                // Горизонтальные линии сетки
                gl.Begin(BeginMode.Lines);
                gl.Color(ForegroundRGB[0], ForegroundRGB[1], ForegroundRGB[2]);
                //верхняя линия
                gl.Vertex(HMinDraw, RefLevel2);
                gl.Vertex(HMaxDraw / 2, RefLevel2);

                for (double i = (((int)RefLevel2) / 10) * 10; i >= (int)LowestLevel; i -= (RefLevel2 - LowestLevel) / 10)
                {
                    double shift = HMinDraw + wGrid * String.Concat((int)i, LevelUnit).Length * 7.5;
                    gl.Vertex(shift, i);
                    gl.Vertex(HMaxDraw / 2, i);
                }
                //нижняя линия
                gl.Vertex(HMinDraw, LowestLevel);
                gl.Vertex(HMaxDraw / 2, LowestLevel);
                gl.End();

                //подписи шкалы
                for (int i = 0; i < 11; i++)
                {
                    int y = (int)(((int)RefLevel2 / 10) * 10 - (Range / 10) * i);
                    if (y < RefLevel2 - Range / 50 && y > RefLevel2 - Range + Range / 50)
                    {
                        double y1 = MAP(y, (float)LowestLevel, (float)RefLevel2, 0, (float)h);
                        gl.DrawText(2, (int)y1 - 3, ForegroundRGB[0], ForegroundRGB[1], ForegroundRGB[2], "Segoe UI", 10.0f, string.Format(String.Concat((int)(RefLevel / 10) * 10 - (Range / 10) * i, LevelUnit), 10, 8));
                        //gl.End();
                        gl.Flush();

                    }
                }
                gl.DrawText((int)w - 210, (int)h - 15, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, (IQ.Length / 2).ToString());
                gl.DrawText((int)w - 210, (int)h - 50, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, TriggerOffset.ToString());
                gl.DrawText((int)w - 210, (int)h - 70, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, RefLevel2.ToString());
                gl.DrawText((int)w - 210, (int)h - 90, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, (IQ.Length * SampleTimeLength / 2).ToString());
                gl.DrawText((int)w - 210, (int)h - 130, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, (TriggerOffset - HMarkerDraw * SampleTimeLength).ToString());
                //gl.DrawText((int)w - 210, (int)h - 15, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, Freq.Length.ToString() + "  " +
                //    Math.Round(Adapter.RBW, 2) + "  " + Math.Round(Freq[10] - Freq[9], 2));
                gl.Flush();
                if (IQ != null && IQ.Length > 0)
                {
                    double d = 0;
                    gl.Begin(BeginMode.LineStrip);
                    gl.Color(Trace1RGB[0], Trace1RGB[1], Trace1RGB[2]);
                    for (int i = HMinDraw; i < HMaxDraw; i++)
                    {
                        d = Math.Sqrt(Math.Pow(IQ[0 + i * 2], 2) + Math.Pow(IQ[1 + i * 2], 2));
                        gl.Vertex(i, d);

                    }
                    gl.End();

                }
                gl.Flush();

                if ((this.Name == "ANIQ"|| this.Name == "AN2IQ") && ANAdapter != null)
                {
                    //double d = 0;
                    //gl.Begin(BeginMode.LineStrip);
                    //gl.Color(Trace1RGB[1], Trace1RGB[0], Trace1RGB[2]);
                    //for (int i = 0; i < IQ.Length / 2; i++)
                    //{
                    //    d = Math.Sqrt(IQ[0 + i * 2]);
                    //    gl.Vertex(i, d);

                    //}
                    //gl.End();
                    gl.Begin(BeginMode.Lines);
                    //float fcol = 0.3098f; //((i % 10) == 0) ? 0.3f : 0.15f;
                    gl.Color(1f, 0f, 0f);


                    gl.Vertex(TriggerOffset / SampleTimeLength, RefLevel2);
                    gl.Vertex(TriggerOffset / SampleTimeLength, LowestLevel);
                    gl.End();
                    gl.DrawText(0, (int)h - 70, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, "12332132");


                    gl.Flush();
                }
                else if (this.Name == "SHIQ" && SHAdapter != null)
                {
                    //double d = 0;
                    //gl.Begin(BeginMode.LineStrip);
                    //gl.Color(Trace1RGB[1], Trace1RGB[0], Trace1RGB[2]);
                    //for (int i = 0; i < IQ.Length / 2; i++)
                    //{
                    //    d = Math.Sqrt(IQ[1 + i * 2]);
                    //    gl.Vertex(i, d);

                    //}
                    //gl.End();
                    gl.Begin(BeginMode.Lines);
                    //float fcol = 0.3098f; //((i % 10) == 0) ? 0.3f : 0.15f;
                    gl.Color(1f, 0f, 0f);


                    gl.Vertex(TriggerOffset / SampleTimeLength, RefLevel2);
                    gl.Vertex(TriggerOffset / SampleTimeLength, LowestLevel);
                    gl.End();
                    gl.DrawText(0, (int)h - 70, 1.0f, 0.0f, 0.0f, "Segoe UI", 14.0f, "12332132");


                    gl.Flush();
                }
                gl.Begin(BeginMode.Lines);
                //float fcol = 0.3098f; //((i % 10) == 0) ? 0.3f : 0.15f;
                gl.Color(1f, 0f, 0f);
                    gl.Vertex(HMarkerDraw, RefLevel2);
                    gl.Vertex(HMarkerDraw, LowestLevel);
                gl.End();
            }
            catch (Exception e)
            {
            }
        }



        private void CircleGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;


            gl.LoadIdentity();
            gl.Ortho(0 - CircleRefLevel2, CircleRefLevel2, 0 - CircleRefLevel2, CircleRefLevel2, 1, -1);
        }
        private void CircleGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            args.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
            args.OpenGL.ClearColor(0, 0, 0, 0);

        }

        private void CircleGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
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

                //IQ = ANAdapter.IQArr;
                //Level = ANAdapter.LevelArr;// new Equipment.TracePoint[rcv.TracePoints];
                //Freq = ANAdapter.FreqArr;

                CircleRefLevel2 =  CircleRefLevel;


                #endregion
            }
            catch (Exception e)
            {
            }
            try
            {
                double wGrid = (HMaxDraw - 0) / w;
                //OpenGL gl = openGLControl.OpenGL;
                var gl = args.OpenGL;
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


                gl.LoadIdentity();
                gl.Ortho(0 - CircleRefLevel2, CircleRefLevel2, 0 - CircleRefLevel2, CircleRefLevel2, 1, -1);
                //gl.Ortho2D(0 - wGrid * 0.1, IQ.Length / 2 + wGrid * 0.1, LowestLevel - 0.01, RefLevel + 0.01);

                //gl.Enable(OpenGL.GL_BLEND);
                gl.ClearColor(BackgroundRGB[0], BackgroundRGB[1], BackgroundRGB[2], 1.0f);//0.89453f, 0.89453f, 0.89453f, 0.0f); //


                
                if (IQ != null && IQ.Length > 0)
                {
                    double d = 0;
                    gl.Begin(BeginMode.LineStrip);
                    gl.Color(Trace1RGB[0], Trace1RGB[1], Trace1RGB[2]);
                    for (int i = HMinDraw; i < HMaxDraw; i++)
                    {
                        gl.Vertex(IQ[0 + i * 2], IQ[1 + i * 2]);
                    }
                    gl.End();

                }
                gl.Flush();

                
            }
            catch (Exception e)
            {
            }
        }
    }
}
