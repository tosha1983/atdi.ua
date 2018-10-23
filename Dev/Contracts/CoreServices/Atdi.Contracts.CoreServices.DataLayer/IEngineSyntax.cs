using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IEngineSyntax
    {
        string EncodeTableName(string name);

        string EncodeTableName(string schema, string name);

        string SourceExpression(string sourceExpression, string alias);

        string EncodeFieldName(string name);

        string EncodeFieldNameExpression(string name);

        string EncodeParameterName(string name);

        string EncodeFieldName(string source, string name);

        string EncodeValue(string value);

        string EncodeValue(int value);

        string EncodeValue(bool value);

        string EncodeValue(DateTime value);

        string EncodeValue(float value);

        string EncodeValue(double value);

        string EncodeValue(decimal value);

        IConstraintEngineSyntax Constraint { get; }

        int MaxLengthAlias { get; }

        string ColumnExpression(string expression, string alias);

        string SortedColumn(string expression, SortDirection direction);

        string SetColumnValueExpression(string columnExpression, string valueExpression);

        string FromExpression(string expression, string alias);

        string SelectExpression(string[] selectColumns, string fromExpression, string whereExpression = null, string[] orderByColumns = null, DataLimit limit = null, string[] groupByColumns = null);

        string InsertExpression(string sourceExpression, string columnsExpression, string valuesExpression);

        string UpdateExpression(string sourceExpression, string valuesExpression, string fromExpression = null, string whereExpression = null);

        string DeleteExpression(string sourceExpression, string fromExpression = null, string whereExpression = null);
    }
}
