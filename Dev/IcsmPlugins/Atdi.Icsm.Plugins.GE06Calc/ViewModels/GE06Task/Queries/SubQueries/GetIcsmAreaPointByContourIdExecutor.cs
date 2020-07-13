using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.Platform.Cqrs;
using ICSM;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries
{
    public class GetIcsmAreaPointByContourIdExecutor : IReadQueryExecutor<GetIcsmAreaPointByContourId, AreaPoint[]>
    {
        private readonly IObjectReader _objectReader;
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly DataMaper _mapper;

        public GetIcsmAreaPointByContourIdExecutor(IObjectReader objectReader, AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _objectReader = objectReader;
            _config = config;
            _dataLayer = dataLayer;
            _mapper = new DataMaper(_objectReader);
        }
        public AreaPoint[] Read(GetIcsmAreaPointByContourId criterion)
        {
            var conturs = new List<AreaPoint>();
            string contursString = "";
            IMRecordset rs = new IMRecordset("ITU_CONTOUR", IMRecordset.Mode.ReadOnly);
            rs.Select("TPR_XXX");
            rs.SetWhere("NUM", IMRecordset.Operation.Eq, criterion.ContourId);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                contursString = rs.GetS("TPR_XXX");
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            if (!string.IsNullOrEmpty(contursString))
            {
                string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var areaPoints = split_string(contursString, 15);

                foreach (var item in areaPoints)
                {
                    double.TryParse(item.Substring(0, 8).Replace("E", sep).Replace("W", sep), out double areaLon);
                    double.TryParse(item.Substring(8, 7).Replace("N", sep).Replace("S", sep), out double areaLat);
                    if (item.Substring(0, 8).Contains("W"))
                        areaLon = -areaLon;
                    if (item.Substring(8, 7).Contains("S"))
                        areaLat = -areaLat;

                    if (areaLon > -180 && areaLon < 180 && areaLat > -90 && areaLat < 90)
                    {
                        conturs.Add(new AreaPoint() { Lon_DEC = IMPosition.Dms2Dec(areaLon), Lat_DEC = IMPosition.Dms2Dec(areaLat) });
                    }
                }
            }

            return conturs.ToArray();
        }
        private string[] split_string(string str, int interval)
        {
            List<string> substrs = new List<string>();
            int i;
            for (i = 0; i < str.Length - interval; i += interval)
            {
                substrs.Add(str.Substring(i, interval));
            }
            substrs.Add(str.Substring(i));
            return substrs.ToArray();
        }
    }
}
