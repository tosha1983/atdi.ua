using Atdi.Platform.ConfigElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Configurator
{
    class ConfigParameters : IConfigParameters
    {
        class ConfigParameter : IConfigParameter
        {
            public ConfigParameter(KeyValuePair<string, string> source)
            {
                this.Name = source.Key;
                this.Value = source.Value;
            }

            public ConfigParameter(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }

            public string Name { get; private set; }

            public object Value { get; private set; }
        }

        private Dictionary<string, string> _storage;

        public ConfigParameters(ParametersConfigElement source)
        {
            this._storage = new Dictionary<string, string>();

            if (source != null && source.Count > 0)
            {
                foreach (ParameterConfigElement parameter in source)
                {
                    this._storage[parameter.NameProperty] = parameter.ValueProperty;
                }
            }
        }

        public object this[string name] { get => this._storage[name]; }

        public IConfigParameter this[int index]
        {
            get
            {
                int count = 0;
                foreach (var item in _storage)
                {
                    if (count == index)
                        return new ConfigParameter(item);
                    ++count;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public int Count => this._storage.Count;

        public IConfigParameter GetByName(string name)
        {
            return new ConfigParameter(name, _storage[name]);
        }

        public bool Has(string name)
        {
            return _storage.ContainsKey(name);
        }
    }
}
