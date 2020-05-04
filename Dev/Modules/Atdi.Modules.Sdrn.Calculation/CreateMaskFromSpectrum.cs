using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.DataModels.Sdrns.Device;

namespace Atdi.Modules.Sdrn.Calculation
{

    public struct EmittingSpectrum
    {
        public float[] Levels_dBm;
        public double StartFreq_MHz;
    }

    public struct SpectrumMask
    {
        public float[] MaskLevels_dB;
        public double[] MaskFrequencies_MHz;
        public double CentralFrequency_MHz;
        public double Power_dBm;
    }

    public static class CreateMaskFromSpectrum
    {
        
        //public static T[] SubArray<T>(this T[] data, int index, int length)
        //{
        //    T[] result = new T[length];
        //    Array.Copy(data, index, result, 0, length);
        //    return result;
        //}
        private static float MaxInSubArray(in float[] data, int index, int length)
        {
            try
            {
                float maxValue = data[index];
                for (int i = index + 1; i < index + length; i++)
                {
                    if (maxValue < data[i])
                    {
                        maxValue = data[i];
                    }
                }
                return maxValue;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Creates mask from smoothed Emitting spectrum
        /// </summary>
        /// <param name="spectrumLevels_dBm">Levels of Emitting spectum</param>
        /// <param name="spectrumStartFreq_MHz">Start frequency of Emitting spectrum</param>
        /// <param name="spectrumSteps_kHz">Step between frequencies of Emitting spectrum</param>
        /// <param name="maskMargin_dB">Level at which mask should exceed maximum power of emitting</param>
        /// <param name="initialCutoff_dB">Level at whish estimated in-block limit (recommended 6 .. 15 dB)</param>
        /// <param name="numberOfMask_pt">Numper of point in the mask. Should be even, from 6 to 18 points</param>
        /// <returns></returns>
        private static SpectrumMask CreateMaskFromEmitting(float[] spectrumLevels_dBm, double spectrumStartFreq_MHz, double spectrumSteps_kHz, float initialCutoff_dB, int numberOfMask_pt)
        {

            if (numberOfMask_pt % 2 != 0 || numberOfMask_pt < 6)
            {
                throw new InvalidOperationException("Spectrum mask should have at least 6 points. Numbet of points should be even");
            }

            SpectrumMask resultMask;
            
            int minBeforeMaxIndex = 0;
            int mediumBeforeCutoffIndex = 0;

            int cutoffBeforeMaxIndex = 0;
            int cutoffAfterMaxIndex = 0;

            int mediumAfterCutoffIndex = 0;
            int minAfterMaxIndex = 0;

            float minBeforeMax = 0.0f;
            float minAfterMax = 0.0f;

            float maxLevel_dBm = spectrumLevels_dBm.Max();
            //bool WasMax = false;
            bool WasFirstCutoff = false;
            bool WasLastCutoff = false;

            resultMask.MaskLevels_dB = new float[numberOfMask_pt];
            resultMask.MaskFrequencies_MHz = new double[numberOfMask_pt];
            //  Searching for intersection of cutoff level with emitting spectrum and spectrum minimums
            for (int i = 0; i < spectrumLevels_dBm.Length; i++)
            {
                if ((spectrumLevels_dBm[i] == maxLevel_dBm) || (WasFirstCutoff && WasLastCutoff))
                {
                    //WasMax = true;
                    break;
                }
                else
                {
                    if ((spectrumLevels_dBm[i] < minBeforeMax) && !WasFirstCutoff)
                    {
                        minBeforeMax = spectrumLevels_dBm[i];
                        minBeforeMaxIndex = i;
                    }
                    if ((spectrumLevels_dBm[i] > (maxLevel_dBm - initialCutoff_dB)) && !WasFirstCutoff )
                    {
                        cutoffBeforeMaxIndex = i;
                        WasFirstCutoff = true;
                    }

                    int j = spectrumLevels_dBm.Length - i - 1;

                    if (spectrumLevels_dBm[j] > (maxLevel_dBm - initialCutoff_dB) && !WasLastCutoff)
                    {
                        cutoffAfterMaxIndex = j;
                        WasLastCutoff = true;
                    }
                    if (spectrumLevels_dBm[j] < minAfterMax && !WasLastCutoff)
                    {
                        minAfterMax = spectrumLevels_dBm[j];
                        minAfterMaxIndex = j;
                    }
                }
            }

            // First 6 points of mask are found
            int mediumExtension = (Math.Max(cutoffBeforeMaxIndex, spectrumLevels_dBm.Length - cutoffAfterMaxIndex) / (numberOfMask_pt - 2));

            resultMask.CentralFrequency_MHz = spectrumStartFreq_MHz + spectrumSteps_kHz * minBeforeMaxIndex * 0.001 + (spectrumSteps_kHz * minAfterMaxIndex * 0.001 - spectrumSteps_kHz * minBeforeMaxIndex * 0.001) / 2;

            resultMask.MaskLevels_dB[0] = spectrumLevels_dBm[minBeforeMaxIndex];// + maskMargin_dB;
            resultMask.MaskFrequencies_MHz[0] = - resultMask.CentralFrequency_MHz + spectrumStartFreq_MHz + spectrumSteps_kHz * minBeforeMaxIndex * 0.001;
            resultMask.MaskLevels_dB[1] = MaxInSubArray(in spectrumLevels_dBm, minBeforeMaxIndex, mediumExtension);// + maskMargin_dB;
            resultMask.MaskFrequencies_MHz[1] = - resultMask.CentralFrequency_MHz + spectrumStartFreq_MHz + spectrumSteps_kHz * minBeforeMaxIndex * 0.001;

            //float cutoffLevel_dBm = Math.Max(spectrumLevels_dBm[cutoffBeforeMaxIndex], spectrumLevels_dBm[cutoffAfterMaxIndex]);
            resultMask.MaskLevels_dB[(numberOfMask_pt / 2) - 1] = maxLevel_dBm;// cutoffLevel_dBm + initialCutoff_dB;// + maskMargin_dB;
            resultMask.MaskFrequencies_MHz[(numberOfMask_pt / 2) - 1] = - resultMask.CentralFrequency_MHz + spectrumStartFreq_MHz + spectrumSteps_kHz * cutoffBeforeMaxIndex * 0.001;
            resultMask.MaskLevels_dB[(numberOfMask_pt / 2)] = maxLevel_dBm;// cutoffLevel_dBm + initialCutoff_dB;// + maskMargin_dB;
            resultMask.MaskFrequencies_MHz[(numberOfMask_pt / 2)] = - resultMask.CentralFrequency_MHz + spectrumStartFreq_MHz + spectrumSteps_kHz * cutoffAfterMaxIndex * 0.001;

            resultMask.MaskLevels_dB[numberOfMask_pt - 2] = MaxInSubArray(in spectrumLevels_dBm, minAfterMaxIndex - mediumExtension, mediumExtension);// + maskMargin_dB;
            resultMask.MaskFrequencies_MHz[numberOfMask_pt - 2] = - resultMask.CentralFrequency_MHz + spectrumStartFreq_MHz + spectrumSteps_kHz * minAfterMaxIndex * 0.001;

            resultMask.MaskLevels_dB[numberOfMask_pt - 1] = spectrumLevels_dBm[minAfterMaxIndex];//+ maskMargin_dB;
            resultMask.MaskFrequencies_MHz[numberOfMask_pt - 1] = - resultMask.CentralFrequency_MHz + spectrumStartFreq_MHz + spectrumSteps_kHz * minAfterMaxIndex * 0.001;

            // If mask should have more than 6 ppoints - transition band divided into equel parts and covers spectrum of emitting
            if (numberOfMask_pt >= 8)
            {
                for(int i = 2; i < (numberOfMask_pt / 2) - 1; i++)
                {
                    mediumBeforeCutoffIndex = (i - 1) * (cutoffBeforeMaxIndex - minBeforeMaxIndex) / (numberOfMask_pt / 2 - 2);
                    resultMask.MaskLevels_dB[i] = MaxInSubArray(in spectrumLevels_dBm, mediumBeforeCutoffIndex - mediumExtension, mediumExtension * 2);// + maskMargin_dB;
                    resultMask.MaskFrequencies_MHz[i] = - resultMask.CentralFrequency_MHz + spectrumStartFreq_MHz + spectrumSteps_kHz * mediumBeforeCutoffIndex * 0.001;

                    mediumAfterCutoffIndex = cutoffAfterMaxIndex + ((i - 1) * (minAfterMaxIndex - cutoffAfterMaxIndex) / (numberOfMask_pt / 2 - 2));
                    resultMask.MaskLevels_dB[i + numberOfMask_pt / 2 - 1] = MaxInSubArray(in spectrumLevels_dBm, mediumAfterCutoffIndex - mediumExtension, 2 * mediumExtension);// + maskMargin_dB;
                    resultMask.MaskFrequencies_MHz[i + numberOfMask_pt / 2 - 1] = - resultMask.CentralFrequency_MHz + spectrumStartFreq_MHz + spectrumSteps_kHz * mediumAfterCutoffIndex * 0.001;
                }
            }

            double power_W = 0;
            //mask power
            for (int i = 1; i < resultMask.MaskLevels_dB.Length - 1; i++)
            {
                if (i == resultMask.MaskLevels_dB.Length / 2)
                {
                    power_W += 2 * resultMask.MaskFrequencies_MHz[i] * Math.Pow(10, 0.1 * resultMask.MaskLevels_dB[i]) / spectrumSteps_kHz;
                }
                else if ((resultMask.MaskFrequencies_MHz[i] - resultMask.MaskFrequencies_MHz[i-1]) != 0)
                {
                    power_W += .5 * Math.Abs(resultMask.MaskFrequencies_MHz[i - 1] - resultMask.MaskFrequencies_MHz[i]) * (Math.Pow(10, 0.1 * resultMask.MaskLevels_dB[i]) + Math.Pow(10, 0.1 * resultMask.MaskLevels_dB[i - 1])) / spectrumSteps_kHz;
                }
            }

            resultMask.Power_dBm = 10 * Math.Log10(power_W / 0.001);

            return resultMask;

        }

        // Example
        /// <summary>
        /// Creates list of non overlapping masks
        /// </summary>
        /// <param name="emittingSpectrum">Array of emitting spectrums that include start frequencies and array of levels</param>
        /// <param name="spectrumSteps_kHz">Step between frequencies of Emitting spectrum</param>
        /// <param name="initialCutoff_dB">Level at whish estimated in-block limit (recommended 6 .. 15 dB)</param>
        /// <param name="numberOfMask_pt">Numper of point in the mask. Should be even, from 6 to 18 points</param>
        /// <returns></returns>
        public static List<SpectrumMask> CreateMasksFromEmittings(EmittingSpectrum[] emittingSpectrum, double spectrumSteps_kHz, float initialCutoff_dB, int numberOfMask_pt)
        {

            if (numberOfMask_pt % 2 != 0 || numberOfMask_pt < 6)
            {
                throw new InvalidOperationException("Spectrum mask should have at least 6 points. Numbet of points should be even");
            }

            initialCutoff_dB = Math.Abs(initialCutoff_dB);

            List <SpectrumMask> spectrumMasks = new List<SpectrumMask>();
            foreach (var emitting in emittingSpectrum)
            {
                var result = CreateMaskFromEmitting(emitting.Levels_dBm, emitting.StartFreq_MHz, spectrumSteps_kHz, initialCutoff_dB, numberOfMask_pt);
                spectrumMasks.Add(result);
            }
            // проверка на пересечение
            int spectrumMasksCount = spectrumMasks.Count;
            for (int i=0; i < spectrumMasksCount; i++)
            {
                for (int j = 0; j < spectrumMasksCount; j++)
                {
                    if (i != j)
                    {
                        var frqA0 = spectrumMasks[i].CentralFrequency_MHz + spectrumMasks[i].MaskFrequencies_MHz[0];
                        var frqAL = spectrumMasks[i].CentralFrequency_MHz - spectrumMasks[i].MaskFrequencies_MHz[0];
                        var lvlA = spectrumMasks[i].MaskLevels_dB[numberOfMask_pt / 2];
                        var frqB0 = spectrumMasks[j].CentralFrequency_MHz + spectrumMasks[j].MaskFrequencies_MHz[0];
                        var frqBL = spectrumMasks[j].CentralFrequency_MHz - spectrumMasks[j].MaskFrequencies_MHz[0];
                        var lvlB = spectrumMasks[j].MaskLevels_dB[numberOfMask_pt / 2];
                        if ((lvlA >= lvlB) && (frqA0 <= frqB0) && (frqAL >= frqBL))
                        {
                            spectrumMasks.RemoveRange(j, 1);
                            spectrumMasksCount--;
                            j--;
                        }
                    }
                   
                }
            }

            return spectrumMasks;
        }
        
    }
}
