using Atdi.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Atdi.AppUnits.Sdrn.CalcServer.DataModel;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal class IterationDescriptor
	{
		public delegate object HandlerInvoker(object instance, ITaskContext taskContext, object data);

		public IterationDescriptor(Type handlerType)
		{
			this.HandlerType = handlerType;
			var contractName = typeof(IIterationHandler<,>).Name;
			var handlerInterface = handlerType.GetInterface(contractName);
			if (handlerInterface == null)
			{
				throw new InvalidOperationException($"The handler type '{handlerType}' does not implement a contract '{contractName}'");
			}

			if (handlerInterface.GenericTypeArguments.Length != 2)
			{
				throw new InvalidOperationException($"Invalid handler type '{handlerType}' definition");
			}

			this.DataType = handlerInterface.GenericTypeArguments[0];
			this.ResultType = handlerInterface.GenericTypeArguments[1];

			this.Invoker = CreateInvoker(handlerType.GetMethod("Run"));
		}

		public string Key => MakeKey(this.DataType, this.ResultType);

		public static string MakeKey(Type dataType, Type resultType)
		{
			return $"[{dataType}]:[{resultType}]";
		}
		public static string MakeKey<TData, TResult>()
		{
			return MakeKey(typeof(TData), typeof(TResult));
		}

		private HandlerInvoker CreateInvoker(MethodInfo method)
		{
			var targetArg = Expression.Parameter(typeof(object));

			var contextParam = Expression.Parameter(typeof(ITaskContext));
			var dataParam = Expression.Parameter(this.DataType);
			//var resultParam = Expression.Parameter(this.ResultType);

			var instance = Expression.Convert(targetArg, method.DeclaringType);

			var methodParams = method.GetParameters();
			if (methodParams.Length != 2)
			{
				throw new InvalidOperationException("Invalid result handler definition");
			}
			if (methodParams[0].ParameterType != typeof(ITaskContext))
			{
				throw new InvalidOperationException("Invalid result handler definition");
			}

			var contextArg = Expression.Convert(contextParam, methodParams[0].ParameterType);
			var dataArg = Expression.Convert(dataParam, methodParams[1].ParameterType);

			Expression body = Expression.Call(instance, method, contextArg, dataArg);

			if (body.Type == typeof(void))
			{
				throw new InvalidOperationException("Invalid result handler definition");
			}

			//var block = Expression.Block(body, Expression.Constant(null));
			var block = Expression.Convert(body, this.ResultType);
			var lambda = Expression.Lambda<HandlerInvoker>(block, targetArg, contextParam, dataParam);

			return lambda.Compile();
		}

		public Type HandlerType { get; }

		public Type DataType { get; }

		public Type ResultType { get; }

		public HandlerInvoker Invoker { get; }

	}
}
