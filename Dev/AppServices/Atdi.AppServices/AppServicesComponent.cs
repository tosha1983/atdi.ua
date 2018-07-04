using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Atdi.Platform.Logging;
using Atdi.Platform.DependencyInjection;

namespace Atdi.AppServices
{
    public abstract class AppServicesComponent<TAppService> : ComponentBase
    {
        public AppServicesComponent(string name) : base(name, ComponentType.AppServices, ComponentBehavior.SingleInstance)
        {
        }
        public AppServicesComponent(string name, ComponentBehavior behavior) : base(name, ComponentType.AppServices, behavior | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            var contractType = typeof(TAppService);
            var implementType = CreateProxy(this.GetType().Assembly, contractType);
            this.Container.Register(contractType, implementType, ServiceLifetime.PerThread);
        }

        private Type CreateProxy(Assembly implementAssembly, Type source)
        {
            var handlerResolver = this.Container.GetResolver<IHandlerResolver>();
            var assemblyName = implementAssembly.GetName().Name + ".ContractProxies";
            var proxyClassName = source.Name.Substring(1, source.Name.Length - 1) + "Proxy";

            var dynGeneratorHostAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName($"{assemblyName}, Version=1.0.0.1"),
                AssemblyBuilderAccess.RunAndSave);

            var dynModule = dynGeneratorHostAssembly.DefineDynamicModule(
                $"{assemblyName}",
                $"{assemblyName}.dll", true);

            var proxyType = dynModule.DefineType(
                $"{assemblyName}.{proxyClassName}",
                    TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.BeforeFieldInit, typeof(object), new Type[] { source, typeof(IDisposable) });

            var logerField = proxyType.DefineField("_logger", typeof(ILogger), FieldAttributes.Private | FieldAttributes.InitOnly);
            var resolverField = proxyType.DefineField("_resolver", typeof(IHandlerResolver), FieldAttributes.Private | FieldAttributes.InitOnly);
            var disposedField = proxyType.DefineField("_disposed", typeof(bool), FieldAttributes.Private);

            var sourceMethods = source.GetMethods();
            var privateFields = new FieldBuilder[sourceMethods.Length];
            var implMethods = new Type[sourceMethods.Length];
            for (int i = 0; i < sourceMethods.Length; i++)
            {
                var sourceMethod = sourceMethods[i];
                var implemMethodType = GetHandlerTypeByOperationName(implementAssembly, sourceMethod.Name); // implementAssembly.GetType("Atdi.AppServer.AppServices.CertainService.Handlers." + sourceMethod.Name + "Handler");

                handlerResolver.RegisterHandler(implemMethodType, ServiceLifetime.PerThread);

                implMethods[i] = implemMethodType;
                privateFields[i] = proxyType.DefineField($"_{sourceMethod.Name}Handler", implemMethodType, FieldAttributes.Private | FieldAttributes.InitOnly);
            }


            // begin constructors

            var constructor = proxyType.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(ILogger), typeof(IHandlerResolver) });
            var ctorLoggerParameter = constructor.DefineParameter(1, ParameterAttributes.None, "logger");
            var ctorResolverParameter = constructor.DefineParameter(2, ParameterAttributes.None, "resolver");
            

            var ctorIL = constructor.GetILGenerator();

            var objectType = typeof(object);
            var objectCtor = objectType.GetConstructor(new Type[0]);

            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, objectCtor);

            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, logerField);

            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_2);
            ctorIL.Emit(OpCodes.Stfld, resolverField);


            var resolverResolveMethod = typeof(IHandlerResolver).GetMethod("ResolveHandler");
            for (int i = 0; i < sourceMethods.Length; i++)
            {
                var sourceMethod = sourceMethods[i];
                var implemMethodType = implMethods[i];
                var privateFiled = privateFields[i];
                var mt = resolverResolveMethod.MakeGenericMethod(implemMethodType);

                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Ldfld, resolverField);
                ctorIL.Emit(OpCodes.Callvirt, mt);
                ctorIL.Emit(OpCodes.Stfld, privateFiled);
            }

            ctorIL.Emit(OpCodes.Ret);
            // end constructors

            // Dispose Method
            var desposeMethod = proxyType.DefineMethod("Dispose",
                MethodAttributes.Public |
                MethodAttributes.NewSlot |
                MethodAttributes.Final |
                MethodAttributes.HideBySig |
                MethodAttributes.Virtual, CallingConventions.Standard);

            var desposeIL = desposeMethod.GetILGenerator();

            var desposeLabelExit = desposeIL.DefineLabel();

            // if  _disposable == false
            desposeIL.Emit(OpCodes.Ldarg_0);
            desposeIL.Emit(OpCodes.Ldfld, disposedField);
            desposeIL.Emit(OpCodes.Brtrue_S, desposeLabelExit);
            // { _disposable = true;
            desposeIL.Emit(OpCodes.Ldarg_0);
            desposeIL.Emit(OpCodes.Ldc_I4_1);
            desposeIL.Emit(OpCodes.Stfld, disposedField);

            var resolverReleaseMethod = typeof(IHandlerResolver).GetMethod("ReleaseHandler");

            for (int i = 0; i < sourceMethods.Length; i++)
            {
                var sourceMethod = sourceMethods[i];
                var implemMethodType = implMethods[i];
                var privateFiled = privateFields[i];
                var mt = resolverReleaseMethod.MakeGenericMethod(implemMethodType);

                desposeIL.Emit(OpCodes.Ldarg_0);
                desposeIL.Emit(OpCodes.Ldfld, resolverField);

                desposeIL.Emit(OpCodes.Ldarg_0);
                desposeIL.Emit(OpCodes.Ldfld, privateFiled);
                desposeIL.Emit(OpCodes.Callvirt, mt);
            }
            // }

            desposeIL.MarkLabel(desposeLabelExit);
            desposeIL.Emit(OpCodes.Ret);

            // Service methods
            for (int i = 0; i < sourceMethods.Length; i++)
            {
                var sourceMethod = sourceMethods[i];
                var implemMethodType = implMethods[i];
                var handlerMethodInfo = implemMethodType.GetMethod("Handle");
                var privateFiled = privateFields[i];
                var paramsInfo = sourceMethod.GetParameters();
                var serviceMethodParams = paramsInfo.Select(p => p.ParameterType).ToArray();


                var serviceMethod = proxyType.DefineMethod(sourceMethod.Name,
                    MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual
                    , CallingConventions.Standard, sourceMethod.ReturnType, serviceMethodParams);

                for (int j = 0; j < paramsInfo.Length; j++)
                {
                    serviceMethod.DefineParameter(j + 1, ParameterAttributes.None, paramsInfo[j].Name);
                }

                var serviceMethodIL = serviceMethod.GetILGenerator();
                var exIndex = 0;
                if (sourceMethod.ReturnType != typeof(void))
                {
                    serviceMethodIL.DeclareLocal(sourceMethod.ReturnType); //.SetLocalSymInfo("result");
                    ++exIndex;
                }

                serviceMethodIL.DeclareLocal(typeof(Exception)); //.SetLocalSymInfo("e");

                var serviceMethodLabelExit = desposeIL.DefineLabel();


                serviceMethodIL.BeginExceptionBlock();

                serviceMethodIL.Emit(OpCodes.Ldarg_0);
                serviceMethodIL.Emit(OpCodes.Ldfld, privateFiled);


                if (paramsInfo.Length > 0)
                {
                    for (int j = 0; j < paramsInfo.Length; j++)
                    {
                        serviceMethodIL.Emit(OpCodes.Ldarg, j + 1);
                    }

                }

                serviceMethodIL.Emit(OpCodes.Callvirt, handlerMethodInfo);
                if (sourceMethod.ReturnType != typeof(void))
                {
                    serviceMethodIL.Emit(OpCodes.Stloc_0);
                }
                //serviceMethodIL.Emit(OpCodes.Leave_S, serviceMethodLabelExit);

                serviceMethodIL.BeginCatchBlock(typeof(Exception));
                if (exIndex == 0)
                    serviceMethodIL.Emit(OpCodes.Stloc_0);
                else
                    serviceMethodIL.Emit(OpCodes.Stloc_1);
                serviceMethodIL.Emit(OpCodes.Rethrow);
                serviceMethodIL.EndExceptionBlock();

                //serviceMethodIL.MarkLabel(serviceMethodLabelExit);
                if (sourceMethod.ReturnType != typeof(void))
                {
                    serviceMethodIL.Emit(OpCodes.Ldloc_0);
                }
                serviceMethodIL.Emit(OpCodes.Ret);
            }

            var result = proxyType.CreateType();
            //dynGeneratorHostAssembly.Save($"{assemblyName}.dll");

            return result;
        }

        private Type GetHandlerTypeByOperationName(Assembly assembly, string operationName)
        {
            var types = assembly.GetExportedTypes();

            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if ((type.Name.Equals(operationName, StringComparison.OrdinalIgnoreCase) 
                    || type.Name.Equals(operationName + "Handler", StringComparison.OrdinalIgnoreCase))
                    && type.IsSealed && type.IsClass)
                {
                    var method = type.GetMethod("Handle");
                    if (method != null && method.IsPublic)
                    {
                        return type;
                    }
                }
            }
            return null;
        }
    }
}
