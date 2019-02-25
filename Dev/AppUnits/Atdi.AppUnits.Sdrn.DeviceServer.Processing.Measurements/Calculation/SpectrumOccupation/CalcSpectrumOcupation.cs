using Atdi.Contracts.Sdrn.DeviceServer;
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
        public static SpectrumOcupation Calc(MesureTraceResult result, TaskParameters taskParameters, SensorParameters sensorParameters = null, LastResultParameters lastResultParameters = null)
        {
            var spectrumOcupationResult = new SpectrumOcupation();
            if ((taskParameters.Type_of_SO == SOType.FreqBandwidthOccupation) || (taskParameters.Type_of_SO == SOType.FreqChannelOccupation))
            {
                // вот собственно само измерение
                SemplFreq[] F_ch_res_ = new SemplFreq[taskParameters.List_freq_CH.Count];
                // замер 
                              
                // Вот и дополнили значениями SO и прочим теперь значения красивые по микроканальчикам
                // Вычисляем занятость для данного замера по каналам 
                SemplFreq[] F_ch_res_temp = new SemplFreq[taskParameters.List_freq_CH.Count]; // здест будут храниться замеры приведенные к каналу
                int start = 0;
                for (int i = 0; i < taskParameters.List_freq_CH.Count; i++) // Цикл по каналам
                {
                    SemplFreq F_SO = new SemplFreq(); // здесь будет храниться один замер приведенный к каналу
                    int sempl_in_freq = 0; //количество замеров идущие в один канал 
                    for (int j = start; j < result.Level.Length; j++) // цикл по замерам по канальчикам
                    {
                        if ( 1000000 * taskParameters.List_freq_CH[i] +  500 * taskParameters.StepSO_kHz < result.Freq_Hz[j]) { start = j; break; }
                        if ((1000000 * taskParameters.List_freq_CH[i] - 500 * taskParameters.StepSO_kHz <= result.Freq_Hz[j]) && (1000000 * taskParameters.List_freq_CH[i] + 500 * taskParameters.StepSO_kHz  > result.Freq_Hz[j])) // проверка на попадание в диапазон частот
                        {
                            sempl_in_freq = sempl_in_freq + 1;
                            if (sempl_in_freq == 1)// заполняем первое попадание как есть
                            {
                                F_SO.Freq = (float)taskParameters.List_freq_CH[i];
                                F_SO.LeveldBm = result.Level[j];
                                if (taskParameters.Type_of_SO == SOType.FreqBandwidthOccupation) // частотная занятость
                                {
                                    if (result.Level[j] > taskParameters.LevelMinOccup_dBm + 10 * Math.Log10(taskParameters.RBW_Hz / (taskParameters.StepSO_kHz * 1000)))
                                    { F_SO.OcupationPt = 100; }
                                }
                            }
                            else // накапливаем уровень синнала
                            {
                                F_SO.LeveldBm = (float)(Math.Pow(10, F_SO.LeveldBm / 10) + Math.Pow(10, result.Level[j] / 10));
                                F_SO.LeveldBm = (float)(10 * Math.Log10(F_SO.LeveldBm));
                                if (taskParameters.Type_of_SO == SOType.FreqBandwidthOccupation) // частотная занятость //накапливаем
                                {
                                    if (result.Level[j] > taskParameters.LevelMinOccup_dBm + 10 * Math.Log10(taskParameters.RBW_Hz / (taskParameters.StepSO_kHz * 1000)))
                                    { F_SO.OcupationPt = F_SO.OcupationPt + 100; }
                                }
                            }
                        }
                    }
                    if (taskParameters.Type_of_SO == SOType.FreqBandwidthOccupation) { F_SO.OcupationPt = F_SO.OcupationPt / sempl_in_freq; }
                    if (taskParameters.Type_of_SO == SOType.FreqChannelOccupation) { if (F_SO.LeveldBm > taskParameters.LevelMinOccup_dBm) { F_SO.OcupationPt = 100; } }
                    F_SO.LevelMaxdBm = F_SO.LeveldBm;
                    F_SO.LevelMindBm = F_SO.LeveldBm;
                    //F_SO на данный момент готов
                    F_ch_res_temp[i] = F_SO; // добавляем во временный масив данные.
                }
                // данные единичного замера приведенного к каналам находятся здесь F_ch_res_temp    
                // Собираем статистику  в F_ch_res
                if (lastResultParameters.NN == 0)
                {
                    F_ch_res_ = F_ch_res_temp;
                }
                else
                {
                    for (int i = 0; i < lastResultParameters.FSemples.Length; i++)
                    {
                        SemplFreq Semple = new SemplFreq();
                        Semple.Freq = lastResultParameters.FSemples[i].Freq;
                        Semple.LeveldBm = (float)(10 * Math.Log10(((lastResultParameters.NN * Math.Pow(10, lastResultParameters.FSemples[i].LeveldBm / 10) + Math.Pow(10, F_ch_res_temp[i].LeveldBm / 10)) / (lastResultParameters.NN + 1)))); // изменение 19.01.2018 Максим
                        Semple.OcupationPt = ((lastResultParameters.NN * lastResultParameters.FSemples[i].OcupationPt + F_ch_res_temp[i].OcupationPt) / ((lastResultParameters.NN + 1)));
                        if (lastResultParameters.FSemples[i].LevelMaxdBm < F_ch_res_temp[i].LevelMaxdBm) { Semple.LevelMaxdBm = F_ch_res_temp[i].LevelMaxdBm; } else { Semple.LevelMaxdBm = lastResultParameters.FSemples[i].LevelMaxdBm; }
                        if (lastResultParameters.FSemples[i].LevelMindBm > F_ch_res_temp[i].LevelMindBm) { Semple.LevelMindBm = F_ch_res_temp[i].LevelMindBm; } else { Semple.LevelMindBm = lastResultParameters.FSemples[i].LevelMindBm; }
                        F_ch_res_[i] = Semple;
                    }
                }
                // в данной точке результат находится в переменой F_ch_res и в count мы его должны показать/запомнить.  
                // кстати это происходит у нас циклически
                CalcFSFromLevel Calc = new CalcFSFromLevel(F_ch_res_, sensorParameters);
                spectrumOcupationResult.fSemplesResult = F_ch_res_;
                spectrumOcupationResult.NN = lastResultParameters.NN + 1; // костыль пока признак 0 
            }
            return spectrumOcupationResult;
        }

      

    }
}
