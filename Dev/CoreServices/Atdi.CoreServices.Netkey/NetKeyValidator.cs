using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Netkey;
using System.Runtime.InteropServices;

namespace Atdi.CoreServices.NetKeyValidator
{
    public class NetKeyValidator : INetKeyValidator
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetToken([MarshalAs(UnmanagedType.LPStr)]  string softname, [MarshalAs(UnmanagedType.LPStr)] string exedate);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);


        public int GetTokenValue(string softname, string exedate)
        {
            int val = 0;
            IntPtr pDll = LoadLibrary("netkey.dll");
            IntPtr pAddressOfFunctionToCall = GetProcAddress(pDll, "GetToken");
            GetToken GetToken = (GetToken)Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(GetToken));
            val = GetToken(softname, exedate);
            FreeLibrary(pDll);
            return val;
        }
    }

}
