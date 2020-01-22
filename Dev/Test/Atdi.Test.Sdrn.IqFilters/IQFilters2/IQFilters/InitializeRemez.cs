using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace IQFilters
{
    class InitializeRemez
    {
        //[DllImport("DllApp.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void RemezCall(float[] h, int numtaps, int numband, float[] bands, float[] des, float[] weight, int type);
        [DllImport("DllApp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RemezCall(double[] h, int numtaps, int numband, double[] bands, double[] des, double[] weight, int type);
        //static void Main(string[] args)
        //
        const byte OFF = 0;
        const byte LOWPASS = 1;
        const byte HIGHPASS = 2;
        const byte BANDSTOP = 3;// NOTCH
        const byte BANDPASS = 4;
        //
        //public float[] h;
        public double[] h;

        /**
 * оцениваем необходимый порядок фильтра с помощью формулы Кайзера
 * 
 * @param trband
 *            - нормализованная переходная полоса фильтра
 * @param atten_dB
 *            - степень подавления в дБ.
 * @param ripple_dB
 *            - пульсации в полосе пропускания дБ
 */


        /**
         * Инициализация параметров для расчёта методом Паркса-Маклеллана
         * 
         * @param type
         *            - тип фильтра
         * @param order
         *            - порядок фильтра
         * @param f1
         *            - частота среза ФНЧ и ФВЧ фильтра
         * @param f2
         *            - верхняя частота для полосового и режекторного фильтра
         * @param trband
         *            - нормализованная переходная полоса фильтра
         * @param sampleRate
         *            - частота дискретизации
         * @param atten_dB
         *            - степень подавления в дБ.
         * @param ripple_dB
         *            - пульсации в полосе пропускания дБ
         */
        public void init(byte type, int order, float f1, float f2, float trband, float sampleRate, float atten_dB,
                float ripple_dB)
        {
            f1 /= sampleRate;
            f2 /= sampleRate;
            trband /= sampleRate;
            int numBands = 2;
            float deltaP = (float)(0.5 * (1.0 - Math.Pow(10.0, -0.05 * ripple_dB)));
            float deltaS = (float)Math.Pow(10.0, -0.05 * atten_dB);
            float rippleRatio = deltaP / deltaS;
            // оцениваем необходимый порядок фильтра с помощью формулы Кайзера
            if (order < 1)
            {
                order = (int)(Math.Round((-10.0 * Math.Log10(deltaP * deltaS) - 13) / (14.6 * trband)));
            }
            int numTaps = order + 1;
            
            //int numTaps = order;
            if (type == BANDPASS || type == BANDSTOP)
                numBands = 3;
            //float[] desired = new float[numBands];
            //float[] bands = new float[numBands << 1];
            //float[] weights = new float[numBands];
            //switch (type)
            //{
            //    case LOWPASS:
            //        desired[0] = 1.0f;
            //        desired[1] = 0.0f;
            //        bands[0] = 0.0f;
            //        bands[1] = f1;
            //        bands[2] = f1 + trband;
            //        bands[3] = 0.5f;
            //        weights[0] = 1.0f;
            //        weights[1] = rippleRatio;
            //        break;
            //    case HIGHPASS:
            //        desired[0] = 0.0f;
            //        desired[1] = 1.0f;
            //        bands[0] = 0.0f;
            //        bands[1] = f1 - trband;
            //        bands[2] = f1;
            //        bands[3] = 0.5f;
            //        weights[0] = rippleRatio;
            //        weights[1] = 1.0f;
            //        break;
            //    case BANDPASS:
            //        desired[0] = 0.0f;
            //        desired[1] = 1.0f;
            //        desired[2] = 0.0f;
            //        bands[0] = 0.0f;
            //        bands[1] = f1 - trband;
            //        bands[2] = f1;
            //        bands[3] = f2;
            //        bands[4] = f2 + trband;
            //        bands[5] = 0.5f;
            //        weights[0] = rippleRatio;
            //        weights[1] = 1.0f;
            //        weights[2] = rippleRatio;
            //        break;
            //    case BANDSTOP:
            //        desired[0] = 1.0f;
            //        desired[1] = 0.0f;
            //        desired[2] = 1.0f;
            //        bands[0] = 0.0f;
            //        bands[1] = f1 - trband;
            //        bands[2] = f1;
            //        bands[3] = f2;
            //        bands[4] = f2 + trband;
            //        bands[5] = 0.5f;
            //        weights[0] = 1;
            //        weights[1] = rippleRatio;
            //        weights[2] = 1;
            //        break;
            //}

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
            // BANDPASS противоположен DIFFERENTIATOR или HILBERT !
            //float[] h = new float[numTaps];
            h = new double[numTaps];
            int numBand = desired.Length;

            RemezCall(h, numTaps, numBand, bands, desired, weights, 1);
            //numTaps, bands, desired, weights, type == LOWPASS ? BANDPASS : type, m_fir
        }
    }
}
