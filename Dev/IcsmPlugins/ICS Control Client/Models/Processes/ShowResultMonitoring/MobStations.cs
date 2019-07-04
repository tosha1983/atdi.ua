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
    public class MobStations : IRepositoryEntity, IRepositoryReadedEntity
    {
        public long Id;
        public string Name;


        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.MobStations.Fields.Id,
                MD.MobStations.Fields.Name
            };
        }

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.MobStations.Fields.Id;
        }

        string IRepositoryEntity.GetTableName()
        {
            return MD.MobStations.TableName;
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.MobStations.Fields.Id);
            this.Name = source.GetS(MD.MobStations.Fields.Name);
        }
    }
}
