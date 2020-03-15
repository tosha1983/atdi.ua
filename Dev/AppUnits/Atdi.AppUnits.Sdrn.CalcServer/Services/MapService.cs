﻿using Atdi.Contracts.Sdrn.CalcServer.Internal;
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

namespace Atdi.AppUnits.Sdrn.CalcServer.Services
{
	internal class MapService : IMapService
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ILogger _logger;

		public MapService(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
			_logger = logger;
		}

		public ProjectMapData GetMapByName(long projectId, string mapName)
		{
			using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
			{
				var mapData = this.LoadProjectMap(calcDbScope, projectId, mapName);
				if (mapData == null)
				{
					throw new InvalidOperationException($"Failed to load map by name '{mapName}' for project with ID #{projectId}");
				}

				mapData.ReliefContent =
					this.LoadProjectMapContent<short>(calcDbScope, mapData.Id, ProjectMapType.Relief);

				mapData.ClutterContent =
					this.LoadProjectMapContent<byte>(calcDbScope, mapData.Id, ProjectMapType.Clutter);

				mapData.BuildingContent =
					this.LoadProjectMapContent<byte>(calcDbScope, mapData.Id, ProjectMapType.Building);

				return mapData;
			}
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
					AxisX = new Axis()
					{
						Number = reader.GetValue(c => c.AxisXNumber).GetValueOrDefault(),
						Step = reader.GetValue(c => c.AxisXStep).GetValueOrDefault()
					},
					AxisY = new Axis()
					{
						Number = reader.GetValue(c => c.AxisYNumber).GetValueOrDefault(),
						Step = reader.GetValue(c => c.AxisYStep).GetValueOrDefault()
					},
					UpperLeft = new Coordinate()
					{
						X = reader.GetValue(c => c.UpperLeftX).GetValueOrDefault(),
						Y = reader.GetValue(c => c.UpperLeftY).GetValueOrDefault(),
					},
					LowerRight = new Coordinate()
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
	}
}
