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

        private static double CalcHtoRn(double hA, double hB, double wavelength, double dAB, in short[] profile, int profileStart, int profilePointsNumber, double inv2rE, double dN, int n)
        {
            double invDaB = 1 / dAB;
            double htr = -0.5;

            if (n > profileStart && n < profileStart + profilePointsNumber)
            {
                double dAN = dN * (n - profileStart);
                double dNB = dN * (profileStart + profilePointsNumber - n);
                double h = profile[n] + dAN * dNB * inv2rE - (hA * dNB + hB * dAN) * invDaB;
                htr = h * Math.Sqrt(dAB / (wavelength * dAN * dNB));
            }
            return htr;
        }

        private static NuMaxOut FindMaxHtoR(double hA, double hB, double wavelength, double dAB, in short[] profile, int profileStart, int profilePointsNumber, double inv2rE, double dN, bool mainHillCalc)
        {
            double dAN = dN;
            double dNB = dN * (profilePointsNumber - 1);
            //double inv2rE = 1 / (2 * RE);
            double invDaB = 1 / dAB;

            //double hNMax = 0;
            NuMaxOut nuMaxOut = new NuMaxOut
            {
                nuMax = -0.5,
                nMax = profileStart
            };

            int secondaryHillCorrectionStart = 1;
            int secondaryHillCorrectionEnd = 1;

            if ( mainHillCalc == false )
            {
                // calculate correction to excludebuilding counted when main hill calculated
                double secondaryHillCorrectionDistance = 70; //[m]
                int secondaryHillCorrectionPoints = (int)Math.Ceiling(secondaryHillCorrectionDistance / dN);
                
                if (profileStart + secondaryHillCorrectionPoints < profile.Length)
                {
                    for (int n = profileStart+1; n < profileStart + secondaryHillCorrectionPoints; n++)
                    {
                        double htrN = CalcHtoRn(hA, hB, wavelength, dAB, in profile, profileStart, profilePointsNumber, inv2rE, dN, n);
                        double htrNm1 = CalcHtoRn(hA, hB, wavelength, dAB, in profile, profileStart, profilePointsNumber, inv2rE, dN, n -1);
                        if (htrN <= -0.5 && htrNm1 >= -0.5)
                        {
                            secondaryHillCorrectionStart = n - profileStart + 1;
                        }
                    }
                }
                
                if (profileStart + profilePointsNumber - secondaryHillCorrectionPoints > 1)
                {
                    //NuMaxOut nuMaxEnd = FindMaxHtoR(hA, hB, wavelength, secondaryHillCorrectionDistance, in profile, profileStart + profilePointsNumber - secondaryHillCorrectionPoints, secondaryHillCorrectionPoints, inv2rE, dN, true);
                    for (int n = profileStart + profilePointsNumber - 1; n > profileStart + profilePointsNumber - secondaryHillCorrectionPoints; n--)
                    {
                        double htrN = CalcHtoRn(hA, hB, wavelength, dAB, in profile, profileStart, profilePointsNumber, inv2rE, dN, n);
                        double htrNm1 = CalcHtoRn(hA, hB, wavelength, dAB, in profile, profileStart, profilePointsNumber, inv2rE, dN, n + 1);
                        if (htrN <= -0.5 && htrNm1 > -0.5)
                        {
                            secondaryHillCorrectionEnd = profilePointsNumber + profileStart - n + 1;
                        }
                    }
                }
            }

            // Main block: search of max h/r 
            if (profilePointsNumber - secondaryHillCorrectionStart - secondaryHillCorrectionEnd > 1)
            {
                for (int n = profileStart + secondaryHillCorrectionStart; n < profileStart + profilePointsNumber - secondaryHillCorrectionEnd; n++)
                {
                    double nuN = CalcHtoRn(hA, hB, wavelength, dAB, in profile, profileStart, profilePointsNumber, inv2rE, dN, n);

                    if (nuN > nuMaxOut.nuMax)
                    {
                        nuMaxOut.nuMax = nuN;
                        nuMaxOut.nMax = n;
                    }
                    //System.Console.WriteLine($"h_to_r={nuN}, h_n={h}, n={n}");
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
            double dN = dAB / profilePointsNumber;
            double rE = rE_km * 1000;
            double wavelength = 300 / Freq_MHz;
            ha_m += profile_m[profileStartIndex];
            hb_m += profile_m[profileEndIndex];
            double inv2rE = 1 / (2 * rE);
            NuMaxOut htrP = FindMaxHtoR(ha_m, hb_m, wavelength, dAB, profile_m, profileStartIndex, profilePointsNumber, inv2rE, dN, mainHill);
            
            double dAP = dN * htrP.nMax;
            double dPB = dN * (profileEndIndex - htrP.nMax);

            double diffractionLoss_dB = 0;

            if (htrP.nuMax > -0.5)
            {
                mainHill = false;
                NuMaxOut htrT = FindMaxHtoR(ha_m, profile_m[htrP.nMax], wavelength, dAP, in profile_m, profileStartIndex, htrP.nMax - profileStartIndex, inv2rE, dN, mainHill);
                NuMaxOut htrR = FindMaxHtoR(profile_m[htrP.nMax], hb_m, wavelength, dPB, in profile_m, htrP.nMax, profileEndIndex - htrP.nMax, inv2rE, dN, mainHill);
                double Ltc = 0;
                double alpha = 0;
                double htrS = -0.5;

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
                // Parameters to calculate correction if obstacles are too close to each other
                double p = 1.41421356 * htrP.nuMax;
                double q = 0;
                if (htrT.nuMax > htrR.nuMax && htrT.nuMax > -0.5)
                {
                    q = 1.41421356 * CalcHtoRn(ha_m, hb_m, wavelength, dAB, profile_m, profileStartIndex, profilePointsNumber, inv2rE, dN, htrT.nMax);
                    htrS = htrT.nuMax;
                    alpha = Math.Atan(Math.Sqrt(dTP * (dAT + dTP + dPR) / (dAT * dPR)));
                }
                else if (htrT.nuMax < htrR.nuMax && htrR.nuMax > -0.5)
                {
                    double dRB = dAB - dPR;
                    q = 1.41421356 * CalcHtoRn(ha_m, hb_m, wavelength, dAB, profile_m, profileStartIndex, profilePointsNumber, inv2rE, dN, htrT.nMax);
                    htrS = htrR.nuMax;
                    alpha = Math.Atan(Math.Sqrt(dPR * (dTP + dPR + dRB) / (dTP * dRB)));
                }
                if (q / p > 0)
                {
                    Ltc = (12 - 20 * Math.Log10(2 / (1 - alpha / Math.PI))) * Math.Pow(q / p, 2 * p);
                }
                
                diffractionLoss_dB = J(htrP.nuMax) + J(htrS) + SubDiffractionLoss - Ltc;
                //System.Console.WriteLine($"    L_mh={J(htrP.nuMax)}dB, L_sh={J(htrS)}dB;  h/r={htrP.nuMax}, h/r_s={htrS}, nM={htrP.nMax}, ns1={htrR.nMax}, ns2={htrT.nMax}");
            }
            
            return diffractionLoss_dB;
        }
    }

}
