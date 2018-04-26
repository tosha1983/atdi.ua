using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.Models.CreateMeasTask
{
    public class Inspection : IRepositoryEntity, IRepositoryReadedEntity
    {
        public int Id;
        public string Status;
        public int StationTableId;
        public string StationTableName;
        public int TourId;

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.Inspection.Fields.Id,
                MD.Inspection.Fields.StationTableId,
                MD.Inspection.Fields.StationTableName,
                MD.Inspection.Fields.Status,
                MD.Inspection.Fields.TourId
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
        }
    }
}
