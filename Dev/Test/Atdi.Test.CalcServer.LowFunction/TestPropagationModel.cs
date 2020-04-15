using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.DeepServices.RadioSystem;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal;

namespace Atdi.Test.CalcServer.LowFunction
{
    class TestPropagationModel
    {
        private PropagationModel FilltModel()
        {
            PropagationModel propagationModel = new PropagationModel()
            {
                AbsorptionBlock = new AbsorptionCalcBlock()
                {
                    Available = true,
                    ModelType = AbsorptionCalcBlockModelType.FlatAndLinear
                },
                AdditionalBlock = new AdditionalCalcBlock()
                {
                    Available = false
                },
                AtmosphericBlock = new AtmosphericCalcBlock()
                {
                    Available = false
                },
                ClutterBlock = new ClutterCalcBlock()
                {
                    Available = false
                },
                DiffractionBlock = new DiffractionCalcBlock()
                {
                    Available = true,
                    ModelType = DiffractionCalcBlockModelType.Deygout91
                },
                DuctingBlock = new DuctingCalcBlock()
                {
                    Available = false
                },
                MainBlock = new MainCalcBlock()
                {
                    ModelType = MainCalcBlockModelType.ITU525
                },
                Parameters = new GlobalParams()
                {
                    EarthRadius_km = 8500,
                    Location_pc = 50,
                    Time_pc = 50
                },
                ReflectionBlock = new ReflectionCalcBlock()
                {
                    Available = false
                },
                SubPathDiffractionBlock = new SubPathDiffractionCalcBlock()
                {
                    Available = false
                },
                TropoBlock = new TropoCalcBlock()
                {
                    Available = false
                }
            };
            return propagationModel;
        }

        public void Test()
        {
            TestProfiles testProfile = new TestProfiles();
            CalcLossArgs calcLossArgs = new CalcLossArgs();
            calcLossArgs.Model = FilltModel();
            TestProfiles prof = new TestProfiles();
            for (double Freq = 100; Freq < 40000; Freq = Freq * 1.5)
            {
                calcLossArgs.Freq_Mhz = Freq;
                for (double H1 = 10; H1 < 100; H1 = H1 * 2)
                {
                    calcLossArgs.Ha_m = H1;
                    for (double H2 = 10; H2 < 100; H2 = H2 * 2)
                    {
                        calcLossArgs.Hb_m = H2;
                        for (int i = 0; i<prof.testProfiles.Length; i++)
                        {
                            calcLossArgs.HeightProfile = prof.testProfiles[i].profileH;
                            calcLossArgs.HeightStartIndex = 0;
                            calcLossArgs.ReliefProfile = prof.testProfiles[i].profileR;
                            calcLossArgs.ReliefStartIndex = 0;
                            calcLossArgs.ClutterProfile = prof.testProfiles[i].profileCl;
                            calcLossArgs.ClutterStartIndex = 0;
                            calcLossArgs.BuildingProfile = prof.testProfiles[i].profileB;
                            calcLossArgs.BuildingStartIndex = 0;
                            calcLossArgs.ProfileLength = prof.testProfiles[i].profileR.Length;
                            calcLossArgs.D_km = prof.testProfiles[i].profileDistance_km;
                            var reslts = PropagationLoss.Calc(calcLossArgs);
                        }
                    }
                }
            }
        }
    }
}
