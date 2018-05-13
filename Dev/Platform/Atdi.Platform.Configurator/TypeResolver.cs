using Atdi.Platform.ConfigElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    class TypeResolver : ITypeResolver
    {
        public static readonly string SearchAssembliesStartPath = "SearchAssembliesStartPath";
        public static readonly string SearchAssembliesPattern = "SearchAssembliesPattern";

        private readonly AtdiPlatformConfigElement _config;
        private readonly string[] _assembliesDirectories;

        public TypeResolver(AtdiPlatformConfigElement config)
        {
            this._config = config;
            this._assembliesDirectories = GetSearchAssembliesDirectories();
        }
        

        public TBase CreateInstance<TBase>(AssemblyName name)
        {
            var assembly = this.ResolveAssembly(name);
            if (assembly == null)
            {
                throw new InvalidOperationException($"Not found an assembly with name '{name.FullName}'");
            }

            var type = this.ResolveType<TBase>(assembly);
            if (type == null)
            {
                throw new InvalidOperationException($"Undefined any type which extended type '{typeof(TBase).Name}' in the assembly '{name.FullName}' ");
            }
            var  instance = Activator.CreateInstance(type);

            return (TBase)instance; 
        }

        public Assembly ResolveAssembly(AssemblyName name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                var assemblyName = assembly.GetName();

                if (assemblyName.FullName == name.FullName)
                {
                    return assembly;
                }
                if (assemblyName.Name == name.Name)
                {
                    return assembly;
                }
            }

            return GetAssembly(name);
        }

        public Type ResolveType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type == null)
            {
                type = Type.GetType(typeName, this.GetAssembly, null);
            }

            return type;
        }

        public Type ResolveType<TBase>(Assembly assembly)
        {
            var types = assembly.GetExportedTypes();
            var baseType = typeof(TBase);

            for (int i = 0; i < types.Length; i++)
            {
                var curType = types[i];
                if (baseType.IsInterface)
                {
                    if (curType.GetInterfaces().Contains(baseType))
                    {
                        return curType;
                    }
                }
                else if (baseType.IsClass && curType.IsAssignableFrom(baseType))
                {
                    return curType;
                }
            }

            return null;
        }


        private string[] GetSearchAssembliesDirectories()
        {
            var startPath = this._config.PropertiesSection.GetProperty(SearchAssembliesStartPath);
            var pattern = this._config.PropertiesSection.GetProperty(SearchAssembliesPattern);

            if (startPath == null || !startPath.HasValue)
            {
                return new string[] { };
            }

            var patternValue = string.Empty;
            if (pattern != null && pattern.HasValue)
            {
                patternValue = pattern.ValueProperty.Replace(@"\", @"\\");
            }
            var directories = Directory.EnumerateDirectories(startPath.ValueProperty, "*", SearchOption.AllDirectories)
                .Where(p => string.IsNullOrEmpty(patternValue) || Regex.IsMatch(p, patternValue))
                .ToArray();

            return directories;
        }

        private Assembly GetAssembly(AssemblyName name)
        {

            if (this._assembliesDirectories == null || this._assembliesDirectories.Length == 0)
            {
                return Assembly.Load(name);
            }

            var fullPath = string.Empty;
            foreach (var path in this._assembliesDirectories)
            {
                fullPath = Path.Combine(path, name.Name + ".dll");
                if (File.Exists(fullPath))
                {
                    break;
                }
                fullPath = string.Empty;
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                return Assembly.Load(name);
            }

            return Assembly.LoadFrom(fullPath);
        }
    }
}
