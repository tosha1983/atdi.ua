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
            Console.WriteLine($"Press any key to start ...");
            Console.ReadLine();

            //var CalcAntennaPattern = new TestCalcAntennaPattern();
            //CalcAntennaPattern.Test();
            //var CalcPropagationLoss = new TestPropagationModel();
            //CalcPropagationLoss.Test();
            //var TestDistance = new TestDistance();
            //TestDistance.Test1();


            TestGE06GetBoundaryPointsFromAllotments.Test();

            //TestGE06EstimationAssignmentsPointsForEtalonNetwork.Test();

            //TestGE06GetEtalonBroadcastingAssignmentFromAllotment.Test();

            //TestGE06GetEtalonBroadcastingAssignmentFromAllotment.Test();


            //CalcBarycenter testBaryCentr = new CalcBarycenter();
            //                       testBaryCentr.Test();

            //PutPointToContour putPointToContour = new PutPointToContour();
            //putPointToContour.Test();

            //CreateContourFromPointByDistance createContourFromPointByDistance = new CreateContourFromPointByDistance();
            //createContourFromPointByDistance.Test();

            //CreateContourForStationByTriggerFieldStrengths createContourForStationByTriggerFieldStrengths = new CreateContourForStationByTriggerFieldStrengths();
            //createContourForStationByTriggerFieldStrengths.Test();

            //CreateContourFromContureByDistance createContourFromContureByDistance = new CreateContourFromContureByDistance();
            //createContourFromContureByDistance.Test();



            //GetADMByPoint getADMByPoint = new GetADMByPoint();
            //getADMByPoint.Test();

            //GetNearestPointByADM getNearestPointByADM = new GetNearestPointByADM();
            //getNearestPointByADM.Test();

            //GetADMByPointAndDistanse GetADMByPointAndDistanse = new GetADMByPointAndDistanse();
            //GetADMByPointAndDistanse.Test();

        }
    }
}
