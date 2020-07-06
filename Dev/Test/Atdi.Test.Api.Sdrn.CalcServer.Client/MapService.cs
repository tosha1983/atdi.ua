using Atdi.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Test.Api.Sdrn.CalcServer.Client.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM_CS = Atdi.DataModels.Sdrn.CalcServer.Entities;
using DM_IC = Atdi.DataModels.Sdrn.Infocenter.Entities;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client
{
	internal class MapService
	{
		public static MapFile LoadMapSectorContent(long sectorId)
		{
			var endpoint = new WebApiEndpoint(new Uri("http://localhost:15075/"), "/appserver/v1");
			var dataContext = new WebApiDataContext("SDRN_Infocenter_DB");

			var dataLayer = new WebApiDataLayer(endpoint, dataContext);

			var webQuery = dataLayer.GetBuilder<DM_IC.IMapSector>()
					.Read()
					.Select(c => c.SectorName)
					.Select(c => c.MAP.MapName)
					.Select(c => c.MAP.MapNote)
					.Select(c => c.AxisXNumber)
					.Select(c => c.MAP.AxisXStep)
					.Select(c => c.AxisYNumber)
					.Select(c => c.MAP.AxisYStep)
					.Select(c => c.UpperLeftX)
					.Select(c => c.UpperLeftY)
					.Select(c => c.LowerRightX)
					.Select(c => c.LowerRightY)
					.Select(c => c.MAP.Projection)
					.Select(c => c.MAP.StepUnit)
					.Select(c => c.MAP.TypeCode)
					.Select(c => c.MAP.TypeName)
					.Select(c => c.MAP.StepDataType)
					.Select(c => c.MAP.StepDataSize)
					.Select(c => c.ContentType)
					.Select(c => c.ContentEncoding)
					.Select(c => c.Content)
					.Filter(c => c.Id, sectorId)
				;
			var mapFile = new MapFile();
			var reader = dataLayer.Executor.ExecuteReader(webQuery);
			if (!reader.Read())
			{
				return mapFile;
			}


			//if ("MAP.MapName".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.MapName = $"Sector_{sectorId}"; //reader.GetValue(c => c.MAP.MapName);
			}
			//if ("MAP.MapNote".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.MapNote = $"Sector_{sectorId}";  //$"Map='{reader.GetValue(c => c.MAP.MapName)}', Sector:'{reader.GetValue(c => c.SectorName)}'";
			}
			//if ("MAP.AxisXNumber".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.AxisX.Number = reader.GetValue(c => c.AxisXNumber);
			}
			//if ("MAP.AxisXStep".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.AxisX.Step = reader.GetValue(c => c.MAP.AxisXStep);
			}
			//if ("MAP.AxisYNumber".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.AxisY.Number = reader.GetValue(c => c.AxisYNumber);
			}
			//if ("MAP.AxisYStep".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.AxisY.Step = reader.GetValue(c => c.MAP.AxisYStep);
			}

			//if ("MAP.UpperLeftX".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Coordinates.UpperLeft.X = reader.GetValue(c => c.UpperLeftX);
			}
			//if ("MAP.UpperLeftY".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Coordinates.UpperLeft.Y = reader.GetValue(c => c.UpperLeftY);
			}
			//if ("MAP.LowerRightX".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Coordinates.LowerRight.X = reader.GetValue(c => c.LowerRightX);
			}
			//if ("MAP.LowerRightY".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Coordinates.LowerRight.Y = reader.GetValue(c => c.LowerRightY);
			}

			mapFile.Coordinates.LowerLeft.X = mapFile.Coordinates.UpperLeft.X;
			mapFile.Coordinates.LowerLeft.Y = mapFile.Coordinates.LowerRight.Y;
			mapFile.Coordinates.UpperRight.X = mapFile.Coordinates.LowerRight.X;
			mapFile.Coordinates.UpperRight.Y = mapFile.Coordinates.UpperLeft.Y;

			//if ("MAP.Projection".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Projection = reader.GetValue(c => c.MAP.Projection);
			}
			//if ("MAP.StepUnit".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.StepUnit = reader.GetValue(c => c.MAP.StepUnit);
			}

			//if ("TypeCode".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.MapType = (MapType)reader.GetValue(c => c.MAP.TypeCode); ;
			}
			//if ("TypeName".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.MapTypeName = reader.GetValue(c => c.MAP.TypeName);
			}
			//if ("StepDataType".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.StepDataType = reader.GetValue(c => c.MAP.StepDataType);
			}
			//if ("StepDataSize".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.StepDataSize = reader.GetValue(c => c.MAP.StepDataSize);
			}
			//if ("ContentType".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.ContentType = reader.GetValue(c => c.ContentType);
			}
			//if ("ContentEncoding".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.ContentEncoding = reader.GetValue(c => c.ContentEncoding);
			}
			//if ("Content".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Content = reader.GetValue(c => c.Content);  //Convert.FromBase64String(Convert.ToString(result.Record[i])); // Convert.ToString(result.Record[i])?.Select(c => (byte)c).ToArray();
			}

			// return project ID.
			return mapFile;
		}

		public static MapFile LoadProjectMapContent(long contentId)
		{
			var endpoint = new WebApiEndpoint(new Uri("http://localhost:15070/"), "/appserver/v1");
			var dataContext = new WebApiDataContext("SDRN_CalcServer_DB");

			var dataLayer = new WebApiDataLayer(endpoint, dataContext);

			var webQuery = dataLayer.GetBuilder<DM_CS.IProjectMapContent>()
					.Read()
					.Select(c => c.MAP.MapName)
					.Select(c => c.MAP.MapNote)
					.Select(c => c.MAP.AxisXNumber)
					.Select(c => c.MAP.AxisXStep)
					.Select(c => c.MAP.AxisYNumber)
					.Select(c => c.MAP.AxisYStep)
					.Select(c => c.MAP.UpperLeftX)
					.Select(c => c.MAP.UpperLeftY)
					.Select(c => c.MAP.LowerRightX)
					.Select(c => c.MAP.LowerRightY)
					.Select(c => c.MAP.PROJECT.Projection)
					.Select(c => c.MAP.StepUnit)
					.Select(c => c.TypeCode)
					.Select(c => c.TypeName)
					.Select(c => c.StepDataType)
					.Select(c => c.StepDataSize)
					.Select(c => c.ContentType)
					.Select(c => c.ContentEncoding)
					.Select(c => c.Content)
					.Filter(c => c.Id, contentId)
				;
			var mapFile = new MapFile();
			var reader = dataLayer.Executor.ExecuteReader(webQuery);
			if (!reader.Read())
			{
				return mapFile;
			}


			//if ("MAP.MapName".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.MapName = reader.GetValue(c => c.MAP.MapName);
			}
			//if ("MAP.MapNote".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.MapNote = reader.GetValue(c => c.MAP.MapNote);
			}
			//if ("MAP.AxisXNumber".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.AxisX.Number = reader.GetValue(c => c.MAP.AxisXNumber).GetValueOrDefault();
			}
			//if ("MAP.AxisXStep".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.AxisX.Step = reader.GetValue(c => c.MAP.AxisXStep).GetValueOrDefault();
			}
			//if ("MAP.AxisYNumber".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.AxisY.Number = reader.GetValue(c => c.MAP.AxisYNumber).GetValueOrDefault();
			}
			//if ("MAP.AxisYStep".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.AxisY.Step = reader.GetValue(c => c.MAP.AxisYStep).GetValueOrDefault();
			}

			//if ("MAP.UpperLeftX".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Coordinates.UpperLeft.X = reader.GetValue(c => c.MAP.UpperLeftX).GetValueOrDefault();
			}
			//if ("MAP.UpperLeftY".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Coordinates.UpperLeft.Y = reader.GetValue(c => c.MAP.UpperLeftY).GetValueOrDefault();
			}
			//if ("MAP.LowerRightX".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Coordinates.LowerRight.X = reader.GetValue(c => c.MAP.LowerRightX).GetValueOrDefault();
			}
			//if ("MAP.LowerRightY".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Coordinates.LowerRight.Y = reader.GetValue(c => c.MAP.LowerRightY).GetValueOrDefault();
			}

			mapFile.Coordinates.LowerLeft.X = mapFile.Coordinates.UpperLeft.X;
			mapFile.Coordinates.LowerLeft.Y = mapFile.Coordinates.LowerRight.Y;
			mapFile.Coordinates.UpperRight.X = mapFile.Coordinates.LowerRight.X;
			mapFile.Coordinates.UpperRight.Y = mapFile.Coordinates.UpperLeft.Y;

			//if ("MAP.Projection".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Projection = reader.GetValue(c => c.MAP.PROJECT.Projection);
			}
			//if ("MAP.StepUnit".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.StepUnit = reader.GetValue(c => c.MAP.StepUnit);
			}

			//if ("TypeCode".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.MapType = (MapType)reader.GetValue(c => c.TypeCode); ;
			}
			//if ("TypeName".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.MapTypeName = reader.GetValue(c => c.TypeName);
			}
			//if ("StepDataType".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.StepDataType = reader.GetValue(c => c.StepDataType);
			}
			//if ("StepDataSize".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.StepDataSize = reader.GetValue(c => c.StepDataSize);
			}
			//if ("ContentType".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.ContentType = reader.GetValue(c => c.ContentType);
			}
			//if ("ContentEncoding".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.ContentEncoding = reader.GetValue(c => c.ContentEncoding);
			}
			//if ("Content".Equals(field.Path, StringComparison.OrdinalIgnoreCase))
			{
				mapFile.Content = reader.GetValue(c => c.Content);  //Convert.FromBase64String(Convert.ToString(result.Record[i])); // Convert.ToString(result.Record[i])?.Select(c => (byte)c).ToArray();
			}

			// return project ID.
			return mapFile;
		}
	}
}
