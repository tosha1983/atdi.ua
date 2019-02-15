using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class CommandHandler
    {
        public CommandHandler(MethodInfo method, Type commandType, Type resultType)
        {
            this.CommandType = commandType;
            this.ResultType = resultType;
            this.Invoker = CreateInvoker(method);
        }

        public Action<IAdapter, ICommand, IExecutionContext> Invoker { get; private set; }
        public Type ResultType { get; private set; }
        public Type CommandType { get; private set; }

        static Action<IAdapter, ICommand, IExecutionContext> CreateInvoker(MethodInfo method)
        {
            var targetArg = Expression.Parameter(typeof(IAdapter));
            var commandParam = Expression.Parameter(typeof(ICommand));
            var contextParam = Expression.Parameter(typeof(IExecutionContext));

            var instance = Expression.Convert(targetArg, method.DeclaringType);

            var methodParams = method.GetParameters();
            if (methodParams.Length != 2)
            {
                throw new InvalidOperationException("Invalid handler definition");
            }
            if (methodParams[1].ParameterType != typeof(IExecutionContext))
            {
                throw new InvalidOperationException("Invalid handler definition");
            }
            var commandArg = Expression.Convert(commandParam, methodParams[0].ParameterType);
            var contextArg = Expression.Convert(contextParam, methodParams[1].ParameterType);



            Expression body = Expression.Call(instance, method, commandArg, contextArg);

            if (body.Type != typeof(void))
            {
                throw new InvalidOperationException("Invalid handler definition");
            }

            var block = Expression.Block(body, Expression.Constant(null));


            var lambda = Expression.Lambda<Action<IAdapter, ICommand, IExecutionContext>>(block, targetArg, commandParam, contextParam);

            return lambda.Compile();
        }
    }
}
