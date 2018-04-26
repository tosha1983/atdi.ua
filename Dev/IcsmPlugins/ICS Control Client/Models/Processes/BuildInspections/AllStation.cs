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
    public class AllStation : IRepositoryEntity, IRepositoryReadedEntity
    {
        public int Id;
        public string Standart;
        public string TableName;
        public int TableId;
        public double Latitude;
        public double Longitude;

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.AllStations.Fields.Id,
                MD.AllStations.Fields.Latitude,
                MD.AllStations.Fields.Longitude,
                MD.AllStations.Fields.Standart,
                MD.AllStations.Fields.TableName,
                MD.AllStations.Fields.TableId
            };
        }

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.AllStations.Fields.Id;
        }

        string IRepositoryEntity.GetTableName()
        {
            return MD.AllStations.TableName;
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.AllStations.Fields.Id);
            this.Latitude = source.GetD(MD.AllStations.Fields.Latitude);
            this.Longitude = source.GetD(MD.AllStations.Fields.Longitude);
            this.TableId  = source.GetI(MD.AllStations.Fields.TableId);
            this.TableName = source.GetS(MD.AllStations.Fields.TableName);
            this.Standart = source.GetS(MD.AllStations.Fields.Standart);
        }
    }
}
