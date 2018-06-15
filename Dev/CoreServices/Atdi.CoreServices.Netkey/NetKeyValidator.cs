using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedNetKey;
using Atdi.Contracts.CoreServices.Netkey;

namespace Atdi.CoreServices.Netkey
{
    public class NetKeyValidator : INetKey
    {
        public int GetToken(string softname, string exedate)
        {
            int value = 0;
            NetKey net = new NetKey();
            unsafe
            {
                string soft = softname;
                string exe = exedate;
                sbyte* result_soft = stackalloc sbyte[soft.Length + 1];
                result_soft[soft.Length] = 0;
                fixed (char* p = soft) {
                    for (int i = 0; i < soft.Length; i++)
                        result_soft[i] = (sbyte)p[i];
                }
                sbyte* result_exe = stackalloc sbyte[exe.Length + 1];
                result_exe[exe.Length] = 0;
                fixed (char* p = exe) {
                    for (int i = 0; i < exe.Length; i++)
                        result_exe[i] = (sbyte)p[i];
                }
                value = net.GetToken(result_soft, result_exe);
            }
            return value;
        }
    }
}
