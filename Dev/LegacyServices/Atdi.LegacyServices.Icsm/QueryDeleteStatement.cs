using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QueryDeleteStatement : IQueryDeleteStatement
    {
        private readonly List<Condition> _conditions;
        private readonly string _tableName;

        public QueryDeleteStatement(string tableName)
        {
            this._tableName = tableName;
            this._conditions = new List<Condition>();
        }
        public List<Condition> Conditions => this._conditions;
        public string TableName => this._tableName;

        public IQueryDeleteStatement Where(Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            this._conditions.Add(condition);
            return this;
        }
    }
}
