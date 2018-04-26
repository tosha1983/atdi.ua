using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.Models.BuildInspections
{
    public class NfraApplication : IRepositoryEntity, IRepositoryReadedEntity
    {
        public int Id;
        public string ObjTable;
        public int ObjId1;
        public int ObjId2;
        public int ObjId3;
        public int ObjId4;
        public int ObjId5;
        public int ObjId6;
        public DateTime DozvDateFrom;
        public DateTime DozvDateTo;
        public DateTime DozvDateCancel;

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.NfraApplication.Fields.Id,
                MD.NfraApplication.Fields.ObjId1,
                MD.NfraApplication.Fields.ObjId2,
                MD.NfraApplication.Fields.ObjId3,
                MD.NfraApplication.Fields.ObjId4,
                MD.NfraApplication.Fields.ObjId5,
                MD.NfraApplication.Fields.ObjId6,
                MD.NfraApplication.Fields.ObjTable,
                MD.NfraApplication.Fields.DozvDateFrom,
                MD.NfraApplication.Fields.DozvDateTo,
                MD.NfraApplication.Fields.DozvDateCancel
            };
        }

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.NfraApplication.Fields.Id;
        }

        string IRepositoryEntity.GetTableName()
        {
            return MD.NfraApplication.TableName;
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.NfraApplication.Fields.Id);
            this.DozvDateCancel = source.GetT(MD.NfraApplication.Fields.DozvDateCancel);
            this.DozvDateFrom = source.GetT(MD.NfraApplication.Fields.DozvDateFrom);
            this.DozvDateTo = source.GetT(MD.NfraApplication.Fields.DozvDateTo);
            this.ObjId1 = source.GetI(MD.NfraApplication.Fields.ObjId1);
            this.ObjId2 = source.GetI(MD.NfraApplication.Fields.ObjId2);
            this.ObjId3 = source.GetI(MD.NfraApplication.Fields.ObjId3);
            this.ObjId4 = source.GetI(MD.NfraApplication.Fields.ObjId4);
            this.ObjId5 = source.GetI(MD.NfraApplication.Fields.ObjId5);
            this.ObjId6 = source.GetI(MD.NfraApplication.Fields.ObjId6);
            this.ObjTable = source.GetS(MD.NfraApplication.Fields.ObjTable);

        }
    }
}
