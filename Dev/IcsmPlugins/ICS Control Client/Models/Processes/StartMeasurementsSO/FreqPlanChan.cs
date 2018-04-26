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
    public class FreqPlanChan : IRepositoryEntity, IRepositoryReadedEntity
    {
        public int PlanId;
        public double Freq;

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.FreqPlanChan.Fields.PlanId,
                MD.FreqPlanChan.Fields.Freq
            };
        }

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.FreqPlanChan.Fields.PlanId;
        }

        string IRepositoryEntity.GetTableName()
        {
            return MD.FreqPlanChan.TableName;
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.PlanId = source.GetI(MD.FreqPlanChan.Fields.PlanId);
            this.Freq = source.GetD(MD.FreqPlanChan.Fields.Freq);
        }
    }
}
