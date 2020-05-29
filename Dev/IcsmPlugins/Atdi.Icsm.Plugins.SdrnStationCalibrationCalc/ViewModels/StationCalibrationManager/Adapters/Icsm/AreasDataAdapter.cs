using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using System.Globalization;
using OrmCs;
using ICSM;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters
{
    public class AreasDataAdapter : DataAdapter<YArea, AreaModel, AreasDataAdapter>
    {
        private DataLocationModel[] ConvertToDataLocationModel(string pointsString, string csys)
        {
            string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var points = new List<DataLocationModel>();
            if (!string.IsNullOrEmpty(pointsString))
            {
                foreach (var a in pointsString.Split(new[] { "\r\n" }, StringSplitOptions.None))
                {
                    if (!string.IsNullOrEmpty(a))
                    {
                        string[] b = a.Split(new[] { "\t" }, StringSplitOptions.None);
                        if (b.Length == 2)
                        {
                            double k1;
                            double k2;
                            if (double.TryParse(b[0].Replace(".", sep), out k1) && double.TryParse(b[1].Replace(".", sep), out k2))
                            {
                                if ("4DMS".Equals(csys, StringComparison.OrdinalIgnoreCase))
                                {
                                    var point = new DataLocationModel()
                                    {
                                        Longitude = IMPosition.Dms2Dec(k1),
                                        Latitude = IMPosition.Dms2Dec(k2)
                                    };
                                    points.Add(point);
                                }
                                else
                                {
                                    var point = new DataLocationModel()
                                    {
                                        Longitude = k1,
                                        Latitude = k2
                                    };
                                    points.Add(point);
                                }
                            }
                        }
                    }
                }
            }
            return points.ToArray();
        }

        public void Refresh()
        {
            var areas = new List<YArea>();
            IMRecordset rs = new IMRecordset("AREA", IMRecordset.Mode.ReadOnly);
            rs.Select("ID,NAME,DENSITY,CREATED_BY,DATE_CREATED,POINTS,CSYS");
            rs.OrderBy("NAME", OrderDirection.Ascending);
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var area = new YArea()
                {
                    m_id = rs.GetI("ID"),
                    m_name = rs.GetS("NAME"),
                    m_type = rs.GetS("DENSITY"),
                    m_created_by = rs.GetS("CREATED_BY"),
                    m_date_created = rs.GetT("DATE_CREATED"),
                    m_points = rs.GetS("POINTS")
                };
                areas.Add(area);

                string csys = rs.GetS("CSYS");

                if (!"4DEC".Equals(csys, StringComparison.OrdinalIgnoreCase) && !"4DMS".Equals(csys, StringComparison.OrdinalIgnoreCase))
                    continue;

            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();
            this.Source = areas.ToArray();
        }
        protected override Func<YArea, AreaModel> GetMapper()
        {
            return source => new AreaModel
            {
                IdentifierFromICSM = source.m_id,
                Name = source.m_name,
                DateCreated = source.m_date_created,
                TypeArea = source.m_type,
                CreatedBy = source.m_created_by,
                Location = ConvertToDataLocationModel(source.m_points, source.m_csys)
            };
        }
        
    }
}
