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
        string EncodeFieldName(string name);

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

        string ColumnExpression(string expression, string alias);

        string SortedColumn(string expression, SortDirection direction);

        string FromExpression(string expression, string alias);

        string SelectExpression(string[] selectColumns, string fromExpression, string whereExpression = null, string[] orderByColumns = null, DataLimit limit = null, string[] groupByColumns = null);
    }
}
