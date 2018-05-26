using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Common.Extensions
{
    public static class SerializationExtension
    {
        public static byte[] Serialize<T>(this T value)
        {
            if (value == null) return new byte[] { };

            var binaryFormatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, value);
                stream.Position = 0;
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] value)
            where T : class
        {
            if (value == null) return null;

            var binaryFormatter = new BinaryFormatter();
            using (var stream = new MemoryStream(value))
            {
                return binaryFormatter.Deserialize(stream) as T;
            }
        }
    }
}
