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

        public List<double[]> Arr1 = new List<double[]>();
        private void Fill(/*Массив  Arr*/
                        List<double[]> Arr,

                        /*Размеры области*/
                        double LonMin,
                        double LonMax,
                        double LatMin,
                        double LatMax)
        {
            zedGraphControl1.GraphPane.XAxis.IsShowGrid = true;
            zedGraphControl1.GraphPane.YAxis.IsShowGrid = true;
            zedGraphControl1.GraphPane.XAxis.GridDashOn = 20;
            zedGraphControl1.GraphPane.XAxis.GridDashOff = 10;
            zedGraphControl1.GraphPane.YAxis.GridDashOn = 20;
            zedGraphControl1.GraphPane.YAxis.GridDashOff = 10;
            zedGraphControl1.GraphPane.Title = "Lat Average Pow";
            ZedGraph.GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.CurveList.Clear();

            ZedGraph.PointPairList listArr = new ZedGraph.PointPairList();
            myPane.XAxis.Min = LonMin;
            myPane.XAxis.Max = LonMax;
            myPane.YAxis.Min = LatMin;
            myPane.YAxis.Max = LatMax;
            for (int j = 0; Arr.Count>j; j++)
            {
                var Arrtemp = Arr[j];
                for (int i = 0; Arrtemp.Length > i; i = i + 2)
                {
                    listArr.Add(Arrtemp[i], Arrtemp[i + 1]);
                }
                switch (j)
                {
                    case 0:
                        ZedGraph.LineItem myCurveArr0 = myPane.AddCurve("100m Near", listArr, Color.Blue, ZedGraph.SymbolType.None);
                        break;
                    case 1:
                        ZedGraph.LineItem myCurveArr1 = myPane.AddCurve("100m", listArr, Color.PowderBlue, ZedGraph.SymbolType.None);
                        break;
                    case 2:
                        ZedGraph.LineItem myCurveArr2 = myPane.AddCurve("200m Near", listArr, Color.Black, ZedGraph.SymbolType.None);
                        break;
                    case 3:
                        ZedGraph.LineItem myCurveArr3 = myPane.AddCurve("200m", listArr, Color.Gray, ZedGraph.SymbolType.None);
                        break;
                    case 4:
                        ZedGraph.LineItem myCurveArr4 = myPane.AddCurve("300m Near", listArr, Color.Red, ZedGraph.SymbolType.None);
                        break;
                    case 5:
                        ZedGraph.LineItem myCurveArr5 = myPane.AddCurve("300m", listArr, Color.Red, ZedGraph.SymbolType.None);
                        break;
                }
            }
             zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double LonMin = 9999;
            double LonMax = -9999;
            double LatMin = 9999;
            double LatMax = -9999;
            for (int j = 0; j < Arr1.Count; j++)
            {
                var Arr2 = Arr1[j];

                for (int i = 0; i < Arr2.Length; i = i + 2)
                {
                    if (LonMin > Arr2[i]) { LonMin = Arr2[i]; }
                    if (LonMax < Arr2[i]) { LonMax = Arr2[i]; }
                    if (LatMin > Arr2[i + 1]) { LatMin = Arr2[i + 1]; }
                    if (LatMax < Arr2[i + 1]) { LatMax = Arr2[i + 1]; }
                }
            }
            Fill(Arr1, LonMin, LonMax, LatMin, LatMax);
        }
    }
}
