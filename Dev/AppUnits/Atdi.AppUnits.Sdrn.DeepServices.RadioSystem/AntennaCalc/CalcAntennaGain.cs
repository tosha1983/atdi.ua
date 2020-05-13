using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
    internal static class CalcAntennaGain
    {

        public static double Calc(in CalcAntennaGainArgs args)
        {
            var antenna = args.Antenna;
            if (antenna is null) { return 0; }
            var tilt_deg = args.TiltToTarget_deg;
            var azimut_deg = args.AzimutToTarget_deg;
            var polarizationEquipment = args.PolarizationEquipment;
            var polarizationWave = args.PolarizationWave;

            double tilt = tilt_deg - antenna.Tilt_deg;
            double azimut = azimut_deg - antenna.Azimuth_deg;
            float Gain = 0;
            if (antenna.ItuPattern == AntennaItuPattern.None)
            {
                Gain = CalcAntennaGainByPatterns(in antenna, tilt, azimut, polarizationEquipment, polarizationWave);
            }
            else
            {
                Gain = CalcAntennaGainByITUModel(antenna.ItuPattern, antenna.Gain_dB, tilt, azimut);
            }
            return Gain;
        }
        private static float CalcAntennaGainByPatterns(in StationAntenna antenna, double tilt, double azimut, PolarizationType polarizationEquipment, PolarizationType polarizationWave)
        {
            const float addLossCircletoLinear = 3;
            float addLoss = 0;
            if (polarizationWave == PolarizationType.U) { polarizationWave = polarizationEquipment; }
            float GainPattern = antenna.Gain_dB;
            float patternH = 0;
            float patternV = 0;
            switch (polarizationWave)
            {
                case PolarizationType.H:
                    patternH = CalcPatternLossH(in antenna.HhPattern, azimut);
                    patternV = CalcPatternLossV(in antenna.VhPattern, tilt);
                    if (polarizationEquipment == PolarizationType.V)
                    {
                        addLoss = antenna.XPD_dB;
                    }
                    if ((polarizationEquipment == PolarizationType.RL) || (polarizationEquipment == PolarizationType.CL))
                    {
                        addLoss = addLossCircletoLinear;
                    }
                    break;
                case PolarizationType.V:
                    patternH = CalcPatternLossH(in antenna.HvPattern, azimut);
                    patternV = CalcPatternLossV(in antenna.VvPattern, tilt);
                    if (polarizationEquipment == PolarizationType.H)
                    {
                        addLoss = antenna.XPD_dB;
                    }
                    if ((polarizationEquipment == PolarizationType.RL) || (polarizationEquipment == PolarizationType.CL))
                    {
                        addLoss = addLossCircletoLinear;
                    }
                    break;
                case PolarizationType.CL:
                    patternH = Math.Min(CalcPatternLossH(in antenna.HvPattern, azimut), CalcPatternLossH(in antenna.HhPattern, azimut));
                    patternV = Math.Min(CalcPatternLossV(in antenna.VvPattern, tilt), CalcPatternLossV(in antenna.VhPattern, tilt));
                    if (polarizationEquipment == PolarizationType.RL)
                    {
                        addLoss = antenna.XPD_dB;
                    }
                    if ((polarizationEquipment == PolarizationType.H) || (polarizationEquipment == PolarizationType.V))
                    {
                        addLoss = addLossCircletoLinear;
                    }
                    break;
                case PolarizationType.RL:
                    patternH = Math.Min(CalcPatternLossH(in antenna.HvPattern, azimut), CalcPatternLossH(in antenna.HhPattern, azimut));
                    patternV = Math.Min(CalcPatternLossV(in antenna.VvPattern, tilt), CalcPatternLossV(in antenna.VhPattern, tilt));
                    if (polarizationEquipment == PolarizationType.CL)
                    {
                        addLoss = antenna.XPD_dB;
                    }
                    if ((polarizationEquipment == PolarizationType.H) || (polarizationEquipment == PolarizationType.V))
                    {
                        addLoss = addLossCircletoLinear;
                    }
                    break;
                case PolarizationType.M:
                case PolarizationType.U:
                default:
                    patternH = Math.Min(CalcPatternLossH(in antenna.HvPattern, azimut), CalcPatternLossH(in antenna.HhPattern, azimut));
                    patternV = Math.Min(CalcPatternLossV(in antenna.VvPattern, tilt), CalcPatternLossV(in antenna.VhPattern, tilt));
                    break;
            }
            float GainByPatterns = GainPattern - patternH - patternV - addLoss;
            return GainByPatterns;
        }
        /// <summary>
        /// Calculation Horizontal pattern loss 
        /// </summary>
        /// <param name="pattern">Have to normalized angle from 0 to 360</param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static float CalcPatternLossH(in StationAntennaPattern pattern, double angle)
        { // НЕ ТЕСТИЛ
            if (pattern.Angle_deg.Length == 0) { return 0; }
            if (pattern.Angle_deg.Length == 1) { return pattern.Loss_dB[0]; }
            if (angle > 360) { angle = angle % 360; }
            if (angle < 0) { angle = -Math.Floor(angle / 360) * 360 + angle; }
            double PatternLoss = 0;
            if (pattern.Angle_deg[0] > angle)
            {
                PatternLoss = pattern.Loss_dB[pattern.Angle_deg.Length - 1] + (pattern.Loss_dB[0] - pattern.Loss_dB[pattern.Angle_deg.Length - 1]) * (angle + 360 - pattern.Angle_deg[pattern.Angle_deg.Length - 1]) / (pattern.Angle_deg[0] + 360 - pattern.Angle_deg[pattern.Angle_deg.Length - 1]);
            }
            else if (pattern.Angle_deg[pattern.Angle_deg.Length - 1] < angle)
            {
                PatternLoss = pattern.Loss_dB[pattern.Angle_deg.Length - 1] + (pattern.Loss_dB[0] - pattern.Loss_dB[pattern.Angle_deg.Length - 1]) * (angle - pattern.Angle_deg[pattern.Angle_deg.Length - 1]) / (pattern.Angle_deg[0] + 360 - pattern.Angle_deg[pattern.Angle_deg.Length - 1]);
            }
            else
            {
                for (int i = 0; pattern.Angle_deg.Length - 1 > i; i++)
                {
                    if ((angle > pattern.Angle_deg[i]) && (angle < pattern.Angle_deg[i + 1]))
                    {
                        PatternLoss = pattern.Loss_dB[i] + (pattern.Loss_dB[i + 1] - pattern.Loss_dB[i]) * (angle - pattern.Angle_deg[i]) / (pattern.Angle_deg[i + 1] - pattern.Angle_deg[i]);
                        break;
                    }
                }
            }
            return (float)PatternLoss;
        }
        /// <summary>
        /// Calculation Vertical pattern loss 
        /// </summary>
        /// <param name="pattern">Have to normalized angle from -90 (terran) to 90</param>
        /// <param name="angle">from - 90 to 90</param>
        /// <returns></returns>
        private static float CalcPatternLossV(in StationAntennaPattern pattern, double angle)
        { // НЕ ТЕСТИЛ
            if (pattern.Angle_deg.Length == 0) { return 0; }
            if (pattern.Angle_deg.Length == 1) { return pattern.Loss_dB[0]; }
            if (angle > pattern.Angle_deg[pattern.Angle_deg.Length - 1]) { return pattern.Loss_dB[pattern.Angle_deg.Length - 1]; }
            if (angle < pattern.Angle_deg[0]) { return pattern.Loss_dB[0]; }
            double PatternLoss = 0;
            for (int i = 0; pattern.Angle_deg.Length - 1 > i; i++)
            {
                if ((angle > pattern.Angle_deg[i]) && (angle < pattern.Angle_deg[i + 1]))
                {
                    PatternLoss = pattern.Loss_dB[i] + (pattern.Loss_dB[i + 1] - pattern.Loss_dB[i]) * (angle - pattern.Angle_deg[i]) / (pattern.Angle_deg[i + 1] - pattern.Angle_deg[i]);
                    break;
                }
            }
            return (float)PatternLoss;
        }
        private static float CalcAntennaGainByITUModel(AntennaItuPattern antennaItuPattern, float gain, double tilt, double azimut)
        {
            return gain;
        }
    }
}
