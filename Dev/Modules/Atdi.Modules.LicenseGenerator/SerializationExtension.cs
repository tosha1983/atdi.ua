using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.LicenseGenerator
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
                binaryFormatter.Binder = new DeserializationBinder();
                var result = binaryFormatter.Deserialize(stream);
                
                return result as T;
            }
        }
    }

    internal sealed class DeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {

            var resultType = Type.GetType($"{typeName}, {assemblyName}");

            return resultType;
        }
    }
}
