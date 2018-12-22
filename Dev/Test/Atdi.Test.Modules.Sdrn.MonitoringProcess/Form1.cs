using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using ServerTCP;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml.Serialization;
using Atdi.Modules.MonitoringProcess.SingleHound;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer.Contracts;
using Atdi.Modules.MonitoringProcess.ProcessSignal;
using Atdi.Modules.MonitoringProcess;
using Atdi.Modules.MonitoringProcess.Measurement;
using Atdi.Sdrn.Modules.MonitoringProcess;
using System.Runtime.Serialization.Formatters.Binary;


namespace Atdi.Test.Modules.Sdrn.MonitoringProcess
{
    public partial class Form1 : Form
    {
        // public SDR_BB60C SDR_test;
        private delegate void UpdateStatusCallback(string strMessage);
        //private Server mainServer;
        public int id = -1;
        private bbStatus status { get; set; }
        private string AllText = "";
        public Thread thrListene;


        public Form1()
        {
            InitializeComponent();
            status = bbStatus.bbNoError;
            //thrListene = new Thread();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1">x - кордината первой точки</param>
        /// <param name="y1">y - кордината первой точки</param>
        /// <param name="x2">x - кордината второй точки</param>
        /// <param name="y2">y - кордината второй точки</param>
        public void drawLine(int x1, int y1, int x2, int y2)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            Pen pen = new Pen(Color.Red, 1);
            g.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
            g.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">x - кордината  точки</param>
        /// <param name="y">y - кордината  точки</param>
        public void drawPoint(int x, int y)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.FillRectangle(new SolidBrush(Color.FromArgb(250, Color.Red)), x, y, 1, 1);
            g.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">x - кордината  точки</param>
        /// <param name="y">y - кордината  точки</param>
        public void drawPointBlue(int x, int y)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.FillRectangle(new SolidBrush(Color.FromArgb(250, Color.Blue)), x, y, 5, 5);
            g.Dispose();
        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            drawLine(0, 0, 100, 100);
            drawLine(100, 100, 100, 240);
            drawPoint(240, 240);
            drawPoint(240, 242);
            drawPoint(240, 246);
        }

        //private void button4_Click(object sender, EventArgs e)


        private void button5_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.Clear(pictureBox1.BackColor);
        }

  
        private void Form1_Shown(object sender, EventArgs e)
        {
            //Listen();
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //mainServer.Close();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            // Успешно пройден 04_09_2018 Максим, 03_12_2018 Максим
            //  Тестовый код для отработки функции измерения MeasProsess 
            // тестирование занятия спектра
            MeasSdrTask mEAS_TASK_SDR = new MeasSdrTask();
            mEAS_TASK_SDR.Id = 1;
            mEAS_TASK_SDR.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasTaskId.Value = 2;
            mEAS_TASK_SDR.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasSubTaskId.Value = 3;
            mEAS_TASK_SDR.MeasSubTaskStationId = 4;
            mEAS_TASK_SDR.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR.MeasFreqParam.Step = 200; // 50 кГц
            Atdi.AppServer.Contracts.Sdrns.MeasFreq[] Freqs = new Atdi.AppServer.Contracts.Sdrns.MeasFreq[31];
            Freqs[0] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[0].Freq = 945;
            Freqs[1] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[1].Freq = 945.2;
            Freqs[2] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[2].Freq = 945.4;
            Freqs[3] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[3].Freq = 945.6;
            Freqs[4] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[4].Freq = 945.8;
            Freqs[5] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[5].Freq = 946;
            Freqs[6] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[6].Freq = 946.2;
            Freqs[7] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[7].Freq = 946.4;
            Freqs[8] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[8].Freq = 946.6;
            Freqs[9] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[9].Freq = 946.8;
            Freqs[10] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[10].Freq = 947;
            Freqs[11] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[11].Freq = 947.2;
            Freqs[12] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[12].Freq = 947.4;
            Freqs[13] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[13].Freq = 947.6;
            Freqs[14] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[14].Freq = 947.8;
            Freqs[15] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[15].Freq = 948;
            Freqs[16] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[16].Freq = 948.2;
            Freqs[17] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[17].Freq = 948.4;
            Freqs[18] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[18].Freq = 948.6;
            Freqs[19] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[19].Freq = 948.8;
            Freqs[20] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[20].Freq = 949;
            Freqs[21] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[21].Freq = 949.2;
            Freqs[22] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[22].Freq = 949.4;
            Freqs[23] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[23].Freq = 949.6;
            Freqs[24] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[24].Freq = 949.8;
            Freqs[25] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[25].Freq = 950;
            Freqs[26] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[26].Freq = 950.2;
            Freqs[27] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[27].Freq = 950.4;
            Freqs[28] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[28].Freq = 950.6;
            Freqs[29] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[29].Freq = 950.8;
            Freqs[30] = new Atdi.AppServer.Contracts.Sdrns.MeasFreq(); Freqs[30].Freq = 951;


            mEAS_TASK_SDR.MeasFreqParam.MeasFreqs = Freqs;
            mEAS_TASK_SDR.MeasFreqParam.RgL = 945;
            mEAS_TASK_SDR.MeasFreqParam.RgU = 949;
            mEAS_TASK_SDR.MeasSDRParam = new MeasSdrParam();
            mEAS_TASK_SDR.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.Avarage;
            mEAS_TASK_SDR.MeasSDRParam.PreamplificationSDR = 0;
            mEAS_TASK_SDR.MeasSDRParam.RBW = 200;
            mEAS_TASK_SDR.MeasSDRParam.VBW = 200;
            mEAS_TASK_SDR.MeasSDRParam.ref_level_dbm = -40;
            mEAS_TASK_SDR.MeasSDRParam.RfAttenuationSDR = 0;
            mEAS_TASK_SDR.MeasSDRParam.MeasTime = 0.01;
            mEAS_TASK_SDR.SwNumber = 10;
            mEAS_TASK_SDR.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.SpectrumOccupation;
            mEAS_TASK_SDR.TypeM = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
            mEAS_TASK_SDR.MeasSDRSOParam = new MeasSdrSOParam();
            mEAS_TASK_SDR.MeasSDRSOParam.TypeSO = Atdi.AppServer.Contracts.Sdrns.SpectrumOccupationType.FreqChannelOccupation;
            mEAS_TASK_SDR.MeasSDRSOParam.NChenal = 10;
            mEAS_TASK_SDR.MeasSDRSOParam.LevelMinOccup = -75;
            mEAS_TASK_SDR.NumberScanPerTask = -999;
            mEAS_TASK_SDR.Time_start = DateTime.Now - TimeSpan.FromSeconds(10);
            mEAS_TASK_SDR.Time_stop = DateTime.Now + TimeSpan.FromSeconds(65);
            mEAS_TASK_SDR.PerInterval = 10;
            mEAS_TASK_SDR.status = "A";
            SDRBB60C SDR = new SDRBB60C();
            Sensor sensor = new Sensor();
            MeasSdrResults Res = new MeasSdrResults();
            CirculatingData circulatingData = null;
            ReferenceSignal[] referenceSignals = null;
            MeasurementProcessing MeasProcessing = new MeasurementProcessing();
            int i = 0;
            while (mEAS_TASK_SDR.status == "A") // цикл имитирует процесс измерения
            {
                // поиск таска Task из ListTask
                // MeasProcessing (Task)
                // Если есть результаты они передаются в другой поток ()
                if ((mEAS_TASK_SDR.Time_start < DateTime.Now) && (mEAS_TASK_SDR.Time_stop > DateTime.Now))
                {
                    Res = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR, sensor, ref circulatingData, Res, referenceSignals) as MeasSdrResults;
                }
                else
                {
                    if (mEAS_TASK_SDR.Time_stop < DateTime.Now) { mEAS_TASK_SDR.status = "C"; }
                }
                i++; // просто счетчик
                Thread.Sleep(10);
            }

        }


        private void button16_Click(object sender, EventArgs e)
        {
            ////  Тестовый код для отработки функции измерения MeasProsess
            //// тестирование занятия спектра
            //MeasSdrTask mEAS_TASK_SDR = new MeasSdrTask();
            //mEAS_TASK_SDR.Id = 1;
            //mEAS_TASK_SDR.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            //mEAS_TASK_SDR.MeasTaskId.Value = 2;
            //mEAS_TASK_SDR.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            //mEAS_TASK_SDR.MeasSubTaskId.Value = 3;
            //mEAS_TASK_SDR.MeasSubTaskStationId = 4;
            //mEAS_TASK_SDR.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            //mEAS_TASK_SDR.MeasFreqParam.Step = 100; // 50 кГц
            //mEAS_TASK_SDR.MeasFreqParam.RgL = 925.00;
            //mEAS_TASK_SDR.MeasFreqParam.RgU = 960.00;
            //mEAS_TASK_SDR.MeasSDRParam = new MeasSdrParam();
            //mEAS_TASK_SDR.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.Avarage;
            //mEAS_TASK_SDR.MeasSDRParam.PreamplificationSDR = 0;
            //mEAS_TASK_SDR.MeasSDRParam.RBW = 100;
            //mEAS_TASK_SDR.MeasSDRParam.VBW = 100;
            //mEAS_TASK_SDR.MeasSDRParam.ref_level_dbm = -30;
            //mEAS_TASK_SDR.MeasSDRParam.RfAttenuationSDR = 0;
            //mEAS_TASK_SDR.MeasSDRParam.MeasTime = 0.001;
            //mEAS_TASK_SDR.SwNumber = 1;
            //mEAS_TASK_SDR.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.Level;
            //mEAS_TASK_SDR.TypeM = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
            //SDR_BB60C SDR = new SDR_BB60C();
            //Atdi.AppServer.Contracts.Sdrns.Sensor sensor = new Atdi.AppServer.Contracts.Sdrns.Sensor();
            //MeasSdrResults Res = new MeasSdrResults();
            //List<MeasSdrResults> List_Res = new List<MeasSdrResults>();
            //MeasurementProcessingSingleHoundBB60C MeasProcessing = new MeasurementProcessingSingleHoundBB60C();

            //Res = MeasProcessing.MeasurementProcessing(ref SDR, ref mEAS_TASK_SDR, sensor, Res);
            //DateTime t1 = DateTime.Now;

            //for (int i = 0; i < 1000; i++)
            //{
            //    Res = MeasProcessing.MeasurementProcessing(ref SDR, ref mEAS_TASK_SDR, sensor);
            //    List_Res.Add(Res);
            //    Random rds = new Random();
            //    double m = rds.Next(1, 10);
            //    Thread.Sleep((int)m);
            //}
            ///*
            //DateTime t2 = DateTime.Now;
            //string Str = "";

            //for (int i = 0; i < List_Res[0].FSemples.Count(); i++)
            //{
            //    Double first_row = List_Res[0].FSemples[i].Freq;
            //    Str += string.Format("{0};", first_row);
            //}
            //Str += Environment.NewLine;
            //string Val = "";
            //for (int j = 0; j < List_Res.Count; j++)
            //{
            //    string new_line = "";
            //    for (int i = 0; i < List_Res[j].FSemples.Count(); i++)
            //    {
            //        Double row = List_Res[j].FSemples[i].LeveldBm;
            //        new_line += string.Format("{0};", row);
            //    }
            //    DateTime t_save = List_Res[j].DataMeas;
            //    new_line += string.Format("[Time - {0}:{1}:{2}.{3}]", t_save.Hour, t_save.Minute, t_save.Second, t_save.Millisecond) + Environment.NewLine;
            //    Val += new_line+Environment.NewLine;
            //}

            //System.IO.File.WriteAllText("C:\\Temp\\Result.txt", Str+Val);*/

        }

        private void button17_Click(object sender, EventArgs e)
        {
            /*
            // тестирование RealTime сканирования
            MeasSdrTask mEAS_TASK_SDR = new MeasSdrTask();
            mEAS_TASK_SDR.Id = 1;
            mEAS_TASK_SDR.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasTaskId.Value = 2;
            mEAS_TASK_SDR.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasSubTaskId.Value = 3;
            mEAS_TASK_SDR.MeasSubTaskStationId = 4;
            mEAS_TASK_SDR.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR.MeasFreqParam.Step = 2.465820; // 50 кГц
            mEAS_TASK_SDR.MeasFreqParam.RgL = 959.00;
            mEAS_TASK_SDR.MeasFreqParam.RgU = 960.00;
            mEAS_TASK_SDR.MeasSDRParam = new  MeasSdrParam();
            mEAS_TASK_SDR.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.Avarage;
            mEAS_TASK_SDR.MeasSDRParam.PreamplificationSDR = 0;
            mEAS_TASK_SDR.MeasSDRParam.RBW = 2.465820;
            mEAS_TASK_SDR.MeasSDRParam.VBW = 2.465820;
            mEAS_TASK_SDR.MeasSDRParam.ref_level_dbm = -30;
            mEAS_TASK_SDR.MeasSDRParam.RfAttenuationSDR = 0;
            mEAS_TASK_SDR.MeasSDRParam.MeasTime = 0.001;
            mEAS_TASK_SDR.SwNumber = 1;
            mEAS_TASK_SDR.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.Level;
            mEAS_TASK_SDR.TypeM = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.RealTime;
            mEAS_TASK_SDR.status = "RT";
            SDR_BB60C SDR = new SDR_BB60C();
            Atdi.AppServer.Contracts.Sdrns.Sensor sensor = new Atdi.AppServer.Contracts.Sdrns.Sensor();
            MeasSdrResults Res = new MeasSdrResults();
            List<MeasSdrResults> List_Res = new List<MeasSdrResults>();
            for (int i = 0; i < 45; i++)
            {
                Res = MeasProcessing(ref SDR, ref mEAS_TASK_SDR, sensor, Res); // тут результат уже получен
                List_Res.Add(Res);
                // здесь отобразим результат
                int W = Res.RealTimeImages.GetUpperBound(0)+1;
                int H = Res.RealTimeImages.GetUpperBound(1)+1;
                Bitmap pt = new Bitmap(H, W);
                float[,] arr = Res.RealTimeImages;
                float MaxProb = 0;
                for (int i1 = 0; i1 < W; i1++)
                {
                    for (int j = 0; j < H; j++)
                    {
                        if (arr[i1, j] > MaxProb) { MaxProb = arr[i1, j];}
                    }
                }
                Double graa;
                for (int i1 = 0; i1 <W; i1++)
                {
                    for (int j = 0; j < H; j++)
                    {
                        if (arr[i1,j] == 0) {graa = 0; } else 
                        {
                            graa = 255 * (arr[i1, j] / MaxProb);
                        }
                        pt.SetPixel(j, W - i1-1, Color.FromArgb((int)graa, 0, 0));
                    }
                }
                pictureBox1.Image = pt;
            }
            */
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        unsafe private void button18_Click(object sender, EventArgs e)
        {
            // инициализация устройства   
            int id = -1;
            textBox1.Text += "SW Opening Device, Please Wait" + Environment.NewLine;
            status = bb_api.bbOpenDevice(ref id);
            if (status != bbStatus.bbNoError)
            {
                textBox1.Text += "SW Error: Unable to open BB60" + Environment.NewLine;
                textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine; return;
            }
            else { textBox1.Text += "SW Device Found" + Environment.NewLine; }
            // Конец инициализации
            // Установка параметров измерения 
            Double f_central = 959200000;
            Double span = 200000;
            bb_api.bbConfigureCenterSpan(id, f_central, span);
            bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);
            bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);
            bb_api.bbConfigureIQ(id, 1, 15000000);
            bb_api.bbConfigureIO(id, 0, bb_api.BB_PORT2_IN_TRIGGER_RISING_EDGE);
            //bb_api.bbSyncCPUtoGPS(3, 38400);
            status = bb_api.bbInitiate(id, bb_api.BB_STREAMING, bb_api.BB_STREAM_IQ);
            if (status != bbStatus.bbNoError) { return; }
            double bandwidth = 0;
            int sampleRate = 0;
            int rr = 0;
            bb_api.bbQueryStreamInfo(id, ref rr, ref bandwidth, ref sampleRate);
            const int BlockSize = 524288;
            //float* bufer = stackalloc float[BlockSize * 2];
            //int* triggers = stackalloc int[71];
            //bbIQPacket pkt = new bbIQPacket();
            //pkt.iqData = bufer;
            //pkt.iqCount = BlockSize;
            //pkt.triggers = triggers;
            //pkt.triggerCount = 70;
            //pkt.purge = 1;
            //bb_api.bbGetIQ(id, ref pkt);
            string file = "";
            List<bbIQPacket> listpkt = new List<bbIQPacket>();
            for (int i = 0; i < 500; i++)
            {
                float[] bufer_ = new float[BlockSize * 2];
                int[] triggers_ = new int[71];
                fixed (float* bufer = bufer_)
                {
                    fixed (int* triggers = triggers_)
                    {
                        var pkt = new bbIQPacket()
                        {
                            iqCount = BlockSize,
                            triggerCount = 70,
                            purge = 1,
                            iqData = bufer,
                            triggers = triggers
                        };
                        bb_api.bbGetIQ(id, ref pkt);
                        listpkt.Add(pkt);
                    }

                }
                bufer_ = null;
                triggers_ = null;
            }
            //for (int i = 0; i < 500; i++)
            //{
            //    bb_api.bbGetIQ(id, ref pkt);
            //    file+= "Triggers: ";
            //    for (int j = 0; j < 10; j++)
            //    {
            //        file+= pkt.triggers[j].ToString()+ " ";
            //    }
            //    file+= Environment.NewLine;
            //    file+= "iqData: ";
            //    for (int j = 0; j < 10; j++)
            //    {
            //        file += pkt.iqData[j].ToString() + " ";
            //    }
            //    file += Environment.NewLine;

            //}
            //            System.IO.File.WriteAllText("C:\\Temp\\ResSharp.txt",file);
            bb_api.bbCloseDevice(id);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            // инициализация устройства   
            int id = -1;
            textBox1.Text += "SW Opening Device, Please Wait" + Environment.NewLine;
            status = bb_api.bbOpenDevice(ref id);
            if (status != bbStatus.bbNoError)
            {
                textBox1.Text += "SW Error: Unable to open BB60" + Environment.NewLine;
                textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine; return;
            }
            else { textBox1.Text += "SW Device Found" + Environment.NewLine; }
            // Конец инициализации
            // Установка параметров измерения 
            Double f_central = 959200000;
            Double span = 200000;
            bb_api.bbConfigureCenterSpan(id, f_central, span);
            bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);
            bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);
            bb_api.bbConfigureIQ(id, 1, span);
            bb_api.bbConfigureIO(id, 0, bb_api.BB_PORT2_IN_TRIGGER_RISING_EDGE);
            status = bb_api.bbInitiate(id, bb_api.BB_STREAMING, 0x10);
            if (status != bbStatus.bbNoError) { return; }
            int return_len = 0; int samples_per_sec = 0; double bandwidth = 0.0;
            bb_api.bbQueryStreamInfo(id, ref return_len, ref bandwidth, ref samples_per_sec);
            float[] iq_samples = new float[return_len * 2];
            int[] triggers = new int[80];
            List<int[]> listTrigger = new List<int[]>();
            for (int i = 0; i < 500; i++)
            {
                bb_api.bbFetchRaw(id, iq_samples, triggers);
                listTrigger.Add(triggers);
            }

            bool hit = false;
            for (int i = 0; i < listTrigger.Count; i++)
            {
                for (int j = 0; j < listTrigger[i].Length; j++)
                {
                    if (listTrigger[i][j] != 0)
                    {
                        hit = true;
                    }
                }
            }


            // Чегото это не пашет. 

            bb_api.bbCloseDevice(id);
        }

        private void CreatePictureForBlokIQ(BlockOfSignal Block, int from, int to, List<int> Blue_point)
        {
            Double Max_IQ = 0;
            for (int i = 0; i < Block.IQStream.Length; i++) { if (Max_IQ < Block.IQStream[i]) { Max_IQ = Block.IQStream[i]; } }
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.Clear(pictureBox1.BackColor);
            drawPointBlue((int)200, (int)200);

            for (int i = from * 2; i < to * 2; i = i + 2)
            {
                Double x = 200 * Block.IQStream[i] / Max_IQ + 200;//I
                Double y = 200 * Block.IQStream[i + 1] / Max_IQ + 200;//Q
                if (Blue_point.Contains(i / 2))//ПРАВКА
                {
                    drawPointBlue((int)x, (int)y);
                }
                else
                {
                    drawPoint((int)x, (int)y);
                }
                System.Threading.Thread.Sleep(5);

            }
        }
        /// <summary>
        /// TDOA Test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button20_Click(object sender, EventArgs e)
        {

            BlockOfSignal Block = new BlockOfSignal();
            List<int> Point_rotation = new List<int>();
            Double Penalty = 0;
            List<Double> RotationPhase = new List<Double>();
            GetTimeStamp test_stream = new GetTimeStamp(null, 40000000, 200, GetTimeStamp.TypeTechnology.GSM);
            CreatePictureForBlokIQ(Block, Point_rotation[1] - 50, Point_rotation[4] + 50, Point_rotation.GetRange(1, 4));
            MessageBox.Show("Банзай");
            CreatePictureForBlokIQ(Block, Point_rotation[4] - 50, Point_rotation[7] + 50, Point_rotation.GetRange(4, 4));
            MessageBox.Show("Еще банзай");
            CreatePictureForBlokIQ(Block, Point_rotation[7] - 50, Point_rotation[10] + 50, Point_rotation.GetRange(7, 4));
            MessageBox.Show("Еще банзай....");
            CreatePictureForBlokIQ(Block, Point_rotation[10] - 50, Point_rotation[13] + 50, Point_rotation.GetRange(10, 4));
        }

        private void button21_Click(object sender, EventArgs e)
        {
            // тест пройден 04.09.2018 Максим, 03.12.2018 Максим.
            // тестирование BW 
            // формируем таск
            MeasSdrTask mEAS_TASK_SDR = new MeasSdrTask();
            mEAS_TASK_SDR.Id = 1;
            mEAS_TASK_SDR.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasTaskId.Value = 2;
            mEAS_TASK_SDR.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasSubTaskId.Value = 3;
            mEAS_TASK_SDR.MeasSubTaskStationId = 4;
            mEAS_TASK_SDR.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR.MeasFreqParam.RgL = 954.0;
            mEAS_TASK_SDR.MeasFreqParam.RgU = 954.4;
            mEAS_TASK_SDR.MeasSDRParam = new MeasSdrParam();
            mEAS_TASK_SDR.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.MaxPeak;
            mEAS_TASK_SDR.MeasSDRParam.PreamplificationSDR = -1;
            mEAS_TASK_SDR.MeasSDRParam.RBW = 2;
            mEAS_TASK_SDR.MeasSDRParam.VBW = 2;
            mEAS_TASK_SDR.MeasSDRParam.ref_level_dbm = -50;
            mEAS_TASK_SDR.MeasSDRParam.RfAttenuationSDR = -1;
            mEAS_TASK_SDR.MeasSDRParam.MeasTime = 0.001;
            mEAS_TASK_SDR.SwNumber = 400;
            mEAS_TASK_SDR.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.BandwidthMeas;
            mEAS_TASK_SDR.TypeM = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
            mEAS_TASK_SDR.NumberScanPerTask = -999;
            mEAS_TASK_SDR.Time_start = DateTime.Now - TimeSpan.FromSeconds(10);
            mEAS_TASK_SDR.Time_stop = DateTime.Now + TimeSpan.FromSeconds(650);
            mEAS_TASK_SDR.PerInterval = 10;
            mEAS_TASK_SDR.status = "A";
            mEAS_TASK_SDR.NumberScanPerTask = 1;
            // таск сформирован



            SDRBB60C SDR = new SDRBB60C();
            Atdi.AppServer.Contracts.Sdrns.Sensor sensor = new Atdi.AppServer.Contracts.Sdrns.Sensor();
            MeasSdrResults Res = new MeasSdrResults();
            MeasurementProcessing MeasProcessing = new MeasurementProcessing();
            CirculatingData circulatingData = new CirculatingData();
            int i = 0;
            while (mEAS_TASK_SDR.status == "A") // цикл имитирует процесс измерения
            {
                // поиск таска Task из ListTask
                // MeasProcessing (Task)
                // Если есть результаты они передаются в другой поток ()
                if ((mEAS_TASK_SDR.Time_start < DateTime.Now) && (mEAS_TASK_SDR.Time_stop > DateTime.Now))
                {
                    Res = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR, sensor, ref circulatingData, Res) as MeasSdrResults;
                }
                else
                {
                    if (mEAS_TASK_SDR.Time_stop < DateTime.Now) { mEAS_TASK_SDR.status = "C"; }
                }
                i++; // просто счетчик
                Thread.Sleep(10);
            }
        }

        private void BB60CTest_Click(object sender, EventArgs e)
        {
            textBox1.Text += "SW Opening Device, Please Wait" + Environment.NewLine;
            status = bb_api.bbOpenDevice(ref id);
            if (status != bbStatus.bbNoError)
            {
                textBox1.Text += "SW Error: Unable to open BB60" + Environment.NewLine;
                textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
                return;
            }
            else
            {
                textBox1.Text += "SW Device Found" + Environment.NewLine;
            }
            // типа должны проициализировать устройства 
            // ниже рабочее тело кода итак SW 
            Double f_min = 100000000;
            Double f_max = 1000000000;
            Double Level_min = -110;
            Double Level_max = -10;
            Double Time = 0.001;
            Double RBW = 1000;
            Double VBW = 1000;
            int need_to_count = 10;

            label3.Text = (f_min / 1000000).ToString();
            label4.Text = (f_max / 1000000).ToString();
            label7.Text = Level_min.ToString();
            label8.Text = Level_max.ToString();



            bb_api.bbConfigureAcquisition(id, bb_api.BB_MIN_AND_MAX, bb_api.BB_LOG_SCALE);
            bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);
            bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);
            bb_api.bbConfigureSweepCoupling(id, RBW, VBW, Time, bb_api.BB_NUTALL, bb_api.BB_NO_SPUR_REJECT);
            bb_api.bbConfigureProcUnits(id, bb_api.BB_LOG);
            bb_api.bbConfigureCenterSpan(id, (f_max + f_min) / 2, f_max - f_min);







            status = bb_api.bbInitiate(id, bb_api.BB_SWEEPING, 0);

            if (status != bbStatus.bbNoError)
            {
                textBox1.Text += "SW Error: Unable to initialize BB60";
                textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
                return;
            }
            uint trace_len = 0;
            double bin_size = 0.0;
            double start_freq = 0.0;
            status = bb_api.bbQueryTraceInfo(id, ref trace_len, ref bin_size, ref start_freq);
            float[] sweep_max, sweep_min;
            sweep_max = new float[trace_len];
            sweep_min = new float[trace_len];
            int j = 0;
            long Npoint = trace_len * need_to_count;
            DateTime dateTime = DateTime.Now;
            while (j++ < need_to_count)
            {
                bb_api.bbFetchTrace_32f(id, unchecked((int)trace_len), sweep_min, sweep_max);
                ////textBox1.Text += "Sweep Retrieved";
                //System.Threading.Thread.Sleep(150);
                //Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
                //label9.Text = j.ToString();
                //g.Clear(pictureBox1.BackColor);
                //for (int i = 0; i < trace_len; i++)
                //{
                //    Double x = 400 * i / trace_len;
                //    Double y_min = (-sweep_min[i] + Level_max) * 400 / (Level_max - Level_min);
                //    Double y_max = (-sweep_max[i] + Level_max) * 400 / (Level_max - Level_min);
                //    //drawPoint((int)x, (int)y_min);
                //    drawPoint((int)x, (int)(y_max / 2 + y_max / 2));
                //}
            }
            TimeSpan timeSpan = DateTime.Now - dateTime;
            //конец рабочего тела.
            // гасим устройство 
            textBox1.Text += "SW Closing Device";
            bb_api.bbCloseDevice(id);
        }

        private void TestSign_Click(object sender, EventArgs e)
        {
            // 
            //  Тестовый код для отработки функции измерения MeasProsess 
            // тестирование сигнализации
            MeasSdrTask mEAS_TASK_SDR = new MeasSdrTask();
            mEAS_TASK_SDR.Id = 1;
            mEAS_TASK_SDR.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasTaskId.Value = 2;
            mEAS_TASK_SDR.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasSubTaskId.Value = 3;
            mEAS_TASK_SDR.MeasSubTaskStationId = 4;
            mEAS_TASK_SDR.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR.MeasFreqParam.Step = 25; // 10 кГц
            mEAS_TASK_SDR.MeasFreqParam.RgL = 900;
            mEAS_TASK_SDR.MeasFreqParam.RgU = 1000;
            mEAS_TASK_SDR.MeasSDRParam = new MeasSdrParam();
            mEAS_TASK_SDR.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.Avarage;
            mEAS_TASK_SDR.MeasSDRParam.PreamplificationSDR = -1;
            mEAS_TASK_SDR.MeasSDRParam.RBW = 25; // 10 кГц
            mEAS_TASK_SDR.MeasSDRParam.VBW = 25; // 10 кГц
            mEAS_TASK_SDR.MeasSDRParam.ref_level_dbm = -40;
            mEAS_TASK_SDR.MeasSDRParam.RfAttenuationSDR = -1;
            mEAS_TASK_SDR.MeasSDRParam.MeasTime = 0.001;
            mEAS_TASK_SDR.SwNumber = 1;
            mEAS_TASK_SDR.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.PICode;
            mEAS_TASK_SDR.TypeM = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
            mEAS_TASK_SDR.MeasSDRSOParam = new MeasSdrSOParam();
            mEAS_TASK_SDR.NumberScanPerTask = -999;
            mEAS_TASK_SDR.Time_start = DateTime.Now - TimeSpan.FromSeconds(10);
            mEAS_TASK_SDR.Time_stop = DateTime.Now + TimeSpan.FromSeconds(65);
            mEAS_TASK_SDR.PerInterval = 55;
            mEAS_TASK_SDR.status = "A";
            SDRBB60C SDR = new SDRBB60C();
            Sensor sensor = new Sensor();
            MeasSdrResults_v2 Res = null;
            CirculatingData circulatingData = null;
            ReferenceSignal[] referenceSignals = null;
            MeasurementProcessing MeasProcessing = new MeasurementProcessing();
            List<Emitting> emittings = new List<Emitting>();
            int i = 0;
            while (mEAS_TASK_SDR.status == "A") // цикл имитирует процесс измерения
            {
                // поиск таска Task из ListTask
                // MeasProcessing (Task)
                // Если есть результаты они передаются в другой поток ()
                if ((mEAS_TASK_SDR.Time_start < DateTime.Now) && (mEAS_TASK_SDR.Time_stop > DateTime.Now))
                {
                    Res = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR, sensor, ref circulatingData, Res, referenceSignals) as MeasSdrResults_v2;
                    emittings.AddRange(Res.emittings);
                }
                else
                {
                    if (mEAS_TASK_SDR.Time_stop < DateTime.Now) { mEAS_TASK_SDR.status = "C"; }
                }
                i++; // просто счетчик
                //Thread.Sleep(10);
            }


        }

        private void IQStream_Sweep_Click(object sender, EventArgs e)
        {
            MeasSdrTask mEAS_TASK_SDR_BW = new MeasSdrTask();
            mEAS_TASK_SDR_BW.Id = 1;
            mEAS_TASK_SDR_BW.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR_BW.MeasTaskId.Value = 2;
            mEAS_TASK_SDR_BW.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR_BW.MeasSubTaskId.Value = 3;
            mEAS_TASK_SDR_BW.MeasSubTaskStationId = 4;
            mEAS_TASK_SDR_BW.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR_BW.MeasFreqParam.RgL = 959.0;
            mEAS_TASK_SDR_BW.MeasFreqParam.RgU = 959.4;
            mEAS_TASK_SDR_BW.MeasSDRParam = new MeasSdrParam();
            mEAS_TASK_SDR_BW.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.MaxPeak;
            mEAS_TASK_SDR_BW.MeasSDRParam.PreamplificationSDR = -1;
            mEAS_TASK_SDR_BW.MeasSDRParam.RBW = 2;
            mEAS_TASK_SDR_BW.MeasSDRParam.VBW = 2;
            mEAS_TASK_SDR_BW.MeasSDRParam.ref_level_dbm = -50;
            mEAS_TASK_SDR_BW.MeasSDRParam.RfAttenuationSDR = -1;
            mEAS_TASK_SDR_BW.MeasSDRParam.MeasTime = 0.001;
            mEAS_TASK_SDR_BW.SwNumber = 1;
            mEAS_TASK_SDR_BW.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.BandwidthMeas;
            mEAS_TASK_SDR_BW.TypeM = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
            mEAS_TASK_SDR_BW.NumberScanPerTask = -999;
            mEAS_TASK_SDR_BW.Time_start = DateTime.Now - TimeSpan.FromSeconds(10);
            mEAS_TASK_SDR_BW.Time_stop = DateTime.Now + TimeSpan.FromSeconds(650);
            mEAS_TASK_SDR_BW.PerInterval = 10;
            mEAS_TASK_SDR_BW.status = "A";
            mEAS_TASK_SDR_BW.NumberScanPerTask = 1;
            // таск на BW сформирован
            MeasSdrTask mEAS_TASK_SDR_IQ = new MeasSdrTask();
            mEAS_TASK_SDR_IQ.Id = 5;
            mEAS_TASK_SDR_IQ.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR_IQ.MeasTaskId.Value = 6;
            mEAS_TASK_SDR_IQ.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR_IQ.MeasSubTaskId.Value = 7;
            mEAS_TASK_SDR_IQ.MeasSubTaskStationId = 8;
            mEAS_TASK_SDR_IQ.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR_IQ.MeasFreqParam.RgL = 959.0;
            mEAS_TASK_SDR_IQ.MeasFreqParam.RgU = 959.4;
            mEAS_TASK_SDR_IQ.MeasSDRParam = new MeasSdrParam();
            mEAS_TASK_SDR_IQ.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.MaxPeak;
            mEAS_TASK_SDR_IQ.MeasSDRParam.PreamplificationSDR = -1;
            mEAS_TASK_SDR_IQ.MeasSDRParam.RBW = 400;
            mEAS_TASK_SDR_IQ.MeasSDRParam.VBW = 400;
            mEAS_TASK_SDR_IQ.MeasSDRParam.ref_level_dbm = -50;
            mEAS_TASK_SDR_IQ.MeasSDRParam.RfAttenuationSDR = -1;
            mEAS_TASK_SDR_IQ.MeasSDRParam.MeasTime = 0.001;
            mEAS_TASK_SDR_IQ.SwNumber = 1;
            mEAS_TASK_SDR_IQ.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.SoundID;
            mEAS_TASK_SDR_IQ.NumberScanPerTask = -999;
            mEAS_TASK_SDR_IQ.Time_start = DateTime.Now - TimeSpan.FromSeconds(10);
            mEAS_TASK_SDR_IQ.Time_stop = DateTime.Now + TimeSpan.FromSeconds(650);
            mEAS_TASK_SDR_IQ.PerInterval = 10;
            mEAS_TASK_SDR_IQ.status = "A";
            mEAS_TASK_SDR_IQ.NumberScanPerTask = 1;
            // таск1 на IQ сформирован
            MeasSdrTask mEAS_TASK_SDR_IQ1 = new MeasSdrTask();
            mEAS_TASK_SDR_IQ1.Id = 10;
            mEAS_TASK_SDR_IQ1.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR_IQ1.MeasTaskId.Value = 11;
            mEAS_TASK_SDR_IQ1.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR_IQ1.MeasSubTaskId.Value = 12;
            mEAS_TASK_SDR_IQ1.MeasSubTaskStationId = 13;
            mEAS_TASK_SDR_IQ1.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR_IQ1.MeasFreqParam.RgL = 969.0;
            mEAS_TASK_SDR_IQ1.MeasFreqParam.RgU = 969.4;
            mEAS_TASK_SDR_IQ1.MeasSDRParam = new MeasSdrParam();
            mEAS_TASK_SDR_IQ1.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.MaxPeak;
            mEAS_TASK_SDR_IQ1.MeasSDRParam.PreamplificationSDR = -1;
            mEAS_TASK_SDR_IQ1.MeasSDRParam.RBW = 400;
            mEAS_TASK_SDR_IQ1.MeasSDRParam.VBW = 400;
            mEAS_TASK_SDR_IQ1.MeasSDRParam.ref_level_dbm = -50;
            mEAS_TASK_SDR_IQ1.MeasSDRParam.RfAttenuationSDR = -1;
            mEAS_TASK_SDR_IQ1.MeasSDRParam.MeasTime = 0.001;
            mEAS_TASK_SDR_IQ1.SwNumber = 1;
            mEAS_TASK_SDR_IQ1.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.SoundID;
            mEAS_TASK_SDR_IQ1.NumberScanPerTask = -999;
            mEAS_TASK_SDR_IQ1.Time_start = DateTime.Now - TimeSpan.FromSeconds(10);
            mEAS_TASK_SDR_IQ1.Time_stop = DateTime.Now + TimeSpan.FromSeconds(650);
            mEAS_TASK_SDR_IQ1.PerInterval = 1;
            mEAS_TASK_SDR_IQ1.status = "A";
            mEAS_TASK_SDR_IQ1.NumberScanPerTask = 1;
            // таск2 на IQ сформирован


            SDRBB60C SDR = new SDRBB60C();
            Atdi.AppServer.Contracts.Sdrns.Sensor sensor = new Atdi.AppServer.Contracts.Sdrns.Sensor();
            MeasSdrResults ResBW = new MeasSdrResults();
            MeasSdrResults ResIQ = new MeasSdrResults();
            MeasSdrResults ResIQ1 = new MeasSdrResults();
            MeasurementProcessing MeasProcessing = new MeasurementProcessing();
            CirculatingData circulatingData = new CirculatingData();
            ResBW = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR_BW, sensor, ref circulatingData, ResBW) as MeasSdrResults;
            DateTime dateTime = DateTime.Now;
            ResIQ = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR_IQ, sensor, ref circulatingData, ResIQ) as MeasSdrResults;
            TimeSpan timeSpan = DateTime.Now - dateTime;
            ResIQ1 = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR_IQ1, sensor, ref circulatingData, ResIQ1) as MeasSdrResults;
            TimeSpan timeSpan1 = DateTime.Now - dateTime;
            ResBW = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR_BW, sensor, ref circulatingData, ResBW) as MeasSdrResults;
            TimeSpan timeSpan2 = DateTime.Now - dateTime;
        }

        private void TestChangeGreed_Click(object sender, EventArgs e)
        {
            int NumberPointInNewLevels = 50;
            double StartOldFreq_MHz = 100;
            double OldStep_kHz = 1000;
            double StartNewFreq_MHz = 98;
            double NewStep_kHz = 300;
            double[] Levels = new double[] { -100, -100, -90, -80, -10, -70, -80, -100 };
            double[] NewLevels = new double[NumberPointInNewLevels];
            NewLevels = ChangeTraceGrid.ChangeGrid(ref Levels, StartOldFreq_MHz, OldStep_kHz, StartNewFreq_MHz, NewStep_kHz, NumberPointInNewLevels);

            NumberPointInNewLevels = 40;
            StartOldFreq_MHz = 100;
            OldStep_kHz = 1000;
            StartNewFreq_MHz = 101;
            NewStep_kHz = 150;
            NewLevels = new double[NumberPointInNewLevels];
            NewLevels = ChangeTraceGrid.ChangeGrid(ref Levels, StartOldFreq_MHz, OldStep_kHz, StartNewFreq_MHz, NewStep_kHz, NumberPointInNewLevels);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Тестирование измерение уровня 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Тестирование записи в IQ поток
            MeasSdrTask mEAS_TASK_SDR = new MeasSdrTask();
            mEAS_TASK_SDR.Id = 1;
            mEAS_TASK_SDR.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasTaskId.Value = 2;
            mEAS_TASK_SDR.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasSubTaskId.Value = 3;
            mEAS_TASK_SDR.MeasSubTaskStationId = 4;
            mEAS_TASK_SDR.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR.MeasFreqParam.Step = 200; // 10 кГц
            mEAS_TASK_SDR.MeasFreqParam.RgL = 941.4 - 0.200 / 2.0;
            mEAS_TASK_SDR.MeasFreqParam.RgU = 941.4 + 0.200 / 2.0;
            mEAS_TASK_SDR.MeasSDRParam = new MeasSdrParam();
            mEAS_TASK_SDR.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.Avarage;
            mEAS_TASK_SDR.MeasSDRParam.PreamplificationSDR = -1;
            mEAS_TASK_SDR.MeasSDRParam.RBW = 200; // 10 кГц
            mEAS_TASK_SDR.MeasSDRParam.VBW = 200; // 10 кГц
            mEAS_TASK_SDR.MeasSDRParam.ref_level_dbm = -40;
            mEAS_TASK_SDR.MeasSDRParam.RfAttenuationSDR = -1;
            mEAS_TASK_SDR.MeasSDRParam.MeasTime = 0.001;
            mEAS_TASK_SDR.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.SoundID;
            mEAS_TASK_SDR.TypeM = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
            mEAS_TASK_SDR.MeasSDRSOParam = new MeasSdrSOParam();
            mEAS_TASK_SDR.NumberScanPerTask = -999;
            mEAS_TASK_SDR.Time_start = DateTime.Now - TimeSpan.FromSeconds(10);
            mEAS_TASK_SDR.Time_stop = DateTime.Now + TimeSpan.FromSeconds(65);
            mEAS_TASK_SDR.PerInterval = 55;
            mEAS_TASK_SDR.status = "A";
            SDRBB60C SDR = new SDRBB60C();
            Sensor sensor = new Sensor();
            MeasSdrResults_v2 Res = null;
            CirculatingData circulatingData = null;
            ReferenceSignal[] referenceSignals = null;
            MeasurementProcessing MeasProcessing = new MeasurementProcessing();
            List<Emitting> emittings = new List<Emitting>();
            int i = 0;
            Res = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR, sensor, ref circulatingData, Res, referenceSignals) as MeasSdrResults_v2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Тестирование получение таймштампов
            // Тестирование записи в IQ поток
            MeasSdrTask mEAS_TASK_SDR = new MeasSdrTask();
            mEAS_TASK_SDR.Id = 1;
            mEAS_TASK_SDR.MeasTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasTaskId.Value = 2;
            mEAS_TASK_SDR.MeasSubTaskId = new Atdi.AppServer.Contracts.Sdrns.MeasTaskIdentifier();
            mEAS_TASK_SDR.MeasSubTaskId.Value = 3;
            mEAS_TASK_SDR.MeasSubTaskStationId = 4;
            mEAS_TASK_SDR.MeasFreqParam = new Atdi.AppServer.Contracts.Sdrns.MeasFreqParam();
            mEAS_TASK_SDR.MeasFreqParam.Step = 200; // 10 кГц
            mEAS_TASK_SDR.MeasFreqParam.RgL = 952.2 - 0.200 / 2.0;
            mEAS_TASK_SDR.MeasFreqParam.RgU = 952.2 + 0.200 / 2.0;
            mEAS_TASK_SDR.MeasSDRParam = new MeasSdrParam();
            mEAS_TASK_SDR.MeasSDRParam.DetectTypeSDR = Atdi.AppServer.Contracts.Sdrns.DetectingType.Avarage;
            mEAS_TASK_SDR.MeasSDRParam.PreamplificationSDR = -1;
            mEAS_TASK_SDR.MeasSDRParam.RBW = 200; // 10 кГц
            mEAS_TASK_SDR.MeasSDRParam.VBW = 200; // 10 кГц
            mEAS_TASK_SDR.MeasSDRParam.ref_level_dbm = -40;
            mEAS_TASK_SDR.MeasSDRParam.RfAttenuationSDR = -1;
            mEAS_TASK_SDR.MeasSDRParam.MeasTime = 0.001;
            mEAS_TASK_SDR.MeasDataType = Atdi.AppServer.Contracts.Sdrns.MeasurementType.Program;
            mEAS_TASK_SDR.TypeM = Atdi.AppServer.Contracts.Sdrns.SpectrumScanType.Sweep;
            mEAS_TASK_SDR.MeasSDRSOParam = new MeasSdrSOParam();
            mEAS_TASK_SDR.NumberScanPerTask = -999;
            mEAS_TASK_SDR.Time_start = DateTime.Now - TimeSpan.FromSeconds(10);
            mEAS_TASK_SDR.Time_stop = DateTime.Now + TimeSpan.FromSeconds(65);
            mEAS_TASK_SDR.PerInterval = 55;
            mEAS_TASK_SDR.status = "A";
            SDRBB60C SDR = new SDRBB60C();
            Sensor sensor = new Sensor();
            MeasSdrResults_v2 Res = null;
            CirculatingData circulatingData = null;
            ReferenceSignal[] referenceSignals = null;
            MeasurementProcessing MeasProcessing = new MeasurementProcessing();
            List<Emitting> emittings = new List<Emitting>();
            int i = 0;
            Res = MeasProcessing.TaskProcessing(SDR, mEAS_TASK_SDR, sensor, ref circulatingData, Res, referenceSignals) as MeasSdrResults_v2;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string fileName = "C:\\TEMP\\GetTimestamp.txt";
            double Freq_MHz;
            int minut = 0 ;
            string[] allText = File.ReadAllLines(fileName);//чтение всех строк файла в массив строк
            minut = Convert.ToInt32(allText[0]);
            Freq_MHz = Convert.ToDouble(allText[1]);
            DateTime curTime = DateTime.Now;
            DateTime TimeStart = new DateTime(curTime.Year, curTime.Month, curTime.Day, curTime.Hour, minut, 0);
            
            // стартуем запись IQ потока 

            // заполняем таск 
            TaskParameters taskParameters = new TaskParameters();
            taskParameters.MinFreq_MHz = Freq_MHz - 0.1;
            taskParameters.MaxFreq_MHz = Freq_MHz + 0.1;
            taskParameters.TypeTechnology = GetTimeStamp.TypeTechnology.GSM;
            taskParameters.RBW_Hz = 200000;
            taskParameters.VBW_Hz = 200000;
            taskParameters.ReceivedIQStreemDuration_sec = 0.05;

            // Создаем сенсор для измерения
            SDRBB60C SDR = new SDRBB60C();
            SDR.Initiation();
            SDR.Calibration();
            // Создаем настройки для сенсора
            SDRParameters sDRParameters = new SDRParameters();
            sDRParameters.MeasurementType = MeasType.IQReceive;
            sDRParameters.MinFreq_MHz = taskParameters.MinFreq_MHz;
            sDRParameters.MaxFreq_MHz = taskParameters.MaxFreq_MHz;
            sDRParameters.PreamplificationSDR = -1;
            sDRParameters.RefLevel_dBm = -40;
            sDRParameters.RfAttenuationSDR = -1;
            sDRParameters.RBW_Hz = taskParameters.RBW_Hz;
            sDRParameters.VBW_Hz = taskParameters.VBW_Hz;
            if (SDR.GetSDRState() == SDRState.ReadyForMeasurements)
            {
                SDR.SetConfiguration(sDRParameters); // Конфигурируем сенсор
                if (SDR.SetConfiguration(sDRParameters))
                {
                    // Стартуем измерение 
                    ReceivedIQStream receivedIQStream = new ReceivedIQStream();
                    ReceivedIQStream receivedIQStream2 = new ReceivedIQStream();
                    DateTime now_time;
                    do
                    {
                        now_time = DateTime.Now;
                    }
                    while (now_time < TimeStart);
                    bool done = SDR.GetIQStream(ref receivedIQStream, taskParameters.ReceivedIQStreemDuration_sec, true);
                    done = SDR.GetIQStream(ref receivedIQStream2, taskParameters.ReceivedIQStreemDuration_sec, true);
                    if (done == true)
                    { // поток приняли !!!
                        GetTimeStamp TimeStamp = new GetTimeStamp(receivedIQStream, 40000000, 1000 * (taskParameters.MaxFreq_MHz - taskParameters.MinFreq_MHz), taskParameters.TypeTechnology);
                        GetTimeStamp TimeStamp2 = new GetTimeStamp(receivedIQStream2, 40000000, 1000 * (taskParameters.MaxFreq_MHz - taskParameters.MinFreq_MHz), taskParameters.TypeTechnology);
                        // Пишем в файл TimeShtamp
                        string st0 = "C:\\TEMP\\GSM900_1_";
                        string st02 = "C:\\TEMP\\GSM900_2_";
                        string st1 = Freq_MHz.ToString();
                        string st2 = "Time";
                        string st3 = TimeStart.ToString();
                        string st4 = ".txt";
                        string FileNameSaved = st1+st2+st3+st4;
                        string FileNameSaved1 = st0 + FileNameSaved.Replace(":", "_");
                        string FileNameSaved2 = st02 + FileNameSaved.Replace(":", "_");
                        SerializeObject(FileNameSaved1, TimeStamp);
                        SerializeObject(FileNameSaved2, TimeStamp);
                    }
                }
            }
        }
        private void SerializeObject(string File, object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(File, FileMode.OpenOrCreate);
            formatter.Serialize(fs, obj);
            fs.Close();
            fs.Dispose();
        }
        public object DeserializeObject(string File)
        {
            object obj = null;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(File, FileMode.OpenOrCreate);
            obj = formatter.Deserialize(fs);
            fs.Close();
            fs.Dispose();
            return obj;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            if (textBox2.Text == "")
            {
                textBox2.Text = filename;
            }
            else
            {
                if (textBox3.Text == "")
                {
                    textBox3.Text = filename;
                }
                else
                {
                    textBox2.Text = filename;
                    textBox3.Text = "";
                }
            }
            
            

        }

        private void button8_Click(object sender, EventArgs e)
        {
            GetTimeStamp TimeStamp1 = (DeserializeObject(textBox2.Text) as GetTimeStamp);
            GetTimeStamp TimeStamp2 = (DeserializeObject(textBox3.Text) as GetTimeStamp);
            EstimationTimeDelayBetweenTwoTimestamp estimationTimeDelayBetweenTwoTimestamp = new EstimationTimeDelayBetweenTwoTimestamp(TimeStamp1.IQStreamTimeStampBloks, TimeStamp2.IQStreamTimeStampBloks);
        }
    }
}

