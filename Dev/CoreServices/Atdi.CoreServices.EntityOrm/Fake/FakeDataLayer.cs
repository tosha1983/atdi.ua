using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers.Fake
{
    class FakeDataLayer<TDataOrm> : IDataLayer<TDataOrm>
        where TDataOrm : IDataOrm
    {
        class FakeQueryBuilder : IQueryBuilder
        {
            class FakeQueryDeleteStatement : IQueryDeleteStatement
            {
                public IQueryDeleteStatement Where(Condition condition)
                {
                    return this;
                }
            }

            class FakeQuerySelectStatement : IQuerySelectStatement
            {
                public IQuerySelectStatement Distinct()
                {
                    return this;
                }

                public IQuerySelectStatement OnPercentTop(int percent)
                {
                    return this;
                }

                public IQuerySelectStatement OnTop(int count)
                {
                    return this;
                }

                public IQuerySelectStatement OrderByAsc(params string[] columns)
                {
                    return this;
                }

                public IQuerySelectStatement OrderByDesc(params string[] columns)
                {
                    return this;
                }

                public IQuerySelectStatement Select(params string[] columns)
                {
                    return this;
                }

                public IQuerySelectStatement Where(Condition condition)
                {
                    return this;
                }
            }

            class FakeQuerySelectStatement<TModel> : IQuerySelectStatement<TModel>
            {
                public IQuerySelectStatement<TModel> Distinct()
                {
                    return this;
                }

                public IQuerySelectStatement<TModel> OnPercentTop(int percent)
                {
                    return this;
                }

                public IQuerySelectStatement<TModel> OnTop(int count)
                {
                    return this;
                }

                public IQuerySelectStatement<TModel> OrderByAsc(params Expression<Func<TModel, object>>[] columnsExpressions)
                {
                    return this;
                }

                public IQuerySelectStatement<TModel> OrderByDesc(params Expression<Func<TModel, object>>[] columnsExpressions)
                {
                    return this;
                }

                public IQuerySelectStatement<TModel> Select(params Expression<Func<TModel, object>>[] columnsExpressions)
                {
                    return this;
                }

                public IQuerySelectStatement<TModel> Where(Expression<Func<TModel, bool>> expression)
                {
                    return this;
                }

                public IQuerySelectStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression)
                {
                    return this;
                }

                public IQuerySelectStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
                {
                    return this;
                }
            }

            class FakeQueryInsertStatement : IQueryInsertStatement
            {
                public IQueryInsertStatement SetValue(ColumnValue columnValue)
                {
                    return this;
                }

                public IQueryInsertStatement SetValues(ColumnValue[] columnsValues)
                {
                    return this;
                }
            }

            class FakeQueryUpdateStatement : IQueryUpdateStatement
            {
                public IQueryUpdateStatement SetValue(ColumnValue columnValue)
                {
                    return this;
                }

                public IQueryUpdateStatement SetValues(ColumnValue[] columnsValues)
                {
                    return this;
                }

                public IQueryUpdateStatement Where(Condition condition)
                {
                    return this;
                }
            }

            public IQueryDeleteStatement Delete(string tableName)
            {
                return new FakeQueryDeleteStatement();
            }

            public IQuerySelectStatement From(string tableName)
            {
                return new FakeQuerySelectStatement();
            }

            public IQuerySelectStatement<TModel> From<TModel>()
            {
                return new FakeQuerySelectStatement<TModel>();
            }

            public IQueryInsertStatement Insert(string tableName)
            {
                return new FakeQueryInsertStatement();
            }

            public IQueryUpdateStatement Update(string tableName)
            {
                return new FakeQueryUpdateStatement();
            }
        }

        class FakeQueryExecutor : IQueryExecutor
        {
            public int Execute(IQueryStatement statement)
            {
                return 0;
            }

            public int Execute<TModel>(IQueryStatement statement)
            {
                throw new NotImplementedException();
            }

            public TResult Fetch<TResult>(IQuerySelectStatement statement, Func<IDataReader, TResult> handler)
            {
                return default(TResult);
            }

            public TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
            {
                return default(TResult);
            }

            public DataSet Fetch(IQuerySelectStatement statement, DataSetColumn[] columns, DataSetStructure structure)
            {
                return new DataSet();
            }
        }

        class FakeQueryBuilder<TModel> : IQueryBuilder<TModel>
        {
            class FakeQueryDeleteStatement<TModelIn> : IQueryDeleteStatement<TModelIn>
            {
                public IQueryDeleteStatement<TModelIn> Where(Expression<Func<TModelIn, bool>> expression)
                {
                    return this;
                }

                public IQueryDeleteStatement<TModelIn> Where(Expression<Func<TModelIn, IQuerySelectStatementOperation, bool>> expression)
                {
                    return this;
                }

                public IQueryDeleteStatement<TModelIn> Where<TValue>(Expression<Func<TModelIn, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
                {
                    return this;
                }
            }

            class FakeQuerySelectStatement<TModelIn> : IQuerySelectStatement<TModelIn>
            {
                public IQuerySelectStatement<TModelIn> Distinct()
                {
                    return this;
                }

                public IQuerySelectStatement<TModelIn> OnPercentTop(int percent)
                {
                    return this;
                }

                public IQuerySelectStatement<TModelIn> OnTop(int count)
                {
                    return this;
                }

                public IQuerySelectStatement<TModelIn> OrderByAsc(params Expression<Func<TModelIn, object>>[] columnsExpressions)
                {
                    return this;
                }

                public IQuerySelectStatement<TModelIn> OrderByDesc(params Expression<Func<TModelIn, object>>[] columnsExpressions)
                {
                    return this;
                }

                public IQuerySelectStatement<TModelIn> Select(params Expression<Func<TModelIn, object>>[] columnsExpressions)
                {
                    return this;
                }

                public IQuerySelectStatement<TModelIn> Where(Expression<Func<TModelIn, bool>> expression)
                {
                    return this;
                }

                public IQuerySelectStatement<TModelIn> Where(Expression<Func<TModelIn, IQuerySelectStatementOperation, bool>> expression)
                {
                    return this;
                }

                public IQuerySelectStatement<TModelIn> Where<TValue>(Expression<Func<TModelIn, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
                {
                    return this;
                }
            }

            class FakeQueryInsertStatement<TModelIn> : IQueryInsertStatement<TModelIn>
            {
                public IQueryInsertStatement<TModelIn> SetValue<TValue>(Expression<Func<TModelIn, TValue>> columnsExpression, TValue value)
                {
                    return this;
                }
            }

            class FakeQueryUpdateStatement<TModelIn> : IQueryUpdateStatement<TModelIn>
            {
                public IQueryUpdateStatement<TModelIn> SetValue<TValue>(Expression<Func<TModelIn, TValue>> columnsExpression, TValue value)
                {
                    return this;
                }

                public IQueryUpdateStatement<TModelIn> Where(Expression<Func<TModelIn, bool>> expression)
                {
                    return this;
                }

                public IQueryUpdateStatement<TModelIn> Where(Expression<Func<TModelIn, IQuerySelectStatementOperation, bool>> expression)
                {
                    return this;
                }

                public IQueryUpdateStatement<TModelIn> Where<TValue>(Expression<Func<TModelIn, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values)
                {
                    return this;
                }
            }

            public IQueryDeleteStatement<TModel> Delete()
            {
                return new FakeQueryDeleteStatement<TModel>();
            }

            public IQuerySelectStatement<TModel> From()
            {
               return new FakeQuerySelectStatement<TModel>();
            }

            public IQueryInsertStatement<TModel> Insert()
            {
                return new FakeQueryInsertStatement<TModel>();
            }

            public IQueryUpdateStatement<TModel> Update()
            {
                return new FakeQueryUpdateStatement<TModel>();
            }
        }

        class FakeDataEngine : IDataEngine
        {
            public IDataEngineConfig Config => throw new NotImplementedException();

            public IEngineSyntax Syntax => new Atdi.CoreServices.DataLayer.OracleEngineSyntax();

            public void Execute(EngineCommand command, Action<System.Data.IDataReader> handler)
            {
                
            }

            public int Execute(EngineCommand command)
            {
                return 0;
            }

            public object ExecuteScalar(EngineCommand command)
            {
                return null;
            }
        }

        public IQueryBuilder Builder => new FakeQueryBuilder();

        public IQueryExecutor Executor<TContext>() where TContext : IDataContext, new()
        {
            return new FakeQueryExecutor();
        }

        public IQueryBuilder<TModel> GetBuilder<TModel>()
        {
            return new FakeQueryBuilder<TModel>();
        }

        public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            return new FakeDataEngine();
        }
    }
}
