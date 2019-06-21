using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public class EngineCommandParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DataType DataType { get; set; }

        public EngineParameterDirection Direction { get; set; }

        public override string ToString()
        {
            var showValue = Value;
            var type = Value?.GetType();
            if (type == typeof(string) )
            {
                if (showValue != null)
                {
                    var str = (string)showValue;
                    if (str.Length > 50)
                    {
                        showValue = str.Substring(0, 50);
                    }
                }
                
            }
            else if(type == typeof(char[]))
            {
                if (showValue != null)
                {
                    var str = (char[])showValue;
                    if (str.Length > 50)
                    {
                        showValue = "Big char array: " + str.Length;
                    }
                }

            }
            return $"Name = '{this.Name}', DataType = '{this.DataType}', VarType = '{Value?.GetType().FullName}', Value = '{showValue}'";
        }
    }

}
