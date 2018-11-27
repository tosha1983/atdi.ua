using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDR.Server.MeasurementProcessing.SingleHound
{
    /*
    class MeasurementProcessingSingleHoundBB60C
    {
        /// <summary>
        /// Start Meass 
        /// </summary>
        /// <param name="SDR">SDR - phisical object BC60C</param>
        /// <param name="TaskSDR">MeasSDRTask for measurements</param>
        /// <param name="sensor">Sensore with parameters</param>
        /// <param name="LastSDRRes">Last result, can be null</param>
        /// <returns>Result of Measurements</returns>
        public MeasSdrResults MeasurementProcessing (ref SDR_BB60C SDR, ref MeasSdrTask TaskSDR, Atdi.AppServer.Contracts.Sdrns.Sensor sensor, MeasSdrResults LastSDRRes = null)
        {
            MeasSdrResults MSDRRes = new MeasSdrResults();
                // инициализация и калибровка SDR если он выключен, настройка конфигураци измерения
                if (SDR.id_dev == 999)
                {
                    SDR.initiation_SDR(); if (SDR.status != bbStatus.bbNoError) { SDR.err = "Error of initialization"; SDR.Close_dev(); return MSDRRes; }
                    SDR.calibration(); if (SDR.status != bbStatus.bbNoError) { SDR.err = "Error of calibration"; SDR.Close_dev(); return MSDRRes; }
                    SDR.PutConfiguration(ref TaskSDR); if (SDR.status != bbStatus.bbNoError) { SDR.err = "Error of configuration"; SDR.Close_dev(); return MSDRRes; }
                }
                else if (SDR.IdLastMeasTask != TaskSDR.MeasTaskId.Value)
                {
                    SDR.PutConfiguration(ref TaskSDR); if (SDR.status != bbStatus.bbNoError) { SDR.err = "Error of configuration"; SDR.Close_dev(); return MSDRRes; }
                }
                DateTime Time_start = DateTime.Now; // замер времени измерения
                SDR.MEAS_SDR_RESULTS = new MeasSdrResults();
                SDR.F_semples = new List<FSemples>();
                SDR.process_meas_BB60C(ref TaskSDR, sensor, LastSDRRes);
                MSDRRes = SDR.MEAS_SDR_RESULTS;
                SDR.IdLastMeasTask = TaskSDR.MeasTaskId.Value;
                TimeSpan TimeMeasurements = DateTime.Now - Time_start;
                // здесь будет обновление таска
                UpdateMeasTaskAfterMeasurements(ref TaskSDR, TimeMeasurements);
                return MSDRRes;
        }
        /// <summary>
        /// Данный метод производит изменение статуса и изменение времени измерения. Это нужно для регулирования времени работы SDR
        /// </summary>
        /// <param name="TaskSDR"></param>
        /// <param name="TimeMeasurements"></param>
        private void UpdateMeasTaskAfterMeasurements(ref MeasSdrTask TaskSDR, TimeSpan TimeMeasurements)
        {
            if ((TaskSDR.status != "O") && (TaskSDR.status != "RT"))// для онлайн измерений не производим коректировку
            {
                if (TaskSDR.NumberScanPerTask == -999)
                { // Нужно задать количество измерений
                    TaskSDR.NumberScanPerTask = (int)(TaskSDR.PerInterval / TimeMeasurements.TotalSeconds) - 1; // задали 
                    if (TaskSDR.NumberScanPerTask > 0) // коректировка измерений
                    {
                        Double LostTimeMeas = TimeMeasurements.TotalMilliseconds * TaskSDR.NumberScanPerTask; // оставшиеся время измерения милисек
                        Double LostTime = (TaskSDR.Time_stop - DateTime.Now).TotalMilliseconds; // оставшееся время 
                        if (LostTime > LostTimeMeas)
                        {
                            Double Time = (LostTime - LostTimeMeas) / TaskSDR.NumberScanPerTask - TimeMeasurements.TotalMilliseconds; // время до следующего измерения
                            TaskSDR.Time_start = DateTime.Now - TimeSpan.FromMilliseconds(Time);
                        }
                    }
                }
                else
                {
                    if (TaskSDR.NumberScanPerTask <= 1)
                    {//Изменить статус надо все конец
                        TaskSDR.status = "C";  //это костыль должен быть запуск функции MeasTaskSDR.UpdateStatus() 
                    }
                    else
                    {// статус менять не надо но возможна коректировка скорости измерения
                        TaskSDR.NumberScanPerTask = TaskSDR.NumberScanPerTask - 1;
                        Double LostTimeMeas = TimeMeasurements.TotalMilliseconds * TaskSDR.NumberScanPerTask; // оставшиеся время измерения милисек
                        Double LostTime = (TaskSDR.Time_stop - DateTime.Now).TotalMilliseconds; // оставшееся время 
                        if (LostTime > LostTimeMeas)
                        {
                            Double Time = (LostTime - LostTimeMeas) / TaskSDR.NumberScanPerTask - TimeMeasurements.TotalMilliseconds; // время до следующего измерения
                            TaskSDR.Time_start = DateTime.Now + TimeSpan.FromMilliseconds(Time);
                        }
                    }
                }
            }
            else
            {
                TaskSDR.Time_start = DateTime.Now + TimeSpan.FromMilliseconds(20);
            }
        }
    }*/
}

