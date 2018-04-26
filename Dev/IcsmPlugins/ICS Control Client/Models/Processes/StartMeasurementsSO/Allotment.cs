using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.Models.StartMeasurementsSO
{
    public class Allotment : IRepositoryEntity, IRepositoryReadedEntity, IRepositoryUpdatedEntity
    {
        public int Id;
        public string Status;
        public string TableName;
        public string UserType;
        public DateTime CustDate1;
        public DateTime CustDate2;
        public double CustNum2;
        public double CustNum3;
        public string CustText1;
        public string CustText2;
        public string CustText3;
        public string CustText4;
        public int MeasTaskId;

        public Plan PlanRef;

        public class Plan
        {
            public int Id;
            public double Bandwidth;
            public double ChannelSep;
        }

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.Allotments.Fields.Id,
                MD.Allotments.Fields.Status,
                MD.Allotments.Fields.TableName,
                MD.Allotments.Fields.UserType,
                MD.Allotments.Fields.CustDate1,
                MD.Allotments.Fields.CustDate2,
                MD.Allotments.Fields.CustNum2,
                MD.Allotments.Fields.CustNum3,
                MD.Allotments.Fields.CustText1,
                MD.Allotments.Fields.CustText2,
                MD.Allotments.Fields.CustText3,
                MD.Allotments.Fields.CustText4,
                MD.Allotments.Fields.Plan.Id,
                MD.Allotments.Fields.Plan.Bandwidth,
                MD.Allotments.Fields.Plan.ChannelSep,
                MD.Allotments.Fields.MeasTaskId,
            };
        }

        public bool IsNewState => MD.Allotments.Statuses.New.Equals(this.Status, StringComparison.OrdinalIgnoreCase);

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.Allotments.Fields.Id;
        }

        string IRepositoryEntity.GetTableName()
        {
            return MD.Allotments.TableName;
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.Allotments.Fields.Id);
            this.TableName = source.GetS(MD.Allotments.Fields.TableName);
            this.Status = source.GetS(MD.Allotments.Fields.Status);
            this.UserType = source.GetS(MD.Allotments.Fields.UserType);
            this.CustDate1 = source.GetT(MD.Allotments.Fields.CustDate1);
            this.CustDate2 = source.GetT(MD.Allotments.Fields.CustDate2);
            this.CustNum2 = source.GetD(MD.Allotments.Fields.CustNum2);
            this.CustNum3 = source.GetD(MD.Allotments.Fields.CustNum3);
            this.CustText1 = source.GetS(MD.Allotments.Fields.CustText1);
            this.CustText2 = source.GetS(MD.Allotments.Fields.CustText2);
            this.CustText3 = source.GetS(MD.Allotments.Fields.CustText3);
            this.CustText4 = source.GetS(MD.Allotments.Fields.CustText4);
            this.MeasTaskId = source.GetI(MD.Allotments.Fields.MeasTaskId);

            this.PlanRef = new Plan
            {
                Id = source.GetI(MD.Allotments.Fields.Plan.Id),
                ChannelSep = source.GetD(MD.Allotments.Fields.Plan.ChannelSep),
                Bandwidth = source.GetD(MD.Allotments.Fields.Plan.Bandwidth)
            };
        }

        int IRepositoryUpdatedEntity.GetId()
        {
            return this.Id;
        }

        string[] IRepositoryUpdatedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.Allotments.Fields.Id,
                MD.Allotments.Fields.Status,
                MD.Allotments.Fields.MeasTaskId
            };
        }

        void IRepositoryUpdatedEntity.SaveToRecordset(IMRecordset source)
        {
            source.Put(MD.Allotments.Fields.Status, this.Status);
            source.Put(MD.Allotments.Fields.MeasTaskId, this.MeasTaskId);
        }
    }
}
