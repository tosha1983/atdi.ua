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
    public class Inspection : IRepositoryEntity, IRepositoryCreatedEntity
    {
        public int Id;
        public string Status;
        public int StationTableId;
        public string StationTableName;
        public int TourId;
        public string Type;
        public DateTime DoItAfter;
        public DateTime DoItBefore;

        string[] IRepositoryCreatedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.Inspection.Fields.Id,
                MD.Inspection.Fields.DoItAfter,
                MD.Inspection.Fields.DoItBefore,
                MD.Inspection.Fields.StationTableId,
                MD.Inspection.Fields.StationTableName,
                MD.Inspection.Fields.Status,
                MD.Inspection.Fields.TourId,
                MD.Inspection.Fields.Type
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



        void IRepositoryCreatedEntity.SaveToRecordset(IMRecordset source)
        {
            source.Put(MD.Inspection.Fields.Id, this.Id);
            source.Put(MD.Inspection.Fields.Type, this.Type);
            source.Put(MD.Inspection.Fields.Status, this.Status);
            source.Put(MD.Inspection.Fields.DoItAfter, this.DoItAfter);
            source.Put(MD.Inspection.Fields.DoItBefore, this.DoItBefore);
            source.Put(MD.Inspection.Fields.TourId, this.TourId);
            source.Put(MD.Inspection.Fields.StationTableName, this.StationTableName);
            source.Put(MD.Inspection.Fields.StationTableId, this.StationTableId);
        }

        void IRepositoryCreatedEntity.SetId(int id)
        {
            this.Id = id;
        }
    }
}
