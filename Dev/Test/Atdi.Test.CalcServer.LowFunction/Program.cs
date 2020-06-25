using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF = Atdi.Test.DeepServices.Client.WPF;


namespace Atdi.Test.CalcServer.LowFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            //var CalcAntennaPattern = new TestCalcAntennaPattern();
            //CalcAntennaPattern.Test();
            //var CalcPropagationLoss = new TestPropagationModel();
            //CalcPropagationLoss.Test();
            var TestDistance = new TestDistance();
            TestDistance.Test1();
            WPF.RunApp.Start(WPF.TypeObject.Points, 
                new WPF.Location[] { new WPF.Location(30, 50), new WPF.Location(30, 51), new WPF.Location(31, 51), new WPF.Location(31, 50)}, 
                WPF.TypeObject.Points,
                new WPF.Location[] { new WPF.Location(29, 50), new WPF.Location(29, 51), new WPF.Location(29.5, 51), new WPF.Location(29.5, 50) }
                );
        }
    }
}
