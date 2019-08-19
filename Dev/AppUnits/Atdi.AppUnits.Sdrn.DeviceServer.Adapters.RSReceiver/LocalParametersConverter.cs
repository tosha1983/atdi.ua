//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Atdi.DataModels.Sdrn.DeviceServer.Adapters;

//namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSReceiver
//{
//    class LocalParametersConverter
//    {

//        public decimal PScanFreqStart(LocalRSReceiverInfo uniqueData, decimal FreqStartFromParameter)
//        {
//            decimal res = 0;
//            if (FreqStartFromParameter < uniqueData.FreqMin) res = uniqueData.FreqMin;
//            else if (FreqStartFromParameter > uniqueData.FreqMax) res = uniqueData.FreqMax - 1000000;
//            else res = FreqStartFromParameter;
//            return res;
//        }
//        public decimal PScanFreqStop(LocalRSReceiverInfo uniqueData, decimal FreqStopFromParameter)
//        {
//            decimal res = 0;
//            if (FreqStopFromParameter > uniqueData.FreqMax) res = uniqueData.FreqMax;
//            else if (FreqStopFromParameter < uniqueData.FreqMin) res = uniqueData.FreqMin + 1000000;
//            else res = FreqStopFromParameter;
//            return res;
//        }
//        public (decimal, int) FFMFreqCentrSpan(LocalRSReceiverInfo uniqueData, decimal FreqStartFromParameter, decimal FreqStopFromParameter)
//        {
//            decimal centr = 0;
//            int span = 0;

//            if (FreqStopFromParameter - FreqStartFromParameter <= uniqueData.FFMSpanBW[uniqueData.FFMSpanBW.Length - 1])
//            {

//                if (FreqStartFromParameter < uniqueData.FreqMin) FreqStartFromParameter = uniqueData.FreqMin;
//                else if (FreqStartFromParameter > uniqueData.FreqMax) FreqStartFromParameter = uniqueData.FreqMax - 1000000;

//                if (FreqStopFromParameter < uniqueData.FreqMin) FreqStopFromParameter = uniqueData.FreqMin + 1000000;
//                else if (FreqStopFromParameter > uniqueData.FreqMax) FreqStopFromParameter = uniqueData.FreqMax;

//                centr = (FreqStartFromParameter + FreqStopFromParameter) / 2;

//                decimal d = FreqStopFromParameter - FreqStartFromParameter;
//                bool set = false;
//                for (int i = 0; i < uniqueData.FFMSpanBW.Length; i++)
//                {
//                    if (!set && uniqueData.FFMSpanBW[i] >= d)
//                    {
//                        set = true;
//                        span = i;
//                    }
//                }
//            }
//            return (centr, span);
//        }
//        public decimal FFMFreqStop(LocalRSReceiverInfo uniqueData, decimal FreqStopFromParameter)
//        {
//            decimal res = 0;
//            if (FreqStopFromParameter > uniqueData.FreqMax) res = uniqueData.FreqMax;
//            else if (FreqStopFromParameter < uniqueData.FreqMin) res = uniqueData.FreqMin + 1000000;
//            else res = FreqStopFromParameter;
//            return res;
//        }
//    }
//}
