using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal class CommandHandler
    {
        private readonly IStatistics _statistics;

        public CommandHandler(MethodInfo method, Type commandType, Type resultType, IStatistics statistics)
        {
            _statistics = statistics;
            this.CommandType = commandType;
            this.ResultType = resultType;
            this.Invoker = CreateInvoker(method);

            if (this._statistics != null)
            {
                var context = string.Intern($"{commandType.Name}({resultType.Name})");
                this.StartedCounter = _statistics.Counter(Monitoring.DefineAdapterCommandCounter(context, "Started"));
                this.RunningCounter = _statistics.Counter(Monitoring.DefineAdapterCommandCounter(context, "Running"));
                this.CompletedCounter = _statistics.Counter(Monitoring.DefineAdapterCommandCounter(context, "Completed"));
                this.CanceledCounter = _statistics.Counter(Monitoring.DefineAdapterCommandCounter(context, "Canceled"));
                this.AbortedCounter = _statistics.Counter(Monitoring.DefineAdapterCommandCounter(context, "Aborted"));
            }

        }

        public IStatisticCounter AbortedCounter { get;  }

        public IStatisticCounter CanceledCounter { get; }

        public IStatisticCounter CompletedCounter { get; }

        public IStatisticCounter RunningCounter { get; }

        public IStatisticCounter StartedCounter { get; }

        public Action<IAdapter, ICommand, IExecutionContext> Invoker { get; }
        private Type ResultType { get; }
        private Type CommandType { get; }

        private static Action<IAdapter, ICommand, IExecutionContext> CreateInvoker(MethodInfo method)
        {
            var targetArg = Expression.Parameter(typeof(IAdapter));
            var commandParam = Expression.Parameter(typeof(ICommand));
            var contextParam = Expression.Parameter(typeof(IExecutionContext));

            var instance = Expression.Convert(targetArg, method.DeclaringType ?? throw new InvalidOperationException());

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
