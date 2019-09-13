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

        public float[] Level { get; set; }

        public TraceType TraceType { get; set; }

        public int TraceCount { get; set; }

        public double RBW_kHz { get; set; }

        public double SweepTime_s { get; set; }

        public DetectorType DetectorType { get; set; }

        public OnlineMeasType OnlineMeasType { get; set; }

        public int RefLevel_dBm { get; set; }

        public int PreAmp_dB { get; set; }

        public int Att_dB { get; set; }

        public float MaxLevel_dBm { get; } = 0;
        public float MinLevel_dBm { get; } = -180;
        public double StartLevelOnChart { get; set; }
        public double StepLevelOnChart_PointTodB { get; set; }
        public double StartFreqOnChart { get; set; }
        public double StepFreqOnChart_PointToMHz { get; set; }
        public float[] MaxLevels { get; set; }
        public float[] MinLevels { get; set; }
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
            if ((staticData == null) || (staticData.Contaier.Freq_MHz == null) || (staticData.Contaier.Freq_MHz.Length < 2))
            {
                return null;
            }

            // этот объект нужно подготовить и вернуть
            var options = new FastChartGripOptions();
            // константы
            var ColorBorderLine = Colors.Black;
            var ColorVerticalLine = Colors.Black;
            var ColorVerticalLabel = Brushes.Black;
            var ColorHorizontalLine = Colors.DarkSlateGray;
            var ColorHorizontalLabel = Brushes.Black;


            // рисую рамку
            options.BorderLine = new GridLine
            {
                Offset = 0,
                Length = 0,
                Space = 0,
                Thickness = 1,
                Color = ColorBorderLine
            };

            // расчет количества вертикальных линий и шага между ними stepFreqLine
            double stepFreqLine;
            double deltaFreq = staticData.Contaier.Freq_MHz[staticData.Contaier.Freq_MHz.Length - 1] - staticData.Contaier.Freq_MHz[0];
            stepFreqLine = deltaFreq;
            int maxnumber = (int)Math.Floor(context.Width / 100.0);
            int StartIndexDiv = (int)Math.Floor(Math.Log10(deltaFreq));
            double Delitel = Math.Pow(10, StartIndexDiv);
            int number5 = (int)Math.Floor(deltaFreq / (Delitel * 5));
            if (number5 <= maxnumber) { stepFreqLine = Delitel * 5; }
            int number2 = (int)Math.Floor(deltaFreq / (Delitel * 2));
            if (number2 <= maxnumber) { stepFreqLine = Delitel * 2; }
            int number1 = (int)Math.Floor(deltaFreq / (Delitel * 1));
            if (number1 <= maxnumber) { stepFreqLine = Delitel; }

            // определение первого отступа
            var startFreq = staticData.Contaier.Freq_MHz[0];
            double PointInMHz = context.Width / deltaFreq;
            staticData.Contaier.StartFreqOnChart = startFreq;
            staticData.Contaier.StepFreqOnChart_PointToMHz = PointInMHz;
            double startDelta = Math.Ceiling(startFreq / stepFreqLine) * stepFreqLine - startFreq;

            // определяем точное количество вертикальных линий
            int NumberFreqLine = 0;
            for (int i = 0; 100 > i; i++)
            {
                if ((int)((startDelta + (i) * stepFreqLine) * PointInMHz) > context.Width) { break; }
                NumberFreqLine = i + 1;
            }

            // если вертикальные лини есть то рисуем их
            if (NumberFreqLine > 0)
            {
                options.VerticalPoints = new GridPoint[NumberFreqLine];
                for (int i = 0; NumberFreqLine > i; i++)
                {
                    options.VerticalPoints[i] = new GridPoint();
                    string freq = (Math.Ceiling(startFreq / stepFreqLine) * stepFreqLine + (i) * stepFreqLine).ToString();
                    options.VerticalPoints[i].BottomRightDash = new GridDash
                    {
                        Offset = (int)((startDelta + (i) * stepFreqLine) * PointInMHz),
                        Label = new TextDescriptor { Text = freq, Forecolor = ColorVerticalLabel },
                        Size = context.Height,
                        Thickness = 1,
                        Color = ColorVerticalLine
                    };
                }
            }


            // определяем минимальные и максимальные значения
            var Level = staticData.Contaier.Level;
            float max_level; float min_level;
            if (Level != null)
            {
                max_level = Level[0];
                min_level = Level[0];
                for (int i = 0; Level.Length > i; i++)
                {
                    if (Level[i] > max_level) { max_level = Level[i]; }
                    if (Level[i] < min_level) { min_level = Level[i]; }
                }
            }
            else
            {
                max_level = -40;
                min_level = -120;
            }
            if (staticData.Contaier.RefLevel_dBm == 1000000000) { max_level = max_level + 10; min_level = min_level - 10; }
            else { max_level = staticData.Contaier.RefLevel_dBm; min_level = (float)Math.Min(min_level - 10.0, max_level - 100); }
            if (max_level > staticData.Contaier.MaxLevel_dBm) { max_level = staticData.Contaier.MaxLevel_dBm; }
            if (min_level < staticData.Contaier.MinLevel_dBm) { min_level = staticData.Contaier.MinLevel_dBm; }


            staticData.Contaier.StartLevelOnChart = min_level;
            double PointIndB = context.Height / (max_level - min_level);
            staticData.Contaier.StepLevelOnChart_PointTodB = PointIndB;

            // определяем шаг сетки
            double stepLevelLine;
            double deltaLevel = max_level - min_level;
            stepLevelLine = deltaLevel;
            maxnumber = (int)Math.Floor(context.Height / 25.0);
            StartIndexDiv = (int)Math.Floor(Math.Log10(deltaLevel));
            Delitel = Math.Pow(10, StartIndexDiv);
            number5 = (int)Math.Floor(deltaLevel / (Delitel * 0.5));
            if (number5 <= maxnumber) { stepLevelLine = Delitel * 0.5; }
            number2 = (int)Math.Floor(deltaLevel / (Delitel * 0.2));
            if (number2 <= maxnumber) { stepLevelLine = Delitel * 0.2; }
            number1 = (int)Math.Floor(deltaLevel / (Delitel * 0.1));
            if (number1 <= maxnumber) { stepLevelLine = Delitel * 0.1; }

            // определение первого отступа
            startDelta = Math.Ceiling(min_level / stepLevelLine) * stepLevelLine - min_level;

            // определяем точное количество вертикальных линий
            int NumberLevelLine = 0;
            for (int i = 0; 100 > i; i++)
            {
                if ((int)((startDelta + (i) * stepLevelLine) * PointIndB) > context.Height) { break; }
                NumberLevelLine = i + 1;
            }

            // если вертикальные лини есть то рисуем их
            if (NumberLevelLine > 0)
            {
                options.HorizontalPoints = new GridPoint[NumberLevelLine];
                for (int i = 0; NumberLevelLine > i; i++)
                {
                    options.HorizontalPoints[i] = new GridPoint();
                    string level1 = (Math.Ceiling(min_level / stepLevelLine) * stepLevelLine + (i) * stepLevelLine).ToString();
                    options.HorizontalPoints[i].LeftTopDash = new GridDash
                    {
                        Offset = (int)((startDelta + (i) * stepLevelLine) * PointIndB),
                        Label = new TextDescriptor { Text = level1, Forecolor = ColorHorizontalLabel },
                        Size = context.Width,
                        Thickness = 1,
                        Color = ColorVerticalLine
                    };
                }
            }
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
            var ColorMaxLine = Colors.Red;
            var ColorMinLine = Colors.Blue;
            var ColorLine = Colors.Black;
            staticData.LeftTitle = null;
            if ((dynamicData == null) || (dynamicData.Contaier.Level.Length != staticData.Contaier.Freq_MHz.Length))
            {
                return;
            }
            var levels = dynamicData.Contaier.Level;
            if (staticData.Contaier.Level == null)
            {
                staticData.Contaier.Level = new float[levels.Length];
                for (int i = 0; levels.Length > i; i++) { staticData.Contaier.Level[i] = levels[i]; }
            }
            if ((staticData.Contaier.MaxLevels == null) || (levels.Length != staticData.Contaier.MaxLevels.Length))
            {
                staticData.Contaier.MaxLevels = new float[levels.Length];
                for (int i = 0; levels.Length > i; i++) { staticData.Contaier.MaxLevels[i] = levels[i]; }
            }
            if ((staticData.Contaier.MinLevels == null) || (levels.Length != staticData.Contaier.MinLevels.Length))
            {
                staticData.Contaier.MinLevels = new float[levels.Length];
                for (int i = 0; levels.Length > i; i++) { staticData.Contaier.MinLevels[i] = levels[i]; }
            }
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] > staticData.Contaier.MaxLevels[i])
                {
                    staticData.Contaier.MaxLevels[i] = levels[i];
                }
                if (levels[i] < staticData.Contaier.MinLevels[i])
                {
                    staticData.Contaier.MinLevels[i] = levels[i];
                }
            }

            var points = getPolyline(levels, staticData);
            if (points != null) {context.PushPolyline(points, ColorLine);}
            var pointsMax = getPolyline(staticData.Contaier.MaxLevels, staticData);
            if (pointsMax != null) { context.PushPolyline(pointsMax, ColorMaxLine); }
            var pointsMin = getPolyline(staticData.Contaier.MinLevels, staticData);
            if (pointsMin != null) { context.PushPolyline(pointsMin, ColorMinLine); }
        }
        private int[] getPolyline(float[] levels, IFastChartData<OnlineMeasLineChartStaticData> staticData)
        {
            var freq = staticData.Contaier.Freq_MHz;
            var startFreq = staticData.Contaier.StartFreqOnChart;
            var stepFreq = staticData.Contaier.StepFreqOnChart_PointToMHz;
            var startLevel = staticData.Contaier.StartLevelOnChart;
            var stepLevel = staticData.Contaier.StepLevelOnChart_PointTodB;
            if (levels.Length != freq.Length) { return null; }
            var points = new int[levels.Length * 2];
            for (int i = 0; i < levels.Length; i++)
            {
                points[i * 2] = (int)Math.Round((freq[i] - startFreq)*stepFreq);
                points[i * 2 + 1] =  (int)Math.Round((levels[i] - startLevel)*stepLevel);
            }
            return points;
        }
    }
}
