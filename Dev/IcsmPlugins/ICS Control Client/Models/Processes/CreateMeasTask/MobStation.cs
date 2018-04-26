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
    public class MobStation : IRepositoryEntity, IRepositoryReadedEntity
    {
        public int Id;
        public string Name;
        public string Standart;
        public double Agl;
        public double Power;
        public double Azimut;
        public double BW;
        public string DesignEmission;
        public string CustTxt13;

        public Owner OwnerRef;
        public Position PositionRef;

        public MobStationFrequencies[] Frequencies;

        public class Owner
        {
            public int Id;
            public string Name;
            public string RegistNum;
            public string PostCode;
            public string Code;
            public string Address;
        }
        public class Position
        {
            public double Longitude;
            public double Latitude;
            public string Address;
            public string SubProvince;
        }

        string[] IRepositoryReadedEntity.GetFieldNames()
        {
            return new string[]
            {
                MD.MobStations.Fields.Id,
                MD.MobStations.Fields.Agl,
                MD.MobStations.Fields.Azimut,
                MD.MobStations.Fields.BW,
                MD.MobStations.Fields.DesignEmission,
                MD.MobStations.Fields.Name,
                MD.MobStations.Fields.CustTxt13,
                MD.MobStations.Fields.Power,
                MD.MobStations.Fields.Standart,
                MD.MobStations.Fields.Owner.Id,
                MD.MobStations.Fields.Owner.Address,
                MD.MobStations.Fields.Owner.Code,
                MD.MobStations.Fields.Owner.Name,
                MD.MobStations.Fields.Owner.PostCode,
                MD.MobStations.Fields.Owner.RegistNum,
                MD.MobStations.Fields.Position.Address,
                MD.MobStations.Fields.Position.Latitude,
                MD.MobStations.Fields.Position.Longitude,
                MD.MobStations.Fields.Position.SubProvince
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
            this.Agl = source.GetD(MD.MobStations.Fields.Agl);
            this.Azimut = source.GetD(MD.MobStations.Fields.Azimut);
            this.BW  = source.GetD(MD.MobStations.Fields.BW);
            this.DesignEmission = source.GetS(MD.MobStations.Fields.DesignEmission);
            this.Name = source.GetS(MD.MobStations.Fields.Name);
            this.CustTxt13 = source.GetS(MD.MobStations.Fields.CustTxt13);
            this.Power = source.GetD(MD.MobStations.Fields.Power);
            this.Standart = source.GetS(MD.MobStations.Fields.Standart);

            this.OwnerRef = new Owner
            {
                Id =  source.GetI(MD.MobStations.Fields.Owner.Id),
                Address = source.GetS(MD.MobStations.Fields.Owner.Address),
                Code = source.GetS(MD.MobStations.Fields.Owner.Code),
                Name = source.GetS(MD.MobStations.Fields.Owner.Name),
                PostCode = source.GetS(MD.MobStations.Fields.Owner.PostCode),
                RegistNum = source.GetS(MD.MobStations.Fields.Owner.RegistNum ),
            };

            this.PositionRef = new Position
            {
                Address = source.GetS(MD.MobStations.Fields.Position.Address),
                Latitude = source.GetD(MD.MobStations.Fields.Position.Latitude),
                Longitude = source.GetD(MD.MobStations.Fields.Position.Longitude),
                SubProvince  = source.GetS(MD.MobStations.Fields.Position.SubProvince),
            };

        }
    }
}
