using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServerTCP;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml.Serialization;
using Atdi.SDR.Server.MeasurementProcessing.SingleHound;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer.Contracts;
using Atdi.SDR.Server.MeasurementProcessing.SingleHound.ProcessSignal;
using Atdi.SDR.Server.MeasurementProcessing;
using Atdi.SDR.Server.MeasurementProcessing.Measurement;

namespace BB60C_
{
    public partial class Form1 : Form
    {
       // public SDR_BB60C SDR_test;
        private delegate void UpdateStatusCallback(string strMessage);
        private Server mainServer;
        public int id = -1;
        private bbStatus status {get; set;}
        private string AllText = "";
        public Thread thrListene;
        
            
        public Form1()
        {
            InitializeComponent();
            status = bbStatus.bbNoError;
            //thrListene = new Thread();
        }

        public void mainServer_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            //this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { e.EventMessage });
        }

        private void Listen()
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            mainServer = new Server(ipAddr);
            Server.StatusChanged += new StatusChangedEventHandler(mainServer_StatusChanged);
            mainServer.StartListening();
        }
        //private void UpdateStatus(string strMessage)
        //{
        //    /*AllText += strMessage;
        //    if (strMessage.Contains("<F_semples />"))
        //    {
        //        try
        //        {
        //            Application.DoEvents();
        //            AllText = AllText.Replace("</SDR_BB60C_test>", "");
        //            AllText += "\r\n" + "</SDR_BB60C_test>";
        //            String tempPath_answer = System.IO.Path.GetTempPath();
        //            string xml_file = string.Format(tempPath_answer + "\\{0}", Guid.NewGuid().ToString()) + ".xml";
        //            File.WriteAllText(xml_file, AllText, Encoding.UTF8);

        //            XmlSerializer ser = new XmlSerializer(typeof(SDR_BB60C));
        //            TextReader reader = new System.IO.StreamReader(xml_file, false);
        //            object vb = ser.Deserialize(reader);
        //            SDR_BB60C test_une_sw = (SDR_BB60C)vb;
        //            reader.Close();
        //            File.Delete(xml_file);
        //            AllText = "";
        //            if (test_une_sw.TypeFunction==1)
        //                F1(test_une_sw);
        //            if (test_une_sw.TypeFunction == 2)
        //                F2(test_une_sw);
        //            if (test_une_sw.TypeFunction == 3)
        //                F3(test_une_sw);
                 
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }

        //    }
        //     */
        //}


        //private void button1_Click(object sender, EventArgs e) 
        //{
        //     try
        //     {

        //        textBox1.Text += "Opening Device, Please Wait" + Environment.NewLine;
        //        status = bb_api.bbOpenDevice(ref id);
        //        if (status != bbStatus.bbNoError)
        //        {
        //            textBox1.Text += "Error: Unable to open BB60" + Environment.NewLine;
        //            textBox1.Text+=bb_api.bbGetStatusString(status) + Environment.NewLine;
        //            return;
        //        }
        //        else
        //        {
        //            textBox1.Text += "Device Found" + Environment.NewLine;
        //        }
        //        textBox1.Text += "API Version: " + bb_api.bbGetAPIString() + Environment.NewLine;
        //        textBox1.Text += "Device Type: " + bb_api.bbGetDeviceName(id) + Environment.NewLine;
        //        textBox1.Text += "Serial Number: " + bb_api.bbGetSerialString(id) + Environment.NewLine;
        //        textBox1.Text += "Firmware Version: " + bb_api.bbGetFirmwareString(id) + Environment.NewLine;


        //        float temp = 0.0F, voltage = 0.0F, current = 0.0F;
        //        bb_api.bbGetDeviceDiagnostics(id, ref temp, ref voltage, ref current);
        //        textBox1.Text += "Device Diagnostics" +
        //            "Temperature: " + temp.ToString() + " C" +
        //            "USB Voltage: " + voltage.ToString() + " V" +
        //            "USB Current: " + current.ToString() + " mA";


        //        textBox1.Text += "Configuring Device For a Sweep";
        //        bb_api.bbConfigureAcquisition(id, bb_api.BB_MIN_AND_MAX, bb_api.BB_LOG_SCALE);
        //        bb_api.bbConfigureCenterSpan(id, 1.0e9, 20.0e6);
        //        bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);
        //        bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);
        //        bb_api.bbConfigureSweepCoupling(id, 10.0e3, 10.0e3, 0.001,
        //            bb_api.BB_NON_NATIVE_RBW, bb_api.BB_NO_SPUR_REJECT);
        //        bb_api.bbConfigureProcUnits(id, bb_api.BB_LOG);

        //        status = bb_api.bbInitiate(id, bb_api.BB_SWEEPING, 0);
        //        if (status != bbStatus.bbNoError)
        //        {
        //            textBox1.Text += "Error: Unable to initialize BB60";
        //            textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
        //            return;
        //        }

        //        uint trace_len = 0;
        //        double bin_size = 0.0;
        //        double start_freq = 0.0;
        //        status = bb_api.bbQueryTraceInfo(id, ref trace_len, ref bin_size, ref start_freq);

        //        float[] sweep_max, sweep_min;
        //        sweep_max = new float[trace_len];
        //        sweep_min = new float[trace_len];

        //        bb_api.bbFetchTrace_32f(id, unchecked((int)trace_len), sweep_min, sweep_max);
        //        textBox1.Text += "Sweep Retrieved";

        //        textBox1.Text += "Configuring the deviceto stream I/Q data";
        //        bb_api.bbConfigureCenterSpan(id, 2400.0e6, 20.0e6);
        //        bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);
        //        bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);
        //        bb_api.bbConfigureIQ(id, bb_api.BB_MIN_DECIMATION, 20.0e6);

                

        //        status = bb_api.bbInitiate(id, bb_api.BB_STREAMING, bb_api.BB_STREAM_IQ);
        //        if (status != bbStatus.bbNoError)
        //        {
        //            textBox1.Text += "Error: Unable to initialize BB60 for streaming";
        //            textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
        //            return;
        //        }

        //        int return_len = 0;
        //        int samples_per_sec = 0;
        //        double bandwidth = 0.0;
        //        bb_api.bbQueryStreamInfo(id, ref return_len, ref bandwidth, ref samples_per_sec);
        //        textBox1.Text += "Initialized Stream for ";
        //        textBox1.Text += "Samples per second: " + (samples_per_sec / 1.0e6).ToString() + " MS/s";
        //        textBox1.Text += "Bandwidth: " + (bandwidth / 1.0e6).ToString() + " MHz";
        //        textBox1.Text += "Samples per function call: " + return_len.ToString() + Environment.NewLine;

        //        // Alternating I/Q samples
        //        // return_len is the number of I/Q pairs, so.. allocate twice as many floats
        //        float[] iq_samples = new float[return_len * 2];
        //        int[] triggers = new int[80];

        //        bb_api.bbFetchRaw(id, iq_samples, triggers);
        //        textBox1.Text += "Retrieved one I/Q packet";
        //        textBox1.Text += "Closing Device";
        //        bb_api.bbCloseDevice(id);
                
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //} //просто тестирование
        //private void button2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        textBox1.Text += "Opening Device, Please Wait" + Environment.NewLine;
        //        status = bb_api.bbOpenDevice(ref id);
        //        if (status != bbStatus.bbNoError)
        //        {
        //            textBox1.Text += "Error: Unable to open BB60" + Environment.NewLine;
        //            textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
        //            return;
        //        }
        //        else
        //        {
        //            textBox1.Text += "Device Found" + Environment.NewLine;
        //        }
        //        // типа должны проициализировать устройства 
        //        // ниже рабочее тело кода 
        //        Double f_min = 90000000;
        //        Double f_max = 110000000;

        //        bb_api.bbConfigureAcquisition(id, bb_api.BB_MIN_AND_MAX, bb_api.BB_LOG_SCALE);
        //        bb_api.bbConfigureCenterSpan(id, (f_max+f_min )/2, f_max- f_min);
        //        label3.Text = f_min.ToString();
        //        label4.Text = f_max.ToString();
        //        bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);
        //        bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);
        //                        bb_api.bbConfigureSweepCoupling(id, 10.0e3, 10.0e3, 0.001,
        //            bb_api.BB_NON_NATIVE_RBW, bb_api.BB_NO_SPUR_REJECT);
        //        bb_api.bbConfigureProcUnits(id, bb_api.BB_LOG);

        //        status = bb_api.bbInitiate(id, bb_api.BB_SWEEPING, 0);
        //        if (status != bbStatus.bbNoError)
        //        {
        //            textBox1.Text += "Error: Unable to initialize BB60";
        //            textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
        //            return;
        //        }

        //        uint trace_len = 0;
        //        double bin_size = 0.0;
        //        double start_freq = 0.0;
        //        status = bb_api.bbQueryTraceInfo(id, ref trace_len, ref bin_size, ref start_freq);

        //        float[] sweep_max, sweep_min;
        //        sweep_max = new float[trace_len];
        //        sweep_min = new float[trace_len];

        //        bb_api.bbFetchTrace_32f(id, unchecked((int)trace_len), sweep_min, sweep_max);
        //        textBox1.Text += "Sweep Retrieved";


        //        //конец рабочего тела.
        //        // гасим устройство 
        //        textBox1.Text += "Closing Device";
        //        bb_api.bbCloseDevice(id);
        //    }
        //    catch (Exception ex)
        //    {

        //    }

            
        //}

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
            g.DrawLine(pen, new Point(x1,y1),  new Point(x2,y2));
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
            drawLine(0,0,100,100);
            drawLine(100,100,100,240);
            drawPoint(240,240);
            drawPoint(240, 242);
            drawPoint(240, 246);
        }

        //private void button4_Click(object sender, EventArgs e)
        

        private void button5_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.Clear(pictureBox1.BackColor);
        }

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        textBox1.Text += "RT Opening Device, Please Wait" + Environment.NewLine;
        //        status = bb_api.bbOpenDevice(ref id);
        //        if (status != bbStatus.bbNoError)
        //        {
        //            textBox1.Text += "RT Error: Unable to open BB60" + Environment.NewLine;
        //            textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
        //            return;
        //        }
        //        else
        //        {
        //            textBox1.Text += "RT Device Found" + Environment.NewLine;
        //        }
        //        // типа должны проициализировать устройства 
        //        // ниже рабочее тело кода итак SW 
        //        Double f_min = 103000000;
        //        Double f_max = 105000000;
        //        Double Level_min = -110;
        //        Double Level_max = -10;
        //        Double Time = 0.001;
        //        Double RBW = 9863.28125;
        //        Double VBW = 9863.28125;

        //        bb_api.bbConfigureAcquisition(id, bb_api.BB_MIN_AND_MAX, bb_api.BB_LOG_SCALE);
        //        bb_api.bbConfigureCenterSpan(id, (f_max + f_min) / 2, f_max - f_min);
        //        label3.Text = (f_min / 1000000).ToString();
        //        label4.Text = (f_max / 1000000).ToString();
        //        label7.Text = Level_min.ToString();
        //        label8.Text = Level_max.ToString();
        //        bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);
        //        bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);
        //        bb_api.bbConfigureSweepCoupling(id, RBW, VBW, Time,
        //        bb_api.BB_NUTALL, bb_api.BB_NO_SPUR_REJECT);
        //        bb_api.bbConfigureRealTime(id, 100.0, 30);

        //        status = bb_api.bbInitiate(id, bb_api.BB_REAL_TIME, 0);
        //        if (status != bbStatus.bbNoError)
        //        {
        //            textBox1.Text += "RT Error: Unable to initialize BB60";
        //            textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
        //            return;
        //        }


        //        uint sweepsize = 0;
        //        double bin_size = 0.0;
        //        double start_freq = 0.0;
        //        status = bb_api.bbQueryTraceInfo(id, ref sweepsize, ref bin_size, ref start_freq);
        //        int frameWidth=0;
        //        int frameHeight=0;
        //        status = bb_api.bbQueryRealTimeInfo(id, ref frameWidth, ref frameHeight);
        //        float[] sweep, frame;
        //        sweep = new float[sweepsize];
        //        frame = new float[frameWidth*frameHeight];



        //        int frameCount = 0;
        //        while (frameCount++ < 100)
        //        {
        //            bb_api.bbFetchRealTimeFrame(id, sweep, frame);
        //            label9.Text = frameCount.ToString();


        //            for (int i = 0; i < sweepsize; i++)
        //            {
        //                Double x = 400 * i / sweepsize;
        //                Double y_min = (-sweep[i] + Level_max) * 400 / (Level_max - Level_min);
        //                //Double y_max = (-sweep[i] + Level_max) * 400 / (Level_max - Level_min);
        //                drawPoint((int)x, (int)y_min);
        //                //drawPoint((int)x, (int)(y_max / 2 + y_max / 2));
        //            }
        //        }

        //        // Конец рабочего тела оборудования
        //        // Закрываем оборудование
        //        textBox1.Text += "Closing Device";
        //        bb_api.bbCloseDevice(id);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void button7_Click(object sender, EventArgs e)
        //{
        //    /*string err = "No error";
        //    SDR_BB60C test_une_sw = new SDR_BB60C();
        //    /*
        //    test_une_sw.f_min = 102; // берем из формы
        //    test_une_sw.f_max = 105; // берем из формы
        //    test_une_sw.ref_level_dbm = -20; // константа
        //    test_une_sw.VBW = 10000; // берем из формы
        //    test_une_sw.RBW = 10000; // берем из формы
        //    int sw_time = 50;//  берем из формы
        //    string Type_of_m = "RT"; // тип режима измерения. Есть 2 режима. SW и RT 
        //     */
        //    /*
        //    test_une_sw.f_min = 100; // берем из формы
        //    test_une_sw.f_max = 110; // берем из формы
        //    test_une_sw.ref_level_dbm = -20; // константа
        //    test_une_sw.VBW = 10000; // берем из формы
        //    test_une_sw.RBW = 10000; // берем из формы
        //    int sw_time = 5;//  берем из формы
        //    string Type_of_m = "RT"; // тип режима измерения. Есть 2 режима. SW и RT 


        //    test_une_sw.initiation_SDR();
        //    if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of initialization"; test_une_sw.Close_dev(); return;}
        //    test_une_sw.calibration();
        //    if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of calibration"; test_une_sw.Close_dev(); return; }
        //    if (Type_of_m == "SW")
        //    {
        //        test_une_sw.put_config_for_sweep();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for SW"; test_une_sw.Close_dev(); return; }
        //        test_une_sw.Sweep(sw_time);
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of SW"; test_une_sw.Close_dev(); return; }
        //    }
        //    else if (Type_of_m == "RT")
        //    {
        //        test_une_sw.put_config_for_RT();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for RT"; test_une_sw.Close_dev(); return; }
        //        test_une_sw.Real_time(sw_time);
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of RT"; test_une_sw.Close_dev(); return; }
        //    }
        //    test_une_sw.Close_dev();

        //    // в данной точке результат находится в переменой test_une_sw.F_semples мы ее возвращаем только и всего а также возвращаем err




        //    // ниже код нужен только для тестов его после прибить
        //    Double Level_min = -110;
        //    Double Level_max = -10;
        //    for (int i = 0; i < test_une_sw.F_semples.Count-1; i++)
        //    {
        //        Double x = 400 * i / (test_une_sw.F_semples.Count - 1);
        //        Double y_max = (-test_une_sw.F_semples[i].Level_dBm + Level_max) * 400 / (Level_max - Level_min);
        //        drawPoint((int)x, (int)(y_max / 2 + y_max / 2));
        //    }


        //    */
        //}


        //private void F1(SDR_BB60C input)
        //{
        //    /*
        //    try
        //    {
        //        string err = "No error";
        //        SDR_BB60C test_une_sw = new SDR_BB60C();
        //        test_une_sw.f_min = input.f_min; // берем из формы
        //        test_une_sw.f_max = input.f_max; // берем из формы
        //        test_une_sw.ref_level_dbm = -20; // константа
        //        test_une_sw.VBW = input.VBW; // берем из формы
        //        test_une_sw.RBW = input.RBW; // берем из формы
        //        int sw_time = input.sw_time;//  берем из формы
        //        string Type_of_m = input.Type_of_m;//  берем из формы

        //        test_une_sw.TypeFunction = 1;

        //        test_une_sw.initiation_SDR();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of initialization"; test_une_sw.isError = true; test_une_sw.isAbort = true; }
        //        test_une_sw.calibration();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of calibration"; test_une_sw.isError = true; test_une_sw.isAbort = true; }
        //        if (Type_of_m == "SW")
        //        {
        //            test_une_sw.put_config_for_sweep();
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for SW"; test_une_sw.isError = true; test_une_sw.isAbort = true; }
        //            test_une_sw.Sweep(sw_time);
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of SW"; test_une_sw.isError = true; test_une_sw.isAbort = true; }
        //        }
        //        else if (Type_of_m == "RT")
        //        {
        //            test_une_sw.put_config_for_RT();
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for RT"; test_une_sw.isError = true; test_une_sw.isAbort = true; }
        //            test_une_sw.Real_time(sw_time);
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of RT"; test_une_sw.isError = true; test_une_sw.isAbort = true; }
        //        }
        //        test_une_sw.Close_dev();

        //        if (test_une_sw.F_semples.Count > 0)
        //            test_une_sw.F_semples = CorrectMass(input.N, test_une_sw.F_semples);

        //        //Сериализация объекта и отправка на клиента
        //        String tempPath_answer = System.IO.Path.GetTempPath();
        //        string xml_file = string.Format(tempPath_answer + "\\{0}", Guid.NewGuid().ToString()) + ".xml";
        //        XmlSerializer ser = new XmlSerializer(typeof(SDR_BB60C));
        //        var writer = new System.IO.StreamWriter(xml_file, false);
        //        ser.Serialize(writer, test_une_sw);
        //        writer.Flush();
        //        writer.Close();
        //        string nString = File.ReadAllText(xml_file, Encoding.UTF8);
        //        Server.SendAdminMessage(nString.ToString());
        //        File.Delete(xml_file);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    */
        //}

        //private void button8_Click(object sender, EventArgs e)
        //{
        //    /*
        //    string err = "No error";
        //    SDR_BB60C test_une_sw = new SDR_BB60C();
        //    test_une_sw.f_min = 102; // берем из формы
        //    test_une_sw.f_max = 105; // берем из формы
        //    test_une_sw.ref_level_dbm = -20; // константа
        //    test_une_sw.VBW = 10000; // берем из формы
        //    test_une_sw.RBW = 10000; // берем из формы

        //    int sw_time = 5;//  берем из формы
        //    string Type_of_m = "RT"; // тип режима измерения. Есть 2 режима. SW и RT 


        //    test_une_sw.initiation_SDR();
        //    if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of initialization"; return; }
        //    test_une_sw.calibration();
        //    if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of calibration"; return; }
        //    if (Type_of_m == "SW")
        //    {
        //        test_une_sw.put_config_for_sweep();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for SW"; return; }

        //        int count = 0;
        //        while (count++<50)
        //        {
        //                test_une_sw.Sweep(sw_time);
        //                if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of SW"; return; }
        //            // в данной точке результат находится в переменой test_une_sw.F_semples мы ее возвращаем только и всего а также возвращаем err 
        //            // кстати это происходит у нас циклически
        //            /// ниже код нужен только для тестов его после прибить
        //                Double Level_min = -110;
        //                Double Level_max = -10;
        //                for (int i = 0; i < test_une_sw.F_semples.Count - 1; i++)
        //                {
        //                    Double x = 400 * i / (test_une_sw.F_semples.Count - 1);
        //                    Double y_max = (-test_une_sw.F_semples[i].Level_dBm + Level_max) * 400 / (Level_max - Level_min);
        //                    drawPoint((int)x, (int)(y_max / 2 + y_max / 2));
        //                }
        //        }


        //    }
        //    else if (Type_of_m == "RT")
        //    {
        //        test_une_sw.put_config_for_RT();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for RT"; return; }

        //        int count = 0;
        //        while (count++ < 50)
        //        {
        //            test_une_sw.Real_time(sw_time);
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of RT"; return; }
        //            // в данной точке результат находится в переменой test_une_sw.F_semples мы ее возвращаем только и всего а также возвращаем err 
        //            // кстати это происходит у нас циклически
        //            /// ниже код нужен только для тестов его после прибить
        //            Double Level_min = -110;
        //            Double Level_max = -10;
        //            for (int i = 0; i < test_une_sw.F_semples.Count - 1; i++)
        //            {
        //                Double x = 400 * i / (test_une_sw.F_semples.Count - 1);
        //                Double y_max = (-test_une_sw.F_semples[i].Level_dBm + Level_max) * 400 / (Level_max - Level_min);
        //                drawPoint((int)x, (int)(y_max / 2 + y_max / 2));
        //            }
        //        }


        //    }
        //    test_une_sw.Close_dev();
        //    */
        //}

        ////private void F2(SDR_BB60C input)
        //{
        //    /*
        //    try
        //    {
        //        string err = "No error";
        //        SDR_BB60C test_une_sw = new SDR_BB60C();
        //        test_une_sw.f_min = input.f_min; // берем из формы
        //        test_une_sw.f_max = input.f_max; // берем из формы
        //        test_une_sw.ref_level_dbm = -20; // константа
        //        test_une_sw.VBW = input.VBW; // берем из формы
        //        test_une_sw.RBW = input.RBW; // берем из формы
        //        int sw_time = input.sw_time;//  берем из формы
        //        string Type_of_m = input.Type_of_m;//  берем из формы
        //        mainServer.isAbort = false;
        //        test_une_sw.TypeFunction = 2;
        //        test_une_sw.initiation_SDR();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of initialization"; test_une_sw.isAbort = true; test_une_sw.isError = true; }
        //        test_une_sw.calibration();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of calibration"; test_une_sw.isAbort = true; test_une_sw.isError = true; }
        //        if (Type_of_m == "RT")
        //        {
        //            test_une_sw.put_config_for_RT();
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for RT"; test_une_sw.isAbort = true; test_une_sw.isError = true; }

        //            double c = 1.02;
        //            while (true)
        //            {
        //                Application.DoEvents();
        //                if (mainServer.isAbt()) break;


        //                test_une_sw.Real_time(sw_time);
        //                if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of SW"; test_une_sw.isAbort = true; test_une_sw.isError = true; }

        //                if (test_une_sw.F_semples.Count > 0)
        //                    test_une_sw.F_semples = CorrectMass(input.N, test_une_sw.F_semples);


        //                // в данной точке результат находится в переменой test_une_sw.F_semples мы ее возвращаем только и всего а также возвращаем err 
        //                // кстати это происходит у нас циклически
        //                /// ниже код нужен только для тестов его после прибить
        //                //Сериализация объекта и отправка на клиента
        //                String tempPath_answer = System.IO.Path.GetTempPath();
        //                string xml_file = string.Format(tempPath_answer + "\\{0}", Guid.NewGuid().ToString()) + ".xml";
        //                XmlSerializer ser = new XmlSerializer(typeof(SDR_BB60C));
        //                var writer = new System.IO.StreamWriter(xml_file, false);
        //                ser.Serialize(writer, test_une_sw);
        //                writer.Flush();
        //                writer.Close();
        //                string nString = File.ReadAllText(xml_file, Encoding.UTF8);
        //                Server.SendAdminMessage(nString.ToString());
        //                File.Delete(xml_file);

        //            }
        //        }
        //        else if (Type_of_m == "SW")
        //        {
        //            test_une_sw.put_config_for_sweep();
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for SW"; test_une_sw.isAbort = true; test_une_sw.isError = true; }

        //            double c = 1.02;
        //            while (true)
        //            {
        //                Application.DoEvents();
        //                if (mainServer.isAbt()) break;



        //                test_une_sw.Sweep(sw_time);
        //                if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of SW"; test_une_sw.isAbort = true; test_une_sw.isError = true; }

        //                if (test_une_sw.F_semples.Count > 0)
        //                    test_une_sw.F_semples = CorrectMass(input.N, test_une_sw.F_semples);


        //                // в данной точке результат находится в переменой test_une_sw.F_semples мы ее возвращаем только и всего а также возвращаем err 
        //                // кстати это происходит у нас циклически
        //                /// ниже код нужен только для тестов его после прибить
        //                //Сериализация объекта и отправка на клиента
        //                String tempPath_answer = System.IO.Path.GetTempPath();
        //                string xml_file = string.Format(tempPath_answer + "\\{0}", Guid.NewGuid().ToString()) + ".xml";
        //                XmlSerializer ser = new XmlSerializer(typeof(SDR_BB60C));
        //                var writer = new System.IO.StreamWriter(xml_file, false);
        //                ser.Serialize(writer, test_une_sw);
        //                writer.Flush();
        //                writer.Close();
        //                string nString = File.ReadAllText(xml_file, Encoding.UTF8);
        //                Server.SendAdminMessage(nString.ToString());
        //                File.Delete(xml_file);

        //            }
        //        }
        //        test_une_sw.Close_dev();

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    */
        //}

        //private void F3(SDR_BB60C input)
        //{
        //    /*
        //    try
        //    {
        //        mainServer.isAbort = false;
        //        // Начало работы процедуры Spectr Ocup
        //        string err = "No error";
        //        SDR_BB60C test_une_sw = new SDR_BB60C();
        //        //test_une_sw.List_freq_CH = new List<Double>() { 100.5, 100.7, 100.8, 100.9, 100, 100.1, 100.2, 100.3, 101 };// формируема на основании канального плана  
        //        test_une_sw.List_freq_CH = input.List_freq_CH;
        //        int sw_time = input.sw_time;//  берем из формы
        //        string Type_of_m = input.Type_of_m;//  берем из формы
        //        test_une_sw.BW_CH = input.BW_CH;
        //        test_une_sw.ref_level_dbm = -20; // константа
        //        test_une_sw.n_in_chenal = input.n_in_chenal;
        //        test_une_sw.Level_min_occup = input.Level_min_occup;
        //        test_une_sw.Type_of_SO = input.Type_of_SO;
        //        // формируем начало и конец для измерений 
        //        test_une_sw.TypeFunction = 3;
        //        test_une_sw.List_freq_CH.Sort();
        //        test_une_sw.f_min = input.f_min;
        //        test_une_sw.f_max = input.f_max;
        //        //test_une_sw.f_min = test_une_sw.List_freq_CH[0] - test_une_sw.BW_CH / 2000;
        //        //test_une_sw.f_max = test_une_sw.List_freq_CH[test_une_sw.List_freq_CH.Count - 1] + test_une_sw.BW_CH / 2000;
        //        // расчитываем желаемое RBW и VBW
        //        test_une_sw.VBW = test_une_sw.BW_CH * 1000 / test_une_sw.n_in_chenal;
        //        test_une_sw.RBW = test_une_sw.BW_CH * 1000 / test_une_sw.n_in_chenal;

        //        // коректировка режима измерения 
        //        if ((Type_of_m == "RT") && ((test_une_sw.f_max - test_une_sw.f_min > 20) || ((test_une_sw.f_max - test_une_sw.f_min) * 1000 / test_une_sw.RBW > 8000))) { Type_of_m = "SW"; }


        //        test_une_sw.initiation_SDR();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of initialization"; test_une_sw.isAbort = true; test_une_sw.isError = true; }
        //        test_une_sw.calibration();
        //        if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of calibration"; test_une_sw.isAbort = true; test_une_sw.isError = true; }
        //        // настройка 
        //        if (Type_of_m == "SW")
        //        {
        //            test_une_sw.put_config_for_sweep();
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for SW"; test_une_sw.isAbort = true; test_une_sw.isError = true; }
        //        }
        //        else if (Type_of_m == "RT")
        //        {
        //            test_une_sw.put_config_for_RT();
        //            if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for RT"; test_une_sw.isAbort = true; test_une_sw.isError = true; }
        //        }
        //        // вот и настроили
        //        int count = 0;
        //        List<f_semples> F_ch_res = new List<f_semples>();
        //        while (true)
        //        {
        //            count++;
        //            Application.DoEvents();
        //            if (mainServer.isAbt()) break;

        //            // сохраняем предыдущий результат если это не первый замер
        //            List<f_semples> F_temp = new List<f_semples>();
        //            if (count != 1) { F_temp = test_une_sw.F_semples; }

        //            // замер 
        //            if (Type_of_m == "SW") { test_une_sw.Sweep(sw_time); if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of SW"; test_une_sw.isAbort = true; test_une_sw.isError = true; } }
        //            else if (Type_of_m == "RT") { test_une_sw.Real_time(sw_time); if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of RT"; test_une_sw.isAbort = true; test_une_sw.isError = true; } }
        //            // замер выполнен он находится в  test_une_sw.F_semple

        //            // дополняем замер значениями SO и прочим теперь значения красивые по микроканальчикам
        //            for (int i = 0; i <= test_une_sw.F_semples.Count - 1; i++)
        //            {
        //                test_une_sw.F_semples[i].Level_max_dBm = test_une_sw.F_semples[i].Level_dBm;
        //                test_une_sw.F_semples[i].Level_min_dBm = test_une_sw.F_semples[i].Level_dBm;
        //                if (test_une_sw.F_semples[i].Level_dBm > test_une_sw.Level_min_occup)
        //                { test_une_sw.F_semples[i].Ocupation_pt = 100; }
        //                else { test_une_sw.F_semples[i].Ocupation_pt = 0; }
        //            }
        //            // Вот и дополнили значениями SO и прочим теперь значения красивые по микроканальчикам
        //            // Вычисляем занятость для данного замера по каналам 
        //            List<f_semples> F_ch_res_temp = new List<f_semples>(); // здест будут храниться замеры приведенные к каналу
        //            int start = 0;
        //            for (int i = 0; i <= test_une_sw.List_freq_CH.Count - 1; i++) // Цикл по каналам
        //            {
        //                f_semples F_SO = new f_semples(); // здесь будет храниться один замер приведенный к каналу
        //                int sempl_in_freq = 0; //количество замеров идущие в один канал 
        //                for (int j = start; j <= test_une_sw.F_semples.Count - 1; j++) // цикл по замерам по канальчикам
        //                {
        //                    if (test_une_sw.List_freq_CH[i] + test_une_sw.BW_CH / 2000 < test_une_sw.F_semples[j].Freq) { start = j; break; }
        //                    if ((test_une_sw.List_freq_CH[i] - test_une_sw.BW_CH / 2000 <= test_une_sw.F_semples[j].Freq) && (test_une_sw.List_freq_CH[i] + test_une_sw.BW_CH / 2000 > test_une_sw.F_semples[j].Freq)) // проверка на попадание в диапазон частот
        //                    {
        //                        sempl_in_freq = sempl_in_freq + 1;
        //                        if (sempl_in_freq == 1)// заполняем первое попадание как есть
        //                        {
        //                            F_SO.Freq = test_une_sw.List_freq_CH[i];
        //                            F_SO.Level_dBm = test_une_sw.F_semples[j].Level_dBm;
        //                            if (test_une_sw.Type_of_SO == "FBO") // частотная занятость
        //                            {
        //                                if (test_une_sw.F_semples[j].Level_dBm > test_une_sw.Level_min_occup + 10 * Math.Log10(test_une_sw.RBW / (test_une_sw.BW_CH * 1000)))
        //                                { F_SO.Ocupation_pt = 100; }
        //                            }
        //                        }
        //                        else // накапливаем уровень синнала
        //                        {
        //                            F_SO.Level_dBm = Math.Pow(10, F_SO.Level_dBm / 10) + Math.Pow(10, test_une_sw.F_semples[j].Level_dBm / 10);
        //                            F_SO.Level_dBm = 10 * Math.Log10(F_SO.Level_dBm);
        //                            if (test_une_sw.Type_of_SO == "FBO") // частотная занятость //накапливаем
        //                            {
        //                                if (test_une_sw.F_semples[j].Level_dBm > test_une_sw.Level_min_occup + 10 * Math.Log10(test_une_sw.RBW / (test_une_sw.BW_CH * 1000)))
        //                                { F_SO.Ocupation_pt = F_SO.Ocupation_pt + 100; }
        //                            }
        //                        }
        //                    }
        //                }
        //                if (test_une_sw.Type_of_SO == "FBO") { F_SO.Ocupation_pt = F_SO.Ocupation_pt / sempl_in_freq; }
        //                if (test_une_sw.Type_of_SO == "FCO") { if (F_SO.Level_dBm > test_une_sw.Level_min_occup) { F_SO.Ocupation_pt = 100; } }
        //                F_SO.Level_max_dBm = F_SO.Level_dBm;
        //                F_SO.Level_min_dBm = F_SO.Level_dBm;
        //                //F_SO на данный момент готов
        //                F_ch_res_temp.Add(F_SO); // добавляем во временный масив данные.
        //            }
        //            // данные единичного замера приведенного к каналам находятся здесь F_ch_res_temp    
        //            // Собираем статистику  в F_ch_res
        //            try
        //            {
        //                if (count == 1)
        //                { F_ch_res = F_ch_res_temp; }
        //                else
        //                {
        //                    if (F_ch_res.Count != F_ch_res_temp.Count)
        //                    {
        //                        int q = 1;
        //                    }
        //                   // F_ch_res = F_ch_res_temp;
        //                    for (int i = 0; i <= F_ch_res.Count - 1; i++)
        //                    {
        //                        F_ch_res[i].Level_dBm = (count * F_ch_res[i].Level_dBm + F_ch_res_temp[i].Level_dBm) / (count + 1);
        //                        F_ch_res[i].Ocupation_pt = (count * F_ch_res[i].Ocupation_pt + F_ch_res_temp[i].Ocupation_pt) / (count + 1);
        //                        if (F_ch_res[i].Level_max_dBm < F_ch_res_temp[i].Level_max_dBm) { F_ch_res[i].Level_max_dBm = F_ch_res_temp[i].Level_max_dBm; }
        //                        if (F_ch_res[i].Level_min_dBm > F_ch_res_temp[i].Level_min_dBm) { F_ch_res[i].Level_min_dBm = F_ch_res_temp[i].Level_min_dBm; }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            test_une_sw.F_semples.Clear();
        //            test_une_sw.F_semples.AddRange(F_ch_res);
        //            test_une_sw.N = count;
        //            //test_une_sw.F_semples = F_ch_res;

        //              //  var test_une_sw_tmp = new SDR_BB60C_test();
        //              //  test_une_sw_tmp = test_une_sw;
        //             //  test_une_sw_tmp.F_semples = F_ch_res;




        //            // в данной точке результат находится в переменой test_une_sw.F_semples мы ее возвращаем только и всего а также возвращаем err 
        //            // кстати это происходит у нас циклически
        //            /// ниже код нужен только для тестов его после прибить
        //            //Сериализация объекта и отправка на клиента
        //            String tempPath_answer = System.IO.Path.GetTempPath();
        //            string xml_file = string.Format(tempPath_answer + "\\{0}", Guid.NewGuid().ToString()) + ".xml";
        //            XmlSerializer ser = new XmlSerializer(typeof(SDR_BB60C));
        //            var writer = new System.IO.StreamWriter(xml_file, false);
        //            ser.Serialize(writer, test_une_sw);
        //            writer.Flush();
        //            writer.Close();
        //            string nString = File.ReadAllText(xml_file, Encoding.UTF8);
        //            Server.SendAdminMessage(nString.ToString());
        //            File.Delete(xml_file);


        //           // break;
        //            // в данной точке результат находится в переменой F_ch_res и в count мы его должны показать/запомнить.  
        //            // кстати это происходит у нас циклически

        //        }
        //        test_une_sw.Close_dev();
        //    }
        //    catch (Exception ex)
        //    {
        //        //test_une_sw.Close_dev();
        //    }
        //     */
        //}

        //private List<FSemples> CorrectMass(int MAX_N, List<FSemples> semples)
        //{
        //    List<FSemples> OutMass = new List<FSemples>();
        //    if (semples.Count>MAX_N) {
        //        double Val = semples.Count / MAX_N;
        //        double N = Math.Ceiling(Val);
        //        int NN = (int)N;
        //        if (NN > 1) {
        //            int index = 0;
        //            while (index < semples.Count){
        //                if ((index % N) == 0) {
        //                    if (OutMass.Count<MAX_N)
        //                        OutMass.Add(semples[index]);
        //                }
        //                index = index + 1;
        //            }
        //        }
        //        else OutMass = semples;
        //    }
        //    else OutMass = semples;
        //    return OutMass;
        //}

        private void Form1_Shown(object sender, EventArgs e)
        {
            Listen();
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainServer.Close();
        }

        //        private void button10_Click(object sender, EventArgs e)
        //        {
        //            /*
        //            // Начало работы процедуры Spectr Ocup
        //                string err = "No error";
        //                SDR_BB60C test_une_sw = new SDR_BB60C();
        //                test_une_sw.List_freq_CH = new List<Double>() { 100.5, 100.7, 100.8, 100.9, 100, 100.1, 100.2, 100.3, 101, 101.1, 101.2, 101.3, 101.4, 101.5, 101.6, 101.7, 101.8, 101.9, 102.0, 103.1, 103.2, 103.3, 103.4, 103.5, 103.6, 103.7, 103.8, 103.9, 104.0 };// формируема на основании канального плана  
        //                test_une_sw.BW_CH = 100; // формируема на основании канального плана 
        //                int sw_time = 5;//  берем из формы
        //                string Type_of_m = "SW"; // тип режима измерения. Есть 2 режима. SW и RT
        //                test_une_sw.ref_level_dbm = -20; // константа
        //                test_une_sw.n_in_chenal = 10;//показывает сколько измерений надо сделать в канале; данную штуку надо засунуть в файл конфигурации
        //                test_une_sw.Level_min_occup = -80; // берем из формы
        //                test_une_sw.Type_of_SO = "FBO"; //берем из формы
        //                // формируем начало и конец для измерений 
        //                test_une_sw.List_freq_CH.Sort();
        //                test_une_sw.f_min = test_une_sw.List_freq_CH[0]-test_une_sw.BW_CH/2000;
        //                test_une_sw.f_max = test_une_sw.List_freq_CH[test_une_sw.List_freq_CH.Count-1] + test_une_sw.BW_CH / 2000;
        //                // расчитываем желаемое RBW и VBW
        //                test_une_sw.VBW = test_une_sw.BW_CH * 1000 / test_une_sw.n_in_chenal;
        //                test_une_sw.RBW = test_une_sw.BW_CH * 1000 / test_une_sw.n_in_chenal;

        //                // коректировка режима измерения 
        //                if ((Type_of_m == "RT") && ((test_une_sw.f_max - test_une_sw.f_min > 20) || ((test_une_sw.f_max - test_une_sw.f_min) * 1000 / test_une_sw.RBW > 8000))) { Type_of_m = "SW";} 


        //                test_une_sw.initiation_SDR();
        //                if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of initialization"; return; }
        //                test_une_sw.calibration();
        //                if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of calibration"; return; }
        //                // настройка 
        //                if (Type_of_m == "SW") 
        //                {
        //                    test_une_sw.put_config_for_sweep();
        //                    if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for SW"; return; }
        //                }
        //                 else if (Type_of_m == "RT")
        //                {
        //                     test_une_sw.put_config_for_RT();
        //                     if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of config for RT"; return; }
        //                }
        //                // вот и настроили
        //                int count = 0;
        //                List<f_semples> F_ch_res = new List<f_semples>();
        //                while (count++ < 50)  
        //                {
        //                    // сохраняем предыдущий результат если это не первый замер
        //                    List<f_semples> F_temp = new List<f_semples>();
        //                    if (count != 1) {F_temp = test_une_sw.F_semples;}

        //                    // замер 
        //                    if (Type_of_m == "SW") {test_une_sw.Sweep(sw_time);if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of SW"; return; }}
        //                    else if (Type_of_m == "RT"){test_une_sw.Real_time(sw_time);if (test_une_sw.status != bbStatus.bbNoError) { err = "Error of RT"; return; }}
        //                    // замер выполнен он находится в  test_une_sw.F_semple

        //                    // дополняем замер значениями SO и прочим теперь значения красивые по микроканальчикам
        //                    for (int i = 0; i<test_une_sw.F_semples.Count-1; i++)
        //                    {
        //                        test_une_sw.F_semples[i].Level_max_dBm = test_une_sw.F_semples[i].Level_dBm;
        //                        test_une_sw.F_semples[i].Level_min_dBm = test_une_sw.F_semples[i].Level_dBm;
        //                        if (test_une_sw.F_semples[i].Level_dBm > test_une_sw.Level_min_occup)
        //                        {test_une_sw.F_semples[i].Ocupation_pt = 100;}
        //                        else{test_une_sw.F_semples[i].Ocupation_pt = 0;}
        //                    }
        //                    // Вот и дополнили значениями SO и прочим теперь значения красивые по микроканальчикам
        //                    // Вычисляем занятость для данного замера по каналам 
        //                    List<f_semples> F_ch_res_temp = new List<f_semples>(); // здест будут храниться замеры приведенные к каналу
        //                    int start = 0;
        //                    for (int i = 0; i < test_une_sw.List_freq_CH.Count - 1; i++) // Цикл по каналам
        //                    {
        //                        f_semples F_SO = new f_semples(); // здесь будет храниться один замер приведенный к каналу
        //                        int sempl_in_freq = 0; //количество замеров идущие в один канал 
        //                        for (int j = start; j < test_une_sw.F_semples.Count - 1; j++) // цикл по замерам по канальчикам
        //                        {
        //                            if (test_une_sw.List_freq_CH[i] + test_une_sw.BW_CH/2000 < test_une_sw.F_semples[j].Freq) {start = j; break;}
        //                            if ((test_une_sw.List_freq_CH[i] - test_une_sw.BW_CH / 2000 <= test_une_sw.F_semples[j].Freq) && (test_une_sw.List_freq_CH[i] + test_une_sw.BW_CH / 2000 > test_une_sw.F_semples[j].Freq)) // проверка на попадание в диапазон частот
        //                            {
        //                                sempl_in_freq = sempl_in_freq+1;
        //                                if (sempl_in_freq == 1)// заполняем первое попадание как есть
        //                                {
        //                                   F_SO.Freq = test_une_sw.List_freq_CH[i];
        //                                   F_SO.Level_dBm = test_une_sw.F_semples[j].Level_dBm;
        //                                   if (test_une_sw.Type_of_SO == "FBO") // частотная занятость
        //                                   {
        //                                       if (test_une_sw.F_semples[j].Level_dBm > test_une_sw.Level_min_occup + 10*Math.Log10(test_une_sw.RBW/(test_une_sw.BW_CH*1000)))
        //                                       {F_SO.Ocupation_pt = 100;}
        //                                   }
        //                                }
        //                                else // накапливаем уровень синнала
        //                                {
        //                                   F_SO.Level_dBm = Math.Pow(10,F_SO.Level_dBm/10) + Math.Pow(10,test_une_sw.F_semples[j].Level_dBm/10);
        //                                   F_SO.Level_dBm = 10 *Math.Log10(F_SO.Level_dBm);
        //                                   if (test_une_sw.Type_of_SO == "FBO") // частотная занятость //накапливаем
        //                                   {
        //                                       if (test_une_sw.F_semples[j].Level_dBm > test_une_sw.Level_min_occup + 10*Math.Log10(test_une_sw.RBW/(test_une_sw.BW_CH*1000)))
        //                                       {F_SO.Ocupation_pt = F_SO.Ocupation_pt + 100;}
        //                                   }
        //                                }
        //                            }     
        //                        }
        //                        if (test_une_sw.Type_of_SO == "FBO") {F_SO.Ocupation_pt = F_SO.Ocupation_pt/sempl_in_freq;}
        //                        if (test_une_sw.Type_of_SO == "FCO") {if (F_SO.Level_dBm >test_une_sw.Level_min_occup) {F_SO.Ocupation_pt =100;}}
        //                        F_SO.Level_max_dBm = F_SO.Level_dBm;
        //                        F_SO.Level_min_dBm = F_SO.Level_dBm;
        //                        //F_SO на данный момент готов
        //                        F_ch_res_temp.Add(F_SO); // добавляем во временный масив данные.
        //                  }
        //                  // данные единичного замера приведенного к каналам находятся здесь F_ch_res_temp    
        //                  // Собираем статистику  в F_ch_res
        //                  if (count ==1)
        //                  {F_ch_res = F_ch_res_temp;}
        //                  else
        //                  {
        //                      for (int i = 0; i < F_ch_res.Count - 1;i++)
        //                      {
        //                          F_ch_res[i].Level_dBm = (count*F_ch_res[i].Level_dBm + F_ch_res_temp[i].Level_dBm)/(count +1);
        //                          F_ch_res[i].Ocupation_pt = (count*F_ch_res[i].Ocupation_pt + F_ch_res_temp[i].Ocupation_pt)/(count +1);
        //                          if (F_ch_res[i].Level_max_dBm < F_ch_res_temp[i].Level_max_dBm) {F_ch_res[i].Level_max_dBm = F_ch_res_temp[i].Level_max_dBm;}
        //                          if (F_ch_res[i].Level_min_dBm > F_ch_res_temp[i].Level_min_dBm) {F_ch_res[i].Level_min_dBm = F_ch_res_temp[i].Level_min_dBm;}
        //                      }
        //                  }

        //                 // в данной точке результат находится в переменой F_ch_res и в count мы его должны показать/запомнить.  
        //                  // кстати это происходит у нас циклически

        //              }
        //             test_une_sw.Close_dev();
        //             */
        //       }

        //        private void button11_Click(object sender, EventArgs e)
        //        {

        //        }

        //        private void button12_Click(object sender, EventArgs e)
        //        {


        //            MEAS_TASK_SDR mEAS_TASK_SDR = new MEAS_TASK_SDR();
        //            mEAS_TASK_SDR.ident_task = "1234586";
        //            mEAS_TASK_SDR.id_meas_sub_task_st = "99094";
        //            mEAS_TASK_SDR.MEAS_SDR = new MEAS_SDR();
        //            mEAS_TASK_SDR.MEAS_SDR.IDENT = "BBC60C_123";
        //            mEAS_TASK_SDR.MEAS_SDR.Type_SDR = "BBC60C";
        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM = new MEAS_SDR_FREQ_PARAM();
        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.BW_CH = 50; // 50 кГц
        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.f_min = 95;
        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.f_max = 105;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM = new MEAS_SDR_PARAM();
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.DETECT_TYPE = "Average";
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.PREAMPLIFICATION = 0;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.RBW = 50;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.VBW = 50;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.ref_level_dbm = -20;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.RF_ATTENUATION = 0;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.Time_of_m = 10;
        //            mEAS_TASK_SDR.status = "A";
        //            mEAS_TASK_SDR.sw_time = 10;
        //            mEAS_TASK_SDR.TypeFunction = "Scan";
        //            mEAS_TASK_SDR.Type_of_m = "RT";

        //            SDR_test.process_meas_BB60C(mEAS_TASK_SDR);
        //            MEAS_SDR_RESULTS results = new MEAS_SDR_RESULTS();
        //            results = SDR_test.MEAS_SDR_RESULTS;*/
        //        }

        //        private void button13_Click(object sender, EventArgs e)
        //        {
        //            /*
        //            // тестирование занятия спектра 
        //            MEAS_TASK_SDR mEAS_TASK_SDR = new MEAS_TASK_SDR();
        //            mEAS_TASK_SDR.ident_task = "123456";
        //            mEAS_TASK_SDR.id_meas_sub_task_st = "99094";
        //            mEAS_TASK_SDR.MEAS_SDR = new MEAS_SDR();
        //            mEAS_TASK_SDR.MEAS_SDR.IDENT = "BBC60C_123";
        //            mEAS_TASK_SDR.MEAS_SDR.Type_SDR = "BBC60C";
        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM = new MEAS_SDR_FREQ_PARAM();
        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.BW_CH = 100; // 50 кГц


        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST = new List<MEAS_SDR_FREQ_LST>();
        //            MEAS_SDR_FREQ_LST fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 102; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 99.1; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 99.3; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 99.5; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 100; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 100.1; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 100.2; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 100.3; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 100.4; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 100.5; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            fr_lst = new MEAS_SDR_FREQ_LST(); fr_lst.freq_ch = 100.6; mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.FREQ_LST.Add(fr_lst);
        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.f_min = 99.05;
        //            mEAS_TASK_SDR.MEAS_SDR_FREQ_PARAM.f_max = 102.05;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM = new MEAS_SDR_PARAM();
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.DETECT_TYPE = "Average";
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.PREAMPLIFICATION = 0;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.RBW = 100;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.VBW = 100;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.ref_level_dbm = -20;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.RF_ATTENUATION = 0;
        //            mEAS_TASK_SDR.MEAS_SDR_PARAM.Time_of_m = 10;
        //            mEAS_TASK_SDR.status = "A";
        //            mEAS_TASK_SDR.sw_time = 10;
        //            mEAS_TASK_SDR.TypeFunction = "FBO";
        //            mEAS_TASK_SDR.Type_of_m = "RT";
        //            mEAS_TASK_SDR.MEAS_SDR_SO_PARAM = new MEAS_SDR_SO_PARAM();
        //            mEAS_TASK_SDR.MEAS_SDR_SO_PARAM.Level_min_occup = -80;
        //            mEAS_TASK_SDR.MEAS_SDR_SO_PARAM.n_in_chenal =10;
        //            mEAS_TASK_SDR.MEAS_SDR_SO_PARAM.Type_of_SO = "FBO"; // FCO
        //            SDR_test.process_meas_BB60C(mEAS_TASK_SDR);

        //            MEAS_SDR_RESULTS results = new MEAS_SDR_RESULTS();
        //            results = SDR_test.MEAS_SDR_RESULTS;*/
        //        }

        //        private void button14_Click(object sender, EventArgs e)
        //        {
        //            // тупа создаем обект
        //            SDR_test = new SDR_BB60C();
        //        }

        // функция максима исключительно для теста стрима старой функции bbFetchRaw
        //private void button15_Click(object sender, EventArgs e)
        //{
        //    //try
        //    //{
        //        // Инициализация
        //        int id = -1;
        //        textBox1.Text += "SW Opening Device, Please Wait" + Environment.NewLine;
        //        status = bb_api.bbOpenDevice(ref id);
        //        if (status != bbStatus.bbNoError)
        //        {   textBox1.Text += "SW Error: Unable to open BB60" + Environment.NewLine;
        //            textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine;
        //            return;}
        //        else {textBox1.Text += "SW Device Found" + Environment.NewLine;}
        //        // Конец инициализации
        //        // Установка параметров измерения 


        //        bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);
        //        bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);

        //        Double f_central = 959200000;
        //        Double span = 200000;
        //        List<Double> Lsum_I = new List<double>();
        //        List<Double> Lsum_Q = new List<double>();
        //        List<Double> Lsum_atan = new List<double>();
        //        List<Double> Lsum_IQ = new List<double>();
        //        List<Double> Lpenalty = new List<double>();

        //        for (int i = 0; i < 10; i++ )
        //        {
        //            double f = f_central;// +i * 1000 - 5000;
        //            bb_api.bbConfigureCenterSpan(id, f, span);
        //            bb_api.bbConfigureIQ(id, 1, span);
        //            status = bb_api.bbInitiate(id, bb_api.BB_STREAMING, bb_api.BB_STREAM_IQ);
        //            if (status != bbStatus.bbNoError) { return; }
        //            int return_len = 0; int samples_per_sec = 0; double bandwidth = 0.0;
        //            bb_api.bbQueryStreamInfo(id, ref return_len, ref bandwidth, ref samples_per_sec);
        //            float[] iq_samples = new float[return_len * 2];
        //            int[] triggers = new int[80];
        //            bb_api.bbFetchRaw(id, iq_samples, triggers);

        //            Double max_ampl = -999; Double sum_I = 0; Double sum_Q = 0; Double sum_IQ = 0; Double sum_atan = 0;

        //            for (int j = 0; j < return_len * 2-1; j++)
        //            {
        //                double cur_I = iq_samples[j];
        //                double cur_Q = iq_samples[j + 1];
        //                sum_I += cur_I;
        //                sum_Q += cur_Q;
        //                sum_atan += Math.Atan(cur_I / cur_Q);
        //                sum_IQ += Math.Sign(cur_I) * cur_Q + cur_I * Math.Sign(cur_Q);
        //                if (Math.Abs(iq_samples[j]) > max_ampl) { max_ampl = Math.Abs(iq_samples[j]); }
        //                if (Math.Abs(sum_atan) < 1)
        //                {
        //                    int h = 0;
        //                }
        //            }
        //            Lsum_I.Add(sum_I);
        //            Lsum_Q.Add(sum_Q);
        //            Lsum_IQ.Add(sum_IQ);
        //            Lsum_atan.Add(sum_atan);

        //            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
        //            g.Clear(pictureBox1.BackColor);

        //            int first_0 = 0; 

        //            for (int i1 = 100; i1 < return_len * 2 - 2; i1 = i1 + 2)
        //            {
        //                if ((iq_samples[i1] > 0) && (iq_samples[i1 + 2] < 0) || (iq_samples[i1] < 0) && (iq_samples[i1 + 2] > 0)) { first_0 = i1; break; }
        //            }
        //            Double time_of_semple = 1/40000000.0;
        //            Double f_max_bod = 210000;
        //            Double f_min_bod = 190000;
        //            Double steps_of_boads = 1000;
        //            Double window = 0.25; //окно для срабатывания.
        //            Double penalty = 0;
        //            for (Double f_bod = f_min_bod; f_bod < f_max_bod; f_bod = f_bod + steps_of_boads)
        //            {
        //                for (int i1 = 100; i1 < return_len * 2 - 2; i1 = i1 + 2)
        //                {
        //                    if ((iq_samples[i1] > 0) && (iq_samples[i1 + 2] < 0) || (iq_samples[i1] < 0) && (iq_samples[i1 + 2] > 0))
        //                    {
        //                        double delta = (i1 - first_0) * time_of_semple;
        //                        double for_if = Math.Abs(delta % (1 / f_bod)) * f_bod;
        //                        if (for_if > window)
        //                        { 
        //                            penalty += 1;  
        //                        }
        //                    }
        //                }
        //                Lpenalty.Add(penalty);
        //                penalty = 0;
        //            }


        //            int k = 0; int z = 0;
        //            for (int i1 = 0; i1 < 100000; i1 = i1 + 2)
        //            {
        //                Double x = (i1-k)/10.0;//200 * iq_samples[i1] / max_ampl + 200;//I
        //                if (i1 - k >= 4000) {
        //                    break;
        //                }
        //                Double y = 100 * iq_samples[i1 + 1+z] / max_ampl + 100;//Q
        //                Double y1 = 100 * iq_samples[i1+z] / max_ampl + 300;//Q
        //                drawPoint((int)x, (int)y);
        //                drawPoint((int)x, (int)y1);
        //            }

        //        }







        //    /*

        //        Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
        //        g.Clear(pictureBox1.BackColor);
        //        do
        //        { j++;
        //            Double  start_time = DateTime.Now.Ticks;
        //            DateTime time = DateTime.Now;                    
        //            label9.Text = j.ToString();
        //            int return_len = 0; int samples_per_sec = 0; double bandwidth = 0.0;
        //            bb_api.bbQueryStreamInfo(id, ref return_len, ref bandwidth, ref samples_per_sec);
        //            //System.Threading.Thread.Sleep(1000);
        //            float[] iq_samples = new float[return_len * 2];
        //            int[] triggers = new int[80];


        //            bb_api.bbFetchRaw(id, iq_samples, triggers);
        //            for (int i = 0; i <100000; i = i + 2)
        //            {
        //                Double x = 200*iq_samples[i]/max_ampl+200;//I
        //                Double y = 200*iq_samples[i + 1] / max_ampl + 200;//Q
        //                drawPoint((int)x, (int)y);
        //            }
        //          }
        //        while (j < 1);

        //    */









        //        bb_api.bbCloseDevice(id);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    textBox1.Text += "Some error in block work with streem";
        //    //}
        //  }

        private void button11_Click_1(object sender, EventArgs e)
        {
            // Успешно пройден 04_09_2018 Максим.
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
            CirculatingData circulatingData =null;
            ReferenceSignal[] referenceSignals = null;
            MeasurementProcessing MeasProcessing = new MeasurementProcessing();
            int i = 0;
            while (mEAS_TASK_SDR.status == "A") // цикл имитирует процесс измерения
            {
                // поиск таска Task из ListTask
                // MeasProcessing (Task)
                // Если есть результаты они передаются в другой поток ()
                if ((mEAS_TASK_SDR.Time_start < DateTime.Now)&&(mEAS_TASK_SDR.Time_stop > DateTime.Now))
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
            {   textBox1.Text += "SW Error: Unable to open BB60" + Environment.NewLine;
                textBox1.Text += bb_api.bbGetStatusString(status) + Environment.NewLine; return;}
            else {textBox1.Text += "SW Device Found" + Environment.NewLine;}
            // Конец инициализации
            // Установка параметров измерения 
            Double f_central = 959200000;
            Double span = 200000;
            bb_api.bbConfigureCenterSpan(id, f_central, span);
            bb_api.bbConfigureLevel(id, -20.0, bb_api.BB_AUTO_ATTEN);                
            bb_api.bbConfigureGain(id, bb_api.BB_AUTO_GAIN);
            bb_api.bbConfigureIQ(id, 2, 15000000);
            bb_api.bbConfigureIO(id, 0, bb_api.BB_PORT2_IN_TRIGGER_RISING_EDGE);
            //bb_api.bbSyncCPUtoGPS(3, 38400);
            status = bb_api.bbInitiate(id, bb_api.BB_STREAMING, bb_api.BB_STREAM_IQ);
            if (status != bbStatus.bbNoError) { return; }
            double bandwidth =0 ;
            int sampleRate = 0;
            int rr = 0; 
            bb_api.bbQueryStreamInfo(id, ref rr, ref bandwidth, ref sampleRate);
            const int BlockSize = 262144;
            float *bufer = stackalloc float[BlockSize * 2];
            int *triggers = stackalloc int[71];
            bbIQPacket pkt = new bbIQPacket();
            pkt.iqData = bufer;
            pkt.iqCount = BlockSize;
            pkt.triggers = triggers;
            pkt.triggerCount = 70;
            pkt.purge = 1;
            //bb_api.bbGetIQ(id, ref pkt);
            string file = "";
            for (int i = 0; i < 500; i++)
            {
                bb_api.bbGetIQ(id, ref pkt);
                file+= "Triggers: ";
                for (int j = 0; j < 10; j++)
                {
                    file+= pkt.triggers[j].ToString()+ " ";
                }
                file+= Environment.NewLine;
                //file+= "iqData: ";
                //for (int j = 0; j < 10; j++)
                //{
                //    file += pkt.iqData[j].ToString() + " ";
                //}
                //file += Environment.NewLine;

            }
            System.IO.File.WriteAllText("C:\\Temp\\ResSharp.txt",file);
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
                for(int j = 0; j<listTrigger[i].Length; j++)
                {
                    if (listTrigger[i][j]!=0)
                    {
                        hit = true; 
                    }
                }
            }


            // Чегото это не пашет. 

            bb_api.bbCloseDevice(id);
        }

        private void CreatePictureForBlokIQ (BlockOfSignal Block, int from, int to, List<int>Blue_point)
        {
            Double Max_IQ = 0;
            for (int i = 0; i < Block.IQStream.Length; i++){ if (Max_IQ < Block.IQStream[i]) { Max_IQ = Block.IQStream[i];} }
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.Clear(pictureBox1.BackColor);
            drawPointBlue((int)200, (int)200);

            for (int i = from*2; i < to*2; i = i + 2)
            {
                Double x = 200 * Block.IQStream[i] / Max_IQ + 200;//I
                Double y = 200 * Block.IQStream[i + 1] / Max_IQ + 200;//Q
                if (Blue_point.Contains(i/2))//ПРАВКА
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
            MainProcessReceveIQStreamAndGetTimeStamp test_stream = new MainProcessReceveIQStreamAndGetTimeStamp(1,959.2,200, MainProcessReceveIQStreamAndGetTimeStamp.TypeTechnology.GSM);
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
            // тест пройден 04.09.2018 Максим
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
            mEAS_TASK_SDR.MeasFreqParam.RgL = 959.0;
            mEAS_TASK_SDR.MeasFreqParam.RgU = 959.4;
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
            MeasurementProcessing MeasProcessing = new  MeasurementProcessing();
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
            bb_api.bbConfigureSweepCoupling(id, RBW, VBW, Time,bb_api.BB_NUTALL, bb_api.BB_NO_SPUR_REJECT);
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
            //NewLevels = ChangeTraceGrid.ChangeGrid(ref Levels, StartOldFreq_MHz, OldStep_kHz, StartNewFreq_MHz, NewStep_kHz, NumberPointInNewLevels);

            NumberPointInNewLevels = 40;
            StartOldFreq_MHz = 100;
            OldStep_kHz = 1000;
            StartNewFreq_MHz = 101;
            NewStep_kHz = 150;
            NewLevels = new double[NumberPointInNewLevels];
            NewLevels = ChangeTraceGrid.ChangeGrid(ref Levels, StartOldFreq_MHz, OldStep_kHz, StartNewFreq_MHz, NewStep_kHz, NumberPointInNewLevels);






        }
    }
}

