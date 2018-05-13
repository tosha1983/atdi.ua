using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface ITypeResolver
    {
        Assembly ResolveAssembly(AssemblyName name);

        Type ResolveType(string typeName);

        Type ResolveType<TBase>(Assembly assembly);

        TBase CreateInstance<TBase>(AssemblyName name);
    }
}
