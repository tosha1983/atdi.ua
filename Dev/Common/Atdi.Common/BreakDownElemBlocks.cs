using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Atdi.Common
{
    public static class BreakDownElemBlocks
    {
        private const int CountInParams = 1000;
        public static List<int[]> BreakDown(int[] elements)
        {
            var arrIntEmitting = new List<int[]>();
            var listIntEmitting = new List<int>();
            int cnt = 1;
            for (int i = 0; i < elements.Length; i++)
            {
                if (cnt <= CountInParams)
                {
                    listIntEmitting.Add(elements[i]);
                }
                else
                {
                    arrIntEmitting.Add(listIntEmitting.ToArray());
                    listIntEmitting.Clear();
                    cnt = 0;
                }
                ++cnt;
            }
            if ((listIntEmitting != null) && (listIntEmitting.Count > 0))
            {
                arrIntEmitting.Add(listIntEmitting.ToArray());
            }
            return arrIntEmitting;
        }
        public static List<int?[]> BreakDown(int?[] elements)
        {
            var arrIntEmitting = new List<int?[]>();
            var listIntEmitting = new List<int?>();
            int cnt = 1;
            for (int i = 0; i < elements.Length; i++)
            {
                if (cnt <= CountInParams)
                {
                    listIntEmitting.Add(elements[i]);
                }
                else
                {
                    arrIntEmitting.Add(listIntEmitting.ToArray());
                    listIntEmitting.Clear();
                    cnt = 0;
                }
                ++cnt;
            }
            if ((listIntEmitting != null) && (listIntEmitting.Count > 0))
            {
                arrIntEmitting.Add(listIntEmitting.ToArray());
            }
            return arrIntEmitting;
        }
    }
}
