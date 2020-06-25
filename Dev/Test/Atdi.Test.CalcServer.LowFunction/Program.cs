using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF = Atdi.Test.DeepServices.Client.WPF;
using Atdi.AppUnits.Sdrn.DeepServices.GN06;
using GE = Atdi.DataModels.Sdrn.DeepServices.GN06;



namespace Atdi.Test.CalcServer.LowFunction
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //var CalcAntennaPattern = new TestCalcAntennaPattern();
            //CalcAntennaPattern.Test();
            //var CalcPropagationLoss = new TestPropagationModel();
            //CalcPropagationLoss.Test();
            //var TestDistance = new TestDistance();
            //TestDistance.Test1();
            var TestGE06 = new TestGE06();
            TestGE06.Test();
            
           
        }
    }
}
