using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDR.Server.MeasurementProcessing
{
    public class LastResultParameters
    {
        public int NN;
        public FSemples[] FSemples;
        public int APIversion;
        public LastResultParameters(object LastResult)
        {
            if (LastResult != null)
            {
                if (LastResult is MeasSdrResults)
                {
                    FillingMeasParameters_v1(LastResult as MeasSdrResults);
                }
                if (LastResult is MeasSdrResults_v2)
                {
                    FillingMeasParameters_v2(LastResult as MeasSdrResults_v2);
                }
            }
        }
        private void FillingMeasParameters_v1(MeasSdrResults measSdrResults)
        {
            APIversion = 1;
            NN = measSdrResults.NN;
            FSemples = measSdrResults.FSemples;
        }
        private void FillingMeasParameters_v2(MeasSdrResults_v2 measSdrResults)
        {
            APIversion = 2;
            NN = measSdrResults.NN;
            FSemples = measSdrResults.FSemples;
        }
    }
    
}
