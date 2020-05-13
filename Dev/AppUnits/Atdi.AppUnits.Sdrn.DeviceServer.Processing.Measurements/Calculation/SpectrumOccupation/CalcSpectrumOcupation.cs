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
using Atdi.Common;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CalcSpectrumOcupation
    {
        /*
        public class LevelSpectrumArr
        {
            public int[] LevelMin { get; set; }
            public float[] OcuupationPt { get; set; }
        }

        public class LevelSpectrumIteration
        {
            public LevelSpectrumArr IterationOld { get; set; }
            public LevelSpectrumArr IterationNewWithCorrection { get; set; }
            public LevelSpectrumArr IterationNew { get; set; }
            public LevelSpectrumArr IterationSummary { get; set; }
        }
        */


        private static int MaxCountSpectrumOccupationArr = 100;
        /// <summary>
        /// Перерасчет значений OcupationPt для формирования массива SpectrumOccupationArr
        /// </summary>
        /// <param name="result"></param>
        /// <param name="taskParameters"></param>
        /// <param name="freqCH"></param>
        /// <param name="start"></param>
        /// <param name="realRBW_Hz"></param>
        /// <param name="tempFreqChResTemp"></param>
        /// <param name="levelMinArr"></param>
        public static void RecalcSpectrumOccupationForFirstIteration(MesureTraceResult result, TaskParameters taskParameters, double freqCH, ref int start, double realRBW_Hz, ref SemplFreq tempFreqChResTemp, float levelMinArr)
        {
            var idx = 0; var countLoop = 0;
            float tempLevelMinArr = levelMinArr;
            bool isFindOccup100 = false;

            // в данном цикле выполняется поиск значения levelMinArr, которое соответсвует OcupationPt равное 100 (последнее вхождение 100)
            for (int m = 0; m < (MaxCountSpectrumOccupationArr * 2); m++)
            {
                start = 0;
                var tempFreqChResTempCopy = CopyHelper.CreateDeepCopy(tempFreqChResTemp);
                // расчет OcupationPt c очередным значением tempLevelMinArr
                CalcSemplFreq(result, ref start, ref tempFreqChResTempCopy, freqCH, taskParameters.StepSO_kHz, taskParameters.TypeOfSO, tempLevelMinArr, realRBW_Hz);
                if (tempFreqChResTempCopy.OcupationPt != 100)
                {
                    // если OcupationPt меньше 100, то уменьшаем levelMinArr на 1 и выходим с цикла, запомнив значение (levelMinArr + idx)  
                    --idx;
                    tempLevelMinArr = levelMinArr + idx;
                    if (isFindOccup100)
                    {
                        break;
                    }
                }
                else
                {
                    // если OcupationPt равно 100, то увеличиваем levelMinArr на 1
                    ++idx;
                    tempLevelMinArr = levelMinArr + idx;
                    isFindOccup100 = true;
                }
                if (countLoop > MaxCountSpectrumOccupationArr * 2)
                {
                    break;
                }
                countLoop++;
            }

            // в данном цикле, выполняется вычисление значений OcupationPt начиная с levelMinArr, которое было получено на пред. шаге 
            var lstSpectrumOccupationRecalc = new List<float>();
            for (var j = 0; j < MaxCountSpectrumOccupationArr; j++)
            {
                start = 0;
                var tempFreqChResTemp2 = CopyHelper.CreateDeepCopy(tempFreqChResTemp);
                // расчет OcupationPt c очередным значением tempLevelMinArr + j
                CalcSemplFreq(result, ref start, ref tempFreqChResTemp2, freqCH, taskParameters.StepSO_kHz, taskParameters.TypeOfSO, tempLevelMinArr + j, realRBW_Hz);
                lstSpectrumOccupationRecalc.Add(tempFreqChResTemp2.OcupationPt);
                // если OcupationPt равно 0, то выход с цикла
                if (tempFreqChResTemp2.OcupationPt == 0)
                {
                    break;
                }
            }
            tempFreqChResTemp.LevelMinArr = (int)tempLevelMinArr;
            tempFreqChResTemp.SpectrumOccupationArr = lstSpectrumOccupationRecalc.ToArray();
        }

        public static void RecalcSpectrumOccupationForAnotherIteration(MesureTraceResult result, TaskParameters taskParameters, double freqCH, ref int start, double realRBW_Hz, ref SemplFreq tempFreqChResTemp, float levelMinArr)
        {
            var tempLevelMinArr = levelMinArr;
            var idx = 0;
            bool isFindOccup100 = false;
            float middleLevelMinArr = levelMinArr;
            // в данном цикле, выполняется вычисление значений OcupationPt начиная с levelMinArr, которое было получено на пред. шаге 
            var lstSpectrumOccupation = new List<float>();
            if (taskParameters.TypeOfSO == SOType.FreqChannelOccupation)
            {
                var tempFreqChannelOccupation = CopyHelper.CreateDeepCopy(tempFreqChResTemp);
                CalcSemplFreq(result, ref start, ref tempFreqChannelOccupation, freqCH, taskParameters.StepSO_kHz, taskParameters.TypeOfSO, middleLevelMinArr, realRBW_Hz);
                for (int i = 0; i < (MaxCountSpectrumOccupationArr * 2); i++)
                {
                    if (tempFreqChannelOccupation.LeveldBm > tempLevelMinArr)
                    {
                        tempFreqChannelOccupation.OcupationPt = 100;
                        lstSpectrumOccupation.Add(tempFreqChannelOccupation.OcupationPt);
                        break;
                    }
                    else
                    {
                        lstSpectrumOccupation.Add(tempFreqChannelOccupation.OcupationPt);
                    }
                    --idx;
                    tempLevelMinArr = middleLevelMinArr + idx;
                }
            }
            else if (taskParameters.TypeOfSO == SOType.FreqBandwidthOccupation)
            {
                for (int i = 0; i < (MaxCountSpectrumOccupationArr * 2); i++)
                {
                    start = 0;
                    // расчет OcupationPt c очередным значением middleLevelMinArr + idx
                    var tempFreqChResTempCopy = CopyHelper.CreateDeepCopy(tempFreqChResTemp);
                    CalcSemplFreq(result, ref start, ref tempFreqChResTempCopy, freqCH, taskParameters.StepSO_kHz, taskParameters.TypeOfSO, middleLevelMinArr + idx, realRBW_Hz);

                    lstSpectrumOccupation.Add(tempFreqChResTempCopy.OcupationPt);
                    // если OcupationPt равно 0, то выход с цикла
                    if ((tempFreqChResTempCopy.OcupationPt < 100) && (tempFreqChResTempCopy.OcupationPt > 0))
                    {
                        if (isFindOccup100 == false)
                        {
                            --idx;
                        }
                        else
                        {
                            ++idx;
                        }
                    }
                    else if (tempFreqChResTempCopy.OcupationPt == 100)
                    {
                        tempLevelMinArr = middleLevelMinArr + idx;
                        if (isFindOccup100 == false)
                        {
                            idx = 0;
                            isFindOccup100 = true;
                        }
                        ++idx;
                    }
                    else if (tempFreqChResTempCopy.OcupationPt == 0)
                    {
                        if (isFindOccup100 == true)
                        {
                            break;
                        }
                        else
                        {
                            --idx;
                        }
                    }
                    else
                    {
                        ++idx;
                    }

                    if (tempFreqChResTempCopy.OcupationPt == 0)
                    {
                        if (isFindOccup100 == true)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException($"For 'TypeOfSO' = '{taskParameters.TypeOfSO}', the behavior in the 'RecalcSpectrumOccupationForAnotherIteration' method is not implemented");
            }
            lstSpectrumOccupation.Sort();
            lstSpectrumOccupation.Reverse();
            tempFreqChResTemp.LevelMinArr = (int)tempLevelMinArr;
            tempFreqChResTemp.SpectrumOccupationArr = lstSpectrumOccupation.ToArray();
        }

        /// <summary>
        /// Вычисление OcupationPt
        /// </summary>
        /// <param name="result"></param>
        /// <param name="start"></param>
        /// <param name="semplFreq"></param>
        /// <param name="freqCH"></param>
        /// <param name="stepSO_kHz"></param>
        /// <param name="typeOfSO"></param>
        /// <param name="levelMinOccup_dBm"></param>
        /// <param name="realRBW_Hz"></param>
        public static void CalcSemplFreq(MesureTraceResult result, ref int start, ref SemplFreq semplFreq, double freqCH, double stepSO_kHz, SOType typeOfSO, double levelMinOccup_dBm, double realRBW_Hz)
        {
            int semplInFreq = 0; //количество замеров идущие в один канал 
            semplFreq.OcupationPt = 0; //обнуление значения SpectrumOccupation
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

        public static float GetSpectrumOccupationByLevelMin(int levelMinValue, int[] lvlMinArr, float[] spectrumOccupationArr)
        {
            float val = -1;
            float minValue = lvlMinArr.Min();
            float maxValue = lvlMinArr.Max();
            for (int k = 0; k < lvlMinArr.Length; k++)
            {
                if (lvlMinArr[k] == levelMinValue)
                {
                    val = spectrumOccupationArr[k];
                    break;
                }
                else if (minValue > levelMinValue)
                {
                    val = 100;
                    break;
                }
                else if (maxValue < levelMinValue)
                {
                    val = 0;
                    break;
                }
            }
            return val;
        }
        public static SpectrumOcupationResult Calc(MesureTraceResult result, TaskParameters taskParameters, SensorParameters sensorParameters = null, SpectrumOcupationResult lastResultParameters = null)
        {
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
                            //if (i != 30) continue;
                            var Semple = new SemplFreq();
                            Semple.Freq = lastResultParameters.fSemplesResult[i].Freq;
                            if (taskParameters.SupportMultyLevel)
                            {
                                var spectrumOccupationArr = lastResultParameters.fSemplesResult[i].SpectrumOccupationArr;
                                var tempFreqChResTemp = CopyHelper.CreateDeepCopy(lastResultParameters.fSemplesResult[i]);
                                var tempLevelMinArr = (int)((2 * lastResultParameters.fSemplesResult[i].LevelMinArr.Value + spectrumOccupationArr.Length) / 2);
                                RecalcSpectrumOccupationForAnotherIteration(result, taskParameters, taskParameters.ListFreqCH[i], ref start, realRBW_Hz, ref tempFreqChResTemp, tempLevelMinArr);
                                var spectrumOccupationArrNew = CopyHelper.CreateDeepCopy(tempFreqChResTemp.SpectrumOccupationArr);
                                var levelMinArrNew = (int)tempFreqChResTemp.LevelMinArr;
                                var tempspectrumOccupationArrNew = new List<float>();
                                var levelMinArrOld = (int)lastResultParameters.fSemplesResult[i].LevelMinArr;
                                var tempLvlMinArrOld = new int[spectrumOccupationArr.Length];
                                var tempSpectrumOccupationArrNew = new List<float>();

                                var masslevelMinArrNew = new int[tempFreqChResTemp.SpectrumOccupationArr.Length];
                                for (int k = 0; k < tempFreqChResTemp.SpectrumOccupationArr.Length; k++)
                                {
                                    masslevelMinArrNew[k] = levelMinArrNew + k;
                                }

                                for (int k = 0; k < spectrumOccupationArr.Length; k++)
                                {
                                    tempLvlMinArrOld[k] = levelMinArrOld + k;
                                }
                                // если levelMinArr для текущей итерации больше чем на предыдущей итерации, тогда добавляем недостающие значения 100
                                if (tempLvlMinArrOld[0] < masslevelMinArrNew[0])
                                {
                                    //присвоение levelMinArr с предыдущей итерации для использования в текущей итерации
                                    levelMinArrNew = tempLvlMinArrOld[0];
                                    var sub = Math.Abs(masslevelMinArrNew[0] - tempLvlMinArrOld[0]);
                                    for (int l = 0; l < sub; l++)
                                    {
                                        tempSpectrumOccupationArrNew.Add(100);
                                    }
                                }
                                tempSpectrumOccupationArrNew.AddRange(tempFreqChResTemp.SpectrumOccupationArr);
                                // если массив SpactrumOccupation для текущей итерации меньше по размеру чем на предыдущей итерации, тогда добавляем недостающие значения 0
                                if (tempLvlMinArrOld[tempLvlMinArrOld.Length - 1] > masslevelMinArrNew[masslevelMinArrNew.Length - 1])
                                {
                                    var sub = Math.Abs(tempLvlMinArrOld[tempLvlMinArrOld.Length - 1] - (masslevelMinArrNew[masslevelMinArrNew.Length - 1]));
                                    for (int l = 0; l < sub; l++)
                                    {
                                        tempSpectrumOccupationArrNew.Add(0);
                                    }
                                }
                                tempFreqChResTemp.SpectrumOccupationArr = tempSpectrumOccupationArrNew.ToArray();

                                int index = 0;
                                int levelMin100 = levelMinArrNew;
                                for (int m = 0; m < (MaxCountSpectrumOccupationArr * 2); m++)
                                {
                                    var valOccupOld = GetSpectrumOccupationByLevelMin(levelMinArrNew + index, tempLvlMinArrOld, spectrumOccupationArr);
                                    var valOccupNew = ((valOccupOld * lastResultParameters.NN + tempFreqChResTemp.SpectrumOccupationArr[index]) / (lastResultParameters.NN + 1));

                                    if (valOccupNew == 100)
                                    {
                                        levelMin100 = levelMinArrNew + index;
                                    }
                                    index++;

                                    tempspectrumOccupationArrNew.Add(valOccupNew);
                                    if ((valOccupNew == 0) || (tempFreqChResTemp.SpectrumOccupationArr.Length - 1 < index))
                                    {
                                        break;
                                    }
                                }

                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                ///
                                /*
                                var levelSpectrumIteration = new LevelSpectrumIteration();
                                levelSpectrumIteration.IterationNew = new LevelSpectrumArr();
                                levelSpectrumIteration.IterationNewWithCorrection = new LevelSpectrumArr();
                                levelSpectrumIteration.IterationOld = new LevelSpectrumArr();
                                levelSpectrumIteration.IterationSummary = new LevelSpectrumArr();

                                levelSpectrumIteration.IterationNew = new LevelSpectrumArr()
                                {
                                    LevelMin = masslevelMinArrNew,
                                    OcuupationPt = spectrumOccupationArrNew
                                };

                                levelSpectrumIteration.IterationOld = new LevelSpectrumArr()
                                {
                                    LevelMin = tempLvlMinArrOld,
                                    OcuupationPt = spectrumOccupationArr
                                };


                                var masslevelMinArrNewWithCorrection = new int[tempSpectrumOccupationArrNew.Count];
                                for (int k = 0; k < tempSpectrumOccupationArrNew.Count; k++)
                                {
                                    masslevelMinArrNewWithCorrection[k] = levelMinArrNew + k;
                                }

                                levelSpectrumIteration.IterationNewWithCorrection = new LevelSpectrumArr()
                                {
                                    LevelMin = masslevelMinArrNewWithCorrection,
                                    OcuupationPt = tempSpectrumOccupationArrNew.ToArray()
                                };

                                var masslevelMinArrSumm = new int[tempspectrumOccupationArrNew.Count];
                                for (int k = 0; k < tempspectrumOccupationArrNew.Count; k++)
                                {
                                    masslevelMinArrSumm[k] = levelMin100 + k;
                                }

                                levelSpectrumIteration.IterationSummary = new LevelSpectrumArr()
                                {
                                    LevelMin = masslevelMinArrSumm,
                                    OcuupationPt = tempspectrumOccupationArrNew.ToArray()
                                };

                                */
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                                freqChResTemp[i].SpectrumOccupationArr = tempspectrumOccupationArrNew.ToArray();
                                freqChResTemp[i].LevelMinArr = levelMin100;
                                lastResultParameters.fSemplesResult[i].LevelMinArr = freqChResTemp[i].LevelMinArr;
                                lastResultParameters.fSemplesResult[i].SpectrumOccupationArr = freqChResTemp[i].SpectrumOccupationArr;
                                Semple.LevelMinArr = lastResultParameters.fSemplesResult[i].LevelMinArr.Value;
                                Semple.SpectrumOccupationArr = lastResultParameters.fSemplesResult[i].SpectrumOccupationArr;
                            }
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
                            var tempFreqChResTemp = CopyHelper.CreateDeepCopy(freqChResTemp[k]);
                            RecalcSpectrumOccupationForFirstIteration(result, taskParameters, taskParameters.ListFreqCH[k], ref start, realRBW_Hz, ref tempFreqChResTemp, levelMinArr);
                            freqChResTemp[k].SpectrumOccupationArr = tempFreqChResTemp.SpectrumOccupationArr;
                            freqChResTemp[k].LevelMinArr = tempFreqChResTemp.LevelMinArr;
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
