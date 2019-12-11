﻿using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcSpectrumOcupation
    { 
        public static SpectrumOcupationResult Calc(MesureTraceResult result, TaskParameters taskParameters, SensorParameters sensorParameters = null, SpectrumOcupationResult lastResultParameters = null)
        {
            //var spectrumOcupationResult = new SpectrumOcupationResult();
            if ((taskParameters.TypeOfSO == SOType.FreqBandwidthOccupation) || (taskParameters.TypeOfSO == SOType.FreqChannelOccupation))
            {
                // вот собственно само измерение
                var F_ch_res_ = new SemplFreq[taskParameters.ListFreqCH.Count];
                // замер 

                // Вот и дополнили значениями SO и прочим теперь значения красивые по микроканальчикам
                // Вычисляем занятость для данного замера по каналам 
                var F_ch_res_temp = new SemplFreq[taskParameters.ListFreqCH.Count]; // здест будут храниться замеры приведенные к каналу
                int start = 0;

                double realRBW_Hz = result.FrequencyStep_Hz;//.Freq_Hz[1] - result.Freq_Hz[0]; //Вставить проверку на наличие result.Freq_Hz[1] - result.Freq_Hz[0] если отсутвует выходить тиз функции с ошибкой что принятый результат не верен

                for (var i = 0; i < taskParameters.ListFreqCH.Count; i++) // Цикл по каналам
                {
                    var F_SO = new SemplFreq(); // здесь будет храниться один замер приведенный к каналу
                    int sempl_in_freq = 0; //количество замеров идущие в один канал 
                    for (var j = start; j < result.Level.Length; j++) // цикл по замерам по канальчикам
                    {
                        if ( 1000000 * taskParameters.ListFreqCH[i] +  500 * taskParameters.StepSO_kHz < (result.FrequencyStart_Hz + j * result.FrequencyStep_Hz)) { start = j; break; }
                        if ((1000000 * taskParameters.ListFreqCH[i] - 500 * taskParameters.StepSO_kHz <= (result.FrequencyStart_Hz + j * result.FrequencyStep_Hz)) && (1000000 * taskParameters.ListFreqCH[i] + 500 * taskParameters.StepSO_kHz  > (result.FrequencyStart_Hz + j * result.FrequencyStep_Hz))) // проверка на попадание в диапазон частот
                        {
                            sempl_in_freq = sempl_in_freq + 1;
                            if (sempl_in_freq == 1)// заполняем первое попадание как есть
                            {
                                F_SO.Freq = (float)taskParameters.ListFreqCH[i];
                                F_SO.LeveldBm = result.Level[j];
                                if (taskParameters.TypeOfSO == SOType.FreqBandwidthOccupation) // частотная занятость
                                {
                                    if (result.Level[j] > taskParameters.LevelMinOccup_dBm + 10 * Math.Log10(realRBW_Hz / (taskParameters.StepSO_kHz * 1000)))
                                    { F_SO.OcupationPt = 100; }
                                }
                            }
                            else // накапливаем уровень синнала
                            {
                                F_SO.LeveldBm = (float)(Math.Pow(10, F_SO.LeveldBm / 10) + Math.Pow(10, result.Level[j] / 10));
                                F_SO.LeveldBm = (float)(10 * Math.Log10(F_SO.LeveldBm));
                                if (taskParameters.TypeOfSO == SOType.FreqBandwidthOccupation) // частотная занятость //накапливаем
                                {
                                    if (result.Level[j] > taskParameters.LevelMinOccup_dBm + 10 * Math.Log10(realRBW_Hz / (taskParameters.StepSO_kHz * 1000)))
                                    { F_SO.OcupationPt = F_SO.OcupationPt + 100; }
                                }
                            }
                        }
                    }
                    if (taskParameters.TypeOfSO == SOType.FreqBandwidthOccupation) { F_SO.OcupationPt = F_SO.OcupationPt / sempl_in_freq; }
                    if (taskParameters.TypeOfSO == SOType.FreqChannelOccupation) { if (F_SO.LeveldBm > taskParameters.LevelMinOccup_dBm) { F_SO.OcupationPt = 100; } }
                    F_SO.LevelMaxdBm = F_SO.LeveldBm;
                    F_SO.LevelMindBm = F_SO.LeveldBm;
                    //F_SO на данный момент готов
                    F_ch_res_temp[i] = F_SO; // добавляем во временный масив данные.
                }
                // данные единичного замера приведенного к каналам находятся здесь F_ch_res_temp    
                // Собираем статистику  в F_ch_res
                int NN = 0; 
                if (lastResultParameters != null)
                {
                    if (lastResultParameters.NN == 0)
                    {
                        F_ch_res_ = F_ch_res_temp;
                    }
                    else
                    {
                        for (var i = 0; i < lastResultParameters.fSemplesResult.Length; i++)
                        {
                            SemplFreq Semple = new SemplFreq();
                            Semple.Freq = lastResultParameters.fSemplesResult[i].Freq;
                            Semple.LeveldBm = (float)(10 * Math.Log10(((lastResultParameters.NN * Math.Pow(10, lastResultParameters.fSemplesResult[i].LeveldBm / 10) + Math.Pow(10, F_ch_res_temp[i].LeveldBm / 10)) / (lastResultParameters.NN + 1)))); // изменение 19.01.2018 Максим
                            Semple.OcupationPt = ((lastResultParameters.NN * lastResultParameters.fSemplesResult[i].OcupationPt + F_ch_res_temp[i].OcupationPt) / ((lastResultParameters.NN + 1)));
                            if (lastResultParameters.fSemplesResult[i].LevelMaxdBm < F_ch_res_temp[i].LevelMaxdBm) { Semple.LevelMaxdBm = F_ch_res_temp[i].LevelMaxdBm; } else { Semple.LevelMaxdBm = lastResultParameters.fSemplesResult[i].LevelMaxdBm; }
                            if (lastResultParameters.fSemplesResult[i].LevelMindBm > F_ch_res_temp[i].LevelMindBm) { Semple.LevelMindBm = F_ch_res_temp[i].LevelMindBm; } else { Semple.LevelMindBm = lastResultParameters.fSemplesResult[i].LevelMindBm; }
                            F_ch_res_[i] = Semple;
                        }
                        NN = lastResultParameters.NN;
                    }
                }
                else
                {
                    F_ch_res_ = F_ch_res_temp;
                }
                // в данной точке результат находится в переменой F_ch_res и в count мы его должны показать/запомнить.  
                // кстати это происходит у нас циклически
                lastResultParameters = new SpectrumOcupationResult();
                var Calc1 = new CalcFSFromLevel(F_ch_res_temp, sensorParameters);
                lastResultParameters.fSemplesResult = F_ch_res_;
                lastResultParameters.NN = NN+1;

            }
            return lastResultParameters;
        }

      

    }
}
