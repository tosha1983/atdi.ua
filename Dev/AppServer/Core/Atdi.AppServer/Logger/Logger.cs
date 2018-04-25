using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;

namespace Atdi.AppServer
{
    internal class Logger : ILogger
    {
        private readonly Castle.Core.Logging.ILogger _castleLogger;

        public Logger(Castle.Core.Logging.ILogger castleLogger)
        {
            this._castleLogger = castleLogger;
        }

        bool ILogger.IsFatalLevelEnabled => true;

        bool ILogger.IsWarnLevelEnabled => true;

        bool ILogger.IsInfoLevelEnabled => true;

        bool ILogger.IsDebugLevelEnabled => true;

        bool ILogger.IsTraceLevelEnabled => true;

        bool ILogger.IsErrorLevelEnabled => true;

        private static string MakeMessage(Guid code, string message)
        {
            return string.Concat("[", code.ToString("B"), "] ", message);
        }

        Guid ILogger.Debug(string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Debug(string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, format.With(args)));
            return code;
        }

        Guid ILogger.Debug(IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, format.With(formatProvider, args)));
            return code;
        }

        Guid ILogger.Debug(Func<string> messageFactory)
        {
            var code = Guid.NewGuid();
            var message = messageFactory();
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Debug<TArg>(Func<TArg, string> messageFactory, TArg arg)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Debug<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Debug<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Debug<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Debug<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4, arg5);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Debug(Exception exception)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, ""), exception);
            return code;
        }

        Guid ILogger.Debug(Exception exception, string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, message), exception);
            return code;
        }

        Guid ILogger.Debug(Exception exception, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, format.With(args)), exception);
            return code;
        }

        Guid ILogger.Debug(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, format.With(formatProvider, args)), exception);
            return code;
        }

        Guid ILogger.Error(string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Error(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Error(string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Error(MakeMessage(code, format.With(args)));
            return code;
        }

        Guid ILogger.Error(IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Error(MakeMessage(code, format.With(formatProvider, args)));
            return code;
        }

        Guid ILogger.Error(Func<string> messageFactory)
        {
            var code = Guid.NewGuid();
            var message = messageFactory();
            this._castleLogger.Error(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Error<TArg>(Func<TArg, string> messageFactory, TArg arg)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg);
            this._castleLogger.Error(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Error<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2);
            this._castleLogger.Error(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Error<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3);
            this._castleLogger.Error(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Error<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4);
            this._castleLogger.Error(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Error<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4, arg5);
            this._castleLogger.Error(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Error(Exception exception)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Error(MakeMessage(code, ""), exception);
            return code;
        }

        Guid ILogger.Error(Exception exception, string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Error(MakeMessage(code, message), exception);
            return code;
        }

        Guid ILogger.Error(Exception exception, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Error(MakeMessage(code, format.With(args)), exception);
            return code;
        }

        Guid ILogger.Error(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Error(MakeMessage(code, format.With(formatProvider, args)), exception);
            return code;
        }

        Guid ILogger.Fatal(string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Fatal(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Fatal(string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Fatal(MakeMessage(code, format.With(args)));
            return code;
        }

        Guid ILogger.Fatal(IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Fatal(MakeMessage(code, format.With(formatProvider, args)));
            return code;
        }

        Guid ILogger.Fatal(Func<string> messageFactory)
        {
            var code = Guid.NewGuid();
            var message = messageFactory();
            this._castleLogger.Fatal(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Fatal<TArg>(Func<TArg, string> messageFactory, TArg arg)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg);
            this._castleLogger.Fatal(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Fatal<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2);
            this._castleLogger.Fatal(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Fatal<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3);
            this._castleLogger.Fatal(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Fatal<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4);
            this._castleLogger.Fatal(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Fatal<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4, arg5);
            this._castleLogger.Fatal(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Fatal(Exception exception)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Fatal(MakeMessage(code, ""), exception);
            return code;
        }

        Guid ILogger.Fatal(Exception exception, string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Fatal(MakeMessage(code, message), exception);
            return code;
        }

        Guid ILogger.Fatal(Exception exception, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Fatal(MakeMessage(code, format.With(args)), exception);
            return code;
        }

        Guid ILogger.Fatal(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Fatal(MakeMessage(code, format.With(formatProvider, args)), exception);
            return code;
        }

        Guid ILogger.Info(string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Info(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Info(string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Info(MakeMessage(code, format.With(args)));
            return code;
        }

        Guid ILogger.Info(IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Info(MakeMessage(code, format.With(formatProvider, args)));
            return code;
        }

        Guid ILogger.Info(Func<string> messageFactory)
        {
            var code = Guid.NewGuid();
            var message = messageFactory();
            this._castleLogger.Info(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Info<TArg>(Func<TArg, string> messageFactory, TArg arg)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg);
            this._castleLogger.Info(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Info<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2);
            this._castleLogger.Info(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Info<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3);
            this._castleLogger.Info(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Info<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4);
            this._castleLogger.Info(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Info<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4, arg5);
            this._castleLogger.Info(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Info(Exception exception)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Info(MakeMessage(code, ""), exception);
            return code;
        }

        Guid ILogger.Info(Exception exception, string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Info(MakeMessage(code, message), exception);
            return code;
        }

        Guid ILogger.Info(Exception exception, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Info(MakeMessage(code, format.With(args)), exception);
            return code;
        }

        Guid ILogger.Info(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Info(MakeMessage(code, format.With(formatProvider, args)), exception);
            return code;
        }

        Guid ILogger.Trace(string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Trace(string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, format.With(args)));
            return code;
        }

        Guid ILogger.Trace(IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, format.With(formatProvider, args)));
            return code;
        }

        Guid ILogger.Trace(Func<string> messageFactory)
        {
            var code = Guid.NewGuid();
            var message = messageFactory();
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Trace<TArg>(Func<TArg, string> messageFactory, TArg arg)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Trace<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Trace<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Trace<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Trace<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4, arg5);
            this._castleLogger.Debug(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Trace(Exception exception)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, ""), exception);
            return code;
        }

        Guid ILogger.Trace(Exception exception, string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, message), exception);
            return code;
        }

        Guid ILogger.Trace(Exception exception, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, format.With(args)), exception);
            return code;
        }

        Guid ILogger.Trace(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Debug(MakeMessage(code, format.With(formatProvider, args)), exception);
            return code;
        }

        Guid ILogger.Warn(string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Warn(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Warn(string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Warn(MakeMessage(code, format.With(args)));
            return code;
        }

        Guid ILogger.Warn(IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Warn(MakeMessage(code, format.With(formatProvider, args)));
            return code;
        }

        Guid ILogger.Warn(Func<string> messageFactory)
        {
            var code = Guid.NewGuid();
            var message = messageFactory();
            this._castleLogger.Warn(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Warn<TArg>(Func<TArg, string> messageFactory, TArg arg)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg);
            this._castleLogger.Warn(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Warn<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2);
            this._castleLogger.Warn(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Warn<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3);
            this._castleLogger.Warn(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Warn<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4);
            this._castleLogger.Warn(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Warn<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            var code = Guid.NewGuid();
            var message = messageFactory(arg1, arg2, arg3, arg4, arg5);
            this._castleLogger.Warn(MakeMessage(code, message));
            return code;
        }

        Guid ILogger.Warn(Exception exception)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Warn(MakeMessage(code, ""), exception);
            return code;
        }

        Guid ILogger.Warn(Exception exception, string message)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Warn(MakeMessage(code, message), exception);
            return code;
        }

        Guid ILogger.Warn(Exception exception, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Warn(MakeMessage(code, format.With(args)), exception);
            return code;
        }

        Guid ILogger.Warn(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            var code = Guid.NewGuid();
            this._castleLogger.Warn(MakeMessage(code, format.With(formatProvider, args)), exception);
            return code;
        }
    }
}
