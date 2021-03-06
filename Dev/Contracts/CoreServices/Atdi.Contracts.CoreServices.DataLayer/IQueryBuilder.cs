﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IQueryBuilder
    {
        IQuerySelectStatement From(string tableName);

        IQuerySelectStatement<TModel> From<TModel>();

        IQueryInsertStatement Insert(string tableName);

        IQueryUpdateStatement Update(string tableName);

        IQueryDeleteStatement Delete(string tableName);
    }

    public interface IQueryBuilder<TModel>
    {
        IQuerySelectStatement<TModel> From();

        IQueryInsertStatement<TModel> Insert();

        IQueryUpdateStatement<TModel> Update();

        IQueryDeleteStatement<TModel> Delete();
    }
}
