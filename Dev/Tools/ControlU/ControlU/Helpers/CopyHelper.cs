using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ControlU.Helpers
{

    public static class ExtensionMethods
    {
        // Deep clone
        public static T DeepCloneXML<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer formatter = new XmlSerializer(a.GetType());//xml т.к. ругается на INotifyPropertyChanged
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
        
        // Deep clone
        public static T DeepCloneBin<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();//ругается на INotifyPropertyChanged
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
    
}
