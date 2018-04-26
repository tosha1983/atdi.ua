using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.Models.BuildInspections
{
    public class Tour : IRepositoryEntity, IRepositoryReadedEntity, IRepositoryUpdatedEntity
    {
        public int Id;
        public string LocationList;
        public string RadioTechList;
        public string Status;
        public DateTime StartDate;
        public DateTime StopDate;

        public bool IsNewState => MD.Tours.Statuses.New.Equals(this.Status, StringComparison.OrdinalIgnoreCase);


        string IRepositoryEntity.GetTableName()
        {
            return MD.Tours.TableName;
        }

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.Tours.Fields.Id;
        }

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.Tours.Fields.Id,
                MD.Tours.Fields.LocationList,
                MD.Tours.Fields.RadioTechList,
                MD.Tours.Fields.Status,
                MD.Tours.Fields.StartDate,
                MD.Tours.Fields.StopDate
            };
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.Tours.Fields.Id);
            this.Status = source.GetS(MD.Tours.Fields.Status);
            this.LocationList = source.GetS(MD.Tours.Fields.LocationList);
            this.RadioTechList = source.GetS(MD.Tours.Fields.RadioTechList);
            this.StartDate = source.GetT(MD.Tours.Fields.StartDate);
            this.StopDate = source.GetT(MD.Tours.Fields.StopDate);
        }

        int IRepositoryUpdatedEntity.GetId()
        {
            return this.Id;
        }

        string[] IRepositoryUpdatedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.Tours.Fields.Id,
                MD.Tours.Fields.Status
            };
        }

        void IRepositoryUpdatedEntity.SaveToRecordset(IMRecordset source)
        {
            source.Put(MD.Tours.Fields.Status, this.Status);
        }
    }
}
