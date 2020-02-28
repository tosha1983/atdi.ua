using System;
using System.Windows;
using System.Drawing;

namespace Atdi.Modules.Sdrn.Chart
{
    public enum ChartType
    {
        Line,
        Columns
    }
    public class ChartStyle
    {
        private double xmin = 0;
        private double xmax = 10;
        private double ymin = 0;
        private double ymax = 10;
        private ChartType _chartType = ChartType.Line;

        private Bitmap chartCanvas;

        public ChartStyle()
        {
        }

        public ChartType ChartType
        {
            get => this._chartType;
            set => this._chartType = value;
        }

        public Bitmap ChartCanvas
        {
            get { return chartCanvas; }
            set { chartCanvas = value; }
        }

        public double Xmin
        {
            get { return xmin; }
            set { xmin = value; }
        }

        public double Xmax
        {
            get { return xmax; }
            set { xmax = value; }
        }

        public double Ymin
        {
            get { return ymin; }
            set { ymin = value; }
        }

        public double Ymax
        {
            get { return ymax; }
            set { ymax = value; }
        }

        public PointF NormalizePoint(PointF pt)
        {
            PointF result = new PointF();
            result.X = (float)((pt.X - Xmin) * ChartCanvas.Width / (Xmax - Xmin));
            result.Y = (float)(ChartCanvas.Height - (pt.Y - Ymin) * ChartCanvas.Height / (Ymax - Ymin));
            return result;
        }
    }
}
