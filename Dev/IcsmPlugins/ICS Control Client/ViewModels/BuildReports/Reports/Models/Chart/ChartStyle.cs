using System;
using System.Windows;
using System.Drawing;

namespace XICSM.ICSControlClient.ViewModels.Chart
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


        public ChartStyle()
        {
        }

        public ChartType ChartType
        {
            get => this._chartType;
            set => this._chartType = value;
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

    }
}
