using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Modules.MonitoringProcess;


namespace Atdi.Modules.MonitoringProcess.Measurement
{
    /// <summary>
    /// Данный клас обслуживат измерения полчения спектрограммы. 
    /// </summary>
    public class Trace
    {
        public float[] Frequencies_MHz;
        public float[] Levels_dBm;
        public SemplFreq[] fSemples;
        public Trace(ISDR SDR, TaskParameters taskParameters, SensorParameters  sensorParameters = null, LastResultParameters lastResultParameters = null)
        {
            if (taskParameters.MeasurementType == MeasType.PICode)
            { // костыль пока не будет добавлено новый тип вместо MeasurementType.PICode 
                GetTraceOnLine(SDR, lastResultParameters);
            }
            else
            {
                if (taskParameters.status == "O") { GetTraceOnLine(SDR, lastResultParameters); }
                else { GetTraceOffLine(SDR, sensorParameters, lastResultParameters); }
            }
        }
        /// <summary>
        /// Получение результатов для измерения Offline результат будет в FSemples
        /// </summary>
        /// <param name="SDR"></param>
        /// <param name="sensor"></param>
        /// <param name="lastResultParameters"></param>
        private void GetTraceOffLine(ISDR SDR, SensorParameters sensor, LastResultParameters lastResultParameters)
        {
            float[] CurLevels_dBm = SDR.GetTrace(); // получили результат
            if (CurLevels_dBm == null) { return; }
            var TraceParameters = SDR.GetSDRTraceParameters();
            bool NeedAddToOldResult = false;
            if (lastResultParameters.FSemples != null)
            {
                NeedAddToOldResult = ((lastResultParameters.FSemples.Length == CurLevels_dBm.Length) && (lastResultParameters.NN > 0));
            }
            // формирование Fsemples 
            fSemples = new SemplFreq[CurLevels_dBm.Length];
            for (int i = 0; i < CurLevels_dBm.Length; i++)
            {
                fSemples[i] = new SemplFreq();
                fSemples[i].LeveldBm = CurLevels_dBm[i];
                if (NeedAddToOldResult)
                {
                    if (CurLevels_dBm[i] > lastResultParameters.FSemples[i].LevelMaxdBm) { fSemples[i].LevelMaxdBm = CurLevels_dBm[i]; } else { fSemples[i].LevelMaxdBm = lastResultParameters.FSemples[i].LevelMaxdBm; }
                    if (CurLevels_dBm[i] < lastResultParameters.FSemples[i].LevelMindBm) { fSemples[i].LevelMaxdBm = CurLevels_dBm[i]; } else { fSemples[i].LevelMindBm = lastResultParameters.FSemples[i].LevelMindBm; }
                }
                else
                {
                    fSemples[i].LevelMaxdBm = CurLevels_dBm[i];
                    fSemples[i].LevelMindBm = CurLevels_dBm[i];
                }
                fSemples[i].Freq = (float)((TraceParameters.StartFreq_Hz + TraceParameters.StepFreq_Hz * i) / 1000000.0);
            }
            CalcFSFromLevel Calc = new CalcFSFromLevel(fSemples, sensor);
        }
        /// <summary>
        ///  Функция измерения изменяет уровень online результат будет в  Frequencies_MHz и Levels_dBm также может использоваться где не нужна первоначальная обработка измерений
        /// </summary>
        /// <param name="SDR"></param>
        /// <param name="lastResultParameters"></param>
        private void GetTraceOnLine(ISDR SDR, LastResultParameters lastResultParameters)
        {
            Levels_dBm = SDR.GetTrace(); // получили результат
            if (Levels_dBm == null) { return; }
            if (lastResultParameters == null)
            {
                var TraceParameters = SDR.GetSDRTraceParameters();
                Frequencies_MHz = new float[Levels_dBm.Length];
                for (int i = 0; i < Levels_dBm.Length; i++)
                {
                    Frequencies_MHz[i] = (float)((TraceParameters.StartFreq_Hz + TraceParameters.StepFreq_Hz * i) / 1000000.0);
                }
            }
        }
    }
       
}
