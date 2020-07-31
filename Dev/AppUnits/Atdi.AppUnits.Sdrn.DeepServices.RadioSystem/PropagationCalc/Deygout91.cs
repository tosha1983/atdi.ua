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

        private static NuMaxOut FindMaxHtoR(double hA, double hB, double wavelength, double dAB, in short[] profile, int profileStart, int profilePointsNumber, double RE, bool mainHillCalc)
        {
            //int __profileArrayLength = (__profileCount - profileStart);
            double dN = dAB / profilePointsNumber;
            double dAN = dN;
            double dNB = dN * (profilePointsNumber - 1);
            double inv2rE = 1 / (2 * RE);
            double invDaB = 1 / dAB;

            double hNMax = 0;
            NuMaxOut nuMaxOut;

            int secondaryHillCorrection = 1;

            if ( mainHillCalc )
            {
                hNMax = profile[profileStart] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;

                nuMaxOut = new NuMaxOut
                {
                    nuMax = hNMax * Math.Sqrt(dAB / (wavelength * dAN * dNB)),
                    nMax = profileStart
                };
            }
            else
            {
                secondaryHillCorrection = (int)Math.Ceiling(50 / dN);
                profileStart += secondaryHillCorrection;
                
                nuMaxOut = new NuMaxOut
                {
                    nuMax = -0.8,
                    nMax = profileStart
                };
            }
            
            
            if (profilePointsNumber - secondaryHillCorrection - profileStart > 1)
            {
                for (int n = profileStart + 1; n < profileStart + profilePointsNumber - secondaryHillCorrection; n++)
                {
                    dAN = dN * (n - profileStart);
                    dNB = dN * (profileStart + profilePointsNumber - n);
                    double h = profile[n] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;

                    double nuN = h * Math.Sqrt(dAB / (wavelength * dAN * dNB));

                    if (nuN > nuMaxOut.nuMax)
                    {
                        nuMaxOut.nuMax = nuN;
                        nuMaxOut.nMax = n;
                    }
                }
            }
            return nuMaxOut;
        }

        private static double J(double htr)
        {
            if (htr <= -0.5)
            {
                return 0.0f;
            }
            else if (-0.5 < htr && htr <= 0.5)
            {
                return 6 + 12 * htr;
            }
            else if (0.5 < htr && htr <= 1.0)
            {
                return 8 * (1 + htr);
            }
            else
            {
                return (double)(16 + 20 * Math.Log10(htr));
            }
        }
        

        public static double Calc(double ha_m, double hb_m, double Freq_MHz, double d_km, in short[] profile_m, int profileStartIndex, int profilePointsNumber, double rE_km, double SubDiffractionLoss)
        {
            int profileEndIndex = profileStartIndex + profilePointsNumber - 1;
            bool mainHill = true;
            double dAB = d_km * 1000;
            double rE = rE_km * 1000;
            double wavelength = 300 / Freq_MHz;
            ha_m += profile_m[profileStartIndex];
            hb_m += profile_m[profileEndIndex];
            NuMaxOut htrP = FindMaxHtoR(ha_m, hb_m, wavelength, dAB, profile_m, profileStartIndex, profilePointsNumber, rE, mainHill);
            
            double dN = dAB / (profilePointsNumber);
            double dAP = dN * htrP.nMax;
            double dPB = dN * (profileEndIndex - htrP.nMax);

            
            double p = 1.41421356 * htrP.nuMax;

            double diffractionLoss_dB = 0;

            if (htrP.nuMax > -0.5)
            {
                mainHill = false;
                NuMaxOut htrR = FindMaxHtoR(profile_m[htrP.nMax], hb_m, wavelength, dPB, in profile_m, htrP.nMax, profileEndIndex - htrP.nMax, rE, mainHill);
                NuMaxOut htrT = FindMaxHtoR(ha_m, profile_m[htrP.nMax], wavelength, dAP, in profile_m, profileStartIndex, htrP.nMax - profileStartIndex, rE, mainHill);

                double Ltc = 0;
                double alpha = 0;
                double htrS = -0.8;

                double dAT = 1;
                if (htrR.nMax > 0)
                { 
                    dAT = dN * htrT.nMax;
                }
                double dTP = dAB - htrT.nMax * dN;
                double dPR = 1;
                if (htrR.nMax > 0)
                {
                    dPR = htrR.nMax * dN;
                }

                if (htrT.nuMax > htrR.nuMax && htrT.nuMax > -0.5)
                {
                    double q = 1.41421356 * htrT.nuMax;
                    htrS = htrT.nuMax;
                    alpha = Math.Atan(Math.Sqrt(dTP * (dAT + dTP + dPR) / (dAT * dPR)));
                    Ltc = (12 - 20 * Math.Log10(2 / (1 - alpha / Math.PI))) * Math.Pow(Math.Abs(q / p), 2 * p);
                }
                else if (htrT.nuMax < htrR.nuMax && htrR.nuMax > -0.5)
                {
                    double dRB = dPB - dPR;
                    double q = 1.41421356 * htrR.nuMax;
                    htrS = htrR.nuMax;
                    alpha = Math.Atan(Math.Sqrt(dPR * (dTP + dPR + dRB) / (dTP * dRB)));
                    Ltc = (12 - 20 * Math.Log10(2 / (1 - alpha / Math.PI))) * Math.Pow(Math.Abs(q / p), 2 * p);
                }
                
                diffractionLoss_dB = J(htrP.nuMax) + J(htrS) + SubDiffractionLoss - Ltc;
            }
            
            return diffractionLoss_dB;
        }
    }

}
