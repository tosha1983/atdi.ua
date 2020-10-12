using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace Atdi.Test.CalcServer.LowFunction
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            
        }

       



        private void Fill(/*Массив  Arr*/
                        double[] Arr,

                        /*Размеры области*/
                        double LonMin,
                        double LonMax,
                        double LatMin,
                        double LatMax)
        {
            zedGraphControl1.GraphPane.XAxis.IsShowGrid = true;
            zedGraphControl1.GraphPane.YAxis.IsShowGrid = true;
            zedGraphControl1.GraphPane.XAxis.GridDashOn = 10;
            zedGraphControl1.GraphPane.XAxis.GridDashOff = 5;
            zedGraphControl1.GraphPane.YAxis.GridDashOn = 10;
            zedGraphControl1.GraphPane.YAxis.GridDashOff = 5;
            ZedGraph.GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.CurveList.Clear();

            ZedGraph.PointPairList listArr = new ZedGraph.PointPairList();

            listArr.Add(Arr[0], 49.1 );
            listArr.Add(Arr[1], 49.2);
            listArr.Add(Arr[2], 49.4);
            listArr.Add(Arr[3], 49.8);
            listArr.Add(Arr[4], 49.6);
          

            myPane.XAxis.Min = LonMin;
            myPane.XAxis.Max = LonMax;
            myPane.YAxis.Min = LatMin;
            myPane.YAxis.Max = LatMax;
            ZedGraph.LineItem myCurveArr1 = myPane.AddCurve("Arr1", listArr, Color.Blue, ZedGraph.SymbolType.None);


            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double LonMin = 29;
            double LonMax = 30;
            double LatMin = 49;
            double LatMax = 50;


            Fill(new double[5] { 29.1, 29.5, 29.1, 29.4, 29.9 }, LonMin, LonMax, LatMin, LatMax);
        }
    }
}
