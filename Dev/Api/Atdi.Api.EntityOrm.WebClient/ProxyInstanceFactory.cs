using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RE = System.Reflection.Emit;

namespace Atdi.Api.EntityOrm.WebClient
{
	public sealed class ProxyInstanceFactory
	{
		private readonly ConcurrentDictionary<Type, Type> _proxyInterfaceTypes;

		internal ProxyInstanceFactory()
		{
			_proxyInterfaceTypes = new ConcurrentDictionary<Type, Type>();
		}
		public TInterface Create<TInterface>()
		{
			var proxyType = this.GetProxyType<TInterface>();
			var instance = Activator.CreateInstance(proxyType);
			return (TInterface)instance;
		}

		public Type GetProxyType<TInterface>()
		{
			var interfaceType = typeof(TInterface);
			if (!_proxyInterfaceTypes.TryGetValue(interfaceType, out var proxyType))
			{
				proxyType = GenerateProxyType(interfaceType);
				if (!_proxyInterfaceTypes.TryAdd(interfaceType, proxyType))
				{
					if (!_proxyInterfaceTypes.TryGetValue(interfaceType, out proxyType))
					{
						throw new InvalidOperationException($"Can't append Interface Proxy Type to concurrent cache by Interface Еype '{interfaceType}'");
					}
				}
			}
			return proxyType;
		}

		private static Type GenerateProxyType(Type baseInterface)
		{
			var ns = baseInterface.Namespace;
			var name = baseInterface.Name.Substring(1, baseInterface.Name.Length - 1);

			var an = baseInterface.Assembly.GetName();

			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName($"{an.Name}.EntitiesProxy, Version=1.0.0.1"),
				RE.AssemblyBuilderAccess.RunAndSave);

			var dynamicModule = assemblyBuilder.DefineDynamicModule(
				$"{an.Name}.EntitiesProxy.Module",
				$"{an.Name}.EntitiesProxy.dll");

			var proxyTypeBuilder = dynamicModule.DefineType(
				$"{ns}.{name}_Proxy",
				TypeAttributes.BeforeFieldInit | TypeAttributes.Public, typeof(object), new Type[] { baseInterface });

			// генерируем переменные под свойства
			var propertiesInfo = GetPropertiesWithInherited(baseInterface);
			for (int i = 0; i < propertiesInfo.Length; i++)
			{
				var propertyInfo = propertiesInfo[i];
				PropertyEmitter(proxyTypeBuilder, propertyInfo.Name, propertyInfo.PropertyType);
			}

			// генерируем конструктор по умолчанию
			var constructor = proxyTypeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, new Type[] { });

			var ctorIL = constructor.GetILGenerator();

			var objectType = typeof(object);
			var objectCtor = objectType.GetConstructor(new Type[0]);

			ctorIL.Emit(RE.OpCodes.Ldarg_0);
			ctorIL.Emit(RE.OpCodes.Call, objectCtor);
			ctorIL.Emit(RE.OpCodes.Ret);


			var proxyType = proxyTypeBuilder.CreateType();
			assemblyBuilder.Save($"{an.Name}.EntitiesProxy.dll");

			return proxyType;
		}

		private static void PropertyEmitter(RE.TypeBuilder typeBuilder, string name, Type propertyType)
		{
			var ci = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute).GetConstructor(new Type[] { });
			var attrBuilder = new RE.CustomAttributeBuilder(ci, new object[0]);

			var fieldBuilder = typeBuilder.DefineField(String.Format("<{0}>k__BackingField", name), propertyType, FieldAttributes.Private);
			fieldBuilder.SetCustomAttribute(attrBuilder);

			var methodAttrs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

			var getterBuilder = typeBuilder.DefineMethod(String.Format("get_{0}", name), methodAttrs, propertyType, Type.EmptyTypes);
			getterBuilder.SetCustomAttribute(attrBuilder);
			var getterIl = getterBuilder.GetILGenerator();
			getterIl.Emit(RE.OpCodes.Ldarg_0);
			getterIl.Emit(RE.OpCodes.Ldfld, fieldBuilder);
			getterIl.Emit(RE.OpCodes.Ret);

			var setterBuilder = typeBuilder.DefineMethod(String.Format("set_{0}", name), methodAttrs, typeof(void), new[] { propertyType });
			setterBuilder.SetCustomAttribute(attrBuilder);
			var setterIl = setterBuilder.GetILGenerator();
			setterIl.Emit(RE.OpCodes.Ldarg_0);
			setterIl.Emit(RE.OpCodes.Ldarg_1);
			setterIl.Emit(RE.OpCodes.Stfld, fieldBuilder);
			setterIl.Emit(RE.OpCodes.Ret);

			var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, CallingConventions.HasThis, propertyType, null);
			propertyBuilder.SetGetMethod(getterBuilder);
			propertyBuilder.SetSetMethod(setterBuilder);
		}

		private static PropertyInfo[] GetPropertiesWithInherited(Type type)
		{
			if (!type.IsInterface)
				return type.GetProperties();

			return (new Type[] { type })
				.Concat(type.GetInterfaces())
				.SelectMany(i => i.GetProperties()).ToArray();
		}
	}
}
