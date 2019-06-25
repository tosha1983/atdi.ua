using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public sealed class EngineCommand
    {
        private readonly IDictionary<string, EngineCommandParameter> _parameters;

        public EngineCommand()
        {
            this._parameters = new Dictionary<string, EngineCommandParameter>();
        }

        public EngineCommand(IDictionary<string, EngineCommandParameter> parameters)
        {
            this._parameters = parameters;
        }

        public string Text { get; set; }


        public IDictionary<string, EngineCommandParameter> Parameters => _parameters;

        public override string ToString()
        {
            if (this.Parameters.Count == 0)
            {
                return $"Command: {this.Text}";
            }

            return $"Command: {this.Text}" + Environment.NewLine + $"Parameters: count = {this.Parameters.Count}";
        }

        public void AddParameter(string name, DataType dataType, object value, EngineParameterDirection direction = EngineParameterDirection.Input)
        {
            var parameter = new EngineCommandParameter
            {
                Name = name,
                DataType = dataType,
                Direction = direction
            };
            parameter.SetValue(value);
            this.Parameters.Add(parameter.Name, parameter);
        }
    }
}
