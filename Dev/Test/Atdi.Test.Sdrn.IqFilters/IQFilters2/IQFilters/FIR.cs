using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQFilters
{
    class FIR 
    {
        public byte filterType;
        public double f1_Hz;
        public double f2_Hz;
        public double filterAatt_dB;
        public double desiredRipple_dB;
        public int sampleRate_Hz;//?
        public double transitionBand;
        private int filterOrder;

        public FIR(byte filterType, double f1, double f2, int sampleRate, double filterAatt_dB, double desiredRipple_dB, double transitionBand)
        {
            this.filterType = filterType;
            this.f1_Hz = f1;
            this.f2_Hz = f2;
            this.filterAatt_dB = filterAatt_dB;
            this.desiredRipple_dB = desiredRipple_dB;
            this.sampleRate_Hz = sampleRate;//?
            this.transitionBand = transitionBand;
            this.filterOrder = estimateFilterOrderKaiser(transitionBand, filterAatt_dB, desiredRipple_dB);
        }

        // Estimate filter order 
        private int estimateFilterOrderKaiser(double transitionBand, double atten_dB, double ripple_dB)
        {
            double deltaP = 0.5 * (1.0 - Math.Pow(10.0, -0.05 * ripple_dB));
            double deltaS = Math.Pow(10.0, -0.05 * atten_dB);
            // Kaiser's formula
            double orderK = Math.Round((-10.0 * Math.Log10(deltaP * deltaS) - 13) / (14.6 * transitionBand));//Math.Round((float)((-10.0 * Math.Log10(deltaP * deltaS) - 13) / (14.6 * trband)))

            // 'Rule of thumb'
            double orderT = (20 * Math.Log10(deltaS)) / (22 * transitionBand);

            return (int)(Math.Max(orderK, orderT));
        }

        // Create impulse response // Calculate coefficients
        // Describe window functions
        // Perform streeam filtration

    }
}
