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
    public struct FieldStrengthCalcData
	{
		public PropagationModel PropagationModel;
        
		public StationAntenna Antenna;

		public AtdiCoordinate PointCoordinate;

		public AtdiCoordinate TargetCoordinate;

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
		public double PointAltitude_m;

        /// <summary>
        /// Высота второй точки (Точька, Абонент, Кореспондирующая станция)
        /// </summary>
        public double TargetAltitude_m;
    }
}
