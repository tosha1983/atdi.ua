//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;

//namespace Atdi.WcfServices.Sdrn.Device
//{
//    class LocalBinder : SerializationBinder
//    {
//        public override Type BindToType(string assemblyName, string typeName)
//        {
//            Type type = null;
//            var shortAssemblyName = assemblyName.Split(',')[0];
//            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//            foreach (var assembly in assemblies)
//            {
//                if (shortAssemblyName == assembly.FullName.Split(',')[0])
//                {
//                    type = assembly.GetType(typeName);
//                    break;
//                }

//            }
//            return type;
//        }
//    }
//}
