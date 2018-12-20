using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.MonitoringProcess.ProcessSignal;
//using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.Modules.MonitoringProcess.Measurement
{
    public class Signaling
    {
        public Emitting[] emittingsComplete;
        public Signaling(ISDR SDR, TaskParameters taskParameters, SensorParameters sensorParameters, LastResultParameters lastResultParameters, ref CirculatingData circulatingData, ReferenceSignal[] referenceSignals)
        {
            // Константы
            double triggerLevel_dBm_Hz = -130;// костыль            
            //double maxDeviationBWSignal = 0.1;
            // Конец константам

            if (circulatingData == null)
            {// т.е. еще ничего не измерялось
                //circulatingData = new CirculatingData();
                circulatingData.TaskId = taskParameters.TaskId;
                SDRTraceParameters sDRTraceParameters = SDR.GetSDRTraceParameters();
                circulatingData.referenceLevels = new ReferenceLevels(referenceSignals, sDRTraceParameters, triggerLevel_dBm_Hz, sensorParameters);
            }
            // получение потока данных
            SDRTraceParameters traceParameters = SDR.GetSDRTraceParameters();
                        Trace trace = new Trace(SDR, taskParameters, null, lastResultParameters);
            CompareTrace(ref circulatingData.emittings, circulatingData.referenceLevels.levels, trace.Levels_dBm, traceParameters.StepFreq_Hz/1000.0, traceParameters.StartFreq_Hz/1000000.0);

        }
        private bool CompareTrace(ref Emitting[] emitting, float[] refLevels, float[] levels, double stepBW_kHz, double startFreq_MHz)
        { // результат работы обновление данных в emitting
            if (refLevels.Length != levels.Length){return false;}
            bool excess = false; int startSignalIndex =0;

            if (levels[0] > refLevels[0]) { excess = true; startSignalIndex = 0; }
            // выделение мест где произошло превышение порога 
            List<int> index_start_stop = new List<int>();
            for (int i = 0; i < refLevels.Length; i++)
            {
                if (levels[i] > refLevels[i])
                { //Превышение
                    if (!excess)
                    {//начало превышения
                        startSignalIndex = i;
                        excess = true;
                    }
                }
                else
                { // Не превышение
                    if (excess)
                    { // Конец превышения
                        index_start_stop.Add(startSignalIndex);
                        index_start_stop.Add(i);
                        excess = false;
                    }
                }
            }
            if (excess)
            { // Конец превышения
                index_start_stop.Add(startSignalIndex);
                index_start_stop.Add(refLevels.Length-1);
                excess = false;
            }
            // выделение произошло. 
            Emitting[] newEmittings = Createemittings(levels, refLevels,index_start_stop, stepBW_kHz, startFreq_MHz);
            // сформировали новые параметры излучения теперь надо накатить старые по идее.
            emitting = EmittingUpdate(emitting, newEmittings, stepBW_kHz);
            ImpositionCompleteEmitting(ref emitting);
            return true;
        }
        private Emitting[] Createemittings(float[] levels, float[] refLevel, List<int> index_start_stop, double stepBW_kHz, double startFreq_MHz)
        { // задача локализовать излучения
            // константы 
            double DiffLevelForCalcBW = 25;
            double windowBW = 2;

            //
            List<Emitting> emittings = new List<Emitting>();
            for (int i = 0; i < index_start_stop.Count; i = i + 2)
            {
                // для каждого излучения вычислим BW 
                int start = index_start_stop[i]; int stop = index_start_stop[i + 1];
                int start_ = start; int stop_ = stop;
                double winBW = (stop - start) * windowBW;
                if ((start + stop - winBW) / 2 > 0) { start_ = (int)((start + stop - winBW) / 2.0); } else { start_ = 0; }
                if ((start + stop + winBW) / 2 < levels.Length - 1) { stop_ = (int)((start + stop + winBW) / 2.0); } else {stop_ = levels.Length - 1; }

                Emitting emitting = new Emitting();
                double [] templevel = new double [stop_ - start_];
                Array.Copy(levels, start_, templevel, 0, stop_ - start_);
                MeasBandwidthResult measSdrBandwidthResults = BandwidthEstimation.GetBandwidthPoint(templevel, BandwidthEstimation.BandwidthEstimationType.xFromCentr, DiffLevelForCalcBW, 0);
                if (measSdrBandwidthResults.СorrectnessEstimations != null)
                {
                    if (measSdrBandwidthResults.СorrectnessEstimations.Value)
                    {
                        if (measSdrBandwidthResults.T1 != null) { start = start+ measSdrBandwidthResults.T1.Value; }
                        if (measSdrBandwidthResults.T2 != null) { stop = start + measSdrBandwidthResults.T2.Value; }
                    }
                }
                emitting.StartFrequency_MHz = startFreq_MHz + stepBW_kHz * start/1000.0; 
                emitting.StopFrequency_MHz = startFreq_MHz + stepBW_kHz * stop / 1000.0; 
                emitting.ReferenceLevel_dBm = 0;
                emitting.AvaragePower_dBm = 0;
                for (int j = start; j < stop; j++)
                {
                    emitting.ReferenceLevel_dBm = emitting.ReferenceLevel_dBm + Math.Pow(10, refLevel[j] / 10);
                    emitting.AvaragePower_dBm = emitting.AvaragePower_dBm + Math.Pow(10, levels[j] / 10);
                }
                emitting.ReferenceLevel_dBm = 10 * Math.Log10(emitting.ReferenceLevel_dBm);
                emitting.AvaragePower_dBm = 10 * Math.Log10(emitting.AvaragePower_dBm);
                emittings.Add (emitting);
            }
            return emittings.ToArray();
        }
        private Emitting[] EmittingUpdate(Emitting[] emitting, Emitting[] newEmitting, double step_kHz)
        {
            // константа 
            TimeSpan timeSpanTrigger = TimeSpan.FromMilliseconds(100); //Время реакции 100 милисекунд
            double persentFluctationSignalByStep = 30; 

            DateTime currentTime = DateTime.Now;
            List<Emitting> resEmittings = new List<Emitting>();
            int emmitingCount = 0; int newEmmitingCount = 0;
            if (emitting == null)
            {
                for (int i = newEmmitingCount; i < newEmitting.Length; i++)
                {// добавляем
                    newEmitting[i].StartEmitting = currentTime;
                    newEmitting[i].Count = 1;
                    newEmitting[i].MaxPower_dBm = newEmitting[i].AvaragePower_dBm;
                    newEmitting[i].MinPower_dBm = newEmitting[i].AvaragePower_dBm;
                    resEmittings.Add(newEmitting[i]);
                }
            }
            else
            {
                if (newEmitting.Length == 0)
                {
                    for (int i = 0; i < emitting.Length; i++)
                    {
                        if (currentTime - emitting[i].StartEmitting > timeSpanTrigger)
                        {
                            //добавляем с завершением
                            emitting[i].StopEmitting = currentTime;
                            resEmittings.Add(emitting[i]);
                        }
                        else
                        {
                            // добавляем с продлением
                            resEmittings.Add(emitting[i]);
                        }
                    }
                }
                else
                {
                    if (emitting.Length == 0)
                    {
                        // все существующие уже есть добавляем новые
                        for (int i = 0; i < newEmitting.Length; i++)
                        {// добавляем
                            newEmitting[i].StartEmitting = currentTime;
                            newEmitting[i].Count = 1;
                            newEmitting[i].MaxPower_dBm = newEmitting[i].AvaragePower_dBm;
                            newEmitting[i].MinPower_dBm = newEmitting[i].AvaragePower_dBm;
                            resEmittings.Add(newEmitting[i]);
                        }
                    }
                    else
                    {
                        do
                        {

                            // определяем пересечение 
                            if ((emitting[emmitingCount].StartFrequency_MHz > newEmitting[newEmmitingCount].StopFrequency_MHz) || (emitting[emmitingCount].StopFrequency_MHz < newEmitting[newEmmitingCount].StartFrequency_MHz))
                            {// пересечения нет
                             // наращиваем счетчик
                                if (emitting[emmitingCount].StartFrequency_MHz > newEmitting[newEmmitingCount].StartFrequency_MHz)
                                { // новое не актуально для анализа поэтому его записываем

                                    newEmitting[newEmmitingCount].StartEmitting = currentTime;
                                    newEmitting[newEmmitingCount].Count = 1;
                                    newEmitting[newEmmitingCount].MaxPower_dBm = newEmitting[newEmmitingCount].AvaragePower_dBm;
                                    newEmitting[newEmmitingCount].MinPower_dBm = newEmitting[newEmmitingCount].AvaragePower_dBm;
                                    resEmittings.Add(newEmitting[newEmmitingCount]);

                                    // проверка на последнее значение
                                    newEmmitingCount = newEmmitingCount + 1;
                                    if (newEmmitingCount >= newEmitting.Length)
                                    {
                                        // все новые расмотренны обновляем старые 
                                        for (int i = emmitingCount; i < emitting.Length; i++)
                                        {
                                            if (currentTime - emitting[i].StartEmitting > timeSpanTrigger)
                                            {
                                                //добавляем с завершением
                                                emitting[i].StopEmitting = currentTime;
                                                resEmittings.Add(emitting[i]);
                                            }
                                            else
                                            {
                                                // добавляем с продлением
                                                resEmittings.Add(emitting[i]);
                                            }
                                        }
                                        break;
                                    }
                                }
                                else
                                { // старое уже не актуальное делаем обновление
                                    if (currentTime - emitting[emmitingCount].StartEmitting > timeSpanTrigger)
                                    {
                                        //добавляем с завершением
                                        emitting[emmitingCount].StopEmitting = currentTime;
                                        resEmittings.Add(emitting[emmitingCount]);
                                    }
                                    else
                                    {
                                        // добавляем с продлением
                                        resEmittings.Add(emitting[emmitingCount]);
                                    }
                                    // проверка на последнее значение
                                    emmitingCount = emmitingCount + 1;
                                    if (emmitingCount >= emitting.Length)
                                    {
                                        // все существующие уже есть добавляем новые
                                        for (int i = newEmmitingCount; i < newEmitting.Length; i++)
                                        {// добавляем
                                            newEmitting[i].StartEmitting = currentTime;
                                            newEmitting[i].Count = 1;
                                            newEmitting[i].MaxPower_dBm = newEmitting[i].AvaragePower_dBm;
                                            newEmitting[i].MinPower_dBm = newEmitting[i].AvaragePower_dBm;
                                            resEmittings.Add(newEmitting[i]);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {// пересечение есть
                                double div = (emitting[emmitingCount].StopFrequency_MHz - emitting[emmitingCount].StartFrequency_MHz) / (newEmitting[newEmmitingCount].StopFrequency_MHz - newEmitting[newEmmitingCount].StartFrequency_MHz);
                                if (((div * 100 > 100 - persentFluctationSignalByStep) && (div * 100 < 100 + persentFluctationSignalByStep)) || ((emitting[emmitingCount].StopFrequency_MHz - emitting[emmitingCount].StartFrequency_MHz) / (Math.Abs(newEmitting[newEmmitingCount].StopFrequency_MHz - newEmitting[newEmmitingCount].StartFrequency_MHz - newEmitting[newEmmitingCount].StopFrequency_MHz + newEmitting[newEmmitingCount].StartFrequency_MHz)) <= step_kHz / 1000))
                                {// совпадение сигналов
                                    newEmitting[newEmmitingCount].StartEmitting = emitting[emmitingCount].StartEmitting;
                                    newEmitting[newEmmitingCount].Count = emitting[emmitingCount].Count + 1;
                                    newEmitting[newEmmitingCount].MaxPower_dBm = Math.Max(newEmitting[newEmmitingCount].AvaragePower_dBm, emitting[emmitingCount].MaxPower_dBm);
                                    newEmitting[newEmmitingCount].MinPower_dBm = Math.Min(newEmitting[newEmmitingCount].AvaragePower_dBm, emitting[emmitingCount].MinPower_dBm);
                                    newEmitting[newEmmitingCount].AvaragePower_dBm = (Math.Pow(10, newEmitting[newEmmitingCount].AvaragePower_dBm / 10) + Math.Pow(10, emitting[emmitingCount].AvaragePower_dBm / 10) * emitting[emmitingCount].Count) / (emitting[emmitingCount].Count + 1);
                                    newEmitting[newEmmitingCount].AvaragePower_dBm = 10 * Math.Log10(newEmitting[newEmmitingCount].AvaragePower_dBm);
                                    resEmittings.Add(newEmitting[newEmmitingCount]);
                                }
                                else
                                { //несовпадение сигналов
                                  // новое уже не актуально для анализа поэтому его записываем
                                    newEmitting[newEmmitingCount].StartEmitting = currentTime;
                                    newEmitting[newEmmitingCount].Count = 1;
                                    newEmitting[newEmmitingCount].MaxPower_dBm = newEmitting[newEmmitingCount].AvaragePower_dBm;
                                    newEmitting[newEmmitingCount].MinPower_dBm = newEmitting[newEmmitingCount].AvaragePower_dBm;
                                    resEmittings.Add(newEmitting[newEmmitingCount]);
                                    //старое добавляем с завершением
                                    emitting[emmitingCount].StopEmitting = currentTime;
                                    resEmittings.Add(emitting[emmitingCount]);
                                }
                                // проверка на последнее значение
                                newEmmitingCount = newEmmitingCount + 1;
                                emmitingCount = emmitingCount + 1;
                                if (newEmmitingCount >= newEmitting.Length)
                                {
                                    // все новые расмотренны обновляем старые 
                                    for (int i = emmitingCount; i < emitting.Length; i++)
                                    {

                                        if (DateTime.Now - emitting[i].StartEmitting > timeSpanTrigger)
                                        {
                                            //добавляем с завершением
                                            emitting[i].StopEmitting = currentTime;
                                            resEmittings.Add(emitting[i]);
                                        }
                                        else
                                        {
                                            // добавляем с продлением
                                            resEmittings.Add(emitting[i]);
                                        }
                                    }
                                    break;
                                }
                                if (emmitingCount >= emitting.Length)
                                {
                                    // все существующие уже есть добавляем новые
                                    for (int i = newEmmitingCount; i < newEmitting.Length; i++)
                                    {// добавляем
                                        newEmitting[i].StartEmitting = currentTime;
                                        newEmitting[i].Count = 0;
                                        newEmitting[i].MaxPower_dBm = newEmitting[i].AvaragePower_dBm;
                                        newEmitting[i].MinPower_dBm = newEmitting[i].AvaragePower_dBm;
                                        resEmittings.Add(newEmitting[i]);
                                    }
                                    break;
                                }
                            }
                        } while ((newEmmitingCount < newEmitting.Length) || (emmitingCount < emitting.Length));
                    }
                }
            }
            return resEmittings.ToArray();
        }
        private void ImpositionCompleteEmitting(ref Emitting[] emitting)
        {
            List<Emitting> emittingsRes = new List<Emitting>();
            List<Emitting> emittingsList = emitting.ToList();
            for (int i = 0; i< emittingsList.Count; i++)
            {
                if (emittingsList[i].StopEmitting != DateTime.MinValue)
                {
                    emittingsRes.Add(emittingsList[i]);
                    emittingsList.RemoveAt(i);
                }
                //else if (emittingsList[i].Count == 1)
                //{
                //    emittingsRes.Add(emittingsList[i]);
                //}
            }
            emitting = emittingsList.ToArray();
            emittingsComplete = emittingsRes.ToArray();
        }
    }
}
