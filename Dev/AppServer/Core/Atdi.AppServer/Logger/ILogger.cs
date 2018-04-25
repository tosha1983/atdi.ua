using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer
{
    /// <summary>
    /// Represent the logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets a value indicating whether logging is enabled for the Fatal level
        /// </summary>
        bool IsFatalLevelEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the Warn level
        /// </summary>
        bool IsWarnLevelEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the Info level
        /// </summary>
        bool IsInfoLevelEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the Debug level.
        /// </summary>
        bool IsDebugLevelEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the Trace level
        /// </summary>
        bool IsTraceLevelEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the Error level.
        /// </summary>
        bool IsErrorLevelEnabled { get; }


        Guid Info(string message);
        Guid Info(string format, params object[] args);
        Guid Info(IFormatProvider formatProvider, string format, params object[] args);
        Guid Info(Func<string> messageFactory);
        Guid Info<TArg>(Func<TArg, string> messageFactory, TArg arg);
        Guid Info<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2);
        Guid Info<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3);
        Guid Info<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
        Guid Info<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
        Guid Info(Exception exception);
        Guid Info(Exception exception, string message);
        Guid Info(Exception exception, string format, params object[] args);
        Guid Info(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

        Guid Warn(string message);
        Guid Warn(string format, params object[] args);
        Guid Warn(IFormatProvider formatProvider, string format, params object[] args);
        Guid Warn(Func<string> messageFactory);
        Guid Warn<TArg>(Func<TArg, string> messageFactory, TArg arg);
        Guid Warn<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2);
        Guid Warn<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3);
        Guid Warn<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
        Guid Warn<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
        Guid Warn(Exception exception);
        Guid Warn(Exception exception, string message);
        Guid Warn(Exception exception, string format, params object[] args);
        Guid Warn(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

        Guid Trace(string message);
        Guid Trace(string format, params object[] args);
        Guid Trace(IFormatProvider formatProvider, string format, params object[] args);
        Guid Trace(Func<string> messageFactory);
        Guid Trace<TArg>(Func<TArg, string> messageFactory, TArg arg);
        Guid Trace<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2);
        Guid Trace<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3);
        Guid Trace<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
        Guid Trace<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
        Guid Trace(Exception exception);
        Guid Trace(Exception exception, string message);
        Guid Trace(Exception exception, string format, params object[] args);
        Guid Trace(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

        Guid Debug(string message);
        Guid Debug(string format, params object[] args);
        Guid Debug(IFormatProvider formatProvider, string format, params object[] args);
        Guid Debug(Func<string> messageFactory);
        Guid Debug<TArg>(Func<TArg, string> messageFactory, TArg arg);
        Guid Debug<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2);
        Guid Debug<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3);
        Guid Debug<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
        Guid Debug<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
        Guid Debug(Exception exception);
        Guid Debug(Exception exception, string message);
        Guid Debug(Exception exception, string format, params object[] args);
        Guid Debug(Exception exception, IFormatProvider formatProvider, string format, params object[] args);


        Guid Error(string message);
        Guid Error(string format, params object[] args);
        Guid Error(IFormatProvider formatProvider, string format, params object[] args);
        Guid Error(Func<string> messageFactory);
        Guid Error<TArg>(Func<TArg, string> messageFactory, TArg arg);
        Guid Error<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2);
        Guid Error<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3);
        Guid Error<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
        Guid Error<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
        Guid Error(Exception exception);
        Guid Error(Exception exception, string message);
        Guid Error(Exception exception, string format, params object[] args);
        Guid Error(Exception exception, IFormatProvider formatProvider, string format, params object[] args);


        Guid Fatal(string message);
        Guid Fatal(string format, params object[] args);
        Guid Fatal(IFormatProvider formatProvider, string format, params object[] args);
        Guid Fatal(Func<string> messageFactory);
        Guid Fatal<TArg>(Func<TArg, string> messageFactory, TArg arg);
        Guid Fatal<TArg1, TArg2>(Func<TArg1, TArg2, string> messageFactory, TArg1 arg1, TArg2 arg2);
        Guid Fatal<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3);
        Guid Fatal<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
        Guid Fatal<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, string> messageFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
        Guid Fatal(Exception exception);
        Guid Fatal(Exception exception, string message);
        Guid Fatal(Exception exception, string format, params object[] args);
        Guid Fatal(Exception exception, IFormatProvider formatProvider, string format, params object[] args);
    }
}
