using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface ITypeResolver
    {
        Assembly ResolveAssembly(AssemblyName name);

        Type ResolveType(string typeName);

        Type ResolveType<TBase>(Assembly assembly);

        Type[] ResolveTypes<TBase>(Assembly assembly);

        Type[] ResolveTypes(Assembly assembly, Type baseType);

        TBase CreateInstance<TBase>(AssemblyName name);

        IEnumerable<Type> ForeachInAllAssemblies(Func<Type, bool> predicate);
    }

    public static class TypeResolverExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Type> GetTypesByInterface<TInterface>(this ITypeResolver typeResolver)
        {
            var interfaceType = typeof(TInterface);

            var resultTypes = typeResolver.ForeachInAllAssemblies(
                    (type) =>
                    {
                        if (!type.IsClass
                        || type.IsNotPublic
                        || type.IsAbstract
                        || type.IsInterface
                        || type.IsEnum)
                        {
                            return false;
                        }

                        return type.GetInterface(interfaceType.FullName) != null;
                    }
                );

            return resultTypes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Type> GetTypesByInterface(this ITypeResolver typeResolver, Type interfaceType)
        {
            var resultTypes = typeResolver.ForeachInAllAssemblies(
                    (type) =>
                    {
                        if (!type.IsClass
                        || type.IsNotPublic
                        || type.IsAbstract
                        || type.IsInterface
                        || type.IsEnum)
                        {
                            return false;
                        }

                        return type.GetInterface(interfaceType.FullName) != null;
                    }
                );

            return resultTypes;
        }
    }
}
