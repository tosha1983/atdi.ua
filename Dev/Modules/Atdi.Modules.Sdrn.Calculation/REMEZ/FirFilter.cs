using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Atdi.Modules.Sdrn.Calculation.REMEZ
{
    class FirFilter
    {
        /// <summary>
        /// Simple frequency sampling algorithm to determine the impulse response h[] from A's found in ComputeA
        /// </summary>
        /// <param name="h">Array of filter coefficients</param>
        /// <param name="numtaps">Number of filter coefficients</param>
        /// <param name="numband">Number of bands in filter specification</param>
        /// <param name="bands">User-specified band edges [2 * numband]</param>
        /// <param name="des">User-specified band responses [numband]</param>
        /// <param name="weight">User-specified error weights [numband]</param>
        /// <param name="type">Type of filter</param>
        [DllImport("DllApp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemezCall(double[] h, int numtaps, int numband, double[] bands, double[] des, double[] weight, int type);

        const byte OFF = 0;
        const byte LOWPASS = 1;
        const byte HIGHPASS = 2;
        const byte BANDSTOP = 3;// NOTCH
        const byte BANDPASS = 4;
        // array of filter coefficients 
        public double[] h;

        /// <summary>
        /// Initialization of parameters for Parks-McCellan algorythm
        /// </summary>
        /// <param name="type">Filter type, off = 0 lowpass = 1 highpass = 2 bandstop = 3 bandpass = 4</param>
        /// <param name="order">Filter order, if (order less than 1) - calculated</param>
        /// <param name="f1">Frequency in Hz, luwer cunoff frequency, only frequency parameter for lowpass and highpass fileters</param>
        /// <param name="f2">Frequency in Hz, upper cutoff frequency, should be lower than f1, valuable only for bandpass and bandstop filters</param
        /// <param name="trband">Transition band in Hz</param>
        /// <param name="sampleRate">Sampliing frequency in Hz</param>
        /// <param name="atten_dB">Filter attenuation in dB</param>
        /// <param name="ripple_dB">Ripple size in dB of impulse response in passband</param>
        public void Initialize(byte type, int order, float f1, float f2, float trband, float sampleRate, float atten_dB,
                float ripple_dB)
        {
            f1 /= sampleRate;
            f2 /= sampleRate;
            trband /= sampleRate;

            if (f1 > 0.5 || f2 > 0.5)
            {
                throw new InvalidOperationException("Higest frequency should be lower than half of sampling frequrncy (f1|f2 < fs / 2)");
            }
            if (f1 >= f2 & (type == 3 || type == 4))
            {
                throw new InvalidOperationException("Higest cutoff frequency should be lower than lower cutoff frequency for bandpass or bandstop filter");
            }

            int numBands = 2;

            float deltaP = (float)(0.5 * (1.0 - Math.Pow(10.0, -0.05 * ripple_dB)));
            float deltaS = (float)Math.Pow(10.0, -0.05 * atten_dB);
            float rippleRatio = deltaP / deltaS;

            // Estimation of filter order using Kaiser's formula
            if (order < 1)
            {
                order = (int)(Math.Round((-10.0 * Math.Log10(deltaP * deltaS) - 13) / (14.6 * trband)));
            }
            int numTaps = order + 1;

            if (type == BANDPASS || type == BANDSTOP)
                numBands = 3;

            double[] desired = new double[numBands];
            double[] bands = new double[numBands << 1];
            double[] weights = new double[numBands];
            switch (type)
            {
                case LOWPASS:
                    desired[0] = 1.0;
                    desired[1] = 0.0;
                    bands[0] = 0.0;
                    bands[1] = f1;
                    bands[2] = f1 + trband;
                    bands[3] = 0.5;
                    weights[0] = 1.0;
                    weights[1] = rippleRatio;
                    break;
                case HIGHPASS:
                    desired[0] = 0.0;
                    desired[1] = 1.0;
                    bands[0] = 0.0;
                    bands[1] = f1 - trband;
                    bands[2] = f1;
                    bands[3] = 0.5;
                    weights[0] = rippleRatio;
                    weights[1] = 1.0;
                    break;
                case BANDPASS:
                    desired[0] = 0.0;
                    desired[1] = 1.0;
                    desired[2] = 0.0;
                    bands[0] = 0.0;
                    bands[1] = f1 - trband;
                    bands[2] = f1;
                    bands[3] = f2;
                    bands[4] = f2 + trband;
                    bands[5] = 0.5;
                    weights[0] = rippleRatio;
                    weights[1] = 1.0;
                    weights[2] = rippleRatio;
                    break;
                case BANDSTOP:
                    desired[0] = 1.0;
                    desired[1] = 0.0;
                    desired[2] = 1.0;
                    bands[0] = 0.0;
                    bands[1] = f1 - trband;
                    bands[2] = f1;
                    bands[3] = f2;
                    bands[4] = f2 + trband;
                    bands[5] = 0.5;
                    weights[0] = 1;
                    weights[1] = rippleRatio;
                    weights[2] = 1;
                    break;
            }
            // BANDPASS is opposite to DIFFERENTIATOR or HILBERT !
            h = new double[numTaps];
            int numBand = desired.Length;

            RemezCall(h, numTaps, numBand, bands, desired, weights, 1);
        }
        /// <summary>
        /// Filtering of IQ stream
        /// </summary>
        /// <param name="iqInput">Array of input IQ stream values</param>
        /// <param name="iqFiltered">Array of filtered IQ stream values</param>
        /// <param name="filerType">type of filter, lowpass = 1 highpass = 2 bandstop = 3 bandpass = 4</param>
        /// <param name="order">Filter order, if (order less than 1) - calculated</param>
        /// <param name="f1">Frequency in Hz, luwer cunoff frequency, only frequency parameter for lowpass and highpass fileters</param>
        /// <param name="f2">Frequency in Hz, upper cutoff frequency, should be lower than f1, valuable only for bandpass and bandstop filters</param
        /// <param name="trband">Transition band in Hz</param>
        /// <param name="sampleRate">Sampliing frequency in Hz</param>
        /// <param name="atten_dB">Filter attenuation in dB</param>
        /// <param name="ripple_dB">Ripple size in dB of impulse response in passband</param>
        public void FfilteringIQ(double[] iqInput, double[] iqFiltered, byte filerType, int order, float f1, float f2, float trband, float sampleRate, float atten_dB,
                float ripple_dB)
        {
            Initialize(filerType, 0, f1, f2, trband, sampleRate, atten_dB, ripple_dB);

            int midTap = h.Length / 2;
            int streamLength = iqInput.Length / 2;
            int delayCompensation = streamLength + midTap;

            for (int n = midTap; n < delayCompensation; n++)
            {
                for (int i = 0; i < h.Length - 1; i++)
                {
                    if ((n - i) >= 0)
                    {
                        if ((n - i) <= streamLength - 1)
                        {
                            //iFiltered[n - midTap] += hS[i] * iStream[n - i];
                            //qFiltered[n - midTap] += hS[i] * qStream[n - i];
                            iqFiltered[2 * (n - midTap)] += h[i] * iqInput[2 * (n - i)];
                            iqFiltered[2 * (n - midTap) + 1] += h[i] * iqInput[2 * (n - i) + 1];

                        }
                        else
                        {
                            //iFiltered[n - midTap] += hS[i] * iStream[streamLength - 1];
                            //qFiltered[n - midTap] += hS[i] * qStream[streamLength - 1];
                            iqFiltered[2 * (n - midTap)] += h[i] * iqInput[2 * (streamLength - 1)];
                            iqFiltered[2 * (n - midTap) + 1] += h[i] * iqInput[2 * (streamLength - 1) + 1];
                        }
                    }
                    else
                    {
                        //iFiltered[n - midTap] += hS[i] * iStream[0];
                        //qFiltered[n - midTap] += hS[i] * qStream[0];
                        iqFiltered[2 * (n - midTap)] += h[i] * iqInput[0];
                        iqFiltered[2 * (n - midTap) + 1] += h[i] * iqInput[1];

                    }
                }
            }//end of filtering
        }
    }
}
