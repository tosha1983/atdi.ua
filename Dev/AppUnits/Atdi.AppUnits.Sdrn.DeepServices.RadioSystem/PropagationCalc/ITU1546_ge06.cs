using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength;

//namespace Atdi.Test.DeepServices.PropagaionModels
namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
    //public class land_sea
    //{
    //    public double land = 0;
    //    public double sea = 0;

    //    public double Length { get { return land + sea; } }
    //}

    public class ITU1546_ge06
    {
        // табличное представление кривых для каждого сочетания расстояния, процента времени и высоты для определения напряженности поля
        private double Get_land_100m_50t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 89.975852;
            table[1, 1] = 80.275128;
            table[2, 1] = 74.166239;
            table[3, 1] = 69.518418;
            table[4, 1] = 65.69942;
            table[5, 1] = 62.435852;
            table[6, 1] = 59.580317;
            table[7, 1] = 57.041171;
            table[8, 1] = 54.755982;
            table[9, 1] = 52.679639;
            table[10, 1] = 50.778224;
            table[11, 1] = 49.025504;
            table[12, 1] = 47.400748;
            table[13, 1] = 45.887292;
            table[14, 1] = 44.471549;
            table[15, 1] = 43.142302;
            table[16, 1] = 41.890198;
            table[17, 1] = 40.707358;
            table[18, 1] = 39.587089;
            table[19, 1] = 38.523662;
            table[20, 1] = 33.906877;
            table[21, 1] = 30.181101;
            table[22, 1] = 27.102165;
            table[23, 1] = 24.51782;
            table[24, 1] = 22.324232;
            table[25, 1] = 20.44567;
            table[26, 1] = 18.824162;
            table[27, 1] = 17.413824;
            table[28, 1] = 16.177532;
            table[29, 1] = 15.084811;
            table[30, 1] = 14.110428;
            table[31, 1] = 13.233379;
            table[32, 1] = 12.436137;
            table[33, 1] = 11.70406;
            table[34, 1] = 11.024921;
            table[35, 1] = 10.388525;
            table[36, 1] = 9.21146;
            table[37, 1] = 8.120958;
            table[38, 1] = 7.081758;
            table[39, 1] = 6.070394;
            table[40, 1] = 5.071714;
            table[41, 1] = 4.076377;
            table[42, 1] = 3.079057;
            table[43, 1] = 2.077166;
            table[44, 1] = 1.069944;
            table[45, 1] = 0.057808;
            table[46, 1] = -2.485571;
            table[47, 1] = -5.026417;
            table[48, 1] = -7.538997;
            table[49, 1] = -10.003377;
            table[50, 1] = -12.406876;
            table[51, 1] = -14.743468;
            table[52, 1] = -17.012525;
            table[53, 1] = -19.217485;
            table[54, 1] = -21.364677;
            table[55, 1] = -23.462348;
            table[56, 1] = -25.519898;
            table[57, 1] = -27.54727;
            table[58, 1] = -29.55447;
            table[59, 1] = -31.551176;
            table[60, 1] = -33.54641;
            table[61, 1] = -35.548264;
            table[62, 1] = -37.56365;
            table[63, 1] = -39.598079;
            table[64, 1] = -41.655471;
            table[65, 1] = -43.737982;
            table[66, 1] = -45.845868;
            table[67, 1] = -47.977393;
            table[68, 1] = -50.128777;
            table[69, 1] = -52.294211;
            table[70, 1] = -54.465939;
            table[71, 1] = -56.634413;
            table[72, 1] = -58.788536;
            table[73, 1] = -60.915978;
            table[74, 1] = -63.00358;
            table[75, 1] = -65.037809;
            table[76, 1] = -67.005279;
            table[77, 1] = -68.893276;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_50t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 92.181157;
            table[1, 1] = 83.090807;
            table[2, 1] = 77.529572;
            table[3, 1] = 73.354816;
            table[4, 1] = 69.920619;
            table[5, 1] = 66.95776;
            table[6, 1] = 64.332176;
            table[7, 1] = 61.967283;
            table[8, 1] = 59.813965;
            table[9, 1] = 57.837704;
            table[10, 1] = 56.012601;
            table[11, 1] = 54.318341;
            table[12, 1] = 52.738521;
            table[13, 1] = 51.259635;
            table[14, 1] = 49.870406;
            table[15, 1] = 48.561319;
            table[16, 1] = 47.324273;
            table[17, 1] = 46.152327;
            table[18, 1] = 45.039488;
            table[19, 1] = 43.980553;
            table[20, 1] = 39.353167;
            table[21, 1] = 35.575326;
            table[22, 1] = 32.409272;
            table[23, 1] = 29.703944;
            table[24, 1] = 27.356109;
            table[25, 1] = 25.291689;
            table[26, 1] = 23.455968;
            table[27, 1] = 21.807891;
            table[28, 1] = 20.316355;
            table[29, 1] = 18.957532;
            table[30, 1] = 17.712841;
            table[31, 1] = 16.5674;
            table[32, 1] = 15.508884;
            table[33, 1] = 14.526731;
            table[34, 1] = 13.611629;
            table[35, 1] = 12.75521;
            table[36, 1] = 11.188785;
            table[37, 1] = 9.775007;
            table[38, 1] = 8.47176;
            table[39, 1] = 7.246432;
            table[40, 1] = 6.074722;
            table[41, 1] = 4.93919;
            table[42, 1] = 3.827778;
            table[43, 1] = 2.732491;
            table[44, 1] = 1.64831;
            table[45, 1] = 0.57233;
            table[46, 1] = -2.088893;
            table[47, 1] = -4.707595;
            table[48, 1] = -7.273236;
            table[49, 1] = -9.774718;
            table[50, 1] = -12.204732;
            table[51, 1] = -14.560645;
            table[52, 1] = -16.844031;
            table[53, 1] = -19.059792;
            table[54, 1] = -21.215247;
            table[55, 1] = -23.319328;
            table[56, 1] = -25.381915;
            table[57, 1] = -27.413291;
            table[58, 1] = -29.423708;
            table[59, 1] = -31.423023;
            table[60, 1] = -33.420393;
            table[61, 1] = -35.42401;
            table[62, 1] = -37.440862;
            table[63, 1] = -39.476519;
            table[64, 1] = -41.534947;
            table[65, 1] = -43.618337;
            table[66, 1] = -45.726974;
            table[67, 1] = -47.859144;
            table[68, 1] = -50.011086;
            table[69, 1] = -52.177004;
            table[70, 1] = -54.349154;
            table[71, 1] = -56.517998;
            table[72, 1] = -58.672447;
            table[73, 1] = -60.800177;
            table[74, 1] = -62.888035;
            table[75, 1] = -64.922493;
            table[76, 1] = -66.890166;
            table[77, 1] = -68.778346;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_50t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.635547;
            table[1, 1] = 86.00138;
            table[2, 1] = 80.82345;
            table[3, 1] = 77.014863;
            table[4, 1] = 73.924833;
            table[5, 1] = 71.272338;
            table[6, 1] = 68.916118;
            table[7, 1] = 66.778309;
            table[8, 1] = 64.812656;
            table[9, 1] = 62.989553;
            table[10, 1] = 61.288551;
            table[11, 1] = 59.694488;
            table[12, 1] = 58.195448;
            table[13, 1] = 56.781648;
            table[14, 1] = 55.444826;
            table[15, 1] = 54.177862;
            table[16, 1] = 52.974551;
            table[17, 1] = 51.829432;
            table[18, 1] = 50.737681;
            table[19, 1] = 49.695009;
            table[20, 1] = 45.096999;
            table[21, 1] = 41.290077;
            table[22, 1] = 38.052688;
            table[23, 1] = 35.239021;
            table[24, 1] = 32.748142;
            table[25, 1] = 30.508235;
            table[26, 1] = 28.46789;
            table[27, 1] = 26.590669;
            table[28, 1] = 24.851189;
            table[29, 1] = 23.231999;
            table[30, 1] = 21.721003;
            table[31, 1] = 20.309404;
            table[32, 1] = 18.990163;
            table[33, 1] = 17.756983;
            table[34, 1] = 16.603707;
            table[35, 1] = 15.524069;
            table[36, 1] = 13.559993;
            table[37, 1] = 11.813599;
            table[38, 1] = 10.237193;
            table[39, 1] = 8.789752;
            table[40, 1] = 7.438162;
            table[41, 1] = 6.156958;
            table[42, 1] = 4.927268;
            table[43, 1] = 3.73554;
            table[44, 1] = 2.572312;
            table[45, 1] = 1.431171;
            table[46, 1] = -1.348877;
            table[47, 1] = -4.044637;
            table[48, 1] = -6.661949;
            table[49, 1] = -9.199125;
            table[50, 1] = -11.654455;
            table[51, 1] = -14.028746;
            table[52, 1] = -16.325754;
            table[53, 1] = -18.551801;
            table[54, 1] = -20.715154;
            table[55, 1] = -22.82539;
            table[56, 1] = -24.892837;
            table[57, 1] = -26.928099;
            table[58, 1] = -28.941657;
            table[59, 1] = -30.943536;
            table[60, 1] = -32.943019;
            table[61, 1] = -34.948391;
            table[62, 1] = -36.966712;
            table[63, 1] = -39.003609;
            table[64, 1] = -41.063088;
            table[65, 1] = -43.147376;
            table[66, 1] = -45.256785;
            table[67, 1] = -47.389621;
            table[68, 1] = -49.54214;
            table[69, 1] = -51.708563;
            table[70, 1] = -53.881154;
            table[71, 1] = -56.050386;
            table[72, 1] = -58.205177;
            table[73, 1] = -60.333211;
            table[74, 1] = -62.421338;
            table[75, 1] = -64.456036;
            table[76, 1] = -66.423924;
            table[77, 1] = -68.312296;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_50t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 97.384505;
            table[1, 1] = 89.207574;
            table[2, 1] = 84.350391;
            table[3, 1] = 80.83122;
            table[4, 1] = 78.021358;
            table[5, 1] = 75.640654;
            table[6, 1] = 73.542303;
            table[7, 1] = 71.642351;
            table[8, 1] = 69.890311;
            table[9, 1] = 68.254774;
            table[10, 1] = 66.71556;
            table[11, 1] = 65.259188;
            table[12, 1] = 63.876181;
            table[13, 1] = 62.559472;
            table[14, 1] = 61.303469;
            table[15, 1] = 60.103501;
            table[16, 1] = 58.955517;
            table[17, 1] = 57.855898;
            table[18, 1] = 56.801363;
            table[19, 1] = 55.788902;
            table[20, 1] = 51.267648;
            table[21, 1] = 47.459335;
            table[22, 1] = 44.171197;
            table[23, 1] = 41.267845;
            table[24, 1] = 38.652583;
            table[25, 1] = 36.256256;
            table[26, 1] = 34.030371;
            table[27, 1] = 31.942327;
            table[28, 1] = 29.971576;
            table[29, 1] = 28.106245;
            table[30, 1] = 26.340144;
            table[31, 1] = 24.670258;
            table[32, 1] = 23.094821;
            table[33, 1] = 21.612009;
            table[34, 1] = 20.219207;
            table[35, 1] = 18.912717;
            table[36, 1] = 16.538857;
            table[37, 1] = 14.444151;
            table[38, 1] = 12.577955;
            table[39, 1] = 10.892439;
            table[40, 1] = 9.346499;
            table[41, 1] = 7.906945;
            table[42, 1] = 6.548108;
            table[43, 1] = 5.250735;
            table[44, 1] = 4.000724;
            table[45, 1] = 2.78794;
            table[46, 1] = -0.122912;
            table[47, 1] = -2.90354;
            table[48, 1] = -5.577762;
            table[49, 1] = -8.154263;
            table[50, 1] = -10.637506;
            table[51, 1] = -13.032086;
            table[52, 1] = -15.344155;
            table[53, 1] = -17.581593;
            table[54, 1] = -19.753705;
            table[55, 1] = -21.870779;
            table[56, 1] = -23.943635;
            table[57, 1] = -25.983228;
            table[58, 1] = -28.000291;
            table[59, 1] = -30.005036;
            table[60, 1] = -32.006883;
            table[61, 1] = -34.014221;
            table[62, 1] = -36.03419;
            table[63, 1] = -38.072478;
            table[64, 1] = -40.133139;
            table[65, 1] = -42.218437;
            table[66, 1] = -44.328714;
            table[67, 1] = -46.4623;
            table[68, 1] = -48.61547;
            table[69, 1] = -50.782459;
            table[70, 1] = -52.955548;
            table[71, 1] = -55.125217;
            table[72, 1] = -57.280394;
            table[73, 1] = -59.40877;
            table[74, 1] = -61.4972;
            table[75, 1] = -63.532169;
            table[76, 1] = -65.5003;
            table[77, 1] = -67.38889;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_50t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 100.318067;
            table[1, 1] = 92.674184;
            table[2, 1] = 88.142741;
            table[3, 1] = 84.885018;
            table[4, 1] = 82.313726;
            table[5, 1] = 80.163501;
            table[6, 1] = 78.291523;
            table[7, 1] = 76.612716;
            table[8, 1] = 75.073307;
            table[9, 1] = 73.6382;
            table[10, 1] = 72.284151;
            table[11, 1] = 70.995694;
            table[12, 1] = 69.76252;
            table[13, 1] = 68.577707;
            table[14, 1] = 67.436528;
            table[15, 1] = 66.335633;
            table[16, 1] = 65.272514;
            table[17, 1] = 64.245156;
            table[18, 1] = 63.251832;
            table[19, 1] = 62.290968;
            table[20, 1] = 57.923133;
            table[21, 1] = 54.161053;
            table[22, 1] = 50.859376;
            table[23, 1] = 47.901729;
            table[24, 1] = 45.19899;
            table[25, 1] = 42.685292;
            table[26, 1] = 40.314208;
            table[27, 1] = 38.055323;
            table[28, 1] = 35.89097;
            table[29, 1] = 33.812984;
            table[30, 1] = 31.819589;
            table[31, 1] = 29.912603;
            table[32, 1] = 28.095169;
            table[33, 1] = 26.370112;
            table[34, 1] = 24.738938;
            table[35, 1] = 23.201374;
            table[36, 1] = 20.397099;
            table[37, 1] = 17.923485;
            table[38, 1] = 15.732972;
            table[39, 1] = 13.774902;
            table[40, 1] = 12.002404;
            table[41, 1] = 10.375587;
            table[42, 1] = 8.862259;
            table[43, 1] = 7.437347;
            table[44, 1] = 6.081793;
            table[45, 1] = 4.78136;
            table[46, 1] = 1.709288;
            table[47, 1] = -1.176759;
            table[48, 1] = -3.922033;
            table[49, 1] = -6.547809;
            table[50, 1] = -9.066119;
            table[51, 1] = -11.486241;
            table[52, 1] = -13.817302;
            table[53, 1] = -16.069123;
            table[54, 1] = -18.252309;
            table[55, 1] = -20.378035;
            table[56, 1] = -22.457741;
            table[57, 1] = -24.502821;
            table[58, 1] = -26.524328;
            table[59, 1] = -28.532708;
            table[60, 1] = -30.537555;
            table[61, 1] = -32.547389;
            table[62, 1] = -34.569451;
            table[63, 1] = -36.609505;
            table[64, 1] = -38.671667;
            table[65, 1] = -40.758248;
            table[66, 1] = -42.869628;
            table[67, 1] = -45.004167;
            table[68, 1] = -47.158165;
            table[69, 1] = -49.325876;
            table[70, 1] = -51.499597;
            table[71, 1] = -53.669822;
            table[72, 1] = -55.825489;
            table[73, 1] = -57.9543;
            table[74, 1] = -60.043117;
            table[75, 1] = -62.07843;
            table[76, 1] = -64.046869;
            table[77, 1] = -65.935735;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_50t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 103.120502;
            table[1, 1] = 96.119692;
            table[2, 1] = 91.968629;
            table[3, 1] = 88.993402;
            table[4, 1] = 86.660124;
            table[5, 1] = 84.727135;
            table[6, 1] = 83.063318;
            table[7, 1] = 81.589073;
            table[8, 1] = 80.252365;
            table[9, 1] = 79.017504;
            table[10, 1] = 77.859317;
            table[11, 1] = 76.759834;
            table[12, 1] = 75.706242;
            table[13, 1] = 74.689515;
            table[14, 1] = 73.703427;
            table[15, 1] = 72.743795;
            table[16, 1] = 71.807899;
            table[17, 1] = 70.894024;
            table[18, 1] = 70.001117;
            table[19, 1] = 69.128528;
            table[20, 1] = 65.057416;
            table[21, 1] = 61.436134;
            table[22, 1] = 58.194286;
            table[23, 1] = 55.251096;
            table[24, 1] = 52.532544;
            table[25, 1] = 49.978308;
            table[26, 1] = 47.543206;
            table[27, 1] = 45.196392;
            table[28, 1] = 42.919531;
            table[29, 1] = 40.704406;
            table[30, 1] = 38.550265;
            table[31, 1] = 36.46116;
            table[32, 1] = 34.443562;
            table[33, 1] = 32.504418;
            table[34, 1] = 30.649757;
            table[35, 1] = 28.883852;
            table[36, 1] = 25.624759;
            table[37, 1] = 22.720205;
            table[38, 1] = 20.138921;
            table[39, 1] = 17.837208;
            table[40, 1] = 15.768665;
            table[41, 1] = 13.890122;
            table[42, 1] = 12.164385;
            table[43, 1] = 10.560916;
            table[44, 1] = 9.055424;
            table[45, 1] = 7.628982;
            table[46, 1] = 4.321613;
            table[47, 1] = 1.279258;
            table[48, 1] = -1.57246;
            table[49, 1] = -4.272572;
            table[50, 1] = -6.844046;
            table[51, 1] = -9.303027;
            table[52, 1] = -11.663058;
            table[53, 1] = -13.936864;
            table[54, 1] = -16.137;
            table[55, 1] = -18.275985;
            table[56, 1] = -20.366198;
            table[57, 1] = -22.419702;
            table[58, 1] = -24.448036;
            table[59, 1] = -26.462001;
            table[60, 1] = -28.47146;
            table[61, 1] = -30.485133;
            table[62, 1] = -32.510413;
            table[63, 1] = -34.553186;
            table[64, 1] = -36.617659;
            table[65, 1] = -38.706215;
            table[66, 1] = -40.819293;
            table[67, 1] = -42.955299;
            table[68, 1] = -45.110571;
            table[69, 1] = -47.279393;
            table[70, 1] = -49.454087;
            table[71, 1] = -51.625169;
            table[72, 1] = -53.781592;
            table[73, 1] = -55.911072;
            table[74, 1] = -58.000484;
            table[75, 1] = -60.036329;
            table[76, 1] = -62.005242;
            table[77, 1] = -63.894534;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_50t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 105.242609;
            table[1, 1] = 98.857707;
            table[2, 1] = 95.095801;
            table[3, 1] = 92.41247;
            table[4, 1] = 90.320287;
            table[5, 1] = 88.600472;
            table[6, 1] = 87.135157;
            table[7, 1] = 85.853051;
            table[8, 1] = 84.707392;
            table[9, 1] = 83.665614;
            table[10, 1] = 82.703995;
            table[11, 1] = 81.80468;
            table[12, 1] = 80.953922;
            table[13, 1] = 80.141009;
            table[14, 1] = 79.357567;
            table[15, 1] = 78.59709;
            table[16, 1] = 77.854603;
            table[17, 1] = 77.126392;
            table[18, 1] = 76.409783;
            table[19, 1] = 75.702949;
            table[20, 1] = 72.290194;
            table[21, 1] = 69.082123;
            table[22, 1] = 66.099112;
            table[23, 1] = 63.331892;
            table[24, 1] = 60.746502;
            table[25, 1] = 58.301307;
            table[26, 1] = 55.95751;
            table[27, 1] = 53.683883;
            table[28, 1] = 51.458278;
            table[29, 1] = 49.267439;
            table[30, 1] = 47.105869;
            table[31, 1] = 44.974219;
            table[32, 1] = 42.877491;
            table[33, 1] = 40.823265;
            table[34, 1] = 38.820156;
            table[35, 1] = 36.876559;
            table[36, 1] = 33.19543;
            table[37, 1] = 29.817323;
            table[38, 1] = 26.75086;
            table[39, 1] = 23.981533;
            table[40, 1] = 21.480733;
            table[41, 1] = 19.213831;
            table[42, 1] = 17.145787;
            table[43, 1] = 15.244355;
            table[44, 1] = 13.481508;
            table[45, 1] = 11.833768;
            table[46, 1] = 8.100969;
            table[47, 1] = 4.767244;
            table[48, 1] = 1.713116;
            table[49, 1] = -1.130267;
            table[50, 1] = -3.805172;
            table[51, 1] = -6.340266;
            table[52, 1] = -8.757319;
            table[53, 1] = -11.074556;
            table[54, 1] = -13.308275;
            table[55, 1] = -15.473584;
            table[56, 1] = -17.584693;
            table[57, 1] = -19.654974;
            table[58, 1] = -21.696918;
            table[59, 1] = -23.722031;
            table[60, 1] = -25.740699;
            table[61, 1] = -27.762042;
            table[62, 1] = -29.793758;
            table[63, 1] = -31.841967;
            table[64, 1] = -33.911062;
            table[65, 1] = -36.003571;
            table[66, 1] = -38.120048;
            table[67, 1] = -40.258993;
            table[68, 1] = -42.416816;
            table[69, 1] = -44.587865;
            table[70, 1] = -46.76451;
            table[71, 1] = -48.937307;
            table[72, 1] = -51.095244;
            table[73, 1] = -53.226066;
            table[74, 1] = -55.316672;
            table[75, 1] = -57.35358;
            table[76, 1] = -59.323444;
            table[77, 1] = -61.21359;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_50t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.356584;
            table[1, 1] = 100.284591;
            table[2, 1] = 96.730553;
            table[3, 1] = 94.207699;
            table[4, 1] = 92.24977;
            table[5, 1] = 90.648882;
            table[6, 1] = 89.29397;
            table[7, 1] = 88.118573;
            table[8, 1] = 87.079662;
            table[9, 1] = 86.147696;
            table[10, 1] = 85.301451;
            table[11, 1] = 84.525108;
            table[12, 1] = 83.806508;
            table[13, 1] = 83.13607;
            table[14, 1] = 82.506076;
            table[15, 1] = 81.9102;
            table[16, 1] = 81.343177;
            table[17, 1] = 80.800578;
            table[18, 1] = 80.278643;
            table[19, 1] = 79.77416;
            table[20, 1] = 77.430187;
            table[21, 1] = 75.24705;
            table[22, 1] = 73.137574;
            table[23, 1] = 71.082292;
            table[24, 1] = 69.082851;
            table[25, 1] = 67.139023;
            table[26, 1] = 65.243052;
            table[27, 1] = 63.38176;
            table[28, 1] = 61.540308;
            table[29, 1] = 59.705222;
            table[30, 1] = 57.866225;
            table[31, 1] = 56.017025;
            table[32, 1] = 54.155361;
            table[33, 1] = 52.282561;
            table[34, 1] = 50.402856;
            table[35, 1] = 48.522573;
            table[36, 1] = 44.791371;
            table[37, 1] = 41.153416;
            table[38, 1] = 37.666046;
            table[39, 1] = 34.370263;
            table[40, 1] = 31.288462;
            table[41, 1] = 28.426623;
            table[42, 1] = 25.778524;
            table[43, 1] = 23.330171;
            table[44, 1] = 21.063518;
            table[45, 1] = 18.95917;
            table[46, 1] = 14.287235;
            table[47, 1] = 10.263072;
            table[48, 1] = 6.706292;
            table[49, 1] = 3.495549;
            table[50, 1] = 0.549411;
            table[51, 1] = -2.188532;
            table[52, 1] = -4.759376;
            table[53, 1] = -7.194799;
            table[54, 1] = -9.520527;
            table[55, 1] = -11.758346;
            table[56, 1] = -13.927251;
            table[57, 1] = -16.044088;
            table[58, 1] = -18.123902;
            table[59, 1] = -20.180098;
            table[60, 1] = -22.224492;
            table[61, 1] = -24.267293;
            table[62, 1] = -26.317035;
            table[63, 1] = -28.380489;
            table[64, 1] = -30.462556;
            table[65, 1] = -32.566167;
            table[66, 1] = -34.692198;
            table[67, 1] = -36.839404;
            table[68, 1] = -39.004406;
            table[69, 1] = -41.18172;
            table[70, 1] = -43.363856;
            table[71, 1] = -45.541484;
            table[72, 1] = -47.703689;
            table[73, 1] = -49.838292;
            table[74, 1] = -51.932259;
            table[75, 1] = -53.972166;
            table[76, 1] = -55.944713;
            table[77, 1] = -57.837266;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_10t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 89.975852;
            table[1, 1] = 80.275128;
            table[2, 1] = 74.166239;
            table[3, 1] = 69.518418;
            table[4, 1] = 65.69942;
            table[5, 1] = 62.435852;
            table[6, 1] = 59.580317;
            table[7, 1] = 57.041171;
            table[8, 1] = 54.755982;
            table[9, 1] = 52.679639;
            table[10, 1] = 50.778224;
            table[11, 1] = 49.180462;
            table[12, 1] = 47.758715;
            table[13, 1] = 46.446861;
            table[14, 1] = 45.231014;
            table[15, 1] = 44.099929;
            table[16, 1] = 43.04436;
            table[17, 1] = 42.056599;
            table[18, 1] = 41.130144;
            table[19, 1] = 40.259445;
            table[20, 1] = 36.594361;
            table[21, 1] = 33.803228;
            table[22, 1] = 31.634669;
            table[23, 1] = 29.922985;
            table[24, 1] = 28.550806;
            table[25, 1] = 27.431626;
            table[26, 1] = 26.500332;
            table[27, 1] = 25.707397;
            table[28, 1] = 25.015006;
            table[29, 1] = 24.394292;
            table[30, 1] = 23.823306;
            table[31, 1] = 23.285468;
            table[32, 1] = 22.768372;
            table[33, 1] = 22.262851;
            table[34, 1] = 21.762241;
            table[35, 1] = 21.261803;
            table[36, 1] = 20.249418;
            table[37, 1] = 19.211059;
            table[38, 1] = 18.142187;
            table[39, 1] = 17.043616;
            table[40, 1] = 15.918869;
            table[41, 1] = 14.77274;
            table[42, 1] = 13.61051;
            table[43, 1] = 12.437492;
            table[44, 1] = 11.258744;
            table[45, 1] = 10.078889;
            table[46, 1] = 7.149735;
            table[47, 1] = 4.285127;
            table[48, 1] = 1.511336;
            table[49, 1] = -1.161827;
            table[50, 1] = -3.735694;
            table[51, 1] = -6.21848;
            table[52, 1] = -8.622251;
            table[53, 1] = -10.960878;
            table[54, 1] = -13.248688;
            table[55, 1] = -15.499581;
            table[56, 1] = -17.726439;
            table[57, 1] = -19.940741;
            table[58, 1] = -22.152296;
            table[59, 1] = -24.369064;
            table[60, 1] = -26.597037;
            table[61, 1] = -28.840161;
            table[62, 1] = -31.100301;
            table[63, 1] = -33.377243;
            table[64, 1] = -35.668735;
            table[65, 1] = -37.97057;
            table[66, 1] = -40.276719;
            table[67, 1] = -42.579505;
            table[68, 1] = -44.869828;
            table[69, 1] = -47.137438;
            table[70, 1] = -49.371249;
            table[71, 1] = -51.559685;
            table[72, 1] = -53.691059;
            table[73, 1] = -55.753953;
            table[74, 1] = -57.737607;
            table[75, 1] = -59.632279;
            table[76, 1] = -61.429579;
            table[77, 1] = -63.122742;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_10t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 92.181157;
            table[1, 1] = 83.090807;
            table[2, 1] = 77.529572;
            table[3, 1] = 73.354816;
            table[4, 1] = 69.920619;
            table[5, 1] = 66.95776;
            table[6, 1] = 64.332176;
            table[7, 1] = 61.967283;
            table[8, 1] = 59.813965;
            table[9, 1] = 57.837704;
            table[10, 1] = 56.012601;
            table[11, 1] = 54.363552;
            table[12, 1] = 52.946592;
            table[13, 1] = 51.629934;
            table[14, 1] = 50.401349;
            table[15, 1] = 49.250842;
            table[16, 1] = 48.170139;
            table[17, 1] = 47.152303;
            table[18, 1] = 46.191456;
            table[19, 1] = 45.282567;
            table[20, 1] = 41.382975;
            table[21, 1] = 38.309712;
            table[22, 1] = 35.835532;
            table[23, 1] = 33.811714;
            table[24, 1] = 32.132955;
            table[25, 1] = 30.720914;
            table[26, 1] = 29.515519;
            table[27, 1] = 28.4699;
            table[28, 1] = 27.547167;
            table[29, 1] = 26.718196;
            table[30, 1] = 25.960014;
            table[31, 1] = 25.254563;
            table[32, 1] = 24.587725;
            table[33, 1] = 23.948535;
            table[34, 1] = 23.32855;
            table[35, 1] = 22.721332;
            table[36, 1] = 21.527058;
            table[37, 1] = 20.340397;
            table[38, 1] = 19.148681;
            table[39, 1] = 17.946855;
            table[40, 1] = 16.734332;
            table[41, 1] = 15.513027;
            table[42, 1] = 14.286141;
            table[43, 1] = 13.057393;
            table[44, 1] = 11.830549;
            table[45, 1] = 10.609135;
            table[46, 1] = 7.598747;
            table[47, 1] = 4.67646;
            table[48, 1] = 1.860643;
            table[49, 1] = -0.844001;
            table[50, 1] = -3.442064;
            table[51, 1] = -5.943861;
            table[52, 1] = -8.362849;
            table[53, 1] = -10.713847;
            table[54, 1] = -13.011852;
            table[55, 1] = -15.271241;
            table[56, 1] = -17.505252;
            table[57, 1] = -19.725631;
            table[58, 1] = -21.942389;
            table[59, 1] = -24.163646;
            table[60, 1] = -26.395517;
            table[61, 1] = -28.642045;
            table[62, 1] = -30.905175;
            table[63, 1] = -33.184757;
            table[64, 1] = -35.478589;
            table[65, 1] = -37.78251;
            table[66, 1] = -40.090524;
            table[67, 1] = -42.394985;
            table[68, 1] = -44.686816;
            table[69, 1] = -46.955791;
            table[70, 1] = -49.190838;
            table[71, 1] = -51.3804;
            table[72, 1] = -53.5128;
            table[73, 1] = -55.576632;
            table[74, 1] = -57.561146;
            table[75, 1] = -59.456608;
            table[76, 1] = -61.254636;
            table[77, 1] = -62.94847;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_10t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.635547;
            table[1, 1] = 86.00138;
            table[2, 1] = 80.82345;
            table[3, 1] = 77.014863;
            table[4, 1] = 73.924833;
            table[5, 1] = 71.272338;
            table[6, 1] = 68.916118;
            table[7, 1] = 66.778309;
            table[8, 1] = 64.812656;
            table[9, 1] = 62.989553;
            table[10, 1] = 61.288551;
            table[11, 1] = 59.745544;
            table[12, 1] = 58.366634;
            table[13, 1] = 57.073791;
            table[14, 1] = 55.857123;
            table[15, 1] = 54.708499;
            table[16, 1] = 53.621141;
            table[17, 1] = 52.589329;
            table[18, 1] = 51.608182;
            table[19, 1] = 50.673499;
            table[20, 1] = 46.583629;
            table[21, 1] = 43.254953;
            table[22, 1] = 40.492807;
            table[23, 1] = 38.169137;
            table[24, 1] = 36.192496;
            table[25, 1] = 34.493678;
            table[26, 1] = 33.018245;
            table[27, 1] = 31.722374;
            table[28, 1] = 30.570368;
            table[29, 1] = 29.533043;
            table[30, 1] = 28.586554;
            table[31, 1] = 27.711502;
            table[32, 1] = 26.892187;
            table[33, 1] = 26.115986;
            table[34, 1] = 25.372823;
            table[35, 1] = 24.65472;
            table[36, 1] = 23.270028;
            table[37, 1] = 21.926978;
            table[38, 1] = 20.605479;
            table[39, 1] = 19.294907;
            table[40, 1] = 17.990528;
            table[41, 1] = 16.691144;
            table[42, 1] = 15.397563;
            table[43, 1] = 14.111615;
            table[44, 1] = 12.835543;
            table[45, 1] = 11.571619;
            table[46, 1] = 8.477912;
            table[47, 1] = 5.496033;
            table[48, 1] = 2.636502;
            table[49, 1] = -0.101019;
            table[50, 1] = -2.724382;
            table[51, 1] = -5.246048;
            table[52, 1] = -7.680919;
            table[53, 1] = -10.04481;
            table[54, 1] = -12.353419;
            table[55, 1] = -14.621635;
            table[56, 1] = -16.863069;
            table[57, 1] = -19.089746;
            table[58, 1] = -21.311894;
            table[59, 1] = -23.537797;
            table[60, 1] = -25.773699;
            table[61, 1] = -28.023748;
            table[62, 1] = -30.28997;
            table[63, 1] = -32.57228;
            table[64, 1] = -34.868533;
            table[65, 1] = -37.174609;
            table[66, 1] = -39.484552;
            table[67, 1] = -41.790743;
            table[68, 1] = -44.084135;
            table[69, 1] = -46.354519;
            table[70, 1] = -48.590845;
            table[71, 1] = -50.78157;
            table[72, 1] = -52.91503;
            table[73, 1] = -54.979833;
            table[74, 1] = -56.965235;
            table[75, 1] = -58.861515;
            table[76, 1] = -60.660295;
            table[77, 1] = -62.354823;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_10t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 97.384505;
            table[1, 1] = 89.207574;
            table[2, 1] = 84.350391;
            table[3, 1] = 80.83122;
            table[4, 1] = 78.021358;
            table[5, 1] = 75.640654;
            table[6, 1] = 73.542303;
            table[7, 1] = 71.642351;
            table[8, 1] = 69.890311;
            table[9, 1] = 68.254774;
            table[10, 1] = 66.71556;
            table[11, 1] = 65.285859;
            table[12, 1] = 63.988288;
            table[13, 1] = 62.759186;
            table[14, 1] = 61.590778;
            table[15, 1] = 60.476831;
            table[16, 1] = 59.412251;
            table[17, 1] = 58.392794;
            table[18, 1] = 57.414871;
            table[19, 1] = 56.475403;
            table[20, 1] = 52.271654;
            table[21, 1] = 48.733314;
            table[22, 1] = 45.712015;
            table[23, 1] = 43.106965;
            table[24, 1] = 40.843884;
            table[25, 1] = 38.864587;
            table[26, 1] = 37.121516;
            table[27, 1] = 35.574823;
            table[28, 1] = 34.19075;
            table[29, 1] = 32.94065;
            table[30, 1] = 31.800305;
            table[31, 1] = 30.749378;
            table[32, 1] = 29.770916;
            table[33, 1] = 28.850905;
            table[34, 1] = 27.977846;
            table[35, 1] = 27.142382;
            table[36, 1] = 25.555493;
            table[37, 1] = 24.046241;
            table[38, 1] = 22.587228;
            table[39, 1] = 21.162091;
            table[40, 1] = 19.761622;
            table[41, 1] = 18.381133;
            table[42, 1] = 17.018689;
            table[43, 1] = 15.67395;
            table[44, 1] = 14.347417;
            table[45, 1] = 13.039961;
            table[46, 1] = 9.860773;
            table[47, 1] = 6.817325;
            table[48, 1] = 3.912225;
            table[49, 1] = 1.140143;
            table[50, 1] = -1.51;
            table[51, 1] = -4.05281;
            table[52, 1] = -6.504647;
            table[53, 1] = -8.882349;
            table[54, 1] = -11.202343;
            table[55, 1] = -13.480047;
            table[56, 1] = -15.729469;
            table[57, 1] = -17.96293;
            table[58, 1] = -20.190886;
            table[59, 1] = -22.421798;
            table[60, 1] = -24.662049;
            table[61, 1] = -26.915895;
            table[62, 1] = -29.185452;
            table[63, 1] = -31.470707;
            table[64, 1] = -33.76957;
            table[65, 1] = -36.077973;
            table[66, 1] = -38.389996;
            table[67, 1] = -40.698056;
            table[68, 1] = -42.993131;
            table[69, 1] = -45.265037;
            table[70, 1] = -47.502744;
            table[71, 1] = -49.694724;
            table[72, 1] = -51.829329;
            table[73, 1] = -53.895179;
            table[74, 1] = -55.881541;
            table[75, 1] = -57.778703;
            table[76, 1] = -59.578294;
            table[77, 1] = -61.273572;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_10t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 100.318067;
            table[1, 1] = 92.674184;
            table[2, 1] = 88.142741;
            table[3, 1] = 84.885018;
            table[4, 1] = 82.313726;
            table[5, 1] = 80.163501;
            table[6, 1] = 78.291523;
            table[7, 1] = 76.612716;
            table[8, 1] = 75.073307;
            table[9, 1] = 73.6382;
            table[10, 1] = 72.284151;
            table[11, 1] = 70.995694;
            table[12, 1] = 69.788869;
            table[13, 1] = 68.663761;
            table[14, 1] = 67.584058;
            table[15, 1] = 66.544356;
            table[16, 1] = 65.540512;
            table[17, 1] = 64.569305;
            table[18, 1] = 63.628191;
            table[19, 1] = 62.715123;
            table[20, 1] = 58.519874;
            table[21, 1] = 54.851581;
            table[22, 1] = 51.624167;
            table[23, 1] = 48.773565;
            table[24, 1] = 46.24771;
            table[25, 1] = 44.002364;
            table[26, 1] = 41.998865;
            table[27, 1] = 40.202931;
            table[28, 1] = 38.584076;
            table[29, 1] = 37.115324;
            table[30, 1] = 35.773021;
            table[31, 1] = 34.536651;
            table[32, 1] = 33.388612;
            table[33, 1] = 32.313956;
            table[34, 1] = 31.30011;
            table[35, 1] = 30.336585;
            table[36, 1] = 28.527313;
            table[37, 1] = 26.833832;
            table[38, 1] = 25.221618;
            table[39, 1] = 23.668443;
            table[40, 1] = 22.160368;
            table[41, 1] = 20.688917;
            table[42, 1] = 19.249131;
            table[43, 1] = 17.838252;
            table[44, 1] = 16.454842;
            table[45, 1] = 15.09821;
            table[46, 1] = 11.822039;
            table[47, 1] = 8.708266;
            table[48, 1] = 5.750787;
            table[49, 1] = 2.93877;
            table[50, 1] = 0.257552;
            table[51, 1] = -2.30987;
            table[52, 1] = -4.781508;
            table[53, 1] = -7.175357;
            table[54, 1] = -9.508681;
            table[55, 1] = -11.797508;
            table[56, 1] = -14.056301;
            table[57, 1] = -16.297726;
            table[58, 1] = -18.532505;
            table[59, 1] = -20.769303;
            table[60, 1] = -23.014665;
            table[61, 1] = -25.272978;
            table[62, 1] = -27.546458;
            table[63, 1] = -29.835176;
            table[64, 1] = -32.137112;
            table[65, 1] = -34.448253;
            table[66, 1] = -36.762725;
            table[67, 1] = -39.072984;
            table[68, 1] = -41.37004;
            table[69, 1] = -43.643738;
            table[70, 1] = -45.88307;
            table[71, 1] = -48.076528;
            table[72, 1] = -50.212482;
            table[73, 1] = -52.279564;
            table[74, 1] = -54.267057;
            table[75, 1] = -56.165258;
            table[76, 1] = -57.965806;
            table[77, 1] = -59.661967;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_10t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 103.120502;
            table[1, 1] = 96.119692;
            table[2, 1] = 91.968629;
            table[3, 1] = 88.993402;
            table[4, 1] = 86.660124;
            table[5, 1] = 84.727135;
            table[6, 1] = 83.063318;
            table[7, 1] = 81.589073;
            table[8, 1] = 80.252365;
            table[9, 1] = 79.017504;
            table[10, 1] = 77.859317;
            table[11, 1] = 76.759834;
            table[12, 1] = 75.706242;
            table[13, 1] = 74.689515;
            table[14, 1] = 73.726774;
            table[15, 1] = 72.807312;
            table[16, 1] = 71.912328;
            table[17, 1] = 71.038464;
            table[18, 1] = 70.183246;
            table[19, 1] = 69.344879;
            table[20, 1] = 65.372756;
            table[21, 1] = 61.735109;
            table[22, 1] = 58.417625;
            table[23, 1] = 55.40529;
            table[24, 1] = 52.676361;
            table[25, 1] = 50.205582;
            table[26, 1] = 47.966749;
            table[27, 1] = 45.934061;
            table[28, 1] = 44.082863;
            table[29, 1] = 42.390111;
            table[30, 1] = 40.834674;
            table[31, 1] = 39.397502;
            table[32, 1] = 38.061683;
            table[33, 1] = 36.812401;
            table[34, 1] = 35.636828;
            table[35, 1] = 34.523967;
            table[36, 1] = 32.450445;
            table[37, 1] = 30.533463;
            table[38, 1] = 28.732327;
            table[39, 1] = 27.019181;
            table[40, 1] = 25.37522;
            table[41, 1] = 23.787875;
            table[42, 1] = 22.2488;
            table[43, 1] = 20.752453;
            table[44, 1] = 19.295117;
            table[45, 1] = 17.874235;
            table[46, 1] = 14.47041;
            table[47, 1] = 11.263232;
            table[48, 1] = 8.2357;
            table[49, 1] = 5.369988;
            table[50, 1] = 2.646816;
            table[51, 1] = 0.046058;
            table[52, 1] = -2.452464;
            table[53, 1] = -4.868283;
            table[54, 1] = -7.21977;
            table[55, 1] = -9.523773;
            table[56, 1] = -11.795365;
            table[57, 1] = -14.047679;
            table[58, 1] = -16.291791;
            table[59, 1] = -18.536646;
            table[60, 1] = -20.78901;
            table[61, 1] = -23.053442;
            table[62, 1] = -25.332299;
            table[63, 1] = -27.625767;
            table[64, 1] = -29.931918;
            table[65, 1] = -32.246814;
            table[66, 1] = -34.564648;
            table[67, 1] = -36.877925;
            table[68, 1] = -39.177703;
            table[69, 1] = -41.453861;
            table[70, 1] = -43.695425;
            table[71, 1] = -45.890913;
            table[72, 1] = -48.028719;
            table[73, 1] = -50.097496;
            table[74, 1] = -52.086542;
            table[75, 1] = -53.98617;
            table[76, 1] = -55.788032;
            table[77, 1] = -57.485406;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_10t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 105.242609;
            table[1, 1] = 98.857707;
            table[2, 1] = 95.095801;
            table[3, 1] = 92.41247;
            table[4, 1] = 90.320287;
            table[5, 1] = 88.600472;
            table[6, 1] = 87.135157;
            table[7, 1] = 85.853051;
            table[8, 1] = 84.707392;
            table[9, 1] = 83.665614;
            table[10, 1] = 82.703995;
            table[11, 1] = 81.80468;
            table[12, 1] = 80.953922;
            table[13, 1] = 80.141009;
            table[14, 1] = 79.357567;
            table[15, 1] = 78.59709;
            table[16, 1] = 77.861298;
            table[17, 1] = 77.158592;
            table[18, 1] = 76.469435;
            table[19, 1] = 75.790931;
            table[20, 1] = 72.496098;
            table[21, 1] = 69.303602;
            table[22, 1] = 66.223975;
            table[23, 1] = 63.331892;
            table[24, 1] = 60.746502;
            table[25, 1] = 58.301307;
            table[26, 1] = 55.95751;
            table[27, 1] = 53.683883;
            table[28, 1] = 51.458278;
            table[29, 1] = 49.440598;
            table[30, 1] = 47.669995;
            table[31, 1] = 46.01731;
            table[32, 1] = 44.469453;
            table[33, 1] = 43.014338;
            table[34, 1] = 41.641004;
            table[35, 1] = 40.33964;
            table[36, 1] = 37.919162;
            table[37, 1] = 35.695782;
            table[38, 1] = 33.626675;
            table[39, 1] = 31.680329;
            table[40, 1] = 29.833804;
            table[41, 1] = 28.070481;
            table[42, 1] = 26.378297;
            table[43, 1] = 24.748424;
            table[44, 1] = 23.174305;
            table[45, 1] = 21.650956;
            table[46, 1] = 18.040844;
            table[47, 1] = 14.680446;
            table[48, 1] = 11.536734;
            table[49, 1] = 8.581233;
            table[50, 1] = 5.787464;
            table[51, 1] = 3.130334;
            table[52, 1] = 0.586172;
            table[53, 1] = -1.867058;
            table[54, 1] = -4.249556;
            table[55, 1] = -6.579523;
            table[56, 1] = -8.873053;
            table[57, 1] = -11.144055;
            table[58, 1] = -13.404208;
            table[59, 1] = -15.662927;
            table[60, 1] = -17.927348;
            table[61, 1] = -20.202326;
            table[62, 1] = -22.490458;
            table[63, 1] = -24.792123;
            table[64, 1] = -27.105552;
            table[65, 1] = -29.426937;
            table[66, 1] = -31.750579;
            table[67, 1] = -34.069076;
            table[68, 1] = -36.37356;
            table[69, 1] = -38.653975;
            table[70, 1] = -40.899401;
            table[71, 1] = -43.098405;
            table[72, 1] = -45.239418;
            table[73, 1] = -47.311128;
            table[74, 1] = -49.302865;
            table[75, 1] = -51.204965;
            table[76, 1] = -53.009106;
            table[77, 1] = -54.708583;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_10t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.356584;
            table[1, 1] = 100.284591;
            table[2, 1] = 96.730553;
            table[3, 1] = 94.207699;
            table[4, 1] = 92.24977;
            table[5, 1] = 90.648882;
            table[6, 1] = 89.29397;
            table[7, 1] = 88.118573;
            table[8, 1] = 87.079662;
            table[9, 1] = 86.147696;
            table[10, 1] = 85.301451;
            table[11, 1] = 84.525108;
            table[12, 1] = 83.806508;
            table[13, 1] = 83.13607;
            table[14, 1] = 82.506076;
            table[15, 1] = 81.9102;
            table[16, 1] = 81.343177;
            table[17, 1] = 80.800968;
            table[18, 1] = 80.288805;
            table[19, 1] = 79.796381;
            table[20, 1] = 77.546374;
            table[21, 1] = 75.493831;
            table[22, 1] = 73.502122;
            table[23, 1] = 71.501235;
            table[24, 1] = 69.469027;
            table[25, 1] = 67.414214;
            table[26, 1] = 65.359589;
            table[27, 1] = 63.38176;
            table[28, 1] = 61.540308;
            table[29, 1] = 59.705222;
            table[30, 1] = 57.866225;
            table[31, 1] = 56.017025;
            table[32, 1] = 54.155361;
            table[33, 1] = 52.390815;
            table[34, 1] = 50.804857;
            table[35, 1] = 49.281879;
            table[36, 1] = 46.410035;
            table[37, 1] = 43.745347;
            table[38, 1] = 41.259811;
            table[39, 1] = 38.928991;
            table[40, 1] = 36.732326;
            table[41, 1] = 34.652843;
            table[42, 1] = 32.676644;
            table[43, 1] = 30.792348;
            table[44, 1] = 28.990591;
            table[45, 1] = 27.263588;
            table[46, 1] = 23.232411;
            table[47, 1] = 19.549704;
            table[48, 1] = 16.156168;
            table[49, 1] = 13.004383;
            table[50, 1] = 10.05432;
            table[51, 1] = 7.271147;
            table[52, 1] = 4.624129;
            table[53, 1] = 2.086046;
            table[54, 1] = -0.367155;
            table[55, 1] = -2.756578;
            table[56, 1] = -5.100524;
            table[57, 1] = -7.414609;
            table[58, 1] = -9.711837;
            table[59, 1] = -12.00267;
            table[60, 1] = -14.295075;
            table[61, 1] = -16.594573;
            table[62, 1] = -18.904301;
            table[63, 1] = -21.225077;
            table[64, 1] = -23.555492;
            table[65, 1] = -25.892039;
            table[66, 1] = -28.229266;
            table[67, 1] = -30.55998;
            table[68, 1] = -32.875486;
            table[69, 1] = -35.165879;
            table[70, 1] = -37.420364;
            table[71, 1] = -39.627615;
            table[72, 1] = -41.776158;
            table[73, 1] = -43.85476;
            table[74, 1] = -45.852818;
            table[75, 1] = -47.760732;
            table[76, 1] = -49.57023;
            table[77, 1] = -51.274652;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_1t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 89.975852;
            table[1, 1] = 80.275128;
            table[2, 1] = 74.166239;
            table[3, 1] = 69.518418;
            table[4, 1] = 65.69942;
            table[5, 1] = 62.435852;
            table[6, 1] = 59.647654;
            table[7, 1] = 57.462246;
            table[8, 1] = 55.541032;
            table[9, 1] = 53.83052;
            table[10, 1] = 52.292451;
            table[11, 1] = 50.8985;
            table[12, 1] = 49.627082;
            table[13, 1] = 48.461338;
            table[14, 1] = 47.387822;
            table[15, 1] = 46.395609;
            table[16, 1] = 45.475678;
            table[17, 1] = 44.620473;
            table[18, 1] = 43.823589;
            table[19, 1] = 43.079529;
            table[20, 1] = 40.004022;
            table[21, 1] = 37.729065;
            table[22, 1] = 36.003919;
            table[23, 1] = 34.667242;
            table[24, 1] = 33.6089;
            table[25, 1] = 32.751013;
            table[26, 1] = 32.03729;
            table[27, 1] = 31.426471;
            table[28, 1] = 30.88806;
            table[29, 1] = 30.399414;
            table[30, 1] = 29.943719;
            table[31, 1] = 29.508545;
            table[32, 1] = 29.084791;
            table[33, 1] = 28.665906;
            table[34, 1] = 28.247298;
            table[35, 1] = 27.825863;
            table[36, 1] = 26.967422;
            table[37, 1] = 26.083249;
            table[38, 1] = 25.17272;
            table[39, 1] = 24.237894;
            table[40, 1] = 23.281568;
            table[41, 1] = 22.306613;
            table[42, 1] = 21.315906;
            table[43, 1] = 20.3124;
            table[44, 1] = 19.299142;
            table[45, 1] = 18.279204;
            table[46, 1] = 15.719024;
            table[47, 1] = 13.173425;
            table[48, 1] = 10.666889;
            table[49, 1] = 8.211162;
            table[50, 1] = 5.808968;
            table[51, 1] = 3.457314;
            table[52, 1] = 1.149821;
            table[53, 1] = -1.121717;
            table[54, 1] = -3.366291;
            table[55, 1] = -5.59296;
            table[56, 1] = -7.810359;
            table[57, 1] = -10.02634;
            table[58, 1] = -12.247722;
            table[59, 1] = -14.480101;
            table[60, 1] = -16.727723;
            table[61, 1] = -18.993385;
            table[62, 1] = -21.27838;
            table[63, 1] = -23.582471;
            table[64, 1] = -25.903895;
            table[65, 1] = -28.2394;
            table[66, 1] = -30.584314;
            table[67, 1] = -32.932648;
            table[68, 1] = -35.277229;
            table[69, 1] = -37.609864;
            table[70, 1] = -39.921541;
            table[71, 1] = -42.202651;
            table[72, 1] = -44.443227;
            table[73, 1] = -46.633212;
            table[74, 1] = -48.762718;
            table[75, 1] = -50.822302;
            table[76, 1] = -52.803212;
            table[77, 1] = -54.697636;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_1t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 92.181157;
            table[1, 1] = 83.090807;
            table[2, 1] = 77.529572;
            table[3, 1] = 73.354816;
            table[4, 1] = 69.920619;
            table[5, 1] = 66.95776;
            table[6, 1] = 64.332176;
            table[7, 1] = 62.166257;
            table[8, 1] = 60.276;
            table[9, 1] = 58.579589;
            table[10, 1] = 57.043072;
            table[11, 1] = 55.64088;
            table[12, 1] = 54.353265;
            table[13, 1] = 53.164646;
            table[14, 1] = 52.062497;
            table[15, 1] = 51.036589;
            table[16, 1] = 50.078448;
            table[17, 1] = 49.180967;
            table[18, 1] = 48.33812;
            table[19, 1] = 47.544752;
            table[20, 1] = 44.18257;
            table[21, 1] = 41.576886;
            table[22, 1] = 39.503607;
            table[23, 1] = 37.822196;
            table[24, 1] = 36.436929;
            table[25, 1] = 35.278415;
            table[26, 1] = 34.294187;
            table[27, 1] = 33.443605;
            table[28, 1] = 32.694847;
            table[29, 1] = 32.022944;
            table[30, 1] = 31.408354;
            table[31, 1] = 30.835873;
            table[32, 1] = 30.293753;
            table[33, 1] = 29.772993;
            table[34, 1] = 29.26676;
            table[35, 1] = 28.769915;
            table[36, 1] = 27.790136;
            table[37, 1] = 26.813946;
            table[38, 1] = 25.831456;
            table[39, 1] = 24.8384;
            table[40, 1] = 23.833682;
            table[41, 1] = 22.817977;
            table[42, 1] = 21.792939;
            table[43, 1] = 20.760721;
            table[44, 1] = 19.723692;
            table[45, 1] = 18.68423;
            table[46, 1] = 16.089282;
            table[47, 1] = 13.521688;
            table[48, 1] = 10.999931;
            table[49, 1] = 8.532776;
            table[50, 1] = 6.121579;
            table[51, 1] = 3.762674;
            table[52, 1] = 1.449278;
            table[53, 1] = -0.827097;
            table[54, 1] = -3.075659;
            table[55, 1] = -5.305638;
            table[56, 1] = -7.525801;
            table[57, 1] = -9.744107;
            table[58, 1] = -11.967459;
            table[59, 1] = -14.201519;
            table[60, 1] = -16.450584;
            table[61, 1] = -18.717492;
            table[62, 1] = -21.00357;
            table[63, 1] = -23.308608;
            table[64, 1] = -25.630862;
            table[65, 1] = -27.9671;
            table[66, 1] = -30.312665;
            table[67, 1] = -32.661577;
            table[68, 1] = -35.006674;
            table[69, 1] = -37.339773;
            table[70, 1] = -39.651867;
            table[71, 1] = -41.933352;
            table[72, 1] = -44.174269;
            table[73, 1] = -46.364563;
            table[74, 1] = -48.494351;
            table[75, 1] = -50.554192;
            table[76, 1] = -52.535337;
            table[77, 1] = -54.429976;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_1t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.635547;
            table[1, 1] = 86.080425;
            table[2, 1] = 80.898262;
            table[3, 1] = 77.049171;
            table[4, 1] = 73.941921;
            table[5, 1] = 71.318085;
            table[6, 1] = 69.040039;
            table[7, 1] = 67.02449;
            table[8, 1] = 65.216427;
            table[9, 1] = 63.577213;
            table[10, 1] = 62.07846;
            table[11, 1] = 60.698594;
            table[12, 1] = 59.420776;
            table[13, 1] = 58.231577;
            table[14, 1] = 57.120101;
            table[15, 1] = 56.077371;
            table[16, 1] = 55.095904;
            table[17, 1] = 54.16939;
            table[18, 1] = 53.292466;
            table[19, 1] = 52.460531;
            table[20, 1] = 48.853317;
            table[21, 1] = 45.945739;
            table[22, 1] = 43.545141;
            table[23, 1] = 41.534063;
            table[24, 1] = 39.832721;
            table[25, 1] = 38.381453;
            table[26, 1] = 37.132624;
            table[27, 1] = 36.046955;
            table[28, 1] = 35.091781;
            table[29, 1] = 34.240037;
            table[30, 1] = 33.469502;
            table[31, 1] = 32.762117;
            table[32, 1] = 32.103352;
            table[33, 1] = 31.481622;
            table[34, 1] = 30.887771;
            table[35, 1] = 30.314613;
            table[36, 1] = 29.20929;
            table[37, 1] = 28.134703;
            table[38, 1] = 27.073078;
            table[39, 1] = 26.014819;
            table[40, 1] = 24.95533;
            table[41, 1] = 23.893014;
            table[42, 1] = 22.828023;
            table[43, 1] = 21.761467;
            table[44, 1] = 20.694926;
            table[45, 1] = 19.630133;
            table[46, 1] = 16.986612;
            table[47, 1] = 14.38598;
            table[48, 1] = 11.841429;
            table[49, 1] = 9.35813;
            table[50, 1] = 6.935144;
            table[51, 1] = 4.567366;
            table[52, 1] = 2.247115;
            table[53, 1] = -0.034678;
            table[54, 1] = -2.287599;
            table[55, 1] = -4.52114;
            table[56, 1] = -6.744251;
            table[57, 1] = -8.965024;
            table[58, 1] = -11.190458;
            table[59, 1] = -13.426291;
            table[60, 1] = -15.676877;
            table[61, 1] = -17.945099;
            table[62, 1] = -20.232318;
            table[63, 1] = -22.538352;
            table[64, 1] = -24.861482;
            table[65, 1] = -27.198493;
            table[66, 1] = -29.544742;
            table[67, 1] = -31.894264;
            table[68, 1] = -34.239905;
            table[69, 1] = -36.573491;
            table[70, 1] = -38.886025;
            table[71, 1] = -41.167907;
            table[72, 1] = -43.409183;
            table[73, 1] = -45.599802;
            table[74, 1] = -47.729887;
            table[75, 1] = -49.789997;
            table[76, 1] = -51.771391;
            table[77, 1] = -53.666257;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_1t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 97.384505;
            table[1, 1] = 89.406937;
            table[2, 1] = 84.62303;
            table[3, 1] = 81.109918;
            table[4, 1] = 78.287085;
            table[5, 1] = 75.900643;
            table[6, 1] = 73.817893;
            table[7, 1] = 71.960987;
            table[8, 1] = 70.280268;
            table[9, 1] = 68.741937;
            table[10, 1] = 67.321774;
            table[11, 1] = 66.001691;
            table[12, 1] = 64.767719;
            table[13, 1] = 63.608779;
            table[14, 1] = 62.515892;
            table[15, 1] = 61.481655;
            table[16, 1] = 60.499881;
            table[17, 1] = 59.565338;
            table[18, 1] = 58.673567;
            table[19, 1] = 57.820736;
            table[20, 1] = 54.038764;
            table[21, 1] = 50.877923;
            table[22, 1] = 48.183124;
            table[23, 1] = 45.863298;
            table[24, 1] = 43.857111;
            table[25, 1] = 42.116866;
            table[26, 1] = 40.601733;
            table[27, 1] = 39.275443;
            table[28, 1] = 38.105778;
            table[29, 1] = 37.06455;
            table[30, 1] = 36.12756;
            table[31, 1] = 35.274397;
            table[32, 1] = 34.488109;
            table[33, 1] = 33.75479;
            table[34, 1] = 33.063149;
            table[35, 1] = 32.404103;
            table[36, 1] = 31.156258;
            table[37, 1] = 29.969557;
            table[38, 1] = 28.818411;
            table[39, 1] = 27.68763;
            table[40, 1] = 26.56863;
            table[41, 1] = 25.456953;
            table[42, 1] = 24.350662;
            table[43, 1] = 23.249315;
            table[44, 1] = 22.1533;
            table[45, 1] = 21.063422;
            table[46, 1] = 18.371853;
            table[47, 1] = 15.737989;
            table[48, 1] = 13.16986;
            table[49, 1] = 10.669394;
            table[50, 1] = 8.233589;
            table[51, 1] = 5.856015;
            table[52, 1] = 3.528121;
            table[53, 1] = 1.240258;
            table[54, 1] = -1.017562;
            table[55, 1] = -3.255109;
            table[56, 1] = -5.481536;
            table[57, 1] = -7.705083;
            table[58, 1] = -9.932858;
            table[59, 1] = -12.170685;
            table[60, 1] = -14.42298;
            table[61, 1] = -16.692677;
            table[62, 1] = -18.981178;
            table[63, 1] = -21.288332;
            table[64, 1] = -23.612446;
            table[65, 1] = -25.950324;
            table[66, 1] = -28.297342;
            table[67, 1] = -30.647548;
            table[68, 1] = -32.993801;
            table[69, 1] = -35.327936;
            table[70, 1] = -37.640962;
            table[71, 1] = -39.923289;
            table[72, 1] = -42.164968;
            table[73, 1] = -44.355954;
            table[74, 1] = -46.486371;
            table[75, 1] = -48.546786;
            table[76, 1] = -50.528457;
            table[77, 1] = -52.423578;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_1t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 100.318067;
            table[1, 1] = 92.913003;
            table[2, 1] = 88.49469;
            table[3, 1] = 85.279152;
            table[4, 1] = 82.713969;
            table[5, 1] = 80.554537;
            table[6, 1] = 78.671937;
            table[7, 1] = 76.990495;
            table[8, 1] = 75.462348;
            table[9, 1] = 74.055474;
            table[10, 1] = 72.747458;
            table[11, 1] = 71.522;
            table[12, 1] = 70.36685;
            table[13, 1] = 69.272534;
            table[14, 1] = 68.231541;
            table[15, 1] = 67.23779;
            table[16, 1] = 66.286269;
            table[17, 1] = 65.37279;
            table[18, 1] = 64.493811;
            table[19, 1] = 63.646313;
            table[20, 1] = 59.801572;
            table[21, 1] = 56.472067;
            table[22, 1] = 53.544384;
            table[23, 1] = 50.956735;
            table[24, 1] = 48.669187;
            table[25, 1] = 46.649283;
            table[26, 1] = 44.866342;
            table[27, 1] = 43.290189;
            table[28, 1] = 41.891654;
            table[29, 1] = 40.643476;
            table[30, 1] = 39.521002;
            table[31, 1] = 38.50256;
            table[32, 1] = 37.56951;
            table[33, 1] = 36.706107;
            table[34, 1] = 35.899221;
            table[35, 1] = 35.138028;
            table[36, 1] = 33.718969;
            table[37, 1] = 32.396438;
            table[38, 1] = 31.136527;
            table[39, 1] = 29.917784;
            table[40, 1] = 28.726947;
            table[41, 1] = 27.556061;
            table[42, 1] = 26.400556;
            table[43, 1] = 25.257985;
            table[44, 1] = 24.127195;
            table[45, 1] = 23.007794;
            table[46, 1] = 20.259655;
            table[47, 1] = 17.586377;
            table[48, 1] = 14.98995;
            table[49, 1] = 12.468614;
            table[50, 1] = 10.01705;
            table[51, 1] = 7.627323;
            table[52, 1] = 5.289883;
            table[53, 1] = 2.994401;
            table[54, 1] = 0.730413;
            table[55, 1] = -1.512192;
            table[56, 1] = -3.742811;
            table[57, 1] = -5.969867;
            table[58, 1] = -8.200607;
            table[59, 1] = -10.440958;
            table[60, 1] = -12.695419;
            table[61, 1] = -14.966986;
            table[62, 1] = -17.257112;
            table[63, 1] = -19.565685;
            table[64, 1] = -21.891045;
            table[65, 1] = -24.230024;
            table[66, 1] = -26.578017;
            table[67, 1] = -28.929091;
            table[68, 1] = -31.276118;
            table[69, 1] = -33.610948;
            table[70, 1] = -35.9246;
            table[71, 1] = -38.207492;
            table[72, 1] = -40.449682;
            table[73, 1] = -42.641131;
            table[74, 1] = -44.771972;
            table[75, 1] = -46.832772;
            table[76, 1] = -48.814796;
            table[77, 1] = -50.71024;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_1t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 103.120502;
            table[1, 1] = 96.330851;
            table[2, 1] = 92.297846;
            table[3, 1] = 89.384151;
            table[4, 1] = 87.076855;
            table[5, 1] = 85.14712;
            table[6, 1] = 83.473142;
            table[7, 1] = 81.982671;
            table[8, 1] = 80.629604;
            table[9, 1] = 79.382942;
            table[10, 1] = 78.220996;
            table[11, 1] = 77.128102;
            table[12, 1] = 76.092652;
            table[13, 1] = 75.105837;
            table[14, 1] = 74.160837;
            table[15, 1] = 73.252274;
            table[16, 1] = 72.375833;
            table[17, 1] = 71.528001;
            table[18, 1] = 70.70588;
            table[19, 1] = 69.907057;
            table[20, 1] = 66.201213;
            table[21, 1] = 62.874054;
            table[22, 1] = 59.850246;
            table[23, 1] = 57.09645;
            table[24, 1] = 54.595855;
            table[25, 1] = 52.335108;
            table[26, 1] = 50.298836;
            table[27, 1] = 48.468469;
            table[28, 1] = 46.823056;
            table[29, 1] = 45.340683;
            table[30, 1] = 43.999787;
            table[31, 1] = 42.780108;
            table[32, 1] = 41.66327;
            table[33, 1] = 40.63303;
            table[34, 1] = 39.675312;
            table[35, 1] = 38.778103;
            table[36, 1] = 37.126317;
            table[37, 1] = 35.615155;
            table[38, 1] = 34.201868;
            table[39, 1] = 32.857738;
            table[40, 1] = 31.563722;
            table[41, 1] = 30.307335;
            table[42, 1] = 29.080475;
            table[43, 1] = 27.877941;
            table[44, 1] = 26.696425;
            table[45, 1] = 25.533834;
            table[46, 1] = 22.702545;
            table[47, 1] = 19.970893;
            table[48, 1] = 17.332232;
            table[49, 1] = 14.779532;
            table[50, 1] = 12.30414;
            table[51, 1] = 9.895948;
            table[52, 1] = 7.543949;
            table[53, 1] = 5.236812;
            table[54, 1] = 2.963367;
            table[55, 1] = 0.712997;
            table[56, 1] = -1.524068;
            table[57, 1] = -3.756526;
            table[58, 1] = -5.991833;
            table[59, 1] = -8.236075;
            table[60, 1] = -10.493874;
            table[61, 1] = -12.768326;
            table[62, 1] = -15.060958;
            table[63, 1] = -17.371722;
            table[64, 1] = -19.699005;
            table[65, 1] = -22.039682;
            table[66, 1] = -24.38918;
            table[67, 1] = -26.741594;
            table[68, 1] = -29.089819;
            table[69, 1] = -31.425722;
            table[70, 1] = -33.74034;
            table[71, 1] = -36.024104;
            table[72, 1] = -38.267083;
            table[73, 1] = -40.45925;
            table[74, 1] = -42.590743;
            table[75, 1] = -44.652138;
            table[76, 1] = -46.634707;
            table[77, 1] = -48.530651;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_1t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 105.242609;
            table[1, 1] = 98.981402;
            table[2, 1] = 95.296115;
            table[3, 1] = 92.660434;
            table[4, 1] = 90.595743;
            table[5, 1] = 88.888396;
            table[6, 1] = 87.424232;
            table[7, 1] = 86.135039;
            table[8, 1] = 84.976828;
            table[9, 1] = 83.919622;
            table[10, 1] = 82.942125;
            table[11, 1] = 82.028717;
            table[12, 1] = 81.167645;
            table[13, 1] = 80.349877;
            table[14, 1] = 79.568356;
            table[15, 1] = 78.817484;
            table[16, 1] = 78.092762;
            table[17, 1] = 77.390535;
            table[18, 1] = 76.707805;
            table[19, 1] = 76.042091;
            table[20, 1] = 72.907005;
            table[21, 1] = 69.995061;
            table[22, 1] = 67.238854;
            table[23, 1] = 64.618607;
            table[24, 1] = 62.136332;
            table[25, 1] = 59.800606;
            table[26, 1] = 57.618438;
            table[27, 1] = 55.592001;
            table[28, 1] = 53.718228;
            table[29, 1] = 51.989735;
            table[30, 1] = 50.396156;
            table[31, 1] = 48.925447;
            table[32, 1] = 47.564941;
            table[33, 1] = 46.302112;
            table[34, 1] = 45.125092;
            table[35, 1] = 44.022964;
            table[36, 1] = 42.005224;
            table[37, 1] = 40.183615;
            table[38, 1] = 38.509019;
            table[39, 1] = 36.945474;
            table[40, 1] = 35.467073;
            table[41, 1] = 34.05531;
            table[42, 1] = 32.697007;
            table[43, 1] = 31.38277;
            table[44, 1] = 30.105863;
            table[45, 1] = 28.861406;
            table[46, 1] = 25.870763;
            table[47, 1] = 23.025809;
            table[48, 1] = 20.30439;
            table[49, 1] = 17.689778;
            table[50, 1] = 15.167082;
            table[51, 1] = 12.722071;
            table[52, 1] = 10.340942;
            table[53, 1] = 8.010419;
            table[54, 1] = 5.717957;
            table[55, 1] = 3.451943;
            table[56, 1] = 1.201875;
            table[57, 1] = -1.041493;
            table[58, 1] = -3.286031;
            table[59, 1] = -5.538144;
            table[60, 1] = -7.802702;
            table[61, 1] = -10.082996;
            table[62, 1] = -12.380708;
            table[63, 1] = -14.695912;
            table[64, 1] = -17.027098;
            table[65, 1] = -19.371219;
            table[66, 1] = -21.723771;
            table[67, 1] = -24.078904;
            table[68, 1] = -26.42956;
            table[69, 1] = -28.767643;
            table[70, 1] = -31.084223;
            table[71, 1] = -33.369758;
            table[72, 1] = -35.614342;
            table[73, 1] = -37.807965;
            table[74, 1] = -39.940784;
            table[75, 1] = -42.00339;
            table[76, 1] = -43.987066;
            table[77, 1] = -45.884026;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_100m_1t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.356584;
            table[1, 1] = 100.326495;
            table[2, 1] = 96.796558;
            table[3, 1] = 94.288663;
            table[4, 1] = 92.339713;
            table[5, 1] = 90.743377;
            table[6, 1] = 89.389552;
            table[7, 1] = 88.212494;
            table[8, 1] = 87.16978;
            table[9, 1] = 86.232441;
            table[10, 1] = 85.37981;
            table[11, 1] = 84.596634;
            table[12, 1] = 83.871334;
            table[13, 1] = 83.194915;
            table[14, 1] = 82.56025;
            table[15, 1] = 81.961599;
            table[16, 1] = 81.394265;
            table[17, 1] = 80.854358;
            table[18, 1] = 80.338611;
            table[19, 1] = 79.844255;
            table[20, 1] = 77.620202;
            table[21, 1] = 75.675244;
            table[22, 1] = 73.887009;
            table[23, 1] = 72.175662;
            table[24, 1] = 70.488647;
            table[25, 1] = 68.795896;
            table[26, 1] = 67.086869;
            table[27, 1] = 65.366085;
            table[28, 1] = 63.647122;
            table[29, 1] = 61.946772;
            table[30, 1] = 60.28081;
            table[31, 1] = 58.661767;
            table[32, 1] = 57.098278;
            table[33, 1] = 55.595357;
            table[34, 1] = 54.155055;
            table[35, 1] = 52.777204;
            table[36, 1] = 50.200945;
            table[37, 1] = 47.843002;
            table[38, 1] = 45.67433;
            table[39, 1] = 43.666446;
            table[40, 1] = 41.79402;
            table[41, 1] = 40.035628;
            table[42, 1] = 38.373649;
            table[43, 1] = 36.793793;
            table[44, 1] = 35.284539;
            table[45, 1] = 33.836602;
            table[46, 1] = 30.438996;
            table[47, 1] = 27.295843;
            table[48, 1] = 24.351903;
            table[49, 1] = 21.568192;
            table[50, 1] = 18.914767;
            table[51, 1] = 16.367081;
            table[52, 1] = 13.904139;
            table[53, 1] = 11.507572;
            table[54, 1] = 9.161162;
            table[55, 1] = 6.850605;
            table[56, 1] = 4.563402;
            table[57, 1] = 2.2888;
            table[58, 1] = 0.017777;
            table[59, 1] = -2.25696;
            table[60, 1] = -4.540977;
            table[61, 1] = -6.83811;
            table[62, 1] = -9.150479;
            table[63, 1] = -11.47851;
            table[64, 1] = -13.820975;
            table[65, 1] = -16.175062;
            table[66, 1] = -18.536457;
            table[67, 1] = -20.899468;
            table[68, 1] = -23.257168;
            table[69, 1] = -25.601573;
            table[70, 1] = -27.923845;
            table[71, 1] = -30.21452;
            table[72, 1] = -32.46376;
            table[73, 1] = -34.661612;
            table[74, 1] = -36.798284;
            table[75, 1] = -38.864407;
            table[76, 1] = -40.851302;
            table[77, 1] = -42.751214;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }

        private double Get_sea_100m_50t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 97.930628;
            table[1, 1] = 88.378741;
            table[2, 1] = 82.64764;
            table[3, 1] = 78.482409;
            table[4, 1] = 75.16738;
            table[5, 1] = 72.383643;
            table[6, 1] = 69.96179;
            table[7, 1] = 67.801424;
            table[8, 1] = 65.838428;
            table[9, 1] = 64.029568;
            table[10, 1] = 62.344477;
            table[11, 1] = 60.761155;
            table[12, 1] = 59.263201;
            table[13, 1] = 57.838251;
            table[14, 1] = 56.476738;
            table[15, 1] = 55.171205;
            table[16, 1] = 53.915772;
            table[17, 1] = 52.70576;
            table[18, 1] = 51.53742;
            table[19, 1] = 50.407723;
            table[20, 1] = 45.287453;
            table[21, 1] = 40.876789;
            table[22, 1] = 37.107846;
            table[23, 1] = 33.911983;
            table[24, 1] = 31.199864;
            table[25, 1] = 28.941001;
            table[26, 1] = 27.041944;
            table[27, 1] = 25.433073;
            table[28, 1] = 24.051738;
            table[29, 1] = 22.844258;
            table[30, 1] = 21.766544;
            table[31, 1] = 20.783736;
            table[32, 1] = 19.869209;
            table[33, 1] = 19.003265;
            table[34, 1] = 18.171781;
            table[35, 1] = 17.364893;
            table[36, 1] = 15.800399;
            table[37, 1] = 14.279588;
            table[38, 1] = 12.790971;
            table[39, 1] = 11.331795;
            table[40, 1] = 9.903311;
            table[41, 1] = 8.508394;
            table[42, 1] = 7.150227;
            table[43, 1] = 5.831201;
            table[44, 1] = 4.552695;
            table[45, 1] = 3.314739;
            table[46, 1] = 0.387266;
            table[47, 1] = -2.33462;
            table[48, 1] = -4.898166;
            table[49, 1] = -7.34379;
            table[50, 1] = -9.700756;
            table[51, 1] = -11.988549;
            table[52, 1] = -14.219746;
            table[53, 1] = -16.402765;
            table[54, 1] = -18.54398;
            table[55, 1] = -20.649106;
            table[56, 1] = -22.723917;
            table[57, 1] = -24.77449;
            table[58, 1] = -26.807128;
            table[59, 1] = -28.828138;
            table[60, 1] = -30.843561;
            table[61, 1] = -32.858874;
            table[62, 1] = -34.878818;
            table[63, 1] = -36.907199;
            table[64, 1] = -38.946777;
            table[65, 1] = -40.9992;
            table[66, 1] = -43.064971;
            table[67, 1] = -45.143452;
            table[68, 1] = -47.232892;
            table[69, 1] = -49.330485;
            table[70, 1] = -51.43244;
            table[71, 1] = -53.534076;
            table[72, 1] = -55.629935;
            table[73, 1] = -57.713903;
            table[74, 1] = -59.779354;
            table[75, 1] = -61.819301;
            table[76, 1] = -63.826558;
            table[77, 1] = -65.793903;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_50t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 102.26272;
            table[1, 1] = 92.571754;
            table[2, 1] = 86.624759;
            table[3, 1] = 82.298446;
            table[4, 1] = 78.869361;
            table[5, 1] = 76.007063;
            table[6, 1] = 73.533803;
            table[7, 1] = 71.343369;
            table[8, 1] = 69.36748;
            table[9, 1] = 67.559808;
            table[10, 1] = 65.8876;
            table[11, 1] = 64.326946;
            table[12, 1] = 62.859889;
            table[13, 1] = 61.472724;
            table[14, 1] = 60.154735;
            table[15, 1] = 58.897449;
            table[16, 1] = 57.694068;
            table[17, 1] = 56.539078;
            table[18, 1] = 55.42796;
            table[19, 1] = 54.356973;
            table[20, 1] = 49.526882;
            table[21, 1] = 45.365228;
            table[22, 1] = 41.758547;
            table[23, 1] = 38.61902;
            table[24, 1] = 35.862683;
            table[25, 1] = 33.460053;
            table[26, 1] = 31.343442;
            table[27, 1] = 29.465351;
            table[28, 1] = 27.783982;
            table[29, 1] = 26.263328;
            table[30, 1] = 24.87306;
            table[31, 1] = 23.588188;
            table[32, 1] = 22.388552;
            table[33, 1] = 21.258181;
            table[34, 1] = 20.18463;
            table[35, 1] = 19.158297;
            table[36, 1] = 17.219614;
            table[37, 1] = 15.401692;
            table[38, 1] = 13.680572;
            table[39, 1] = 12.041692;
            table[40, 1] = 10.475973;
            table[41, 1] = 8.977535;
            table[42, 1] = 7.542273;
            table[43, 1] = 6.166678;
            table[44, 1] = 4.847417;
            table[45, 1] = 3.580865;
            table[46, 1] = 0.618384;
            table[47, 1] = -2.106444;
            table[48, 1] = -4.655817;
            table[49, 1] = -7.079099;
            table[50, 1] = -9.41126;
            table[51, 1] = -11.675186;
            table[52, 1] = -13.88514;
            table[53, 1] = -16.050098;
            table[54, 1] = -18.176368;
            table[55, 1] = -20.26931;
            table[56, 1] = -22.334261;
            table[57, 1] = -24.376873;
            table[58, 1] = -26.403077;
            table[59, 1] = -28.418874;
            table[60, 1] = -30.430052;
            table[61, 1] = -32.441898;
            table[62, 1] = -34.458994;
            table[63, 1] = -36.485025;
            table[64, 1] = -38.522656;
            table[65, 1] = -40.573458;
            table[66, 1] = -42.637873;
            table[67, 1] = -44.715216;
            table[68, 1] = -46.803698;
            table[69, 1] = -48.900481;
            table[70, 1] = -51.001748;
            table[71, 1] = -53.1028;
            table[72, 1] = -55.198158;
            table[73, 1] = -57.281697;
            table[74, 1] = -59.34678;
            table[75, 1] = -61.386409;
            table[76, 1] = -63.39339;
            table[77, 1] = -65.360496;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_50t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 105.610792;
            table[1, 1] = 96.979598;
            table[2, 1] = 91.136248;
            table[3, 1] = 86.746072;
            table[4, 1] = 83.233852;
            table[5, 1] = 80.3;
            table[6, 1] = 77.77251;
            table[7, 1] = 75.544787;
            table[8, 1] = 73.546705;
            table[9, 1] = 71.729898;
            table[10, 1] = 70.059727;
            table[11, 1] = 68.510613;
            table[12, 1] = 67.063156;
            table[13, 1] = 65.70237;
            table[14, 1] = 64.416422;
            table[15, 1] = 63.19584;
            table[16, 1] = 62.032931;
            table[17, 1] = 60.921372;
            table[18, 1] = 59.855907;
            table[19, 1] = 58.832125;
            table[20, 1] = 54.23816;
            table[21, 1] = 50.281227;
            table[22, 1] = 46.809554;
            table[23, 1] = 43.719299;
            table[24, 1] = 40.929719;
            table[25, 1] = 38.410005;
            table[26, 1] = 36.111123;
            table[27, 1] = 34.001631;
            table[28, 1] = 32.055568;
            table[29, 1] = 30.251099;
            table[30, 1] = 28.56968;
            table[31, 1] = 26.995505;
            table[32, 1] = 25.515122;
            table[33, 1] = 24.117114;
            table[34, 1] = 22.791849;
            table[35, 1] = 21.531214;
            table[36, 1] = 19.177657;
            table[37, 1] = 17.01386;
            table[38, 1] = 15.009302;
            table[39, 1] = 13.14159;
            table[40, 1] = 11.393791;
            table[41, 1] = 9.752607;
            table[42, 1] = 8.207141;
            table[43, 1] = 6.747911;
            table[44, 1] = 5.366409;
            table[45, 1] = 4.054696;
            table[46, 1] = 1.032558;
            table[47, 1] = -1.704106;
            table[48, 1] = -4.240182;
            table[49, 1] = -6.638779;
            table[50, 1] = -8.943083;
            table[51, 1] = -11.180415;
            table[52, 1] = -13.366997;
            table[53, 1] = -15.512328;
            table[54, 1] = -17.622519;
            table[55, 1] = -19.702458;
            table[56, 1] = -21.756948;
            table[57, 1] = -23.791148;
            table[58, 1] = -25.810576;
            table[59, 1] = -27.820889;
            table[60, 1] = -29.827609;
            table[61, 1] = -31.835813;
            table[62, 1] = -33.849919;
            table[63, 1] = -35.873482;
            table[64, 1] = -37.909065;
            table[65, 1] = -39.958162;
            table[66, 1] = -42.021151;
            table[67, 1] = -44.097296;
            table[68, 1] = -46.18477;
            table[69, 1] = -48.280699;
            table[70, 1] = -50.381243;
            table[71, 1] = -52.481679;
            table[72, 1] = -54.576511;
            table[73, 1] = -56.6596;
            table[74, 1] = -58.724295;
            table[75, 1] = -60.763589;
            table[76, 1] = -62.770281;
            table[77, 1] = -64.737135;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_50t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.739519;
            table[1, 1] = 99.990928;
            table[2, 1] = 95.248778;
            table[3, 1] = 91.348432;
            table[4, 1] = 88.031961;
            table[5, 1] = 85.16762;
            table[6, 1] = 82.658;
            table[7, 1] = 80.429022;
            table[8, 1] = 78.42491;
            table[9, 1] = 76.60363;
            table[10, 1] = 74.9332;
            table[11, 1] = 73.388958;
            table[12, 1] = 71.951598;
            table[13, 1] = 70.605803;
            table[14, 1] = 69.339236;
            table[15, 1] = 68.141844;
            table[16, 1] = 67.005335;
            table[17, 1] = 65.922798;
            table[18, 1] = 64.888416;
            table[19, 1] = 63.89725;
            table[20, 1] = 59.47107;
            table[21, 1] = 55.664152;
            table[22, 1] = 52.294607;
            table[23, 1] = 49.245286;
            table[24, 1] = 46.435801;
            table[25, 1] = 43.831823;
            table[26, 1] = 41.395442;
            table[27, 1] = 39.10487;
            table[28, 1] = 36.944411;
            table[29, 1] = 34.902148;
            table[30, 1] = 32.96849;
            table[31, 1] = 31.13528;
            table[32, 1] = 29.395272;
            table[33, 1] = 27.741858;
            table[34, 1] = 26.168946;
            table[35, 1] = 24.670902;
            table[36, 1] = 21.879104;
            table[37, 1] = 19.330117;
            table[38, 1] = 16.993923;
            table[39, 1] = 14.845747;
            table[40, 1] = 12.864796;
            table[41, 1] = 11.033107;
            table[42, 1] = 9.334689;
            table[43, 1] = 7.754931;
            table[44, 1] = 6.280329;
            table[45, 1] = 4.898341;
            table[46, 1] = 1.774779;
            table[47, 1] = -0.994671;
            table[48, 1] = -3.527356;
            table[49, 1] = -5.906296;
            table[50, 1] = -8.185937;
            table[51, 1] = -10.399216;
            table[52, 1] = -12.564647;
            table[53, 1] = -14.692299;
            table[54, 1] = -16.788085;
            table[55, 1] = -18.856422;
            table[56, 1] = -20.901602;
            table[57, 1] = -22.928326;
            table[58, 1] = -24.941728;
            table[59, 1] = -26.947162;
            table[60, 1] = -28.949904;
            table[61, 1] = -30.954853;
            table[62, 1] = -32.966279;
            table[63, 1] = -34.987623;
            table[64, 1] = -37.021362;
            table[65, 1] = -39.068917;
            table[66, 1] = -41.130615;
            table[67, 1] = -43.205674;
            table[68, 1] = -45.292231;
            table[69, 1] = -47.387385;
            table[70, 1] = -49.487271;
            table[71, 1] = -51.587146;
            table[72, 1] = -53.681501;
            table[73, 1] = -55.764179;
            table[74, 1] = -57.828522;
            table[75, 1] = -59.867513;
            table[76, 1] = -61.873942;
            table[77, 1] = -63.840569;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_50t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.888964;
            table[1, 1] = 100.790232;
            table[2, 1] = 97.062241;
            table[3, 1] = 94.192533;
            table[4, 1] = 91.71942;
            table[5, 1] = 89.470537;
            table[6, 1] = 87.383511;
            table[7, 1] = 85.436467;
            table[8, 1] = 83.619073;
            table[9, 1] = 81.922509;
            table[10, 1] = 80.337235;
            table[11, 1] = 78.853199;
            table[12, 1] = 77.46046;
            table[13, 1] = 76.149647;
            table[14, 1] = 74.912198;
            table[15, 1] = 73.740431;
            table[16, 1] = 72.627534;
            table[17, 1] = 71.567493;
            table[18, 1] = 70.555016;
            table[19, 1] = 69.585448;
            table[20, 1] = 65.262726;
            table[21, 1] = 61.547969;
            table[22, 1] = 58.245506;
            table[23, 1] = 55.229511;
            table[24, 1] = 52.417307;
            table[25, 1] = 49.769554;
            table[26, 1] = 47.251855;
            table[27, 1] = 44.845628;
            table[28, 1] = 42.539536;
            table[29, 1] = 40.326698;
            table[30, 1] = 38.202819;
            table[31, 1] = 36.164931;
            table[32, 1] = 34.210614;
            table[33, 1] = 32.337536;
            table[34, 1] = 30.54325;
            table[35, 1] = 28.825128;
            table[36, 1] = 25.606197;
            table[37, 1] = 22.657964;
            table[38, 1] = 19.958198;
            table[39, 1] = 17.48613;
            table[40, 1] = 15.222407;
            table[41, 1] = 13.148629;
            table[42, 1] = 11.24689;
            table[43, 1] = 9.499649;
            table[44, 1] = 7.889697;
            table[45, 1] = 6.400432;
            table[46, 1] = 3.104874;
            table[47, 1] = 0.256264;
            table[48, 1] = -2.304839;
            table[49, 1] = -4.687793;
            table[50, 1] = -6.961467;
            table[51, 1] = -9.165772;
            table[52, 1] = -11.322196;
            table[53, 1] = -13.441863;
            table[54, 1] = -15.530919;
            table[55, 1] = -17.593699;
            table[56, 1] = -19.634324;
            table[57, 1] = -21.657314;
            table[58, 1] = -23.667645;
            table[59, 1] = -25.670539;
            table[60, 1] = -27.671169;
            table[61, 1] = -29.674356;
            table[62, 1] = -31.684303;
            table[63, 1] = -33.704402;
            table[64, 1] = -35.737089;
            table[65, 1] = -37.783754;
            table[66, 1] = -39.844697;
            table[67, 1] = -41.919115;
            table[68, 1] = -44.005127;
            table[69, 1] = -46.099818;
            table[70, 1] = -48.19931;
            table[71, 1] = -50.298849;
            table[72, 1] = -52.392916;
            table[73, 1] = -54.475349;
            table[74, 1] = -56.539482;
            table[75, 1] = -58.578292;
            table[76, 1] = -60.584565;
            table[77, 1] = -62.551057;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_50t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.899552;
            table[1, 1] = 100.874455;
            table[2, 1] = 97.337504;
            table[3, 1] = 94.804852;
            table[4, 1] = 92.805244;
            table[5, 1] = 91.124192;
            table[6, 1] = 89.644665;
            table[7, 1] = 88.296276;
            table[8, 1] = 87.034958;
            table[9, 1] = 85.833279;
            table[10, 1] = 84.674904;
            table[11, 1] = 83.550883;
            table[12, 1] = 82.456936;
            table[13, 1] = 81.391467;
            table[14, 1] = 80.354195;
            table[15, 1] = 79.345298;
            table[16, 1] = 78.364927;
            table[17, 1] = 77.412992;
            table[18, 1] = 76.489095;
            table[19, 1] = 75.592546;
            table[20, 1] = 71.491678;
            table[21, 1] = 67.887206;
            table[22, 1] = 64.652624;
            table[23, 1] = 61.682866;
            table[24, 1] = 58.900096;
            table[25, 1] = 56.263684;
            table[26, 1] = 53.737396;
            table[27, 1] = 51.300746;
            table[28, 1] = 48.941374;
            table[29, 1] = 46.652469;
            table[30, 1] = 44.430833;
            table[31, 1] = 42.275437;
            table[32, 1] = 40.186368;
            table[33, 1] = 38.16412;
            table[34, 1] = 36.209157;
            table[35, 1] = 34.321685;
            table[36, 1] = 30.748327;
            table[37, 1] = 27.439191;
            table[38, 1] = 24.385703;
            table[39, 1] = 21.577078;
            table[40, 1] = 19.000998;
            table[41, 1] = 16.643696;
            table[42, 1] = 14.489784;
            table[43, 1] = 12.522474;
            table[44, 1] = 10.723714;
            table[45, 1] = 9.074869;
            table[46, 1] = 5.488472;
            table[47, 1] = 2.462017;
            table[48, 1] = -0.208187;
            table[49, 1] = -2.660785;
            table[50, 1] = -4.981229;
            table[51, 1] = -7.218275;
            table[52, 1] = -9.398244;
            table[53, 1] = -11.535112;
            table[54, 1] = -13.636858;
            table[55, 1] = -15.709064;
            table[56, 1] = -17.756729;
            table[57, 1] = -19.785004;
            table[58, 1] = -21.799322;
            table[59, 1] = -23.80524;
            table[60, 1] = -25.80817;
            table[61, 1] = -27.813121;
            table[62, 1] = -29.824426;
            table[63, 1] = -31.845577;
            table[64, 1] = -33.879086;
            table[65, 1] = -35.926397;
            table[66, 1] = -37.987854;
            table[67, 1] = -40.062685;
            table[68, 1] = -42.149033;
            table[69, 1] = -44.244;
            table[70, 1] = -46.343721;
            table[71, 1] = -48.443455;
            table[72, 1] = -50.537688;
            table[73, 1] = -52.620263;
            table[74, 1] = -54.684519;
            table[75, 1] = -56.723437;
            table[76, 1] = -58.729805;
            table[77, 1] = -60.696379;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_50t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.89999;
            table[1, 1] = 100.879246;
            table[2, 1] = 97.356813;
            table[3, 1] = 94.856437;
            table[4, 1] = 92.914926;
            table[5, 1] = 91.325394;
            table[6, 1] = 89.976912;
            table[7, 1] = 88.802712;
            table[8, 1] = 87.759199;
            table[9, 1] = 86.816116;
            table[10, 1] = 85.951445;
            table[11, 1] = 85.148553;
            table[12, 1] = 84.39451;
            table[13, 1] = 83.679047;
            table[14, 1] = 82.993897;
            table[15, 1] = 82.332363;
            table[16, 1] = 81.689018;
            table[17, 1] = 81.059505;
            table[18, 1] = 80.440373;
            table[19, 1] = 79.828952;
            table[20, 1] = 76.84136;
            table[21, 1] = 73.917581;
            table[22, 1] = 71.086017;
            table[23, 1] = 68.370967;
            table[24, 1] = 65.767055;
            table[25, 1] = 63.272117;
            table[26, 1] = 60.863612;
            table[27, 1] = 58.525745;
            table[28, 1] = 56.246374;
            table[29, 1] = 54.017374;
            table[30, 1] = 51.834161;
            table[31, 1] = 49.694903;
            table[32, 1] = 47.599695;
            table[33, 1] = 45.549816;
            table[34, 1] = 43.547154;
            table[35, 1] = 41.593777;
            table[36, 1] = 37.84253;
            table[37, 1] = 34.308461;
            table[38, 1] = 30.998692;
            table[39, 1] = 27.915484;
            table[40, 1] = 25.05702;
            table[41, 1] = 22.417914;
            table[42, 1] = 19.98939;
            table[43, 1] = 17.759689;
            table[44, 1] = 15.714279;
            table[45, 1] = 13.836643;
            table[46, 1] = 9.759995;
            table[47, 1] = 6.351382;
            table[48, 1] = 3.387631;
            table[49, 1] = 0.711192;
            table[50, 1] = -1.77875;
            table[51, 1] = -4.143353;
            table[52, 1] = -6.418978;
            table[53, 1] = -8.62757;
            table[54, 1] = -10.783248;
            table[55, 1] = -12.896214;
            table[56, 1] = -14.974889;
            table[57, 1] = -17.02693;
            table[58, 1] = -19.059604;
            table[59, 1] = -21.079811;
            table[60, 1] = -23.093953;
            table[61, 1] = -25.107769;
            table[62, 1] = -27.126136;
            table[63, 1] = -29.152955;
            table[64, 1] = -31.191046;
            table[65, 1] = -33.242089;
            table[66, 1] = -35.306605;
            table[67, 1] = -37.383964;
            table[68, 1] = -39.472413;
            table[69, 1] = -41.569139;
            table[70, 1] = -43.670344;
            table[71, 1] = -45.771333;
            table[72, 1] = -47.866638;
            table[73, 1] = -49.950132;
            table[74, 1] = -52.015179;
            table[75, 1] = -54.054781;
            table[76, 1] = -56.061744;
            table[77, 1] = -58.028837;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_50t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.9;
            table[1, 1] = 100.879398;
            table[2, 1] = 97.35756;
            table[3, 1] = 94.858745;
            table[4, 1] = 92.92045;
            table[5, 1] = 91.336633;
            table[6, 1] = 89.997354;
            table[7, 1] = 88.836949;
            table[8, 1] = 87.813019;
            table[9, 1] = 86.896567;
            table[10, 1] = 86.066862;
            table[11, 1] = 85.308538;
            table[12, 1] = 84.609871;
            table[13, 1] = 83.961686;
            table[14, 1] = 83.356646;
            table[15, 1] = 82.788773;
            table[16, 1] = 82.253112;
            table[17, 1] = 81.745493;
            table[18, 1] = 81.262358;
            table[19, 1] = 80.800635;
            table[20, 1] = 78.730328;
            table[21, 1] = 76.895671;
            table[22, 1] = 75.151521;
            table[23, 1] = 73.415101;
            table[24, 1] = 71.643414;
            table[25, 1] = 69.837557;
            table[26, 1] = 67.99953;
            table[27, 1] = 66.138772;
            table[28, 1] = 64.263772;
            table[29, 1] = 62.381188;
            table[30, 1] = 60.496363;
            table[31, 1] = 58.614005;
            table[32, 1] = 56.73865;
            table[33, 1] = 54.874833;
            table[34, 1] = 53.02711;
            table[35, 1] = 51.199934;
            table[36, 1] = 47.623753;
            table[37, 1] = 44.175137;
            table[38, 1] = 40.874881;
            table[39, 1] = 37.735574;
            table[40, 1] = 34.762908;
            table[41, 1] = 31.95743;
            table[42, 1] = 29.316092;
            table[43, 1] = 26.833436;
            table[44, 1] = 24.502405;
            table[45, 1] = 22.314864;
            table[46, 1] = 17.414774;
            table[47, 1] = 13.20943;
            table[48, 1] = 9.554562;
            table[49, 1] = 6.320556;
            table[50, 1] = 3.40036;
            table[51, 1] = 0.711673;
            table[52, 1] = -1.805544;
            table[53, 1] = -4.19395;
            table[54, 1] = -6.483968;
            table[55, 1] = -8.69799;
            table[56, 1] = -10.853323;
            table[57, 1] = -12.964049;
            table[58, 1] = -15.042066;
            table[59, 1] = -17.09763;
            table[60, 1] = -19.13959;
            table[61, 1] = -21.175472;
            table[62, 1] = -23.211486;
            table[63, 1] = -25.252527;
            table[64, 1] = -27.302162;
            table[65, 1] = -29.362644;
            table[66, 1] = -31.434929;
            table[67, 1] = -33.518724;
            table[68, 1] = -35.612538;
            table[69, 1] = -37.713763;
            table[70, 1] = -39.818762;
            table[71, 1] = -41.922969;
            table[72, 1] = -44.021016;
            table[73, 1] = -46.106859;
            table[74, 1] = -48.173927;
            table[75, 1] = -50.215277;
            table[76, 1] = -52.223755;
            table[77, 1] = -54.192169;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_10t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 97.934646;
            table[1, 1] = 88.378741;
            table[2, 1] = 82.64764;
            table[3, 1] = 78.482409;
            table[4, 1] = 75.16738;
            table[5, 1] = 72.383643;
            table[6, 1] = 69.96179;
            table[7, 1] = 67.801424;
            table[8, 1] = 65.838428;
            table[9, 1] = 64.029568;
            table[10, 1] = 62.344477;
            table[11, 1] = 60.761155;
            table[12, 1] = 59.263201;
            table[13, 1] = 57.838251;
            table[14, 1] = 56.476738;
            table[15, 1] = 55.171205;
            table[16, 1] = 53.915772;
            table[17, 1] = 52.70576;
            table[18, 1] = 51.53742;
            table[19, 1] = 50.407723;
            table[20, 1] = 45.287453;
            table[21, 1] = 40.876789;
            table[22, 1] = 37.447146;
            table[23, 1] = 34.846647;
            table[24, 1] = 32.884332;
            table[25, 1] = 31.427912;
            table[26, 1] = 30.40164;
            table[27, 1] = 29.709636;
            table[28, 1] = 29.168763;
            table[29, 1] = 28.678516;
            table[30, 1] = 28.195633;
            table[31, 1] = 27.69947;
            table[32, 1] = 27.182019;
            table[33, 1] = 26.642093;
            table[34, 1] = 26.082729;
            table[35, 1] = 25.50833;
            table[36, 1] = 24.333334;
            table[37, 1] = 23.150941;
            table[38, 1] = 21.983777;
            table[39, 1] = 20.845641;
            table[40, 1] = 19.742407;
            table[41, 1] = 18.66784;
            table[42, 1] = 17.602814;
            table[43, 1] = 16.5912;
            table[44, 1] = 15.620505;
            table[45, 1] = 14.682537;
            table[46, 1] = 12.449865;
            table[47, 1] = 10.343741;
            table[48, 1] = 8.33992;
            table[49, 1] = 6.423062;
            table[50, 1] = 4.582273;
            table[51, 1] = 2.809305;
            table[52, 1] = 1.097814;
            table[53, 1] = -0.557665;
            table[54, 1] = -2.161197;
            table[55, 1] = -3.716226;
            table[56, 1] = -5.225595;
            table[57, 1] = -6.691659;
            table[58, 1] = -8.116409;
            table[59, 1] = -9.501458;
            table[60, 1] = -10.848244;
            table[61, 1] = -12.157978;
            table[62, 1] = -13.431713;
            table[63, 1] = -14.670377;
            table[64, 1] = -15.8748;
            table[65, 1] = -17.045739;
            table[66, 1] = -18.183888;
            table[67, 1] = -19.289904;
            table[68, 1] = -20.364408;
            table[69, 1] = -21.408001;
            table[70, 1] = -22.42127;
            table[71, 1] = -23.404792;
            table[72, 1] = -24.359142;
            table[73, 1] = -25.284895;
            table[74, 1] = -26.182626;
            table[75, 1] = -27.052916;
            table[76, 1] = -27.896351;
            table[77, 1] = -28.713523;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_10t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 102.299456;
            table[1, 1] = 92.582377;
            table[2, 1] = 86.624759;
            table[3, 1] = 82.298446;
            table[4, 1] = 78.869361;
            table[5, 1] = 76.007063;
            table[6, 1] = 73.533803;
            table[7, 1] = 71.343369;
            table[8, 1] = 69.36748;
            table[9, 1] = 67.559808;
            table[10, 1] = 65.8876;
            table[11, 1] = 64.326946;
            table[12, 1] = 62.859889;
            table[13, 1] = 61.472724;
            table[14, 1] = 60.154735;
            table[15, 1] = 58.897449;
            table[16, 1] = 57.694068;
            table[17, 1] = 56.539078;
            table[18, 1] = 55.42796;
            table[19, 1] = 54.356973;
            table[20, 1] = 49.526882;
            table[21, 1] = 45.505014;
            table[22, 1] = 42.233212;
            table[23, 1] = 39.579294;
            table[24, 1] = 37.442025;
            table[25, 1] = 35.760058;
            table[26, 1] = 34.530981;
            table[27, 1] = 33.489585;
            table[28, 1] = 32.606515;
            table[29, 1] = 31.79914;
            table[30, 1] = 31.029776;
            table[31, 1] = 30.278292;
            table[32, 1] = 29.534972;
            table[33, 1] = 28.796084;
            table[34, 1] = 28.061661;
            table[35, 1] = 27.333231;
            table[36, 1] = 25.902842;
            table[37, 1] = 24.520552;
            table[38, 1] = 23.195438;
            table[39, 1] = 21.930961;
            table[40, 1] = 20.72561;
            table[41, 1] = 19.570152;
            table[42, 1] = 18.447689;
            table[43, 1] = 17.383762;
            table[44, 1] = 16.366516;
            table[45, 1] = 15.387876;
            table[46, 1] = 13.076309;
            table[47, 1] = 10.9165;
            table[48, 1] = 8.876181;
            table[49, 1] = 6.934534;
            table[50, 1] = 5.077053;
            table[51, 1] = 3.293082;
            table[52, 1] = 1.5746;
            table[53, 1] = -0.084903;
            table[54, 1] = -1.690343;
            table[55, 1] = -3.245744;
            table[56, 1] = -4.75437;
            table[57, 1] = -6.218889;
            table[58, 1] = -7.641518;
            table[59, 1] = -9.024057;
            table[60, 1] = -10.368067;
            table[61, 1] = -11.674855;
            table[62, 1] = -12.94555;
            table[63, 1] = -14.181135;
            table[64, 1] = -15.382482;
            table[65, 1] = -16.550376;
            table[66, 1] = -17.685536;
            table[67, 1] = -18.788634;
            table[68, 1] = -19.860302;
            table[69, 1] = -20.901149;
            table[70, 1] = -21.911765;
            table[71, 1] = -22.892732;
            table[72, 1] = -23.844624;
            table[73, 1] = -24.768015;
            table[74, 1] = -25.66348;
            table[75, 1] = -26.531597;
            table[76, 1] = -27.372949;
            table[77, 1] = -28.188126;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_10t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 105.725647;
            table[1, 1] = 97.060527;
            table[2, 1] = 91.196055;
            table[3, 1] = 86.788651;
            table[4, 1] = 83.264817;
            table[5, 1] = 80.322778;
            table[6, 1] = 77.789377;
            table[7, 1] = 75.557363;
            table[8, 1] = 73.556272;
            table[9, 1] = 71.7376;
            table[10, 1] = 70.066644;
            table[11, 1] = 68.517884;
            table[12, 1] = 67.072001;
            table[13, 1] = 65.714093;
            table[14, 1] = 64.432452;
            table[15, 1] = 63.217737;
            table[16, 1] = 62.062396;
            table[17, 1] = 60.960251;
            table[18, 1] = 59.951023;
            table[19, 1] = 58.895986;
            table[20, 1] = 54.406804;
            table[21, 1] = 50.634735;
            table[22, 1] = 47.436983;
            table[23, 1] = 44.764084;
            table[24, 1] = 42.534858;
            table[25, 1] = 40.690672;
            table[26, 1] = 39.155045;
            table[27, 1] = 37.792323;
            table[28, 1] = 36.587767;
            table[29, 1] = 35.475676;
            table[30, 1] = 34.424876;
            table[31, 1] = 33.417127;
            table[32, 1] = 32.442369;
            table[33, 1] = 31.495525;
            table[34, 1] = 30.574637;
            table[35, 1] = 29.679158;
            table[36, 1] = 27.964866;
            table[37, 1] = 26.353455;
            table[38, 1] = 24.842269;
            table[39, 1] = 23.425281;
            table[40, 1] = 22.093914;
            table[41, 1] = 20.835796;
            table[42, 1] = 19.636093;
            table[43, 1] = 18.502646;
            table[44, 1] = 17.423561;
            table[45, 1] = 16.39054;
            table[46, 1] = 13.971428;
            table[47, 1] = 11.735641;
            table[48, 1] = 9.641237;
            table[49, 1] = 7.660692;
            table[50, 1] = 5.774992;
            table[51, 1] = 3.970431;
            table[52, 1] = 2.23684;
            table[53, 1] = 0.566338;
            table[54, 1] = -1.047091;
            table[55, 1] = -2.608231;
            table[56, 1] = -4.120902;
            table[57, 1] = -5.588187;
            table[58, 1] = -7.012601;
            table[59, 1] = -8.3962;
            table[60, 1] = -9.740707;
            table[61, 1] = -11.04757;
            table[62, 1] = -12.318018;
            table[63, 1] = -13.553117;
            table[64, 1] = -14.7538;
            table[65, 1] = -15.920902;
            table[66, 1] = -17.055179;
            table[67, 1] = -18.15733;
            table[68, 1] = -19.228012;
            table[69, 1] = -20.26785;
            table[70, 1] = -21.27745;
            table[71, 1] = -22.257401;
            table[72, 1] = -23.208287;
            table[73, 1] = -24.130687;
            table[74, 1] = -25.025181;
            table[75, 1] = -25.89235;
            table[76, 1] = -26.732779;
            table[77, 1] = -27.54706;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_10t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.905039;
            table[1, 1] = 100.206533;
            table[2, 1] = 95.486675;
            table[3, 1] = 91.566194;
            table[4, 1] = 88.226972;
            table[5, 1] = 85.349137;
            table[6, 1] = 82.835221;
            table[7, 1] = 80.608831;
            table[8, 1] = 78.61213;
            table[9, 1] = 76.801604;
            table[10, 1] = 75.144308;
            table[11, 1] = 73.614932;
            table[12, 1] = 72.193764;
            table[13, 1] = 70.86522;
            table[14, 1] = 69.616811;
            table[15, 1] = 68.438405;
            table[16, 1] = 67.321687;
            table[17, 1] = 66.259766;
            table[18, 1] = 65.27989;
            table[19, 1] = 64.278163;
            table[20, 1] = 59.972027;
            table[21, 1] = 56.346515;
            table[22, 1] = 53.234776;
            table[23, 1] = 50.53292;
            table[24, 1] = 48.178969;
            table[25, 1] = 46.128567;
            table[26, 1] = 44.332996;
            table[27, 1] = 42.690077;
            table[28, 1] = 41.198388;
            table[29, 1] = 39.807114;
            table[30, 1] = 38.491934;
            table[31, 1] = 37.237565;
            table[32, 1] = 36.034841;
            table[33, 1] = 34.878529;
            table[34, 1] = 33.765768;
            table[35, 1] = 32.694862;
            table[36, 1] = 30.673954;
            table[37, 1] = 28.806363;
            table[38, 1] = 27.080563;
            table[39, 1] = 25.48273;
            table[40, 1] = 23.998157;
            table[41, 1] = 22.611075;
            table[42, 1] = 21.307702;
            table[43, 1] = 20.082208;
            table[44, 1] = 18.921323;
            table[45, 1] = 17.815863;
            table[46, 1] = 15.250455;
            table[47, 1] = 12.907152;
            table[48, 1] = 10.732702;
            table[49, 1] = 8.691647;
            table[50, 1] = 6.759508;
            table[51, 1] = 4.918804;
            table[52, 1] = 3.156669;
            table[53, 1] = 1.463439;
            table[54, 1] = -0.168318;
            table[55, 1] = -1.744386;
            table[56, 1] = -3.26933;
            table[57, 1] = -4.746791;
            table[58, 1] = -6.179704;
            table[59, 1] = -7.570469;
            table[60, 1] = -8.921053;
            table[61, 1] = -10.233103;
            table[62, 1] = -11.508008;
            table[63, 1] = -12.746957;
            table[64, 1] = -13.950985;
            table[65, 1] = -15.121008;
            table[66, 1] = -16.257848;
            table[67, 1] = -17.36226;
            table[68, 1] = -18.434943;
            table[69, 1] = -19.476561;
            table[70, 1] = -20.487749;
            table[71, 1] = -21.469123;
            table[72, 1] = -22.421289;
            table[73, 1] = -23.344844;
            table[74, 1] = -24.240382;
            table[75, 1] = -25.1085;
            table[76, 1] = -25.949793;
            table[77, 1] = -26.764861;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_10t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.062383;
            table[1, 1] = 101.065047;
            table[2, 1] = 97.470838;
            table[3, 1] = 94.679941;
            table[4, 1] = 92.231225;
            table[5, 1] = 89.973636;
            table[6, 1] = 87.866818;
            table[7, 1] = 85.90258;
            table[8, 1] = 84.076169;
            table[9, 1] = 82.379644;
            table[10, 1] = 80.802313;
            table[11, 1] = 79.33249;
            table[12, 1] = 77.958679;
            table[13, 1] = 76.670237;
            table[14, 1] = 75.457626;
            table[15, 1] = 74.312446;
            table[16, 1] = 73.227373;
            table[17, 1] = 72.196046;
            table[18, 1] = 71.224486;
            table[19, 1] = 70.273299;
            table[20, 1] = 66.098754;
            table[21, 1] = 62.568803;
            table[22, 1] = 59.497064;
            table[23, 1] = 56.771976;
            table[24, 1] = 54.328033;
            table[25, 1] = 52.124695;
            table[26, 1] = 50.130359;
            table[27, 1] = 48.264582;
            table[28, 1] = 46.537497;
            table[29, 1] = 44.909852;
            table[30, 1] = 43.363764;
            table[31, 1] = 41.887489;
            table[32, 1] = 40.473721;
            table[33, 1] = 39.118123;
            table[34, 1] = 37.818013;
            table[35, 1] = 36.571536;
            table[36, 1] = 34.233137;
            table[37, 1] = 32.088993;
            table[38, 1] = 30.122737;
            table[39, 1] = 28.315518;
            table[40, 1] = 26.64825;
            table[41, 1] = 25.101201;
            table[42, 1] = 23.658988;
            table[43, 1] = 22.312124;
            table[44, 1] = 21.043547;
            table[45, 1] = 19.842011;
            table[46, 1] = 17.078045;
            table[47, 1] = 14.582609;
            table[48, 1] = 12.289839;
            table[49, 1] = 10.155334;
            table[50, 1] = 8.148271;
            table[51, 1] = 6.246644;
            table[52, 1] = 4.434279;
            table[53, 1] = 2.699218;
            table[54, 1] = 1.03221;
            table[55, 1] = -0.573858;
            table[56, 1] = -2.12456;
            table[57, 1] = -3.624318;
            table[58, 1] = -5.076669;
            table[59, 1] = -6.484503;
            table[60, 1] = -7.850163;
            table[61, 1] = -9.175602;
            table[62, 1] = -10.462458;
            table[63, 1] = -11.712124;
            table[64, 1] = -12.925805;
            table[65, 1] = -14.104556;
            table[66, 1] = -15.249317;
            table[67, 1] = -16.360941;
            table[68, 1] = -17.440213;
            table[69, 1] = -18.487866;
            table[70, 1] = -19.504599;
            table[71, 1] = -20.49108;
            table[72, 1] = -21.44796;
            table[73, 1] = -22.375877;
            table[74, 1] = -23.27546;
            table[75, 1] = -24.147336;
            table[76, 1] = -24.992127;
            table[77, 1] = -25.810458;



            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_10t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.073529;
            table[1, 1] = 101.15606;
            table[2, 1] = 97.780137;
            table[3, 1] = 95.385386;
            table[4, 1] = 93.494513;
            table[5, 1] = 91.894266;
            table[6, 1] = 90.470278;
            table[7, 1] = 89.155896;
            table[8, 1] = 87.911834;
            table[9, 1] = 86.716046;
            table[10, 1] = 85.557422;
            table[11, 1] = 84.431432;
            table[12, 1] = 83.337012;
            table[13, 1] = 82.274549;
            table[14, 1] = 81.244714;
            table[15, 1] = 80.247891;
            table[16, 1] = 79.283982;
            table[17, 1] = 78.352411;
            table[18, 1] = 77.458538;
            table[19, 1] = 76.582126;
            table[20, 1] = 72.631874;
            table[21, 1] = 69.218285;
            table[22, 1] = 66.204112;
            table[23, 1] = 63.48892;
            table[24, 1] = 61.008376;
            table[25, 1] = 58.724456;
            table[26, 1] = 56.613523;
            table[27, 1] = 54.60432;
            table[28, 1] = 52.716858;
            table[29, 1] = 50.918997;
            table[30, 1] = 49.198093;
            table[31, 1] = 47.545963;
            table[32, 1] = 45.957717;
            table[33, 1] = 44.430706;
            table[34, 1] = 42.963394;
            table[35, 1] = 41.554734;
            table[36, 1] = 38.909042;
            table[37, 1] = 36.481936;
            table[38, 1] = 34.257466;
            table[39, 1] = 32.215802;
            table[40, 1] = 30.336315;
            table[41, 1] = 28.594009;
            table[42, 1] = 26.966301;
            table[43, 1] = 25.460159;
            table[44, 1] = 24.05059;
            table[45, 1] = 22.722251;
            table[46, 1] = 19.689469;
            table[47, 1] = 16.978795;
            table[48, 1] = 14.511326;
            table[49, 1] = 12.233387;
            table[50, 1] = 10.107267;
            table[51, 1] = 8.105788;
            table[52, 1] = 6.208837;
            table[53, 1] = 4.401533;
            table[54, 1] = 2.672299;
            table[55, 1] = 1.012234;
            table[56, 1] = -0.585644;
            table[57, 1] = -2.126872;
            table[58, 1] = -3.615878;
            table[59, 1] = -5.056287;
            table[60, 1] = -6.451029;
            table[61, 1] = -7.802544;
            table[62, 1] = -9.112875;
            table[63, 1] = -10.383754;
            table[64, 1] = -11.616669;
            table[65, 1] = -12.812916;
            table[66, 1] = -13.973641;
            table[67, 1] = -15.099871;
            table[68, 1] = -16.192542;
            table[69, 1] = -17.252519;
            table[70, 1] = -18.280611;
            table[71, 1] = -19.277587;
            table[72, 1] = -20.244183;
            table[73, 1] = -21.181113;
            table[74, 1] = -22.089073;
            table[75, 1] = -22.968748;
            table[76, 1] = -23.820813;
            table[77, 1] = -24.645939;



            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_10t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.07399;
            table[1, 1] = 101.161233;
            table[2, 1] = 97.801866;
            table[3, 1] = 95.445377;
            table[4, 1] = 93.625065;
            table[5, 1] = 92.137258;
            table[6, 1] = 90.874362;
            table[7, 1] = 89.772269;
            table[8, 1] = 88.789438;
            table[9, 1] = 87.897241;
            table[10, 1] = 87.075016;
            table[11, 1] = 86.307332;
            table[12, 1] = 85.582378;
            table[13, 1] = 84.890971;
            table[14, 1] = 84.225912;
            table[15, 1] = 83.581549;
            table[16, 1] = 82.95347;
            table[17, 1] = 82.338266;
            table[18, 1] = 81.74037;
            table[19, 1] = 81.136808;
            table[20, 1] = 78.244942;
            table[21, 1] = 75.481905;
            table[22, 1] = 72.863039;
            table[23, 1] = 70.390048;
            table[24, 1] = 68.054974;
            table[25, 1] = 65.851797;
            table[26, 1] = 63.773385;
            table[27, 1] = 61.754994;
            table[28, 1] = 59.829224;
            table[29, 1] = 57.969164;
            table[30, 1] = 56.167363;
            table[31, 1] = 54.419791;
            table[32, 1] = 52.7248;
            table[33, 1] = 51.082313;
            table[34, 1] = 49.492998;
            table[35, 1] = 47.957659;
            table[36, 1] = 45.050741;
            table[37, 1] = 42.360064;
            table[38, 1] = 39.876755;
            table[39, 1] = 37.585646;
            table[40, 1] = 35.468772;
            table[41, 1] = 33.493166;
            table[42, 1] = 31.618313;
            table[43, 1] = 29.904299;
            table[44, 1] = 28.311338;
            table[45, 1] = 26.816597;
            table[46, 1] = 23.4209;
            table[47, 1] = 20.405741;
            table[48, 1] = 17.680605;
            table[49, 1] = 15.18368;
            table[50, 1] = 12.870636;
            table[51, 1] = 10.70882;
            table[52, 1] = 8.673636;
            table[53, 1] = 6.746515;
            table[54, 1] = 4.912892;
            table[55, 1] = 3.161422;
            table[56, 1] = 1.483123;
            table[57, 1] = -0.129188;
            table[58, 1] = -1.681299;
            table[59, 1] = -3.17796;
            table[60, 1] = -4.623041;
            table[61, 1] = -6.019769;
            table[62, 1] = -7.370852;
            table[63, 1] = -8.678583;
            table[64, 1] = -9.944927;
            table[65, 1] = -11.171586;
            table[66, 1] = -12.360057;
            table[67, 1] = -13.511668;
            table[68, 1] = -14.627614;
            table[69, 1] = -15.708985;
            table[70, 1] = -16.756788;
            table[71, 1] = -17.771963;
            table[72, 1] = -18.755397;
            table[73, 1] = -19.707935;
            table[74, 1] = -20.630391;
            table[75, 1] = -21.523553;
            table[76, 1] = -22.388187;
            table[77, 1] = -23.225047;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_10t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.161397;
            table[2, 1] = 97.802709;
            table[3, 1] = 95.448073;
            table[4, 1] = 93.631676;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.899509;
            table[7, 1] = 89.814614;
            table[8, 1] = 88.856082;
            table[9, 1] = 87.996632;
            table[10, 1] = 87.216863;
            table[11, 1] = 86.502468;
            table[12, 1] = 85.842574;
            table[13, 1] = 85.228708;
            table[14, 1] = 84.654125;
            table[15, 1] = 84.113356;
            table[16, 1] = 83.601898;
            table[17, 1] = 83.115987;
            table[18, 1] = 82.654031;
            table[19, 1] = 82.208542;
            table[20, 1] = 80.211252;
            table[21, 1] = 78.452352;
            table[22, 1] = 76.81956;
            table[23, 1] = 75.245885;
            table[24, 1] = 73.693412;
            table[25, 1] = 72.149018;
            table[26, 1] = 70.609324;
            table[27, 1] = 69.026718;
            table[28, 1] = 67.435786;
            table[29, 1] = 65.824479;
            table[30, 1] = 64.19775;
            table[31, 1] = 62.563762;
            table[32, 1] = 60.931856;
            table[33, 1] = 59.311272;
            table[34, 1] = 57.710662;
            table[35, 1] = 56.137336;
            table[36, 1] = 53.09467;
            table[37, 1] = 50.213973;
            table[38, 1] = 47.507669;
            table[39, 1] = 44.975548;
            table[40, 1] = 42.609514;
            table[41, 1] = 40.363908;
            table[42, 1] = 38.161177;
            table[43, 1] = 36.177823;
            table[44, 1] = 34.34827;
            table[45, 1] = 32.636688;
            table[46, 1] = 28.752641;
            table[47, 1] = 25.306824;
            table[48, 1] = 22.202037;
            table[49, 1] = 19.372321;
            table[50, 1] = 16.768654;
            table[51, 1] = 14.35339;
            table[52, 1] = 12.097157;
            table[53, 1] = 9.976784;
            table[54, 1] = 7.973856;
            table[55, 1] = 6.073619;
            table[56, 1] = 4.26417;
            table[57, 1] = 2.535843;
            table[58, 1] = 0.880734;
            table[59, 1] = -0.707664;
            table[60, 1] = -2.234746;
            table[61, 1] = -3.705018;
            table[62, 1] = -5.122272;
            table[63, 1] = -6.489728;
            table[64, 1] = -7.810142;
            table[65, 1] = -9.085893;
            table[66, 1] = -10.31906;
            table[67, 1] = -11.51147;
            table[68, 1] = -12.664755;
            table[69, 1] = -13.780381;
            table[70, 1] = -14.859682;
            table[71, 1] = -15.903883;
            table[72, 1] = -16.914123;
            table[73, 1] = -17.891465;
            table[74, 1] = -18.836919;
            table[75, 1] = -19.751442;
            table[76, 1] = -20.635953;
            table[77, 1] = -21.491338;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_1t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 101.482869;
            table[1, 1] = 90.908296;
            table[2, 1] = 84.475124;
            table[3, 1] = 79.875116;
            table[4, 1] = 76.276588;
            table[5, 1] = 73.297786;
            table[6, 1] = 70.736199;
            table[7, 1] = 68.473519;
            table[8, 1] = 66.435708;
            table[9, 1] = 64.574188;
            table[10, 1] = 62.856068;
            table[11, 1] = 61.258713;
            table[12, 1] = 59.766509;
            table[13, 1] = 58.368845;
            table[14, 1] = 57.05679;
            table[15, 1] = 55.831999;
            table[16, 1] = 54.685738;
            table[17, 1] = 53.619057;
            table[18, 1] = 52.746406;
            table[19, 1] = 51.721031;
            table[20, 1] = 48.389034;
            table[21, 1] = 46.274619;
            table[22, 1] = 45.094925;
            table[23, 1] = 44.308201;
            table[24, 1] = 43.704906;
            table[25, 1] = 43.187106;
            table[26, 1] = 42.705714;
            table[27, 1] = 42.23397;
            table[28, 1] = 41.75698;
            table[29, 1] = 41.267356;
            table[30, 1] = 40.763266;
            table[31, 1] = 40.246474;
            table[32, 1] = 39.721249;
            table[33, 1] = 39.192592;
            table[34, 1] = 38.665439;
            table[35, 1] = 38.143575;
            table[36, 1] = 37.123845;
            table[37, 1] = 36.138142;
            table[38, 1] = 35.304775;
            table[39, 1] = 34.517703;
            table[40, 1] = 33.752277;
            table[41, 1] = 33.001453;
            table[42, 1] = 32.260252;
            table[43, 1] = 31.536543;
            table[44, 1] = 30.82542;
            table[45, 1] = 30.125321;
            table[46, 1] = 28.421585;
            table[47, 1] = 26.785246;
            table[48, 1] = 25.217177;
            table[49, 1] = 23.716585;
            table[50, 1] = 22.272292;
            table[51, 1] = 20.909244;
            table[52, 1] = 19.597241;
            table[53, 1] = 18.342835;
            table[54, 1] = 17.143643;
            table[55, 1] = 15.997526;
            table[56, 1] = 14.896673;
            table[57, 1] = 13.857038;
            table[58, 1] = 12.859401;
            table[59, 1] = 11.908152;
            table[60, 1] = 11.001868;
            table[61, 1] = 10.139158;
            table[62, 1] = 9.318629;
            table[63, 1] = 8.538887;
            table[64, 1] = 7.798467;
            table[65, 1] = 7.096023;
            table[66, 1] = 6.42991;
            table[67, 1] = 5.798683;
            table[68, 1] = 5.200802;
            table[69, 1] = 4.634713;
            table[70, 1] = 4.09885;
            table[71, 1] = 3.591652;
            table[72, 1] = 3.111563;
            table[73, 1] = 2.657046;
            table[74, 1] = 2.226591;
            table[75, 1] = 1.81872;
            table[76, 1] = 1.432096;
            table[77, 1] = 1.065148;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_1t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 103.914531;
            table[1, 1] = 93.932616;
            table[2, 1] = 87.628143;
            table[3, 1] = 83.064312;
            table[4, 1] = 79.478938;
            table[5, 1] = 76.510267;
            table[6, 1] = 73.962754;
            table[7, 1] = 71.720416;
            table[8, 1] = 69.709634;
            table[9, 1] = 67.881354;
            table[10, 1] = 66.201744;
            table[11, 1] = 64.646931;
            table[12, 1] = 63.199867;
            table[13, 1] = 61.848333;
            table[14, 1] = 60.582393;
            table[15, 1] = 59.399659;
            table[16, 1] = 58.291854;
            table[17, 1] = 57.257387;
            table[18, 1] = 56.378491;
            table[19, 1] = 55.398947;
            table[20, 1] = 51.951635;
            table[21, 1] = 49.645393;
            table[22, 1] = 48.200707;
            table[23, 1] = 47.105546;
            table[24, 1] = 46.202385;
            table[25, 1] = 45.414148;
            table[26, 1] = 44.69913;
            table[27, 1] = 44.032671;
            table[28, 1] = 43.399673;
            table[29, 1] = 42.791173;
            table[30, 1] = 42.202356;
            table[31, 1] = 41.631109;
            table[32, 1] = 41.076829;
            table[33, 1] = 40.539479;
            table[34, 1] = 40.018949;
            table[35, 1] = 39.514748;
            table[36, 1] = 38.551266;
            table[37, 1] = 37.638414;
            table[38, 1] = 36.764723;
            table[39, 1] = 35.920755;
            table[40, 1] = 35.099844;
            table[41, 1] = 34.296659;
            table[42, 1] = 33.507519;
            table[43, 1] = 32.736175;
            table[44, 1] = 31.97955;
            table[45, 1] = 31.236521;
            table[46, 1] = 29.436282;
            table[47, 1] = 27.716615;
            table[48, 1] = 26.075727;
            table[49, 1] = 24.510793;
            table[50, 1] = 23.016445;
            table[51, 1] = 21.594466;
            table[52, 1] = 20.235744;
            table[53, 1] = 18.938794;
            table[54, 1] = 17.700659;
            table[55, 1] = 16.518737;
            table[56, 1] = 15.390736;
            table[57, 1] = 14.314595;
            table[58, 1] = 13.288513;
            table[59, 1] = 12.310761;
            table[60, 1] = 11.379725;
            table[61, 1] = 10.493846;
            table[62, 1] = 9.651589;
            table[63, 1] = 8.851432;
            table[64, 1] = 8.091813;
            table[65, 1] = 7.37125;
            table[66, 1] = 6.688069;
            table[67, 1] = 6.040724;
            table[68, 1] = 5.427611;
            table[69, 1] = 4.847112;
            table[70, 1] = 4.297613;
            table[71, 1] = 3.7775;
            table[72, 1] = 3.285177;
            table[73, 1] = 2.819068;
            table[74, 1] = 2.377627;
            table[75, 1] = 1.959343;
            table[76, 1] = 1.562823;
            table[77, 1] = 1.186519;



            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_1t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.061644;
            table[1, 1] = 97.479539;
            table[2, 1] = 91.529472;
            table[3, 1] = 87.04653;
            table[4, 1] = 83.473091;
            table[5, 1] = 80.500935;
            table[6, 1] = 77.950821;
            table[7, 1] = 75.711898;
            table[8, 1] = 73.711782;
            table[9, 1] = 71.901102;
            table[10, 1] = 70.245033;
            table[11, 1] = 68.718377;
            table[12, 1] = 67.302564;
            table[13, 1] = 65.983732;
            table[14, 1] = 64.750883;
            table[15, 1] = 63.597831;
            table[16, 1] = 62.516651;
            table[17, 1] = 61.591363;
            table[18, 1] = 60.611472;
            table[19, 1] = 59.750484;
            table[20, 1] = 56.311505;
            table[21, 1] = 53.866441;
            table[22, 1] = 52.056421;
            table[23, 1] = 50.61319;
            table[24, 1] = 49.404534;
            table[25, 1] = 48.357746;
            table[26, 1] = 47.428631;
            table[27, 1] = 46.587912;
            table[28, 1] = 45.815151;
            table[29, 1] = 45.095781;
            table[30, 1] = 44.419371;
            table[31, 1] = 43.778437;
            table[32, 1] = 43.167519;
            table[33, 1] = 42.58247;
            table[34, 1] = 42.019959;
            table[35, 1] = 41.477163;
            table[36, 1] = 40.441122;
            table[37, 1] = 39.457913;
            table[38, 1] = 38.515182;
            table[39, 1] = 37.604179;
            table[40, 1] = 36.71902;
            table[41, 1] = 35.855573;
            table[42, 1] = 35.01108;
            table[43, 1] = 34.185629;
            table[44, 1] = 33.377573;
            table[45, 1] = 32.586064;
            table[46, 1] = 30.676672;
            table[47, 1] = 28.862393;
            table[48, 1] = 27.13849;
            table[49, 1] = 25.499906;
            table[50, 1] = 23.946962;
            table[51, 1] = 22.458105;
            table[52, 1] = 21.045282;
            table[53, 1] = 19.698874;
            table[54, 1] = 18.415297;
            table[55, 1] = 17.191432;
            table[56, 1] = 16.030794;
            table[57, 1] = 14.912293;
            table[58, 1] = 13.852493;
            table[59, 1] = 12.843199;
            table[60, 1] = 11.88258;
            table[61, 1] = 10.968891;
            table[62, 1] = 10.10044;
            table[63, 1] = 9.27556;
            table[64, 1] = 8.492579;
            table[65, 1] = 7.749871;
            table[66, 1] = 7.045719;
            table[67, 1] = 6.37847;
            table[68, 1] = 5.746443;
            table[69, 1] = 5.147957;
            table[70, 1] = 4.581335;
            table[71, 1] = 4.044912;
            table[72, 1] = 3.537042;
            table[73, 1] = 3.056107;
            table[74, 1] = 2.60052;
            table[75, 1] = 2.168736;
            table[76, 1] = 1.759295;
            table[77, 1] = 1.370681;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_1t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.085854;
            table[1, 1] = 100.525599;
            table[2, 1] = 95.787198;
            table[3, 1] = 91.785665;
            table[4, 1] = 88.379502;
            table[5, 1] = 85.459857;
            table[6, 1] = 82.923945;
            table[7, 1] = 80.689832;
            table[8, 1] = 78.696143;
            table[9, 1] = 76.897439;
            table[10, 1] = 75.259856;
            table[11, 1] = 73.757847;
            table[12, 1] = 72.371878;
            table[13, 1] = 71.08682;
            table[14, 1] = 69.890812;
            table[15, 1] = 68.774429;
            table[16, 1] = 67.730064;
            table[17, 1] = 66.784813;
            table[18, 1] = 65.83331;
            table[19, 1] = 64.971029;
            table[20, 1] = 61.377677;
            table[21, 1] = 58.629262;
            table[22, 1] = 56.465481;
            table[23, 1] = 54.682572;
            table[24, 1] = 53.170108;
            table[25, 1] = 51.860063;
            table[26, 1] = 50.707064;
            table[27, 1] = 49.678614;
            table[28, 1] = 48.750188;
            table[29, 1] = 47.902645;
            table[30, 1] = 47.120769;
            table[31, 1] = 46.392359;
            table[32, 1] = 45.707603;
            table[33, 1] = 45.058623;
            table[34, 1] = 44.439115;
            table[35, 1] = 43.84406;
            table[36, 1] = 42.712231;
            table[37, 1] = 41.640295;
            table[38, 1] = 40.613916;
            table[39, 1] = 39.623964;
            table[40, 1] = 38.664517;
            table[41, 1] = 37.731858;
            table[42, 1] = 36.823577;
            table[43, 1] = 35.936912;
            table[44, 1] = 35.071013;
            table[45, 1] = 34.225058;
            table[46, 1] = 32.192946;
            table[47, 1] = 30.271935;
            table[48, 1] = 28.454032;
            table[49, 1] = 26.731751;
            table[50, 1] = 25.110515;
            table[51, 1] = 23.546615;
            table[52, 1] = 22.071666;
            table[53, 1] = 20.668274;
            table[54, 1] = 19.332159;
            table[55, 1] = 18.05964;
            table[56, 1] = 16.859986;
            table[57, 1] = 15.693054;
            table[58, 1] = 14.593732;
            table[59, 1] = 13.547331;
            table[60, 1] = 12.551782;
            table[61, 1] = 11.605135;
            table[62, 1] = 10.705518;
            table[63, 1] = 9.851107;
            table[64, 1] = 9.040105;
            table[65, 1] = 8.270725;
            table[66, 1] = 7.541201;
            table[67, 1] = 6.84976;
            table[68, 1] = 6.194638;
            table[69, 1] = 5.574077;
            table[70, 1] = 4.986334;
            table[71, 1] = 4.429685;
            table[72, 1] = 3.902427;
            table[73, 1] = 3.402895;
            table[74, 1] = 2.929457;
            table[75, 1] = 2.48053;
            table[76, 1] = 2.054576;
            table[77, 1] = 1.650115;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_1t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.25362;
            table[1, 1] = 101.501157;
            table[2, 1] = 98.094842;
            table[3, 1] = 95.368463;
            table[4, 1] = 92.881362;
            table[5, 1] = 90.534951;
            table[6, 1] = 88.333377;
            table[7, 1] = 86.290256;
            table[8, 1] = 84.406538;
            table[9, 1] = 82.672967;
            table[10, 1] = 81.075715;
            table[11, 1] = 79.600045;
            table[12, 1] = 78.232074;
            table[13, 1] = 76.959423;
            table[14, 1] = 75.771335;
            table[15, 1] = 74.658565;
            table[16, 1] = 73.613197;
            table[17, 1] = 72.64284;
            table[18, 1] = 71.698454;
            table[19, 1] = 70.818185;
            table[20, 1] = 67.040116;
            table[21, 1] = 63.997652;
            table[22, 1] = 61.503765;
            table[23, 1] = 59.401313;
            table[24, 1] = 57.596598;
            table[25, 1] = 56.025938;
            table[26, 1] = 54.643891;
            table[27, 1] = 53.416244;
            table[28, 1] = 52.315929;
            table[29, 1] = 51.320713;
            table[30, 1] = 50.411942;
            table[31, 1] = 49.573878;
            table[32, 1] = 48.79334;
            table[33, 1] = 48.059472;
            table[34, 1] = 47.363521;
            table[35, 1] = 46.698561;
            table[36, 1] = 45.441295;
            table[37, 1] = 44.257758;
            table[38, 1] = 43.130071;
            table[39, 1] = 42.047194;
            table[40, 1] = 41.001995;
            table[41, 1] = 39.990041;
            table[42, 1] = 39.008413;
            table[43, 1] = 38.052812;
            table[44, 1] = 37.122241;
            table[45, 1] = 36.215541;
            table[46, 1] = 34.046434;
            table[47, 1] = 32.00594;
            table[48, 1] = 30.082466;
            table[49, 1] = 28.265887;
            table[50, 1] = 26.565458;
            table[51, 1] = 24.918542;
            table[52, 1] = 23.372979;
            table[53, 1] = 21.904634;
            table[54, 1] = 20.508469;
            table[55, 1] = 19.180182;
            table[56, 1] = 17.934202;
            table[57, 1] = 16.712945;
            table[58, 1] = 15.567939;
            table[59, 1] = 14.478522;
            table[60, 1] = 13.442361;
            table[61, 1] = 12.457278;
            table[62, 1] = 11.521201;
            table[63, 1] = 10.632132;
            table[64, 1] = 9.788133;
            table[65, 1] = 8.987245;
            table[66, 1] = 8.227634;
            table[67, 1] = 7.507398;
            table[68, 1] = 6.82468;
            table[69, 1] = 6.177637;
            table[70, 1] = 5.56445;
            table[71, 1] = 4.983324;
            table[72, 1] = 4.432497;
            table[73, 1] = 3.910247;
            table[74, 1] = 3.414892;
            table[75, 1] = 2.944803;
            table[76, 1] = 2.498359;
            table[77, 1] = 2.074111;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_1t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.265583;
            table[1, 1] = 101.606995;
            table[2, 1] = 98.480438;
            table[3, 1] = 96.283149;
            table[4, 1] = 94.536955;
            table[5, 1] = 93.027661;
            table[6, 1] = 91.64043;
            table[7, 1] = 90.315292;
            table[8, 1] = 89.029539;
            table[9, 1] = 87.768688;
            table[10, 1] = 86.533742;
            table[11, 1] = 85.329992;
            table[12, 1] = 84.163081;
            table[13, 1] = 83.037195;
            table[14, 1] = 81.954593;
            table[15, 1] = 80.915818;
            table[16, 1] = 79.920119;
            table[17, 1] = 78.975531;
            table[18, 1] = 78.05106;
            table[19, 1] = 77.173323;
            table[20, 1] = 73.278397;
            table[21, 1] = 70.001471;
            table[22, 1] = 67.234011;
            table[23, 1] = 64.856245;
            table[24, 1] = 62.788906;
            table[25, 1] = 60.973352;
            table[26, 1] = 59.365698;
            table[27, 1] = 57.931946;
            table[28, 1] = 56.644568;
            table[29, 1] = 55.480391;
            table[30, 1] = 54.419437;
            table[31, 1] = 53.444383;
            table[32, 1] = 52.540355;
            table[33, 1] = 51.694829;
            table[34, 1] = 50.8975;
            table[35, 1] = 50.140059;
            table[36, 1] = 48.719826;
            table[37, 1] = 47.396376;
            table[38, 1] = 46.146131;
            table[39, 1] = 44.954016;
            table[40, 1] = 43.810073;
            table[41, 1] = 42.707554;
            table[42, 1] = 41.641833;
            table[43, 1] = 40.609043;
            table[44, 1] = 39.606685;
            table[45, 1] = 38.632752;
            table[46, 1] = 36.312044;
            table[47, 1] = 34.139038;
            table[48, 1] = 32.098171;
            table[49, 1] = 30.17647;
            table[50, 1] = 28.384745;
            table[51, 1] = 26.647669;
            table[52, 1] = 25.022855;
            table[53, 1] = 23.481466;
            table[54, 1] = 22.017639;
            table[55, 1] = 20.626393;
            table[56, 1] = 19.325843;
            table[57, 1] = 18.045206;
            table[58, 1] = 16.848335;
            table[59, 1] = 15.70999;
            table[60, 1] = 14.627549;
            table[61, 1] = 13.598579;
            table[62, 1] = 12.62079;
            table[63, 1] = 11.691993;
            table[64, 1] = 10.810084;
            table[65, 1] = 9.972934;
            table[66, 1] = 9.178606;
            table[67, 1] = 8.425067;
            table[68, 1] = 7.710352;
            table[69, 1] = 7.032525;
            table[70, 1] = 6.389678;
            table[71, 1] = 5.77994;
            table[72, 1] = 5.201479;
            table[73, 1] = 4.652506;
            table[74, 1] = 4.131281;
            table[75, 1] = 3.636122;
            table[76, 1] = 3.165328;
            table[77, 1] = 2.717455;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_1t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.265997;
            table[1, 1] = 101.612492;
            table[2, 1] = 98.506034;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.337058;
            table[9, 1] = 89.512871;
            table[10, 1] = 88.741167;
            table[11, 1] = 88.006251;
            table[12, 1] = 87.296038;
            table[13, 1] = 86.601295;
            table[14, 1] = 85.915227;
            table[15, 1] = 85.232835;
            table[16, 1] = 84.559611;
            table[17, 1] = 83.90428;
            table[18, 1] = 83.231501;
            table[19, 1] = 82.570829;
            table[20, 1] = 79.338945;
            table[21, 1] = 76.277918;
            table[22, 1] = 73.501962;
            table[23, 1] = 71.011924;
            table[24, 1] = 68.783874;
            table[25, 1] = 66.785085;
            table[26, 1] = 64.984598;
            table[27, 1] = 63.355675;
            table[28, 1] = 61.875512;
            table[29, 1] = 60.524378;
            table[30, 1] = 59.284907;
            table[31, 1] = 58.141689;
            table[32, 1] = 57.081091;
            table[33, 1] = 56.091189;
            table[34, 1] = 55.161694;
            table[35, 1] = 54.283824;
            table[36, 1] = 52.654435;
            table[37, 1] = 51.156856;
            table[38, 1] = 49.759044;
            table[39, 1] = 48.439073;
            table[40, 1] = 47.182012;
            table[41, 1] = 45.976645;
            table[42, 1] = 44.814987;
            table[43, 1] = 43.696464;
            table[44, 1] = 42.615207;
            table[45, 1] = 41.567658;
            table[46, 1] = 39.081091;
            table[47, 1] = 36.762882;
            table[48, 1] = 34.593118;
            table[49, 1] = 32.555785;
            table[50, 1] = 30.659556;
            table[51, 1] = 28.826956;
            table[52, 1] = 27.114631;
            table[53, 1] = 25.492511;
            table[54, 1] = 23.953842;
            table[55, 1] = 22.492913;
            table[56, 1] = 21.128696;
            table[57, 1] = 19.785501;
            table[58, 1] = 18.531149;
            table[59, 1] = 17.338562;
            table[60, 1] = 16.2048;
            table[61, 1] = 15.127157;
            table[62, 1] = 14.103102;
            table[63, 1] = 13.130236;
            table[64, 1] = 12.206266;
            table[65, 1] = 11.328907;
            table[66, 1] = 10.496065;
            table[67, 1] = 9.705581;
            table[68, 1] = 8.955372;
            table[69, 1] = 8.243395;
            table[70, 1] = 7.567647;
            table[71, 1] = 6.926171;
            table[72, 1] = 6.317053;
            table[73, 1] = 5.738432;
            table[74, 1] = 5.188503;
            table[75, 1] = 4.665521;
            table[76, 1] = 4.167727;
            table[77, 1] = 3.693624;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_100m_1t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.50664;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303439;
            table[12, 1] = 87.704761;
            table[13, 1] = 87.145586;
            table[14, 1] = 86.619619;
            table[15, 1] = 86.121697;
            table[16, 1] = 85.647456;
            table[17, 1] = 85.193198;
            table[18, 1] = 84.758126;
            table[19, 1] = 84.33225;
            table[20, 1] = 82.356601;
            table[21, 1] = 80.453513;
            table[22, 1] = 78.552541;
            table[23, 1] = 76.673317;
            table[24, 1] = 74.809395;
            table[25, 1] = 72.988956;
            table[26, 1] = 71.234757;
            table[27, 1] = 69.56094;
            table[28, 1] = 67.974645;
            table[29, 1] = 66.478322;
            table[30, 1] = 65.071381;
            table[31, 1] = 63.75115;
            table[32, 1] = 62.513422;
            table[33, 1] = 61.352824;
            table[34, 1] = 60.263149;
            table[35, 1] = 59.237694;
            table[36, 1] = 57.35222;
            table[37, 1] = 55.645155;
            table[38, 1] = 54.0742;
            table[39, 1] = 52.607782;
            table[40, 1] = 51.223654;
            table[41, 1] = 49.903532;
            table[42, 1] = 48.63401;
            table[43, 1] = 47.421909;
            table[44, 1] = 46.255524;
            table[45, 1] = 45.128854;
            table[46, 1] = 42.464092;
            table[47, 1] = 39.989547;
            table[48, 1] = 37.680729;
            table[49, 1] = 35.518493;
            table[50, 1] = 33.503727;
            table[51, 1] = 31.573449;
            table[52, 1] = 29.766561;
            table[53, 1] = 28.057264;
            table[54, 1] = 26.437859;
            table[55, 1] = 24.901858;
            table[56, 1] = 23.464242;
            table[57, 1] = 22.05878;
            table[58, 1] = 20.74284;
            table[59, 1] = 19.492275;
            table[60, 1] = 18.303801;
            table[61, 1] = 17.174419;
            table[62, 1] = 16.101339;
            table[63, 1] = 15.081942;
            table[64, 1] = 14.113718;
            table[65, 1] = 13.194264;
            table[66, 1] = 12.321246;
            table[67, 1] = 11.492406;
            table[68, 1] = 10.705535;
            table[69, 1] = 9.958479;
            table[70, 1] = 9.249133;
            table[71, 1] = 8.575449;
            table[72, 1] = 7.935431;
            table[73, 1] = 7.327143;
            table[74, 1] = 6.748712;
            table[75, 1] = 6.198331;
            table[76, 1] = 5.674248;
            table[77, 1] = 5.174797;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }

        private double Get_land_600m_50t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 92.681406;
            table[1, 1] = 81.107521;
            table[2, 1] = 73.479844;
            table[3, 1] = 67.692607;
            table[4, 1] = 63.064352;
            table[5, 1] = 59.229038;
            table[6, 1] = 55.96489;
            table[7, 1] = 53.129751;
            table[8, 1] = 50.628133;
            table[9, 1] = 48.393229;
            table[10, 1] = 46.376627;
            table[11, 1] = 44.54218;
            table[12, 1] = 42.862189;
            table[13, 1] = 41.314956;
            table[14, 1] = 39.883148;
            table[15, 1] = 38.552685;
            table[16, 1] = 37.311955;
            table[17, 1] = 36.151258;
            table[18, 1] = 35.062392;
            table[19, 1] = 34.038353;
            table[20, 1] = 29.70409;
            table[21, 1] = 26.338804;
            table[22, 1] = 23.637778;
            table[23, 1] = 21.410699;
            table[24, 1] = 19.530817;
            table[25, 1] = 17.910054;
            table[26, 1] = 16.485339;
            table[27, 1] = 15.210508;
            table[28, 1] = 14.051204;
            table[29, 1] = 12.981548;
            table[30, 1] = 11.981866;
            table[31, 1] = 11.037112;
            table[32, 1] = 10.135723;
            table[33, 1] = 9.268792;
            table[34, 1] = 8.429446;
            table[35, 1] = 7.612383;
            table[36, 1] = 6.029681;
            table[37, 1] = 4.498043;
            table[38, 1] = 3.004213;
            table[39, 1] = 1.540602;
            table[40, 1] = 0.103019;
            table[41, 1] = -1.310727;
            table[42, 1] = -2.701682;
            table[43, 1] = -4.070278;
            table[44, 1] = -5.416648;
            table[45, 1] = -6.740821;
            table[46, 1] = -9.95452;
            table[47, 1] = -13.032537;
            table[48, 1] = -15.98112;
            table[49, 1] = -18.809484;
            table[50, 1] = -21.52868;
            table[51, 1] = -24.150578;
            table[52, 1] = -26.687129;
            table[53, 1] = -29.149857;
            table[54, 1] = -31.549548;
            table[55, 1] = -33.896061;
            table[56, 1] = -36.198241;
            table[57, 1] = -38.463878;
            table[58, 1] = -40.69971;
            table[59, 1] = -42.911452;
            table[60, 1] = -45.103832;
            table[61, 1] = -47.280643;
            table[62, 1] = -49.444793;
            table[63, 1] = -51.598365;
            table[64, 1] = -53.742674;
            table[65, 1] = -55.878321;
            table[66, 1] = -58.00526;
            table[67, 1] = -60.122848;
            table[68, 1] = -62.229913;
            table[69, 1] = -64.324807;
            table[70, 1] = -66.405472;
            table[71, 1] = -68.469497;
            table[72, 1] = -70.514179;
            table[73, 1] = -72.536584;
            table[74, 1] = -74.533606;
            table[75, 1] = -76.502026;
            table[76, 1] = -78.438569;
            table[77, 1] = -80.339962;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_50t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.867805;
            table[1, 1] = 84.291273;
            table[2, 1] = 77.690067;
            table[3, 1] = 72.674653;
            table[4, 1] = 68.555821;
            table[5, 1] = 65.046542;
            table[6, 1] = 61.991812;
            table[7, 1] = 59.292567;
            table[8, 1] = 56.879415;
            table[9, 1] = 54.70128;
            table[10, 1] = 52.719393;
            table[11, 1] = 50.903652;
            table[12, 1] = 49.230254;
            table[13, 1] = 47.680068;
            table[14, 1] = 46.2375;
            table[15, 1] = 44.889675;
            table[16, 1] = 43.625841;
            table[17, 1] = 42.436922;
            table[18, 1] = 41.31519;
            table[19, 1] = 40.254008;
            table[20, 1] = 35.678927;
            table[21, 1] = 31.998693;
            table[22, 1] = 28.930085;
            table[23, 1] = 26.303887;
            table[24, 1] = 24.01313;
            table[25, 1] = 21.986438;
            table[26, 1] = 20.173317;
            table[27, 1] = 18.535914;
            table[28, 1] = 17.044446;
            table[29, 1] = 15.674697;
            table[30, 1] = 14.406621;
            table[31, 1] = 13.223499;
            table[32, 1] = 12.111386;
            table[33, 1] = 11.058681;
            table[34, 1] = 10.055775;
            table[35, 1] = 9.094739;
            table[36, 1] = 7.273419;
            table[37, 1] = 5.55562;
            table[38, 1] = 3.915369;
            table[39, 1] = 2.335508;
            table[40, 1] = 0.804722;
            table[41, 1] = -0.684478;
            table[42, 1] = -2.137095;
            table[43, 1] = -3.556532;
            table[44, 1] = -4.945178;
            table[45, 1] = -6.304783;
            table[46, 1] = -9.585134;
            table[47, 1] = -12.708581;
            table[48, 1] = -15.68923;
            table[49, 1] = -18.540907;
            table[50, 1] = -21.277483;
            table[51, 1] = -23.912624;
            table[52, 1] = -26.459454;
            table[53, 1] = -28.930294;
            table[54, 1] = -31.336478;
            table[55, 1] = -33.688257;
            table[56, 1] = -35.994757;
            table[57, 1] = -38.263973;
            table[58, 1] = -40.502799;
            table[59, 1] = -42.717066;
            table[60, 1] = -44.911592;
            table[61, 1] = -47.090239;
            table[62, 1] = -49.255971;
            table[63, 1] = -51.410915;
            table[64, 1] = -53.556418;
            table[65, 1] = -55.693112;
            table[66, 1] = -57.820971;
            table[67, 1] = -59.939372;
            table[68, 1] = -62.047159;
            table[69, 1] = -64.142696;
            table[70, 1] = -66.223936;
            table[71, 1] = -68.288477;
            table[72, 1] = -70.333623;
            table[73, 1] = -72.356446;
            table[74, 1] = -74.353847;
            table[75, 1] = -76.322611;
            table[76, 1] = -78.259468;
            table[77, 1] = -80.161146;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_50t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 97.071593;
            table[1, 1] = 87.091699;
            table[2, 1] = 81.045988;
            table[3, 1] = 76.574722;
            table[4, 1] = 72.941728;
            table[5, 1] = 69.834215;
            table[6, 1] = 67.096211;
            table[7, 1] = 64.639948;
            table[8, 1] = 62.410368;
            table[9, 1] = 60.369523;
            table[10, 1] = 58.489279;
            table[11, 1] = 56.747689;
            table[12, 1] = 55.127041;
            table[13, 1] = 53.612713;
            table[14, 1] = 52.192442;
            table[15, 1] = 50.855831;
            table[16, 1] = 49.593977;
            table[17, 1] = 48.399208;
            table[18, 1] = 47.264871;
            table[19, 1] = 46.185163;
            table[20, 1] = 41.447893;
            table[21, 1] = 37.520538;
            table[22, 1] = 34.147736;
            table[23, 1] = 31.181864;
            table[24, 1] = 28.534695;
            table[25, 1] = 26.150562;
            table[26, 1] = 23.991075;
            table[27, 1] = 22.026772;
            table[28, 1] = 20.232915;
            table[29, 1] = 18.587646;
            table[30, 1] = 17.071299;
            table[31, 1] = 15.66623;
            table[32, 1] = 14.356772;
            table[33, 1] = 13.129203;
            table[34, 1] = 11.971651;
            table[35, 1] = 10.873957;
            table[36, 1] = 8.825031;
            table[37, 1] = 6.928696;
            table[38, 1] = 5.147419;
            table[39, 1] = 3.455212;
            table[40, 1] = 1.83411;
            table[41, 1] = 0.271634;
            table[42, 1] = -1.240969;
            table[43, 1] = -2.709938;
            table[44, 1] = -4.139817;
            table[45, 1] = -5.534014;
            table[46, 1] = -8.879506;
            table[47, 1] = -12.0474;
            table[48, 1] = -15.059436;
            table[49, 1] = -17.933938;
            table[50, 1] = -20.687532;
            table[51, 1] = -23.335638;
            table[52, 1] = -25.892532;
            table[53, 1] = -28.371314;
            table[54, 1] = -30.783856;
            table[55, 1] = -33.140791;
            table[56, 1] = -35.451519;
            table[57, 1] = -37.724239;
            table[58, 1] = -39.965996;
            table[59, 1] = -42.182735;
            table[60, 1] = -44.379362;
            table[61, 1] = -46.559808;
            table[62, 1] = -48.72709;
            table[63, 1] = -50.883376;
            table[64, 1] = -53.030049;
            table[65, 1] = -55.167767;
            table[66, 1] = -57.296527;
            table[67, 1] = -59.415726;
            table[68, 1] = -61.524219;
            table[69, 1] = -63.620386;
            table[70, 1] = -65.702189;
            table[71, 1] = -67.767235;
            table[72, 1] = -69.812835;
            table[73, 1] = -71.836069;
            table[74, 1] = -73.833841;
            table[75, 1] = -75.802942;
            table[76, 1] = -77.740105;
            table[77, 1] = -79.642063;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_50t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 99.699425;
            table[1, 1] = 90.356383;
            table[2, 1] = 84.741455;
            table[3, 1] = 80.666574;
            table[4, 1] = 77.421173;
            table[5, 1] = 74.686779;
            table[6, 1] = 72.295849;
            table[7, 1] = 70.151627;
            table[8, 1] = 68.194686;
            table[9, 1] = 66.386707;
            table[10, 1] = 64.70182;
            table[11, 1] = 63.121715;
            table[12, 1] = 61.632786;
            table[13, 1] = 60.224446;
            table[14, 1] = 58.888111;
            table[15, 1] = 57.616594;
            table[16, 1] = 56.403726;
            table[17, 1] = 55.244118;
            table[18, 1] = 54.133012;
            table[19, 1] = 53.066168;
            table[20, 1] = 48.275982;
            table[21, 1] = 44.161797;
            table[22, 1] = 40.517015;
            table[23, 1] = 37.224409;
            table[24, 1] = 34.21899;
            table[25, 1] = 31.463915;
            table[26, 1] = 28.935622;
            table[27, 1] = 26.615571;
            table[28, 1] = 24.486364;
            table[29, 1] = 22.530444;
            table[30, 1] = 20.730086;
            table[31, 1] = 19.06787;
            table[32, 1] = 17.527214;
            table[33, 1] = 16.092782;
            table[34, 1] = 14.750725;
            table[35, 1] = 13.48877;
            table[36, 1] = 11.163745;
            table[37, 1] = 9.048646;
            table[38, 1] = 7.093218;
            table[39, 1] = 5.261431;
            table[40, 1] = 3.527577;
            table[41, 1] = 1.87327;
            table[42, 1] = 0.285253;
            table[43, 1] = -1.246147;
            table[44, 1] = -2.728111;
            table[45, 1] = -4.166082;
            table[46, 1] = -7.594216;
            table[47, 1] = -10.818657;
            table[48, 1] = -13.870709;
            table[49, 1] = -16.774352;
            table[50, 1] = -19.549699;
            table[51, 1] = -22.214391;
            table[52, 1] = -24.78417;
            table[53, 1] = -27.273123;
            table[54, 1] = -29.693811;
            table[55, 1] = -32.057353;
            table[56, 1] = -34.373503;
            table[57, 1] = -36.650716;
            table[58, 1] = -38.896232;
            table[59, 1] = -41.116142;
            table[60, 1] = -43.315465;
            table[61, 1] = -45.498218;
            table[62, 1] = -47.667487;
            table[63, 1] = -49.825496;
            table[64, 1] = -51.97367;
            table[65, 1] = -54.112703;
            table[66, 1] = -56.24262;
            table[67, 1] = -58.362841;
            table[68, 1] = -60.472242;
            table[69, 1] = -62.569217;
            table[70, 1] = -64.651742;
            table[71, 1] = -66.717436;
            table[72, 1] = -68.76362;
            table[73, 1] = -70.78738;
            table[74, 1] = -72.785629;
            table[75, 1] = -74.755162;
            table[76, 1] = -76.692719;
            table[77, 1] = -78.595036;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_50t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 102.345079;
            table[1, 1] = 93.803042;
            table[2, 1] = 88.624052;
            table[3, 1] = 84.877225;
            table[4, 1] = 81.920329;
            table[5, 1] = 79.458796;
            table[6, 1] = 77.332912;
            table[7, 1] = 75.44673;
            table[8, 1] = 73.738588;
            table[9, 1] = 72.167011;
            table[10, 1] = 70.703189;
            table[11, 1] = 69.326623;
            table[12, 1] = 68.022429;
            table[13, 1] = 66.779586;
            table[14, 1] = 65.589765;
            table[15, 1] = 64.446524;
            table[16, 1] = 63.34475;
            table[17, 1] = 62.280284;
            table[18, 1] = 61.249655;
            table[19, 1] = 60.249899;
            table[20, 1] = 55.633514;
            table[21, 1] = 51.500685;
            table[22, 1] = 47.712833;
            table[23, 1] = 44.19358;
            table[24, 1] = 40.905889;
            table[25, 1] = 37.834178;
            table[26, 1] = 34.971802;
            table[27, 1] = 32.31361;
            table[28, 1] = 29.852417;
            table[29, 1] = 27.578009;
            table[30, 1] = 25.477498;
            table[31, 1] = 23.536256;
            table[32, 1] = 21.738902;
            table[33, 1] = 20.070155;
            table[34, 1] = 18.515437;
            table[35, 1] = 17.061255;
            table[36, 1] = 14.407003;
            table[37, 1] = 12.025618;
            table[38, 1] = 9.854794;
            table[39, 1] = 7.848192;
            table[40, 1] = 5.971714;
            table[41, 1] = 4.200312;
            table[42, 1] = 2.515511;
            table[43, 1] = 0.903561;
            table[44, 1] = -0.645903;
            table[45, 1] = -2.140813;
            table[46, 1] = -5.676968;
            table[47, 1] = -8.975721;
            table[48, 1] = -12.08056;
            table[49, 1] = -15.022751;
            table[50, 1] = -17.826929;
            table[51, 1] = -20.513639;
            table[52, 1] = -23.10054;
            table[53, 1] = -25.603023;
            table[54, 1] = -28.034555;
            table[55, 1] = -30.406899;
            table[56, 1] = -32.730272;
            table[57, 1] = -35.013476;
            table[58, 1] = -37.264004;
            table[59, 1] = -39.488144;
            table[60, 1] = -41.691062;
            table[61, 1] = -43.876894;
            table[62, 1] = -46.048817;
            table[63, 1] = -48.209125;
            table[64, 1] = -50.359303;
            table[65, 1] = -52.500091;
            table[66, 1] = -54.631553;
            table[67, 1] = -56.753139;
            table[68, 1] = -58.863751;
            table[69, 1] = -60.961805;
            table[70, 1] = -63.045296;
            table[71, 1] = -65.111855;
            table[72, 1] = -67.158818;
            table[73, 1] = -69.183282;
            table[74, 1] = -71.182167;
            table[75, 1] = -73.152279;
            table[76, 1] = -75.090361;
            table[77, 1] = -76.993158;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_50t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 104.590813;
            table[1, 1] = 97.071105;
            table[2, 1] = 92.462483;
            table[3, 1] = 89.107463;
            table[4, 1] = 86.456594;
            table[5, 1] = 84.255654;
            table[6, 1] = 82.365127;
            table[7, 1] = 80.699845;
            table[8, 1] = 79.203884;
            table[9, 1] = 77.838549;
            table[10, 1] = 76.576006;
            table[11, 1] = 75.39565;
            table[12, 1] = 74.281893;
            table[13, 1] = 73.222746;
            table[14, 1] = 72.208868;
            table[15, 1] = 71.232909;
            table[16, 1] = 70.289028;
            table[17, 1] = 69.372546;
            table[18, 1] = 68.479686;
            table[19, 1] = 67.607368;
            table[20, 1] = 63.478802;
            table[21, 1] = 59.617468;
            table[22, 1] = 55.934657;
            table[23, 1] = 52.395075;
            table[24, 1] = 48.992311;
            table[25, 1] = 45.734073;
            table[26, 1] = 42.632485;
            table[27, 1] = 39.698336;
            table[28, 1] = 36.938232;
            table[29, 1] = 34.353689;
            table[30, 1] = 31.941426;
            table[31, 1] = 29.694268;
            table[32, 1] = 27.602277;
            table[33, 1] = 25.653839;
            table[34, 1] = 23.836586;
            table[35, 1] = 22.138097;
            table[36, 1] = 19.050231;
            table[37, 1] = 16.304306;
            table[38, 1] = 13.829681;
            table[39, 1] = 11.570696;
            table[40, 1] = 9.484468;
            table[41, 1] = 7.538297;
            table[42, 1] = 5.707297;
            table[43, 1] = 3.97247;
            table[44, 1] = 2.319201;
            table[45, 1] = 0.736132;
            table[46, 1] = -2.968766;
            table[47, 1] = -6.38474;
            table[48, 1] = -9.573406;
            table[49, 1] = -12.577108;
            table[50, 1] = -15.427457;
            table[51, 1] = -18.149519;
            table[52, 1] = -20.763969;
            table[53, 1] = -23.288256;
            table[54, 1] = -25.737285;
            table[55, 1] = -28.123844;
            table[56, 1] = -30.458895;
            table[57, 1] = -32.751788;
            table[58, 1] = -35.01043;
            table[59, 1] = -37.241418;
            table[60, 1] = -39.450163;
            table[61, 1] = -41.640984;
            table[62, 1] = -43.817207;
            table[63, 1] = -45.981243;
            table[64, 1] = -48.134671;
            table[65, 1] = -50.278306;
            table[66, 1] = -52.412274;
            table[67, 1] = -54.536075;
            table[68, 1] = -56.648653;
            table[69, 1] = -58.74846;
            table[70, 1] = -60.833517;
            table[71, 1] = -62.901482;
            table[72, 1] = -64.949709;
            table[73, 1] = -66.975315;
            table[74, 1] = -68.975234;
            table[75, 1] = -70.946284;
            table[76, 1] = -72.885221;
            table[77, 1] = -74.788796;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_50t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.006874;
            table[1, 1] = 99.416996;
            table[2, 1] = 95.442795;
            table[3, 1] = 92.5621;
            table[4, 1] = 90.289698;
            table[5, 1] = 88.405993;
            table[6, 1] = 86.792017;
            table[7, 1] = 85.375659;
            table[8, 1] = 84.10968;
            table[9, 1] = 82.96132;
            table[10, 1] = 81.90685;
            table[11, 1] = 80.928481;
            table[12, 1] = 80.01251;
            table[13, 1] = 79.148142;
            table[14, 1] = 78.326718;
            table[15, 1] = 77.541189;
            table[16, 1] = 76.785742;
            table[17, 1] = 76.055535;
            table[18, 1] = 75.346498;
            table[19, 1] = 74.655187;
            table[20, 1] = 71.375191;
            table[21, 1] = 68.236575;
            table[22, 1] = 65.124612;
            table[23, 1] = 61.998754;
            table[24, 1] = 58.862088;
            table[25, 1] = 55.739227;
            table[26, 1] = 52.660997;
            table[27, 1] = 49.656187;
            table[28, 1] = 46.74828;
            table[29, 1] = 43.954703;
            table[30, 1] = 41.287076;
            table[31, 1] = 38.751845;
            table[32, 1] = 36.351082;
            table[33, 1] = 34.083331;
            table[34, 1] = 31.944465;
            table[35, 1] = 29.928466;
            table[36, 1] = 26.235472;
            table[37, 1] = 22.940981;
            table[38, 1] = 19.982089;
            table[39, 1] = 17.302337;
            table[40, 1] = 14.853782;
            table[41, 1] = 12.596991;
            table[42, 1] = 10.50008;
            table[43, 1] = 8.53749;
            table[44, 1] = 6.688772;
            table[45, 1] = 4.937529;
            table[46, 1] = 0.905142;
            table[47, 1] = -2.74266;
            table[48, 1] = -6.09929;
            table[49, 1] = -9.227398;
            table[50, 1] = -12.171768;
            table[51, 1] = -14.966197;
            table[52, 1] = -17.637264;
            table[53, 1] = -20.206501;
            table[54, 1] = -22.691691;
            table[55, 1] = -25.107686;
            table[56, 1] = -27.466958;
            table[57, 1] = -29.779975;
            table[58, 1] = -32.055485;
            table[59, 1] = -34.300728;
            table[60, 1] = -36.521606;
            table[61, 1] = -38.722826;
            table[62, 1] = -40.908017;
            table[63, 1] = -43.079833;
            table[64, 1] = -45.240044;
            table[65, 1] = -47.389625;
            table[66, 1] = -49.528827;
            table[67, 1] = -51.657256;
            table[68, 1] = -53.773944;
            table[69, 1] = -55.877413;
            table[70, 1] = -57.965745;
            table[71, 1] = -60.036649;
            table[72, 1] = -62.087522;
            table[73, 1] = -64.115517;
            table[74, 1] = -66.117598;
            table[75, 1] = -68.090612;
            table[76, 1] = -70.031335;
            table[77, 1] = -71.936541;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_50t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.628848;
            table[1, 1] = 100.483698;
            table[2, 1] = 96.865538;
            table[3, 1] = 94.285356;
            table[4, 1] = 92.275275;
            table[5, 1] = 90.626381;
            table[6, 1] = 89.226956;
            table[7, 1] = 88.010127;
            table[8, 1] = 86.932629;
            table[9, 1] = 85.964811;
            table[10, 1] = 85.085439;
            table[11, 1] = 84.27876;
            table[12, 1] = 83.532751;
            table[13, 1] = 82.83801;
            table[14, 1] = 82.187036;
            table[15, 1] = 81.573742;
            table[16, 1] = 80.993108;
            table[17, 1] = 80.440941;
            table[18, 1] = 79.913695;
            table[19, 1] = 79.408336;
            table[20, 1] = 77.128591;
            table[21, 1] = 75.108034;
            table[22, 1] = 73.199613;
            table[23, 1] = 71.29644;
            table[24, 1] = 69.318044;
            table[25, 1] = 67.213324;
            table[26, 1] = 64.965799;
            table[27, 1] = 62.5906;
            table[28, 1] = 60.122366;
            table[29, 1] = 57.601436;
            table[30, 1] = 55.064696;
            table[31, 1] = 52.542021;
            table[32, 1] = 50.056136;
            table[33, 1] = 47.623744;
            table[34, 1] = 45.256796;
            table[35, 1] = 42.963534;
            table[36, 1] = 38.616988;
            table[37, 1] = 34.601441;
            table[38, 1] = 30.91036;
            table[39, 1] = 27.523312;
            table[40, 1] = 24.412992;
            table[41, 1] = 21.549886;
            table[42, 1] = 18.905095;
            table[43, 1] = 16.451857;
            table[44, 1] = 14.166209;
            table[45, 1] = 12.027137;
            table[46, 1] = 7.207561;
            table[47, 1] = 2.976576;
            table[48, 1] = -0.816251;
            table[49, 1] = -4.274936;
            table[50, 1] = -7.473426;
            table[51, 1] = -10.465971;
            table[52, 1] = -13.293581;
            table[53, 1] = -15.988072;
            table[54, 1] = -18.574647;
            table[55, 1] = -21.07359;
            table[56, 1] = -23.501391;
            table[57, 1] = -25.871538;
            table[58, 1] = -28.195069;
            table[59, 1] = -30.480985;
            table[60, 1] = -32.736556;
            table[61, 1] = -34.96756;
            table[62, 1] = -37.178474;
            table[63, 1] = -39.37263;
            table[64, 1] = -41.552345;
            table[65, 1] = -43.719034;
            table[66, 1] = -45.873312;
            table[67, 1] = -48.015082;
            table[68, 1] = -50.143622;
            table[69, 1] = -52.25766;
            table[70, 1] = -54.355449;
            table[71, 1] = -56.434844;
            table[72, 1] = -58.493364;
            table[73, 1] = -60.528264;
            table[74, 1] = -62.536601;
            table[75, 1] = -64.515295;
            table[76, 1] = -66.461189;
            table[77, 1] = -68.371114;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_10t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 92.787962;
            table[1, 1] = 81.955511;
            table[2, 1] = 74.84835;
            table[3, 1] = 69.340279;
            table[4, 1] = 64.859858;
            table[5, 1] = 61.111435;
            table[6, 1] = 57.904857;
            table[7, 1] = 55.112045;
            table[8, 1] = 52.644276;
            table[9, 1] = 50.438319;
            table[10, 1] = 48.447837;
            table[11, 1] = 46.637989;
            table[12, 1] = 44.981958;
            table[13, 1] = 43.458667;
            table[14, 1] = 42.051242;
            table[15, 1] = 40.745945;
            table[16, 1] = 39.531424;
            table[17, 1] = 38.398175;
            table[18, 1] = 37.338143;
            table[19, 1] = 36.344429;
            table[20, 1] = 32.185897;
            table[21, 1] = 29.036014;
            table[22, 1] = 26.584373;
            table[23, 1] = 24.632372;
            table[24, 1] = 23.044678;
            table[25, 1] = 21.725369;
            table[26, 1] = 20.60469;
            table[27, 1] = 19.631041;
            table[28, 1] = 18.765833;
            table[29, 1] = 17.98002;
            table[30, 1] = 17.251677;
            table[31, 1] = 16.564254;
            table[32, 1] = 15.905293;
            table[33, 1] = 15.265463;
            table[34, 1] = 14.637831;
            table[35, 1] = 14.017301;
            table[36, 1] = 12.783889;
            table[37, 1] = 11.547204;
            table[38, 1] = 10.299581;
            table[39, 1] = 9.039237;
            table[40, 1] = 7.767617;
            table[41, 1] = 6.487817;
            table[42, 1] = 5.203641;
            table[43, 1] = 3.919055;
            table[44, 1] = 2.637858;
            table[45, 1] = 1.363503;
            table[46, 1] = -1.773736;
            table[47, 1] = -4.815708;
            table[48, 1] = -7.743408;
            table[49, 1] = -10.549972;
            table[50, 1] = -13.236992;
            table[51, 1] = -15.811615;
            table[52, 1] = -18.284436;
            table[53, 1] = -20.668038;
            table[54, 1] = -22.97598;
            table[55, 1] = -25.2221;
            table[56, 1] = -27.420022;
            table[57, 1] = -29.582806;
            table[58, 1] = -31.722669;
            table[59, 1] = -33.85077;
            table[60, 1] = -35.977015;
            table[61, 1] = -38.109887;
            table[62, 1] = -40.256285;
            table[63, 1] = -42.421374;
            table[64, 1] = -44.608459;
            table[65, 1] = -46.818869;
            table[66, 1] = -49.051879;
            table[67, 1] = -51.304673;
            table[68, 1] = -53.572351;
            table[69, 1] = -55.848002;
            table[70, 1] = -58.122844;
            table[71, 1] = -60.386437;
            table[72, 1] = -62.626976;
            table[73, 1] = -64.83165;
            table[74, 1] = -66.987076;
            table[75, 1] = -69.079776;
            table[76, 1] = -71.096695;
            table[77, 1] = -73.025712;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_10t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.891575;
            table[1, 1] = 84.746633;
            table[2, 1] = 78.44614;
            table[3, 1] = 73.650088;
            table[4, 1] = 69.685977;
            table[5, 1] = 66.284505;
            table[6, 1] = 63.306282;
            table[7, 1] = 60.663443;
            table[8, 1] = 58.293923;
            table[9, 1] = 56.151318;
            table[10, 1] = 54.19986;
            table[11, 1] = 52.411427;
            table[12, 1] = 50.763554;
            table[13, 1] = 49.238048;
            table[14, 1] = 47.819987;
            table[15, 1] = 46.496988;
            table[16, 1] = 45.258664;
            table[17, 1] = 44.096216;
            table[18, 1] = 43.002123;
            table[19, 1] = 41.969904;
            table[20, 1] = 37.562547;
            table[21, 1] = 34.090615;
            table[22, 1] = 31.268602;
            table[23, 1] = 28.921506;
            table[24, 1] = 26.93495;
            table[25, 1] = 25.22953;
            table[26, 1] = 23.746849;
            table[27, 1] = 22.441872;
            table[28, 1] = 21.278738;
            table[29, 1] = 20.228411;
            table[30, 1] = 19.26726;
            table[31, 1] = 18.376093;
            table[32, 1] = 17.539409;
            table[33, 1] = 16.744785;
            table[34, 1] = 15.982341;
            table[35, 1] = 15.244292;
            table[36, 1] = 13.818432;
            table[37, 1] = 12.433527;
            table[38, 1] = 11.070368;
            table[39, 1] = 9.718838;
            table[40, 1] = 8.374391;
            table[41, 1] = 7.035791;
            table[42, 1] = 5.703681;
            table[43, 1] = 4.379674;
            table[44, 1] = 3.065798;
            table[45, 1] = 1.764152;
            table[46, 1] = -1.424085;
            table[47, 1] = -4.500472;
            table[48, 1] = -7.452263;
            table[49, 1] = -10.276233;
            table[50, 1] = -12.976173;
            table[51, 1] = -15.560607;
            table[52, 1] = -18.041026;
            table[53, 1] = -20.430612;
            table[54, 1] = -22.743337;
            table[55, 1] = -24.993328;
            table[56, 1] = -27.194422;
            table[57, 1] = -29.35983;
            table[58, 1] = -31.501886;
            table[59, 1] = -33.631834;
            table[60, 1] = -35.759647;
            table[61, 1] = -37.893859;
            table[62, 1] = -40.04141;
            table[63, 1] = -42.207497;
            table[64, 1] = -44.395451;
            table[65, 1] = -46.606621;
            table[66, 1] = -48.8403;
            table[67, 1] = -51.093683;
            table[68, 1] = -53.361884;
            table[69, 1] = -55.638;
            table[70, 1] = -57.913258;
            table[71, 1] = -60.177224;
            table[72, 1] = -62.418098;
            table[73, 1] = -64.623074;
            table[74, 1] = -66.778773;
            table[75, 1] = -68.871722;
            table[76, 1] = -70.888866;
            table[77, 1] = -72.818089;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_10t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 97.075752;
            table[1, 1] = 87.448916;
            table[2, 1] = 81.617113;
            table[3, 1] = 77.292132;
            table[4, 1] = 73.762043;
            table[5, 1] = 70.727176;
            table[6, 1] = 68.040841;
            table[7, 1] = 65.622186;
            table[8, 1] = 63.421136;
            table[9, 1] = 61.403198;
            table[10, 1] = 59.542579;
            table[11, 1] = 57.818902;
            table[12, 1] = 56.215515;
            table[13, 1] = 54.71852;
            table[14, 1] = 53.316159;
            table[15, 1] = 51.998391;
            table[16, 1] = 50.756573;
            table[17, 1] = 49.583221;
            table[18, 1] = 48.47182;
            table[19, 1] = 47.41667;
            table[20, 1] = 42.828524;
            table[21, 1] = 39.095815;
            table[22, 1] = 35.961723;
            table[23, 1] = 33.274223;
            table[24, 1] = 30.938463;
            table[25, 1] = 28.890545;
            table[26, 1] = 27.083121;
            table[27, 1] = 25.477971;
            table[28, 1] = 24.042562;
            table[29, 1] = 22.74862;
            table[30, 1] = 21.571573;
            table[31, 1] = 20.49029;
            table[32, 1] = 19.486843;
            table[33, 1] = 18.546231;
            table[34, 1] = 17.656056;
            table[35, 1] = 16.806191;
            table[36, 1] = 15.196234;
            table[37, 1] = 13.668674;
            table[38, 1] = 12.193806;
            table[39, 1] = 10.753813;
            table[40, 1] = 9.33854;
            table[41, 1] = 7.942646;
            table[42, 1] = 6.563747;
            table[43, 1] = 5.201196;
            table[44, 1] = 3.855314;
            table[45, 1] = 2.526892;
            table[46, 1] = -0.7115;
            table[47, 1] = -3.821814;
            table[48, 1] = -6.79736;
            table[49, 1] = -9.638474;
            table[50, 1] = -12.351116;
            table[51, 1] = -14.945177;
            table[52, 1] = -17.433037;
            table[53, 1] = -19.828474;
            table[54, 1] = -22.145869;
            table[55, 1] = -24.399639;
            table[56, 1] = -26.603825;
            table[57, 1] = -28.77179;
            table[58, 1] = -30.915981;
            table[59, 1] = -33.047727;
            table[60, 1] = -35.177066;
            table[61, 1] = -37.312583;
            table[62, 1] = -39.461256;
            table[63, 1] = -41.628315;
            table[64, 1] = -43.817114;
            table[65, 1] = -46.029024;
            table[66, 1] = -48.263352;
            table[67, 1] = -50.51731;
            table[68, 1] = -52.78602;
            table[69, 1] = -55.062589;
            table[70, 1] = -57.338251;
            table[71, 1] = -59.60258;
            table[72, 1] = -61.84378;
            table[73, 1] = -64.04905;
            table[74, 1] = -66.205015;
            table[75, 1] = -68.298205;
            table[76, 1] = -70.315568;
            table[77, 1] = -72.244991;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_10t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 99.699425;
            table[1, 1] = 90.67242;
            table[2, 1] = 85.245929;
            table[3, 1] = 81.293582;
            table[4, 1] = 78.128189;
            table[5, 1] = 75.443322;
            table[6, 1] = 73.079615;
            table[7, 1] = 70.946788;
            table[8, 1] = 68.990725;
            table[9, 1] = 67.177343;
            table[10, 1] = 65.483986;
            table[11, 1] = 63.894653;
            table[12, 1] = 62.39731;
            table[13, 1] = 60.982385;
            table[14, 1] = 59.641912;
            table[15, 1] = 58.369048;
            table[16, 1] = 57.15779;
            table[17, 1] = 56.002808;
            table[18, 1] = 54.899332;
            table[19, 1] = 53.843075;
            table[20, 1] = 49.147676;
            table[21, 1] = 45.191809;
            table[22, 1] = 41.76215;
            table[23, 1] = 38.735452;
            table[24, 1] = 36.039575;
            table[25, 1] = 33.628792;
            table[26, 1] = 31.469398;
            table[27, 1] = 29.532452;
            table[28, 1] = 27.790883;
            table[29, 1] = 26.218876;
            table[30, 1] = 24.792168;
            table[31, 1] = 23.488529;
            table[32, 1] = 22.288112;
            table[33, 1] = 21.173598;
            table[34, 1] = 20.130158;
            table[35, 1] = 19.145286;
            table[36, 1] = 17.311426;
            table[37, 1] = 15.609099;
            table[38, 1] = 13.996736;
            table[39, 1] = 12.447569;
            table[40, 1] = 10.944765;
            table[41, 1] = 9.478023;
            table[42, 1] = 8.041247;
            table[43, 1] = 6.631006;
            table[44, 1] = 5.245501;
            table[45, 1] = 3.883908;
            table[46, 1] = 0.583261;
            table[47, 1] = -2.569315;
            table[48, 1] = -5.574565;
            table[49, 1] = -8.437187;
            table[50, 1] = -11.165806;
            table[51, 1] = -13.772;
            table[52, 1] = -16.269253;
            table[53, 1] = -18.672083;
            table[54, 1] = -20.995384;
            table[55, 1] = -23.253933;
            table[56, 1] = -25.462032;
            table[57, 1] = -27.633235;
            table[58, 1] = -29.780129;
            table[59, 1] = -31.914153;
            table[60, 1] = -34.045425;
            table[61, 1] = -36.182594;
            table[62, 1] = -38.332689;
            table[63, 1] = -40.500979;
            table[64, 1] = -42.690849;
            table[65, 1] = -44.903696;
            table[66, 1] = -47.138848;
            table[67, 1] = -49.393533;
            table[68, 1] = -51.662888;
            table[69, 1] = -53.940031;
            table[70, 1] = -56.216206;
            table[71, 1] = -58.480994;
            table[72, 1] = -60.722607;
            table[73, 1] = -62.92825;
            table[74, 1] = -65.084552;
            table[75, 1] = -67.178048;
            table[76, 1] = -69.195689;
            table[77, 1] = -71.125365;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_10t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 102.345079;
            table[1, 1] = 94.076149;
            table[2, 1] = 89.0757;
            table[3, 1] = 85.450752;
            table[4, 1] = 82.577487;
            table[5, 1] = 80.170647;
            table[6, 1] = 78.07617;
            table[7, 1] = 76.202297;
            table[8, 1] = 74.491012;
            table[9, 1] = 72.904246;
            table[10, 1] = 71.416404;
            table[11, 1] = 70.009935;
            table[12, 1] = 68.672546;
            table[13, 1] = 67.395394;
            table[14, 1] = 66.171891;
            table[15, 1] = 64.996923;
            table[16, 1] = 63.866342;
            table[17, 1] = 62.776647;
            table[18, 1] = 61.724784;
            table[19, 1] = 60.708022;
            table[20, 1] = 56.070824;
            table[21, 1] = 52.014926;
            table[22, 1] = 48.3861;
            table[23, 1] = 45.095103;
            table[24, 1] = 42.093952;
            table[25, 1] = 39.356196;
            table[26, 1] = 36.863694;
            table[27, 1] = 34.599507;
            table[28, 1] = 32.54514;
            table[29, 1] = 30.680291;
            table[30, 1] = 28.983695;
            table[31, 1] = 27.434217;
            table[32, 1] = 26.01177;
            table[33, 1] = 24.697936;
            table[34, 1] = 23.476291;
            table[35, 1] = 22.332497;
            table[36, 1] = 20.231102;
            table[37, 1] = 18.316595;
            table[38, 1] = 16.535308;
            table[39, 1] = 14.850809;
            table[40, 1] = 13.238767;
            table[41, 1] = 11.683138;
            table[42, 1] = 10.173447;
            table[43, 1] = 8.702912;
            table[44, 1] = 7.267167;
            table[45, 1] = 5.863402;
            table[46, 1] = 2.483309;
            table[47, 1] = -0.723457;
            table[48, 1] = -3.766942;
            table[49, 1] = -6.657335;
            table[50, 1] = -9.406632;
            table[51, 1] = -12.028558;
            table[52, 1] = -14.538004;
            table[53, 1] = -16.950444;
            table[54, 1] = -19.281427;
            table[55, 1] = -21.546197;
            table[56, 1] = -23.759392;
            table[57, 1] = -25.934813;
            table[58, 1] = -28.085231;
            table[59, 1] = -30.222223;
            table[60, 1] = -32.356015;
            table[61, 1] = -34.49534;
            table[62, 1] = -36.647289;
            table[63, 1] = -38.817184;
            table[64, 1] = -41.008452;
            table[65, 1] = -43.222521;
            table[66, 1] = -45.458748;
            table[67, 1] = -47.714383;
            table[68, 1] = -49.984578;
            table[69, 1] = -52.26247;
            table[70, 1] = -54.539314;
            table[71, 1] = -56.804702;
            table[72, 1] = -59.046855;
            table[73, 1] = -61.252984;
            table[74, 1] = -63.409726;
            table[75, 1] = -65.503621;
            table[76, 1] = -67.521625;
            table[77, 1] = -69.451631;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_10t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 104.590813;
            table[1, 1] = 97.267435;
            table[2, 1] = 92.811922;
            table[3, 1] = 89.573535;
            table[4, 1] = 87.011426;
            table[5, 1] = 84.87677;
            table[6, 1] = 83.033363;
            table[7, 1] = 81.398237;
            table[8, 1] = 79.917143;
            table[9, 1] = 78.552881;
            table[10, 1] = 77.279128;
            table[11, 1] = 76.076882;
            table[12, 1] = 74.932267;
            table[13, 1] = 73.835079;
            table[14, 1] = 72.777773;
            table[15, 1] = 71.754739;
            table[16, 1] = 70.761765;
            table[17, 1] = 69.795638;
            table[18, 1] = 68.853859;
            table[19, 1] = 67.934428;
            table[20, 1] = 63.622062;
            table[21, 1] = 59.690331;
            table[22, 1] = 56.050512;
            table[23, 1] = 52.651122;
            table[24, 1] = 49.468402;
            table[25, 1] = 46.494793;
            table[26, 1] = 43.728813;
            table[27, 1] = 41.168495;
            table[28, 1] = 38.808275;
            table[29, 1] = 36.638339;
            table[30, 1] = 34.645337;
            table[31, 1] = 32.813622;
            table[32, 1] = 31.126525;
            table[33, 1] = 29.567404;
            table[34, 1] = 28.120388;
            table[35, 1] = 26.770844;
            table[36, 1] = 24.313089;
            table[37, 1] = 22.107167;
            table[38, 1] = 20.088401;
            table[39, 1] = 18.210186;
            table[40, 1] = 16.439556;
            table[41, 1] = 14.753427;
            table[42, 1] = 13.135709;
            table[43, 1] = 11.575184;
            table[44, 1] = 10.063992;
            table[45, 1] = 8.596573;
            table[46, 1] = 5.095743;
            table[47, 1] = 1.805992;
            table[48, 1] = -1.296359;
            table[49, 1] = -4.229673;
            table[50, 1] = -7.011021;
            table[51, 1] = -9.657384;
            table[52, 1] = -12.185806;
            table[53, 1] = -14.613216;
            table[54, 1] = -16.95618;
            table[55, 1] = -19.230661;
            table[56, 1] = -21.451815;
            table[57, 1] = -23.633828;
            table[58, 1] = -25.789755;
            table[59, 1] = -27.93139;
            table[60, 1] = -30.069125;
            table[61, 1] = -32.211822;
            table[62, 1] = -34.366673;
            table[63, 1] = -36.539082;
            table[64, 1] = -38.732538;
            table[65, 1] = -40.948522;
            table[66, 1] = -43.186433;
            table[67, 1] = -45.443553;
            table[68, 1] = -47.715067;
            table[69, 1] = -49.994133;
            table[70, 1] = -52.272025;
            table[71, 1] = -54.538352;
            table[72, 1] = -56.78135;
            table[73, 1] = -58.988241;
            table[74, 1] = -61.145673;
            table[75, 1] = -63.240193;
            table[76, 1] = -65.258765;
            table[77, 1] = -67.18929;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_10t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.006874;
            table[1, 1] = 99.510683;
            table[2, 1] = 95.623189;
            table[3, 1] = 92.818755;
            table[4, 1] = 90.612511;
            table[5, 1] = 88.785529;
            table[6, 1] = 87.219342;
            table[7, 1] = 85.842137;
            table[8, 1] = 84.60683;
            table[9, 1] = 83.480739;
            table[10, 1] = 82.440192;
            table[11, 1] = 81.467497;
            table[12, 1] = 80.549121;
            table[13, 1] = 79.674542;
            table[14, 1] = 78.835498;
            table[15, 1] = 78.025456;
            table[16, 1] = 77.239238;
            table[17, 1] = 76.472735;
            table[18, 1] = 75.72269;
            table[19, 1] = 74.986521;
            table[20, 1] = 71.455233;
            table[21, 1] = 68.236575;
            table[22, 1] = 65.124612;
            table[23, 1] = 61.998754;
            table[24, 1] = 58.862088;
            table[25, 1] = 55.739227;
            table[26, 1] = 52.782258;
            table[27, 1] = 50.058724;
            table[28, 1] = 47.476462;
            table[29, 1] = 45.041906;
            table[30, 1] = 42.756561;
            table[31, 1] = 40.617573;
            table[32, 1] = 38.618686;
            table[33, 1] = 36.751275;
            table[34, 1] = 35.005302;
            table[35, 1] = 33.370106;
            table[36, 1] = 30.389694;
            table[37, 1] = 27.73071;
            table[38, 1] = 25.325995;
            table[39, 1] = 23.12196;
            table[40, 1] = 21.07729;
            table[41, 1] = 19.160753;
            table[42, 1] = 17.348962;
            table[43, 1] = 15.62446;
            table[44, 1] = 13.974176;
            table[45, 1] = 12.388257;
            table[46, 1] = 8.659775;
            table[47, 1] = 5.211362;
            table[48, 1] = 1.995371;
            table[49, 1] = -1.021371;
            table[50, 1] = -3.865335;
            table[51, 1] = -6.559618;
            table[52, 1] = -9.125356;
            table[53, 1] = -11.582276;
            table[54, 1] = -13.948899;
            table[55, 1] = -16.242581;
            table[56, 1] = -18.479494;
            table[57, 1] = -20.674568;
            table[58, 1] = -22.841421;
            table[59, 1] = -24.99227;
            table[60, 1] = -27.137835;
            table[61, 1] = -29.287231;
            table[62, 1] = -31.447852;
            table[63, 1] = -33.625257;
            table[64, 1] = -35.823065;
            table[65, 1] = -38.042858;
            table[66, 1] = -40.284118;
            table[67, 1] = -42.544197;
            table[68, 1] = -44.818335;
            table[69, 1] = -47.099736;
            table[70, 1] = -49.379716;
            table[71, 1] = -51.647914;
            table[72, 1] = -53.892595;
            table[73, 1] = -56.101004;
            table[74, 1] = -58.259809;
            table[75, 1] = -60.355575;
            table[76, 1] = -62.37528;
            table[77, 1] = -64.306838;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_10t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.628848;
            table[1, 1] = 100.510607;
            table[2, 1] = 96.916807;
            table[3, 1] = 94.358726;
            table[4, 1] = 92.368797;
            table[5, 1] = 90.738297;
            table[6, 1] = 89.35561;
            table[7, 1] = 88.153903;
            table[8, 1] = 87.089904;
            table[9, 1] = 86.133926;
            table[10, 1] = 85.264679;
            table[11, 1] = 84.466343;
            table[12, 1] = 83.726821;
            table[13, 1] = 83.036636;
            table[14, 1] = 82.388223;
            table[15, 1] = 81.77543;
            table[16, 1] = 81.193193;
            table[17, 1] = 80.63728;
            table[18, 1] = 80.104126;
            table[19, 1] = 79.590692;
            table[20, 1] = 77.238846;
            table[21, 1] = 75.108034;
            table[22, 1] = 73.199613;
            table[23, 1] = 71.29644;
            table[24, 1] = 69.318044;
            table[25, 1] = 67.213324;
            table[26, 1] = 64.965799;
            table[27, 1] = 62.5906;
            table[28, 1] = 60.122366;
            table[29, 1] = 57.601436;
            table[30, 1] = 55.064696;
            table[31, 1] = 52.611461;
            table[32, 1] = 50.393508;
            table[33, 1] = 48.25321;
            table[34, 1] = 46.196419;
            table[35, 1] = 44.225793;
            table[36, 1] = 40.542077;
            table[37, 1] = 37.185074;
            table[38, 1] = 34.123272;
            table[39, 1] = 31.320354;
            table[40, 1] = 28.740728;
            table[41, 1] = 26.352229;
            table[42, 1] = 24.127094;
            table[43, 1] = 22.041996;
            table[44, 1] = 20.077633;
            table[45, 1] = 18.218149;
            table[46, 1] = 13.948541;
            table[47, 1] = 10.110744;
            table[48, 1] = 6.609623;
            table[49, 1] = 3.380205;
            table[50, 1] = 0.374753;
            table[51, 1] = -2.444212;
            table[52, 1] = -5.107696;
            table[53, 1] = -7.642321;
            table[54, 1] = -10.071501;
            table[55, 1] = -12.416129;
            table[56, 1] = -14.694965;
            table[57, 1] = -16.924868;
            table[58, 1] = -19.120907;
            table[59, 1] = -21.29641;
            table[60, 1] = -23.462952;
            table[61, 1] = -25.630317;
            table[62, 1] = -27.806426;
            table[63, 1] = -29.997258;
            table[64, 1] = -32.206767;
            table[65, 1] = -34.436809;
            table[66, 1] = -36.687087;
            table[67, 1] = -38.955135;
            table[68, 1] = -41.236343;
            table[69, 1] = -43.524042;
            table[70, 1] = -45.809649;
            table[71, 1] = -48.082896;
            table[72, 1] = -50.332117;
            table[73, 1] = -52.544624;
            table[74, 1] = -54.707136;
            table[75, 1] = -56.806265;
            table[76, 1] = -58.829031;
            table[77, 1] = -60.763378;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_1t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 92.787962;
            table[1, 1] = 82.389516;
            table[2, 1] = 76.030833;
            table[3, 1] = 71.287481;
            table[4, 1] = 67.458572;
            table[5, 1] = 64.232921;
            table[6, 1] = 61.441606;
            table[7, 1] = 58.980752;
            table[8, 1] = 56.781044;
            table[9, 1] = 54.793542;
            table[10, 1] = 52.982253;
            table[11, 1] = 51.319869;
            table[12, 1] = 49.78515;
            table[13, 1] = 48.361237;
            table[14, 1] = 47.034517;
            table[15, 1] = 45.793829;
            table[16, 1] = 44.629907;
            table[17, 1] = 43.534963;
            table[18, 1] = 42.502387;
            table[19, 1] = 41.526512;
            table[20, 1] = 37.346166;
            table[21, 1] = 34.052188;
            table[22, 1] = 31.392725;
            table[23, 1] = 29.206786;
            table[24, 1] = 27.383496;
            table[25, 1] = 25.842613;
            table[26, 1] = 24.524087;
            table[27, 1] = 23.381924;
            table[28, 1] = 22.380322;
            table[29, 1] = 21.491078;
            table[30, 1] = 20.691788;
            table[31, 1] = 19.964538;
            table[32, 1] = 19.294941;
            table[33, 1] = 18.671408;
            table[34, 1] = 18.084582;
            table[35, 1] = 17.526904;
            table[36, 1] = 16.475735;
            table[37, 1] = 15.481843;
            table[38, 1] = 14.521838;
            table[39, 1] = 13.580699;
            table[40, 1] = 12.648927;
            table[41, 1] = 11.720659;
            table[42, 1] = 10.792428;
            table[43, 1] = 9.862336;
            table[44, 1] = 8.929519;
            table[45, 1] = 7.99379;
            table[46, 1] = 5.644035;
            table[47, 1] = 3.287854;
            table[48, 1] = 0.937497;
            table[49, 1] = -1.396135;
            table[50, 1] = -3.705187;
            table[51, 1] = -5.985246;
            table[52, 1] = -8.235054;
            table[53, 1] = -10.455916;
            table[54, 1] = -12.651046;
            table[55, 1] = -14.824934;
            table[56, 1] = -16.982812;
            table[57, 1] = -19.130196;
            table[58, 1] = -21.272521;
            table[59, 1] = -23.414838;
            table[60, 1] = -25.56157;
            table[61, 1] = -27.716309;
            table[62, 1] = -29.881663;
            table[63, 1] = -32.059124;
            table[64, 1] = -34.248978;
            table[65, 1] = -36.450246;
            table[66, 1] = -38.660657;
            table[67, 1] = -40.876655;
            table[68, 1] = -43.093448;
            table[69, 1] = -45.305084;
            table[70, 1] = -47.504579;
            table[71, 1] = -49.68407;
            table[72, 1] = -51.835008;
            table[73, 1] = -53.948382;
            table[74, 1] = -56.014967;
            table[75, 1] = -58.025587;
            table[76, 1] = -59.971388;
            table[77, 1] = -61.844106;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_1t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.891575;
            table[1, 1] = 85.129934;
            table[2, 1] = 79.199274;
            table[3, 1] = 74.800812;
            table[4, 1] = 71.245131;
            table[5, 1] = 68.233154;
            table[6, 1] = 65.608116;
            table[7, 1] = 63.276697;
            table[8, 1] = 61.178028;
            table[9, 1] = 59.269658;
            table[10, 1] = 57.520483;
            table[11, 1] = 55.906856;
            table[12, 1] = 54.41028;
            table[13, 1] = 53.01596;
            table[14, 1] = 51.711845;
            table[15, 1] = 50.487971;
            table[16, 1] = 49.335988;
            table[17, 1] = 48.248826;
            table[18, 1] = 47.220435;
            table[19, 1] = 46.245591;
            table[20, 1] = 42.033366;
            table[21, 1] = 38.66144;
            table[22, 1] = 35.88964;
            table[23, 1] = 33.563942;
            table[24, 1] = 31.579097;
            table[25, 1] = 29.860145;
            table[26, 1] = 28.352156;
            table[27, 1] = 27.013994;
            table[28, 1] = 25.814258;
            table[29, 1] = 24.728553;
            table[30, 1] = 23.737587;
            table[31, 1] = 22.825846;
            table[32, 1] = 21.98066;
            table[33, 1] = 21.19155;
            table[34, 1] = 20.449753;
            table[35, 1] = 19.747888;
            table[36, 1] = 18.439894;
            table[37, 1] = 17.228025;
            table[38, 1] = 16.083741;
            table[39, 1] = 14.986554;
            table[40, 1] = 13.921914;
            table[41, 1] = 12.879638;
            table[42, 1] = 11.85273;
            table[43, 1] = 10.836503;
            table[44, 1] = 9.827925;
            table[45, 1] = 8.825139;
            table[46, 1] = 6.338038;
            table[47, 1] = 3.87779;
            table[48, 1] = 1.448218;
            table[49, 1] = -0.945707;
            table[50, 1] = -3.300659;
            table[51, 1] = -5.615752;
            table[52, 1] = -7.892445;
            table[53, 1] = -10.134104;
            table[54, 1] = -12.345476;
            table[55, 1] = -14.532188;
            table[56, 1] = -16.700306;
            table[57, 1] = -18.855964;
            table[58, 1] = -21.00505;
            table[59, 1] = -23.152953;
            table[60, 1] = -25.304349;
            table[61, 1] = -27.463023;
            table[62, 1] = -29.631725;
            table[63, 1] = -31.812059;
            table[64, 1] = -34.004399;
            table[65, 1] = -36.207833;
            table[66, 1] = -38.420142;
            table[67, 1] = -40.637813;
            table[68, 1] = -42.856088;
            table[69, 1] = -45.069044;
            table[70, 1] = -47.269718;
            table[71, 1] = -49.450267;
            table[72, 1] = -51.602157;
            table[73, 1] = -53.716391;
            table[74, 1] = -55.783754;
            table[75, 1] = -57.795081;
            table[76, 1] = -59.741525;
            table[77, 1] = -61.614831;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_1t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 97.075752;
            table[1, 1] = 87.816328;
            table[2, 1] = 82.229965;
            table[3, 1] = 78.119452;
            table[4, 1] = 74.807924;
            table[5, 1] = 72.000769;
            table[6, 1] = 69.545455;
            table[7, 1] = 67.353399;
            table[8, 1] = 65.368496;
            table[9, 1] = 63.552672;
            table[10, 1] = 61.878609;
            table[11, 1] = 60.325812;
            table[12, 1] = 58.878352;
            table[13, 1] = 57.5235;
            table[14, 1] = 56.250856;
            table[15, 1] = 55.05177;
            table[16, 1] = 53.918943;
            table[17, 1] = 52.846141;
            table[18, 1] = 51.827986;
            table[19, 1] = 50.859797;
            table[20, 1] = 46.639936;
            table[21, 1] = 43.211898;
            table[22, 1] = 40.349835;
            table[23, 1] = 37.907435;
            table[24, 1] = 35.784953;
            table[25, 1] = 33.912207;
            table[26, 1] = 32.238752;
            table[27, 1] = 30.727674;
            table[28, 1] = 29.35145;
            table[29, 1] = 28.089091;
            table[30, 1] = 26.924172;
            table[31, 1] = 25.843465;
            table[32, 1] = 24.836026;
            table[33, 1] = 23.892576;
            table[34, 1] = 23.005106;
            table[35, 1] = 22.166613;
            table[36, 1] = 20.612599;
            table[37, 1] = 19.189312;
            table[38, 1] = 17.864692;
            table[39, 1] = 16.614105;
            table[40, 1] = 15.418916;
            table[41, 1] = 14.265255;
            table[42, 1] = 13.142953;
            table[43, 1] = 12.044675;
            table[44, 1] = 10.965208;
            table[45, 1] = 9.900914;
            table[46, 1] = 7.292185;
            table[47, 1] = 4.744056;
            table[48, 1] = 2.249937;
            table[49, 1] = -0.19206;
            table[50, 1] = -2.583285;
            table[51, 1] = -4.926107;
            table[52, 1] = -7.224278;
            table[53, 1] = -9.48279;
            table[54, 1] = -11.707557;
            table[55, 1] = -13.905046;
            table[56, 1] = -16.081937;
            table[57, 1] = -18.244813;
            table[58, 1] = -20.3999;
            table[59, 1] = -22.552839;
            table[60, 1] = -24.708496;
            table[61, 1] = -26.870804;
            table[62, 1] = -29.04263;
            table[63, 1] = -31.225667;
            table[64, 1] = -33.420358;
            table[65, 1] = -35.62585;
            table[66, 1] = -37.83997;
            table[67, 1] = -40.059242;
            table[68, 1] = -42.278938;
            table[69, 1] = -44.493159;
            table[70, 1] = -46.694966;
            table[71, 1] = -48.876532;
            table[72, 1] = -51.029337;
            table[73, 1] = -53.144398;
            table[74, 1] = -55.212511;
            table[75, 1] = -57.224518;
            table[76, 1] = -59.171582;
            table[77, 1] = -61.045454;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_1t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 99.699425;
            table[1, 1] = 91.03259;
            table[2, 1] = 85.81557;
            table[3, 1] = 82.003623;
            table[4, 1] = 78.949604;
            table[5, 1] = 76.367244;
            table[6, 1] = 74.10718;
            table[7, 1] = 72.083102;
            table[8, 1] = 70.241354;
            table[9, 1] = 68.546553;
            table[10, 1] = 66.974138;
            table[11, 1] = 65.506261;
            table[12, 1] = 64.129406;
            table[13, 1] = 62.832975;
            table[14, 1] = 61.608399;
            table[15, 1] = 60.448578;
            table[16, 1] = 59.347512;
            table[17, 1] = 58.300045;
            table[18, 1] = 57.301695;
            table[19, 1] = 56.348518;
            table[20, 1] = 52.14991;
            table[21, 1] = 48.681864;
            table[22, 1] = 45.739004;
            table[23, 1] = 43.185192;
            table[24, 1] = 40.927076;
            table[25, 1] = 38.899514;
            table[26, 1] = 37.056605;
            table[27, 1] = 35.365721;
            table[28, 1] = 33.803357;
            table[29, 1] = 32.352183;
            table[30, 1] = 30.998975;
            table[31, 1] = 29.733193;
            table[32, 1] = 28.546037;
            table[33, 1] = 27.429852;
            table[34, 1] = 26.377774;
            table[35, 1] = 25.383528;
            table[36, 1] = 23.54582;
            table[37, 1] = 21.875603;
            table[38, 1] = 20.338333;
            table[39, 1] = 18.905694;
            table[40, 1] = 17.555037;
            table[41, 1] = 16.268631;
            table[42, 1] = 15.032827;
            table[43, 1] = 13.837264;
            table[44, 1] = 12.674158;
            table[45, 1] = 11.537714;
            table[46, 1] = 8.787807;
            table[47, 1] = 6.139094;
            table[48, 1] = 3.571792;
            table[49, 1] = 1.075496;
            table[50, 1] = -1.356754;
            table[51, 1] = -3.731097;
            table[52, 1] = -6.053867;
            table[53, 1] = -8.331855;
            table[54, 1] = -10.572246;
            table[55, 1] = -12.782422;
            table[56, 1] = -14.969728;
            table[57, 1] = -17.141242;
            table[58, 1] = -19.303559;
            table[59, 1] = -21.4626;
            table[60, 1] = -23.623448;
            table[61, 1] = -25.790204;
            table[62, 1] = -27.965863;
            table[63, 1] = -30.152225;
            table[64, 1] = -32.349817;
            table[65, 1] = -34.557852;
            table[66, 1] = -36.774211;
            table[67, 1] = -38.995465;
            table[68, 1] = -41.21692;
            table[69, 1] = -43.432712;
            table[70, 1] = -45.635923;
            table[71, 1] = -47.818749;
            table[72, 1] = -49.972691;
            table[73, 1] = -52.088778;
            table[74, 1] = -54.15782;
            table[75, 1] = -56.170672;
            table[76, 1] = -58.118504;
            table[77, 1] = -59.993078;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_1t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 102.345079;
            table[1, 1] = 94.401485;
            table[2, 1] = 89.602283;
            table[3, 1] = 86.105039;
            table[4, 1] = 83.314557;
            table[5, 1] = 80.962682;
            table[6, 1] = 78.907018;
            table[7, 1] = 77.064224;
            table[8, 1] = 75.382318;
            table[9, 1] = 73.827351;
            table[10, 1] = 72.376293;
            table[11, 1] = 71.012966;
            table[12, 1] = 69.725601;
            table[13, 1] = 68.505325;
            table[14, 1] = 67.345212;
            table[15, 1] = 66.239669;
            table[16, 1] = 65.184042;
            table[17, 1] = 64.174363;
            table[18, 1] = 63.207171;
            table[19, 1] = 62.279403;
            table[20, 1] = 58.144081;
            table[21, 1] = 54.66969;
            table[22, 1] = 51.677339;
            table[23, 1] = 49.043073;
            table[24, 1] = 46.680055;
            table[25, 1] = 44.527472;
            table[26, 1] = 42.542992;
            table[27, 1] = 40.697335;
            table[28, 1] = 38.970274;
            table[29, 1] = 37.347669;
            table[30, 1] = 35.819309;
            table[31, 1] = 34.377395;
            table[32, 1] = 33.015528;
            table[33, 1] = 31.728065;
            table[34, 1] = 30.509753;
            table[35, 1] = 29.355545;
            table[36, 1] = 27.21992;
            table[37, 1] = 25.28366;
            table[38, 1] = 23.512426;
            table[39, 1] = 21.876069;
            table[40, 1] = 20.349085;
            table[41, 1] = 18.910523;
            table[42, 1] = 17.543545;
            table[43, 1] = 16.234838;
            table[44, 1] = 14.973989;
            table[45, 1] = 13.752915;
            table[46, 1] = 10.836669;
            table[47, 1] = 8.068877;
            table[48, 1] = 5.414568;
            table[49, 1] = 2.853425;
            table[50, 1] = 0.371938;
            table[51, 1] = -2.040435;
            table[52, 1] = -4.39304;
            table[53, 1] = -6.694766;
            table[54, 1] = -8.954289;
            table[55, 1] = -11.180064;
            table[56, 1] = -13.380225;
            table[57, 1] = -15.562433;
            table[58, 1] = -17.733724;
            table[59, 1] = -19.900358;
            table[60, 1] = -22.067676;
            table[61, 1] = -24.239983;
            table[62, 1] = -26.420435;
            table[63, 1] = -28.610959;
            table[64, 1] = -30.812183;
            table[65, 1] = -33.023404;
            table[66, 1] = -35.242571;
            table[67, 1] = -37.46631;
            table[68, 1] = -39.689974;
            table[69, 1] = -41.907734;
            table[70, 1] = -44.112708;
            table[71, 1] = -46.297118;
            table[72, 1] = -48.452485;
            table[73, 1] = -50.569861;
            table[74, 1] = -52.640071;
            table[75, 1] = -54.653982;
            table[76, 1] = -56.602781;
            table[77, 1] = -58.478236;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_1t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 104.590813;
            table[1, 1] = 97.503201;
            table[2, 1] = 93.220624;
            table[3, 1] = 90.104375;
            table[4, 1] = 87.625313;
            table[5, 1] = 85.542819;
            table[6, 1] = 83.727063;
            table[7, 1] = 82.100577;
            table[8, 1] = 80.61414;
            table[9, 1] = 79.235221;
            table[10, 1] = 77.941753;
            table[11, 1] = 76.718493;
            table[12, 1] = 75.554744;
            table[13, 1] = 74.442858;
            table[14, 1] = 73.377249;
            table[15, 1] = 72.353705;
            table[16, 1] = 71.368945;
            table[17, 1] = 70.4203;
            table[18, 1] = 69.505518;
            table[19, 1] = 68.622629;
            table[20, 1] = 64.62877;
            table[21, 1] = 61.209627;
            table[22, 1] = 58.224964;
            table[23, 1] = 55.567701;
            table[24, 1] = 53.158363;
            table[25, 1] = 50.939519;
            table[26, 1] = 48.870703;
            table[27, 1] = 46.924118;
            table[28, 1] = 45.081133;
            table[29, 1] = 43.329492;
            table[30, 1] = 41.66115;
            table[31, 1] = 40.070675;
            table[32, 1] = 38.554099;
            table[33, 1] = 37.108156;
            table[34, 1] = 35.729807;
            table[35, 1] = 34.415985;
            table[36, 1] = 31.968896;
            table[37, 1] = 29.739406;
            table[38, 1] = 27.698928;
            table[39, 1] = 25.819708;
            table[40, 1] = 24.076325;
            table[41, 1] = 22.446507;
            table[42, 1] = 20.911347;
            table[43, 1] = 19.455157;
            table[44, 1] = 18.065119;
            table[45, 1] = 16.730872;
            table[46, 1] = 13.588653;
            table[47, 1] = 10.656209;
            table[48, 1] = 7.880043;
            table[49, 1] = 5.227192;
            table[50, 1] = 2.675538;
            table[51, 1] = 0.208637;
            table[52, 1] = -2.18696;
            table[53, 1] = -4.523034;
            table[54, 1] = -6.810333;
            table[55, 1] = -9.05882;
            table[56, 1] = -11.277739;
            table[57, 1] = -13.475583;
            table[58, 1] = -15.660019;
            table[59, 1] = -17.837787;
            table[60, 1] = -20.014606;
            table[61, 1] = -22.195071;
            table[62, 1] = -24.382572;
            table[63, 1] = -26.57922;
            table[64, 1] = -28.785794;
            table[65, 1] = -31.001709;
            table[66, 1] = -33.225015;
            table[67, 1] = -35.452418;
            table[68, 1] = -37.679338;
            table[69, 1] = -39.900005;
            table[70, 1] = -42.10758;
            table[71, 1] = -44.294326;
            table[72, 1] = -46.451799;
            table[73, 1] = -48.571077;
            table[74, 1] = -50.64301;
            table[75, 1] = -52.658488;
            table[76, 1] = -54.608713;
            table[77, 1] = -56.485471;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_1t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.006874;
            table[1, 1] = 99.621854;
            table[2, 1] = 95.828749;
            table[3, 1] = 93.101266;
            table[4, 1] = 90.955512;
            table[5, 1] = 89.173276;
            table[6, 1] = 87.636638;
            table[7, 1] = 86.274343;
            table[8, 1] = 85.04003;
            table[9, 1] = 83.902008;
            table[10, 1] = 82.837915;
            table[11, 1] = 81.83169;
            table[12, 1] = 80.871707;
            table[13, 1] = 79.949561;
            table[14, 1] = 79.059216;
            table[15, 1] = 78.196378;
            table[16, 1] = 77.358027;
            table[17, 1] = 76.542056;
            table[18, 1] = 75.746993;
            table[19, 1] = 74.986521;
            table[20, 1] = 71.455233;
            table[21, 1] = 68.236575;
            table[22, 1] = 65.348775;
            table[23, 1] = 62.792784;
            table[24, 1] = 60.455559;
            table[25, 1] = 58.286661;
            table[26, 1] = 56.247649;
            table[27, 1] = 54.310661;
            table[28, 1] = 52.456429;
            table[29, 1] = 50.672278;
            table[30, 1] = 48.950343;
            table[31, 1] = 47.286068;
            table[32, 1] = 45.677027;
            table[33, 1] = 44.122018;
            table[34, 1] = 42.620426;
            table[35, 1] = 41.171782;
            table[36, 1] = 38.430716;
            table[37, 1] = 35.890653;
            table[38, 1] = 33.538597;
            table[39, 1] = 31.357976;
            table[40, 1] = 29.33051;
            table[41, 1] = 27.437839;
            table[42, 1] = 25.662632;
            table[43, 1] = 23.989208;
            table[44, 1] = 22.403792;
            table[45, 1] = 20.894516;
            table[46, 1] = 17.392124;
            table[47, 1] = 14.188349;
            table[48, 1] = 11.206568;
            table[49, 1] = 8.396277;
            table[50, 1] = 5.722597;
            table[51, 1] = 3.159925;
            table[52, 1] = 0.688238;
            table[53, 1] = -1.708999;
            table[54, 1] = -4.045999;
            table[55, 1] = -6.335284;
            table[56, 1] = -8.588008;
            table[57, 1] = -10.814107;
            table[58, 1] = -13.022346;
            table[59, 1] = -15.220318;
            table[60, 1] = -17.4144;
            table[61, 1] = -19.609712;
            table[62, 1] = -21.810055;
            table[63, 1] = -24.017871;
            table[64, 1] = -26.234208;
            table[65, 1] = -28.458698;
            table[66, 1] = -30.689569;
            table[67, 1] = -32.923673;
            table[68, 1] = -35.156553;
            table[69, 1] = -37.382539;
            table[70, 1] = -39.594878;
            table[71, 1] = -41.785904;
            table[72, 1] = -43.947235;
            table[73, 1] = -46.070001;
            table[74, 1] = -48.145095;
            table[75, 1] = -50.163446;
            table[76, 1] = -52.116288;
            table[77, 1] = -53.995435;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_600m_1t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.628848;
            table[1, 1] = 100.542347;
            table[2, 1] = 96.974225;
            table[3, 1] = 94.43736;
            table[4, 1] = 92.464776;
            table[5, 1] = 90.847916;
            table[6, 1] = 89.475111;
            table[7, 1] = 88.279369;
            table[8, 1] = 87.217196;
            table[9, 1] = 86.258669;
            table[10, 1] = 85.382265;
            table[11, 1] = 84.571962;
            table[12, 1] = 83.815504;
            table[13, 1] = 83.103327;
            table[14, 1] = 82.427848;
            table[15, 1] = 81.782992;
            table[16, 1] = 81.193193;
            table[17, 1] = 80.63728;
            table[18, 1] = 80.104126;
            table[19, 1] = 79.590692;
            table[20, 1] = 77.238846;
            table[21, 1] = 75.108034;
            table[22, 1] = 73.199613;
            table[23, 1] = 71.29644;
            table[24, 1] = 69.318044;
            table[25, 1] = 67.213324;
            table[26, 1] = 64.965799;
            table[27, 1] = 62.835433;
            table[28, 1] = 61.202687;
            table[29, 1] = 59.607412;
            table[30, 1] = 58.040829;
            table[31, 1] = 56.497495;
            table[32, 1] = 54.97461;
            table[33, 1] = 53.471342;
            table[34, 1] = 51.988213;
            table[35, 1] = 50.526594;
            table[36, 1] = 47.675281;
            table[37, 1] = 44.93241;
            table[38, 1] = 42.310044;
            table[39, 1] = 39.815103;
            table[40, 1] = 37.449281;
            table[41, 1] = 35.209967;
            table[42, 1] = 33.091455;
            table[43, 1] = 31.086114;
            table[44, 1] = 29.185323;
            table[45, 1] = 27.380179;
            table[46, 1] = 23.22975;
            table[47, 1] = 19.503175;
            table[48, 1] = 16.105442;
            table[49, 1] = 12.9651;
            table[50, 1] = 10.028694;
            table[51, 1] = 7.255577;
            table[52, 1] = 4.614019;
            table[53, 1] = 2.078517;
            table[54, 1] = -0.371971;
            table[55, 1] = -2.755176;
            table[56, 1] = -5.086242;
            table[57, 1] = -7.378181;
            table[58, 1] = -9.642146;
            table[59, 1] = -11.887597;
            table[60, 1] = -14.122386;
            table[61, 1] = -16.352802;
            table[62, 1] = -18.583584;
            table[63, 1] = -20.817929;
            table[64, 1] = -23.057499;
            table[65, 1] = -25.302427;
            table[66, 1] = -27.551353;
            table[67, 1] = -29.801472;
            table[68, 1] = -32.048609;
            table[69, 1] = -34.287333;
            table[70, 1] = -36.511091;
            table[71, 1] = -38.712386;
            table[72, 1] = -40.882978;
            table[73, 1] = -43.01412;
            table[74, 1] = -45.096812;
            table[75, 1] = -47.12207;
            table[76, 1] = -49.081207;
            table[77, 1] = -50.966105;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }

        private double Get_sea_600m_50t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.891195;
            table[1, 1] = 100.2919;
            table[2, 1] = 94.552531;
            table[3, 1] = 89.791493;
            table[4, 1] = 86.014892;
            table[5, 1] = 82.916591;
            table[6, 1] = 80.293789;
            table[7, 1] = 78.019977;
            table[8, 1] = 76.012327;
            table[9, 1] = 74.213734;
            table[10, 1] = 72.583029;
            table[11, 1] = 71.089383;
            table[12, 1] = 69.708929;
            table[13, 1] = 68.422652;
            table[14, 1] = 67.215023;
            table[15, 1] = 66.073102;
            table[16, 1] = 64.985943;
            table[17, 1] = 63.944221;
            table[18, 1] = 62.939987;
            table[19, 1] = 61.966528;
            table[20, 1] = 57.402598;
            table[21, 1] = 53.168172;
            table[22, 1] = 49.251479;
            table[23, 1] = 45.144765;
            table[24, 1] = 41.13253;
            table[25, 1] = 37.431577;
            table[26, 1] = 34.070701;
            table[27, 1] = 30.981604;
            table[28, 1] = 28.08821;
            table[29, 1] = 25.336842;
            table[30, 1] = 22.693773;
            table[31, 1] = 20.136254;
            table[32, 1] = 17.776117;
            table[33, 1] = 15.529422;
            table[34, 1] = 13.368255;
            table[35, 1] = 11.296936;
            table[36, 1] = 7.515675;
            table[37, 1] = 4.498043;
            table[38, 1] = 3.004213;
            table[39, 1] = 1.540602;
            table[40, 1] = 0.103019;
            table[41, 1] = -1.310727;
            table[42, 1] = -2.701682;
            table[43, 1] = -4.070278;
            table[44, 1] = -5.416648;
            table[45, 1] = -6.740821;
            table[46, 1] = -9.95452;
            table[47, 1] = -13.032537;
            table[48, 1] = -15.98112;
            table[49, 1] = -18.809484;
            table[50, 1] = -21.52868;
            table[51, 1] = -24.150578;
            table[52, 1] = -26.687129;
            table[53, 1] = -29.149857;
            table[54, 1] = -31.549548;
            table[55, 1] = -33.896061;
            table[56, 1] = -36.198241;
            table[57, 1] = -38.463878;
            table[58, 1] = -40.69971;
            table[59, 1] = -42.911452;
            table[60, 1] = -45.103832;
            table[61, 1] = -47.280643;
            table[62, 1] = -49.444793;
            table[63, 1] = -51.598365;
            table[64, 1] = -53.742674;
            table[65, 1] = -55.878321;
            table[66, 1] = -58.00526;
            table[67, 1] = -60.122848;
            table[68, 1] = -62.229913;
            table[69, 1] = -64.324807;
            table[70, 1] = -66.405472;
            table[71, 1] = -68.469497;
            table[72, 1] = -70.514179;
            table[73, 1] = -72.459653;
            table[74, 1] = -74.355305;
            table[75, 1] = -76.2048;
            table[76, 1] = -78.003768;
            table[77, 1] = -79.748207;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_50t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.892877;
            table[1, 1] = 100.768899;
            table[2, 1] = 96.846785;
            table[3, 1] = 93.520985;
            table[4, 1] = 90.437158;
            table[5, 1] = 87.597527;
            table[6, 1] = 85.02858;
            table[7, 1] = 82.7184;
            table[8, 1] = 80.635656;
            table[9, 1] = 78.746003;
            table[10, 1] = 77.018406;
            table[11, 1] = 75.426569;
            table[12, 1] = 73.948654;
            table[13, 1] = 72.566581;
            table[14, 1] = 71.26533;
            table[15, 1] = 70.032378;
            table[16, 1] = 68.857258;
            table[17, 1] = 67.731242;
            table[18, 1] = 66.64709;
            table[19, 1] = 65.598865;
            table[20, 1] = 60.760662;
            table[21, 1] = 56.423726;
            table[22, 1] = 52.511921;
            table[23, 1] = 48.56125;
            table[24, 1] = 44.694903;
            table[25, 1] = 41.035191;
            table[26, 1] = 37.617375;
            table[27, 1] = 34.420235;
            table[28, 1] = 31.409787;
            table[29, 1] = 28.557502;
            table[30, 1] = 25.842063;
            table[31, 1] = 23.246813;
            table[32, 1] = 20.812406;
            table[33, 1] = 18.498031;
            table[34, 1] = 16.288683;
            table[35, 1] = 14.186149;
            table[36, 1] = 10.36173;
            table[37, 1] = 7.207211;
            table[38, 1] = 4.750855;
            table[39, 1] = 2.748357;
            table[40, 1] = 0.97957;
            table[41, 1] = -0.647527;
            table[42, 1] = -2.137095;
            table[43, 1] = -3.556532;
            table[44, 1] = -4.923467;
            table[45, 1] = -6.204564;
            table[46, 1] = -9.274089;
            table[47, 1] = -12.31138;
            table[48, 1] = -15.354925;
            table[49, 1] = -18.342424;
            table[50, 1] = -21.200953;
            table[51, 1] = -23.90152;
            table[52, 1] = -26.453185;
            table[53, 1] = -28.883717;
            table[54, 1] = -31.224208;
            table[55, 1] = -33.501496;
            table[56, 1] = -35.736111;
            table[57, 1] = -37.942798;
            table[58, 1] = -40.131778;
            table[59, 1] = -42.309942;
            table[60, 1] = -44.481734;
            table[61, 1] = -46.649761;
            table[62, 1] = -48.815181;
            table[63, 1] = -50.977967;
            table[64, 1] = -53.137093;
            table[65, 1] = -55.29067;
            table[66, 1] = -57.436072;
            table[67, 1] = -59.570039;
            table[68, 1] = -61.688788;
            table[69, 1] = -63.78811;
            table[70, 1] = -65.863479;
            table[71, 1] = -67.910158;
            table[72, 1] = -69.923201;
            table[73, 1] = -71.890844;
            table[74, 1] = -73.815764;
            table[75, 1] = -75.693367;
            table[76, 1] = -77.519339;
            table[77, 1] = -79.289725;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_50t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.899907;
            table[1, 1] = 100.875088;
            table[2, 1] = 97.317205;
            table[3, 1] = 94.672933;
            table[4, 1] = 92.409711;
            table[5, 1] = 90.370919;
            table[6, 1] = 88.491808;
            table[7, 1] = 86.718706;
            table[8, 1] = 85.026505;
            table[9, 1] = 83.406779;
            table[10, 1] = 81.856931;
            table[11, 1] = 80.375251;
            table[12, 1] = 78.959245;
            table[13, 1] = 77.605408;
            table[14, 1] = 76.309479;
            table[15, 1] = 75.066785;
            table[16, 1] = 73.872541;
            table[17, 1] = 72.722049;
            table[18, 1] = 71.610836;
            table[19, 1] = 70.534728;
            table[20, 1] = 65.55888;
            table[21, 1] = 61.028534;
            table[22, 1] = 56.763467;
            table[23, 1] = 52.693623;
            table[24, 1] = 48.80382;
            table[25, 1] = 45.097636;
            table[26, 1] = 41.579605;
            table[27, 1] = 38.249351;
            table[28, 1] = 35.101366;
            table[29, 1] = 32.126598;
            table[30, 1] = 29.314194;
            table[31, 1] = 26.652925;
            table[32, 1] = 24.132327;
            table[33, 1] = 21.743758;
            table[34, 1] = 19.48163;
            table[35, 1] = 17.344909;
            table[36, 1] = 13.473255;
            table[37, 1] = 10.210204;
            table[38, 1] = 7.542703;
            table[39, 1] = 5.301019;
            table[40, 1] = 3.329413;
            table[41, 1] = 1.548416;
            table[42, 1] = -0.08141;
            table[43, 1] = -1.58767;
            table[44, 1] = -2.996394;
            table[45, 1] = -4.333623;
            table[46, 1] = -7.514344;
            table[47, 1] = -10.641613;
            table[48, 1] = -13.765898;
            table[49, 1] = -16.824003;
            table[50, 1] = -19.74713;
            table[51, 1] = -22.507456;
            table[52, 1] = -25.114746;
            table[53, 1] = -27.597311;
            table[54, 1] = -29.986678;
            table[55, 1] = -32.310048;
            table[56, 1] = -34.588253;
            table[57, 1] = -36.836294;
            table[58, 1] = -39.06461;
            table[59, 1] = -41.280278;
            table[60, 1] = -43.487908;
            table[61, 1] = -45.690247;
            table[62, 1] = -47.888579;
            table[63, 1] = -50.082986;
            table[64, 1] = -52.272538;
            table[65, 1] = -54.455436;
            table[66, 1] = -56.629128;
            table[67, 1] = -58.790428;
            table[68, 1] = -60.935612;
            table[69, 1] = -63.06053;
            table[70, 1] = -65.160708;
            table[71, 1] = -67.231455;
            table[72, 1] = -69.267964;
            table[73, 1] = -71.265418;
            table[74, 1] = -73.219091;
            table[75, 1] = -75.124441;
            table[76, 1] = -76.977201;
            table[77, 1] = -78.773459;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_50t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.899928;
            table[1, 1] = 100.878286;
            table[2, 1] = 97.352046;
            table[3, 1] = 94.841598;
            table[4, 1] = 92.879187;
            table[5, 1] = 91.252311;
            table[6, 1] = 89.843636;
            table[7, 1] = 88.579624;
            table[8, 1] = 87.410147;
            table[9, 1] = 86.299369;
            table[10, 1] = 85.221397;
            table[11, 1] = 84.158041;
            table[12, 1] = 83.097471;
            table[13, 1] = 82.033135;
            table[14, 1] = 80.962681;
            table[15, 1] = 79.886833;
            table[16, 1] = 78.80831;
            table[17, 1] = 77.730864;
            table[18, 1] = 76.65854;
            table[19, 1] = 75.595158;
            table[20, 1] = 70.503893;
            table[21, 1] = 65.865618;
            table[22, 1] = 61.611931;
            table[23, 1] = 57.611454;
            table[24, 1] = 53.764597;
            table[25, 1] = 50.031336;
            table[26, 1] = 46.41738;
            table[27, 1] = 42.945494;
            table[28, 1] = 39.635249;
            table[29, 1] = 36.496445;
            table[30, 1] = 33.530383;
            table[31, 1] = 30.733298;
            table[32, 1] = 28.099342;
            table[33, 1] = 25.622499;
            table[34, 1] = 23.297521;
            table[35, 1] = 21.120082;
            table[36, 1] = 17.192112;
            table[37, 1] = 13.799399;
            table[38, 1] = 10.879556;
            table[39, 1] = 8.352004;
            table[40, 1] = 6.137991;
            table[41, 1] = 4.173055;
            table[42, 1] = 2.40669;
            table[43, 1] = 0.797038;
            table[44, 1] = -0.693109;
            table[45, 1] = -2.097428;
            table[46, 1] = -5.411051;
            table[47, 1] = -8.645861;
            table[48, 1] = -11.866648;
            table[49, 1] = -15.009145;
            table[50, 1] = -18.00948;
            table[51, 1] = -20.84123;
            table[52, 1] = -23.515006;
            table[53, 1] = -26.059761;
            table[54, 1] = -28.507547;
            table[55, 1] = -30.885996;
            table[56, 1] = -33.216301;
            table[57, 1] = -35.513769;
            table[58, 1] = -37.789099;
            table[59, 1] = -40.049595;
            table[60, 1] = -42.300059;
            table[61, 1] = -44.543409;
            table[62, 1] = -46.781077;
            table[63, 1] = -49.013278;
            table[64, 1] = -51.239197;
            table[65, 1] = -53.457139;
            table[66, 1] = -55.664646;
            table[67, 1] = -57.858614;
            table[68, 1] = -60.035394;
            table[69, 1] = -62.190906;
            table[70, 1] = -64.320736;
            table[71, 1] = -66.42025;
            table[72, 1] = -68.484691;
            table[73, 1] = -70.509291;
            table[74, 1] = -72.489366;
            table[75, 1] = -74.420414;
            table[76, 1] = -76.298204;
            table[77, 1] = -78.118859;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_50t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.899998;
            table[1, 1] = 100.879364;
            table[2, 1] = 97.357351;
            table[3, 1] = 94.857982;
            table[4, 1] = 92.918369;
            table[5, 1] = 91.33191;
            table[6, 1] = 89.987912;
            table[7, 1] = 88.819751;
            table[8, 1] = 87.78385;
            table[9, 1] = 86.84981;
            table[10, 1] = 85.995276;
            table[11, 1] = 85.203053;
            table[12, 1] = 84.459397;
            table[13, 1] = 83.752971;
            table[14, 1] = 83.07418;
            table[15, 1] = 82.414762;
            table[16, 1] = 81.767534;
            table[17, 1] = 81.126255;
            table[18, 1] = 80.485547;
            table[19, 1] = 79.840873;
            table[20, 1] = 76.45689;
            table[21, 1] = 72.741069;
            table[22, 1] = 68.827186;
            table[23, 1] = 64.893112;
            table[24, 1] = 61.031215;
            table[25, 1] = 57.260341;
            table[26, 1] = 53.572161;
            table[27, 1] = 49.962419;
            table[28, 1] = 46.440235;
            table[29, 1] = 43.0235;
            table[30, 1] = 39.731086;
            table[31, 1] = 36.578026;
            table[32, 1] = 33.574432;
            table[33, 1] = 30.726491;
            table[34, 1] = 28.03794;
            table[35, 1] = 25.511124;
            table[36, 1] = 20.946606;
            table[37, 1] = 17.02341;
            table[38, 1] = 13.692307;
            table[39, 1] = 10.865885;
            table[40, 1] = 8.443519;
            table[41, 1] = 6.335079;
            table[42, 1] = 4.468493;
            table[43, 1] = 2.786553;
            table[44, 1] = 1.241994;
            table[45, 1] = -0.205226;
            table[46, 1] = -3.59834;
            table[47, 1] = -6.893119;
            table[48, 1] = -10.166089;
            table[49, 1] = -13.353287;
            table[50, 1] = -16.39399;
            table[51, 1] = -19.262934;
            table[52, 1] = -21.971288;
            table[53, 1] = -24.548378;
            table[54, 1] = -27.026533;
            table[55, 1] = -29.433613;
            table[56, 1] = -31.791001;
            table[57, 1] = -34.114162;
            table[58, 1] = -36.413932;
            table[59, 1] = -38.69773;
            table[60, 1] = -40.97046;
            table[61, 1] = -43.235129;
            table[62, 1] = -45.493245;
            table[63, 1] = -47.745092;
            table[64, 1] = -49.989916;
            table[65, 1] = -52.226075;
            table[66, 1] = -54.45116;
            table[67, 1] = -56.66211;
            table[68, 1] = -58.855315;
            table[69, 1] = -60.961805;
            table[70, 1] = -63.045296;
            table[71, 1] = -65.111855;
            table[72, 1] = -67.158818;
            table[73, 1] = -69.183282;
            table[74, 1] = -71.182167;
            table[75, 1] = -73.152279;
            table[76, 1] = -75.090361;
            table[77, 1] = -76.993158;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_50t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.9;
            table[1, 1] = 100.879399;
            table[2, 1] = 97.357566;
            table[3, 1] = 94.858764;
            table[4, 1] = 92.920492;
            table[5, 1] = 91.336709;
            table[6, 1] = 89.99747;
            table[7, 1] = 88.837099;
            table[8, 1] = 87.813179;
            table[9, 1] = 86.896683;
            table[10, 1] = 86.066836;
            table[11, 1] = 85.308215;
            table[12, 1] = 84.609019;
            table[13, 1] = 83.959977;
            table[14, 1] = 83.353634;
            table[15, 1] = 82.783867;
            table[16, 1] = 82.24555;
            table[17, 1] = 81.73431;
            table[18, 1] = 81.246352;
            table[19, 1] = 80.778334;
            table[20, 1] = 78.641343;
            table[21, 1] = 76.642054;
            table[22, 1] = 74.568429;
            table[23, 1] = 72.283109;
            table[24, 1] = 69.735593;
            table[25, 1] = 66.950802;
            table[26, 1] = 63.9883;
            table[27, 1] = 60.901459;
            table[28, 1] = 57.720231;
            table[29, 1] = 54.457611;
            table[30, 1] = 51.126403;
            table[31, 1] = 47.751518;
            table[32, 1] = 44.370454;
            table[33, 1] = 41.025401;
            table[34, 1] = 37.755035;
            table[35, 1] = 34.590573;
            table[36, 1] = 28.66916;
            table[37, 1] = 23.395988;
            table[38, 1] = 18.85464;
            table[39, 1] = 15.065298;
            table[40, 1] = 11.962091;
            table[41, 1] = 9.414626;
            table[42, 1] = 7.279451;
            table[43, 1] = 5.435147;
            table[44, 1] = 3.790053;
            table[45, 1] = 2.27719;
            table[46, 1] = -1.211706;
            table[47, 1] = -4.566213;
            table[48, 1] = -7.88665;
            table[49, 1] = -11.115254;
            table[50, 1] = -14.193584;
            table[51, 1] = -17.097255;
            table[52, 1] = -19.837915;
            table[53, 1] = -22.44522;
            table[54, 1] = -24.95176;
            table[55, 1] = -27.385604;
            table[56, 1] = -29.76831;
            table[57, 1] = -32.115491;
            table[58, 1] = -34.438111;
            table[59, 1] = -36.743697;
            table[60, 1] = -39.037247;
            table[61, 1] = -41.32185;
            table[62, 1] = -43.599087;
            table[63, 1] = -45.869304;
            table[64, 1] = -48.131806;
            table[65, 1] = -50.278306;
            table[66, 1] = -52.412274;
            table[67, 1] = -54.536075;
            table[68, 1] = -56.648653;
            table[69, 1] = -58.74846;
            table[70, 1] = -60.833517;
            table[71, 1] = -62.901482;
            table[72, 1] = -64.949709;
            table[73, 1] = -66.975315;
            table[74, 1] = -68.975234;
            table[75, 1] = -70.946284;
            table[76, 1] = -72.885221;
            table[77, 1] = -74.788796;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_50t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.9;
            table[1, 1] = 100.8794;
            table[2, 1] = 97.357575;
            table[3, 1] = 94.858799;
            table[4, 1] = 92.920595;
            table[5, 1] = 91.336962;
            table[6, 1] = 89.99801;
            table[7, 1] = 88.83814;
            table[8, 1] = 87.815038;
            table[9, 1] = 86.899804;
            table[10, 1] = 86.071821;
            table[11, 1] = 85.315858;
            table[12, 1] = 84.620341;
            table[13, 1] = 83.976265;
            table[14, 1] = 83.37648;
            table[15, 1] = 82.815212;
            table[16, 1] = 82.287725;
            table[17, 1] = 81.790082;
            table[18, 1] = 81.318973;
            table[19, 1] = 80.871578;
            table[20, 1] = 78.915604;
            table[21, 1] = 77.290263;
            table[22, 1] = 75.866674;
            table[23, 1] = 74.552719;
            table[24, 1] = 73.272545;
            table[25, 1] = 71.959367;
            table[26, 1] = 70.555565;
            table[27, 1] = 69.016709;
            table[28, 1] = 67.315725;
            table[29, 1] = 65.443279;
            table[30, 1] = 63.402207;
            table[31, 1] = 61.197453;
            table[32, 1] = 58.826302;
            table[33, 1] = 56.275315;
            table[34, 1] = 53.528647;
            table[35, 1] = 50.585137;
            table[36, 1] = 44.242438;
            table[37, 1] = 37.693907;
            table[38, 1] = 31.387028;
            table[39, 1] = 25.616812;
            table[40, 1] = 20.58023;
            table[41, 1] = 16.395149;
            table[42, 1] = 13.055196;
            table[43, 1] = 10.421514;
            table[44, 1] = 8.29471;
            table[45, 1] = 6.494081;
            table[46, 1] = 2.672818;
            table[47, 1] = -0.826901;
            table[48, 1] = -4.246945;
            table[49, 1] = -7.553692;
            table[50, 1] = -10.701654;
            table[51, 1] = -13.669389;
            table[52, 1] = -16.469638;
            table[53, 1] = -19.132697;
            table[54, 1] = -21.691631;
            table[55, 1] = -24.174893;
            table[56, 1] = -26.60436;
            table[57, 1] = -28.995915;
            table[58, 1] = -31.360752;
            table[59, 1] = -33.706597;
            table[60, 1] = -36.038624;
            table[61, 1] = -38.360069;
            table[62, 1] = -40.672648;
            table[63, 1] = -42.976824;
            table[64, 1] = -45.240044;
            table[65, 1] = -47.389625;
            table[66, 1] = -49.528827;
            table[67, 1] = -51.657256;
            table[68, 1] = -53.773944;
            table[69, 1] = -55.877413;
            table[70, 1] = -57.965745;
            table[71, 1] = -60.036649;
            table[72, 1] = -62.087522;
            table[73, 1] = -64.115517;
            table[74, 1] = -66.117598;
            table[75, 1] = -68.090612;
            table[76, 1] = -70.031335;
            table[77, 1] = -71.936541;



            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_50t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.9;
            table[1, 1] = 100.8794;
            table[2, 1] = 97.357575;
            table[3, 1] = 94.8588;
            table[4, 1] = 92.9206;
            table[5, 1] = 91.336974;
            table[6, 1] = 89.998038;
            table[7, 1] = 88.838197;
            table[8, 1] = 87.815144;
            table[9, 1] = 86.899989;
            table[10, 1] = 86.072128;
            table[11, 1] = 85.316345;
            table[12, 1] = 84.621085;
            table[13, 1] = 83.977367;
            table[14, 1] = 83.378068;
            table[15, 1] = 82.817446;
            table[16, 1] = 82.290804;
            table[17, 1] = 81.79425;
            table[18, 1] = 81.32452;
            table[19, 1] = 80.878855;
            table[20, 1] = 78.939275;
            table[21, 1] = 77.35218;
            table[22, 1] = 76.005744;
            table[23, 1] = 74.831382;
            table[24, 1] = 73.782455;
            table[25, 1] = 72.824129;
            table[26, 1] = 71.928005;
            table[27, 1] = 71.06908;
            table[28, 1] = 70.224039;
            table[29, 1] = 69.370442;
            table[30, 1] = 68.486603;
            table[31, 1] = 67.552051;
            table[32, 1] = 66.548397;
            table[33, 1] = 65.460315;
            table[34, 1] = 64.276156;
            table[35, 1] = 62.98762;
            table[36, 1] = 60.067716;
            table[37, 1] = 56.586978;
            table[38, 1] = 52.282187;
            table[39, 1] = 46.964825;
            table[40, 1] = 40.92752;
            table[41, 1] = 34.73581;
            table[42, 1] = 28.849624;
            table[43, 1] = 23.581256;
            table[44, 1] = 19.131903;
            table[45, 1] = 15.557037;
            table[46, 1] = 9.498405;
            table[47, 1] = 5.292128;
            table[48, 1] = 1.553239;
            table[49, 1] = -1.926893;
            table[50, 1] = -5.214147;
            table[51, 1] = -8.307176;
            table[52, 1] = -11.223458;
            table[53, 1] = -13.995051;
            table[54, 1] = -16.656032;
            table[55, 1] = -19.235602;
            table[56, 1] = -21.756249;
            table[57, 1] = -24.23437;
            table[58, 1] = -26.681599;
            table[59, 1] = -29.106044;
            table[60, 1] = -31.513208;
            table[61, 1] = -33.906618;
            table[62, 1] = -36.288246;
            table[63, 1] = -38.658779;
            table[64, 1] = -41.017827;
            table[65, 1] = -43.364068;
            table[66, 1] = -45.695383;
            table[67, 1] = -48.008969;
            table[68, 1] = -50.143622;
            table[69, 1] = -52.25766;
            table[70, 1] = -54.355449;
            table[71, 1] = -56.434844;
            table[72, 1] = -58.493364;
            table[73, 1] = -60.528264;
            table[74, 1] = -62.536601;
            table[75, 1] = -64.515295;
            table[76, 1] = -66.461189;
            table[77, 1] = -68.371114;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_10t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.038794;
            table[1, 1] = 100.2919;
            table[2, 1] = 94.552531;
            table[3, 1] = 89.791493;
            table[4, 1] = 86.014892;
            table[5, 1] = 82.916591;
            table[6, 1] = 80.293789;
            table[7, 1] = 78.019977;
            table[8, 1] = 76.012327;
            table[9, 1] = 74.213734;
            table[10, 1] = 72.583029;
            table[11, 1] = 71.089383;
            table[12, 1] = 69.708929;
            table[13, 1] = 68.422652;
            table[14, 1] = 67.215023;
            table[15, 1] = 66.073102;
            table[16, 1] = 64.985943;
            table[17, 1] = 63.944221;
            table[18, 1] = 62.939987;
            table[19, 1] = 61.966528;
            table[20, 1] = 57.402598;
            table[21, 1] = 53.168172;
            table[22, 1] = 49.392457;
            table[23, 1] = 46.739005;
            table[24, 1] = 44.651137;
            table[25, 1] = 43.082;
            table[26, 1] = 41.830887;
            table[27, 1] = 40.754835;
            table[28, 1] = 39.780022;
            table[29, 1] = 38.874238;
            table[30, 1] = 38.024322;
            table[31, 1] = 37.224613;
            table[32, 1] = 36.472019;
            table[33, 1] = 35.764111;
            table[34, 1] = 35.098454;
            table[35, 1] = 34.472421;
            table[36, 1] = 33.327866;
            table[37, 1] = 32.306982;
            table[38, 1] = 31.386677;
            table[39, 1] = 30.545849;
            table[40, 1] = 29.766205;
            table[41, 1] = 29.032479;
            table[42, 1] = 28.332304;
            table[43, 1] = 27.655901;
            table[44, 1] = 26.995705;
            table[45, 1] = 26.345983;
            table[46, 1] = 24.742452;
            table[47, 1] = 23.141927;
            table[48, 1] = 21.530837;
            table[49, 1] = 19.908126;
            table[50, 1] = 18.278186;
            table[51, 1] = 16.647258;
            table[52, 1] = 15.021723;
            table[53, 1] = 13.407371;
            table[54, 1] = 11.809164;
            table[55, 1] = 10.231215;
            table[56, 1] = 8.676867;
            table[57, 1] = 7.1488;
            table[58, 1] = 5.649139;
            table[59, 1] = 4.179553;
            table[60, 1] = 2.741332;
            table[61, 1] = 1.335458;
            table[62, 1] = -0.037345;
            table[63, 1] = -1.376561;
            table[64, 1] = -2.681858;
            table[65, 1] = -3.953049;
            table[66, 1] = -5.190077;
            table[67, 1] = -6.392994;
            table[68, 1] = -7.561948;
            table[69, 1] = -8.697165;
            table[70, 1] = -9.798945;
            table[71, 1] = -10.867651;
            table[72, 1] = -11.903698;
            table[73, 1] = -12.907549;
            table[74, 1] = -13.879709;
            table[75, 1] = -14.820719;
            table[76, 1] = -15.731152;
            table[77, 1] = -16.611606;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_10t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.068796;
            table[1, 1] = 101.061903;
            table[2, 1] = 97.266034;
            table[3, 1] = 93.921562;
            table[4, 1] = 90.72082;
            table[5, 1] = 87.760094;
            table[6, 1] = 85.095757;
            table[7, 1] = 82.7184;
            table[8, 1] = 80.635656;
            table[9, 1] = 78.746003;
            table[10, 1] = 77.018406;
            table[11, 1] = 75.426569;
            table[12, 1] = 73.948654;
            table[13, 1] = 72.566581;
            table[14, 1] = 71.26533;
            table[15, 1] = 70.032378;
            table[16, 1] = 68.857258;
            table[17, 1] = 67.731242;
            table[18, 1] = 66.64709;
            table[19, 1] = 65.598865;
            table[20, 1] = 60.760662;
            table[21, 1] = 56.971479;
            table[22, 1] = 53.728381;
            table[23, 1] = 51.191806;
            table[24, 1] = 49.016742;
            table[25, 1] = 47.316128;
            table[26, 1] = 45.924209;
            table[27, 1] = 44.719317;
            table[28, 1] = 43.633036;
            table[29, 1] = 42.631276;
            table[30, 1] = 41.697404;
            table[31, 1] = 40.822735;
            table[32, 1] = 40.001973;
            table[33, 1] = 39.231163;
            table[34, 1] = 38.506819;
            table[35, 1] = 37.825582;
            table[36, 1] = 36.579091;
            table[37, 1] = 35.465388;
            table[38, 1] = 34.459642;
            table[39, 1] = 33.539576;
            table[40, 1] = 32.686035;
            table[41, 1] = 31.883087;
            table[42, 1] = 31.117834;
            table[43, 1] = 30.380059;
            table[44, 1] = 29.661827;
            table[45, 1] = 28.957093;
            table[46, 1] = 27.227568;
            table[47, 1] = 25.514967;
            table[48, 1] = 23.803236;
            table[49, 1] = 22.089436;
            table[50, 1] = 20.376496;
            table[51, 1] = 18.669497;
            table[52, 1] = 16.973886;
            table[53, 1] = 15.294691;
            table[54, 1] = 13.636243;
            table[55, 1] = 12.002128;
            table[56, 1] = 10.395242;
            table[57, 1] = 8.817889;
            table[58, 1] = 7.271868;
            table[59, 1] = 5.758567;
            table[60, 1] = 4.279033;
            table[61, 1] = 2.834034;
            table[62, 1] = 1.424109;
            table[63, 1] = 0.049605;
            table[64, 1] = -1.289289;
            table[65, 1] = -2.592519;
            table[66, 1] = -3.860144;
            table[67, 1] = -5.09232;
            table[68, 1] = -6.28929;
            table[69, 1] = -7.451366;
            table[70, 1] = -8.578924;
            table[71, 1] = -9.672398;
            table[72, 1] = -10.732266;
            table[73, 1] = -11.75905;
            table[74, 1] = -12.753309;
            table[75, 1] = -13.715632;
            table[76, 1] = -14.646637;
            table[77, 1] = -15.546964;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_10t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.073188;
            table[1, 1] = 101.149233;
            table[2, 1] = 97.741781;
            table[3, 1] = 95.258214;
            table[4, 1] = 93.184273;
            table[5, 1] = 91.282605;
            table[6, 1] = 89.443961;
            table[7, 1] = 87.635984;
            table[8, 1] = 85.865973;
            table[9, 1] = 84.152424;
            table[10, 1] = 82.510257;
            table[11, 1] = 80.946952;
            table[12, 1] = 79.463693;
            table[13, 1] = 78.057631;
            table[14, 1] = 76.723787;
            table[15, 1] = 75.456307;
            table[16, 1] = 74.249206;
            table[17, 1] = 73.096773;
            table[18, 1] = 71.993769;
            table[19, 1] = 70.935513;
            table[20, 1] = 66.190967;
            table[21, 1] = 62.163541;
            table[22, 1] = 58.753724;
            table[23, 1] = 55.943057;
            table[24, 1] = 53.675684;
            table[25, 1] = 51.835107;
            table[26, 1] = 50.293102;
            table[27, 1] = 48.950815;
            table[28, 1] = 47.745645;
            table[29, 1] = 46.64151;
            table[30, 1] = 45.618075;
            table[31, 1] = 44.663436;
            table[32, 1] = 43.769943;
            table[33, 1] = 42.932017;
            table[34, 1] = 42.145053;
            table[35, 1] = 41.404908;
            table[36, 1] = 40.049634;
            table[37, 1] = 38.83687;
            table[38, 1] = 37.739936;
            table[39, 1] = 36.735297;
            table[40, 1] = 35.802881;
            table[41, 1] = 34.92605;
            table[42, 1] = 34.091335;
            table[43, 1] = 33.288052;
            table[44, 1] = 32.507874;
            table[45, 1] = 31.744421;
            table[46, 1] = 29.880413;
            table[47, 1] = 28.048186;
            table[48, 1] = 26.229033;
            table[49, 1] = 24.418004;
            table[50, 1] = 22.616468;
            table[51, 1] = 20.828268;
            table[52, 1] = 19.057854;
            table[53, 1] = 17.309441;
            table[54, 1] = 15.586687;
            table[55, 1] = 13.892614;
            table[56, 1] = 12.229645;
            table[57, 1] = 10.599678;
            table[58, 1] = 9.004167;
            table[59, 1] = 7.4442;
            table[60, 1] = 5.920563;
            table[61, 1] = 4.433798;
            table[62, 1] = 2.984243;
            table[63, 1] = 1.57207;
            table[64, 1] = 0.19731;
            table[65, 1] = -1.140122;
            table[66, 1] = -2.440409;
            table[67, 1] = -3.703821;
            table[68, 1] = -4.930698;
            table[69, 1] = -6.121446;
            table[70, 1] = -7.276524;
            table[71, 1] = -8.396437;
            table[72, 1] = -9.481735;
            table[73, 1] = -10.533001;
            table[74, 1] = -11.55085;
            table[75, 1] = -12.535926;
            table[76, 1] = -13.488892;
            table[77, 1] = -14.410435;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_10t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.073998;
            table[1, 1] = 101.161331;
            table[2, 1] = 97.802135;
            table[3, 1] = 95.445404;
            table[4, 1] = 93.622878;
            table[5, 1] = 92.127642;
            table[6, 1] = 90.846592;
            table[7, 1] = 89.707444;
            table[8, 1] = 88.657854;
            table[9, 1] = 87.656518;
            table[10, 1] = 86.669937;
            table[11, 1] = 85.672206;
            table[12, 1] = 84.646108;
            table[13, 1] = 83.58394;
            table[14, 1] = 82.486954;
            table[15, 1] = 81.363205;
            table[16, 1] = 80.224555;
            table[17, 1] = 79.083818;
            table[18, 1] = 77.952743;
            table[19, 1] = 76.840968;
            table[20, 1] = 71.755237;
            table[21, 1] = 67.562938;
            table[22, 1] = 64.141282;
            table[23, 1] = 61.316888;
            table[24, 1] = 58.957178;
            table[25, 1] = 56.964445;
            table[26, 1] = 55.255925;
            table[27, 1] = 53.760064;
            table[28, 1] = 52.421531;
            table[29, 1] = 51.202318;
            table[30, 1] = 50.078051;
            table[31, 1] = 49.033258;
            table[32, 1] = 48.057678;
            table[33, 1] = 47.14392;
            table[34, 1] = 46.286135;
            table[35, 1] = 45.479312;
            table[36, 1] = 44.000776;
            table[37, 1] = 42.675641;
            table[38, 1] = 41.475182;
            table[39, 1] = 40.374476;
            table[40, 1] = 39.352431;
            table[41, 1] = 38.391618;
            table[42, 1] = 37.477934;
            table[43, 1] = 36.600169;
            table[44, 1] = 35.749554;
            table[45, 1] = 34.919329;
            table[46, 1] = 32.902384;
            table[47, 1] = 30.934102;
            table[48, 1] = 28.992753;
            table[49, 1] = 27.071103;
            table[50, 1] = 25.168743;
            table[51, 1] = 23.288113;
            table[52, 1] = 21.432535;
            table[53, 1] = 19.605301;
            table[54, 1] = 17.809307;
            table[55, 1] = 16.046938;
            table[56, 1] = 14.320079;
            table[57, 1] = 12.630171;
            table[58, 1] = 10.978272;
            table[59, 1] = 9.365133;
            table[60, 1] = 7.791243;
            table[61, 1] = 6.256885;
            table[62, 1] = 4.762172;
            table[63, 1] = 3.307072;
            table[64, 1] = 1.891441;
            table[65, 1] = 0.515034;
            table[66, 1] = -0.822475;
            table[67, 1] = -2.121481;
            table[68, 1] = -3.382442;
            table[69, 1] = -4.605864;
            table[70, 1] = -5.792302;
            table[71, 1] = -6.942346;
            table[72, 1] = -8.056624;
            table[73, 1] = -9.135789;
            table[74, 1] = -10.180522;
            table[75, 1] = -11.191526;
            table[76, 1] = -12.16952;
            table[77, 1] = -13.115239;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_10t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.1614;
            table[2, 1] = 97.802723;
            table[3, 1] = 95.448115;
            table[4, 1] = 93.631764;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.899563;
            table[7, 1] = 89.81431;
            table[8, 1] = 88.854792;
            table[9, 1] = 87.993179;
            table[10, 1] = 87.209246;
            table[11, 1] = 86.487508;
            table[12, 1] = 85.815493;
            table[13, 1] = 85.182638;
            table[14, 1] = 84.579577;
            table[15, 1] = 83.997673;
            table[16, 1] = 83.428753;
            table[17, 1] = 82.865002;
            table[18, 1] = 82.29901;
            table[19, 1] = 81.723946;
            table[20, 1] = 78.551282;
            table[21, 1] = 74.882253;
            table[22, 1] = 71.189118;
            table[23, 1] = 67.86407;
            table[24, 1] = 65.007306;
            table[25, 1] = 62.57567;
            table[26, 1] = 60.493271;
            table[27, 1] = 58.689555;
            table[28, 1] = 57.105777;
            table[29, 1] = 55.694577;
            table[30, 1] = 54.419357;
            table[31, 1] = 53.253162;
            table[32, 1] = 52.176728;
            table[33, 1] = 51.176288;
            table[34, 1] = 50.241718;
            table[35, 1] = 49.365209;
            table[36, 1] = 47.761938;
            table[37, 1] = 46.325468;
            table[38, 1] = 45.02321;
            table[39, 1] = 43.828222;
            table[40, 1] = 42.718198;
            table[41, 1] = 41.674883;
            table[42, 1] = 40.683548;
            table[43, 1] = 39.732483;
            table[44, 1] = 38.812498;
            table[45, 1] = 37.916474;
            table[46, 1] = 35.748684;
            table[47, 1] = 33.646042;
            table[48, 1] = 31.583867;
            table[49, 1] = 29.55268;
            table[50, 1] = 27.550341;
            table[51, 1] = 25.577929;
            table[52, 1] = 23.637674;
            table[53, 1] = 21.731982;
            table[54, 1] = 19.863014;
            table[55, 1] = 18.032542;
            table[56, 1] = 16.241931;
            table[57, 1] = 14.492177;
            table[58, 1] = 12.783958;
            table[59, 1] = 11.117689;
            table[60, 1] = 9.493573;
            table[61, 1] = 7.911637;
            table[62, 1] = 6.371769;
            table[63, 1] = 4.873742;
            table[64, 1] = 3.417233;
            table[65, 1] = 2.001841;
            table[66, 1] = 0.627098;
            table[67, 1] = -0.707517;
            table[68, 1] = -2.002576;
            table[69, 1] = -3.258689;
            table[70, 1] = -4.476503;
            table[71, 1] = -5.656696;
            table[72, 1] = -6.79997;
            table[73, 1] = -7.907052;
            table[74, 1] = -8.978687;
            table[75, 1] = -10.015636;
            table[76, 1] = -11.018674;
            table[77, 1] = -11.988587;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_10t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.1614;
            table[2, 1] = 97.802726;
            table[3, 1] = 95.448138;
            table[4, 1] = 93.631859;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.900366;
            table[7, 1] = 89.816185;
            table[8, 1] = 88.85875;
            table[9, 1] = 88.000894;
            table[10, 1] = 87.223339;
            table[11, 1] = 86.511907;
            table[12, 1] = 85.855849;
            table[13, 1] = 85.246809;
            table[14, 1] = 84.678147;
            table[15, 1] = 84.144473;
            table[16, 1] = 83.641332;
            table[17, 1] = 83.164967;
            table[18, 1] = 82.712152;
            table[19, 1] = 82.280061;
            table[20, 1] = 80.348454;
            table[21, 1] = 78.611879;
            table[22, 1] = 76.829198;
            table[23, 1] = 74.803869;
            table[24, 1] = 72.48775;
            table[25, 1] = 70.004322;
            table[26, 1] = 67.530341;
            table[27, 1] = 65.190419;
            table[28, 1] = 63.042667;
            table[29, 1] = 61.105064;
            table[30, 1] = 59.376513;
            table[31, 1] = 57.844262;
            table[32, 1] = 56.486161;
            table[33, 1] = 55.274716;
            table[34, 1] = 54.182507;
            table[35, 1] = 53.185978;
            table[36, 1] = 51.410586;
            table[37, 1] = 49.849207;
            table[38, 1] = 48.44415;
            table[39, 1] = 47.158218;
            table[40, 1] = 45.964801;
            table[41, 1] = 44.843688;
            table[42, 1] = 43.77915;
            table[43, 1] = 42.758871;
            table[44, 1] = 41.773241;
            table[45, 1] = 40.814808;
            table[46, 1] = 38.503404;
            table[47, 1] = 36.272171;
            table[48, 1] = 34.093754;
            table[49, 1] = 31.956638;
            table[50, 1] = 29.85712;
            table[51, 1] = 27.795068;
            table[52, 1] = 25.771754;
            table[53, 1] = 23.788808;
            table[54, 1] = 21.847748;
            table[55, 1] = 19.949808;
            table[56, 1] = 18.09589;
            table[57, 1] = 16.28659;
            table[58, 1] = 14.522241;
            table[59, 1] = 12.802951;
            table[60, 1] = 11.128655;
            table[61, 1] = 9.499142;
            table[62, 1] = 7.914087;
            table[63, 1] = 6.373073;
            table[64, 1] = 4.875607;
            table[65, 1] = 3.421137;
            table[66, 1] = 2.009059;
            table[67, 1] = 0.638726;
            table[68, 1] = -0.690545;
            table[69, 1] = -1.979466;
            table[70, 1] = -3.228776;
            table[71, 1] = -4.439236;
            table[72, 1] = -5.611627;
            table[73, 1] = -6.746745;
            table[74, 1] = -7.845399;
            table[75, 1] = -8.90841;
            table[76, 1] = -9.936608;
            table[77, 1] = -10.930829;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_10t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.1614;
            table[2, 1] = 97.802726;
            table[3, 1] = 95.448138;
            table[4, 1] = 93.63186;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.900374;
            table[7, 1] = 89.816206;
            table[8, 1] = 88.8588;
            table[9, 1] = 88.000999;
            table[10, 1] = 87.223548;
            table[11, 1] = 86.512298;
            table[12, 1] = 85.856543;
            table[13, 1] = 85.247991;
            table[14, 1] = 84.680085;
            table[15, 1] = 84.14755;
            table[16, 1] = 83.646081;
            table[17, 1] = 83.172113;
            table[18, 1] = 82.722663;
            table[19, 1] = 82.29521;
            table[20, 1] = 80.421862;
            table[21, 1] = 78.869837;
            table[22, 1] = 77.532759;
            table[23, 1] = 76.337232;
            table[24, 1] = 75.219494;
            table[25, 1] = 74.113427;
            table[26, 1] = 72.948247;
            table[27, 1] = 71.658394;
            table[28, 1] = 70.203388;
            table[29, 1] = 68.584309;
            table[30, 1] = 66.84149;
            table[31, 1] = 65.034576;
            table[32, 1] = 63.222087;
            table[33, 1] = 61.453642;
            table[34, 1] = 59.772807;
            table[35, 1] = 58.218873;
            table[36, 1] = 55.583356;
            table[37, 1] = 53.53196;
            table[38, 1] = 51.871371;
            table[39, 1] = 50.440141;
            table[40, 1] = 49.148826;
            table[41, 1] = 47.950698;
            table[42, 1] = 46.819276;
            table[43, 1] = 45.737809;
            table[44, 1] = 44.694769;
            table[45, 1] = 43.681802;
            table[46, 1] = 41.24396;
            table[47, 1] = 38.897765;
            table[48, 1] = 36.613748;
            table[49, 1] = 34.378904;
            table[50, 1] = 32.188432;
            table[51, 1] = 30.041376;
            table[52, 1] = 27.938376;
            table[53, 1] = 25.880552;
            table[54, 1] = 23.869003;
            table[55, 1] = 21.904594;
            table[56, 1] = 19.987908;
            table[57, 1] = 18.119251;
            table[58, 1] = 16.29869;
            table[59, 1] = 14.526098;
            table[60, 1] = 12.801187;
            table[61, 1] = 11.123549;
            table[62, 1] = 9.492677;
            table[63, 1] = 7.907986;
            table[64, 1] = 6.368832;
            table[65, 1] = 4.874525;
            table[66, 1] = 3.424332;
            table[67, 1] = 2.017492;
            table[68, 1] = 0.653216;
            table[69, 1] = -0.669305;
            table[70, 1] = -1.950899;
            table[71, 1] = -3.192409;
            table[72, 1] = -4.394691;
            table[73, 1] = -5.558608;
            table[74, 1] = -6.685034;
            table[75, 1] = -7.774849;
            table[76, 1] = -8.828935;
            table[77, 1] = -9.84818;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_10t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.1614;
            table[2, 1] = 97.802726;
            table[3, 1] = 95.448138;
            table[4, 1] = 93.63186;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.900374;
            table[7, 1] = 89.816206;
            table[8, 1] = 88.8588;
            table[9, 1] = 88.001;
            table[10, 1] = 87.223551;
            table[11, 1] = 86.512302;
            table[12, 1] = 85.856551;
            table[13, 1] = 85.248006;
            table[14, 1] = 84.680111;
            table[15, 1] = 84.147594;
            table[16, 1] = 83.646152;
            table[17, 1] = 83.172224;
            table[18, 1] = 82.722834;
            table[19, 1] = 82.295467;
            table[20, 1] = 80.423353;
            table[21, 1] = 78.876088;
            table[22, 1] = 77.553632;
            table[23, 1] = 76.396003;
            table[24, 1] = 75.363947;
            table[25, 1] = 74.429659;
            table[26, 1] = 73.571676;
            table[27, 1] = 72.771636;
            table[28, 1] = 72.011953;
            table[29, 1] = 71.274059;
            table[30, 1] = 70.537206;
            table[31, 1] = 69.778146;
            table[32, 1] = 68.972144;
            table[33, 1] = 68.095653;
            table[34, 1] = 67.130263;
            table[35, 1] = 66.066503;
            table[36, 1] = 63.656707;
            table[37, 1] = 60.955499;
            table[38, 1] = 58.123494;
            table[39, 1] = 55.485421;
            table[40, 1] = 53.355537;
            table[41, 1] = 51.706722;
            table[42, 1] = 50.35167;
            table[43, 1] = 49.152605;
            table[44, 1] = 48.038287;
            table[45, 1] = 46.974294;
            table[46, 1] = 44.439024;
            table[47, 1] = 42.007463;
            table[48, 1] = 39.641434;
            table[49, 1] = 37.32652;
            table[50, 1] = 35.057754;
            table[51, 1] = 32.834298;
            table[52, 1] = 30.656978;
            table[53, 1] = 28.527098;
            table[54, 1] = 26.445894;
            table[55, 1] = 24.414322;
            table[56, 1] = 22.433;
            table[57, 1] = 20.502228;
            table[58, 1] = 18.622034;
            table[59, 1] = 16.792221;
            table[60, 1] = 15.012421;
            table[61, 1] = 13.282128;
            table[62, 1] = 11.600733;
            table[63, 1] = 9.967547;
            table[64, 1] = 8.381824;
            table[65, 1] = 6.842767;
            table[66, 1] = 5.34955;
            table[67, 1] = 3.901314;
            table[68, 1] = 2.497182;
            table[69, 1] = 1.136259;
            table[70, 1] = -0.182362;
            table[71, 1] = -1.459598;
            table[72, 1] = -2.696375;
            table[73, 1] = -3.893623;
            table[74, 1] = -5.052277;
            table[75, 1] = -6.173273;
            table[76, 1] = -7.257548;
            table[77, 1] = -8.306038;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_1t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.072638;
            table[1, 1] = 100.311145;
            table[2, 1] = 95.293496;
            table[3, 1] = 91.161121;
            table[4, 1] = 87.749412;
            table[5, 1] = 84.88708;
            table[6, 1] = 82.436868;
            table[7, 1] = 80.30083;
            table[8, 1] = 78.410002;
            table[9, 1] = 76.715019;
            table[10, 1] = 75.179684;
            table[11, 1] = 73.776815;
            table[12, 1] = 72.485538;
            table[13, 1] = 71.289507;
            table[14, 1] = 70.175698;
            table[15, 1] = 69.13357;
            table[16, 1] = 68.154479;
            table[17, 1] = 67.231252;
            table[18, 1] = 66.419486;
            table[19, 1] = 65.695832;
            table[20, 1] = 62.887597;
            table[21, 1] = 60.824276;
            table[22, 1] = 59.256277;
            table[23, 1] = 58.125861;
            table[24, 1] = 57.364263;
            table[25, 1] = 56.872815;
            table[26, 1] = 56.553135;
            table[27, 1] = 56.330036;
            table[28, 1] = 56.1551;
            table[29, 1] = 56.000146;
            table[30, 1] = 55.849622;
            table[31, 1] = 55.695175;
            table[32, 1] = 55.532421;
            table[33, 1] = 55.441175;
            table[34, 1] = 55.340897;
            table[35, 1] = 55.226614;
            table[36, 1] = 54.956848;
            table[37, 1] = 54.637136;
            table[38, 1] = 54.275662;
            table[39, 1] = 53.881074;
            table[40, 1] = 53.461339;
            table[41, 1] = 53.023279;
            table[42, 1] = 52.572467;
            table[43, 1] = 52.113303;
            table[44, 1] = 51.649173;
            table[45, 1] = 51.182619;
            table[46, 1] = 50.016369;
            table[47, 1] = 48.858056;
            table[48, 1] = 47.686894;
            table[49, 1] = 46.415527;
            table[50, 1] = 45.142406;
            table[51, 1] = 43.880626;
            table[52, 1] = 42.626675;
            table[53, 1] = 41.378043;
            table[54, 1] = 40.133128;
            table[55, 1] = 38.891137;
            table[56, 1] = 37.651968;
            table[57, 1] = 36.41609;
            table[58, 1] = 35.184429;
            table[59, 1] = 33.958256;
            table[60, 1] = 32.739095;
            table[61, 1] = 31.528632;
            table[62, 1] = 30.328644;
            table[63, 1] = 29.140939;
            table[64, 1] = 27.967307;
            table[65, 1] = 26.80948;
            table[66, 1] = 25.669102;
            table[67, 1] = 24.54771;
            table[68, 1] = 23.446717;
            table[69, 1] = 22.367402;
            table[70, 1] = 21.310907;
            table[71, 1] = 20.278234;
            table[72, 1] = 19.270247;
            table[73, 1] = 18.287677;
            table[74, 1] = 17.331124;
            table[75, 1] = 16.401066;
            table[76, 1] = 15.497864;
            table[77, 1] = 14.621768;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_1t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.207795;
            table[1, 1] = 101.201153;
            table[2, 1] = 97.27569;
            table[3, 1] = 93.962223;
            table[4, 1] = 91.039761;
            table[5, 1] = 88.465201;
            table[6, 1] = 86.19493;
            table[7, 1] = 84.180555;
            table[8, 1] = 82.378187;
            table[9, 1] = 80.751526;
            table[10, 1] = 79.271536;
            table[11, 1] = 77.915173;
            table[12, 1] = 76.664088;
            table[13, 1] = 75.503546;
            table[14, 1] = 74.421596;
            table[15, 1] = 73.408444;
            table[16, 1] = 72.45598;
            table[17, 1] = 71.557424;
            table[18, 1] = 70.743872;
            table[19, 1] = 70.000497;
            table[20, 1] = 66.997312;
            table[21, 1] = 64.742294;
            table[22, 1] = 63.004826;
            table[23, 1] = 61.696926;
            table[24, 1] = 60.739278;
            table[25, 1] = 60.042931;
            table[26, 1] = 59.524763;
            table[27, 1] = 59.120278;
            table[28, 1] = 58.785257;
            table[29, 1] = 58.49127;
            table[30, 1] = 58.220445;
            table[31, 1] = 57.961522;
            table[32, 1] = 57.707313;
            table[33, 1] = 57.453165;
            table[34, 1] = 57.216194;
            table[35, 1] = 57.021253;
            table[36, 1] = 56.606301;
            table[37, 1] = 56.158366;
            table[38, 1] = 55.681923;
            table[39, 1] = 55.183111;
            table[40, 1] = 54.668063;
            table[41, 1] = 54.14218;
            table[42, 1] = 53.609882;
            table[43, 1] = 53.074622;
            table[44, 1] = 52.538992;
            table[45, 1] = 52.004873;
            table[46, 1] = 50.683629;
            table[47, 1] = 49.387137;
            table[48, 1] = 48.102683;
            table[49, 1] = 46.780682;
            table[50, 1] = 45.471003;
            table[51, 1] = 44.177958;
            table[52, 1] = 42.897326;
            table[53, 1] = 41.625877;
            table[54, 1] = 40.361341;
            table[55, 1] = 39.102333;
            table[56, 1] = 37.848247;
            table[57, 1] = 36.599145;
            table[58, 1] = 35.35563;
            table[59, 1] = 34.11873;
            table[60, 1] = 32.889786;
            table[61, 1] = 31.670356;
            table[62, 1] = 30.462123;
            table[63, 1] = 29.266828;
            table[64, 1] = 28.086212;
            table[65, 1] = 26.921968;
            table[66, 1] = 25.775707;
            table[67, 1] = 24.648934;
            table[68, 1] = 23.543032;
            table[69, 1] = 22.459251;
            table[70, 1] = 21.398702;
            table[71, 1] = 20.362358;
            table[72, 1] = 19.351053;
            table[73, 1] = 18.365488;
            table[74, 1] = 17.406235;
            table[75, 1] = 16.473746;
            table[76, 1] = 15.568355;
            table[77, 1] = 14.690291;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_1t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.26173;
            table[1, 1] = 101.593208;
            table[2, 1] = 98.436886;
            table[3, 1] = 96.157443;
            table[4, 1] = 94.23561;
            table[5, 1] = 92.437061;
            table[6, 1] = 90.672829;
            table[7, 1] = 88.9407;
            table[8, 1] = 87.270929;
            table[9, 1] = 85.691039;
            table[10, 1] = 84.215107;
            table[11, 1] = 82.84574;
            table[12, 1] = 81.57871;
            table[13, 1] = 80.406545;
            table[14, 1] = 79.320689;
            table[15, 1] = 78.312624;
            table[16, 1] = 77.374396;
            table[17, 1] = 76.49882;
            table[18, 1] = 75.679526;
            table[19, 1] = 74.910924;
            table[20, 1] = 71.67863;
            table[21, 1] = 69.202573;
            table[22, 1] = 67.271021;
            table[23, 1] = 65.760859;
            table[24, 1] = 64.580455;
            table[25, 1] = 63.651724;
            table[26, 1] = 62.908776;
            table[27, 1] = 62.299201;
            table[28, 1] = 61.783494;
            table[29, 1] = 61.332909;
            table[30, 1] = 60.926902;
            table[31, 1] = 60.550872;
            table[32, 1] = 60.194413;
            table[33, 1] = 59.850069;
            table[34, 1] = 59.512457;
            table[35, 1] = 59.177685;
            table[36, 1] = 58.547194;
            table[37, 1] = 57.954225;
            table[38, 1] = 57.347663;
            table[39, 1] = 56.730636;
            table[40, 1] = 56.107122;
            table[41, 1] = 55.480885;
            table[42, 1] = 54.855057;
            table[43, 1] = 54.232046;
            table[44, 1] = 53.613588;
            table[45, 1] = 53.000858;
            table[46, 1] = 51.498046;
            table[47, 1] = 50.03834;
            table[48, 1] = 48.618856;
            table[49, 1] = 47.234139;
            table[50, 1] = 45.878127;
            table[51, 1] = 44.545095;
            table[52, 1] = 43.230085;
            table[53, 1] = 41.929084;
            table[54, 1] = 40.639067;
            table[55, 1] = 39.357961;
            table[56, 1] = 38.084566;
            table[57, 1] = 36.818442;
            table[58, 1] = 35.559787;
            table[59, 1] = 34.309308;
            table[60, 1] = 33.068102;
            table[61, 1] = 31.837537;
            table[62, 1] = 30.619159;
            table[63, 1] = 29.414603;
            table[64, 1] = 28.225527;
            table[65, 1] = 27.053559;
            table[66, 1] = 25.900254;
            table[67, 1] = 24.767068;
            table[68, 1] = 23.65534;
            table[69, 1] = 22.566274;
            table[70, 1] = 21.500943;
            table[71, 1] = 20.460276;
            table[72, 1] = 19.445072;
            table[73, 1] = 18.455993;
            table[74, 1] = 17.493577;
            table[75, 1] = 16.558242;
            table[76, 1] = 15.650292;
            table[77, 1] = 14.769928;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_1t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.265932;
            table[1, 1] = 101.612228;
            table[2, 1] = 98.505661;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.34423;
            table[9, 1] = 89.502027;
            table[10, 1] = 88.690666;
            table[11, 1] = 87.887334;
            table[12, 1] = 87.076191;
            table[13, 1] = 86.249445;
            table[14, 1] = 85.407138;
            table[15, 1] = 84.555303;
            table[16, 1] = 83.703207;
            table[17, 1] = 82.860715;
            table[18, 1] = 82.036502;
            table[19, 1] = 81.237217;
            table[20, 1] = 77.714189;
            table[21, 1] = 74.954908;
            table[22, 1] = 72.7738;
            table[23, 1] = 71.002888;
            table[24, 1] = 69.534853;
            table[25, 1] = 68.305745;
            table[26, 1] = 67.27199;
            table[27, 1] = 66.396821;
            table[28, 1] = 65.646854;
            table[29, 1] = 64.99296;
            table[30, 1] = 64.411174;
            table[31, 1] = 63.882604;
            table[32, 1] = 63.392721;
            table[33, 1] = 62.930473;
            table[34, 1] = 62.487487;
            table[35, 1] = 62.057434;
            table[36, 1] = 61.218213;
            table[37, 1] = 60.387402;
            table[38, 1] = 59.563991;
            table[39, 1] = 58.784401;
            table[40, 1] = 58.011947;
            table[41, 1] = 57.248201;
            table[42, 1] = 56.494537;
            table[43, 1] = 55.751922;
            table[44, 1] = 55.020897;
            table[45, 1] = 54.301639;
            table[46, 1] = 52.553957;
            table[47, 1] = 50.875526;
            table[48, 1] = 49.276489;
            table[49, 1] = 47.811138;
            table[50, 1] = 46.396816;
            table[51, 1] = 45.013941;
            table[52, 1] = 43.65643;
            table[53, 1] = 42.319122;
            table[54, 1] = 40.997922;
            table[55, 1] = 39.689808;
            table[56, 1] = 38.392776;
            table[57, 1] = 37.105728;
            table[58, 1] = 35.828346;
            table[59, 1] = 34.560944;
            table[60, 1] = 33.304325;
            table[61, 1] = 32.059648;
            table[62, 1] = 30.828307;
            table[63, 1] = 29.611828;
            table[64, 1] = 28.411789;
            table[65, 1] = 27.229751;
            table[66, 1] = 26.067218;
            table[67, 1] = 24.925595;
            table[68, 1] = 23.806172;
            table[69, 1] = 22.710108;
            table[70, 1] = 21.638425;
            table[71, 1] = 20.592007;
            table[72, 1] = 19.571604;
            table[73, 1] = 18.577833;
            table[74, 1] = 17.611189;
            table[75, 1] = 16.672046;
            table[76, 1] = 15.760669;
            table[77, 1] = 14.877222;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_1t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.612579;
            table[2, 1] = 98.506638;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.717924;
            table[13, 1] = 87.164006;
            table[14, 1] = 86.644333;
            table[15, 1] = 86.153563;
            table[16, 1] = 85.687045;
            table[17, 1] = 85.240622;
            table[18, 1] = 84.810517;
            table[19, 1] = 84.393265;
            table[20, 1] = 82.401313;
            table[21, 1] = 80.422118;
            table[22, 1] = 78.456335;
            table[23, 1] = 76.605592;
            table[24, 1] = 74.931141;
            table[25, 1] = 73.441754;
            table[26, 1] = 72.123503;
            table[27, 1] = 70.9579;
            table[28, 1] = 69.926753;
            table[29, 1] = 69.012016;
            table[30, 1] = 68.195596;
            table[31, 1] = 67.460016;
            table[32, 1] = 66.789316;
            table[33, 1] = 66.16962;
            table[34, 1] = 65.58929;
            table[35, 1] = 65.038806;
            table[36, 1] = 63.998315;
            table[37, 1] = 63.004193;
            table[38, 1] = 62.029939;
            table[39, 1] = 61.060629;
            table[40, 1] = 60.08874;
            table[41, 1] = 59.111189;
            table[42, 1] = 58.127256;
            table[43, 1] = 57.137219;
            table[44, 1] = 56.275914;
            table[45, 1] = 55.46887;
            table[46, 1] = 53.51624;
            table[47, 1] = 51.650862;
            table[48, 1] = 49.89727;
            table[49, 1] = 48.367609;
            table[50, 1] = 46.907064;
            table[51, 1] = 45.483478;
            table[52, 1] = 44.090253;
            table[53, 1] = 42.721515;
            table[54, 1] = 41.372408;
            table[55, 1] = 40.039213;
            table[56, 1] = 38.719334;
            table[57, 1] = 37.411217;
            table[58, 1] = 36.114221;
            table[59, 1] = 34.828449;
            table[60, 1] = 33.554588;
            table[61, 1] = 32.29374;
            table[62, 1] = 31.047281;
            table[63, 1] = 29.816734;
            table[64, 1] = 28.603679;
            table[65, 1] = 27.409671;
            table[66, 1] = 26.236193;
            table[67, 1] = 25.084619;
            table[68, 1] = 23.956194;
            table[69, 1] = 22.852024;
            table[70, 1] = 21.77307;
            table[71, 1] = 20.720153;
            table[72, 1] = 19.693954;
            table[73, 1] = 18.695028;
            table[74, 1] = 17.723805;
            table[75, 1] = 16.780599;
            table[76, 1] = 15.865623;
            table[77, 1] = 14.978985;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_1t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170133;
            table[14, 1] = 86.654402;
            table[15, 1] = 86.16972;
            table[16, 1] = 85.712285;
            table[17, 1] = 85.278956;
            table[18, 1] = 84.86711;
            table[19, 1] = 84.474532;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.789991;
            table[25, 1] = 76.745132;
            table[26, 1] = 75.722258;
            table[27, 1] = 74.717408;
            table[28, 1] = 73.732545;
            table[29, 1] = 72.772367;
            table[30, 1] = 71.843412;
            table[31, 1] = 70.953272;
            table[32, 1] = 70.108649;
            table[33, 1] = 69.313227;
            table[34, 1] = 68.566794;
            table[35, 1] = 67.865831;
            table[36, 1] = 66.577272;
            table[37, 1] = 65.398039;
            table[38, 1] = 64.284889;
            table[39, 1] = 63.206728;
            table[40, 1] = 62.143606;
            table[41, 1] = 61.083991;
            table[42, 1] = 60.021997;
            table[43, 1] = 58.955075;
            table[44, 1] = 57.882288;
            table[45, 1] = 56.803186;
            table[46, 1] = 54.376677;
            table[47, 1] = 52.352033;
            table[48, 1] = 50.46597;
            table[49, 1] = 48.88792;
            table[50, 1] = 47.392952;
            table[51, 1] = 45.937167;
            table[52, 1] = 44.514124;
            table[53, 1] = 43.117706;
            table[54, 1] = 41.742663;
            table[55, 1] = 40.384884;
            table[56, 1] = 39.041482;
            table[57, 1] = 37.710746;
            table[58, 1] = 36.392006;
            table[59, 1] = 35.085438;
            table[60, 1] = 33.791853;
            table[61, 1] = 32.512495;
            table[62, 1] = 31.248864;
            table[63, 1] = 30.002569;
            table[64, 1] = 28.77523;
            table[65, 1] = 27.568395;
            table[66, 1] = 26.383503;
            table[67, 1] = 25.221851;
            table[68, 1] = 24.084586;
            table[69, 1] = 22.972705;
            table[70, 1] = 21.887055;
            table[71, 1] = 20.828342;
            table[72, 1] = 19.797143;
            table[73, 1] = 18.793909;
            table[74, 1] = 17.81898;
            table[75, 1] = 16.87259;
            table[76, 1] = 15.954876;
            table[77, 1] = 15.065885;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_1t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170146;
            table[14, 1] = 86.654421;
            table[15, 1] = 86.16975;
            table[16, 1] = 85.712332;
            table[17, 1] = 85.279029;
            table[18, 1] = 84.867225;
            table[19, 1] = 84.474709;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.028945;
            table[30, 1] = 73.36085;
            table[31, 1] = 72.711409;
            table[32, 1] = 72.073604;
            table[33, 1] = 71.440628;
            table[34, 1] = 70.806196;
            table[35, 1] = 70.165871;
            table[36, 1] = 68.867732;
            table[37, 1] = 67.576402;
            table[38, 1] = 66.328335;
            table[39, 1] = 65.131917;
            table[40, 1] = 63.974865;
            table[41, 1] = 62.839666;
            table[42, 1] = 61.711773;
            table[43, 1] = 60.581758;
            table[44, 1] = 59.444511;
            table[45, 1] = 58.297507;
            table[46, 1] = 55.377318;
            table[47, 1] = 52.972887;
            table[48, 1] = 50.959325;
            table[49, 1] = 49.342109;
            table[50, 1] = 47.819653;
            table[51, 1] = 46.335578;
            table[52, 1] = 44.884384;
            table[53, 1] = 43.460334;
            table[54, 1] = 42.058257;
            table[55, 1] = 40.674063;
            table[56, 1] = 39.304974;
            table[57, 1] = 37.949515;
            table[58, 1] = 36.607345;
            table[59, 1] = 35.278988;
            table[60, 1] = 33.965555;
            table[61, 1] = 32.668488;
            table[62, 1] = 31.389363;
            table[63, 1] = 30.129757;
            table[64, 1] = 28.891163;
            table[65, 1] = 27.674948;
            table[66, 1] = 26.48233;
            table[67, 1] = 25.31438;
            table[68, 1] = 24.172027;
            table[69, 1] = 23.056064;
            table[70, 1] = 21.967159;
            table[71, 1] = 20.905864;
            table[72, 1] = 19.872623;
            table[73, 1] = 18.867782;
            table[74, 1] = 17.89159;
            table[75, 1] = 16.944211;
            table[76, 1] = 16.025723;
            table[77, 1] = 15.13613;



            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_600m_1t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170146;
            table[14, 1] = 86.654421;
            table[15, 1] = 86.16975;
            table[16, 1] = 85.712332;
            table[17, 1] = 85.279029;
            table[18, 1] = 84.867225;
            table[19, 1] = 84.474709;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.441404;
            table[31, 1] = 72.85296;
            table[32, 1] = 72.284253;
            table[33, 1] = 71.741337;
            table[34, 1] = 71.220977;
            table[35, 1] = 70.720182;
            table[36, 1] = 69.765817;
            table[37, 1] = 68.853437;
            table[38, 1] = 67.947261;
            table[39, 1] = 66.994509;
            table[40, 1] = 65.951477;
            table[41, 1] = 64.816726;
            table[42, 1] = 63.617903;
            table[43, 1] = 62.381784;
            table[44, 1] = 61.123291;
            table[45, 1] = 59.848186;
            table[46, 1] = 56.592519;
            table[47, 1] = 53.5296;
            table[48, 1] = 51.342099;
            table[49, 1] = 49.669977;
            table[50, 1] = 48.11403;
            table[51, 1] = 46.596587;
            table[52, 1] = 45.112543;
            table[53, 1] = 43.657718;
            table[54, 1] = 42.227939;
            table[55, 1] = 40.819366;
            table[56, 1] = 39.429094;
            table[57, 1] = 38.055487;
            table[58, 1] = 36.698124;
            table[59, 1] = 35.357474;
            table[60, 1] = 34.034521;
            table[61, 1] = 32.73046;
            table[62, 1] = 31.446521;
            table[63, 1] = 30.183878;
            table[64, 1] = 28.943624;
            table[65, 1] = 27.72676;
            table[66, 1] = 26.534202;
            table[67, 1] = 25.366782;
            table[68, 1] = 24.225245;
            table[69, 1] = 23.110255;
            table[70, 1] = 22.022389;
            table[71, 1] = 20.962137;
            table[72, 1] = 19.929907;
            table[73, 1] = 18.926019;
            table[74, 1] = 17.950713;
            table[75, 1] = 17.004145;
            table[76, 1] = 16.086395;
            table[77, 1] = 15.197468;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_50t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.233474;
            table[1, 1] = 82.426892;
            table[2, 1] = 74.500997;
            table[3, 1] = 68.368341;
            table[4, 1] = 63.384988;
            table[5, 1] = 59.209334;
            table[6, 1] = 55.627815;
            table[7, 1] = 52.498812;
            table[8, 1] = 49.724664;
            table[9, 1] = 47.235756;
            table[10, 1] = 44.980997;
            table[11, 1] = 42.921934;
            table[12, 1] = 41.028971;
            table[13, 1] = 39.278881;
            table[14, 1] = 37.65312;
            table[15, 1] = 36.136652;
            table[16, 1] = 34.717117;
            table[17, 1] = 33.384229;
            table[18, 1] = 32.129329;
            table[19, 1] = 30.945057;
            table[20, 1] = 25.888694;
            table[21, 1] = 21.921474;
            table[22, 1] = 18.729253;
            table[23, 1] = 16.113692;
            table[24, 1] = 13.939255;
            table[25, 1] = 12.108286;
            table[26, 1] = 10.547741;
            table[27, 1] = 9.201441;
            table[28, 1] = 8.025296;
            table[29, 1] = 6.984223;
            table[30, 1] = 6.050072;
            table[31, 1] = 5.200164;
            table[32, 1] = 4.41621;
            table[33, 1] = 3.683473;
            table[34, 1] = 2.990107;
            table[35, 1] = 2.326621;
            table[36, 1] = 1.060562;
            table[37, 1] = -0.158225;
            table[38, 1] = -1.356401;
            table[39, 1] = -2.549555;
            table[40, 1] = -3.746076;
            table[41, 1] = -4.949737;
            table[42, 1] = -6.161413;
            table[43, 1] = -7.380224;
            table[44, 1] = -8.604293;
            table[45, 1] = -9.831241;
            table[46, 1] = -12.894533;
            table[47, 1] = -15.922888;
            table[48, 1] = -18.887771;
            table[49, 1] = -21.770782;
            table[50, 1] = -24.562405;
            table[51, 1] = -27.260059;
            table[52, 1] = -29.866226;
            table[53, 1] = -32.386908;
            table[54, 1] = -34.830413;
            table[55, 1] = -37.206438;
            table[56, 1] = -39.52537;
            table[57, 1] = -41.797757;
            table[58, 1] = -44.033893;
            table[59, 1] = -46.243495;
            table[60, 1] = -48.435421;
            table[61, 1] = -50.61745;
            table[62, 1] = -52.796071;
            table[63, 1] = -54.976316;
            table[64, 1] = -57.16161;
            table[65, 1] = -59.353647;
            table[66, 1] = -61.552311;
            table[67, 1] = -63.755629;
            table[68, 1] = -65.95978;
            table[69, 1] = -68.159159;
            table[70, 1] = -70.346507;
            table[71, 1] = -72.513108;
            table[72, 1] = -74.649055;
            table[73, 1] = -76.743588;
            table[74, 1] = -78.785479;
            table[75, 1] = -80.763473;
            table[76, 1] = -82.66674;
            table[77, 1] = -84.485345;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_50t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 96.509153;
            table[1, 1] = 85.910188;
            table[2, 1] = 79.13464;
            table[3, 1] = 73.847196;
            table[4, 1] = 69.411974;
            table[5, 1] = 65.580198;
            table[6, 1] = 62.215867;
            table[7, 1] = 59.226709;
            table[8, 1] = 56.544263;
            table[9, 1] = 54.116037;
            table[10, 1] = 51.901136;
            table[11, 1] = 49.867305;
            table[12, 1] = 47.988805;
            table[13, 1] = 46.244859;
            table[14, 1] = 44.618504;
            table[15, 1] = 43.095734;
            table[16, 1] = 41.664867;
            table[17, 1] = 40.316062;
            table[18, 1] = 39.040953;
            table[19, 1] = 37.832367;
            table[20, 1] = 32.595876;
            table[21, 1] = 28.354544;
            table[22, 1] = 24.801547;
            table[23, 1] = 21.754006;
            table[24, 1] = 19.099357;
            table[25, 1] = 16.766276;
            table[26, 1] = 14.706603;
            table[27, 1] = 12.884072;
            table[28, 1] = 11.268035;
            table[29, 1] = 9.830627;
            table[30, 1] = 8.545936;
            table[31, 1] = 7.39008;
            table[32, 1] = 6.341506;
            table[33, 1] = 5.38121;
            table[34, 1] = 4.492787;
            table[35, 1] = 3.662321;
            table[36, 1] = 2.130753;
            table[37, 1] = 0.716088;
            table[38, 1] = -0.628133;
            table[39, 1] = -1.931573;
            table[40, 1] = -3.212504;
            table[41, 1] = -4.481637;
            table[42, 1] = -5.744745;
            table[43, 1] = -7.004444;
            table[44, 1] = -8.261376;
            table[45, 1] = -9.515006;
            table[46, 1] = -12.626079;
            table[47, 1] = -15.684866;
            table[48, 1] = -18.669933;
            table[49, 1] = -21.566792;
            table[50, 1] = -24.368197;
            table[51, 1] = -27.072938;
            table[52, 1] = -29.684356;
            table[53, 1] = -32.209006;
            table[54, 1] = -34.655562;
            table[55, 1] = -37.03397;
            table[56, 1] = -39.354789;
            table[57, 1] = -41.628688;
            table[58, 1] = -43.866052;
            table[59, 1] = -46.076657;
            table[60, 1] = -48.269414;
            table[61, 1] = -50.452135;
            table[62, 1] = -52.631337;
            table[63, 1] = -54.812075;
            table[64, 1] = -56.997787;
            table[65, 1] = -59.190182;
            table[66, 1] = -61.389155;
            table[67, 1] = -63.59274;
            table[68, 1] = -65.797124;
            table[69, 1] = -67.996707;
            table[70, 1] = -70.184233;
            table[71, 1] = -72.350991;
            table[72, 1] = -74.487078;
            table[73, 1] = -76.581734;
            table[74, 1] = -78.623735;
            table[75, 1] = -80.601826;
            table[76, 1] = -82.505182;
            table[77, 1] = -84.323866;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_50t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 98.661743;
            table[1, 1] = 88.757762;
            table[2, 1] = 82.671395;
            table[3, 1] = 78.077502;
            table[4, 1] = 74.253131;
            table[5, 1] = 70.907659;
            table[6, 1] = 67.908503;
            table[7, 1] = 65.185886;
            table[8, 1] = 62.695912;
            table[9, 1] = 60.406379;
            table[10, 1] = 58.291431;
            table[11, 1] = 56.329456;
            table[12, 1] = 54.502111;
            table[13, 1] = 52.793735;
            table[14, 1] = 51.190914;
            table[15, 1] = 49.68212;
            table[16, 1] = 48.257416;
            table[17, 1] = 46.908194;
            table[18, 1] = 45.626969;
            table[19, 1] = 44.4072;
            table[20, 1] = 39.051136;
            table[21, 1] = 34.599268;
            table[22, 1] = 30.756257;
            table[23, 1] = 27.352092;
            table[24, 1] = 24.292122;
            table[25, 1] = 21.526306;
            table[26, 1] = 19.027783;
            table[27, 1] = 16.778559;
            table[28, 1] = 14.761473;
            table[29, 1] = 12.957013;
            table[30, 1] = 11.343135;
            table[31, 1] = 9.896448;
            table[32, 1] = 8.593661;
            table[33, 1] = 7.412761;
            table[34, 1] = 6.333762;
            table[35, 1] = 5.339061;
            table[36, 1] = 3.544267;
            table[37, 1] = 1.933783;
            table[38, 1] = 0.442696;
            table[39, 1] = -0.972094;
            table[40, 1] = -2.338461;
            table[41, 1] = -3.673943;
            table[42, 1] = -4.989191;
            table[43, 1] = -6.290326;
            table[44, 1] = -7.580536;
            table[45, 1] = -8.861155;
            table[46, 1] = -12.020474;
            table[47, 1] = -15.109931;
            table[48, 1] = -18.115328;
            table[49, 1] = -21.026147;
            table[50, 1] = -23.837429;
            table[51, 1] = -26.549342;
            table[52, 1] = -29.166085;
            table[53, 1] = -31.694766;
            table[54, 1] = -34.144429;
            table[55, 1] = -36.525266;
            table[56, 1] = -38.848011;
            table[57, 1] = -41.123456;
            table[58, 1] = -43.362075;
            table[59, 1] = -45.573709;
            table[60, 1] = -47.767317;
            table[61, 1] = -49.950746;
            table[62, 1] = -52.130545;
            table[63, 1] = -54.311787;
            table[64, 1] = -56.497929;
            table[65, 1] = -58.690692;
            table[66, 1] = -60.889982;
            table[67, 1] = -63.093842;
            table[68, 1] = -65.298465;
            table[69, 1] = -67.498256;
            table[70, 1] = -69.685966;
            table[71, 1] = -71.852885;
            table[72, 1] = -73.989115;
            table[73, 1] = -76.083898;
            table[74, 1] = -78.126012;
            table[75, 1] = -80.104204;
            table[76, 1] = -82.00765;
            table[77, 1] = -83.826415;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_50t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 101.148112;
            table[1, 1] = 91.971352;
            table[2, 1] = 86.395388;
            table[3, 1] = 82.308108;
            table[4, 1] = 79.006092;
            table[5, 1] = 76.172083;
            table[6, 1] = 73.642723;
            table[7, 1] = 71.32897;
            table[8, 1] = 69.180884;
            table[9, 1] = 67.169448;
            table[10, 1] = 65.276613;
            table[11, 1] = 63.489928;
            table[12, 1] = 61.799759;
            table[13, 1] = 60.197947;
            table[14, 1] = 58.677196;
            table[15, 1] = 57.23082;
            table[16, 1] = 55.85264;
            table[17, 1] = 54.536941;
            table[18, 1] = 53.278452;
            table[19, 1] = 52.072327;
            table[20, 1] = 46.684263;
            table[21, 1] = 42.080591;
            table[22, 1] = 37.993884;
            table[23, 1] = 34.268845;
            table[24, 1] = 30.825985;
            table[25, 1] = 27.633635;
            table[26, 1] = 24.685226;
            table[27, 1] = 21.982398;
            table[28, 1] = 19.524625;
            table[29, 1] = 17.304715;
            table[30, 1] = 15.308404;
            table[31, 1] = 13.516095;
            table[32, 1] = 11.905236;
            table[33, 1] = 10.452473;
            table[34, 1] = 9.135227;
            table[35, 1] = 7.932683;
            table[36, 1] = 5.799888;
            table[37, 1] = 3.934062;
            table[38, 1] = 2.2493;
            table[39, 1] = 0.686452;
            table[40, 1] = -0.794206;
            table[41, 1] = -2.218847;
            table[42, 1] = -3.604387;
            table[43, 1] = -4.961516;
            table[44, 1] = -6.296776;
            table[45, 1] = -7.613981;
            table[46, 1] = -10.838801;
            table[47, 1] = -13.969966;
            table[48, 1] = -17.003048;
            table[49, 1] = -19.932902;
            table[50, 1] = -22.75767;
            table[51, 1] = -25.479384;
            table[52, 1] = -28.103413;
            table[53, 1] = -30.637616;
            table[54, 1] = -33.091535;
            table[55, 1] = -35.475704;
            table[56, 1] = -37.801092;
            table[57, 1] = -40.07866;
            table[58, 1] = -42.319001;
            table[59, 1] = -44.532048;
            table[60, 1] = -46.726825;
            table[61, 1] = -48.91123;
            table[62, 1] = -51.091848;
            table[63, 1] = -53.273783;
            table[64, 1] = -55.460515;
            table[65, 1] = -57.653785;
            table[66, 1] = -59.853511;
            table[67, 1] = -62.057748;
            table[68, 1] = -64.2627;
            table[69, 1] = -66.462778;
            table[70, 1] = -68.65074;
            table[71, 1] = -70.817882;
            table[72, 1] = -72.954307;
            table[73, 1] = -75.049265;
            table[74, 1] = -77.091534;
            table[75, 1] = -79.069865;
            table[76, 1] = -80.973435;
            table[77, 1] = -82.792312;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_50t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 103.509076;
            table[1, 1] = 95.243999;
            table[2, 1] = 90.170883;
            table[3, 1] = 86.474152;
            table[4, 1] = 83.536323;
            table[5, 1] = 81.068492;
            table[6, 1] = 78.911591;
            table[7, 1] = 76.969512;
            table[8, 1] = 75.181216;
            table[9, 1] = 73.507135;
            table[10, 1] = 71.921585;
            table[11, 1] = 70.408016;
            table[12, 1] = 68.95583;
            table[13, 1] = 67.558177;
            table[14, 1] = 66.210461;
            table[15, 1] = 64.90935;
            table[16, 1] = 63.652175;
            table[17, 1] = 62.436573;
            table[18, 1] = 61.260298;
            table[19, 1] = 60.121141;
            table[20, 1] = 54.90587;
            table[21, 1] = 50.306402;
            table[22, 1] = 46.118719;
            table[23, 1] = 42.209767;
            table[24, 1] = 38.510428;
            table[25, 1] = 34.998845;
            table[26, 1] = 31.681456;
            table[27, 1] = 28.576001;
            table[28, 1] = 25.699225;
            table[29, 1] = 23.060063;
            table[30, 1] = 20.657521;
            table[31, 1] = 18.481628;
            table[32, 1] = 16.515849;
            table[33, 1] = 14.739782;
            table[34, 1] = 13.131474;
            table[35, 1] = 11.669139;
            table[36, 1] = 9.102221;
            table[37, 1] = 6.898914;
            table[38, 1] = 4.952728;
            table[39, 1] = 3.186875;
            table[40, 1] = 1.547648;
            table[41, 1] = -0.001808;
            table[42, 1] = -1.48644;
            table[43, 1] = -2.922936;
            table[44, 1] = -4.322325;
            table[45, 1] = -5.691788;
            table[46, 1] = -9.010568;
            table[47, 1] = -12.201831;
            table[48, 1] = -15.274926;
            table[49, 1] = -18.232348;
            table[50, 1] = -21.076677;
            table[51, 1] = -23.812627;
            table[52, 1] = -26.447246;
            table[53, 1] = -28.989481;
            table[54, 1] = -31.449595;
            table[55, 1] = -33.838615;
            table[56, 1] = -36.167853;
            table[57, 1] = -38.448513;
            table[58, 1] = -40.691366;
            table[59, 1] = -42.906472;
            table[60, 1] = -45.102952;
            table[61, 1] = -47.288778;
            table[62, 1] = -49.470591;
            table[63, 1] = -51.653538;
            table[64, 1] = -53.841131;
            table[65, 1] = -56.035139;
            table[66, 1] = -58.235501;
            table[67, 1] = -60.440289;
            table[68, 1] = -62.64572;
            table[69, 1] = -64.846217;
            table[70, 1] = -67.034547;
            table[71, 1] = -69.202012;
            table[72, 1] = -71.338725;
            table[73, 1] = -73.433936;
            table[74, 1] = -75.476432;
            table[75, 1] = -77.454966;
            table[76, 1] = -79.358717;
            table[77, 1] = -81.177757;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_50t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 105.319204;
            table[1, 1] = 98.115523;
            table[2, 1] = 93.677464;
            table[3, 1] = 90.429385;
            table[4, 1] = 87.851319;
            table[5, 1] = 85.701186;
            table[6, 1] = 83.844581;
            table[7, 1] = 82.198236;
            table[8, 1] = 80.706622;
            table[9, 1] = 79.330825;
            table[10, 1] = 78.042722;
            table[11, 1] = 76.821674;
            table[12, 1] = 75.652503;
            table[13, 1] = 74.524147;
            table[14, 1] = 73.428685;
            table[15, 1] = 72.360577;
            table[16, 1] = 71.316054;
            table[17, 1] = 70.292616;
            table[18, 1] = 69.288628;
            table[19, 1] = 68.303013;
            table[20, 1] = 63.627786;
            table[21, 1] = 59.317457;
            table[22, 1] = 55.279556;
            table[23, 1] = 51.426891;
            table[24, 1] = 47.702633;
            table[25, 1] = 44.085268;
            table[26, 1] = 40.581402;
            table[27, 1] = 37.214207;
            table[28, 1] = 34.012155;
            table[29, 1] = 31.000504;
            table[30, 1] = 28.196354;
            table[31, 1] = 25.606963;
            table[32, 1] = 23.230378;
            table[33, 1] = 21.057336;
            table[34, 1] = 19.073555;
            table[35, 1] = 17.261879;
            table[36, 1] = 14.081786;
            table[37, 1] = 11.376713;
            table[38, 1] = 9.026903;
            table[39, 1] = 6.938675;
            table[40, 1] = 5.042215;
            table[41, 1] = 3.286941;
            table[42, 1] = 1.636818;
            table[43, 1] = 0.066439;
            table[44, 1] = -1.441992;
            table[45, 1] = -2.900885;
            table[46, 1] = -6.381755;
            table[47, 1] = -9.677564;
            table[48, 1] = -12.820643;
            table[49, 1] = -15.826461;
            table[50, 1] = -18.705218;
            table[51, 1] = -21.466268;
            table[52, 1] = -24.119586;
            table[53, 1] = -26.676015;
            table[54, 1] = -29.147088;
            table[55, 1] = -31.544694;
            table[56, 1] = -33.880749;
            table[57, 1] = -36.166888;
            table[58, 1] = -38.414189;
            table[59, 1] = -40.632946;
            table[60, 1] = -42.832446;
            table[61, 1] = -45.020793;
            table[62, 1] = -47.204724;
            table[63, 1] = -49.389464;
            table[64, 1] = -51.578586;
            table[65, 1] = -53.773903;
            table[66, 1] = -55.975393;
            table[67, 1] = -58.181158;
            table[68, 1] = -60.387439;
            table[69, 1] = -62.588679;
            table[70, 1] = -64.777661;
            table[71, 1] = -66.945702;
            table[72, 1] = -69.082923;
            table[73, 1] = -71.178586;
            table[74, 1] = -73.221484;
            table[75, 1] = -75.200376;
            table[76, 1] = -77.10445;
            table[77, 1] = -78.923779;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_50t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.327932;
            table[1, 1] = 99.91626;
            table[2, 1] = 96.070286;
            table[3, 1] = 93.28947;
            table[4, 1] = 91.098686;
            table[5, 1] = 89.283869;
            table[6, 1] = 87.729276;
            table[7, 1] = 86.364676;
            table[8, 1] = 85.143858;
            table[9, 1] = 84.034509;
            table[10, 1] = 83.012945;
            table[11, 1] = 82.061153;
            table[12, 1] = 81.165039;
            table[13, 1] = 80.313345;
            table[14, 1] = 79.496956;
            table[15, 1] = 78.708446;
            table[16, 1] = 77.941764;
            table[17, 1] = 77.192013;
            table[18, 1] = 76.455274;
            table[19, 1] = 75.728468;
            table[20, 1] = 72.179414;
            table[21, 1] = 68.706284;
            table[22, 1] = 65.290549;
            table[23, 1] = 61.921272;
            table[24, 1] = 58.579709;
            table[25, 1] = 55.251954;
            table[26, 1] = 51.93748;
            table[27, 1] = 48.649875;
            table[28, 1] = 45.413047;
            table[29, 1] = 42.2559;
            table[30, 1] = 39.20726;
            table[31, 1] = 36.291993;
            table[32, 1] = 33.528651;
            table[33, 1] = 30.9286;
            table[34, 1] = 28.496298;
            table[35, 1] = 26.230295;
            table[36, 1] = 22.169926;
            table[37, 1] = 18.668163;
            table[38, 1] = 15.628381;
            table[39, 1] = 12.957748;
            table[40, 1] = 10.576596;
            table[41, 1] = 8.420702;
            table[42, 1] = 6.440166;
            table[43, 1] = 4.597054;
            table[44, 1] = 2.862892;
            table[45, 1] = 1.216447;
            table[46, 1] = -2.609932;
            table[47, 1] = -6.132423;
            table[48, 1] = -9.428931;
            table[49, 1] = -12.541643;
            table[50, 1] = -15.496835;
            table[51, 1] = -18.313822;
            table[52, 1] = -21.008923;
            table[53, 1] = -23.597138;
            table[54, 1] = -26.092786;
            table[55, 1] = -28.509671;
            table[56, 1] = -30.861049;
            table[57, 1] = -33.159509;
            table[58, 1] = -35.416825;
            table[59, 1] = -37.6438;
            table[60, 1] = -39.850105;
            table[61, 1] = -42.044131;
            table[62, 1] = -44.232839;
            table[63, 1] = -46.421622;
            table[64, 1] = -48.614189;
            table[65, 1] = -50.812459;
            table[66, 1] = -53.016494;
            table[67, 1] = -55.224464;
            table[68, 1] = -57.432663;
            table[69, 1] = -59.635581;
            table[70, 1] = -61.826035;
            table[71, 1] = -63.995373;
            table[72, 1] = -66.133742;
            table[73, 1] = -68.230424;
            table[74, 1] = -70.27423;
            table[75, 1] = -72.253933;
            table[76, 1] = -74.158733;
            table[77, 1] = -75.978715;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_50t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.731901;
            table[1, 1] = 100.632482;
            table[2, 1] = 97.049054;
            table[3, 1] = 94.497853;
            table[4, 1] = 92.51314;
            table[5, 1] = 90.887149;
            table[6, 1] = 89.508867;
            table[7, 1] = 88.311887;
            table[8, 1] = 87.253261;
            table[9, 1] = 86.303553;
            table[10, 1] = 85.441669;
            table[11, 1] = 84.651932;
            table[12, 1] = 83.922341;
            table[13, 1] = 83.243476;
            table[14, 1] = 82.607779;
            table[15, 1] = 82.009071;
            table[16, 1] = 81.442221;
            table[17, 1] = 80.902902;
            table[18, 1] = 80.387422;
            table[19, 1] = 79.892596;
            table[20, 1] = 77.642045;
            table[21, 1] = 75.603555;
            table[22, 1] = 73.632669;
            table[23, 1] = 71.640678;
            table[24, 1] = 69.570735;
            table[25, 1] = 67.387748;
            table[26, 1] = 65.075695;
            table[27, 1] = 62.636277;
            table[28, 1] = 60.085412;
            table[29, 1] = 57.447962;
            table[30, 1] = 54.752752;
            table[31, 1] = 52.02914;
            table[32, 1] = 49.30509;
            table[33, 1] = 46.606115;
            table[34, 1] = 43.954623;
            table[35, 1] = 41.369465;
            table[36, 1] = 36.454432;
            table[37, 1] = 31.936235;
            table[38, 1] = 27.837811;
            table[39, 1] = 24.144847;
            table[40, 1] = 20.821457;
            table[41, 1] = 17.822549;
            table[42, 1] = 15.101893;
            table[43, 1] = 12.616598;
            table[44, 1] = 10.329077;
            table[45, 1] = 8.207529;
            table[46, 1] = 3.468657;
            table[47, 1] = -0.679351;
            table[48, 1] = -4.411588;
            table[49, 1] = -7.83386;
            table[50, 1] = -11.013457;
            table[51, 1] = -13.99629;
            table[52, 1] = -16.816176;
            table[53, 1] = -19.499837;
            table[54, 1] = -22.069588;
            table[55, 1] = -24.544794;
            table[56, 1] = -26.942642;
            table[57, 1] = -29.278547;
            table[58, 1] = -31.566348;
            table[59, 1] = -33.818374;
            table[60, 1] = -36.045443;
            table[61, 1] = -38.256816;
            table[62, 1] = -40.460121;
            table[63, 1] = -42.661273;
            table[64, 1] = -44.864384;
            table[65, 1] = -47.071696;
            table[66, 1] = -49.283525;
            table[67, 1] = -51.49825;
            table[68, 1] = -53.712329;
            table[69, 1] = -55.920389;
            table[70, 1] = -58.115359;
            table[71, 1] = -60.288677;
            table[72, 1] = -62.430567;
            table[73, 1] = -64.530376;
            table[74, 1] = -66.576966;
            table[75, 1] = -68.559158;
            table[76, 1] = -70.466188;
            table[77, 1] = -72.288174;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_10t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.233474;
            table[1, 1] = 82.426892;
            table[2, 1] = 74.500997;
            table[3, 1] = 68.368341;
            table[4, 1] = 63.439863;
            table[5, 1] = 59.325581;
            table[6, 1] = 55.804047;
            table[7, 1] = 52.734124;
            table[8, 1] = 50.019155;
            table[9, 1] = 47.590482;
            table[10, 1] = 45.397781;
            table[11, 1] = 43.403143;
            table[12, 1] = 41.577332;
            table[13, 1] = 39.89732;
            table[14, 1] = 38.344639;
            table[15, 1] = 36.904233;
            table[16, 1] = 35.563649;
            table[17, 1] = 34.312455;
            table[18, 1] = 33.141809;
            table[19, 1] = 32.044139;
            table[20, 1] = 27.447951;
            table[21, 1] = 23.965533;
            table[22, 1] = 21.259196;
            table[23, 1] = 19.114147;
            table[24, 1] = 17.38463;
            table[25, 1] = 15.967468;
            table[26, 1] = 14.787378;
            table[27, 1] = 13.78814;
            table[28, 1] = 12.92701;
            table[29, 1] = 12.171058;
            table[30, 1] = 11.494686;
            table[31, 1] = 10.8779;
            table[32, 1] = 10.305049;
            table[33, 1] = 9.763891;
            table[34, 1] = 9.244877;
            table[35, 1] = 8.740589;
            table[36, 1] = 7.754639;
            table[37, 1] = 6.774771;
            table[38, 1] = 5.78359;
            table[39, 1] = 4.772349;
            table[40, 1] = 3.737613;
            table[41, 1] = 2.679147;
            table[42, 1] = 1.598582;
            table[43, 1] = 0.498579;
            table[44, 1] = -0.617698;
            table[45, 1] = -1.746905;
            table[46, 1] = -4.605086;
            table[47, 1] = -7.477212;
            table[48, 1] = -10.32685;
            table[49, 1] = -13.12828;
            table[50, 1] = -15.865184;
            table[51, 1] = -18.528831;
            table[52, 1] = -21.116307;
            table[53, 1] = -23.629038;
            table[54, 1] = -26.071603;
            table[55, 1] = -28.45082;
            table[56, 1] = -30.77504;
            table[57, 1] = -33.053585;
            table[58, 1] = -35.296293;
            table[59, 1] = -37.513122;
            table[60, 1] = -39.713792;
            table[61, 1] = -41.907443;
            table[62, 1] = -44.102295;
            table[63, 1] = -46.30531;
            table[64, 1] = -48.521848;
            table[65, 1] = -50.755343;
            table[66, 1] = -53.006987;
            table[67, 1] = -55.275476;
            table[68, 1] = -57.556811;
            table[69, 1] = -59.844206;
            table[70, 1] = -62.128127;
            table[71, 1] = -64.39648;
            table[72, 1] = -66.634981;
            table[73, 1] = -68.827709;
            table[74, 1] = -70.957828;
            table[75, 1] = -73.008459;
            table[76, 1] = -74.963642;
            table[77, 1] = -76.809316;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_10t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 96.509153;
            table[1, 1] = 85.910188;
            table[2, 1] = 79.13464;
            table[3, 1] = 73.847196;
            table[4, 1] = 69.411974;
            table[5, 1] = 65.580198;
            table[6, 1] = 62.215867;
            table[7, 1] = 59.226709;
            table[8, 1] = 56.544263;
            table[9, 1] = 54.148848;
            table[10, 1] = 51.989176;
            table[11, 1] = 50.014755;
            table[12, 1] = 48.199632;
            table[13, 1] = 46.522869;
            table[14, 1] = 44.967346;
            table[15, 1] = 43.518892;
            table[16, 1] = 42.165639;
            table[17, 1] = 40.897539;
            table[18, 1] = 39.70601;
            table[19, 1] = 38.583649;
            table[20, 1] = 33.809894;
            table[21, 1] = 30.064249;
            table[22, 1] = 27.017542;
            table[23, 1] = 24.470306;
            table[24, 1] = 22.297936;
            table[25, 1] = 20.420556;
            table[26, 1] = 18.784411;
            table[27, 1] = 17.350305;
            table[28, 1] = 16.086948;
            table[29, 1] = 14.967596;
            table[30, 1] = 13.968631;
            table[31, 1] = 13.069106;
            table[32, 1] = 12.250632;
            table[33, 1] = 11.497322;
            table[34, 1] = 10.795676;
            table[35, 1] = 10.1344;
            table[36, 1] = 8.897376;
            table[37, 1] = 7.730779;
            table[38, 1] = 6.59908;
            table[39, 1] = 5.480472;
            table[40, 1] = 4.362298;
            table[41, 1] = 3.237859;
            table[42, 1] = 2.104268;
            table[43, 1] = 0.961007;
            table[44, 1] = -0.191005;
            table[45, 1] = -1.35004;
            table[46, 1] = -4.263991;
            table[47, 1] = -7.173519;
            table[48, 1] = -10.048756;
            table[49, 1] = -12.867983;
            table[50, 1] = -15.617458;
            table[51, 1] = -18.290139;
            table[52, 1] = -20.884228;
            table[53, 1] = -23.40189;
            table[54, 1] = -25.848198;
            table[55, 1] = -28.230306;
            table[56, 1] = -30.556794;
            table[57, 1] = -32.837144;
            table[58, 1] = -35.081306;
            table[59, 1] = -37.299321;
            table[60, 1] = -39.500969;
            table[61, 1] = -41.695434;
            table[62, 1] = -43.890968;
            table[63, 1] = -46.09456;
            table[64, 1] = -48.311591;
            table[65, 1] = -50.545507;
            table[66, 1] = -52.797515;
            table[67, 1] = -55.066319;
            table[68, 1] = -57.347929;
            table[69, 1] = -59.635565;
            table[70, 1] = -61.919698;
            table[71, 1] = -64.188238;
            table[72, 1] = -66.426905;
            table[73, 1] = -68.61978;
            table[74, 1] = -70.750031;
            table[75, 1] = -72.80078;
            table[76, 1] = -74.756069;
            table[77, 1] = -76.601839;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_10t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 98.661743;
            table[1, 1] = 88.761896;
            table[2, 1] = 82.671395;
            table[3, 1] = 78.077502;
            table[4, 1] = 74.253131;
            table[5, 1] = 70.907659;
            table[6, 1] = 67.908503;
            table[7, 1] = 65.185886;
            table[8, 1] = 62.695912;
            table[9, 1] = 60.406379;
            table[10, 1] = 58.291431;
            table[11, 1] = 56.329456;
            table[12, 1] = 54.502111;
            table[13, 1] = 52.815603;
            table[14, 1] = 51.276397;
            table[15, 1] = 49.836065;
            table[16, 1] = 48.484153;
            table[17, 1] = 47.211618;
            table[18, 1] = 46.010585;
            table[19, 1] = 44.874167;
            table[20, 1] = 39.97194;
            table[21, 1] = 36.014143;
            table[22, 1] = 32.683353;
            table[23, 1] = 29.793736;
            table[24, 1] = 27.238414;
            table[25, 1] = 24.957356;
            table[26, 1] = 22.915788;
            table[27, 1] = 21.090204;
            table[28, 1] = 19.460513;
            table[29, 1] = 18.006646;
            table[30, 1] = 16.707845;
            table[31, 1] = 15.54322;
            table[32, 1] = 14.492617;
            table[33, 1] = 13.537368;
            table[34, 1] = 12.660751;
            table[35, 1] = 11.848185;
            table[36, 1] = 10.367384;
            table[37, 1] = 9.018006;
            table[38, 1] = 7.748045;
            table[39, 1] = 6.52363;
            table[40, 1] = 5.323438;
            table[41, 1] = 4.134588;
            table[42, 1] = 2.949767;
            table[43, 1] = 1.76528;
            table[44, 1] = 0.579733;
            table[45, 1] = -0.606843;
            table[46, 1] = -3.570981;
            table[47, 1] = -6.51317;
            table[48, 1] = -9.4104;
            table[49, 1] = -12.244861;
            table[50, 1] = -15.005158;
            table[51, 1] = -17.685705;
            table[52, 1] = -20.285634;
            table[53, 1] = -22.807715;
            table[54, 1] = -25.257426;
            table[55, 1] = -27.642197;
            table[56, 1] = -29.970797;
            table[57, 1] = -32.252844;
            table[58, 1] = -34.498386;
            table[59, 1] = -36.717534;
            table[60, 1] = -38.920121;
            table[61, 1] = -41.115371;
            table[62, 1] = -43.311567;
            table[63, 1] = -45.51572;
            table[64, 1] = -47.733229;
            table[65, 1] = -49.967557;
            table[66, 1] = -52.219921;
            table[67, 1] = -54.489034;
            table[68, 1] = -56.770913;
            table[69, 1] = -59.058786;
            table[70, 1] = -61.343126;
            table[71, 1] = -63.61185;
            table[72, 1] = -65.85068;
            table[73, 1] = -68.0437;
            table[74, 1] = -70.17408;
            table[75, 1] = -72.224945;
            table[76, 1] = -74.180338;
            table[77, 1] = -76.026201;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_10t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 101.148112;
            table[1, 1] = 92.000284;
            table[2, 1] = 86.423295;
            table[3, 1] = 82.309886;
            table[4, 1] = 79.006092;
            table[5, 1] = 76.172083;
            table[6, 1] = 73.642723;
            table[7, 1] = 71.32897;
            table[8, 1] = 69.180884;
            table[9, 1] = 67.169448;
            table[10, 1] = 65.276613;
            table[11, 1] = 63.489928;
            table[12, 1] = 61.799759;
            table[13, 1] = 60.197947;
            table[14, 1] = 58.677196;
            table[15, 1] = 57.23082;
            table[16, 1] = 55.85264;
            table[17, 1] = 54.556633;
            table[18, 1] = 53.367391;
            table[19, 1] = 52.235099;
            table[20, 1] = 47.266457;
            table[21, 1] = 43.133857;
            table[22, 1] = 39.54355;
            table[23, 1] = 36.325285;
            table[24, 1] = 33.389172;
            table[25, 1] = 30.694466;
            table[26, 1] = 28.226022;
            table[27, 1] = 25.977904;
            table[28, 1] = 23.943849;
            table[29, 1] = 22.113253;
            table[30, 1] = 20.470728;
            table[31, 1] = 18.997428;
            table[32, 1] = 17.672849;
            table[33, 1] = 16.476434;
            table[34, 1] = 15.388718;
            table[35, 1] = 14.392011;
            table[36, 1] = 12.611345;
            table[37, 1] = 11.034608;
            table[38, 1] = 9.591323;
            table[39, 1] = 8.23362;
            table[40, 1] = 6.929868;
            table[41, 1] = 5.659677;
            table[42, 1] = 4.410275;
            table[43, 1] = 3.173978;
            table[44, 1] = 1.946457;
            table[45, 1] = 0.725563;
            table[46, 1] = -2.300658;
            table[47, 1] = -5.282913;
            table[48, 1] = -8.207021;
            table[49, 1] = -11.060115;
            table[50, 1] = -13.833698;
            table[51, 1] = -16.523954;
            table[52, 1] = -19.131133;
            table[53, 1] = -21.658734;
            table[54, 1] = -24.112719;
            table[55, 1] = -26.500848;
            table[56, 1] = -28.832123;
            table[57, 1] = -31.116327;
            table[58, 1] = -33.363627;
            table[59, 1] = -35.584222;
            table[60, 1] = -37.78801;
            table[61, 1] = -39.984265;
            table[62, 1] = -42.18131;
            table[63, 1] = -44.386183;
            table[64, 1] = -46.604308;
            table[65, 1] = -48.839165;
            table[66, 1] = -51.091987;
            table[67, 1] = -53.361497;
            table[68, 1] = -55.643723;
            table[69, 1] = -57.9319;
            table[70, 1] = -60.216508;
            table[71, 1] = -62.485468;
            table[72, 1] = -64.724508;
            table[73, 1] = -66.917715;
            table[74, 1] = -69.048261;
            table[75, 1] = -71.099275;
            table[76, 1] = -73.054803;
            table[77, 1] = -74.900787;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_10t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 103.509076;
            table[1, 1] = 95.276218;
            table[2, 1] = 90.218886;
            table[3, 1] = 86.521905;
            table[4, 1] = 83.568829;
            table[5, 1] = 81.072152;
            table[6, 1] = 78.911591;
            table[7, 1] = 76.969512;
            table[8, 1] = 75.181216;
            table[9, 1] = 73.507135;
            table[10, 1] = 71.921585;
            table[11, 1] = 70.408016;
            table[12, 1] = 68.95583;
            table[13, 1] = 67.558177;
            table[14, 1] = 66.210461;
            table[15, 1] = 64.90935;
            table[16, 1] = 63.652175;
            table[17, 1] = 62.436573;
            table[18, 1] = 61.260298;
            table[19, 1] = 60.121141;
            table[20, 1] = 55.120946;
            table[21, 1] = 50.932811;
            table[22, 1] = 47.190557;
            table[23, 1] = 43.74305;
            table[24, 1] = 40.513615;
            table[25, 1] = 37.475661;
            table[26, 1] = 34.630149;
            table[27, 1] = 31.987901;
            table[28, 1] = 29.558184;
            table[29, 1] = 27.343214;
            table[30, 1] = 25.337008;
            table[31, 1] = 23.526697;
            table[32, 1] = 21.894817;
            table[33, 1] = 20.421582;
            table[34, 1] = 19.086726;
            table[35, 1] = 17.87077;
            table[36, 1] = 15.725791;
            table[37, 1] = 13.866869;
            table[38, 1] = 12.205071;
            table[39, 1] = 10.677396;
            table[40, 1] = 9.240483;
            table[41, 1] = 7.865082;
            table[42, 1] = 6.531804;
            table[43, 1] = 5.22803;
            table[44, 1] = 3.945742;
            table[45, 1] = 2.680019;
            table[46, 1] = -0.42739;
            table[47, 1] = -3.462102;
            table[48, 1] = -6.421453;
            table[49, 1] = -9.299026;
            table[50, 1] = -12.090107;
            table[51, 1] = -14.793183;
            table[52, 1] = -17.409959;
            table[53, 1] = -19.94488;
            table[54, 1] = -22.404543;
            table[55, 1] = -24.797142;
            table[56, 1] = -27.131982;
            table[57, 1] = -29.419063;
            table[58, 1] = -31.668709;
            table[59, 1] = -33.891237;
            table[60, 1] = -36.096631;
            table[61, 1] = -38.294231;
            table[62, 1] = -40.49241;
            table[63, 1] = -42.698247;
            table[64, 1] = -44.917197;
            table[65, 1] = -47.152762;
            table[66, 1] = -49.406196;
            table[67, 1] = -51.676239;
            table[68, 1] = -53.958929;
            table[69, 1] = -56.247512;
            table[70, 1] = -58.532479;
            table[71, 1] = -60.801756;
            table[72, 1] = -63.041077;
            table[73, 1] = -65.234534;
            table[74, 1] = -67.365303;
            table[75, 1] = -69.416518;
            table[76, 1] = -71.372224;
            table[77, 1] = -73.21837;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_10t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 105.319204;
            table[1, 1] = 98.138163;
            table[2, 1] = 93.71803;
            table[3, 1] = 90.480833;
            table[4, 1] = 87.90588;
            table[5, 1] = 85.750529;
            table[6, 1] = 83.880028;
            table[7, 1] = 82.211187;
            table[8, 1] = 80.706622;
            table[9, 1] = 79.330825;
            table[10, 1] = 78.042722;
            table[11, 1] = 76.821674;
            table[12, 1] = 75.652503;
            table[13, 1] = 74.524147;
            table[14, 1] = 73.428685;
            table[15, 1] = 72.360577;
            table[16, 1] = 71.316054;
            table[17, 1] = 70.292616;
            table[18, 1] = 69.288628;
            table[19, 1] = 68.303013;
            table[20, 1] = 63.627786;
            table[21, 1] = 59.47501;
            table[22, 1] = 55.783558;
            table[23, 1] = 52.298278;
            table[24, 1] = 48.952379;
            table[25, 1] = 45.724723;
            table[26, 1] = 42.624025;
            table[27, 1] = 39.672919;
            table[28, 1] = 36.895618;
            table[29, 1] = 34.310362;
            table[30, 1] = 31.926208;
            table[31, 1] = 29.742909;
            table[32, 1] = 27.752543;
            table[33, 1] = 25.941792;
            table[34, 1] = 24.294169;
            table[35, 1] = 22.791863;
            table[36, 1] = 20.152893;
            table[37, 1] = 17.895966;
            table[38, 1] = 15.916874;
            table[39, 1] = 14.136961;
            table[40, 1] = 12.499156;
            table[41, 1] = 10.963019;
            table[42, 1] = 9.500339;
            table[43, 1] = 8.091665;
            table[44, 1] = 6.72372;
            table[45, 1] = 5.387542;
            table[46, 1] = 2.151761;
            table[47, 1] = -0.966427;
            table[48, 1] = -3.982108;
            table[49, 1] = -6.898935;
            table[50, 1] = -9.718141;
            table[51, 1] = -12.441862;
            table[52, 1] = -15.074116;
            table[53, 1] = -17.620858;
            table[54, 1] = -20.089699;
            table[55, 1] = -22.489526;
            table[56, 1] = -24.830136;
            table[57, 1] = -27.121875;
            table[58, 1] = -29.375323;
            table[59, 1] = -31.600982;
            table[60, 1] = -33.808979;
            table[61, 1] = -36.008759;
            table[62, 1] = -38.208779;
            table[63, 1] = -40.416179;
            table[64, 1] = -42.636465;
            table[65, 1] = -44.87318;
            table[66, 1] = -47.127607;
            table[67, 1] = -49.398513;
            table[68, 1] = -51.681957;
            table[69, 1] = -53.971201;
            table[70, 1] = -56.256749;
            table[71, 1] = -58.52654;
            table[72, 1] = -60.766317;
            table[73, 1] = -62.960179;
            table[74, 1] = -65.091311;
            table[75, 1] = -67.14285;
            table[76, 1] = -69.098848;
            table[77, 1] = -70.945256;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_10t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.327932;
            table[1, 1] = 99.925825;
            table[2, 1] = 96.089284;
            table[3, 1] = 93.31663;
            table[4, 1] = 91.132119;
            table[5, 1] = 89.321126;
            table[6, 1] = 87.767355;
            table[7, 1] = 86.400041;
            table[8, 1] = 85.172505;
            table[9, 1] = 84.052091;
            table[10, 1] = 83.014958;
            table[11, 1] = 82.061153;
            table[12, 1] = 81.165039;
            table[13, 1] = 80.313345;
            table[14, 1] = 79.496956;
            table[15, 1] = 78.708446;
            table[16, 1] = 77.941764;
            table[17, 1] = 77.192013;
            table[18, 1] = 76.455274;
            table[19, 1] = 75.728468;
            table[20, 1] = 72.179414;
            table[21, 1] = 68.706284;
            table[22, 1] = 65.290549;
            table[23, 1] = 62.124518;
            table[24, 1] = 59.02105;
            table[25, 1] = 55.936031;
            table[26, 1] = 52.874292;
            table[27, 1] = 49.858429;
            table[28, 1] = 46.919273;
            table[29, 1] = 44.088045;
            table[30, 1] = 41.391122;
            table[31, 1] = 38.847332;
            table[32, 1] = 36.46725;
            table[33, 1] = 34.253849;
            table[34, 1] = 32.203887;
            table[35, 1] = 30.309557;
            table[36, 1] = 26.942956;
            table[37, 1] = 24.053784;
            table[38, 1] = 21.541817;
            table[39, 1] = 19.320226;
            table[40, 1] = 17.319266;
            table[41, 1] = 15.485316;
            table[42, 1] = 13.778209;
            table[43, 1] = 12.168333;
            table[44, 1] = 10.634067;
            table[45, 1] = 9.159718;
            table[46, 1] = 5.669013;
            table[47, 1] = 2.382851;
            table[48, 1] = -0.747164;
            table[49, 1] = -3.744135;
            table[50, 1] = -6.621007;
            table[51, 1] = -9.387187;
            table[52, 1] = -12.051345;
            table[53, 1] = -14.622495;
            table[54, 1] = -17.110309;
            table[55, 1] = -19.525099;
            table[56, 1] = -21.877659;
            table[57, 1] = -24.179053;
            table[58, 1] = -26.440384;
            table[59, 1] = -28.67254;
            table[60, 1] = -30.885939;
            table[61, 1] = -33.090246;
            table[62, 1] = -35.294086;
            table[63, 1] = -37.504733;
            table[64, 1] = -39.727795;
            table[65, 1] = -41.966898;
            table[66, 1] = -44.223389;
            table[67, 1] = -46.496089;
            table[68, 1] = -48.781099;
            table[69, 1] = -51.071716;
            table[70, 1] = -53.358474;
            table[71, 1] = -55.629333;
            table[72, 1] = -57.870057;
            table[73, 1] = -60.064763;
            table[74, 1] = -62.196649;
            table[75, 1] = -64.248862;
            table[76, 1] = -66.205466;
            table[77, 1] = -68.05242;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_10t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.731901;
            table[1, 1] = 100.634998;
            table[2, 1] = 97.053947;
            table[3, 1] = 94.504919;
            table[4, 1] = 92.522127;
            table[5, 1] = 90.897744;
            table[6, 1] = 89.520675;
            table[7, 1] = 88.324419;
            table[8, 1] = 87.265919;
            table[9, 1] = 86.31563;
            table[10, 1] = 85.452346;
            table[11, 1] = 84.660291;
            table[12, 1] = 83.92738;
            table[13, 1] = 83.244131;
            table[14, 1] = 82.607779;
            table[15, 1] = 82.009071;
            table[16, 1] = 81.442221;
            table[17, 1] = 80.902902;
            table[18, 1] = 80.387422;
            table[19, 1] = 79.892596;
            table[20, 1] = 77.642045;
            table[21, 1] = 75.603555;
            table[22, 1] = 73.632669;
            table[23, 1] = 71.640678;
            table[24, 1] = 69.570735;
            table[25, 1] = 67.447081;
            table[26, 1] = 65.223453;
            table[27, 1] = 62.86859;
            table[28, 1] = 60.403037;
            table[29, 1] = 57.861775;
            table[30, 1] = 55.284584;
            table[31, 1] = 52.709416;
            table[32, 1] = 50.169083;
            table[33, 1] = 47.690106;
            table[34, 1] = 45.292662;
            table[35, 1] = 42.991009;
            table[36, 1] = 38.70665;
            table[37, 1] = 34.860517;
            table[38, 1] = 31.43053;
            table[39, 1] = 28.371082;
            table[40, 1] = 25.628571;
            table[41, 1] = 23.150589;
            table[42, 1] = 20.890391;
            table[43, 1] = 18.808436;
            table[44, 1] = 16.872343;
            table[45, 1] = 15.056116;
            table[46, 1] = 10.915202;
            table[47, 1] = 7.186635;
            table[48, 1] = 3.749056;
            table[49, 1] = 0.533351;
            table[50, 1] = -2.502513;
            table[51, 1] = -5.386623;
            table[52, 1] = -8.13988;
            table[53, 1] = -10.779476;
            table[54, 1] = -13.320668;
            table[55, 1] = -15.777653;
            table[56, 1] = -18.163981;
            table[57, 1] = -20.492701;
            table[58, 1] = -22.776371;
            table[59, 1] = -25.026958;
            table[60, 1] = -27.255693;
            table[61, 1] = -29.472861;
            table[62, 1] = -31.687563;
            table[63, 1] = -33.907447;
            table[64, 1] = -36.13841;
            table[65, 1] = -38.384309;
            table[66, 1] = -40.64668;
            table[67, 1] = -42.924489;
            table[68, 1] = -45.213961;
            table[69, 1] = -47.508492;
            table[70, 1] = -49.798695;
            table[71, 1] = -52.0726;
            table[72, 1] = -54.316027;
            table[73, 1] = -56.513138;
            table[74, 1] = -58.647171;
            table[75, 1] = -60.701309;
            table[76, 1] = -62.659641;
            table[77, 1] = -64.508152;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_1t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 94.233474;
            table[1, 1] = 82.71091;
            table[2, 1] = 75.466077;
            table[3, 1] = 70.026656;
            table[4, 1] = 65.656674;
            table[5, 1] = 62.005913;
            table[6, 1] = 58.87436;
            table[7, 1] = 56.135739;
            table[8, 1] = 53.70501;
            table[9, 1] = 51.522177;
            table[10, 1] = 49.543363;
            table[11, 1] = 47.735527;
            table[12, 1] = 46.073185;
            table[13, 1] = 44.536266;
            table[14, 1] = 43.108683;
            table[15, 1] = 41.777337;
            table[16, 1] = 40.531417;
            table[17, 1] = 39.36189;
            table[18, 1] = 38.261124;
            table[19, 1] = 37.222608;
            table[20, 1] = 32.791024;
            table[21, 1] = 29.313286;
            table[22, 1] = 26.509106;
            table[23, 1] = 24.202611;
            table[24, 1] = 22.274844;
            table[25, 1] = 20.641123;
            table[26, 1] = 19.238959;
            table[27, 1] = 18.021031;
            table[28, 1] = 16.950799;
            table[29, 1] = 15.999636;
            table[30, 1] = 15.144853;
            table[31, 1] = 14.368308;
            table[32, 1] = 13.655391;
            table[33, 1] = 12.994273;
            table[34, 1] = 12.375335;
            table[35, 1] = 11.790729;
            table[36, 1] = 10.7;
            table[37, 1] = 9.683425;
            table[38, 1] = 8.715008;
            table[39, 1] = 7.777301;
            table[40, 1] = 6.858666;
            table[41, 1] = 5.951436;
            table[42, 1] = 5.050666;
            table[43, 1] = 4.153268;
            table[44, 1] = 3.257423;
            table[45, 1] = 2.362173;
            table[46, 1] = 0.12504;
            table[47, 1] = -2.10791;
            table[48, 1] = -4.329879;
            table[49, 1] = -6.533949;
            table[50, 1] = -8.714992;
            table[51, 1] = -10.870243;
            table[52, 1] = -12.999271;
            table[53, 1] = -15.103679;
            table[54, 1] = -17.186715;
            table[55, 1] = -19.252862;
            table[56, 1] = -21.307449;
            table[57, 1] = -23.356295;
            table[58, 1] = -25.405385;
            table[59, 1] = -27.46057;
            table[60, 1] = -29.527294;
            table[61, 1] = -31.610338;
            table[62, 1] = -33.713576;
            table[63, 1] = -35.839759;
            table[64, 1] = -37.990312;
            table[65, 1] = -40.165164;
            table[66, 1] = -42.362606;
            table[67, 1] = -44.5792;
            table[68, 1] = -46.809737;
            table[69, 1] = -49.047261;
            table[70, 1] = -51.283159;
            table[71, 1] = -53.507333;
            table[72, 1] = -55.708452;
            table[73, 1] = -57.874273;
            table[74, 1] = -59.992041;
            table[75, 1] = -62.04894;
            table[76, 1] = -64.032579;
            table[77, 1] = -65.931489;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_1t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 96.509153;
            table[1, 1] = 86.063313;
            table[2, 1] = 79.573138;
            table[3, 1] = 74.676206;
            table[4, 1] = 70.683285;
            table[5, 1] = 67.293706;
            table[6, 1] = 64.344715;
            table[7, 1] = 61.735208;
            table[8, 1] = 59.396635;
            table[9, 1] = 57.279803;
            table[10, 1] = 55.347998;
            table[11, 1] = 53.573019;
            table[12, 1] = 51.932712;
            table[13, 1] = 50.409347;
            table[14, 1] = 48.988511;
            table[15, 1] = 47.658328;
            table[16, 1] = 46.408894;
            table[17, 1] = 45.231859;
            table[18, 1] = 44.120117;
            table[19, 1] = 43.067568;
            table[20, 1] = 38.52945;
            table[21, 1] = 34.899573;
            table[22, 1] = 31.910126;
            table[23, 1] = 29.393841;
            table[24, 1] = 27.23895;
            table[25, 1] = 25.367136;
            table[26, 1] = 23.721526;
            table[27, 1] = 22.259604;
            table[28, 1] = 20.948795;
            table[29, 1] = 19.763588;
            table[30, 1] = 18.683623;
            table[31, 1] = 17.692385;
            table[32, 1] = 16.776289;
            table[33, 1] = 15.924042;
            table[34, 1] = 15.126171;
            table[35, 1] = 14.374684;
            table[36, 1] = 12.984756;
            table[37, 1] = 11.711254;
            table[38, 1] = 10.5229;
            table[39, 1] = 9.396915;
            table[40, 1] = 8.316743;
            table[41, 1] = 7.270406;
            table[42, 1] = 6.249289;
            table[43, 1] = 5.247248;
            table[44, 1] = 4.259944;
            table[45, 1] = 3.284356;
            table[46, 1] = 0.884717;
            table[47, 1] = -1.470028;
            table[48, 1] = -3.785016;
            table[49, 1] = -6.06115;
            table[50, 1] = -8.298679;
            table[51, 1] = -10.498669;
            table[52, 1] = -12.663488;
            table[53, 1] = -14.796812;
            table[54, 1] = -16.903436;
            table[55, 1] = -18.989009;
            table[56, 1] = -21.059742;
            table[57, 1] = -23.122128;
            table[58, 1] = -25.182667;
            table[59, 1] = -27.247611;
            table[60, 1] = -29.322716;
            table[61, 1] = -31.413007;
            table[62, 1] = -33.522554;
            table[63, 1] = -35.654262;
            table[64, 1] = -37.809681;
            table[65, 1] = -39.98884;
            table[66, 1] = -42.190113;
            table[67, 1] = -44.41013;
            table[68, 1] = -46.643738;
            table[69, 1] = -48.884026;
            table[70, 1] = -51.122422;
            table[71, 1] = -53.348861;
            table[72, 1] = -55.552038;
            table[73, 1] = -57.719736;
            table[74, 1] = -59.839219;
            table[75, 1] = -61.89769;
            table[76, 1] = -63.882772;
            table[77, 1] = -65.783009;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_1t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 98.661743;
            table[1, 1] = 88.942512;
            table[2, 1] = 82.995884;
            table[3, 1] = 78.565169;
            table[4, 1] = 74.957393;
            table[5, 1] = 71.875268;
            table[6, 1] = 69.166643;
            table[7, 1] = 66.743037;
            table[8, 1] = 64.547542;
            table[9, 1] = 62.540559;
            table[10, 1] = 60.69288;
            table[11, 1] = 58.982021;
            table[12, 1] = 57.390137;
            table[13, 1] = 55.902742;
            table[14, 1] = 54.50787;
            table[15, 1] = 53.195502;
            table[16, 1] = 51.957153;
            table[17, 1] = 50.785572;
            table[18, 1] = 49.674508;
            table[19, 1] = 48.618531;
            table[20, 1] = 44.017244;
            table[21, 1] = 40.271113;
            table[22, 1] = 37.129999;
            table[23, 1] = 34.436805;
            table[24, 1] = 32.08716;
            table[25, 1] = 30.008829;
            table[26, 1] = 28.150132;
            table[27, 1] = 26.472967;
            table[28, 1] = 24.948412;
            table[29, 1] = 23.553885;
            table[30, 1] = 22.271266;
            table[31, 1] = 21.085663;
            table[32, 1] = 19.984585;
            table[33, 1] = 18.957388;
            table[34, 1] = 17.994892;
            table[35, 1] = 17.089121;
            table[36, 1] = 15.42074;
            table[37, 1] = 13.906212;
            table[38, 1] = 12.510152;
            table[39, 1] = 11.205432;
            table[40, 1] = 9.97136;
            table[41, 1] = 8.792236;
            table[42, 1] = 7.656216;
            table[43, 1] = 6.554401;
            table[44, 1] = 5.480137;
            table[45, 1] = 4.428468;
            table[46, 1] = 1.876269;
            table[47, 1] = -0.591186;
            table[48, 1] = -2.991127;
            table[49, 1] = -5.332434;
            table[50, 1] = -7.620758;
            table[51, 1] = -9.860908;
            table[52, 1] = -12.057899;
            table[53, 1] = -14.217311;
            table[54, 1] = -16.345328;
            table[55, 1] = -18.448627;
            table[56, 1] = -20.534189;
            table[57, 1] = -22.609089;
            table[58, 1] = -24.680276;
            table[59, 1] = -26.754348;
            table[60, 1] = -28.837332;
            table[61, 1] = -30.934467;
            table[62, 1] = -33.049994;
            table[63, 1] = -35.186956;
            table[64, 1] = -37.347015;
            table[65, 1] = -39.530289;
            table[66, 1] = -41.735229;
            table[67, 1] = -43.958526;
            table[68, 1] = -46.195079;
            table[69, 1] = -48.438021;
            table[70, 1] = -50.678816;
            table[71, 1] = -52.90743;
            table[72, 1] = -55.112584;
            table[73, 1] = -57.282085;
            table[74, 1] = -59.403217;
            table[75, 1] = -61.463198;
            table[76, 1] = -63.449667;
            table[77, 1] = -65.351182;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_1t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 101.148112;
            table[1, 1] = 92.187082;
            table[2, 1] = 86.732386;
            table[3, 1] = 82.725883;
            table[4, 1] = 79.500687;
            table[5, 1] = 76.759962;
            table[6, 1] = 74.349309;
            table[7, 1] = 72.180355;
            table[8, 1] = 70.198836;
            table[9, 1] = 68.369354;
            table[10, 1] = 66.667485;
            table[11, 1] = 65.075459;
            table[12, 1] = 63.579723;
            table[13, 1] = 62.169518;
            table[14, 1] = 60.836023;
            table[15, 1] = 59.571825;
            table[16, 1] = 58.370577;
            table[17, 1] = 57.226772;
            table[18, 1] = 56.135575;
            table[19, 1] = 55.092716;
            table[20, 1] = 50.483417;
            table[21, 1] = 46.649439;
            table[22, 1] = 43.371522;
            table[23, 1] = 40.508249;
            table[24, 1] = 37.965099;
            table[25, 1] = 35.677128;
            table[26, 1] = 33.598563;
            table[27, 1] = 31.696247;
            table[28, 1] = 29.945392;
            table[29, 1] = 28.326798;
            table[30, 1] = 26.825035;
            table[31, 1] = 25.427266;
            table[32, 1] = 24.12249;
            table[33, 1] = 22.901072;
            table[34, 1] = 21.754437;
            table[35, 1] = 20.674888;
            table[36, 1] = 18.689954;
            table[37, 1] = 16.898439;
            table[38, 1] = 15.261535;
            table[39, 1] = 13.748053;
            table[40, 1] = 12.333186;
            table[41, 1] = 10.997387;
            table[42, 1] = 9.725355;
            table[43, 1] = 8.505179;
            table[44, 1] = 7.327615;
            table[45, 1] = 6.185506;
            table[46, 1] = 3.451946;
            table[47, 1] = 0.850636;
            table[48, 1] = -1.650093;
            table[49, 1] = -4.068706;
            table[50, 1] = -6.417331;
            table[51, 1] = -8.705245;
            table[52, 1] = -10.940597;
            table[53, 1] = -13.131208;
            table[54, 1] = -15.284891;
            table[55, 1] = -17.409524;
            table[56, 1] = -19.512987;
            table[57, 1] = -21.603038;
            table[58, 1] = -23.687149;
            table[59, 1] = -25.772324;
            table[60, 1] = -27.864912;
            table[61, 1] = -29.970403;
            table[62, 1] = -32.093242;
            table[63, 1] = -34.236636;
            table[64, 1] = -36.402379;
            table[65, 1] = -38.590701;
            table[66, 1] = -40.80014;
            table[67, 1] = -43.027463;
            table[68, 1] = -45.267633;
            table[69, 1] = -47.513835;
            table[70, 1] = -49.757577;
            table[71, 1] = -51.988864;
            table[72, 1] = -54.19645;
            table[73, 1] = -56.368168;
            table[74, 1] = -58.491326;
            table[75, 1] = -60.553164;
            table[76, 1] = -62.541338;
            table[77, 1] = -64.444423;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_1t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 103.509076;
            table[1, 1] = 95.444577;
            table[2, 1] = 90.502226;
            table[3, 1] = 86.887573;
            table[4, 1] = 84.002888;
            table[5, 1] = 81.57343;
            table[6, 1] = 79.450892;
            table[7, 1] = 77.547286;
            table[8, 1] = 75.807214;
            table[9, 1] = 74.194378;
            table[10, 1] = 72.684281;
            table[11, 1] = 71.259944;
            table[12, 1] = 69.909273;
            table[13, 1] = 68.623387;
            table[14, 1] = 67.39553;
            table[15, 1] = 66.220377;
            table[16, 1] = 65.093573;
            table[17, 1] = 64.011441;
            table[18, 1] = 62.97079;
            table[19, 1] = 61.968792;
            table[20, 1] = 57.456773;
            table[21, 1] = 53.606099;
            table[22, 1] = 50.245854;
            table[23, 1] = 47.258013;
            table[24, 1] = 44.56095;
            table[25, 1] = 42.098026;
            table[26, 1] = 39.829587;
            table[27, 1] = 37.727401;
            table[28, 1] = 35.770809;
            table[29, 1] = 33.944104;
            table[30, 1] = 32.234782;
            table[31, 1] = 30.632405;
            table[32, 1] = 29.127886;
            table[33, 1] = 27.713055;
            table[34, 1] = 26.380413;
            table[35, 1] = 25.123;
            table[36, 1] = 22.808328;
            table[37, 1] = 20.722291;
            table[38, 1] = 18.824809;
            table[39, 1] = 17.082133;
            table[40, 1] = 15.46633;
            table[41, 1] = 13.954594;
            table[42, 1] = 12.528502;
            table[43, 1] = 11.173278;
            table[44, 1] = 9.877122;
            table[45, 1] = 8.630639;
            table[46, 1] = 5.686286;
            table[47, 1] = 2.928141;
            table[48, 1] = 0.308607;
            table[49, 1] = -2.201574;
            table[50, 1] = -4.621921;
            table[51, 1] = -6.966849;
            table[52, 1] = -9.248137;
            table[53, 1] = -11.476213;
            table[54, 1] = -13.660794;
            table[55, 1] = -15.811166;
            table[56, 1] = -17.936267;
            table[57, 1] = -20.044662;
            table[58, 1] = -22.144444;
            table[59, 1] = -24.2431;
            table[60, 1] = -26.347358;
            table[61, 1] = -28.463015;
            table[62, 1] = -30.594756;
            table[63, 1] = -32.745985;
            table[64, 1] = -34.918657;
            table[65, 1] = -37.113132;
            table[66, 1] = -39.32806;
            table[67, 1] = -41.560298;
            table[68, 1] = -43.804883;
            table[69, 1] = -46.055065;
            table[70, 1] = -48.302407;
            table[71, 1] = -50.536959;
            table[72, 1] = -52.747514;
            table[73, 1] = -54.92194;
            table[74, 1] = -57.047575;
            table[75, 1] = -59.111683;
            table[76, 1] = -61.101942;
            table[77, 1] = -63.006945;



            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_1t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 105.319204;
            table[1, 1] = 98.250506;
            table[2, 1] = 93.92538;
            table[3, 1] = 90.763047;
            table[4, 1] = 88.247586;
            table[5, 1] = 86.14137;
            table[6, 1] = 84.31411;
            table[7, 1] = 82.686399;
            table[8, 1] = 81.206401;
            table[9, 1] = 79.838715;
            table[10, 1] = 78.558465;
            table[11, 1] = 77.34787;
            table[12, 1] = 76.194105;
            table[13, 1] = 75.08788;
            table[14, 1] = 74.022448;
            table[15, 1] = 72.992888;
            table[16, 1] = 71.995581;
            table[17, 1] = 71.027826;
            table[18, 1] = 70.087553;
            table[19, 1] = 69.173127;
            table[20, 1] = 64.946898;
            table[21, 1] = 61.212041;
            table[22, 1] = 57.870749;
            table[23, 1] = 54.842741;
            table[24, 1] = 52.065833;
            table[25, 1] = 49.494148;
            table[26, 1] = 47.094761;
            table[27, 1] = 44.844265;
            table[28, 1] = 42.725898;
            table[29, 1] = 40.727339;
            table[30, 1] = 38.839117;
            table[31, 1] = 37.053519;
            table[32, 1] = 35.363875;
            table[33, 1] = 33.764119;
            table[34, 1] = 32.248534;
            table[35, 1] = 30.811628;
            table[36, 1] = 28.152735;
            table[37, 1] = 25.746992;
            table[38, 1] = 23.557276;
            table[39, 1] = 21.550446;
            table[40, 1] = 19.697668;
            table[41, 1] = 17.974371;
            table[42, 1] = 16.35994;
            table[43, 1] = 14.837274;
            table[44, 1] = 13.392291;
            table[45, 1] = 12.013447;
            table[46, 1] = 8.797885;
            table[47, 1] = 5.834592;
            table[48, 1] = 3.057746;
            table[49, 1] = 0.42519;
            table[50, 1] = -2.091703;
            table[51, 1] = -4.513817;
            table[52, 1] = -6.857581;
            table[53, 1] = -9.136804;
            table[54, 1] = -11.363696;
            table[55, 1] = -13.549406;
            table[56, 1] = -15.70428;
            table[57, 1] = -17.837961;
            table[58, 1] = -19.959376;
            table[59, 1] = -22.076667;
            table[60, 1] = -24.197078;
            table[61, 1] = -26.326816;
            table[62, 1] = -28.4709;
            table[63, 1] = -30.633;
            table[64, 1] = -32.815292;
            table[65, 1] = -35.018318;
            table[66, 1] = -37.240875;
            table[67, 1] = -39.479945;
            table[68, 1] = -41.730672;
            table[69, 1] = -43.986393;
            table[70, 1] = -46.238746;
            table[71, 1] = -48.477844;
            table[72, 1] = -50.692535;
            table[73, 1] = -52.870735;
            table[74, 1] = -54.99982;
            table[75, 1] = -57.06709;
            table[76, 1] = -59.060255;
            table[77, 1] = -60.967934;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_1t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.327932;
            table[1, 1] = 99.972448;
            table[2, 1] = 96.181655;
            table[3, 1] = 93.450731;
            table[4, 1] = 91.303505;
            table[5, 1] = 89.525621;
            table[6, 1] = 88.001317;
            table[7, 1] = 86.660492;
            table[8, 1] = 85.457186;
            table[9, 1] = 84.359467;
            table[10, 1] = 83.344167;
            table[11, 1] = 82.39393;
            table[12, 1] = 81.495458;
            table[13, 1] = 80.638409;
            table[14, 1] = 79.814683;
            table[15, 1] = 79.017924;
            table[16, 1] = 78.243172;
            table[17, 1] = 77.486592;
            table[18, 1] = 76.745259;
            table[19, 1] = 76.016988;
            table[20, 1] = 72.529735;
            table[21, 1] = 69.26039;
            table[22, 1] = 66.197328;
            table[23, 1] = 63.329194;
            table[24, 1] = 60.636078;
            table[25, 1] = 58.095779;
            table[26, 1] = 55.688499;
            table[27, 1] = 53.398486;
            table[28, 1] = 51.213952;
            table[29, 1] = 49.126308;
            table[30, 1] = 47.12929;
            table[31, 1] = 45.218169;
            table[32, 1] = 43.389143;
            table[33, 1] = 41.638903;
            table[34, 1] = 39.964349;
            table[35, 1] = 38.362413;
            table[36, 1] = 35.363844;
            table[37, 1] = 32.617326;
            table[38, 1] = 30.096277;
            table[39, 1] = 27.774489;
            table[40, 1] = 25.627161;
            table[41, 1] = 23.631607;
            table[42, 1] = 21.767606;
            table[43, 1] = 20.017477;
            table[44, 1] = 18.365971;
            table[45, 1] = 16.800065;
            table[46, 1] = 13.191362;
            table[47, 1] = 9.92202;
            table[48, 1] = 6.905491;
            table[49, 1] = 4.083503;
            table[50, 1] = 1.415331;
            table[51, 1] = -1.128886;
            table[52, 1] = -3.572233;
            table[53, 1] = -5.933481;
            table[54, 1] = -8.228569;
            table[55, 1] = -10.471469;
            table[56, 1] = -12.674693;
            table[57, 1] = -14.849557;
            table[58, 1] = -17.006294;
            table[59, 1] = -19.154075;
            table[60, 1] = -21.300962;
            table[61, 1] = -23.453818;
            table[62, 1] = -25.618191;
            table[63, 1] = -27.798185;
            table[64, 1] = -29.996328;
            table[65, 1] = -32.213454;
            table[66, 1] = -34.448603;
            table[67, 1] = -36.69896;
            table[68, 1] = -38.959839;
            table[69, 1] = -41.224721;
            table[70, 1] = -43.485364;
            table[71, 1] = -45.731989;
            table[72, 1] = -47.953532;
            table[73, 1] = -50.137983;
            table[74, 1] = -52.272788;
            table[75, 1] = -54.345303;
            table[76, 1] = -56.343288;
            table[77, 1] = -58.255405;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_land_2000m_1t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.731901;
            table[1, 1] = 100.647204;
            table[2, 1] = 97.07736;
            table[3, 1] = 94.538574;
            table[4, 1] = 92.565164;
            table[5, 1] = 90.949385;
            table[6, 1] = 89.580214;
            table[7, 1] = 88.391217;
            table[8, 1] = 87.339403;
            table[9, 1] = 86.39529;
            table[10, 1] = 85.537734;
            table[11, 1] = 84.751017;
            table[12, 1] = 84.0231;
            table[13, 1] = 83.344532;
            table[14, 1] = 82.707736;
            table[15, 1] = 82.106526;
            table[16, 1] = 81.535773;
            table[17, 1] = 80.991169;
            table[18, 1] = 80.469052;
            table[19, 1] = 79.966282;
            table[20, 1] = 77.658966;
            table[21, 1] = 75.603555;
            table[22, 1] = 73.632669;
            table[23, 1] = 71.640678;
            table[24, 1] = 69.570735;
            table[25, 1] = 67.447081;
            table[26, 1] = 65.425234;
            table[27, 1] = 63.427055;
            table[28, 1] = 61.453541;
            table[29, 1] = 59.511102;
            table[30, 1] = 57.604681;
            table[31, 1] = 55.73805;
            table[32, 1] = 53.914076;
            table[33, 1] = 52.134917;
            table[34, 1] = 50.402149;
            table[35, 1] = 48.716853;
            table[36, 1] = 45.490814;
            table[37, 1] = 42.457311;
            table[38, 1] = 39.61167;
            table[39, 1] = 36.945308;
            table[40, 1] = 34.446997;
            table[41, 1] = 32.104024;
            table[42, 1] = 29.903114;
            table[43, 1] = 27.831127;
            table[44, 1] = 25.875517;
            table[45, 1] = 24.024607;
            table[46, 1] = 19.787976;
            table[47, 1] = 16.003163;
            table[48, 1] = 12.566901;
            table[49, 1] = 9.4033;
            table[50, 1] = 6.456088;
            table[51, 1] = 3.682625;
            table[52, 1] = 1.049656;
            table[53, 1] = -1.469571;
            table[54, 1] = -3.897229;
            table[55, 1] = -6.252168;
            table[56, 1] = -8.550725;
            table[57, 1] = -10.807232;
            table[58, 1] = -13.034316;
            table[59, 1] = -15.243061;
            table[60, 1] = -17.443067;
            table[61, 1] = -19.642442;
            table[62, 1] = -21.84775;
            table[63, 1] = -24.063925;
            table[64, 1] = -26.294183;
            table[65, 1] = -28.539928;
            table[66, 1] = -30.800675;
            table[67, 1] = -33.07401;
            table[68, 1] = -35.355581;
            table[69, 1] = -37.639155;
            table[70, 1] = -39.916736;
            table[71, 1] = -42.17875;
            table[72, 1] = -44.414312;
            table[73, 1] = -46.611567;
            table[74, 1] = -48.758093;
            table[75, 1] = -50.841363;
            table[76, 1] = -52.849237;
            table[77, 1] = -54.770467;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_50t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.896153;
            table[1, 1] = 100.850778;
            table[2, 1] = 97.265159;
            table[3, 1] = 94.647118;
            table[4, 1] = 92.519505;
            table[5, 1] = 90.663909;
            table[6, 1] = 88.960853;
            table[7, 1] = 87.338462;
            table[8, 1] = 85.751731;
            table[9, 1] = 84.172684;
            table[10, 1] = 82.584954;
            table[11, 1] = 80.980373;
            table[12, 1] = 79.356538;
            table[13, 1] = 77.714974;
            table[14, 1] = 76.059684;
            table[15, 1] = 74.396047;
            table[16, 1] = 72.729995;
            table[17, 1] = 71.067437;
            table[18, 1] = 69.413884;
            table[19, 1] = 67.774236;
            table[20, 1] = 59.906487;
            table[21, 1] = 52.744977;
            table[22, 1] = 46.317639;
            table[23, 1] = 40.544845;
            table[24, 1] = 35.333956;
            table[25, 1] = 30.598744;
            table[26, 1] = 26.266713;
            table[27, 1] = 22.280139;
            table[28, 1] = 18.595849;
            table[29, 1] = 15.185415;
            table[30, 1] = 12.036025;
            table[31, 1] = 9.151226;
            table[32, 1] = 6.549679;
            table[33, 1] = 4.259357;
            table[34, 1] = 2.990107;
            table[35, 1] = 2.326621;
            table[36, 1] = 1.060562;
            table[37, 1] = -0.158225;
            table[38, 1] = -1.356401;
            table[39, 1] = -2.549555;
            table[40, 1] = -3.746076;
            table[41, 1] = -4.949737;
            table[42, 1] = -6.161413;
            table[43, 1] = -7.380224;
            table[44, 1] = -8.604293;
            table[45, 1] = -9.831241;
            table[46, 1] = -12.894533;
            table[47, 1] = -15.922888;
            table[48, 1] = -18.887771;
            table[49, 1] = -21.770782;
            table[50, 1] = -24.562405;
            table[51, 1] = -27.260059;
            table[52, 1] = -29.866226;
            table[53, 1] = -32.253414;
            table[54, 1] = -34.563668;
            table[55, 1] = -36.870658;
            table[56, 1] = -39.18297;
            table[57, 1] = -41.504977;
            table[58, 1] = -43.837926;
            table[59, 1] = -46.180696;
            table[60, 1] = -48.435421;
            table[61, 1] = -50.61745;
            table[62, 1] = -52.796071;
            table[63, 1] = -54.976316;
            table[64, 1] = -57.16161;
            table[65, 1] = -59.353647;
            table[66, 1] = -61.552311;
            table[67, 1] = -63.755629;
            table[68, 1] = -65.95978;
            table[69, 1] = -68.159159;
            table[70, 1] = -70.346507;
            table[71, 1] = -72.513108;
            table[72, 1] = -74.649055;
            table[73, 1] = -76.743588;
            table[74, 1] = -78.494006;
            table[75, 1] = -80.148983;
            table[76, 1] = -81.714014;
            table[77, 1] = -83.18821;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_50t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.899057;
            table[1, 1] = 100.870859;
            table[2, 1] = 97.326603;
            table[3, 1] = 94.781639;
            table[4, 1] = 92.764233;
            table[5, 1] = 91.059136;
            table[6, 1] = 89.547584;
            table[7, 1] = 88.155857;
            table[8, 1] = 86.834669;
            table[9, 1] = 85.549656;
            table[10, 1] = 84.276541;
            table[11, 1] = 82.998424;
            table[12, 1] = 81.704109;
            table[13, 1] = 80.386929;
            table[14, 1] = 79.04382;
            table[15, 1] = 77.674527;
            table[16, 1] = 76.280894;
            table[17, 1] = 74.866239;
            table[18, 1] = 73.434813;
            table[19, 1] = 71.991347;
            table[20, 1] = 64.75575;
            table[21, 1] = 57.809598;
            table[22, 1] = 51.376418;
            table[23, 1] = 45.496852;
            table[24, 1] = 40.137619;
            table[25, 1] = 35.241613;
            table[26, 1] = 30.750537;
            table[27, 1] = 26.613404;
            table[28, 1] = 22.789636;
            table[29, 1] = 19.250456;
            table[30, 1] = 15.979849;
            table[31, 1] = 12.975;
            table[32, 1] = 10.245093;
            table[33, 1] = 7.80681;
            table[34, 1] = 5.675631;
            table[35, 1] = 3.854831;
            table[36, 1] = 2.130753;
            table[37, 1] = 0.716088;
            table[38, 1] = -0.628133;
            table[39, 1] = -1.931573;
            table[40, 1] = -3.212504;
            table[41, 1] = -4.481637;
            table[42, 1] = -5.744745;
            table[43, 1] = -7.004444;
            table[44, 1] = -8.261376;
            table[45, 1] = -9.515006;
            table[46, 1] = -12.626079;
            table[47, 1] = -15.684866;
            table[48, 1] = -18.669933;
            table[49, 1] = -21.566792;
            table[50, 1] = -24.306493;
            table[51, 1] = -26.752959;
            table[52, 1] = -29.130779;
            table[53, 1] = -31.468946;
            table[54, 1] = -33.788904;
            table[55, 1] = -36.104977;
            table[56, 1] = -38.425826;
            table[57, 1] = -40.755888;
            table[58, 1] = -43.096458;
            table[59, 1] = -45.446459;
            table[60, 1] = -47.802962;
            table[61, 1] = -50.161567;
            table[62, 1] = -52.516697;
            table[63, 1] = -54.812075;
            table[64, 1] = -56.997787;
            table[65, 1] = -59.190182;
            table[66, 1] = -61.389155;
            table[67, 1] = -63.59274;
            table[68, 1] = -65.797124;
            table[69, 1] = -67.996707;
            table[70, 1] = -70.184233;
            table[71, 1] = -72.345327;
            table[72, 1] = -74.258597;
            table[73, 1] = -76.090723;
            table[74, 1] = -77.837717;
            table[75, 1] = -79.4966;
            table[76, 1] = -81.065446;
            table[77, 1] = -82.543372;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_50t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.899837;
            table[1, 1] = 100.877526;
            table[2, 1] = 97.349755;
            table[3, 1] = 94.837261;
            table[4, 1] = 92.873361;
            table[5, 1] = 91.247318;
            table[6, 1] = 89.844094;
            table[7, 1] = 88.592674;
            table[8, 1] = 87.445207;
            table[9, 1] = 86.367319;
            table[10, 1] = 85.33318;
            table[11, 1] = 84.322809;
            table[12, 1] = 83.320559;
            table[13, 1] = 82.314215;
            table[14, 1] = 81.294434;
            table[15, 1] = 80.254378;
            table[16, 1] = 79.189418;
            table[17, 1] = 78.096875;
            table[18, 1] = 76.975768;
            table[19, 1] = 75.826544;
            table[20, 1] = 69.740777;
            table[21, 1] = 63.40346;
            table[22, 1] = 57.194721;
            table[23, 1] = 51.324617;
            table[24, 1] = 45.866825;
            table[25, 1] = 40.82369;
            table[26, 1] = 36.168363;
            table[27, 1] = 31.866085;
            table[28, 1] = 27.883985;
            table[29, 1] = 24.195471;
            table[30, 1] = 20.782155;
            table[31, 1] = 17.634379;
            table[32, 1] = 14.75032;
            table[33, 1] = 12.133216;
            table[34, 1] = 9.786556;
            table[35, 1] = 7.708282;
            table[36, 1] = 4.298228;
            table[37, 1] = 1.933783;
            table[38, 1] = 0.442696;
            table[39, 1] = -0.972094;
            table[40, 1] = -2.338461;
            table[41, 1] = -3.673943;
            table[42, 1] = -4.989191;
            table[43, 1] = -6.290326;
            table[44, 1] = -7.580536;
            table[45, 1] = -8.861155;
            table[46, 1] = -12.020474;
            table[47, 1] = -15.109931;
            table[48, 1] = -18.046378;
            table[49, 1] = -20.742974;
            table[50, 1] = -23.308113;
            table[51, 1] = -25.769529;
            table[52, 1] = -28.161006;
            table[53, 1] = -30.511782;
            table[54, 1] = -32.843474;
            table[55, 1] = -35.170528;
            table[56, 1] = -37.501704;
            table[57, 1] = -39.84151;
            table[58, 1] = -42.191306;
            table[59, 1] = -44.550061;
            table[60, 1] = -46.914891;
            table[61, 1] = -49.281432;
            table[62, 1] = -51.644137;
            table[63, 1] = -53.996525;
            table[64, 1] = -56.331406;
            table[65, 1] = -58.641097;
            table[66, 1] = -60.889982;
            table[67, 1] = -63.093842;
            table[68, 1] = -65.298465;
            table[69, 1] = -67.468434;
            table[70, 1] = -69.533781;
            table[71, 1] = -71.528575;
            table[72, 1] = -73.447012;
            table[73, 1] = -75.284158;
            table[74, 1] = -77.036037;
            table[75, 1] = -78.699684;
            table[76, 1] = -80.273181;
            table[77, 1] = -81.755657;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_50t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.899991;
            table[1, 1] = 100.879241;
            table[2, 1] = 97.35674;
            table[3, 1] = 94.856088;
            table[4, 1] = 92.913839;
            table[5, 1] = 91.322718;
            table[6, 1] = 89.971255;
            table[7, 1] = 88.791969;
            table[8, 1] = 87.740362;
            table[9, 1] = 86.785072;
            table[10, 1] = 85.902759;
            table[11, 1] = 85.075241;
            table[12, 1] = 84.28781;
            table[13, 1] = 83.528204;
            table[14, 1] = 82.78596;
            table[15, 1] = 82.05202;
            table[16, 1] = 81.318488;
            table[17, 1] = 80.57849;
            table[18, 1] = 79.826101;
            table[19, 1] = 79.056316;
            table[20, 1] = 74.834484;
            table[21, 1] = 69.926229;
            table[22, 1] = 64.534504;
            table[23, 1] = 58.984299;
            table[24, 1] = 53.529972;
            table[25, 1] = 48.314254;
            table[26, 1] = 43.39804;
            table[27, 1] = 38.796625;
            table[28, 1] = 34.504168;
            table[29, 1] = 30.507364;
            table[30, 1] = 26.792357;
            table[31, 1] = 23.347829;
            table[32, 1] = 20.165859;
            table[33, 1] = 17.241356;
            table[34, 1] = 14.570526;
            table[35, 1] = 12.148828;
            table[36, 1] = 8.019626;
            table[37, 1] = 4.745406;
            table[38, 1] = 2.2493;
            table[39, 1] = 0.686452;
            table[40, 1] = -0.794206;
            table[41, 1] = -2.218847;
            table[42, 1] = -3.604387;
            table[43, 1] = -4.961516;
            table[44, 1] = -6.296776;
            table[45, 1] = -7.613981;
            table[46, 1] = -10.838801;
            table[47, 1] = -13.963492;
            table[48, 1] = -16.824754;
            table[49, 1] = -19.548224;
            table[50, 1] = -22.136744;
            table[51, 1] = -24.61907;
            table[52, 1] = -27.029581;
            table[53, 1] = -29.397898;
            table[54, 1] = -31.745898;
            table[55, 1] = -34.088218;
            table[56, 1] = -36.433755;
            table[57, 1] = -38.787131;
            table[58, 1] = -41.149789;
            table[59, 1] = -43.520773;
            table[60, 1] = -45.897258;
            table[61, 1] = -48.274931;
            table[62, 1] = -50.648291;
            table[63, 1] = -53.010893;
            table[64, 1] = -55.355584;
            table[65, 1] = -57.653785;
            table[66, 1] = -59.853511;
            table[67, 1] = -62.057748;
            table[68, 1] = -64.2627;
            table[69, 1] = -66.462778;
            table[70, 1] = -68.609829;
            table[71, 1] = -70.612313;
            table[72, 1] = -72.538218;
            table[73, 1] = -74.382626;
            table[74, 1] = -76.141574;
            table[75, 1] = -77.812112;
            table[76, 1] = -79.392333;
            table[77, 1] = -80.881375;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_50t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.9;
            table[1, 1] = 100.8794;
            table[2, 1] = 97.357572;
            table[3, 1] = 94.858786;
            table[4, 1] = 92.920549;
            table[5, 1] = 91.336833;
            table[6, 1] = 89.997702;
            table[7, 1] = 88.837485;
            table[8, 1] = 87.813761;
            table[9, 1] = 86.897486;
            table[10, 1] = 86.067848;
            table[11, 1] = 85.30936;
            table[12, 1] = 84.610125;
            table[13, 1] = 83.960736;
            table[14, 1] = 83.353551;
            table[15, 1] = 82.782205;
            table[16, 1] = 82.241258;
            table[17, 1] = 81.725952;
            table[18, 1] = 81.232021;
            table[19, 1] = 80.755556;
            table[20, 1] = 78.512059;
            table[21, 1] = 76.198346;
            table[22, 1] = 73.431702;
            table[23, 1] = 69.940889;
            table[24, 1] = 65.684445;
            table[25, 1] = 60.851521;
            table[26, 1] = 55.726974;
            table[27, 1] = 50.560619;
            table[28, 1] = 45.520483;
            table[29, 1] = 40.703343;
            table[30, 1] = 36.159753;
            table[31, 1] = 31.914372;
            table[32, 1] = 27.97838;
            table[33, 1] = 24.355782;
            table[34, 1] = 21.045901;
            table[35, 1] = 18.043823;
            table[36, 1] = 12.920398;
            table[37, 1] = 8.854389;
            table[38, 1] = 5.659467;
            table[39, 1] = 3.186875;
            table[40, 1] = 1.547648;
            table[41, 1] = -0.001808;
            table[42, 1] = -1.48644;
            table[43, 1] = -2.922936;
            table[44, 1] = -4.322325;
            table[45, 1] = -5.691788;
            table[46, 1] = -9.010568;
            table[47, 1] = -12.194574;
            table[48, 1] = -15.117478;
            table[49, 1] = -17.8912;
            table[50, 1] = -20.522826;
            table[51, 1] = -23.043323;
            table[52, 1] = -25.488352;
            table[53, 1] = -27.888338;
            table[54, 1] = -30.265698;
            table[55, 1] = -32.635448;
            table[56, 1] = -35.006766;
            table[57, 1] = -37.384485;
            table[58, 1] = -39.770221;
            table[59, 1] = -42.163153;
            table[60, 1] = -44.56057;
            table[61, 1] = -46.958256;
            table[62, 1] = -49.35079;
            table[63, 1] = -51.653538;
            table[64, 1] = -53.841131;
            table[65, 1] = -56.035139;
            table[66, 1] = -58.235501;
            table[67, 1] = -60.440289;
            table[68, 1] = -62.64572;
            table[69, 1] = -64.846217;
            table[70, 1] = -67.034547;
            table[71, 1] = -69.202012;
            table[72, 1] = -71.338725;
            table[73, 1] = -73.256204;
            table[74, 1] = -75.028003;
            table[75, 1] = -76.71104;
            table[76, 1] = -78.303423;
            table[77, 1] = -79.804305;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_50t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.9;
            table[1, 1] = 100.8794;
            table[2, 1] = 97.357575;
            table[3, 1] = 94.8588;
            table[4, 1] = 92.9206;
            table[5, 1] = 91.336975;
            table[6, 1] = 89.998039;
            table[7, 1] = 88.8382;
            table[8, 1] = 87.815149;
            table[9, 1] = 86.899998;
            table[10, 1] = 86.072141;
            table[11, 1] = 85.316365;
            table[12, 1] = 84.621114;
            table[13, 1] = 83.977404;
            table[14, 1] = 83.378113;
            table[15, 1] = 82.817495;
            table[16, 1] = 82.290848;
            table[17, 1] = 81.794273;
            table[18, 1] = 81.324496;
            table[19, 1] = 80.878743;
            table[20, 1] = 78.937108;
            table[21, 1] = 77.339358;
            table[22, 1] = 75.954356;
            table[23, 1] = 74.667964;
            table[24, 1] = 73.341981;
            table[25, 1] = 71.785339;
            table[26, 1] = 69.754518;
            table[27, 1] = 67.015142;
            table[28, 1] = 63.457992;
            table[29, 1] = 59.174315;
            table[30, 1] = 54.40508;
            table[31, 1] = 49.420936;
            table[32, 1] = 44.440345;
            table[33, 1] = 39.611459;
            table[34, 1] = 35.026333;
            table[35, 1] = 30.739929;
            table[36, 1] = 23.174052;
            table[37, 1] = 16.999493;
            table[38, 1] = 12.133858;
            table[39, 1] = 8.377091;
            table[40, 1] = 5.474771;
            table[41, 1] = 3.286941;
            table[42, 1] = 1.636818;
            table[43, 1] = 0.066439;
            table[44, 1] = -1.441992;
            table[45, 1] = -2.900885;
            table[46, 1] = -6.381755;
            table[47, 1] = -9.668201;
            table[48, 1] = -12.673092;
            table[49, 1] = -15.511803;
            table[50, 1] = -18.198702;
            table[51, 1] = -20.768179;
            table[52, 1] = -23.25769;
            table[53, 1] = -25.698696;
            table[54, 1] = -28.114269;
            table[55, 1] = -30.519867;
            table[56, 1] = -32.924992;
            table[57, 1] = -35.334724;
            table[58, 1] = -37.750873;
            table[59, 1] = -40.17278;
            table[60, 1] = -42.597869;
            table[61, 1] = -45.020793;
            table[62, 1] = -47.204724;
            table[63, 1] = -49.389464;
            table[64, 1] = -51.578586;
            table[65, 1] = -53.773903;
            table[66, 1] = -55.975393;
            table[67, 1] = -58.181158;
            table[68, 1] = -60.387439;
            table[69, 1] = -62.588679;
            table[70, 1] = -64.777661;
            table[71, 1] = -66.945702;
            table[72, 1] = -69.082923;
            table[73, 1] = -71.178586;
            table[74, 1] = -73.221484;
            table[75, 1] = -75.060364;
            table[76, 1] = -76.668846;
            table[77, 1] = -78.185413;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_50t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.9;
            table[1, 1] = 100.8794;
            table[2, 1] = 97.357575;
            table[3, 1] = 94.8588;
            table[4, 1] = 92.9206;
            table[5, 1] = 91.336975;
            table[6, 1] = 89.998039;
            table[7, 1] = 88.8382;
            table[8, 1] = 87.81515;
            table[9, 1] = 86.9;
            table[10, 1] = 86.072146;
            table[11, 1] = 85.316375;
            table[12, 1] = 84.621133;
            table[13, 1] = 83.977439;
            table[14, 1] = 83.378175;
            table[15, 1] = 82.8176;
            table[16, 1] = 82.291022;
            table[17, 1] = 81.79455;
            table[18, 1] = 81.324928;
            table[19, 1] = 80.8794;
            table[20, 1] = 78.941198;
            table[21, 1] = 77.357562;
            table[22, 1] = 76.018559;
            table[23, 1] = 74.858412;
            table[24, 1] = 73.834179;
            table[25, 1] = 72.915125;
            table[26, 1] = 72.075809;
            table[27, 1] = 71.289543;
            table[28, 1] = 70.519746;
            table[29, 1] = 69.707133;
            table[30, 1] = 68.75236;
            table[31, 1] = 67.499936;
            table[32, 1] = 65.742743;
            table[33, 1] = 63.276427;
            table[34, 1] = 60.001779;
            table[35, 1] = 55.998104;
            table[36, 1] = 46.736087;
            table[37, 1] = 37.326723;
            table[38, 1] = 28.841187;
            table[39, 1] = 21.700542;
            table[40, 1] = 15.978382;
            table[41, 1] = 11.534513;
            table[42, 1] = 8.110688;
            table[43, 1] = 5.425156;
            table[44, 1] = 3.237072;
            table[45, 1] = 1.369317;
            table[46, 1] = -2.565329;
            table[47, 1] = -6.003133;
            table[48, 1] = -9.162714;
            table[49, 1] = -12.102629;
            table[50, 1] = -14.86563;
            table[51, 1] = -17.497846;
            table[52, 1] = -20.042071;
            table[53, 1] = -22.532412;
            table[54, 1] = -24.99337;
            table[55, 1] = -27.441244;
            table[56, 1] = -29.886077;
            table[57, 1] = -32.333318;
            table[58, 1] = -34.785053;
            table[59, 1] = -37.240835;
            table[60, 1] = -39.698259;
            table[61, 1] = -42.044131;
            table[62, 1] = -44.232839;
            table[63, 1] = -46.421622;
            table[64, 1] = -48.614189;
            table[65, 1] = -50.812459;
            table[66, 1] = -53.016494;
            table[67, 1] = -55.224464;
            table[68, 1] = -57.432663;
            table[69, 1] = -59.635581;
            table[70, 1] = -61.826035;
            table[71, 1] = -63.995373;
            table[72, 1] = -66.133742;
            table[73, 1] = -68.230424;
            table[74, 1] = -70.27423;
            table[75, 1] = -72.253933;
            table[76, 1] = -74.152532;
            table[77, 1] = -75.687458;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_50t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 106.9;
            table[1, 1] = 100.8794;
            table[2, 1] = 97.357575;
            table[3, 1] = 94.8588;
            table[4, 1] = 92.9206;
            table[5, 1] = 91.336975;
            table[6, 1] = 89.998039;
            table[7, 1] = 88.8382;
            table[8, 1] = 87.81515;
            table[9, 1] = 86.9;
            table[10, 1] = 86.072146;
            table[11, 1] = 85.316375;
            table[12, 1] = 84.621133;
            table[13, 1] = 83.977439;
            table[14, 1] = 83.378175;
            table[15, 1] = 82.8176;
            table[16, 1] = 82.291022;
            table[17, 1] = 81.79455;
            table[18, 1] = 81.324928;
            table[19, 1] = 80.8794;
            table[20, 1] = 78.9412;
            table[21, 1] = 77.357575;
            table[22, 1] = 76.018639;
            table[23, 1] = 74.8588;
            table[24, 1] = 73.83575;
            table[25, 1] = 72.920599;
            table[26, 1] = 72.092743;
            table[27, 1] = 71.336961;
            table[28, 1] = 70.641679;
            table[29, 1] = 69.997853;
            table[30, 1] = 69.398186;
            table[31, 1] = 68.83647;
            table[32, 1] = 68.306861;
            table[33, 1] = 67.802799;
            table[34, 1] = 67.315127;
            table[35, 1] = 66.828698;
            table[36, 1] = 65.7294;
            table[37, 1] = 63.948181;
            table[38, 1] = 60.302187;
            table[39, 1] = 53.843331;
            table[40, 1] = 45.421963;
            table[41, 1] = 36.724786;
            table[42, 1] = 28.811485;
            table[43, 1] = 22.092614;
            table[44, 1] = 16.618484;
            table[45, 1] = 12.24958;
            table[46, 1] = 4.71951;
            table[47, 1] = -0.199175;
            table[48, 1] = -3.993911;
            table[49, 1] = -7.238635;
            table[50, 1] = -10.170259;
            table[51, 1] = -12.910549;
            table[52, 1] = -15.533583;
            table[53, 1] = -18.087254;
            table[54, 1] = -20.602525;
            table[55, 1] = -23.098925;
            table[56, 1] = -25.588218;
            table[57, 1] = -28.076839;
            table[58, 1] = -30.567478;
            table[59, 1] = -33.060086;
            table[60, 1] = -35.552538;
            table[61, 1] = -38.041082;
            table[62, 1] = -40.460121;
            table[63, 1] = -42.661273;
            table[64, 1] = -44.864384;
            table[65, 1] = -47.071696;
            table[66, 1] = -49.283525;
            table[67, 1] = -51.49825;
            table[68, 1] = -53.712329;
            table[69, 1] = -55.920389;
            table[70, 1] = -58.115359;
            table[71, 1] = -60.288677;
            table[72, 1] = -62.430567;
            table[73, 1] = -64.530376;
            table[74, 1] = -66.576966;
            table[75, 1] = -68.559158;
            table[76, 1] = -70.419687;
            table[77, 1] = -71.974374;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_10t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.071252;
            table[1, 1] = 101.14656;
            table[2, 1] = 97.762087;
            table[3, 1] = 95.364768;
            table[4, 1] = 93.48643;
            table[5, 1] = 91.922894;
            table[6, 1] = 90.57142;
            table[7, 1] = 89.386832;
            table[8, 1] = 88.31644;
            table[9, 1] = 87.333533;
            table[10, 1] = 86.419345;
            table[11, 1] = 85.560251;
            table[12, 1] = 84.746101;
            table[13, 1] = 83.969175;
            table[14, 1] = 83.223505;
            table[15, 1] = 82.504412;
            table[16, 1] = 81.808185;
            table[17, 1] = 81.131859;
            table[18, 1] = 80.473037;
            table[19, 1] = 79.829776;
            table[20, 1] = 76.800097;
            table[21, 1] = 74.008536;
            table[22, 1] = 71.402725;
            table[23, 1] = 68.956782;
            table[24, 1] = 66.654654;
            table[25, 1] = 64.484037;
            table[26, 1] = 62.434241;
            table[27, 1] = 60.495526;
            table[28, 1] = 58.914279;
            table[29, 1] = 57.508267;
            table[30, 1] = 56.192341;
            table[31, 1] = 54.956396;
            table[32, 1] = 53.792278;
            table[33, 1] = 52.693397;
            table[34, 1] = 51.654371;
            table[35, 1] = 50.670709;
            table[36, 1] = 48.854344;
            table[37, 1] = 47.216691;
            table[38, 1] = 45.731419;
            table[39, 1] = 44.371588;
            table[40, 1] = 43.11021;
            table[41, 1] = 41.92199;
            table[42, 1] = 40.785134;
            table[43, 1] = 39.682541;
            table[44, 1] = 38.602155;
            table[45, 1] = 37.536605;
            table[46, 1] = 34.921221;
            table[47, 1] = 32.386777;
            table[48, 1] = 29.965616;
            table[49, 1] = 27.678118;
            table[50, 1] = 25.524661;
            table[51, 1] = 23.491925;
            table[52, 1] = 21.56098;
            table[53, 1] = 19.712764;
            table[54, 1] = 17.930709;
            table[55, 1] = 16.201493;
            table[56, 1] = 14.514864;
            table[57, 1] = 12.86312;
            table[58, 1] = 11.240547;
            table[59, 1] = 9.64294;
            table[60, 1] = 8.067225;
            table[61, 1] = 6.511182;
            table[62, 1] = 4.973236;
            table[63, 1] = 3.452318;
            table[64, 1] = 1.947749;
            table[65, 1] = 0.459166;
            table[66, 1] = -1.013546;
            table[67, 1] = -2.470297;
            table[68, 1] = -3.910834;
            table[69, 1] = -5.334774;
            table[70, 1] = -6.741627;
            table[71, 1] = -8.130825;
            table[72, 1] = -9.501738;
            table[73, 1] = -10.853694;
            table[74, 1] = -12.185995;
            table[75, 1] = -13.497928;
            table[76, 1] = -14.788782;
            table[77, 1] = -16.057854;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_10t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.073748;
            table[1, 1] = 101.159627;
            table[2, 1] = 97.797043;
            table[3, 1] = 95.435065;
            table[4, 1] = 93.606869;
            table[5, 1] = 92.108932;
            table[6, 1] = 90.834044;
            table[7, 1] = 89.718665;
            table[8, 1] = 88.721966;
            table[9, 1] = 87.816121;
            table[10, 1] = 86.981315;
            table[11, 1] = 86.202958;
            table[12, 1] = 85.470031;
            table[13, 1] = 84.774055;
            table[14, 1] = 84.108416;
            table[15, 1] = 83.467908;
            table[16, 1] = 82.848413;
            table[17, 1] = 82.246665;
            table[18, 1] = 81.660075;
            table[19, 1] = 81.086602;
            table[20, 1] = 78.451586;
            table[21, 1] = 76.030135;
            table[22, 1] = 73.73455;
            table[23, 1] = 71.53752;
            table[24, 1] = 69.428249;
            table[25, 1] = 67.402497;
            table[26, 1] = 65.608882;
            table[27, 1] = 63.963965;
            table[28, 1] = 62.425514;
            table[29, 1] = 60.982568;
            table[30, 1] = 59.625523;
            table[31, 1] = 58.346162;
            table[32, 1] = 57.137566;
            table[33, 1] = 55.993951;
            table[34, 1] = 54.910463;
            table[35, 1] = 53.882958;
            table[36, 1] = 51.981553;
            table[37, 1] = 50.263041;
            table[38, 1] = 48.701332;
            table[39, 1] = 47.26941;
            table[40, 1] = 45.94006;
            table[41, 1] = 44.687689;
            table[42, 1] = 43.490186;
            table[43, 1] = 42.330136;
            table[44, 1] = 41.195188;
            table[45, 1] = 40.077698;
            table[46, 1] = 37.342438;
            table[47, 1] = 34.700089;
            table[48, 1] = 32.180856;
            table[49, 1] = 29.803535;
            table[50, 1] = 27.567277;
            table[51, 1] = 25.457778;
            table[52, 1] = 23.455311;
            table[53, 1] = 21.540156;
            table[54, 1] = 19.6952;
            table[55, 1] = 17.906669;
            table[56, 1] = 16.163926;
            table[57, 1] = 14.458944;
            table[58, 1] = 12.78573;
            table[59, 1] = 11.13984;
            table[60, 1] = 9.517989;
            table[61, 1] = 7.917776;
            table[62, 1] = 6.337466;
            table[63, 1] = 4.775849;
            table[64, 1] = 3.23212;
            table[65, 1] = 1.705803;
            table[66, 1] = 0.196686;
            table[67, 1] = -1.295233;
            table[68, 1] = -2.769783;
            table[69, 1] = -4.226651;
            table[70, 1] = -5.665417;
            table[71, 1] = -7.085572;
            table[72, 1] = -8.486541;
            table[73, 1] = -9.867703;
            table[74, 1] = -11.228406;
            table[75, 1] = -12.56798;
            table[76, 1] = -13.885754;
            table[77, 1] = -15.18106;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_10t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.073983;
            table[1, 1] = 101.161241;
            table[2, 1] = 97.802118;
            table[3, 1] = 95.446552;
            table[4, 1] = 93.628514;
            table[5, 1] = 92.145204;
            table[6, 1] = 90.890051;
            table[7, 1] = 89.800068;
            table[8, 1] = 88.834879;
            table[9, 1] = 87.967008;
            table[10, 1] = 87.176877;
            table[11, 1] = 86.450014;
            table[12, 1] = 85.775397;
            table[13, 1] = 85.144425;
            table[14, 1] = 84.550242;
            table[15, 1] = 83.987293;
            table[16, 1] = 83.451008;
            table[17, 1] = 82.937583;
            table[18, 1] = 82.443821;
            table[19, 1] = 81.967014;
            table[20, 1] = 79.767185;
            table[21, 1] = 77.755552;
            table[22, 1] = 75.844792;
            table[23, 1] = 74.002086;
            table[24, 1] = 72.2209;
            table[25, 1] = 70.504588;
            table[26, 1] = 68.857967;
            table[27, 1] = 67.284027;
            table[28, 1] = 65.783331;
            table[29, 1] = 64.354488;
            table[30, 1] = 62.99489;
            table[31, 1] = 61.70134;
            table[32, 1] = 60.470498;
            table[33, 1] = 59.299126;
            table[34, 1] = 58.184185;
            table[35, 1] = 57.122829;
            table[36, 1] = 55.150008;
            table[37, 1] = 53.358722;
            table[38, 1] = 51.725339;
            table[39, 1] = 50.224068;
            table[40, 1] = 48.828236;
            table[41, 1] = 47.512418;
            table[42, 1] = 46.254463;
            table[43, 1] = 45.036807;
            table[44, 1] = 43.846905;
            table[45, 1] = 42.67691;
            table[46, 1] = 39.819857;
            table[47, 1] = 37.067301;
            table[48, 1] = 34.447568;
            table[49, 1] = 31.978021;
            table[50, 1] = 29.656653;
            table[51, 1] = 27.468202;
            table[52, 1] = 25.392148;
            table[53, 1] = 23.408108;
            table[54, 1] = 21.498417;
            table[55, 1] = 19.648836;
            table[56, 1] = 17.848335;
            table[57, 1] = 16.088554;
            table[58, 1] = 14.363214;
            table[59, 1] = 12.667622;
            table[60, 1] = 10.998281;
            table[61, 1] = 9.3526;
            table[62, 1] = 7.72868;
            table[63, 1] = 6.125165;
            table[64, 1] = 4.541122;
            table[65, 1] = 2.97596;
            table[66, 1] = 1.429362;
            table[67, 1] = -0.098765;
            table[68, 1] = -1.608333;
            table[69, 1] = -3.099106;
            table[70, 1] = -4.570731;
            table[71, 1] = -6.022762;
            table[72, 1] = -7.454681;
            table[73, 1] = -8.86592;
            table[74, 1] = -10.255873;
            table[75, 1] = -11.623916;
            table[76, 1] = -12.969415;
            table[77, 1] = -14.291744;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_10t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.073999;
            table[1, 1] = 101.16139;
            table[2, 1] = 97.802681;
            table[3, 1] = 95.448002;
            table[4, 1] = 93.631539;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.8992;
            table[7, 1] = 89.814242;
            table[8, 1] = 88.855708;
            table[9, 1] = 87.996362;
            table[10, 1] = 87.21686;
            table[11, 1] = 86.502959;
            table[12, 1] = 85.843854;
            table[13, 1] = 85.231145;
            table[14, 1] = 84.658167;
            table[15, 1] = 84.11953;
            table[16, 1] = 83.610809;
            table[17, 1] = 83.12832;
            table[18, 1] = 82.668958;
            table[19, 1] = 82.230077;
            table[20, 1] = 80.272677;
            table[21, 1] = 78.582204;
            table[22, 1] = 77.044933;
            table[23, 1] = 75.592459;
            table[24, 1] = 74.184444;
            table[25, 1] = 72.799911;
            table[26, 1] = 71.431133;
            table[27, 1] = 70.078545;
            table[28, 1] = 68.74668;
            table[29, 1] = 67.441345;
            table[30, 1] = 66.168011;
            table[31, 1] = 64.931112;
            table[32, 1] = 63.733897;
            table[33, 1] = 62.57853;
            table[34, 1] = 61.466269;
            table[35, 1] = 60.397619;
            table[36, 1] = 58.390012;
            table[37, 1] = 56.547963;
            table[38, 1] = 54.856052;
            table[39, 1] = 53.293169;
            table[40, 1] = 51.835255;
            table[41, 1] = 50.45829;
            table[42, 1] = 49.140823;
            table[43, 1] = 47.865591;
            table[44, 1] = 46.620135;
            table[45, 1] = 45.396582;
            table[46, 1] = 42.413846;
            table[47, 1] = 39.546259;
            table[48, 1] = 36.820925;
            table[49, 1] = 34.25411;
            table[50, 1] = 31.842799;
            table[51, 1] = 29.570826;
            table[52, 1] = 27.416878;
            table[53, 1] = 25.3599;
            table[54, 1] = 23.381652;
            table[55, 1] = 21.46741;
            table[56, 1] = 19.605732;
            table[57, 1] = 17.787904;
            table[58, 1] = 16.007347;
            table[59, 1] = 14.259104;
            table[60, 1] = 12.539453;
            table[61, 1] = 10.845603;
            table[62, 1] = 9.175482;
            table[63, 1] = 7.527578;
            table[64, 1] = 5.900823;
            table[65, 1] = 4.294502;
            table[66, 1] = 2.708191;
            table[67, 1] = 1.141697;
            table[68, 1] = -0.404979;
            table[69, 1] = -1.931682;
            table[70, 1] = -3.438131;
            table[71, 1] = -4.923945;
            table[72, 1] = -6.388669;
            table[73, 1] = -7.831787;
            table[74, 1] = -9.252747;
            table[75, 1] = -10.650969;
            table[76, 1] = -12.025863;
            table[77, 1] = -13.376843;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_10t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.1614;
            table[2, 1] = 97.802725;
            table[3, 1] = 95.448131;
            table[4, 1] = 93.631842;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.900292;
            table[7, 1] = 89.816057;
            table[8, 1] = 88.858547;
            table[9, 1] = 88.000595;
            table[10, 1] = 87.222932;
            table[11, 1] = 86.51139;
            table[12, 1] = 85.85525;
            table[13, 1] = 85.246199;
            table[14, 1] = 84.677657;
            table[15, 1] = 84.144329;
            table[16, 1] = 83.641883;
            table[17, 1] = 83.16673;
            table[18, 1] = 82.715861;
            table[19, 1] = 82.286726;
            table[20, 1] = 80.400107;
            table[21, 1] = 78.82475;
            table[22, 1] = 77.454075;
            table[23, 1] = 76.220844;
            table[24, 1] = 75.078611;
            table[25, 1] = 73.993573;
            table[26, 1] = 72.940868;
            table[27, 1] = 71.902852;
            table[28, 1] = 70.868153;
            table[29, 1] = 69.830864;
            table[30, 1] = 68.789585;
            table[31, 1] = 67.74629;
            table[32, 1] = 66.705146;
            table[33, 1] = 65.671434;
            table[34, 1] = 64.650671;
            table[35, 1] = 63.647984;
            table[36, 1] = 61.713173;
            table[37, 1] = 59.889255;
            table[38, 1] = 58.182127;
            table[39, 1] = 56.584556;
            table[40, 1] = 55.081064;
            table[41, 1] = 53.652769;
            table[42, 1] = 52.281224;
            table[43, 1] = 50.950892;
            table[44, 1] = 49.65029;
            table[45, 1] = 48.372107;
            table[46, 1] = 45.256931;
            table[47, 1] = 42.264514;
            table[48, 1] = 39.422756;
            table[49, 1] = 36.747821;
            table[50, 1] = 34.236112;
            table[51, 1] = 31.870709;
            table[52, 1] = 29.629532;
            table[53, 1] = 27.490825;
            table[54, 1] = 25.435734;
            table[55, 1] = 23.449002;
            table[56, 1] = 21.518733;
            table[57, 1] = 19.635822;
            table[58, 1] = 17.793351;
            table[59, 1] = 15.986073;
            table[60, 1] = 14.210009;
            table[61, 1] = 12.462147;
            table[62, 1] = 10.74022;
            table[63, 1] = 9.042543;
            table[64, 1] = 7.367893;
            table[65, 1] = 5.71542;
            table[66, 1] = 4.084577;
            table[67, 1] = 2.475061;
            table[68, 1] = 0.886775;
            table[69, 1] = -0.680217;
            table[70, 1] = -2.225715;
            table[71, 1] = -3.749412;
            table[72, 1] = -5.250921;
            table[73, 1] = -6.729787;
            table[74, 1] = -8.185515;
            table[75, 1] = -9.617579;
            table[76, 1] = -11.025435;
            table[77, 1] = -12.408542;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_10t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.1614;
            table[2, 1] = 97.802726;
            table[3, 1] = 95.448138;
            table[4, 1] = 93.631859;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.900372;
            table[7, 1] = 89.816201;
            table[8, 1] = 88.858789;
            table[9, 1] = 88.000982;
            table[10, 1] = 87.223521;
            table[11, 1] = 86.512255;
            table[12, 1] = 85.85648;
            table[13, 1] = 85.247902;
            table[14, 1] = 84.679963;
            table[15, 1] = 84.147387;
            table[16, 1] = 83.645869;
            table[17, 1] = 83.171844;
            table[18, 1] = 82.722333;
            table[19, 1] = 82.294814;
            table[20, 1] = 80.421316;
            table[21, 1] = 78.870972;
            table[22, 1] = 77.542601;
            table[23, 1] = 76.374804;
            table[24, 1] = 75.326766;
            table[25, 1] = 74.36928;
            table[26, 1] = 73.48011;
            table[27, 1] = 72.641449;
            table[28, 1] = 71.838516;
            table[29, 1] = 71.058825;
            table[30, 1] = 70.291891;
            table[31, 1] = 69.529217;
            table[32, 1] = 68.764412;
            table[33, 1] = 67.993329;
            table[34, 1] = 67.214069;
            table[35, 1] = 66.426807;
            table[36, 1] = 64.836853;
            table[37, 1] = 63.248553;
            table[38, 1] = 61.688114;
            table[39, 1] = 60.172188;
            table[40, 1] = 58.705592;
            table[41, 1] = 57.284296;
            table[42, 1] = 55.899909;
            table[43, 1] = 54.543462;
            table[44, 1] = 53.207753;
            table[45, 1] = 51.888322;
            table[46, 1] = 48.654898;
            table[47, 1] = 45.536189;
            table[48, 1] = 42.569556;
            table[49, 1] = 39.775366;
            table[50, 1] = 37.151413;
            table[51, 1] = 34.680848;
            table[52, 1] = 32.34115;
            table[53, 1] = 30.109965;
            table[54, 1] = 27.967836;
            table[55, 1] = 25.898951;
            table[56, 1] = 23.890923;
            table[57, 1] = 21.934214;
            table[58, 1] = 20.021531;
            table[59, 1] = 18.147299;
            table[60, 1] = 16.307254;
            table[61, 1] = 14.498132;
            table[62, 1] = 12.717445;
            table[63, 1] = 10.963313;
            table[64, 1] = 9.234338;
            table[65, 1] = 7.529517;
            table[66, 1] = 5.848163;
            table[67, 1] = 4.189849;
            table[68, 1] = 2.554365;
            table[69, 1] = 0.941674;
            table[70, 1] = -0.648116;
            table[71, 1] = -2.214784;
            table[72, 1] = -3.758017;
            table[73, 1] = -5.277433;
            table[74, 1] = -6.772599;
            table[75, 1] = -8.243048;
            table[76, 1] = -9.688293;
            table[77, 1] = -11.107842;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_10t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.1614;
            table[2, 1] = 97.802726;
            table[3, 1] = 95.448138;
            table[4, 1] = 93.63186;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.900374;
            table[7, 1] = 89.816206;
            table[8, 1] = 88.8588;
            table[9, 1] = 88.001;
            table[10, 1] = 87.22355;
            table[11, 1] = 86.512301;
            table[12, 1] = 85.85655;
            table[13, 1] = 85.248004;
            table[14, 1] = 84.680108;
            table[15, 1] = 84.147589;
            table[16, 1] = 83.646145;
            table[17, 1] = 83.172214;
            table[18, 1] = 82.722821;
            table[19, 1] = 82.295448;
            table[20, 1] = 80.423294;
            table[21, 1] = 78.875949;
            table[22, 1] = 77.553412;
            table[23, 1] = 76.395881;
            table[24, 1] = 75.364589;
            table[25, 1] = 74.432829;
            table[26, 1] = 73.581266;
            table[27, 1] = 72.795294;
            table[28, 1] = 72.063439;
            table[29, 1] = 71.376359;
            table[30, 1] = 70.726175;
            table[31, 1] = 70.106044;
            table[32, 1] = 69.509876;
            table[33, 1] = 68.932164;
            table[34, 1] = 68.367918;
            table[35, 1] = 67.812653;
            table[36, 1] = 66.713933;
            table[37, 1] = 65.612033;
            table[38, 1] = 64.493201;
            table[39, 1] = 63.352015;
            table[40, 1] = 62.18767;
            table[41, 1] = 61.000648;
            table[42, 1] = 59.791266;
            table[43, 1] = 58.559913;
            table[44, 1] = 57.307988;
            table[45, 1] = 56.038611;
            table[46, 1] = 52.82444;
            table[47, 1] = 49.635322;
            table[48, 1] = 46.559728;
            table[49, 1] = 43.644597;
            table[50, 1] = 40.899531;
            table[51, 1] = 38.312279;
            table[52, 1] = 35.861787;
            table[53, 1] = 33.525898;
            table[54, 1] = 31.284873;
            table[55, 1] = 29.122464;
            table[56, 1] = 27.025817;
            table[57, 1] = 24.984952;
            table[58, 1] = 22.992174;
            table[59, 1] = 21.041548;
            table[60, 1] = 19.128491;
            table[61, 1] = 17.249458;
            table[62, 1] = 15.40171;
            table[63, 1] = 13.583144;
            table[64, 1] = 11.792167;
            table[65, 1] = 10.027598;
            table[66, 1] = 8.288592;
            table[67, 1] = 6.574581;
            table[68, 1] = 4.885227;
            table[69, 1] = 3.220376;
            table[70, 1] = 1.580031;
            table[71, 1] = -0.035683;
            table[72, 1] = -1.626539;
            table[73, 1] = -3.192236;
            table[74, 1] = -4.732416;
            table[75, 1] = -6.246678;
            table[76, 1] = -7.734599;
            table[77, 1] = -9.195742;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_10t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.074;
            table[1, 1] = 101.1614;
            table[2, 1] = 97.802726;
            table[3, 1] = 95.448138;
            table[4, 1] = 93.63186;
            table[5, 1] = 92.150242;
            table[6, 1] = 90.900374;
            table[7, 1] = 89.816206;
            table[8, 1] = 88.8588;
            table[9, 1] = 88.001;
            table[10, 1] = 87.223551;
            table[11, 1] = 86.512302;
            table[12, 1] = 85.856551;
            table[13, 1] = 85.248006;
            table[14, 1] = 84.680111;
            table[15, 1] = 84.147595;
            table[16, 1] = 83.646153;
            table[17, 1] = 83.172225;
            table[18, 1] = 82.722836;
            table[19, 1] = 82.29547;
            table[20, 1] = 80.423375;
            table[21, 1] = 78.876192;
            table[22, 1] = 77.554022;
            table[23, 1] = 76.39723;
            table[24, 1] = 75.3673;
            table[25, 1] = 74.437878;
            table[26, 1] = 73.590114;
            table[27, 1] = 72.810032;
            table[28, 1] = 72.086969;
            table[29, 1] = 71.412576;
            table[30, 1] = 70.780175;
            table[31, 1] = 70.184314;
            table[32, 1] = 69.620457;
            table[33, 1] = 69.084763;
            table[34, 1] = 68.57392;
            table[35, 1] = 68.08503;
            table[36, 1] = 67.163051;
            table[37, 1] = 66.30095;
            table[38, 1] = 65.483484;
            table[39, 1] = 64.696964;
            table[40, 1] = 63.928318;
            table[41, 1] = 63.164242;
            table[42, 1] = 62.390624;
            table[43, 1] = 61.592547;
            table[44, 1] = 60.755142;
            table[45, 1] = 59.865368;
            table[46, 1] = 57.368548;
            table[47, 1] = 54.53538;
            table[48, 1] = 51.555459;
            table[49, 1] = 48.60235;
            table[50, 1] = 45.763433;
            table[51, 1] = 43.06294;
            table[52, 1] = 40.495161;
            table[53, 1] = 38.044061;
            table[54, 1] = 35.692174;
            table[55, 1] = 33.423932;
            table[56, 1] = 31.226507;
            table[57, 1] = 29.089687;
            table[58, 1] = 27.005451;
            table[59, 1] = 24.967523;
            table[60, 1] = 22.970992;
            table[61, 1] = 21.012007;
            table[62, 1] = 19.087555;
            table[63, 1] = 17.195286;
            table[64, 1] = 15.333382;
            table[65, 1] = 13.500464;
            table[66, 1] = 11.695506;
            table[67, 1] = 9.917778;
            table[68, 1] = 8.166795;
            table[69, 1] = 6.442273;
            table[70, 1] = 4.744093;
            table[71, 1] = 3.072271;
            table[72, 1] = 1.426933;
            table[73, 1] = -0.191712;
            table[74, 1] = -1.783387;
            table[75, 1] = -3.347772;
            table[76, 1] = -4.884514;
            table[77, 1] = -6.393242;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_1t_10h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170112;
            table[14, 1] = 86.654376;
            table[15, 1] = 86.169691;
            table[16, 1] = 85.712255;
            table[17, 1] = 85.278931;
            table[18, 1] = 84.867101;
            table[19, 1] = 84.474556;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.417326;
            table[31, 1] = 72.81016;
            table[32, 1] = 72.231168;
            table[33, 1] = 71.676959;
            table[34, 1] = 71.144651;
            table[35, 1] = 70.631767;
            table[36, 1] = 69.655968;
            table[37, 1] = 68.737006;
            table[38, 1] = 67.870185;
            table[39, 1] = 67.042965;
            table[40, 1] = 66.248898;
            table[41, 1] = 65.482818;
            table[42, 1] = 64.740558;
            table[43, 1] = 64.018753;
            table[44, 1] = 63.314686;
            table[45, 1] = 62.626176;
            table[46, 1] = 60.962469;
            table[47, 1] = 59.36806;
            table[48, 1] = 57.834335;
            table[49, 1] = 56.35803;
            table[50, 1] = 54.937829;
            table[51, 1] = 53.571877;
            table[52, 1] = 52.256465;
            table[53, 1] = 50.985894;
            table[54, 1] = 49.753108;
            table[55, 1] = 48.550642;
            table[56, 1] = 47.371499;
            table[57, 1] = 46.209744;
            table[58, 1] = 45.060779;
            table[59, 1] = 43.921341;
            table[60, 1] = 42.789324;
            table[61, 1] = 41.6635;
            table[62, 1] = 40.54324;
            table[63, 1] = 39.428255;
            table[64, 1] = 38.31841;
            table[65, 1] = 37.213602;
            table[66, 1] = 36.113704;
            table[67, 1] = 35.018559;
            table[68, 1] = 33.928003;
            table[69, 1] = 32.841922;
            table[70, 1] = 31.760307;
            table[71, 1] = 30.683308;
            table[72, 1] = 29.611291;
            table[73, 1] = 28.54487;
            table[74, 1] = 27.484937;
            table[75, 1] = 26.432671;
            table[76, 1] = 25.389529;
            table[77, 1] = 24.357236;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_1t_20h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170112;
            table[14, 1] = 86.654376;
            table[15, 1] = 86.169691;
            table[16, 1] = 85.712255;
            table[17, 1] = 85.278931;
            table[18, 1] = 84.867101;
            table[19, 1] = 84.474556;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.417326;
            table[31, 1] = 72.81016;
            table[32, 1] = 72.231168;
            table[33, 1] = 71.676959;
            table[34, 1] = 71.144651;
            table[35, 1] = 70.631767;
            table[36, 1] = 69.655968;
            table[37, 1] = 68.737006;
            table[38, 1] = 67.870185;
            table[39, 1] = 67.042965;
            table[40, 1] = 66.248898;
            table[41, 1] = 65.482818;
            table[42, 1] = 64.740558;
            table[43, 1] = 64.018753;
            table[44, 1] = 63.314686;
            table[45, 1] = 62.626176;
            table[46, 1] = 60.962469;
            table[47, 1] = 59.36806;
            table[48, 1] = 57.834335;
            table[49, 1] = 56.35803;
            table[50, 1] = 54.937829;
            table[51, 1] = 53.571877;
            table[52, 1] = 52.256465;
            table[53, 1] = 50.985894;
            table[54, 1] = 49.753108;
            table[55, 1] = 48.550642;
            table[56, 1] = 47.371499;
            table[57, 1] = 46.209744;
            table[58, 1] = 45.060779;
            table[59, 1] = 43.921341;
            table[60, 1] = 42.789324;
            table[61, 1] = 41.6635;
            table[62, 1] = 40.54324;
            table[63, 1] = 39.428255;
            table[64, 1] = 38.31841;
            table[65, 1] = 37.213602;
            table[66, 1] = 36.113704;
            table[67, 1] = 35.018559;
            table[68, 1] = 33.928003;
            table[69, 1] = 32.841922;
            table[70, 1] = 31.760307;
            table[71, 1] = 30.683308;
            table[72, 1] = 29.611291;
            table[73, 1] = 28.54487;
            table[74, 1] = 27.484937;
            table[75, 1] = 26.432671;
            table[76, 1] = 25.389529;
            table[77, 1] = 24.357236;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_1t_38h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170112;
            table[14, 1] = 86.654376;
            table[15, 1] = 86.169691;
            table[16, 1] = 85.712255;
            table[17, 1] = 85.278931;
            table[18, 1] = 84.867101;
            table[19, 1] = 84.474556;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.417326;
            table[31, 1] = 72.81016;
            table[32, 1] = 72.231168;
            table[33, 1] = 71.676959;
            table[34, 1] = 71.144651;
            table[35, 1] = 70.631767;
            table[36, 1] = 69.655968;
            table[37, 1] = 68.737006;
            table[38, 1] = 67.870185;
            table[39, 1] = 67.042965;
            table[40, 1] = 66.248898;
            table[41, 1] = 65.482818;
            table[42, 1] = 64.740558;
            table[43, 1] = 64.018753;
            table[44, 1] = 63.314686;
            table[45, 1] = 62.626176;
            table[46, 1] = 60.962469;
            table[47, 1] = 59.36806;
            table[48, 1] = 57.834335;
            table[49, 1] = 56.35803;
            table[50, 1] = 54.937829;
            table[51, 1] = 53.571877;
            table[52, 1] = 52.256465;
            table[53, 1] = 50.985894;
            table[54, 1] = 49.753108;
            table[55, 1] = 48.550642;
            table[56, 1] = 47.371499;
            table[57, 1] = 46.209744;
            table[58, 1] = 45.060779;
            table[59, 1] = 43.921341;
            table[60, 1] = 42.789324;
            table[61, 1] = 41.6635;
            table[62, 1] = 40.54324;
            table[63, 1] = 39.428255;
            table[64, 1] = 38.31841;
            table[65, 1] = 37.213602;
            table[66, 1] = 36.113704;
            table[67, 1] = 35.018559;
            table[68, 1] = 33.928003;
            table[69, 1] = 32.841922;
            table[70, 1] = 31.760307;
            table[71, 1] = 30.683308;
            table[72, 1] = 29.611291;
            table[73, 1] = 28.54487;
            table[74, 1] = 27.484937;
            table[75, 1] = 26.432671;
            table[76, 1] = 25.389529;
            table[77, 1] = 24.357236;


            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_1t_75h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170112;
            table[14, 1] = 86.654376;
            table[15, 1] = 86.169691;
            table[16, 1] = 85.712255;
            table[17, 1] = 85.278931;
            table[18, 1] = 84.867101;
            table[19, 1] = 84.474556;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.417326;
            table[31, 1] = 72.81016;
            table[32, 1] = 72.231168;
            table[33, 1] = 71.676959;
            table[34, 1] = 71.144651;
            table[35, 1] = 70.631767;
            table[36, 1] = 69.655968;
            table[37, 1] = 68.737006;
            table[38, 1] = 67.870185;
            table[39, 1] = 67.042965;
            table[40, 1] = 66.248898;
            table[41, 1] = 65.482818;
            table[42, 1] = 64.740558;
            table[43, 1] = 64.018753;
            table[44, 1] = 63.314686;
            table[45, 1] = 62.626176;
            table[46, 1] = 60.962469;
            table[47, 1] = 59.36806;
            table[48, 1] = 57.834335;
            table[49, 1] = 56.35803;
            table[50, 1] = 54.937829;
            table[51, 1] = 53.571877;
            table[52, 1] = 52.256465;
            table[53, 1] = 50.985894;
            table[54, 1] = 49.753108;
            table[55, 1] = 48.550642;
            table[56, 1] = 47.371499;
            table[57, 1] = 46.209744;
            table[58, 1] = 45.060779;
            table[59, 1] = 43.921341;
            table[60, 1] = 42.789324;
            table[61, 1] = 41.6635;
            table[62, 1] = 40.54324;
            table[63, 1] = 39.428255;
            table[64, 1] = 38.31841;
            table[65, 1] = 37.213602;
            table[66, 1] = 36.113704;
            table[67, 1] = 35.018559;
            table[68, 1] = 33.928003;
            table[69, 1] = 32.841922;
            table[70, 1] = 31.760307;
            table[71, 1] = 30.683308;
            table[72, 1] = 29.611291;
            table[73, 1] = 28.54487;
            table[74, 1] = 27.484937;
            table[75, 1] = 26.432671;
            table[76, 1] = 25.389529;
            table[77, 1] = 24.357236;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_1t_150h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170112;
            table[14, 1] = 86.654376;
            table[15, 1] = 86.169691;
            table[16, 1] = 85.712255;
            table[17, 1] = 85.278931;
            table[18, 1] = 84.867101;
            table[19, 1] = 84.474556;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.417326;
            table[31, 1] = 72.81016;
            table[32, 1] = 72.231168;
            table[33, 1] = 71.676959;
            table[34, 1] = 71.144651;
            table[35, 1] = 70.631767;
            table[36, 1] = 69.655968;
            table[37, 1] = 68.737006;
            table[38, 1] = 67.870185;
            table[39, 1] = 67.042965;
            table[40, 1] = 66.248898;
            table[41, 1] = 65.482818;
            table[42, 1] = 64.740558;
            table[43, 1] = 64.018753;
            table[44, 1] = 63.314686;
            table[45, 1] = 62.626176;
            table[46, 1] = 60.962469;
            table[47, 1] = 59.36806;
            table[48, 1] = 57.834335;
            table[49, 1] = 56.35803;
            table[50, 1] = 54.937829;
            table[51, 1] = 53.571877;
            table[52, 1] = 52.256465;
            table[53, 1] = 50.985894;
            table[54, 1] = 49.753108;
            table[55, 1] = 48.550642;
            table[56, 1] = 47.371499;
            table[57, 1] = 46.209744;
            table[58, 1] = 45.060779;
            table[59, 1] = 43.921341;
            table[60, 1] = 42.789324;
            table[61, 1] = 41.6635;
            table[62, 1] = 40.54324;
            table[63, 1] = 39.428255;
            table[64, 1] = 38.31841;
            table[65, 1] = 37.213602;
            table[66, 1] = 36.113704;
            table[67, 1] = 35.018559;
            table[68, 1] = 33.928003;
            table[69, 1] = 32.841922;
            table[70, 1] = 31.760307;
            table[71, 1] = 30.683308;
            table[72, 1] = 29.611291;
            table[73, 1] = 28.54487;
            table[74, 1] = 27.484937;
            table[75, 1] = 26.432671;
            table[76, 1] = 25.389529;
            table[77, 1] = 24.357236;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_1t_300h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170112;
            table[14, 1] = 86.654376;
            table[15, 1] = 86.169691;
            table[16, 1] = 85.712255;
            table[17, 1] = 85.278931;
            table[18, 1] = 84.867101;
            table[19, 1] = 84.474556;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.417326;
            table[31, 1] = 72.81016;
            table[32, 1] = 72.231168;
            table[33, 1] = 71.676959;
            table[34, 1] = 71.144651;
            table[35, 1] = 70.631767;
            table[36, 1] = 69.655968;
            table[37, 1] = 68.737006;
            table[38, 1] = 67.870185;
            table[39, 1] = 67.042965;
            table[40, 1] = 66.248898;
            table[41, 1] = 65.482818;
            table[42, 1] = 64.740558;
            table[43, 1] = 64.018753;
            table[44, 1] = 63.314686;
            table[45, 1] = 62.626176;
            table[46, 1] = 60.962469;
            table[47, 1] = 59.36806;
            table[48, 1] = 57.834335;
            table[49, 1] = 56.35803;
            table[50, 1] = 54.937829;
            table[51, 1] = 53.571877;
            table[52, 1] = 52.256465;
            table[53, 1] = 50.985894;
            table[54, 1] = 49.753108;
            table[55, 1] = 48.550642;
            table[56, 1] = 47.371499;
            table[57, 1] = 46.209744;
            table[58, 1] = 45.060779;
            table[59, 1] = 43.921341;
            table[60, 1] = 42.789324;
            table[61, 1] = 41.6635;
            table[62, 1] = 40.54324;
            table[63, 1] = 39.428255;
            table[64, 1] = 38.31841;
            table[65, 1] = 37.213602;
            table[66, 1] = 36.113704;
            table[67, 1] = 35.018559;
            table[68, 1] = 33.928003;
            table[69, 1] = 32.841922;
            table[70, 1] = 31.760307;
            table[71, 1] = 30.683308;
            table[72, 1] = 29.611291;
            table[73, 1] = 28.54487;
            table[74, 1] = 27.484937;
            table[75, 1] = 26.432671;
            table[76, 1] = 25.389529;
            table[77, 1] = 24.357236;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_1t_600h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170112;
            table[14, 1] = 86.654376;
            table[15, 1] = 86.169691;
            table[16, 1] = 85.712255;
            table[17, 1] = 85.278931;
            table[18, 1] = 84.867101;
            table[19, 1] = 84.474556;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.417326;
            table[31, 1] = 72.81016;
            table[32, 1] = 72.231168;
            table[33, 1] = 71.676959;
            table[34, 1] = 71.144651;
            table[35, 1] = 70.631767;
            table[36, 1] = 69.655968;
            table[37, 1] = 68.737006;
            table[38, 1] = 67.870185;
            table[39, 1] = 67.042965;
            table[40, 1] = 66.248898;
            table[41, 1] = 65.482818;
            table[42, 1] = 64.740558;
            table[43, 1] = 64.018753;
            table[44, 1] = 63.314686;
            table[45, 1] = 62.626176;
            table[46, 1] = 60.962469;
            table[47, 1] = 59.36806;
            table[48, 1] = 57.834335;
            table[49, 1] = 56.35803;
            table[50, 1] = 54.937829;
            table[51, 1] = 53.571877;
            table[52, 1] = 52.256465;
            table[53, 1] = 50.985894;
            table[54, 1] = 49.753108;
            table[55, 1] = 48.550642;
            table[56, 1] = 47.371499;
            table[57, 1] = 46.209744;
            table[58, 1] = 45.060779;
            table[59, 1] = 43.921341;
            table[60, 1] = 42.789324;
            table[61, 1] = 41.6635;
            table[62, 1] = 40.54324;
            table[63, 1] = 39.428255;
            table[64, 1] = 38.31841;
            table[65, 1] = 37.213602;
            table[66, 1] = 36.113704;
            table[67, 1] = 35.018559;
            table[68, 1] = 33.928003;
            table[69, 1] = 32.841922;
            table[70, 1] = 31.760307;
            table[71, 1] = 30.683308;
            table[72, 1] = 29.611291;
            table[73, 1] = 28.54487;
            table[74, 1] = 27.484937;
            table[75, 1] = 26.432671;
            table[76, 1] = 25.389529;
            table[77, 1] = 24.357236;

            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        private double Get_sea_2000m_1t_1200h(double d)
        {

            double[,] table = new double[78, 2];
            table[0, 0] = 1;
            table[1, 0] = 2;
            table[2, 0] = 3;
            table[3, 0] = 4;
            table[4, 0] = 5;
            table[5, 0] = 6;
            table[6, 0] = 7;
            table[7, 0] = 8;
            table[8, 0] = 9;
            table[9, 0] = 10;
            table[10, 0] = 11;
            table[11, 0] = 12;
            table[12, 0] = 13;
            table[13, 0] = 14;
            table[14, 0] = 15;
            table[15, 0] = 16;
            table[16, 0] = 17;
            table[17, 0] = 18;
            table[18, 0] = 19;
            table[19, 0] = 20;
            table[20, 0] = 25;
            table[21, 0] = 30;
            table[22, 0] = 35;
            table[23, 0] = 40;
            table[24, 0] = 45;
            table[25, 0] = 50;
            table[26, 0] = 55;
            table[27, 0] = 60;
            table[28, 0] = 65;
            table[29, 0] = 70;
            table[30, 0] = 75;
            table[31, 0] = 80;
            table[32, 0] = 85;
            table[33, 0] = 90;
            table[34, 0] = 95;
            table[35, 0] = 100;
            table[36, 0] = 110;
            table[37, 0] = 120;
            table[38, 0] = 130;
            table[39, 0] = 140;
            table[40, 0] = 150;
            table[41, 0] = 160;
            table[42, 0] = 170;
            table[43, 0] = 180;
            table[44, 0] = 190;
            table[45, 0] = 200;
            table[46, 0] = 225;
            table[47, 0] = 250;
            table[48, 0] = 275;
            table[49, 0] = 300;
            table[50, 0] = 325;
            table[51, 0] = 350;
            table[52, 0] = 375;
            table[53, 0] = 400;
            table[54, 0] = 425;
            table[55, 0] = 450;
            table[56, 0] = 475;
            table[57, 0] = 500;
            table[58, 0] = 525;
            table[59, 0] = 550;
            table[60, 0] = 575;
            table[61, 0] = 600;
            table[62, 0] = 625;
            table[63, 0] = 650;
            table[64, 0] = 675;
            table[65, 0] = 700;
            table[66, 0] = 725;
            table[67, 0] = 750;
            table[68, 0] = 775;
            table[69, 0] = 800;
            table[70, 0] = 825;
            table[71, 0] = 850;
            table[72, 0] = 875;
            table[73, 0] = 900;
            table[74, 0] = 925;
            table[75, 0] = 950;
            table[76, 0] = 975;
            table[77, 0] = 1000;

            table[0, 1] = 107.266;
            table[1, 1] = 101.61258;
            table[2, 1] = 98.506644;
            table[3, 1] = 96.317427;
            table[4, 1] = 94.652783;
            table[5, 1] = 93.313764;
            table[6, 1] = 92.193549;
            table[7, 1] = 91.229284;
            table[8, 1] = 90.38111;
            table[9, 1] = 89.62233;
            table[10, 1] = 88.934298;
            table[11, 1] = 88.303551;
            table[12, 1] = 87.720103;
            table[13, 1] = 87.170112;
            table[14, 1] = 86.654376;
            table[15, 1] = 86.169691;
            table[16, 1] = 85.712255;
            table[17, 1] = 85.278931;
            table[18, 1] = 84.867101;
            table[19, 1] = 84.474556;
            table[20, 1] = 82.737978;
            table[21, 1] = 81.260065;
            table[22, 1] = 79.981556;
            table[23, 1] = 78.856258;
            table[24, 1] = 77.852952;
            table[25, 1] = 76.949089;
            table[26, 1] = 76.127686;
            table[27, 1] = 75.375603;
            table[28, 1] = 74.682469;
            table[29, 1] = 74.03998;
            table[30, 1] = 73.417326;
            table[31, 1] = 72.81016;
            table[32, 1] = 72.231168;
            table[33, 1] = 71.676959;
            table[34, 1] = 71.144651;
            table[35, 1] = 70.631767;
            table[36, 1] = 69.655968;
            table[37, 1] = 68.737006;
            table[38, 1] = 67.870185;
            table[39, 1] = 67.042965;
            table[40, 1] = 66.248898;
            table[41, 1] = 65.482818;
            table[42, 1] = 64.740558;
            table[43, 1] = 64.018753;
            table[44, 1] = 63.314686;
            table[45, 1] = 62.626176;
            table[46, 1] = 60.962469;
            table[47, 1] = 59.36806;
            table[48, 1] = 57.834335;
            table[49, 1] = 56.35803;
            table[50, 1] = 54.937829;
            table[51, 1] = 53.571877;
            table[52, 1] = 52.256465;
            table[53, 1] = 50.985894;
            table[54, 1] = 49.753108;
            table[55, 1] = 48.550642;
            table[56, 1] = 47.371499;
            table[57, 1] = 46.209744;
            table[58, 1] = 45.060779;
            table[59, 1] = 43.921341;
            table[60, 1] = 42.789324;
            table[61, 1] = 41.6635;
            table[62, 1] = 40.54324;
            table[63, 1] = 39.428255;
            table[64, 1] = 38.31841;
            table[65, 1] = 37.213602;
            table[66, 1] = 36.113704;
            table[67, 1] = 35.018559;
            table[68, 1] = 33.928003;
            table[69, 1] = 32.841922;
            table[70, 1] = 31.760307;
            table[71, 1] = 30.683308;
            table[72, 1] = 29.611291;
            table[73, 1] = 28.54487;
            table[74, 1] = 27.484937;
            table[75, 1] = 26.432671;
            table[76, 1] = 25.389529;
            table[77, 1] = 24.357236;
            double E = 0;
            E = table[0, 1];
            if (d < 1)
            {
                return E;
            }
            for (int i = 1; i <= 77; i++)
            {
                if (d > table[i - 1, 0] && d <= table[i, 0])
                {
                    E = table[i - 1, 1] + (table[i, 1] - table[i - 1, 1]) * Math.Log10(d / table[i - 1, 0]) / Math.Log10(table[i, 0] / table[i - 1, 0]);
                    break;
                }

            }

            return E;
        }
        // расчёт напряжённости для трассы длиной до 100 м в 50% времени
        private double Get_land_100m_50t(double h, double d, double f)
        {
            double E = -9999;

            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_100m_50t_10h(12.965338406690355) - Get_land_100m_50t_10h(dH) + Get_land_100m_50t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_100m_50t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }

            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_100m_50t_20h(d);
                Esup = Get_land_100m_50t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_100m_50t_38h(d);
                Esup = Get_land_100m_50t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_100m_50t_75h(d);
                Esup = Get_land_100m_50t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_100m_50t_150h(d);
                Esup = Get_land_100m_50t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_100m_50t_300h(d);
                Esup = Get_land_100m_50t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_100m_50t_600h(d);
            Esup_ = Get_land_100m_50t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_100m_10t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_100m_10t_10h(12.965338406690355) - Get_land_100m_10t_10h(dH) + Get_land_100m_10t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_100m_10t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }

            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_land_100m_10t_10h(d);
                Esup = Get_land_100m_10t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_100m_10t_20h(d);
                Esup = Get_land_100m_10t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_100m_10t_38h(d);
                Esup = Get_land_100m_10t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_100m_10t_75h(d);
                Esup = Get_land_100m_10t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_100m_10t_150h(d);
                Esup = Get_land_100m_10t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_100m_10t_300h(d);
                Esup = Get_land_100m_10t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_100m_10t_600h(d);
            Esup_ = Get_land_100m_10t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_100m_1t(double h, double d, double f)
        {
            double E = -9999;

            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_100m_1t_10h(12.965338406690355) - Get_land_100m_1t_10h(dH) + Get_land_100m_1t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_100m_1t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_land_100m_1t_10h(d);
                Esup = Get_land_100m_1t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_100m_1t_20h(d);
                Esup = Get_land_100m_1t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_100m_1t_38h(d);
                Esup = Get_land_100m_1t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_100m_1t_75h(d);
                Esup = Get_land_100m_1t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_100m_1t_150h(d);
                Esup = Get_land_100m_1t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_100m_1t_300h(d);
                Esup = Get_land_100m_1t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_100m_1t_600h(d);
            Esup_ = Get_land_100m_1t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_100m_50t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                //f = 100 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 50.0);//p = 50.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 50.0); //p = 50.0
                    E10 = Get_land_100m_50t_10h(D20);
                    E20 = Get_land_100m_50t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 1.35; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_100m_50t_10h(d);
                    E20 = Get_land_100m_50t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_100m_50t_10h(d);
                Esup = Get_sea_100m_50t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_50t_20h(d);
                Esup = Get_sea_100m_50t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_50t_38h(d);
                Esup = Get_sea_100m_50t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_50t_75h(d);
                Esup = Get_sea_100m_50t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_50t_150h(d);
                Esup = Get_sea_100m_50t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_50t_300h(d);
                Esup = Get_sea_100m_50t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_100m_50t_600h(d);
            Esup_ = Get_sea_100m_50t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_100m_10t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                //f = 100 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 10.0);//p = 10.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 10.0); //p = 10.0
                    E10 = Get_land_100m_10t_10h(D20);
                    E20 = Get_land_100m_10t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 1.35; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_100m_10t_10h(d);
                    E20 = Get_land_100m_10t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_100m_10t_10h(d);
                Esup = Get_sea_100m_10t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_10t_20h(d);
                Esup = Get_sea_100m_10t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_10t_38h(d);
                Esup = Get_sea_100m_10t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_10t_75h(d);
                Esup = Get_sea_100m_10t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_10t_150h(d);
                Esup = Get_sea_100m_10t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_10t_300h(d);
                Esup = Get_sea_100m_10t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_100m_10t_600h(d);
            Esup_ = Get_sea_100m_10t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_100m_1t(double h, double d, double f)
        {
            double E = -9999;

            if (h < 10)
            {
                //f = 100 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 1.0);//p = 1.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 1.0); //p = 1.0
                    E10 = Get_land_100m_1t_10h(D20);
                    E20 = Get_land_100m_1t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 1.35; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_100m_1t_10h(d);
                    E20 = Get_land_100m_1t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_100m_1t_10h(d);
                Esup = Get_sea_100m_1t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_1t_20h(d);
                Esup = Get_sea_100m_1t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_1t_38h(d);
                Esup = Get_sea_100m_1t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_1t_75h(d);
                Esup = Get_sea_100m_1t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_1t_150h(d);
                Esup = Get_sea_100m_1t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_1t_300h(d);
                Esup = Get_sea_100m_1t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_100m_1t_600h(d);
            Esup_ = Get_sea_100m_1t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_600m_50t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_600m_50t_10h(12.965338406690355) - Get_land_600m_50t_10h(dH) + Get_land_600m_50t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_600m_50t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }

            if (h >= 0 && h < 10)
            {
                double Kv = 3.31;
                double teta_eff = 0.063661951;
                double v;
                v = Kv * teta_eff;
                double Jv;
                Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                double Chneg;
                Chneg = 6.03 - Jv;
                double E10, E20, C1020, Ezero;
                E10 = Get_land_600m_50t_10h(d);
                E20 = Get_land_600m_50t_20h(d);
                C1020 = E10 - E20;
                Ezero = E10 + 0.5 * (C1020 + Chneg);
                E = Ezero + 0.1 * h * (E10 - Ezero);
                return E;
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_land_600m_50t_10h(d);
                Esup = Get_land_600m_50t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_600m_50t_20h(d);
                Esup = Get_land_600m_50t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_600m_50t_38h(d);
                Esup = Get_land_600m_50t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_600m_50t_75h(d);
                Esup = Get_land_600m_50t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_600m_50t_150h(d);
                Esup = Get_land_600m_50t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_600m_50t_300h(d);
                Esup = Get_land_600m_50t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_600m_50t_600h(d);
            Esup_ = Get_land_600m_50t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_600m_10t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_600m_10t_10h(12.965338406690355) - Get_land_600m_10t_10h(dH) + Get_land_600m_10t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_600m_10t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_land_600m_10t_10h(d);
                Esup = Get_land_600m_10t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_600m_10t_20h(d);
                Esup = Get_land_600m_10t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_600m_10t_38h(d);
                Esup = Get_land_600m_10t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_600m_10t_75h(d);
                Esup = Get_land_600m_10t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_600m_10t_150h(d);
                Esup = Get_land_600m_10t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_600m_10t_300h(d);
                Esup = Get_land_600m_10t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_600m_10t_600h(d);
            Esup_ = Get_land_600m_10t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_600m_1t(double h, double d, double f)
        {
            double E = -9999;

            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_600m_1t_10h(12.965338406690355) - Get_land_600m_1t_10h(dH) + Get_land_600m_1t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_600m_1t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_land_600m_1t_10h(d);
                Esup = Get_land_600m_1t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_600m_1t_20h(d);
                Esup = Get_land_600m_1t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_600m_1t_38h(d);
                Esup = Get_land_600m_1t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_600m_1t_75h(d);
                Esup = Get_land_600m_1t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_600m_1t_150h(d);
                Esup = Get_land_600m_1t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_600m_1t_300h(d);
                Esup = Get_land_600m_1t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_600m_1t_600h(d);
            Esup_ = Get_land_600m_1t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_600m_50t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                //f = 600 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 50.0);//p = 50.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 50.0); //p = 50.0
                    E10 = Get_land_600m_50t_10h(D20);
                    E20 = Get_land_600m_50t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 3.31; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_600m_50t_10h(d);
                    E20 = Get_land_600m_50t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_600m_50t_10h(d);
                Esup = Get_sea_600m_50t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_50t_20h(d);
                Esup = Get_sea_600m_50t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_50t_38h(d);
                Esup = Get_sea_600m_50t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_50t_75h(d);
                Esup = Get_sea_600m_50t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_50t_150h(d);
                Esup = Get_sea_600m_50t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_50t_300h(d);
                Esup = Get_sea_600m_50t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_600m_50t_600h(d);
            Esup_ = Get_sea_600m_50t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_600m_10t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                //f = 600 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 10.0);//p = 10.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 10.0); //p = 10.0
                    E10 = Get_land_600m_10t_10h(D20);
                    E20 = Get_land_600m_10t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 3.31; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_600m_10t_10h(d);
                    E20 = Get_land_600m_10t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_600m_10t_10h(d);
                Esup = Get_sea_600m_10t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_10t_20h(d);
                Esup = Get_sea_600m_10t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_10t_38h(d);
                Esup = Get_sea_600m_10t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_10t_75h(d);
                Esup = Get_sea_600m_10t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_10t_150h(d);
                Esup = Get_sea_600m_10t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_10t_300h(d);
                Esup = Get_sea_600m_10t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_600m_10t_600h(d);
            Esup_ = Get_sea_600m_10t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_600m_1t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                //f = 600 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 1.0);//p = 1.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 1.0); //p = 1.0
                    E10 = Get_land_600m_1t_10h(D20);
                    E20 = Get_land_600m_1t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 3.31; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_600m_1t_10h(d);
                    E20 = Get_land_600m_1t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }

            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_600m_1t_10h(d);
                Esup = Get_sea_600m_1t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_1t_20h(d);
                Esup = Get_sea_600m_1t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_1t_38h(d);
                Esup = Get_sea_600m_1t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_1t_75h(d);
                Esup = Get_sea_600m_1t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_1t_150h(d);
                Esup = Get_sea_600m_1t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_600m_1t_300h(d);
                Esup = Get_sea_600m_1t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_600m_1t_600h(d);
            Esup_ = Get_sea_600m_1t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_2000m_50t(double h, double d, double f)
        {
            double E = -9999;

            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_2000m_50t_10h(12.965338406690355) - Get_land_2000m_50t_10h(dH) + Get_land_2000m_50t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_2000m_50t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_land_2000m_50t_10h(d);
                Esup = Get_land_2000m_50t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_50t_20h(d);
                Esup = Get_land_2000m_50t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_50t_38h(d);
                Esup = Get_land_2000m_50t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_50t_75h(d);
                Esup = Get_land_2000m_50t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_50t_150h(d);
                Esup = Get_land_2000m_50t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_50t_300h(d);
                Esup = Get_land_2000m_50t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_2000m_50t_600h(d);
            Esup_ = Get_land_2000m_50t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_2000m_10t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_2000m_10t_10h(12.965338406690355) - Get_land_2000m_10t_10h(dH) + Get_land_2000m_10t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_2000m_10t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_land_2000m_10t_10h(d);
                Esup = Get_land_2000m_10t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_10t_20h(d);
                Esup = Get_land_2000m_10t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_10t_38h(d);
                Esup = Get_land_2000m_10t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_10t_75h(d);
                Esup = Get_land_2000m_10t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_10t_150h(d);
                Esup = Get_land_2000m_10t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_10t_300h(d);
                Esup = Get_land_2000m_10t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_2000m_10t_600h(d);
            Esup_ = Get_land_2000m_10t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_2000m_1t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                double Cneg = 0;
                if (h < 0)
                {
                    double thetaEff = Math.Atan(-h / 9000) * 180.0 / Math.PI;
                    double v_ = 0.036 * Math.Sqrt(f);
                    double v = 0.065 * thetaEff * Math.Sqrt(f);
                    //double Jv_ = 6.9 + 20 * Math.Log10(v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1));
                    //double Jv = 6.9 + 20 * Math.Log10(v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1));
                    Cneg = 20 * Math.Log10((v_ - 0.1 + Math.Sqrt((v_ - 0.1) * (v_ - 0.1) + 1)) / (v - 0.1 + Math.Sqrt((v - 0.1) * (v - 0.1) + 1)));
                    h = 0;
                }

                double dH = 4.1 * Math.Sqrt(h);

                if (d < dH)
                {
                    E = Get_land_2000m_1t_10h(12.965338406690355) - Get_land_2000m_1t_10h(dH) + Get_land_2000m_1t_10h(d);
                }
                if (d > dH)
                {
                    E = Get_land_2000m_1t_10h(12.965338406690355 + d - dH);
                }

                if (Cneg < 0)
                {
                    E += Cneg;
                }
                return E;
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_land_2000m_1t_10h(d);
                Esup = Get_land_2000m_1t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_1t_20h(d);
                Esup = Get_land_2000m_1t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_1t_38h(d);
                Esup = Get_land_2000m_1t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_1t_75h(d);
                Esup = Get_land_2000m_1t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_1t_150h(d);
                Esup = Get_land_2000m_1t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_land_2000m_1t_300h(d);
                Esup = Get_land_2000m_1t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_2000m_1t_600h(d);
            Esup_ = Get_land_2000m_1t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_2000m_50t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                //f = 2000 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 50.0);//p = 50.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 50.0); //p = 50.0
                    E10 = Get_land_2000m_50t_10h(D20);
                    E20 = Get_land_2000m_50t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 6; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_2000m_50t_10h(d);
                    E20 = Get_land_2000m_50t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }

            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_50t_10h(d);
                Esup = Get_sea_2000m_50t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_50t_20h(d);
                Esup = Get_sea_2000m_50t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_50t_38h(d);
                Esup = Get_sea_2000m_50t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_50t_75h(d);
                Esup = Get_sea_2000m_50t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_50t_150h(d);
                Esup = Get_sea_2000m_50t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_50t_300h(d);
                Esup = Get_sea_2000m_50t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_2000m_50t_600h(d);
            Esup_ = Get_sea_2000m_50t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_2000m_10t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                //f = 2000 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 10.0);//p = 10.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 10.0); //p = 10.0
                    E10 = Get_land_2000m_10t_10h(D20);
                    E20 = Get_land_2000m_10t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 6; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_2000m_10t_10h(d);
                    E20 = Get_land_2000m_10t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }

            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_10t_10h(d);
                Esup = Get_sea_2000m_10t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_10t_20h(d);
                Esup = Get_sea_2000m_10t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_10t_38h(d);
                Esup = Get_sea_2000m_10t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_10t_75h(d);
                Esup = Get_sea_2000m_10t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_10t_150h(d);
                Esup = Get_sea_2000m_10t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_10t_300h(d);
                Esup = Get_sea_2000m_10t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_2000m_10t_600h(d);
            Esup_ = Get_sea_2000m_10t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_sea_2000m_1t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                //f = 2000 MHz
                double Dh1 = D06(f, h, 10);
                double D20 = D06(f, 20, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, 1.0);//p = 1.0
                    return E;
                }
                else if (d > Dh1 && d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    Edh1 = Emax_sea(Dh1, 1.0); //p = 1.0
                    E10 = Get_land_2000m_1t_10h(D20);
                    E20 = Get_land_2000m_1t_20h(D20);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    E = Edh1 + (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double Kv = 6; //f = 100 MHz => Kv = 1.35 ;; f = 600 MHz => Kv = 3.31 ;; f = 2000 MHz => Kv = 6.00 ;; 
                    double teta_eff = 0.063661951; // arctg(h / 9000), h = 10
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double E_, E__, Fs, E10, E20, C1020, Ezero;
                    E10 = Get_land_2000m_1t_10h(d);
                    E20 = Get_land_2000m_1t_20h(d);
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);

                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) * 3.321928094887362; // const is 1 / lg(20/10)
                    Fs = (d - D20) / d;

                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }
            if ((h >= 10) && (h < 20))
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_1t_10h(d);
                Esup = Get_sea_2000m_1t_20h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                return E;
            }
            if (h < 37.5)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_1t_20h(d);
                Esup = Get_sea_2000m_1t_38h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 20) / Math.Log10(37.5 / 20);
                return E;
            }
            if (h < 75)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_1t_38h(d);
                Esup = Get_sea_2000m_1t_75h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 37.5) / Math.Log10(75 / 37.5);
                return E;
            }
            if (h < 150)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_1t_75h(d);
                Esup = Get_sea_2000m_1t_150h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 75) / Math.Log10(150 / 75);
                return E;
            }
            if (h < 300)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_1t_150h(d);
                Esup = Get_sea_2000m_1t_300h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 150) / Math.Log10(300 / 150);
                return E;
            }
            if (h < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_2000m_1t_300h(d);
                Esup = Get_sea_2000m_1t_600h(d);
                E = Einf + (Esup - Einf) * Math.Log10(h / 300) / Math.Log10(600 / 300);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_2000m_1t_600h(d);
            Esup_ = Get_sea_2000m_1t_1200h(d);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(h / 600) / Math.Log10(1200 / 600);
            return E;
        }
        private double Get_land_50t(double h, double d, double f)
        {
            double E = -9999;
            if (f < 600)
            {
                double Einf, Esup;
                Einf = Get_land_100m_50t(h, d, f);
                Esup = Get_land_600m_50t(h, d, f);
                E = Einf + (Esup - Einf) * Math.Log10(f / 100) / Math.Log10(600 / 100);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_600m_50t(h, d, f);
            Esup_ = Get_land_2000m_50t(h, d, f);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(f / 600) / Math.Log10(2000.0 / 600);
            return E;
        }
        private double Get_land_10t(double h, double d, double f)
        {
            double E = -9999;
            if (f < 600)
            {
                double Einf, Esup;
                Einf = Get_land_100m_10t(h, d, f);
                Esup = Get_land_600m_10t(h, d, f);
                E = Einf + (Esup - Einf) * Math.Log10(f / 100) / Math.Log10(600 / 100);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_600m_10t(h, d, f);
            Esup_ = Get_land_2000m_10t(h, d, f);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(f / 600) / Math.Log10(2000.0 / 600);
            return E;
        }
        private double Get_land_1t(double h, double d, double f)
        {
            double E = -9999;
            if (f < 600)
            {
                double Einf, Esup;
                Einf = Get_land_100m_1t(h, d, f);
                Esup = Get_land_600m_1t(h, d, f);
                E = Einf + (Esup - Einf) * Math.Log10(f / 100) / Math.Log10(600 / 100);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_land_600m_1t(h, d, f);
            Esup_ = Get_land_2000m_1t(h, d, f);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(f / 600) / Math.Log10(2000.0 / 600);
            return E;
        }
        private double Get_sea_50t(double h, double d, double f)
        {
            double E = -9999;
            if (h < 10)
            {
                double Dh1;
                Dh1 = D06(f, h, 10);
            }
            if (f < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_50t(h, d, f);
                Esup = Get_sea_600m_50t(h, d, f);
                E = Einf + (Esup - Einf) * Math.Log10(f / 100) / Math.Log10(600 / 100);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_600m_50t(h, d, f);
            Esup_ = Get_sea_2000m_50t(h, d, f);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(f / 600) / Math.Log10(2000.0 / 600);
            return E;
        }
        private double Get_sea_10t(double h, double d, double f)
        {
            double E = -9999;
            if (f < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_10t(h, d, f);
                Esup = Get_sea_600m_10t(h, d, f);
                E = Einf + (Esup - Einf) * Math.Log10(f / 100) / Math.Log10(600 / 100);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_600m_10t(h, d, f);
            Esup_ = Get_sea_2000m_10t(h, d, f);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(f / 600) / Math.Log10(2000.0 / 600);
            return E;
        }
        private double Get_sea_1t(double h, double d, double f)
        {
            double E = -9999;
            if (f < 600)
            {
                double Einf, Esup;
                Einf = Get_sea_100m_1t(h, d, f);
                Esup = Get_sea_600m_1t(h, d, f);
                E = Einf + (Esup - Einf) * Math.Log10(f / 100) / Math.Log10(600 / 100);
                return E;
            }
            double Einf_, Esup_;
            Einf_ = Get_sea_600m_1t(h, d, f);
            Esup_ = Get_sea_2000m_1t(h, d, f);
            E = Einf_ + (Esup_ - Einf_) * Math.Log10(f / 600) / Math.Log10(2000.0 / 600);
            return E;
        }
        private double Q(double x)
        {
            double C0 = 2.515517;
            double C1 = 0.802853;
            double C2 = 0.010328;
            double D1 = 1.432788;
            double D2 = 0.189269;
            double D3 = 0.001308;
            if ((x > 0.99) || (x < 0.01))
            {
                return -9999;
            }
            if (x <= 0.5)
            {
                double T;
                T = Math.Pow(-2 * Math.Log(x, Math.E), 0.5);
                double ksi = 0;
                ksi = ((C2 * T + C1) * T + C0) / (((D3 * T + D2) * T + D1) * T + 1);
                double Qi = 0;
                Qi = T - ksi;
                return Qi;
            }
            else
            {
                double T;
                T = Math.Pow(-2 * Math.Log(1 - x, Math.E), 0.5);
                double ksi = 0;
                ksi = ((C2 * T + C1) * T + C0) / (((D3 * T + D2) * T + D1) * T + 1);
                double Qi = 0;
                Qi = -T + ksi;
                return Qi;
            }

        }
        private double Get_land1(double h, double d, double f, double p)
        {
            double E = -9999;
            if (p < 10)
            {
                double Einf, Esup;
                Einf = Get_land_1t(h, d, f);
                Esup = Get_land_10t(h, d, f);
                double Qinf, Qsup, Qt;
                Qt = Q(p / 100);
                Qinf = Q(0.01);
                Qsup = Q(0.1);
                E = Esup * (Qinf - Qt) / (Qinf - Qsup) + Einf * (Qt - Qsup) / (Qinf - Qsup);
                return E;
            }
            else
            {
                double Einf, Esup;
                Einf = Get_land_10t(h, d, f);
                Esup = Get_land_50t(h, d, f);
                double Qinf, Qsup, Qt;
                Qt = Q(p / 100);
                Qinf = Q(0.1);
                Qsup = Q(0.5);
                E = Esup * (Qinf - Qt) / (Qinf - Qsup) + Einf * (Qt - Qsup) / (Qinf - Qsup);
                return E;
            }
        }
        private double Get_sea1(double h, double d, double f, double p)
        {
            double E = -9999;
            if (p < 10)
            {
                double Einf, Esup;
                Einf = Get_sea_1t(h, d, f);
                Esup = Get_sea_10t(h, d, f);
                double Qinf, Qsup, Qt;
                Qt = Q(p / 100);
                Qinf = Q(0.01);
                Qsup = Q(0.1);
                E = Esup * (Qinf - Qt) / (Qinf - Qsup) + Einf * (Qt - Qsup) / (Qinf - Qsup);
                return E;
            }
            else
            {
                double Einf, Esup;
                Einf = Get_sea_10t(h, d, f);
                Esup = Get_sea_50t(h, d, f);
                double Qinf, Qsup, Qt;
                Qt = Q(p / 100);
                Qinf = Q(0.1);
                Qsup = Q(0.5);
                E = Esup * (Qinf - Qt) / (Qinf - Qsup) + Einf * (Qt - Qsup) / (Qinf - Qsup);
                return E;
            }
        }
        // Аппроксимация длины трассыс просвето мв 0,6 зоны Френеля; ур. 38 для расчёта поправки антенны
        private double D06(double f, double h1, double h2)
        {
            double Df, Dh, D;
            Df = 0.0000389 * f * h1 * h2;
            Dh = 4.1 * (Math.Pow(h1, 0.5) + Math.Pow(h2, 0.5));
            D = Df * Dh / (Df + Dh);
            return D;
        }
        private double Emax_land(double d)
        {
            double E;
            E = 106.9 - 20 * Math.Log10(d);
            return E;
        }
        private double Emax_sea(double d, double p)
        {
            double E;
            E = 2.38 * (1 - Math.Exp(-d / 8.94)) * Math.Log10(50 / p);
            E = E + Emax_land(d);
            return E;
        }
        private double Get_land(double ha, double hef, double d, double f, double p)
        {
            if (d <= 3)
            {
                double h;
                double E;
                h = ha;
                E = Get_land1(h, d, f, p);
                return E;
            }
            if ((d > 3) && (d < 15))
            {
                double h;
                double E;
                h = ha + (hef - ha) * (d - 3) / 12;
                E = Get_land1(h, d, f, p);
                return E;
            }
            else
            {
                double h;
                double E;
                h = hef;
                E = Get_land1(h, d, f, p);
                return E;
            }

        }
        private double Get_sea(double ha, double d, double f, double p)
        {
            double h;
            double E;
            if (ha < 1)
            {
                h = 1.0;
            }
            else
            {
                h = ha;
            }
            if (h >= 10)
            {
                E = Get_sea1(h, d, f, p);
            }
            else
            {
                // в данном случае рассматриваем морскую трассу с высотой от 1 до 10 м.
                double Dh1;
                Dh1 = D06(f, h, 10);
                if (d <= Dh1)
                {
                    E = Emax_sea(d, p);
                    return E;
                }
                double D20;
                D20 = D06(f, 20, 10);
                if (d < D20)
                {
                    double Edh1, E10, E20, Ed20;
                    E10 = Get_sea1(10, D20, f, p);
                    E20 = Get_sea1(20, D20, f, p);
                    Ed20 = E10 + (E20 - E10) * Math.Log10(h / 10) / Math.Log10(20 / 10);
                    Edh1 = Emax_sea(Dh1, p);
                    E = (Ed20 - Edh1) * Math.Log10(d / Dh1) / Math.Log10(D20 / Dh1);
                    return E;
                }
                else
                {
                    double E_, E__, Fs, E10, E20;
                    Fs = (d - D20) / d;
                    E10 = Get_sea1(10, d, f, p);
                    E20 = Get_sea1(20, d, f, p);
                    E_ = E10 + (E20 - E10) * Math.Log10(h / 10) / Math.Log10(20 / 10);

                    // calc E__
                    double Kv_100 = 1.35;
                    double Kv_600 = 3.31;
                    double Kv_2000 = 6;
                    double Kv;
                    if (f < 600)
                    {
                        Kv = Kv_100 + (Kv_600 - Kv_100) * Math.Log10(f / 100) / Math.Log10(600 / 100);
                    }
                    else
                    {
                        Kv = Kv_600 + (Kv_2000 - Kv_600) * Math.Log10(f / 600) / Math.Log10(2000 / 600);
                    }

                    double teta_eff = 0.063661951;
                    double v;
                    v = Kv * teta_eff;
                    double Jv;
                    // 1546-5, 1546-6; app5; 4.3
                    if (v > -0.7806)
                    {
                        Jv = 6.9 + 20 * Math.Log10(Math.Pow((v - 0.1) * (v - 0.1) + 1, 0.5) + v - 0.1);
                    }
                    else
                    {
                        Jv = 0;
                    }//
                    double Chneg;
                    Chneg = 6.03 - Jv;
                    double C1020, Ezero;
                    C1020 = E10 - E20;
                    Ezero = E10 + 0.5 * (C1020 + Chneg);
                    E__ = Ezero + 0.1 * h * (E10 - Ezero);
                    E = E_ * (1 - Fs) + E__ * Fs;
                    return E;
                }
            }
            return E;
        }
        private double Get_E_(double ha, double hef, double d, double f, double p, double h_gr, params LandSea[] list1)
        {
            double E;
            double dT, dsT, delta, dlT;
            // определение длины трассы по морю, и суше, ур 24a, 24b
            dsT = 0;
            dlT = 0;
            for (int i = 0; i < list1.Length; i++)
            {
                dsT = dsT + list1[i].sea;
                dlT = dlT + list1[i].land;
            }
            // если трасса только по суше;
            if (dsT < 0.1)
            {
                E = Get_land(ha, hef, d, f, p);
                return E;
            }

            // если трасса только по воде;
            if (dlT < 0.1)
            {
                E = Get_sea(h_gr, d, f, p);
                return E;
            }
            // если трасса по суше +  море;
            // ур 24c
            dT = dlT + dsT;


            var E_land = Get_land(ha, hef, d, f, p);
            var E_sea = Get_sea(h_gr, d, f, p);

            // определение Дельта, ур 26
            delta = E_sea - E_land;

            double v = Math.Max(1, 1 + delta / 40);

            // доля трасы, которая проходить над моерм, ур. 25
            double Fs;
            Fs = dsT / dT;

            double A0;
            A0 = 1 - Math.Pow(1 - Fs, 2.0 / 3.0);
            double A;
            A = Math.Pow(A0, v);
            E = (1 - A) * E_land + A * E_sea;

            return E;
        }
        /// <summary>
        ///  Модель 1546_4 
        ///  Модель в себя не включает (по сравнению с регламентированной):
        ///  - поправку на тропосферное расеивание
        ///  - прправку на просвет местности
        ///  - поправку на углы закрытия
        ///  - поправку на процент територри
        ///  - поправку на распространения на частотах менее 100МГц для моря
        ///  - поправку прием с вытотой менее 10м в условиях моря
        /// </summary>
        /// <param name="ha">высота антенны над поверхностью земли, м (от -200 до 3000м)</param>
        /// <param name="hef">эффективня высота антенны, м  (от -1000 до 3000м) при отсутсвии вставлять значение ha </param>
        /// <param name="d">длинна трассы, км (от 0.1 до 1000км) </param>
        /// <param name="f">частота, МГц (от 30 до 3000МГц)</param>
        /// <param name="p">процент времени (от 1% до 50%)</param>
        /// <param name="h_gr">выоста антенны над уровнем моря, м (от -200 до 3000) используется только в морских трассах</param>
        /// <param name="h2">высота антенны абонента, м (по умолчанию 10м)</param>
        /// <param name="list1">Суша - вода</param>
        /// <returns>напряженность поля в дБ(мкВ/м)</returns>
        public static double Get_E(double ha, double hef, double d, double f, double p, double h_gr, double h2, bool h2aboveSea, params LandSea[] list1)

        {
            // проверка входных данных
            double h2_, ha_, hef_, d_, f_, p_, h_gr_;
            // 
            ha_ = ha;
            hef_ = hef;
            d_ = d;
            f_ = f;
            p_ = p;
            h_gr_ = h_gr;
            h2_ = h2;

            // p 15 short distances


            if (d_ < 1)
            {
                double E;
                if (d_ <= 0.04)
                {
                    double dslope = Math.Sqrt(d * d + 0.000001 * Math.Pow((ha - h2), 2));
                    E = 106.9 - 20 * Math.Log10(dslope);
                    return E;
                }
                else
                {
                    d_ = 1.0;
                }


            }
            if (d_ > 1000)
            {
                d_ = 1000;
            }
            if (f_ < 30)
            {
                f_ = 30;
            }
            if (f_ > 3000)
            {
                f_ = 3000;
            }
            if (ha_ > 3000)
            {
                ha_ = 3000;
            }
            if (ha_ < -200)
            {
                ha_ = -200;
            }
            if (hef_ > 3000)
            {
                hef_ = 3000;
            }
            if (hef_ < -1000)
            {
                hef_ = -1000;
            }
            if (p_ > 50)
            {
                p_ = 50;
            }
            if (p_ < 1)
            {
                p_ = 1;
            }
            if (h_gr_ > 3000)
            {
                h_gr_ = 3000;
            }
            if (h_gr_ < 3)
            {
                h_gr_ = 3;
            }
            for (int i = 0; i < list1.Length; i++)
            {
                if (list1[i].land > 1000)
                {
                    list1[i].land = 1000;
                }
                if (list1[i].land < 0)
                {
                    list1[i].land = 0;
                }
                if (list1[i].sea > 1000)
                {
                    list1[i].sea = 1000;
                }
                if (list1[i].sea < 0)
                {
                    list1[i].sea = 0;
                }
            }
            if (h2_ > 100)
            {
                h2_ = 100;
            }
            if (h2aboveSea)
            {
                if (h2_ < 3)
                {
                    h2_ = 3.0;
                }
            }
            else
            {
                if (h2_ < 1)
                {
                    h2_ = 1.0;
                }
            }


            double E5;
            ITU1546_ge06 E1 = new ITU1546_ge06();
            E5 = E1.Get_E_(ha_, hef_, d_, f_, p_, h_gr_, list1);
            // p 9. расчет поправки для приемной антенны;
            double c10 = (3.2 + 6.2 * Math.Log10(f)) * Math.Log10(h2_ / 10);
            double d10 = E1.D06(f, ha_, 10.0);
            if (h2aboveSea && h2_ < 10.0 && d < d10)
            {
                double dh2 = E1.D06(f, ha_, h2_);
                if (d_ <= dh2)
                {
                    c10 = 0;
                }
                else
                {
                    c10 *= Math.Log10(d_ / dh2) / Math.Log10(d10 / dh2);
                }
            }


            E5 = E5 + c10;
            // ур(2)
            double Egran;
            Egran = E1.Emax_land(d_);
            double tt, ss;
            tt = 0;
            ss = 0;
            for (int i = 0; i < list1.Length; i++)
            {
                tt = list1[i].sea + tt;
            }
            for (int i = 0; i < list1.Length; i++)
            {
                ss = list1[i].sea + list1[i].land + ss;
            }
            if (ss > 0.1)
            {
                tt = tt / ss;
            }

            if (tt < 0)
            {
                tt = 0;
            }
            // Ese – усиление, возникающее для кривых для морских трасс и определяемое как (ур. 1b, 3) 
            Egran = Egran + tt * 2.38 * (1 - Math.Exp(-d_ / 8.94)) * Math.Log10(50 / p_);
            if (E5 > Egran)
            {
                E5 = Egran;
            }

            // p 15
            if (d < 1.0)
            {
                double dslope = Math.Sqrt(d * d + 0.000001 * Math.Pow((ha - h2), 2));
                double dinf = Math.Sqrt(0.0016 + 0.000001 * Math.Pow((ha - h2), 2));
                double dsup = Math.Sqrt(1.0 + 0.000001 * Math.Pow((ha - h2), 2));
                double Einf = 106.9 - 20 * Math.Log10(dinf);
                E5 = Einf + (E5 - Einf) * Math.Log10(dslope / dinf) / Math.Log10(dsup / dinf);
            }

            return E5;
        }
        ////////////////////////////////////////////////////////
        //  Модель 1546_4 
        //  Модель в себя не включает (по сравнению с регламентированной):
        //  - поправку на тропосферное расеивание
        //  - прправку на просвет местности
        //  - поправку на углы закрытия
        //  - поправку на процент територри
        //  - поправку на распространения на частотах менее 100МГц для моря
        //  - поправку прием с вытотой менее 10м в условиях моря
        //    
        //  Результат = напряженность поля в дБ(мкВ/м)
        //  Входные данные: 
        //  - ha, высота антенны над поверхностью земли, м (от -200 до 3000м)
        //  - hef, эффективня высота антенны, м  (от -1000 до 3000м) при отсутсвии вставлять значение ha 
        //  - d, длинна трассы, км (от 0.1 до 1000км) 
        //  - f, частота, МГц (от 30 до 3000МГц)
        //  - p, процент времени (от 1% до 50%)
        //  - h_gr, выоста антенны над уровнем моря, м (от -200 до 3000) используется только в морских трассах
        //  - h2, высота антенны абонента, м (по умолчанию 10м)
        //  - d_land_1, d_land_2, d_land_3, d_land_4 - растояние участков суши, км. для смешаных трасс по умолчанию 0 
        //  - d_sea_1, d_sea_2, d_sea_3, d_sea_4 - растояние участков моря, км. для смешаных трасс по умолчанию 0 
        //
        /////////////////////////////////////////////////////////

    }

}
