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
        [DllImport("netkey.dll", CharSet = CharSet.Unicode)]
        public static extern int GetToken(string softname, string exedate);
        public int GetTokenValue(string softname, string exedate)
        {
            int value = GetToken(softname, exedate);
            return value;
        }
    }
}
