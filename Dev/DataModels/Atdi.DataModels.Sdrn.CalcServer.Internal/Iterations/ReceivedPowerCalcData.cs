using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    [Serializable]
    public struct ReceivedPowerCalcData
    {
        public PropagationModel PropagationModel;

        public StationAntenna TxAntenna;

        public AtdiCoordinate TxCoordinate;

        public double TxFreq_Mhz;

        public StationAntenna RxAntenna;

        public AtdiCoordinate RxCoordinate;

        public double RxFeederLoss_dB;

        public AtdiMapArea MapArea;

        public short[] ReliefContent;

        public byte[] ClutterContent;

        public byte[] BuildingContent;

        public StationTransmitter Transmitter;

        public CluttersDesc CluttersDesc;

        //public double TransmitterFreq_Mhz;

        //public PolarizationType TranmitterPolarization;

        //public float TransmitterMaxPow_dBm;

        //public float TransmitterLoss_dB;

        /// <summary>
        /// Высота первой точки (Базовой станции) 
        /// </summary>
        public double TxAltitude_m;

        /// <summary>
        /// Высота второй точки (Точька, Абонент, Кореспондирующая станция)
        /// </summary>
        public double RxAltitude_m;
    }
}
