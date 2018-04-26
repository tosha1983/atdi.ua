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
    public class MobStationFrequencies : IRepositoryEntity, IRepositoryReadedEntity
    {
        public int Id;
        public int StationId;

        public ChannelTx ChannelTxRef;

        public class ChannelTx
        {
            public int PlanId;
            public int Channel;
            public decimal Freq;
        }

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.MobStationFrequencies.Fields.Id,
                MD.MobStationFrequencies.Fields.StationId,
                MD.MobStationFrequencies.Fields.ChannelTx.Channel,
                MD.MobStationFrequencies.Fields.ChannelTx.Freq,
                MD.MobStationFrequencies.Fields.ChannelTx.PlanId
            };
        }

        string IRepositoryEntity.GetIdFieldName()
        {
            return MD.MobStationFrequencies.Fields.Id;
        }

        string IRepositoryEntity.GetTableName()
        {
            return MD.MobStationFrequencies.TableName;
        }

        void IRepositoryReadedEntity.LoadFromRecordset(IMRecordset source)
        {
            this.Id = source.GetI(MD.MobStationFrequencies.Fields.Id);
            this.StationId = source.GetI(MD.MobStationFrequencies.Fields.StationId);

            this.ChannelTxRef = new ChannelTx
            {
                Channel = source.GetS(MD.MobStationFrequencies.Fields.ChannelTx.Channel).TryToInt(),
                PlanId = source.GetI(MD.MobStationFrequencies.Fields.ChannelTx.PlanId),
                Freq = Convert.ToDecimal(source.GetD(MD.MobStationFrequencies.Fields.ChannelTx.Freq))
            };
        }

        
    }
}
