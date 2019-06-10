/*
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
    public static class BinaryDecoder
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] param)
        {
            T val = default(T);
            if (param != null)
            {
                using (MemoryStream ms = new MemoryStream(param))
                {
                    BinaryFormatter br = new BinaryFormatter();
                    val = (T)br.Deserialize(ms);
                }
            }
            return val;
        }

    }
}
*/