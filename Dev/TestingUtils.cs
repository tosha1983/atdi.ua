using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Atdi.UnitTest.CoreServices.EntityOrm
{
    public class TestingUtils
    {
        public static bool JsonCompare(object obj, object another)
        {
            if (ReferenceEquals(obj, another)) return true;
            if ((obj == null) || (another == null)) return false;
            if (obj.GetType() != another.GetType()) return false;

            var objJson = JsonConvert.SerializeObject(obj);
            var anotherJson = JsonConvert.SerializeObject(another);

            return objJson == anotherJson;
        }
    }
}
