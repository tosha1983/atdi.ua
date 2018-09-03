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
    }
}
