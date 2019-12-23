using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace Atdi.Test.Sdrn.DeviceServer.Processing.Measurements
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != "")
                {
                    FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        var obj = formatter.Deserialize(fs);
                        if (obj != null)
                        {
                            var mesureTraceResult = (obj as MesureTraceResult);

                            Emitting emitting = new Emitting();
                            ReferenceSituation referenceSituation = new ReferenceSituation();
                            MesureTraceDeviceProperties mesureTraceDeviceProperties = new MesureTraceDeviceProperties();



                            //ReferenceLevels referenceLevels = CalcReferenceLevels.CalcRefLevels(referenceSituation, mesureTraceResult, mesureTraceDeviceProperties, ref taskContext.Task.NoiseLevel_dBm);
                        }
                    }
                    catch (SerializationException eX)
                    {
                        Console.WriteLine("Failed to deserialize. Reason: " + eX.Message);
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }

            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            //Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.rangArrCalc(ref arr);
            float[] arr1 = new float[SpectrSygnal.GSM1.Length];
            for (int i = 0; SpectrSygnal.GSM1.Length>i; i++) { arr1[i] = (float)SpectrSygnal.GSM1[i];}
            float[] arr2 = new float[SpectrSygnal.GSM2.Length];
            for (int i = 0; SpectrSygnal.GSM2.Length > i; i++) { arr2[i] = (float)SpectrSygnal.GSM2[i]; }
            float[] arr3 = new float[SpectrSygnal.GSM3.Length];
            for (int i = 0; SpectrSygnal.GSM3.Length > i; i++) { arr3[i] = (float)SpectrSygnal.GSM3[i]; }
            float[] arr4 = new float[SpectrSygnal.GSM4.Length];
            for (int i = 0; SpectrSygnal.GSM4.Length > i; i++) { arr4[i] = (float)SpectrSygnal.GSM4[i]; }
            float[] arr5 = new float[SpectrSygnal.GSM5.Length];
            for (int i = 0; SpectrSygnal.GSM5.Length > i; i++) { arr5[i] = (float)SpectrSygnal.GSM5[i]; }
            float[] arr6 = new float[SpectrSygnal.GSM6.Length];
            for (int i = 0; SpectrSygnal.GSM6.Length > i; i++) { arr6[i] = (float)SpectrSygnal.GSM6[i]; }
            float[] arr7 = new float[SpectrSygnal.GSM7.Length];
            for (int i = 0; SpectrSygnal.GSM7.Length > i; i++) { arr7[i] = (float)SpectrSygnal.GSM7[i]; }
            float[] arr8 = new float[SpectrSygnal.GSM8.Length];
            for (int i = 0; SpectrSygnal.GSM8.Length > i; i++) { arr8[i] = (float)SpectrSygnal.GSM8[i]; }
            float[] arr9 = new float[SpectrSygnal.GSM9.Length];
            for (int i = 0; SpectrSygnal.GSM9.Length > i; i++) { arr9[i] = (float)SpectrSygnal.GSM9[i]; }
            float[] arr10 = new float[SpectrSygnal.GSM10.Length];
            for (int i = 0; SpectrSygnal.GSM10.Length > i; i++) { arr10[i] = (float)SpectrSygnal.GSM10[i]; }

            float[] arr11 = new float[SpectrSygnal.UMTS1.Length];
            for (int i = 0; SpectrSygnal.UMTS1.Length > i; i++) { arr11[i] = (float)SpectrSygnal.UMTS1[i]; }
            float[] arr12 = new float[SpectrSygnal.UMTS2.Length];
            for (int i = 0; SpectrSygnal.UMTS2.Length > i; i++) { arr12[i] = (float)SpectrSygnal.UMTS2[i]; }
            float[] arr13 = new float[SpectrSygnal.UMTS3.Length];
            for (int i = 0; SpectrSygnal.UMTS3.Length > i; i++) { arr13[i] = (float)SpectrSygnal.UMTS3[i]; }
            float[] arr14 = new float[SpectrSygnal.UMTS4.Length];
            for (int i = 0; SpectrSygnal.UMTS4.Length > i; i++) { arr14[i] = (float)SpectrSygnal.UMTS4[i]; }
            float[] arr15 = new float[SpectrSygnal.LTE1.Length];
            for (int i = 0; SpectrSygnal.LTE1.Length > i; i++) { arr15[i] = (float)SpectrSygnal.LTE1[i]; }
            float[] arr16 = new float[SpectrSygnal.LTE2.Length];
            for (int i = 0; SpectrSygnal.LTE2.Length > i; i++) { arr16[i] = (float)SpectrSygnal.LTE2[i]; }
            float[] arr17 = new float[SpectrSygnal.LTE3.Length];
            for (int i = 0; SpectrSygnal.LTE3.Length > i; i++) { arr17[i] = (float)SpectrSygnal.LTE3[i]; }
            float[] arr18 = new float[SpectrSygnal.LTE4.Length];
            for (int i = 0; SpectrSygnal.LTE4.Length > i; i++) { arr18[i] = (float)SpectrSygnal.LTE4[i]; }

            List<float[]> arrs = new List<float[]>();
            arrs.Add(arr1); arrs.Add(arr2); arrs.Add(arr3); arrs.Add(arr4); arrs.Add(arr5); arrs.Add(arr6); arrs.Add(arr7); arrs.Add(arr8); arrs.Add(arr9); arrs.Add(arr10);
            List<float[]> arrsSM = new List<float[]>();
            for (int i = 0; arrs.Count > i; i++)
            {
                arrsSM.Add(Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.SmoothTrace.blackman(arrs[i], arrs[i].Length));
            }

            List<int> CorrelationShiftPearson = new List<int>();
            List<double> CorrelationPearson = new List<double>();
            DateTime startTime = DateTime.Now;
            for (int i = 1; arrs.Count - 1 > i; i++)
            {
                double BestCorr;
                int l = (int)(arrsSM[0].Length / 2);
                int l05 = (int)l/2;
                float[] arrtest1 = new float[l];
                float[] arrtest2 = new float[l];
                Array.Copy(arrsSM[0], l05, arrtest1, 0, l);
                Array.Copy(arrsSM[i], l05, arrtest2, 0, l);
                int shift = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcShiftSpectrum(arrtest1, arrtest2, СorrelationСoefficient.MethodCalcCorrelation.Person, out BestCorr);
                CorrelationShiftPearson.Add(shift);
                CorrelationPearson.Add(BestCorr);

            }
            TimeSpan TimePearson = DateTime.Now - startTime;



            /*
            List<double> CorrelationPearson = new List<double>();
            DateTime startTime = DateTime.Now;
            for (int i = 0; arrs.Count -1> i; i++)
            {
                double corr = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcCorrelation(arrs[0], 900000000, 3054.4 ,arrs[i+1], 900100000, 1567.9, СorrelationСoefficient.MethodCalcCorrelation.Person);
                CorrelationPearson.Add(corr);
                corr = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcCorrelation(arrsSM[0], 900000000, 3054.4, arrsSM[i + 1], 900100000, 1567.9, СorrelationСoefficient.MethodCalcCorrelation.Person);
                CorrelationPearson.Add(corr);
            }
            TimeSpan TimePearson = DateTime.Now - startTime;
            startTime = DateTime.Now;
            List<double> CorrelationPearsonLinear = new List<double>();
            for (int i = 0; arrs.Count - 1 > i; i++)
            {
                double corr = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcCorrelation(arrs[0], 900000000, 3054.4, arrs[i + 1], 900100000, 1567.9, СorrelationСoefficient.MethodCalcCorrelation.PersonLinear);
                CorrelationPearsonLinear.Add(corr);
                corr = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcCorrelation(arrsSM[0], 900000000, 3054.4, arrsSM[i + 1], 900100000, 1567.9, СorrelationСoefficient.MethodCalcCorrelation.PersonLinear);
                CorrelationPearsonLinear.Add(corr);
            }
            TimeSpan TimeLinear = DateTime.Now - startTime;
            startTime = DateTime.Now;
            List<double> CorrelationSpearman = new List<double>();
            for (int i = 0; arrs.Count - 1 > i; i++)
            {
                double corr = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcCorrelation(arrs[0], 900000000, 3054.4, arrs[i + 1], 900100000, 1567.9,  СorrelationСoefficient.MethodCalcCorrelation.Spearman);
                CorrelationSpearman.Add(corr);
                corr = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcCorrelation(arrsSM[0], 900000000, 3054.4, arrsSM[i + 1], 900100000, 1567.9, СorrelationСoefficient.MethodCalcCorrelation.Spearman);
                CorrelationSpearman.Add(corr);
            }
            TimeSpan TimeSpearman = DateTime.Now - startTime;
            startTime = DateTime.Now;
            List<double> CorrelationSpearmanSmmetric = new List<double>();
            for (int i = 0; arrs.Count - 1 > i; i++)
            {
                double corr = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcCorrelation(arrs[0], 900000000, 3054.4, arrs[i + 1], 900100000, 1567.9, СorrelationСoefficient.MethodCalcCorrelation.SpearmanSmmetric);
                CorrelationSpearmanSmmetric.Add(corr);
                corr = Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements.СorrelationСoefficient.CalcCorrelation(arrsSM[0], 900000000, 3054.4, arrsSM[i + 1], 900100000, 1567.9, СorrelationСoefficient.MethodCalcCorrelation.SpearmanSmmetric);
                CorrelationSpearmanSmmetric.Add(corr);
            }
            TimeSpan TimeSmmetric = DateTime.Now - startTime;
            */
        }
    }
}
