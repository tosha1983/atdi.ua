﻿using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryBuilder<TModel> : LoggedObject, IQueryBuilder<TModel>
    {
        public QueryBuilder(ILogger logger) : base(logger)
        {

        }

        public IQuerySelectStatement<TModel> From()
        {
            IQuerySelectStatement<TModel> querySelectStatement = new QuerySelectStatement<TModel>();
            return querySelectStatement;
        }

        public IQueryInsertStatement<TModel> Insert()
        {
            IQueryInsertStatement<TModel> queryInsertStatement =  new QueryInsertStatement<TModel>();
            return queryInsertStatement;
        }

        public IQueryUpdateStatement<TModel> Update()
        {
            IQueryUpdateStatement<TModel> queryUpdateStatement = new QueryUpdateStatement<TModel>();
            return queryUpdateStatement;
        }

        public IQueryDeleteStatement<TModel> Delete()
        {
            IQueryDeleteStatement<TModel> queryDeleteStatement = new QueryDeleteStatement<TModel>();
            return queryDeleteStatement;
        }
    }

    internal sealed class QueryBuilder : LoggedObject, IQueryBuilder
    {
        public QueryBuilder(ILogger logger) : base(logger)
        {

        }

        public IQuerySelectStatement From(string tableName)
        {
            return new QuerySelectStatement(tableName);
        }

        public IQuerySelectStatement<TModel> From<TModel>()
        {
            return new QuerySelectStatement<TModel>();
        }

        public IQueryInsertStatement Insert(string tableName)
        {
            return new QueryInsertStatement(tableName);
        }

        public IQueryUpdateStatement Update(string tableName)
        {
            return new QueryUpdateStatement(tableName);
        }

        public IQueryDeleteStatement Delete(string tableName)
        {
            return new QueryDeleteStatement(tableName);
        }
    }
}