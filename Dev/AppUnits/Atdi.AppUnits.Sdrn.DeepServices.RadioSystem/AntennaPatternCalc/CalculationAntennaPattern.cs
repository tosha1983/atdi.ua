using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.AntennaPattern;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern
{
    public static class CalculationAntennaPattern
    {
        public static DiagrammPoint[] Calc(string points, AntennaPatternType antennaPatternType, double gain)
        {
            var diag = new AntennaDiagramm();
            var pointObjects = new List<DiagrammPoint>();
            diag.SetMaximalGain(gain);
            var patternType = diag.Build(points);
            if (patternType == TypePattern.WIEN)
            {
                var AnglesH = new int[72] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180, 185, 190, 195, 200, 205, 210, 215, 220, 225, 230, 235, 240, 245, 250, 255, 260, 265, 270, 275, 280, 285, 290, 295, 300, 305, 310, 315, 320, 325, 330, 335, 340, 345, 350, 355 };
                var AnglesV = new int[65] { -90, -85, -80, -75, -70, -65, -60, -55, -50, -45, -40, -35, -30, -28, -26, -24, -22, -20, -18, -16, -14, -12, -10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90 };
                if ((antennaPatternType == AntennaPatternType.HH) || (antennaPatternType == AntennaPatternType.VH))
                {
                    for (int i = 0; i < AnglesH.Length; i++)
                    {
                        pointObjects.Add(new DiagrammPoint()
                        {
                            Angle = AnglesH[i],
                            Loss = (float)diag.GetLossesAmount(AnglesH[i])
                        });
                    }
                }
                else if ((antennaPatternType == AntennaPatternType.HV) || (antennaPatternType == AntennaPatternType.VV))
                {
                    for (int i = 0; i < AnglesV.Length; i++)
                    {
                        pointObjects.Add(new DiagrammPoint()
                        {
                            Angle = AnglesV[i],
                            Loss = (float)diag.GetLossesAmount(AnglesV[i])
                        });
                    }
                }

            }
            else
            {
                for (int i = 0; i < diag.Angles.Count; i++)
                {
                    pointObjects.Add(new DiagrammPoint()
                    {
                        Angle = diag.Angles[i],
                        Loss = (float)diag.Losses[i]
                    });
                }
            }
            return pointObjects.ToArray();
        }
    }
}
