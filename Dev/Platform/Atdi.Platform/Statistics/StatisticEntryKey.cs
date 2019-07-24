using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    class StatisticEntryKey<T> : IStatisticEntryKey<T>
    {
        public StatisticEntryKey(string name)
        {
            this.Name = name;
            this.Type = typeof(T);
        }

        public string Name { get; }

        public Type Type { get; }

        public override bool Equals(object obj)
        {
            var key = obj as StatisticEntryKey<T>;
            return key != null &&
                   Name == key.Name;
        }

        public override int GetHashCode()
        {
            return 990326508 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override string ToString()
        {
            return $"Name = '{this.Name}' Type = '{this.Type.Name}'";
        }
    }
}
