using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;

namespace Atdi.Icsm.Plugins.Core
{
    public static class IMRecordsetExtentions
    {
        public static void Select(this IMRecordset recordset, params string[] fields)
        {
            recordset.Select(string.Join(",", fields));
        }

        public static void AddNew(this IMRecordset recordset, string tableNmae, string fieldName)
        {
            recordset.AddNew();
            recordset.PutNextId(tableNmae, fieldName);
        }

        public static void PutNextId(this IMRecordset recordset, string tableNmae, string fieldName)
        {
            recordset.Put(fieldName, IM.AllocID(tableNmae, 1, -1));
        }

        public static OpenedRecordsetScope OpenForAdd(this IMRecordset recordset, params string[] fields)
        {
            recordset.Select(string.Join(",", fields));
            recordset.SetWhere(fields[0], IMRecordset.Operation.Eq, -1);
            return recordset.OpenWithScope();
        }

        public static OpenedRecordsetScope OpenWithScope(this IMRecordset recordset)
        {
            return new OpenedRecordsetScope(recordset);
        }
    }
}
