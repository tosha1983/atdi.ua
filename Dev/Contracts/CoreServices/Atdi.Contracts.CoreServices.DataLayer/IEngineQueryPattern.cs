using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public enum EngineExecutionResultKind
    {
        /// <summary>
        /// Результат отсутвует
        /// </summary>
        None = 0,

        /// <summary>
        /// Результат в виде кол-ва задействоаных строк в результате запроса или комманды 
        /// </summary>
        RowsAffected,

        /// <summary>
        /// Результат в виде скалярного значения
        /// </summary>
        Scalar,

        /// <summary>
        /// Результат в виде потока данных
        /// Используется паттерн пердачи делегата который будет вызвон после выполнения запроса или комманды
        /// Параметры делагате произвольные но главнео наличие ссылки на объект ридера данных
        /// </summary>
        Reader,

        /// <summary>
        /// Результа в виде согласованной структуры в рамках определенного патерна.
        /// </summary>
        Custom
    }

    public interface IEngineExecutionResult
    {
        EngineExecutionResultKind Kind { get; }
    }

    public abstract class EngineExecutionResult : IEngineExecutionResult
    {
        public static readonly EngineExecutionNoResult Default = new EngineExecutionNoResult();

        public EngineExecutionResult (EngineExecutionResultKind resultKind)
        {
            this.Kind = resultKind;
        }

        public EngineExecutionResultKind Kind { get; }

        public override string ToString()
        {
            return $"Kind = '{this.Kind}', Name = '{this.GetType().Name}'";
        }
    }

    public class EngineExecutionNoResult : EngineExecutionResult
    {
        public EngineExecutionNoResult()
            : base(EngineExecutionResultKind.None)
        {
        }
    }

    public class EngineExecutionRowsAffectedResult : EngineExecutionResult
    {
        public EngineExecutionRowsAffectedResult() 
            : base(EngineExecutionResultKind.RowsAffected)
        {
        }

        public int RowsAffected { get; set; }
    }

    public class EngineExecutionScalarResult<T> : EngineExecutionResult
    {
        public EngineExecutionScalarResult()
            : base(EngineExecutionResultKind.Scalar)
        {
        }

        public T Value { get; set; }
    }

    public class EngineExecutionScalarResult : EngineExecutionResult
    {
        public EngineExecutionScalarResult()
            : base(EngineExecutionResultKind.Scalar)
        {
        }
        public object Value { get; set; }
    }

    public class EngineExecutionReaderResult<TReader> : EngineExecutionResult
    {
        public EngineExecutionReaderResult()
            : base(EngineExecutionResultKind.Reader)
        {
        }

        public Action<TReader> Handler { get; set; }
    }

    public class EngineExecutionReaderResult : EngineExecutionReaderResult<IEngineDataReader>
    {
    }

    public abstract class EngineExecutionCustomResult : EngineExecutionResult
    {
        public EngineExecutionCustomResult()
            : base(EngineExecutionResultKind.Custom)
        {
        }
    }

    public interface IEngineQueryPattern
    {
        IEngineExecutionResult Result { get; }

        TResult DefResult<TResult>()
           where TResult : class, IEngineExecutionResult, new();

        TResult AsResult<TResult>()
           where TResult : class, IEngineExecutionResult, new();
    }



    public class EngineQueryPattern : IEngineQueryPattern
    {
        public EngineQueryPattern()
        {
            this.Result = EngineExecutionResult.Default;
        }

        public IEngineExecutionResult Result { get; set; }

        public TResult DefResult<TResult>()
            where TResult : class, IEngineExecutionResult, new()
        {
            this.Result = new TResult();

            return (TResult)this.Result;
        }

        public TResult AsResult<TResult>()
             where TResult : class, IEngineExecutionResult, new()
        {
            return (TResult)this.Result;
        }

        public override string ToString()
        {
            return $"Name = '{this.GetType().Name}', ResultKind = '{this.Result.Kind}' ";
        }

    }

 

    
}
