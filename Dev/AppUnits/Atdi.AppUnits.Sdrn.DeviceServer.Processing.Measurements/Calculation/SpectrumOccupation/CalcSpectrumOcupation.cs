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
        public static void CalcSemplFreq(MesureTraceResult result, ref int start, ref SemplFreq semplFreq, double freqCH, double stepSO_kHz, SOType typeOfSO, double levelMinOccup_dBm, double realRBW_Hz)
        {
            int semplInFreq = 0; //количество замеров идущие в один канал 
            var freqChStart = 1000000 * freqCH - 500 * stepSO_kHz;
            var freqChStop = 1000000 * freqCH + 500 * stepSO_kHz;
            start = (int)Math.Floor((freqChStart - result.FrequencyStart_Hz) / result.FrequencyStep_Hz);
            for (var j = start; j < result.LevelMaxIndex + 1; j++) // цикл по замерам по канальчикам
            {
                var freq = result.FrequencyStart_Hz + j * result.FrequencyStep_Hz;
                if (freqChStop < freq) { break; }
                if ((freqChStart <= freq) && (freqChStop > freq)) // проверка на попадание в диапазон частот
                {
                    semplInFreq = semplInFreq + 1;
                    if (semplInFreq == 1)// заполняем первое попадание как есть
                    {
                        semplFreq.Freq = (float)freqCH;
                        semplFreq.LeveldBm = result.Level[j];
                        if (typeOfSO == SOType.FreqBandwidthOccupation) // частотная занятость
                        {
                            if (result.Level[j] > levelMinOccup_dBm + 10 * Math.Log10(realRBW_Hz / (stepSO_kHz * 1000)))
                            { semplFreq.OcupationPt = 100; }
                        }
                    }
                    else // накапливаем уровень синнала
                    {
                        semplFreq.LeveldBm = (float)(Math.Pow(10, semplFreq.LeveldBm / 10) + Math.Pow(10, result.Level[j] / 10));
                        semplFreq.LeveldBm = (float)(10 * Math.Log10(semplFreq.LeveldBm));
                        if (typeOfSO == SOType.FreqBandwidthOccupation) // частотная занятость //накапливаем
                        {
                            if (result.Level[j] > levelMinOccup_dBm + 10 * Math.Log10(realRBW_Hz / (stepSO_kHz * 1000)))
                            { semplFreq.OcupationPt = semplFreq.OcupationPt + 100; }
                        }
                    }
                }
            }
            if (typeOfSO == SOType.FreqBandwidthOccupation) { semplFreq.OcupationPt = semplFreq.OcupationPt / semplInFreq; }
            if (typeOfSO == SOType.FreqChannelOccupation) { if (semplFreq.LeveldBm > levelMinOccup_dBm) { semplFreq.OcupationPt = 100; } }
            semplFreq.LevelMaxdBm = semplFreq.LeveldBm;
            semplFreq.LevelMindBm = semplFreq.LeveldBm;
        }

        public static SpectrumOcupationResult Calc(MesureTraceResult result, TaskParameters taskParameters, SensorParameters sensorParameters = null, SpectrumOcupationResult lastResultParameters = null)
        {
            int MaxCountSpectrumOccupationArr = 100;
            //var spectrumOcupationResult = new SpectrumOcupationResult();
            if ((taskParameters.TypeOfSO == SOType.FreqBandwidthOccupation) || (taskParameters.TypeOfSO == SOType.FreqChannelOccupation))
            {
                // вот собственно само измерение
                var freqChRes = new SemplFreq[taskParameters.ListFreqCH.Count];
                // замер 
                // Вот и дополнили значениями SO и прочим теперь значения красивые по микроканальчикам
                // Вычисляем занятость для данного замера по каналам 
                var freqChResTemp = new SemplFreq[taskParameters.ListFreqCH.Count]; // здест будут храниться замеры приведенные к каналу
                int start = 0;
                double realRBW_Hz = result.FrequencyStep_Hz;//.Freq_Hz[1] - result.Freq_Hz[0]; //Вставить проверку на наличие result.Freq_Hz[1] - result.Freq_Hz[0] если отсутвует выходить тиз функции с ошибкой что принятый результат не верен
                for (var i = 0; i < taskParameters.ListFreqCH.Count; i++) // Цикл по каналам
                {
                    var F_SO = new SemplFreq(); // здесь будет храниться один замер приведенный к каналу
                    CalcSemplFreq(result, ref start, ref F_SO, taskParameters.ListFreqCH[i], taskParameters.StepSO_kHz, taskParameters.TypeOfSO, taskParameters.LevelMinOccup_dBm, realRBW_Hz);
                    freqChResTemp[i] = F_SO; // добавляем во временный масив данные.
                }
                // данные единичного замера приведенного к каналам находятся здесь F_ch_res_temp    
                // Собираем статистику  в F_ch_res
                int NN = 0;
                if (lastResultParameters != null)
                {
                    if (lastResultParameters.NN == 0)
                    {
                        freqChRes = freqChResTemp;
                    }
                    else
                    {
                        for (var i = 0; i < lastResultParameters.fSemplesResult.Length; i++)
                        {
                            SemplFreq Semple = new SemplFreq();
                            Semple.Freq = lastResultParameters.fSemplesResult[i].Freq;
                            Semple.LevelMinArr = lastResultParameters.fSemplesResult[i].LevelMinArr;
                            Semple.SpectrumOccupationArr = lastResultParameters.fSemplesResult[i].SpectrumOccupationArr;

                            Semple.LeveldBm = (float)(10 * Math.Log10(((lastResultParameters.NN * Math.Pow(10, lastResultParameters.fSemplesResult[i].LeveldBm / 10) + Math.Pow(10, freqChResTemp[i].LeveldBm / 10)) / (lastResultParameters.NN + 1)))); // изменение 19.01.2018 Максим
                            Semple.OcupationPt = ((lastResultParameters.NN * lastResultParameters.fSemplesResult[i].OcupationPt + freqChResTemp[i].OcupationPt) / ((lastResultParameters.NN + 1)));
                            if (lastResultParameters.fSemplesResult[i].LevelMaxdBm < freqChResTemp[i].LevelMaxdBm) { Semple.LevelMaxdBm = freqChResTemp[i].LevelMaxdBm; } else { Semple.LevelMaxdBm = lastResultParameters.fSemplesResult[i].LevelMaxdBm; }
                            if (lastResultParameters.fSemplesResult[i].LevelMindBm > freqChResTemp[i].LevelMindBm) { Semple.LevelMindBm = freqChResTemp[i].LevelMindBm; } else { Semple.LevelMindBm = lastResultParameters.fSemplesResult[i].LevelMindBm; }
                            freqChRes[i] = Semple;
                        }
                        NN = lastResultParameters.NN;
                    }
                }
                else
                {
                    if (taskParameters.SupportMultyLevel)
                    {
                        float levelMinArr = -150 + (float)(10 * Math.Log10(1.38 * 3 * taskParameters.StepSO_kHz));
                        for (int k = 0; k < freqChResTemp.Length; k++)
                        {
                            var oldOcupationPt = freqChResTemp[k].OcupationPt;
                            freqChResTemp[k].LevelMinArr = levelMinArr;
                            freqChResTemp[k].SpectrumOccupationArr = new float[MaxCountSpectrumOccupationArr];
                            for (var j = 0; j < MaxCountSpectrumOccupationArr; j++)
                            {
                                CalcSemplFreq(result, ref start, ref freqChResTemp[k], taskParameters.ListFreqCH[k], taskParameters.StepSO_kHz, taskParameters.TypeOfSO, levelMinArr + j, realRBW_Hz);
                                freqChResTemp[k].SpectrumOccupationArr[j] = freqChResTemp[k].OcupationPt;
                            }
                            freqChResTemp[k].OcupationPt = oldOcupationPt;
                        }
                    }
                    freqChRes = freqChResTemp;
                }
                // в данной точке результат находится в переменой F_ch_res и в count мы его должны показать/запомнить.  
                // кстати это происходит у нас циклически
                lastResultParameters = new SpectrumOcupationResult();
                var Calc1 = new CalcFSFromLevel(freqChResTemp, sensorParameters);
                lastResultParameters.fSemplesResult = freqChRes;
                lastResultParameters.NN = NN + 1;

            }
            return lastResultParameters;
        }



    }
}
