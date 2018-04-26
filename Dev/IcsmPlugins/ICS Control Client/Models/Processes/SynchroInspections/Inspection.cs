using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.Models.SynchroInspections
{
    public class Inspection : IRepositoryEntity, IRepositoryReadedEntity, IRepositoryUpdatedEntity
    {
        public int Id;
        public string Status;
        public int StationTableId;
        public string StationTableName;
        public int TourId;

        public Station StationRef;

        public class Station
        {
            public string Name;
        }

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.Inspection.Fields.Id,
                MD.Inspection.Fields.StationTableId,
                MD.Inspection.Fields.StationTableName,
                MD.Inspection.Fields.Status,
                MD.Inspection.Fields.TourId,
                MD.Inspection.Fields.Station.Name
            };
        }

        

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.Inspection.Fields.Id;
        }

        string IRepositoryEntity.GetTableName()
        {
            return MD.Inspection.TableName;
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.Inspection.Fields.Id);
            this.Status = source.GetS(MD.Inspection.Fields.Status);
            this.StationTableId = source.GetI(MD.Inspection.Fields.StationTableId);
            this.StationTableName = source.GetS(MD.Inspection.Fields.StationTableName);
            this.TourId = source.GetI(MD.Inspection.Fields.TourId);

            this.StationRef = new Station
            {
                Name = source.GetS(MD.Inspection.Fields.Station.Name)
            };
        }

        string[] IRepositoryUpdatedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.Inspection.Fields.Id,
                MD.Inspection.Fields.Status
            };
        }

        int IRepositoryUpdatedEntity.GetId()
        {
            return this.Id;
        }

        void IRepositoryUpdatedEntity.SaveToRecordset(IMRecordset source)
        {
            source.Put(MD.Inspection.Fields.Status, this.Status);
        }
    }
}
