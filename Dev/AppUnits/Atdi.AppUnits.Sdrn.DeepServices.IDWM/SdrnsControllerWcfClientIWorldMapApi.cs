using System;
using System.Collections.Generic;
using Atdi.Contracts.WcfServices.Sdrn.DeepServices.IDWM;
using Atdi.WcfServices;

namespace Atdi.AppUnits.Sdrn.DeepServices.IDWM
{
    public class SdrnsControllerWcfClientIWorldMapApi : WcfServiceClientBase<IWorldMapApi, SdrnsControllerWcfClientIWorldMapApi>
    {
        public SdrnsControllerWcfClientIWorldMapApi() : base("WorldMapApi") { }

        public static string GetADMByPoint(Point point)
        {
            return Execute(contract => contract.GetADMByPoint(point));
        }

        public static AdministrationsResult[] GetADMByPointAndDistance(PointAndDistance  pointAndDistance)
        {
            return Execute(contract => contract.GetADMByPointAndDistance(pointAndDistance));
        }

        public static Point GetNearestPointByADM(PointByADM pointByADM)
        {
            return Execute(contract => contract.GetNearestPointByADM(pointByADM));
        }
    }
}
