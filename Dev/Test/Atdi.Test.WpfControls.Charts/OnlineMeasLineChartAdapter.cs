using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Atdi.WpfControls.Charts;


namespace Atdi.Test.WpfControls.Charts
{
    public class OnlineMeasLineChartStaticData
    {
        public double[] Freq_MHz { get; set; }

        public TraceType TraceType { get; set; }

        public int TraceCount { get; set; }

        public double RBW_kHz { get; set; }

        public double SweepTime_s { get; set; }

        public DetectorType DetectorType { get; set; }

        public OnlineMeasType OnlineMeasType { get; set; }

        public int RefLevel_dBm { get; set; }

        public int PreAmp_dB { get; set; }

        public int Att_dB { get; set; }
    }

    public class OnlineMeasLineChartDynamicData
    {
        /// <summary>
        /// Level
        /// </summary>
        public float[] Level { get; set; }

        /// <summary>
        /// индикатор overload 
        /// </summary>
        public bool Overload { get; set; }
    }

    public class OnlineMeasLineChartAdapter : FastChartDataAdapterBase<OnlineMeasLineChartStaticData, OnlineMeasLineChartDynamicData>
    {
        /// <summary>
        /// Метод подготовки опций рисования фоновой сетки с подписями, линиями и черточками 
        /// </summary>
        /// <param name="staticData">Статические данные результавов измерения в клбючая статическое описание чарта в целом</param>
        /// <param name="context">Объект содержит разрешение рабочей области рисованияю Отсчет координат с 0.0, нижний левый угол )</param>
        /// <returns>Объект описатель сетки</returns>
        public override FastChartGripOptions DefineGrid(IFastChartData<OnlineMeasLineChartStaticData> staticData, IFastChartContext context)
        {
            // этот объект нужно подготовить и вернуть
            var options = new FastChartGripOptions();
            //context.Height // высота
            // context.Width // ширина
            for (int x = 0; x < context.Width; x++)
            {

            }
            for (int y = 0; y < context.Height; y++)
            {

            }
            
            // тут готовим сетку
            for (int i = 0; i < staticData.Contaier.Freq_MHz.Length; i++)
            {

            }
            options.BorderLine = new GridLine
            {
                Offset = 0,
                Length = 4,
                Space = 0,
                Thickness = 3,
                Color = Colors.DarkGreen
            };

            options.HorizontalPoints = new GridPoint[]
            {
                new GridPoint
                {
                    LeftTopDash = new GridDash
                    {
                        Offset = 50,
                        Label = new TextDescriptor { Text = "Text on Left", Forecolor = Brushes.Red },
                        Size = 50,
                        Thickness = 3,
                        Color = Colors.Red
                    },
                    BottomRightDash = new GridDash
                    {
                        Offset = 50,
                        Label = new TextDescriptor { Text = "Text on Left", Forecolor = Brushes.Red },
                        Size = 120,
                        Thickness = 30,
                        Color = Colors.Green
                    }
                },

                new GridPoint
                {
                    LeftTopDash = new GridDash
                    {
                        Offset = 150,
                        Label = new TextDescriptor { Text = "Text on Left", Forecolor = Brushes.Red },
                        Size = 50,
                        Thickness = 3,
                        Color = Colors.Red
                    },
                    BottomRightDash = new GridDash
                    {
                        Offset = 150,
                        Label = new TextDescriptor { Text = "Text on Left", Forecolor = Brushes.Red },
                        Size = 120,
                        Thickness = 30,
                        Color = Colors.Green
                    }
                }
            };
            // возвращаем описание сетки
            return options;
        }

        /// <summary>
        /// Метод рисования нужного изображения графика соглансо статичесикх и дитнамических данных
        /// Инструменты(методы) рисования в объекте контекста
        /// </summary>
        /// <param name="staticData"></param>
        /// <param name="dynamicData"></param>
        /// <param name="context"></param>
        public override void DrawImage(IFastChartData<OnlineMeasLineChartStaticData> staticData, IFastChartData<OnlineMeasLineChartDynamicData> dynamicData, IFastChartContext context)
        {
            if (dynamicData == null)
            {
                return;
            }

            var levels = dynamicData.Contaier.Level;
            var yMin = -120;
            var yMax = 0;

            for (int i = 0; i < levels.Length; i++)
            {

                var y = (int)(levels[i] - yMin) * context.Height / (yMax - yMin);
                if (i < context.Width && y >= 0 && y < context.Height)
                {
                    context.PushPixel(i, y, Colors.Red);
                }

            }

            var points = new int[levels.Length * 2];

            for (int i = 0; i < levels.Length; i++)
            {
                points[i * 2] = i;
                points[i * 2 + 1] = (int)(levels[i] - yMin) * context.Height / (yMax - yMin);

            }
            context.PushPolyline(points, Colors.YellowGreen);
        }
    }
}
