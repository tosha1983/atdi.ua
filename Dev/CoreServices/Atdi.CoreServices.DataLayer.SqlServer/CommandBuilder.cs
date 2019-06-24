using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.SqlServer
{
    class CommandBuilder
    {
        private readonly StringBuilder _sql;
        private int _iteration;
        private int _counter;
        private readonly Dictionary<string, EngineCommandParameter> _parameters;

        public CommandBuilder()
        {
            this._sql = new StringBuilder();
            this._parameters = new Dictionary<string, EngineCommandParameter>();
            this._iteration = 0;
            this._counter = 0;
        }

        public void StartIteration()
        {
            ++this._iteration;
            this._counter = 0;
            _sql.AppendLine($"/* Iterration: #{this._iteration} */");
            _sql.AppendLine();
        }

        public void ExpresaionAlias(int index, string alias)
        {
            _sql.AppendLine($"/* Expression: index = #{index}, alias = '{alias}' */");
        }

        public void Insert(string schema, string source, string[] fields, EngineCommandParameter[] values, EngineCommandParameter idenityParameter = null)
        {
            var fieldsClause = string.Join(", ", fields.Select(f => $"[{f}]").ToArray());
            _sql.AppendLine($"INSERT INTO [{schema}].[{source}]({fieldsClause})");

            for (int i = 0; i < values.Length; i++)
            {
                var parameter = values[i];
                this.AppendParameter(parameter);
            }
            var valuesClause = string.Join(", ", values.Select(v => $"@{v.Name}").ToArray());

            _sql.AppendLine($"VALUES({valuesClause});");

            if (idenityParameter != null)
            {
                this.SelectIdentity(idenityParameter);
            }
        }

        public void SelectIdentity(EngineCommandParameter idenityParameter)
        {
            this.AppendParameter(idenityParameter);
            _sql.AppendLine($"SET @{idenityParameter.Name} = @@IDENTITY;");
        }

        private void AppendParameter(EngineCommandParameter parameter)
        {
            if (parameter.Name == null || !_parameters.ContainsKey(parameter.Name))
            {
                ++this._counter;
                parameter.Name = $"P_I{this._iteration}_{this._counter}";
                _parameters.Add(parameter.Name, parameter);
            }
        }

        public EngineCommand GetCommand()
        {
            var command = new EngineCommand(this._parameters)
            {
                Text = _sql.ToString()
            };

            return command;
        }
    }
}
