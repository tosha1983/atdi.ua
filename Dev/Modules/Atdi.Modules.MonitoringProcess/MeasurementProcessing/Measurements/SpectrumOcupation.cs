using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Modules.MonitoringProcess;

namespace Atdi.Modules.MonitoringProcess.Measurement
{
    public class SpectrumOcupation
    {
        public SemplFreq[] fSemplesResult; // результат со всеми отсчетами
        public int NN; // Количество вычислений
        public SpectrumOcupation(ISDR SDR, TaskParameters taskParameters, SensorParameters sensorParameters = null, LastResultParameters lastResultParameters = null)
        {
            if ((taskParameters.Type_of_SO == SOType.FreqBandwidthOccupation) || (taskParameters.Type_of_SO == SOType.FreqChannelOccupation))
            {
                // вот собственно само измерение
                SemplFreq[] F_ch_res_ = new SemplFreq[taskParameters.List_freq_CH.Count];
                // замер 
                Trace trace = new Trace(SDR, taskParameters, sensorParameters, lastResultParameters);

                // замер выполнен он находится в F_semple  trace.fSemples

                // дополняем замер значениями SO и прочим теперь значения красивые по микроканальчикам
                for (int i = 0; i < trace.fSemples.Length; i++)
                {
                    trace.fSemples[i].LevelMaxdBm = trace.fSemples[i].LeveldBm;
                    trace.fSemples[i].LevelMindBm = trace.fSemples[i].LeveldBm;
                    if (trace.fSemples[i].LeveldBm > taskParameters.LevelMinOccup_dBm) // проблема мы тут смотрим микроканалы или каналы если микроканалы то надо привести к данному значению порог
                    { trace.fSemples[i].OcupationPt = 100; }
                    else { trace.fSemples[i].OcupationPt = 0; }
                }
                // Вот и дополнили значениями SO и прочим теперь значения красивые по микроканальчикам
                // Вычисляем занятость для данного замера по каналам 
                SemplFreq[] F_ch_res_temp = new SemplFreq[taskParameters.List_freq_CH.Count]; // здест будут храниться замеры приведенные к каналу
                int start = 0;
                for (int i = 0; i < taskParameters.List_freq_CH.Count; i++) // Цикл по каналам
                {
                    SemplFreq F_SO = new SemplFreq(); // здесь будет храниться один замер приведенный к каналу
                    int sempl_in_freq = 0; //количество замеров идущие в один канал 
                    for (int j = start; j < trace.fSemples.Length; j++) // цикл по замерам по канальчикам
                    {
                        if (taskParameters.List_freq_CH[i] + taskParameters.StepSO_kHz / 2000 < trace.fSemples[j].Freq) { start = j; break; }
                        if ((taskParameters.List_freq_CH[i] - taskParameters.StepSO_kHz / 2000 <= trace.fSemples[j].Freq) && (taskParameters.List_freq_CH[i] + taskParameters.StepSO_kHz / 2000 > trace.fSemples[j].Freq)) // проверка на попадание в диапазон частот
                        {
                            sempl_in_freq = sempl_in_freq + 1;
                            if (sempl_in_freq == 1)// заполняем первое попадание как есть
                            {
                                F_SO.Freq = (float)taskParameters.List_freq_CH[i];
                                F_SO.LeveldBm = trace.fSemples[j].LeveldBm;
                                if (taskParameters.Type_of_SO == SOType.FreqBandwidthOccupation) // частотная занятость
                                {
                                    if (trace.fSemples[j].LeveldBm > taskParameters.LevelMinOccup_dBm + 10 * Math.Log10(taskParameters.RBW_Hz / (taskParameters.StepSO_kHz * 1000)))
                                    { F_SO.OcupationPt = 100; }
                                }
                            }
                            else // накапливаем уровень синнала
                            {
                                F_SO.LeveldBm = (float)(Math.Pow(10, F_SO.LeveldBm / 10) + Math.Pow(10, trace.fSemples[j].LeveldBm / 10));
                                F_SO.LeveldBm = (float)(10 * Math.Log10(F_SO.LeveldBm));
                                if (taskParameters.Type_of_SO == SOType.FreqBandwidthOccupation) // частотная занятость //накапливаем
                                {
                                    if (trace.fSemples[j].LeveldBm > taskParameters.LevelMinOccup_dBm + 10 * Math.Log10(taskParameters.RBW_Hz / (taskParameters.StepSO_kHz * 1000)))
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
                fSemplesResult = F_ch_res_;
                NN = lastResultParameters.NN + 1; // костыль пока признак 0 
            }
        }
    }
}
