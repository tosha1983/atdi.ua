using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDR.Server.MeasurementProcessing;
using Atdi.SDR.Server.MeasurementProcessing.SingleHound.ProcessSignal;

namespace Atdi.SDR.Server.MeasurementProcessing.Measurement
{
    public class Bandwidth
    {
        public MeasSdrBandwidthResults measSdrBandwidthResults;
        public FSemples[] fSemples;
        public Bandwidth(ISDR SDR, TaskParameters taskParameters, SensorParameters sensorParameters, LastResultParameters lastResultParameters)
        {
            // const 
            BandwidthEstimation.BandwidthEstimationType bandwidthEstimationType = BandwidthEstimation.BandwidthEstimationType.xFromCentr;
            double X_Beta = 25;
            // end const
            // получение потока данных
            Trace trace = new Trace(SDR, taskParameters, sensorParameters, lastResultParameters);
            // измерение произведено и находится в trace.fSemples
            double[] SpecrtumArrdBm = new double[trace.fSemples.Length];
            for (int i = 0; i< trace.fSemples.Length; i++)
            {SpecrtumArrdBm[i] = trace.fSemples[i].LeveldBm;}
            // Расчет BW
            measSdrBandwidthResults = BandwidthEstimation.GetBandwidthPoint(SpecrtumArrdBm, bandwidthEstimationType, X_Beta);
            SDRTraceParameters traceParameters = SDR.GetSDRTraceParameters();
            measSdrBandwidthResults.BandwidthkHz = traceParameters.StepFreq_Hz * (measSdrBandwidthResults.T2 - measSdrBandwidthResults.T1) / 1000.0;
            // заполнение результатов по сути
            fSemples = trace.fSemples;
        }
    }
}



