using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.Models.ShowResultMonitoring
{
    public class MobStations2 : IRepositoryEntity, IRepositoryReadedEntity
    {
        public long Id;
        public string Name;


        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.MobStations2.Fields.Id,
                MD.MobStations2.Fields.Name
            };
        }

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.MobStations2.Fields.Id;
        }

        string IRepositoryEntity.GetTableName()
        {
            return MD.MobStations2.TableName;
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.MobStations2.Fields.Id);
            this.Name = source.GetS(MD.MobStations2.Fields.Name);
        }
    }
}
