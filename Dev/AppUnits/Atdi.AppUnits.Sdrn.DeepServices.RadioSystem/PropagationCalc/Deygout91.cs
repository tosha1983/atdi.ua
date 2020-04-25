using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
    public static class Deygout91
    {
        private struct NuMaxOut
        {
            public double nuMax;
            public int nMax;
        }

        private static NuMaxOut FindMaxNu(double hA, double hB, double wavelength, double dAB, in short[] profile, int profileStart, int profileLength, double RE)
        {
            //int __profileArrayLength = (__profileCount - profileStart);
            double dN = dAB / profileLength;
            double dAN = dN;
            double dNB = dN * (profileLength - 1);
            double inv2rE = 1 / (2 * RE);
            double invDaB = 1 / dAB;

            double hNMax = profile[profileStart] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;

            
            NuMaxOut nuMaxOut = new NuMaxOut
            {
                nuMax = hNMax * Math.Sqrt(2 * dAB / (wavelength * dAN * dNB)),
                nMax = profileStart
            };
            
            if (profileLength > 1)
            {
                for (int n = profileStart + 1; n < profileStart + profileLength - 1; n++)
                {
                    dAN = dN * (n - profileStart);
                    dNB = dN * (profileStart + profileLength - n);
                    double h = profile[n] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;

                    double nuN = h * Math.Sqrt(2 * dAB / (wavelength * dAN * dNB));

                    if (nuN > nuMaxOut.nuMax)
                    {
                        nuMaxOut.nuMax = nuN;
                        nuMaxOut.nMax = n;
                    }
                }
            }
                return nuMaxOut;
        }

        private static double J(double nu)
        {
            if (nu <= -0.78)
            {
                return 0.0f;
            }
            else
            {
                return (double)(6.9 + 20 * Math.Log10(Math.Sqrt(Math.Pow(nu - 0.1, 2) + 1) + nu - 0.1));
            }
        }
        

        public static double Calc(double ha_m, double hb_m, double Freq_MHz, double d_km, in short[] profile_m, int profileStartIndex, int profilePointsNumber, double rE_km, double SubDiffractionLoss)
        {
            int profileEndIndex = profileStartIndex + profilePointsNumber - 1;
            double dAB = d_km * 1000;
            double rE = rE_km * 1000;
            double wavelength = 300 / Freq_MHz;
            ha_m += profile_m[profileStartIndex];
            hb_m += profile_m[profileEndIndex];
            NuMaxOut nu;
            nu = FindMaxNu(ha_m, hb_m, wavelength, dAB, profile_m, profileStartIndex, profilePointsNumber, rE);

            double nuP = nu.nuMax;

            double dN = dAB / (profilePointsNumber);
            double dAP = dN * nu.nMax;

            double dPB = dN * (profileEndIndex - nu.nMax);

            double diffractionLoss_dB = 0;

            if (nuP > -0.78)
            {
                NuMaxOut nuT = FindMaxNu(ha_m, profile_m[nu.nMax], wavelength, dAP, in profile_m, profileStartIndex, nu.nMax - profileStartIndex, rE);

                NuMaxOut nuR = FindMaxNu(profile_m[nu.nMax], hb_m, wavelength, dPB, in profile_m, nu.nMax, profileEndIndex - nu.nMax, rE);
                double C = 10.0f + 0.04 * d_km;
                diffractionLoss_dB = J(nuP) + (1.0 - Math.Exp(-J(nuP) / 6.0f)) * (J(nuT.nuMax) + J(nuR.nuMax) + C + SubDiffractionLoss);
            }
            

            return diffractionLoss_dB;
        }
    }

}
