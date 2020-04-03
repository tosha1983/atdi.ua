using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.DeepServices.RadioSystem;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.Test.CalcServer.LowFunction
{
    class TestCalcAntennaPattern
    {
        public void Test()
        {
            int nstation = 1000;
            int points = 100;
            Random rand = new Random();
            var ArrStation = createStation(nstation);
            SignalService sig = new SignalService();
            float[] resArr = new float[nstation* points];
            DateTime startTime = DateTime.Now;
            for (int i = 0; nstation > i; i++)
            {
                var St = ArrStation[i];
                for (int j = 0; points>j; j++)
                {
                    St.AzimutToPoint_deg = rand.Next(360);
                    St.TiltToPoint_deg = -10 + rand.Next(15);
                    resArr[i*points+j] =  sig.CalcAntennaGain(St);
                }
            }
            TimeSpan timeSpan = DateTime.Now - startTime;
        }
        private CalcAntennaGainArgs[] createStation(int n)
        {
            CalcAntennaGainArgs[] ArrStation = new CalcAntennaGainArgs[n];
            CalcAntennaGainArgs Station = new CalcAntennaGainArgs();
            Array values = Enum.GetValues(typeof(PolarizationType));
            Random random = new Random();
            Random rand = new Random();
            for (int i = 0;  n>i;i++)
            {
                PolarizationType randomBar = (PolarizationType)values.GetValue(random.Next(values.Length));
                Station.PolarizationEquipment = randomBar;
                randomBar = (PolarizationType)values.GetValue(random.Next(values.Length));
                Station.PolarizationWave = randomBar;
                Station.Antenna = new StationAntenna();
                Station.Antenna.Azimuth_deg = rand.Next(360);
                Station.Antenna.Tilt_deg = -10 + rand.Next(15);
                Station.Antenna.XPD_dB = 15 + rand.Next(15);
                Station.Antenna.Gain_dB = rand.Next(30);
                Station.Antenna.HhPattern = StationAntennaPatternH(10 + rand.Next(50), rand.NextDouble()*5.0, 10+ rand.Next(15));
                Station.Antenna.HvPattern = StationAntennaPatternH(10 + rand.Next(50), rand.NextDouble() * 5.0, 10 + rand.Next(15));
                Station.Antenna.VhPattern = StationAntennaPatternV(10 + rand.Next(50), rand.NextDouble() * 5.0, 10 + rand.Next(15));
                Station.Antenna.VvPattern = StationAntennaPatternV(10 + rand.Next(50), rand.NextDouble() * 5.0, 10 + rand.Next(15));
                ArrStation[i] = Station;
            }
            return ArrStation;
        }
        private StationAntennaPattern StationAntennaPatternH(int npoint, double shift, double max_loss)
        {
            Random rand = new Random();
            StationAntennaPattern pattern = new StationAntennaPattern();
            pattern.Angle_deg = new double[npoint];
            pattern.Loss_dB = new float[npoint];
            pattern.Loss_dB[0] = 0;
            pattern.Angle_deg[0] = shift;
            for (int i = 1; i < npoint; i++)
            {
                double step = (360 - pattern.Angle_deg[i-1]) / (npoint - i + 1);
                pattern.Angle_deg[i] = pattern.Angle_deg[i - 1] + rand.NextDouble()*step*2;
                if (i < npoint / 2.0)
                {
                    double stepl = (max_loss - pattern.Loss_dB[i - 1]) / (npoint / 2.0 - i);
                    pattern.Loss_dB[i] = (float)(pattern.Loss_dB[i - 1] + rand.NextDouble() * stepl * 2);
                }
                else if (i > npoint / 2.0)
                {
                    double stepl = (pattern.Loss_dB[i - 1]) / (npoint - i+1);
                    pattern.Loss_dB[i] = (float)(pattern.Loss_dB[i - 1] - rand.NextDouble() * stepl * 2);
                    if (pattern.Loss_dB[i] < 0) { pattern.Loss_dB[i] = 0; }
                }
                else
                {
                    pattern.Loss_dB[i] = (float)max_loss;
                }
            }
            return pattern;
        }
        private StationAntennaPattern StationAntennaPatternV(int npoint, double shift, double max_loss)
        {
            Random rand = new Random();
            StationAntennaPattern pattern = new StationAntennaPattern();
            pattern.Angle_deg = new double[npoint];
            pattern.Loss_dB = new float[npoint];
            pattern.Loss_dB[0] = (float)max_loss;
            pattern.Angle_deg[0] = -90 + shift;
            for (int i = 1; i < npoint; i++)
            {
                double step = (90 - pattern.Angle_deg[i - 1]) / (npoint - i + 1);
                pattern.Angle_deg[i] = pattern.Angle_deg[i - 1] + rand.NextDouble() * step * 2;
                if (i < npoint / 2.0)
                {
                    double stepl = (pattern.Loss_dB[i - 1]) / (npoint / 2.0 - i);
                    pattern.Loss_dB[i] = (float)(pattern.Loss_dB[i - 1] - rand.NextDouble() * stepl * 2);
                    if (pattern.Loss_dB[i] < 0) { pattern.Loss_dB[i] = 0; }
                }
                else if (i > npoint / 2.0)
                {
                    double stepl = (max_loss - pattern.Loss_dB[i - 1]) / (npoint -i +1);
                    pattern.Loss_dB[i] = (float)(pattern.Loss_dB[i - 1] + rand.NextDouble() * stepl * 2);
                }
                else
                {
                    pattern.Loss_dB[i] = 0;
                }
            }
            return pattern;
        }
    }
}
