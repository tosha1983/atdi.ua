using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.WpfControls.EntityOrm.Controls;

namespace Atdi.Icsm.Plugins.GE06Calc.Environment
{
    public static class PluginHelper
    {
        public static OrmEnumBoxData[] EnumToOrmEnumBoxData<T>() where T : Enum
        {
            var enumData = new List<OrmEnumBoxData>();

            foreach (var item in Enum.GetValues(typeof(T)))
                enumData.Add(new OrmEnumBoxData() { Id = (int)item, Name = item.ToString(), ViewName = item.ToString() });

            return enumData.ToArray();
        }
    }
}
