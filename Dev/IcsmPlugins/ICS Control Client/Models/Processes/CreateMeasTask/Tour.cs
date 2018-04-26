using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.Models.CreateMeasTask
{
    public class Tour : IRepositoryEntity, IRepositoryReadedEntity, IRepositoryUpdatedEntity
    {
        public int Id;
        public string Status;
        public DateTime StartDate;
        public DateTime StopDate;
        public int MeasTaskId;
        public string MeasTaskName;
        public string SensorName ;
        public string SensorEquipTechId;

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
                MD.Tours.Fields.Status,
                MD.Tours.Fields.StartDate,
                MD.Tours.Fields.StopDate,
                MD.Tours.Fields.MeasTaskId,
                MD.Tours.Fields.MeasTaskName,
                MD.Tours.Fields.SensorName,
                MD.Tours.Fields.SensorEquipTechId
            };
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.Tours.Fields.Id);
            this.Status = source.GetS(MD.Tours.Fields.Status);
            this.StartDate = source.GetT(MD.Tours.Fields.StartDate);
            this.StopDate = source.GetT(MD.Tours.Fields.StopDate);
            this.MeasTaskId = source.GetI(MD.Tours.Fields.MeasTaskId);
            this.MeasTaskName = source.GetS(MD.Tours.Fields.MeasTaskName);
            this.SensorName = source.GetS(MD.Tours.Fields.SensorName);
            this.SensorEquipTechId = source.GetS(MD.Tours.Fields.SensorEquipTechId);
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
                MD.Tours.Fields.Status,
                MD.Tours.Fields.MeasTaskId
            };
        }

        void IRepositoryUpdatedEntity.SaveToRecordset(IMRecordset source)
        {
            source.Put(MD.Tours.Fields.Status, this.Status);
            source.Put(MD.Tours.Fields.MeasTaskId, this.MeasTaskId);
        }
    }
}
