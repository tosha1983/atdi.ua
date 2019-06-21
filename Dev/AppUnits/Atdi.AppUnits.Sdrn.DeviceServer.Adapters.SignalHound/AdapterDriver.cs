using System;
using System.Runtime.InteropServices;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    public static class AdapterDriver
    {        
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetSerialNumberList(int[] devices, ref int deviceCount);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbOpenDeviceBySerialNumber(ref int device, int serialNumber);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbOpenDevice(ref int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbCloseDevice(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureAcquisition(int device, uint detector, uint scale);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureCenterSpan(int device, double center, double span);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureLevel(int device, double refLevel, double atten);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureGain(int device, int gain);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureSweepCoupling(int device, double rbw, double vbw, double sweepTime, uint rbwShape, uint rejection);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureProcUnits(int device, uint units);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureIO(int device, uint port1, uint port2);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureDemod(int device, int modType, double freq, float ifBandwidth, float lowPassFreq, float highPassFreq, float fmDeemphasis);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureIQ(int device, int downsampleFactor, double bandwidth);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbInitiate(int device, uint mode, uint flag);

        [DllImport("bb_api", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbFetchTrace_32f(int device, int arraySize, float[] min, float[] max);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbFetchTrace(int device, int arraysize, double[] min, double[] max);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbFetchRealTimeFrame(int device, float[] sweep, float[] frame, float[] alphaFrame);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbFetchAudio(int device, float[] audio);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetIQUnpacked(int device, float[] iqData, int iqCount, int[] triggers, int triggerCount, int purge, ref int dataRemaining, ref int sampleLoss, ref int sec, ref int nano);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetIQ(int device, ref bbIQPacket pkt);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbQueryTraceInfo(int device, ref uint trace_len, ref double bin_size, ref double start);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbQueryStreamInfo(int device, ref int return_len, ref double bandwidth, ref int samples_per_sec);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbAbort(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbPreset(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbSelfCal(int device);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbSyncCPUtoGPS(int com_port, int baud_rate);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetDeviceType(int device, ref int type);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetSerialNumber(int device, ref uint serial_number);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetFirmwareVersion(int device, ref int version);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbGetDeviceDiagnostics(int device, ref float temperature, ref float usbVoltage, ref float usbCurrent);

        public static string bbGetDeviceName(int device)
        {
            int device_type = -1;
            bbGetDeviceType(device, ref device_type);
            if (device_type == (int)EN.Device.BB60A)
            {
                return "BB60A";
            }
            else if (device_type == (int)EN.Device.BB60C)
            {
                return "BB60C";
            }
            return "Unknown device";
        }

        public static int bbGetSerialString(int device)
        {
            uint serial_number = 0;
            if (bbGetSerialNumber(device, ref serial_number) == EN.Status.NoError)
            {
                return (int)serial_number;
            }
            return (int)serial_number;
        }

        public static string bbGetFirmwareString(int device)
        {
            int firmware_version = 0;
            if (bbGetFirmwareVersion(device, ref firmware_version) == EN.Status.NoError)
            {
                return firmware_version.ToString();
            }
            return "";
        }

        public static string bbGetAPIString()
        {
            IntPtr str_ptr = bbGetAPIVersion();
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str_ptr);
        }

        public static string bbGetStatusString(EN.Status status)
        {
            IntPtr str_ptr = bbGetErrorString(status);
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str_ptr);
        }

        // Call get_string variants above instead
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr bbGetAPIVersion();
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr bbGetErrorString(EN.Status status);

        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbConfigureRealTime(int device,
            double frameScale, int frameRate);
        [DllImport("bb_api.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern EN.Status bbQueryRealTimeInfo(int device,
            ref int frameWidth, ref int frameHeight);

        public struct bbIQPacket
        {
            public float[] iqData;
            public int iqCount;
            public int[] triggers;
            public int triggerCount;
            public bool purge;
            public int dataRemaining;
            public int sampleLoss;
            public int iqSec;
            public int iqNano;
        }
    }
}
