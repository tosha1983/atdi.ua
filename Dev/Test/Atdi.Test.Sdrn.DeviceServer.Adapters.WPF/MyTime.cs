using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Sdrn.DeviceServer.Adapters.WPF
{
    //public static class MyTime
    //{
    //    const long FILETIME_TO_DATETIMETICKS = 504911232000000000;
    //    static string system = OSInfo.getOSInfo();
    //    static bool From = system.Contains("Windows 8") || system.Contains("Windows 10");//system.Contains("Windows 8.1")

    //    [System.Runtime.InteropServices.DllImport("Kernel32.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
    //    static extern void GetSystemTimePreciseAsFileTime(out long filetime);

    //    [System.Runtime.InteropServices.DllImport("Kernel32.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
    //    static extern void GetSystemTimeAsFileTime(out FILETIME lpSystemTimeAsFileTime);

    //    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    //    public struct FILETIME
    //    {
    //        public const long FILETIME_TO_DATETIMETICKS = 504911232000000000;   // 146097 = days in 400 year Gregorian calendar cycle. 504911232000000000 = 4 * 146097 * 86400 * 1E7
    //        public uint TimeLow;    // least significant digits
    //        public uint TimeHigh;   // most sifnificant digits
    //        public long TimeStamp_FileTimeTicks { get { return TimeHigh * 4294967296 + TimeLow; } }     // ticks since 1-Jan-1601 (1 tick = 100 nanosecs). 4294967296 = 2^32
    //        public long Ticks { get { return TimeStamp_FileTimeTicks + FILETIME_TO_DATETIMETICKS; } }
    //    }
    //    public static long GetTimeStamp()
    //    {
    //        FILETIME ft;
    //        long tick = 0;
    //        if (From)
    //        {
    //            GetSystemTimePreciseAsFileTime(out tick);
    //            return tick;// ft.dwHighDateTime * 4294967296 + ft.dwLowDateTime;
    //        }
    //        else
    //        {
    //            GetSystemTimeAsFileTime(out ft);
    //            return ft.Ticks;// (long)ft.dwHighDateTime * (long)4294967296 + (long)ft.dwLowDateTime + FILETIME_TO_DATETIMETICKS;
    //        }

    //    }
    //}
}
