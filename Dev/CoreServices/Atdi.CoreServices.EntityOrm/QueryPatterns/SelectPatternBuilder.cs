using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm.QueryPatterns
{

    internal sealed class SelectPatternBuilder : LoggedObject, IPatternBuilder
    {
        class BuildingContext
        {
            private int _aliasCounter = 0;

            public string GenerateAlias(IEntityMetadata entity)
            {
                ++_aliasCounter;
                return $"{entity.Name}_{_aliasCounter}";
            }

            public IDataTypeMetadata[] TypeMetadatas { get; set; }
        }

        private readonly IEntityOrm _entityOrm;
        private readonly DataTypeSystem _dataTypeSystem;

        public SelectPatternBuilder(IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
            this._dataTypeSystem = dataTypeSystem;
        }

        public TResult BuildAndExecute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex)
        {
            var statement = executionContex.Statement as QuerySelectStatement;

            var context = new BuildingContext();
            // построение запроса согласно патерна
            var pattern = this.Build(statement);

            // выполняем запросы согласно патерна
            return this.Execute(executionContex, pattern, context);
        }

        private PS.SelectPattern Build(QuerySelectStatement statement)
        {
            var pattren = new PS.SelectPattern
            {

            };

            return pattren;
        }

        private TResult Execute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex, PS.SelectPattern pattern, BuildingContext context)
        {
            var result = default(TResult);


            switch (executionContex.ResultKind)
            {
                case EngineExecutionResultKind.None:
                    executionContex.Executer.Execute(pattern);
                    break;
                case EngineExecutionResultKind.RowsAffected:
                    var execResult = pattern.DefResult<EngineExecutionRowsAffectedResult>();
                    executionContex.Executer.Execute(pattern);
                    result = (TResult)(object)execResult.RowsAffected;
                    break;
                case EngineExecutionResultKind.Scalar:
                    // скалярный тип возвращает что то одно... пока то что сможет вернуть провайдер
                    var scalarResult = pattern.DefResult<EngineExecutionScalarResult>();
                    executionContex.Executer.Execute(pattern);
                    result = (TResult)scalarResult.Value;
                    break;
                case EngineExecutionResultKind.Reader:
                    var readerResult = pattern.DefResult<EngineExecutionReaderResult>();
                    readerResult.Handler = (reader) => 
                    {
                        if (executionContex is PatternExecutionContexWithHandler<TResult, TModel> conetx1)
                        {
                            var ormReader = new QueryDataReader<TModel>(reader, context.TypeMetadatas, _dataTypeSystem);
                            result = conetx1.Handler(ormReader);
                        }
                        else if ((object)executionContex is PatternExecutionContexWithHandler<TResult> conetx2)
                        {
                            var ormReader = new QueryDataReader(reader, context.TypeMetadatas, _dataTypeSystem);
                            result = conetx2.Handler(ormReader);
                        }
                    };
                    executionContex.Executer.Execute(pattern);
                    break;
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{executionContex.ResultKind}'");
            }


            return result;
        }
    }
}
