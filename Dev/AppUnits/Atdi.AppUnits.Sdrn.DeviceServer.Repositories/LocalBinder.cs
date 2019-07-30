using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Runtime.Serialization;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    internal sealed class LocalBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type type = null;
            var shortAssemblyName = assemblyName.Split(',')[0];
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (shortAssemblyName == assembly.FullName.Split(',')[0])
                {
                    type = assembly.GetType(typeName);
                    break;
                }

            }
            return type;
        }
    }
}
