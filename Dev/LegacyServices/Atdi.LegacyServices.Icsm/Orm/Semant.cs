using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class Semant
    {
        public string Name;
        public SemantType Type;
        public double Div;
        public double Min;
        public double Max;
        public string Sym;

        public Semant(string name, SemantType type)
        {
            this.Name = name;
            this.Type = type;
            this.Div = 0.0;
            this.Min = (this.Max = 1E-99);
            this.Sym = null;
        }
    }
}
