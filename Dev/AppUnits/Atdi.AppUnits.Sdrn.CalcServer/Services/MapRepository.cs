using Atdi.Contracts.Sdrn.CalcServer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.AppUnits.Sdrn.CalcServer.Helpers;
using Atdi.Common.Extensions;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;

namespace Atdi.AppUnits.Sdrn.CalcServer.Services
{
	internal class MapRepository : IMapRepository
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ILogger _logger;

		public MapRepository(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
			_logger = logger;
		}

		public ProjectMapData GetMapByName(IDataLayerScope dbScope, long projectId, string mapName)
		{
			var mapData = this.LoadProjectMap(dbScope, projectId, mapName);
			if (mapData == null)
			{
				throw new InvalidOperationException($"Failed to load map by name '{mapName}' for project with ID #{projectId}");
			}

			mapData.ReliefContent =
				this.LoadProjectMapContent<short>(dbScope, mapData.Id, ProjectMapType.Relief);

			mapData.ClutterContent =
				this.LoadProjectMapContent<byte>(dbScope, mapData.Id, ProjectMapType.Clutter);

			mapData.BuildingContent =
				this.LoadProjectMapContent<byte>(dbScope, mapData.Id, ProjectMapType.Building);

			return mapData;
		}

		private ProjectMapData LoadProjectMap(IDataLayerScope calcDbScope, long projectId, string mapName)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.From()
				.Select(
					c => c.Id,
					c => c.AxisXNumber,
					c => c.AxisXStep,
					c => c.AxisYNumber,
					c => c.AxisYStep,
					c => c.UpperLeftX,
					c => c.UpperLeftY,
					c => c.LowerRightX,
					c => c.LowerRightY
				)
				.Where(c => c.MapName, ConditionOperator.Equal, mapName)
				.Where(c => c.PROJECT.Id, ConditionOperator.Equal, projectId);

			return calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return null;
				}
				return new ProjectMapData()
				{
					Id = reader.GetValue(c => c.Id),
					AxisX = new AtdiAxis()
					{
						Number = reader.GetValue(c => c.AxisXNumber).GetValueOrDefault(),
						Step = reader.GetValue(c => c.AxisXStep).GetValueOrDefault()
					},
					AxisY = new AtdiAxis()
					{
						Number = reader.GetValue(c => c.AxisYNumber).GetValueOrDefault(),
						Step = reader.GetValue(c => c.AxisYStep).GetValueOrDefault()
					},
					UpperLeft = new AtdiCoordinate()
					{
						X = reader.GetValue(c => c.UpperLeftX).GetValueOrDefault(),
						Y = reader.GetValue(c => c.UpperLeftY).GetValueOrDefault(),
					},
					LowerRight = new AtdiCoordinate()
					{
						X = reader.GetValue(c => c.LowerRightX).GetValueOrDefault(),
						Y = reader.GetValue(c => c.LowerRightY).GetValueOrDefault(),
					}
				};
			});
		}

		private T[] LoadProjectMapContent<T>(IDataLayerScope calcDbScope, long mapId, ProjectMapType mapType)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMapContent>()
				.From()
				.Select(
					c => c.Content,
					c => c.ContentEncoding,
					c => c.ContentType
				)
				.Where(c => c.TypeCode, ConditionOperator.Equal, (byte)mapType)
				.Where(c => c.MAP.Id, ConditionOperator.Equal, mapId);

			var result = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return null;
				}

				var contentEncoding = reader.GetValue(c => c.ContentEncoding);
				var contentType = reader.GetValue(c => c.ContentType);
				var content = reader.GetValue(c => c.Content);

				return DecodeContent<T>(contentEncoding, contentType, content);
			});

			if (result == null)
			{
				throw new InvalidOperationException($"Undefined content for map with ID #{mapId} and type '{mapType}'");
			}
			return result;
		}

		public T[] DecodeContent<T>(string contentEncoding, string contentType, byte[] content)
		{
			if (content == null)
			{
				return default(T[]);
			}

			var raw = content;
			if (!string.IsNullOrEmpty(contentEncoding))
			{
				if (contentEncoding.Contains("compressed"))
				{
					raw = Compressor.Decompress(raw);
				}
				else
				{
					throw new InvalidOperationException($"Unsupported encoding '{contentEncoding}'");
				}
			}

			var expectedType = typeof(T);
			if (expectedType == typeof(byte) && typeof(byte[]).AssemblyQualifiedName == contentType)
			{
				return (T[])(object)raw.Deserialize<byte[]>();
			}
			else if (expectedType == typeof(short) && typeof(short[]).AssemblyQualifiedName == contentType)
			{
				return (T[])(object)raw.Deserialize<short[]>();
			}
			else
			{
				throw new InvalidOperationException($"Unsupported content type '{contentType}'. Expected type is '{expectedType.AssemblyQualifiedName}'");
			}


		}

		public CluttersDesc GetCluttersDesc(IDataLayerScope dbScope, long mapId)
		{
			var selDescQuery = _calcServerDataLayer.GetBuilder<ICluttersDesc>()
				.From()
				.Select(
					c => c.Id
				)
				.Where(c => c.MAP.Id, ConditionOperator.Equal, mapId)
				.OrderByDesc(c => c.Id)
				.OnTop(1);

			var descId = dbScope.Executor.ExecuteAndFetch(selDescQuery, reader =>
			{
				if (reader.Read())
				{
					return reader.GetValue(c => c.Id);
				}

				return 0;
			});

			if (descId == 0)
			{
				throw  new InvalidOperationException($"A clutter description not found by map id #{mapId}");
			}

			var result = new CluttersDesc
			{
				Id = descId
			};

			var selFreqQuery = _calcServerDataLayer.GetBuilder<ICluttersDescFreq>()
				.From()
				.Select(
					c => c.Freq_MHz,
					c => c.Id
				)
				.Where(c => c.CLUTTERS_DESC.Id, ConditionOperator.Equal, descId)
				.OrderByAsc(c => c.Freq_MHz);

			result.Frequencies = dbScope.Executor.ExecuteAndFetch(selFreqQuery, reader =>
			{
				var records = new List<CluttersDescFreq>();
				while (reader.Read())
				{
					var freq = new CluttersDescFreq
					{
						Id = reader.GetValue(c => c.Id),
						Freq_MHz = reader.GetValue(c => c.Freq_MHz),
						Clutters = new CluttersDescFreqClutter[MapSpecification.CluttersMaxCount]
					};
					records.Add(freq);
				}
				return records.ToArray();
			});

			foreach (var freq in result.Frequencies)
			{
				var selFreqClutterQuery = _calcServerDataLayer.GetBuilder<ICluttersDescFreqClutter>()
					.From()
					.Select(
						c => c.FlatLoss_dB,
						c => c.LinearLoss_dBkm,
						c => c.Reflection,
						c => c.Code
					)
					.Where(c => c.FreqId, ConditionOperator.Equal, freq.Id)
					.OrderByAsc(c => c.Code);

				dbScope.Executor.ExecuteAndFetch(selFreqClutterQuery, reader =>
				{
					while (reader.Read())
					{
						var code = reader.GetValue(c => c.Code);
						freq.Clutters[code].Reflection = reader.GetValue(c => c.Reflection).GetValueOrDefault();
						freq.Clutters[code].FlatLoss_dB = reader.GetValue(c => c.FlatLoss_dB).GetValueOrDefault();
						freq.Clutters[code].LinearLoss_dBkm = reader.GetValue(c => c.LinearLoss_dBkm).GetValueOrDefault();
					}
					return 0;
				});
			}

			var selClutterQuery = _calcServerDataLayer.GetBuilder<ICluttersDescClutter>()
				.From()
				.Select(
					c => c.Code,
					c => c.Height_m
				)
				.Where(c => c.CluttersDescId, ConditionOperator.Equal, descId)
				.OrderByAsc(c => c.Code);

			dbScope.Executor.ExecuteAndFetch(selClutterQuery, reader =>
			{
				while (reader.Read())
				{
					foreach (var freq in result.Frequencies)
					{
						var code = reader.GetValue(c => c.Code);
						freq.Clutters[code].Height_m = reader.GetValue(c => c.Height_m);
					}
				}
				return 0;
			});

			return result;
		}
	}
}
