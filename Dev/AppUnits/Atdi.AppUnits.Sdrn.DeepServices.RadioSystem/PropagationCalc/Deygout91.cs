﻿using System;
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

        private static NuMaxOut FindMaxNu(double hA, double hB, double wavelength, double dAB, in short[] profile, int profileStart, int profileCount, double RE)
        {
            int profileArrayLength = (profileCount - profileStart);
            double dN = dAB / profileArrayLength;
            double dAN = dN;
            double dNB = dN * (profileArrayLength - 1);
            double inv2rE = 1 / (2 * RE);
            double invDaB = 1 / dAB;

            double hNMax = profile[profileStart] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;

            
            NuMaxOut nuMaxOut = new NuMaxOut
            {
                nuMax = (double)(hNMax * Math.Sqrt(2 * dAB / (wavelength * dAN * dNB))),
                nMax = profileStart
            };
            
            if (profileArrayLength > 1)
            {
                for (int n = profileStart + 1; n < profileCount; n++)
                {
                    dAN = dN * (n - profileStart);
                    dNB = dN * (profileCount - n);
                    double h = profile[n] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;

                    double nuN = (double)(h * Math.Sqrt(2 * dAB / (wavelength * dAN * dNB)));

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
        

        public static double Calc(double ha_m, double hb_m, double Freq_MHz, double d_km, in short[] profile_m, int profileStartPosition, int profilePointsNumber, double rE_km, double SubDiffractionLoss)
        {

            double dAB = d_km * 1000;
            double rE = rE_km * 1000;
            double wavelength = 300 / Freq_MHz;
            ha_m += profile_m[profileStartPosition];
            hb_m += profile_m[profilePointsNumber];

            NuMaxOut nu;
            nu = FindMaxNu(ha_m, hb_m, wavelength, dAB, profile_m, profileStartPosition, profilePointsNumber, rE);

            double nuP = nu.nuMax;

            double dN = dAB / (profilePointsNumber - profileStartPosition);
            double dAP = dN * nu.nMax;

            double dPB = dN * (profilePointsNumber - profileStartPosition - nu.nMax);

            double diffractionLoss_dB = 0;

            if (nuP > -0.78)
            {
                NuMaxOut nuT = FindMaxNu(ha_m, profile_m[nu.nMax], wavelength, dAP, in profile_m, profileStartPosition, nu.nMax, rE);

                NuMaxOut nuR = FindMaxNu(profile_m[nu.nMax], hb_m, wavelength, dPB, in profile_m, nu.nMax, profilePointsNumber, rE);
                double C = 10.0f + 0.04 * d_km;
                diffractionLoss_dB = J(nuP) + (1.0 - Math.Exp(-J(nuP) / 6.0f)) * (J(nuT.nuMax) + J(nuR.nuMax) + C + SubDiffractionLoss);
            }
            

            return diffractionLoss_dB;
        }
    }

}
