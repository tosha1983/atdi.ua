using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService
{
    [Serializable]
    public struct CalcLossArgs
    {
        public PropagationModel Model;
        // profiles
        public short[] ReliefProfile;
        public byte[] ClutterProfile;
        public byte[] BuildingProfile;
        public short[] HeightProfile;
        public short[] BuildHeightProfile;
        public byte[] ForestClutterProfile;
        public int ProfileLength;
        public int ReliefStartIndex;
        public int ClutterStartIndex;
        public int BuildingStartIndex;
        public int HeightStartIndex;
        public double Freq_Mhz;
        public bool HybridDiffraction;
        public CluttersDesc CluttersDesc;

        // Дополнительные параметры определяющие геометрию трассы
        /// <summary>
        /// Длинна пролета
        /// </summary>
        public double D_km;
        /// <summary>
        /// Высота первой точки (Базовой станции) 
        /// </summary>
        public double Ha_m;
        /// <summary>
        /// Высота второй точки (Точька, Абонент, Кореспондирующая станция)
        /// </summary>
        public double Hb_m;
    }
}
