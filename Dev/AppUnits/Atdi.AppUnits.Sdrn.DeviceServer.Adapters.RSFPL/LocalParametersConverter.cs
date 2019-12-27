using System;
using System.Collections.Generic;
using System.Linq;
using ENP = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL
{
    public class LocalParametersConverter
    {       
        public decimal FreqCentrIQ(LocalSpectrumAnalyzerInfo uniqueData, decimal FreqStartFromParameter, decimal FreqStopFromParameter)
        {
            if (FreqStopFromParameter < uniqueData.FreqMin || FreqStopFromParameter > uniqueData.FreqMax)
            {
                throw new Exception("The stop frequency must be set to the available range of the instrument.");
            }
            if (FreqStartFromParameter < uniqueData.FreqMin || FreqStartFromParameter > uniqueData.FreqMax)
            {
                throw new Exception("The start frequency must be set to the available range of the instrument.");
            }
            return (FreqStartFromParameter + FreqStopFromParameter) / 2;
        }
        public decimal FreqSpanIQ(LocalSpectrumAnalyzerInfo uniqueData, decimal FreqStartFromParameter, decimal FreqStopFromParameter)
        {
            if (FreqStopFromParameter < uniqueData.FreqMin || FreqStopFromParameter > uniqueData.FreqMax)
            {
                throw new Exception("The stop frequency must be set to the available range of the instrument.");
            }
            if (FreqStartFromParameter < uniqueData.FreqMin || FreqStartFromParameter > uniqueData.FreqMax)
            {
                throw new Exception("The start frequency must be set to the available range of the instrument.");
            }
            decimal res = FreqStopFromParameter - FreqStartFromParameter;
            if (uniqueData.InstrManufacture == 1)
            {
                if (res > uniqueData.IQMaxSampleSpeed * 0.8m)//т.к. такой прибамбас на всех анализаторах от R&S
                {
                    throw new Exception("The IQ band must be set to the available instrument range.");
                }
            }
            else if (uniqueData.InstrManufacture == 2)
            {

            }
            else if (uniqueData.InstrManufacture == 3)
            {

            }

            return res;
        }
        
        public (decimal, bool) SweepTime(LocalSpectrumAnalyzerInfo uniqueData, decimal SweepTimeFromParameter)
        {
            decimal res = 0;
            bool auto = false;
            if (SweepTimeFromParameter != -1)
            {
                if (SweepTimeFromParameter < uniqueData.SWTMin || SweepTimeFromParameter > uniqueData.SWTMax)
                {
                    throw new Exception("The SweepTime must be set to the available range of the instrument.");
                }
                else
                {
                    res = SweepTimeFromParameter;
                }
            }
            else
            {
                auto = true;
            }
            return (res, auto);
        }
              
        
        public decimal SampleSpeed(LocalSpectrumAnalyzerInfo uniqueData, decimal SampleSpeedFromParameter)
        {
            decimal res = 10000;
            if (SampleSpeedFromParameter < uniqueData.IQMinSampleSpeed || SampleSpeedFromParameter > uniqueData.IQMaxSampleSpeed)
            {
                throw new Exception("The SampleSpeed must be set to the available range of the instrument.");
            }
            else
            {
                res = SampleSpeedFromParameter;
            }

            if (uniqueData.InstrManufacture == 1)
            {

            }
            else if (uniqueData.InstrManufacture == 2)
            {

            }
            else if (uniqueData.InstrManufacture == 3)
            {

            }

            return res;
        }
    }
}
