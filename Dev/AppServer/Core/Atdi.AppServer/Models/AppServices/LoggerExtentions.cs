using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;

namespace Atdi.AppServer.Models.AppServices
{
    public static class LoggerExtentions
    {
        public static Guid Trace<TService, TOperation, TOptions, TResult>(this ILogger logger, IAppOperationHandler<TService, TOperation, TOptions, TResult> handler)
            where TService : class, IAppService, new()
            where TOperation : class, IAppOperation, new()
            where TOptions : class, IAppOperationOptions, new()
        {
            return logger.Trace("Execute operation '{0}' of service '{1}'".With(handler.Operation.Name, handler.Service.Name));
        }

        public static Guid Trace<TService, TOperation, TOptions, TResult, TArg>(this ILogger logger, IAppOperationHandler<TService, TOperation, TOptions, TResult> handler, TArg arg)
            where TService : class, IAppService, new()
            where TOperation : class, IAppOperation, new()
            where TOptions : class, IAppOperationOptions, new()
        {
            return logger.Trace((a) => MessageFactory(handler, a), arg);
        }

        public static Guid Trace<TService, TOperation, TOptions, TResult, TArg1, Targ2>(this ILogger logger, IAppOperationHandler<TService, TOperation, TOptions, TResult> handler, TArg1 arg1, Targ2 arg2)
            where TService : class, IAppService, new()
            where TOperation : class, IAppOperation, new()
            where TOptions : class, IAppOperationOptions, new()
        {
            return logger.Trace((a1, a2) => MessageFactory(handler, a1, a2), arg1, arg2);
        }

        public static Guid Trace(this ILogger logger, IAppOperation operation)
        {
            return logger.Trace("Execute operation '{0}'".With(operation.Name));
        }

        public static Guid Trace<TArg>(this ILogger logger, IAppOperation operation, TArg arg)
        {
            return logger.Trace((a) => MessageFactory(operation, a), arg);
        }

        public static Guid Trace<TArg1, TArg2>(this ILogger logger, IAppOperation operation, TArg1 arg1, TArg2 arg2)
        {
            return logger.Trace((a1, a2) => MessageFactory(operation, a1, a2), arg1, arg2);
        }

        private static string MessageFactory<TArg>(IAppOperation operation, TArg arg)
        {
            return string.Format("Execute operation '{0}': Arg = '{1}'".With(operation.Name, arg));
        }

        private static string MessageFactory<TArg1, TArg2>(IAppOperation operation, TArg1 arg1, TArg2 arg2)
        {
            return string.Format("Execute operation '{0}': Arg1 = '{1}', Arg2 = '{2}'".With(operation.Name, arg1, arg2));
        }

        private static string MessageFactory<TService, TOperation, TOptions, TResult, TArg>(IAppOperationHandler<TService, TOperation, TOptions, TResult> handler, TArg arg)
            where TService : class, IAppService, new()
            where TOperation : class, IAppOperation, new()
            where TOptions : class, IAppOperationOptions, new()
        {
            return string.Format("Execute operation '{0}' of service '{1}': Arg = '{2}'".With(handler.Operation.Name, handler.Service.Name, arg));
        }

        private static string MessageFactory<TService, TOperation, TOptions, TResult, TArg1, TArg2>(IAppOperationHandler<TService, TOperation, TOptions, TResult> handler, TArg1 arg1, TArg2 arg2)
            where TService : class, IAppService, new()
            where TOperation : class, IAppOperation, new()
            where TOptions : class, IAppOperationOptions, new()
        {
            return string.Format("Execute operation '{0}' of service '{1}': Arg1 = '{2}', Arg2 = '{3}'".With(handler.Operation.Name, handler.Service.Name, arg1, arg2));
        }
    }
}
