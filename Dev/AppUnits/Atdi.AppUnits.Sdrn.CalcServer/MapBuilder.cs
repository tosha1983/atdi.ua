using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.CalcServer.Helpers;
using Atdi.Common;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using IC=Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.Platform.Logging;
using DM = Atdi.AppUnits.Sdrn.CalcServer.DataModel;
using Atdi.Common.Extensions;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal class MapBuilder
	{
		private struct CoverageSourceMapCell
		{
			public int XIndex;
			public int YIndex;
			public Coordinate UpperLeft;
			public byte[] CellArea;
			public int Amount;

			public void RegionOut(AreaCoordinates region, int xStep, int yStep)
			{
				if (this.Amount <= 0)
				{
					return;
				}

				// минусуем
				for (int y = 0; y < yStep; y++)
				{
					for (int x = 0; x < xStep; x++)
					{
						if (this.CellArea[y * xStep + x] != 0)
						{ 
							if (region.Has(this.UpperLeft.X + x, (this.UpperLeft.Y - 1) - y))
							{
								--this.Amount;
								this.CellArea[y * xStep + x] = (byte)0;
							}
						}
					}
				}
			}
		}

		private struct CoverageSourceMapArea
		{
			public DM.SourceMapData SourceMap;
			public CoverageSourceMapCell[] Cells;

			/// <summary>
			/// Площадь которубю покрывает карта на целеовой ячейке 
			/// </summary>
			public AreaCoordinates CoverageArea;


			public void RegionOut(AreaCoordinates region)
			{
				for (int i = 0; i < Cells.Length; i++)
				{
					Cells[i].RegionOut(region, SourceMap.AxisXStep, SourceMap.AxisYStep);
				}

				Cells = Cells.Where(c => c.Amount > 0).ToArray();
			}
		}



		private struct MapCellDescriptor<T>
			where T : struct
		{
			public int XIndex;
			public int YIndex;

			/// <summary>
			/// Карты покрывающие заданную индексами площадь
			/// </summary>
			public MapReference<T>[] Maps;
		}

		private struct MapRocessingRecord<T>
			where T : struct
		{
			/// <summary>
			/// Индекс расчетного бокса по X
			/// </summary>
			public int XIndex;

			/// <summary>
			/// Индекс расчетного бокса по Y
			/// </summary>
			public int YIndex;

			// ссылка на участок на карте источнике и его значение в покрываемой области
			public MapReference<T> SourceMap;
		}

		private class MapReference<T>
			where T : struct
		{
			/// <summary>
			/// Индекс достeпа к значению карты источника по X
			/// </summary>
			public int XIndex;

			/// <summary>
			/// Индекс достeпа к значению карты источника по Y
			/// </summary>
			public int YIndex;

			/// <summary>
			/// Идентифкатор карты источника
			/// </summary>
			public DM.SourceMapData InfocenterMap;

			/// <summary>
			/// Значение из карты источника
			/// </summary>
			public T Value;

			/// <summary>
			/// Площадь покрытия картой источника 
			/// </summary>
			public long CoverageArea;
		}

		private static readonly object DefaultForRelief = (short) -9999;
		private static readonly object DefaultForClutter = (byte)0;
		private static readonly object DefaultForBuilding = (byte)0;

		private readonly AppServerComponentConfig _config;
		private readonly IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> _infocenterDataLayer;
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ILogger _logger;

		public MapBuilder(
			AppServerComponentConfig config,
			IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> infocenterDataLayer,
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ILogger logger)
		{
			_config = config;
			_infocenterDataLayer = infocenterDataLayer;
			_calcServerDataLayer = calcServerDataLayer;
			_logger = logger;
		}

		public void PrepareMap(long projectMapId)
		{
			try
			{
				_logger.Verbouse(Contexts.ThisComponent, Categories.MapPreparation, $"Preparing project map with id #{projectMapId}");

				using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
				{
					// ставим статус обработки

					if (!this.ChangeProjectMapStatus(calcDbScope, projectMapId, ProjectMapStatusCode.Processing, ProjectMapStatusCode.Pending))
					{
						throw new InvalidOperationException($"Failed to set project map current status in processing status.");
					}

					try
					{
						// все измененяи по карте в рамках тразакции
						calcDbScope.BeginTran();

						// чистим контекст карты
						CleanProjectMapContext(calcDbScope, projectMapId);

						// читаем свойства карты проекта
						var projectMapData = this.LoadProjectMapData(calcDbScope, projectMapId);

						// проверим свойства шага: шаговый бокс и ко-во
						CheckAxis(projectMapData);

						// расчитываем область карты и проверяем ее с трешхолодом (не более 15 000 х 15 000 шагов), при нарушении падаем
						CheckThresholdMaxSteps(projectMapData.StepsNumber);

						// проверка проекции
						CheckProjection(projectMapData.Projection);

						// проверка единици измерения карты
						CheckStepUnit(projectMapData.StepUnit);

						// расчитываем конечные координаты относительно указаных шагов
						projectMapData.AxisXNumber = projectMapData.OwnerAxisXNumber;
						projectMapData.AxisXStep = projectMapData.OwnerAxisXStep;
						projectMapData.AxisYNumber = projectMapData.OwnerAxisYNumber;
						projectMapData.AxisYStep = projectMapData.OwnerAxisYStep;
						projectMapData.UpperLeftX = projectMapData.OwnerUpperLeftX;
						projectMapData.UpperLeftY = projectMapData.OwnerUpperLeftY;

						projectMapData.LowerRightX =
							projectMapData.UpperLeftX + projectMapData.AxisXNumber * projectMapData.AxisXStep;
						projectMapData.LowerRightY =
							projectMapData.UpperLeftY + projectMapData.AxisYNumber * projectMapData.AxisYStep;

						// сохраним расчеті
						SaveProjectMapData(calcDbScope, projectMapData);

						// с этого момент можем приступить к построеннию карт
						using (var infoDbScope = this._infocenterDataLayer.CreateScope<InfocenterDataContext>())
						{
							this.BuildMap<short>(calcDbScope, infoDbScope, projectMapData, ProjectMapType.Relief);
							this.BuildMap<byte>(calcDbScope, infoDbScope, projectMapData, ProjectMapType.Building);
							this.BuildMap<byte>(calcDbScope, infoDbScope, projectMapData, ProjectMapType.Clutter);
						}
							
						// фиксируем состояние
						this.ChangeProjectMapStatus(calcDbScope, projectMapId, ProjectMapStatusCode.Prepared);
						calcDbScope.Commit();
						_logger.Verbouse(Contexts.ThisComponent, Categories.MapPreparation, $"Prepared project map with id #{projectMapId}");
					}
					catch (Exception e)
					{
						calcDbScope.Rollback();
						this.ChangeProjectMapStatus(calcDbScope, projectMapId, ProjectMapStatusCode.Failed, e.ToString());
						throw;
					}
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.MapPreparation, $"Failed to prepare map with ID ${projectMapId}", e, this);
			}
			
		}

		private void BuildMap<T>(IDataLayerScope calcDbScope, IDataLayerScope infoDbScope, DM.ProjectMapData projectMap, ProjectMapType mapType)
			where T : struct
		{
			var mapContent = new DM.MapContentData<T>(projectMap, mapType);

			// подбираем карты но еще их не грузим
			var sourceMaps = this.FindSourceMaps(infoDbScope, projectMap, mapType);
			if (sourceMaps.Length == 0)
			{
				throw new InvalidOperationException($"Could not find any matching maps of type '{mapType}' in the Infocenter DB");
			}

			// для рельефа если есть мастер карта, то выравниваемся  к ней по ее координатной сетке
			var masterMap = sourceMaps[0];
			if (mapType == ProjectMapType.Relief && masterMap.Priority == 0)
			{
				// смещение делаем через индекс
				// берем координаты смещаемой точки
				// поним определяем индекс ячейки
				// по индексу ячейки мастер карты определяем ее верхние координаты - они и есть координата смещения
				// аналогично поступаем с нижней правой точкой
				// по результатм пересчитаем кол-во шагов

				var sourceIndexer = masterMap.CoordinateToIndexes(projectMap.UpperLeftX, projectMap.UpperLeftY);
				var toUpperLeft = masterMap.IndexToUpperLeftCoordinate(sourceIndexer.XIndex, sourceIndexer.YIndex);
				sourceIndexer = masterMap.CoordinateToIndexes(projectMap.LowerRightX, projectMap.LowerRightY);
				var toLowerRight = masterMap.IndexToLowerRightCoordinate(sourceIndexer.XIndex, sourceIndexer.YIndex);

				projectMap.UpperLeftX = toUpperLeft.X;
				projectMap.UpperLeftY = toUpperLeft.Y;

				projectMap.LowerRightX = toLowerRight.X;
				projectMap.LowerRightY = toLowerRight.Y;

				projectMap.AxisXNumber = (toLowerRight.X - toUpperLeft.X) / projectMap.AxisXStep;
				projectMap.AxisYNumber = (toUpperLeft.Y - toLowerRight.Y) / projectMap.AxisYStep;

				if (0 != ((toLowerRight.X - toUpperLeft.X) % projectMap.AxisXStep) 
				 || 0 != ((toUpperLeft.Y - toLowerRight.Y) % projectMap.AxisYStep))
				{
					throw new InvalidOperationException("Something went wrong in the map offset calculation");
				}

				// сохраним карту
				this.SaveProjectMapData(calcDbScope, projectMap);
			}

			// выделяем буффер
			var cellDescriptors = new MapCellDescriptor<T>[projectMap.AxisXNumber * projectMap.AxisYNumber];

			// Работаем в три прохода
			// строим матрицу обхода, в начале идем по целеdой матрице, определяем на кажудю точк уоткуда с какой карты берем значение
			// потом групируем по картам и из них определяем данные
			// третий проход расчет всего что нужна на основании полученных данных

			for (var yIndex = 0; yIndex < projectMap.AxisYNumber; yIndex++)
			{
				for (var xIndex = 0; xIndex < projectMap.AxisXNumber; xIndex++)
				{
					var area = projectMap.IndexToArea(xIndex, yIndex);
					cellDescriptors[yIndex * projectMap.AxisXNumber + xIndex] = new MapCellDescriptor<T>()
					{
						XIndex = xIndex,
						YIndex = yIndex,
						Maps = this.DefineSourceMaps<T>(xIndex, yIndex, area, mapContent, sourceMaps)
					};
				}
			}

			// групируем и грузим карты
			cellDescriptors.SelectMany( c => c.Maps.Select(m => new MapRocessingRecord<T> { SourceMap = m, XIndex = c.XIndex, YIndex = c.YIndex }))
				.GroupBy(g => g.SourceMap.InfocenterMap)
				.Select(g => new { InfocenterMap = g.Key, Count = g.Count(), Use = g.Select(p => p).ToArray()})
				.ToList()
				.ForEach(result => this.HandleSourceMap(infoDbScope, result.InfocenterMap, result.Use));

			// строим типизируемую матрицу
			mapContent.Content = new T[projectMap.AxisXNumber * projectMap.AxisYNumber];

			// площадь ячейки, нужна для рельефа
			var reliefCellArea = (double)projectMap.AxisXStep * projectMap.AxisYStep;

			for (var yIndex = 0; yIndex < projectMap.AxisYNumber; yIndex++)
			{
				for (var xIndex = 0; xIndex < projectMap.AxisXNumber; xIndex++)
				{
					// индекс значения
					var contentIndex = yIndex * projectMap.AxisXNumber + xIndex;
					// делаем расчет ячейки

					
					var cell = cellDescriptors[yIndex * projectMap.AxisXNumber + xIndex];
					if (cell.Maps == null || cell.Maps.Length == 0)
					{
						// нет карты, заполяняем дефолтным значением
						if (mapType == ProjectMapType.Relief)
						{
							mapContent.Content[contentIndex] = (T)MapBuilder.DefaultForRelief;
						}
						else if (mapType == ProjectMapType.Clutter)
						{
							mapContent.Content[contentIndex] = (T)MapBuilder.DefaultForClutter;
						}
						else if (mapType == ProjectMapType.Building)
						{
							mapContent.Content[contentIndex] = (T)MapBuilder.DefaultForBuilding;
						}
						else
						{
							throw new InvalidOperationException($"Unsupported map type '{mapType}'");
						}
					}
					// если карта одна, тут все просто
					else if (cell.Maps.Length == 1)
					{
						mapContent.Content[contentIndex] = cell.Maps[0].Value;
						cell.Maps[0].InfocenterMap.Used = true;
					}
					else
					{
						// определение значения зависит от типа карты
						if (mapType == ProjectMapType.Relief)
						{
							mapContent.Content[contentIndex] = (T)(object)cell.Maps.Sum(c => ((short)(object)(c.Value)) * (c.CoverageArea / reliefCellArea));
						}
						else if (mapType == ProjectMapType.Building|| mapType == ProjectMapType.Clutter)
						{
							var max = cell.Maps.Max(c => c.CoverageArea);
							mapContent.Content[contentIndex] = cell.Maps.Where(c => c.CoverageArea == max).Max(c => c.Value);
						}
						else
						{
							throw new InvalidOperationException($"Unsupported map type '{mapType}'");
						}
					}
				}
			}

			// сохраянем буффер - контент ProjectMapContent и все его источники ProjectMapContentSource
			var contentId = this.SaveProjectMapContent(calcDbScope, mapContent);

			foreach (var sourceMap in sourceMaps)
			{
				// сохраним только те которые подлежали использованию
				if (sourceMap.Used)
				{
					this.SaveSourceMap(calcDbScope, contentId, sourceMap);
				}
			}
		}

		/// <summary>
		/// Обработка карт источников - получение значений
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="infocenterMap"></param>
		/// <param name="projectMap"></param>
		/// <param name="use"></param>
		private void HandleSourceMap<T>(IDataLayerScope infoDbScope, DM.SourceMapData infocenterMap, MapRocessingRecord<T>[] use)
			where T : struct
		{
			// кеш  загруженных секторов
			var sectors = new DM.SourceMapSectorData[infocenterMap.SectorsCount];
			var loadedSectorsCount = 0;
			for (int i = 0; i < use.Length; i++)
			{
				var mapRecord = use[i];
				// задача для указаных в записи  индексов нужно получить значение
				// переводим индексы в координаты
				var sourceMap = mapRecord.SourceMap;
				var upperLeft = infocenterMap.IndexToUpperLeftCoordinate(sourceMap.XIndex, sourceMap.YIndex);
				var lowerRight = infocenterMap.IndexToLowerRightCoordinate(sourceMap.XIndex, sourceMap.YIndex);

				// ищим сектор ране загруженный
				var sector = sectors.FirstOrDefault(
					s => 
						s.UpperLeftX <= upperLeft.X && upperLeft.X < s.LowerRightX
					&&  s.UpperLeftX < lowerRight.X && lowerRight.X <= s.LowerRightX
					&&  s.UpperLeftY >= upperLeft.Y && upperLeft.Y > s.LowerRightY
					&&  s.UpperLeftY > lowerRight.Y && lowerRight.Y >= s.LowerRightY
					);
				// загружаем по координатам сектор, если еще не загружен
				if (sector == null)
				{
					sector = this.LoadMapSector(infoDbScope, infocenterMap, upperLeft, lowerRight);
					sectors[loadedSectorsCount++] = sector;
				}
				// из бинарника сектора определяем значение
				sourceMap.Value = sector.GetValue<T>(sourceMap.XIndex, sourceMap.YIndex);
			}
			
			// возможно тут стоит иницровать сборку муссора... надо подумать
			// GC.Collect();
		}

		private DM.SourceMapSectorData LoadMapSector(IDataLayerScope infoDbScope, DM.SourceMapData infocenterMap, Coordinate upperLeft, Coordinate lowerRight)
		{
			var sourceMapQuery = _infocenterDataLayer.GetBuilder<IC.IMapSector>()
					.From()
					.Select(
						c => c.Id,
						c => c.UpperLeftX,
						c => c.UpperLeftY,
						c => c.LowerRightX,
						c => c.LowerRightY,
						c => c.AxisXNumber,
						c => c.AxisXIndex,
						c => c.AxisYNumber,
						c => c.AxisYIndex,
						c => c.Content,
						c => c.ContentEncoding,
						c => c.ContentType)
					.Where(c => c.MAP.Id, ConditionOperator.Equal, infocenterMap.MapId)
					.Where(c => c.UpperLeftX, ConditionOperator.LessEqual, upperLeft.X) 
					.Where(c => c.LowerRightX, ConditionOperator.GreaterThan, upperLeft.X)
					.Where(c => c.UpperLeftX, ConditionOperator.LessThan, lowerRight.X)
					.Where(c => c.LowerRightX, ConditionOperator.GreaterEqual, lowerRight.X)

					.Where(c => c.UpperLeftY, ConditionOperator.GreaterEqual, upperLeft.Y)
					.Where(c => c.LowerRightY, ConditionOperator.LessThan, upperLeft.Y)
					.Where(c => c.UpperLeftY, ConditionOperator.GreaterThan, lowerRight.Y)
					.Where(c => c.LowerRightY, ConditionOperator.LessEqual, lowerRight.Y)
				;

			return infoDbScope.Executor.ExecuteAndFetch(sourceMapQuery, reader =>
			{
				DM.SourceMapSectorData sourceMapSector = null;
				var count = 0;
				while (reader.Read())
				{
					++count;
					if (count > 1)
					{
						throw new InvalidOperationException($"More than one source map sector detected by coordinates ({upperLeft}x{lowerRight}) and map ID #{infocenterMap.MapId}");
					}
					sourceMapSector = new DM.SourceMapSectorData()
					{
						AxisXNumber = reader.GetValue(c => c.AxisXNumber),
						AxisXIndex = reader.GetValue(c => c.AxisXIndex),
						AxisYNumber = reader.GetValue(c => c.AxisXNumber),
						AxisYIndex = reader.GetValue(c => c.AxisXNumber),
						UpperLeftX = reader.GetValue(c => c.UpperLeftX),
						UpperLeftY = reader.GetValue(c => c.UpperLeftY),
						LowerRightX = reader.GetValue(c => c.LowerRightX),
						LowerRightY = reader.GetValue(c => c.LowerRightY),
						SectorId = reader.GetValue(c => c.Id),
						Content = this.DecodeSourceMapContent(
								reader.GetValue(c => c.Content),
								reader.GetValue(c => c.ContentEncoding),
								reader.GetValue(c => c.ContentType)
							),
						Map = infocenterMap
					};

				}
				return sourceMapSector;
			});
		}

		private byte[] DecodeSourceMapContent(byte[] content, string encoding, string contentType)
		{
			if (typeof(byte[]).AssemblyQualifiedName != contentType)
			{
				throw new InvalidOperationException($"Unsupported content type '{contentType}'");
			}
			if (string.IsNullOrEmpty(encoding))
			{
				return content;
			}

			if (encoding.Contains("compressed"))
			{
				return Compressor.Decompress(content);
			}
			throw new InvalidOperationException($"Unsupported encoding '{encoding}'");
		}
		private MapReference<T>[] DefineSourceMaps<T>(int xIndex, int yIndex, AreaCoordinates area, DM.MapContentData<T> mapContent, DM.SourceMapData[] sourceMaps)
			where T : struct
		{

			/*
			 * Возможное описание алгоритма (основняа идея отнять пересекающуюся площадь вверхлежащей карты во всех карта лежащих ниже текущей):
			 * 1. Бежим по картам от приоритетных к менее приоритетны пока
			 *    не найдем карту покрывающую полностью всю ячейку основания или достигнем конца.
			 *    Найденнаяч карта является самой "низкой"
			 * 2. в цыкле от низкой карты поднимаемся в "верх" по картам, при этом на каждой итерации
			 *		2.1. Вычислем набор чеек попадающих в зону покрытия и расчитываем по каждой плоащадь
			 *	    2.2. Считаем сумарную площадь покрытия
			 *      2.3. Организовываем цыкл от этой карты в низ лежащим картам
			 *           и на каждйо итерации отнимаем пересекающие области в ячейках низ лежащих карт.
			 *           При этом если для ячейки площадь ушла в 0 удаляем ячейку из набора
			 *
			 *     Для каждой чейки строим метрову матрицу для фикисрования разницы 
			 */
			var coveredSourceMapsCount = 0; // индекс самой низкой карты
			var coveredSourceMaps = new CoverageSourceMapArea[sourceMaps.Length];
			for (int i = 0; i < sourceMaps.Length; i++)
			{
				var sourceMap = sourceMaps[i];
				if (!sourceMap.IntersectWith(area, out var coverageArea))
				{
					// карта которая не покрывает ячейку [yIndex,xIndex] нам не интересна, отбрасываем ее
					continue;
				}
				// есть покрытие. фикисруем карту - возможно это последня карта по ряду причин (их две)

				coveredSourceMaps[coveredSourceMapsCount++] = new CoverageSourceMapArea
				{
					SourceMap = sourceMap,
					CoverageArea = coverageArea,
					Cells = this.DecomposeIntoCells(sourceMap, coverageArea)
				};
				
				if (area.Area == coverageArea.Area)
				{
					// это полное покрытие целевой ячейки [yIndex,xIndex] 
					// больше сканировать не нужно
					break;
				}
				// частичное покрытие, бежим глубже
			}

			// теперь бежим с низу в верх и отнимаем с верзу в низ площади
			for (int i = coveredSourceMapsCount - 1; i >= 0 ; i--)
			{

				if (i + 1 > coveredSourceMapsCount)
				{
					// раскладываем карту на молекули - ячейки по метрам
					var coveredSourceMapArea = coveredSourceMaps[i];

					for (int j = i + 1; j < coveredSourceMapsCount; j++)
					{
						var lowerCoveredSourceMapArea = coveredSourceMaps[j];
						lowerCoveredSourceMapArea.RegionOut(coveredSourceMapArea.CoverageArea);
					}
				}
			}

			return coveredSourceMaps
				.SelectMany(
					m => m.Cells
						.Select( 
							c=> new MapReference<T>
							{
								InfocenterMap = m.SourceMap,
								XIndex = c.XIndex,
								YIndex = c.YIndex,
								CoverageArea = c.Amount
							})
					).ToArray();
		}

		private CoverageSourceMapCell[] DecomposeIntoCells(DM.SourceMapData sourceMap, AreaCoordinates coverageArea)
		{
			// верхний индекс - при определни смещаемся на один метр
			var upperLeftIndexer = sourceMap.CoordinateToIndexes(coverageArea.UpperLeft.X, coverageArea.UpperLeft.Y - 1);
			var lowerRightIndexer = sourceMap.CoordinateToIndexes(coverageArea.LowerRight.X - 1, coverageArea.LowerRight.Y);
			var yCount = lowerRightIndexer.YIndex - upperLeftIndexer.YIndex;
			var xCount = lowerRightIndexer.XIndex - upperLeftIndexer.XIndex;

			var cells = new CoverageSourceMapCell[yCount * xCount];
			var position = 0;
			for (var yIndex = upperLeftIndexer.YIndex; yIndex <= lowerRightIndexer.YIndex; yIndex++)
			{
				for (var xIndex = upperLeftIndexer.XIndex; xIndex <= lowerRightIndexer.XIndex; xIndex++)
				{
					var cell = new CoverageSourceMapCell()
					{
						XIndex = xIndex,
						YIndex = yIndex,
						UpperLeft = sourceMap.IndexToUpperLeftCoordinate(xIndex, yIndex),
						CellArea = new byte[sourceMap.AxisYStep * sourceMap.AxisXStep]
					};
					
					// определдяем реальнео персечение
					for (int y = 0; y < sourceMap.AxisYStep; y++)
					{
						var yy = y * sourceMap.AxisXStep;
						var upperLeftYY = (cell.UpperLeft.Y - 1) - y;
						for (int x = 0; x < sourceMap.AxisXStep; x++)
						{
							if (coverageArea.Has(cell.UpperLeft.X + x, upperLeftYY))
							{
								++cell.Amount;
								cell.CellArea[yy + x] = (byte) 1;
							}
						}
					}

					cells[position++] = cell;
				}
			}
			return cells;
		}

		private long SaveProjectMapContent<T>(IDataLayerScope calcDbScope, DM.MapContentData<T> mapContent)
			where T : struct
		{
			var compressedBuffer = Compressor.Compress(mapContent.Content.Serialize());
			var insertQuery = _calcServerDataLayer.GetBuilder<IProjectMapContent>()
					.Insert()
					.SetValue(c => c.MAP.Id, mapContent.ProjectMap.ProjectMapId)
					.SetValue(c => c.TypeCode, (byte) mapContent.MapType)
					.SetValue(c => c.TypeName, mapContent.MapType.ToString())
					.SetValue(c => c.StepDataSize, mapContent.StepDataSize)
					.SetValue(c => c.StepDataType, DefineMapStepDataType(mapContent.MapType).AssemblyQualifiedName)
					.SetValue(c => c.SourceCount, mapContent.SourceCount)
					.SetValue(c => c.SourceCoverage, mapContent.SourceCoverage)
					.SetValue(c => c.ContentSize, mapContent.Content.Length)
					.SetValue(c => c.ContentType, mapContent.Content.GetType().AssemblyQualifiedName)
					.SetValue(c => c.ContentEncoding, "compressed")
					.SetValue(c => c.Content, compressedBuffer)
				;

			var pk = calcDbScope.Executor.Execute<IProjectMapContent_PK>(insertQuery);
			return pk.Id;
		}

		private static Type DefineMapStepDataType(ProjectMapType mapType)
		{
			if (mapType == ProjectMapType.Clutter || mapType == ProjectMapType.Building)
			{
				return typeof(byte);
			}
			else if (mapType == ProjectMapType.Relief)
			{
				return typeof(short);
			}
			throw new InvalidOperationException($"Unsupported the project map type '{mapType}'");
		}

		private long SaveSourceMap(IDataLayerScope calcDbScope, long contentId, DM.SourceMapData source)
		{
			var insertQuery = _calcServerDataLayer.GetBuilder<IProjectMapContentSource>()
					.Insert()
					.SetValue(c => c.CONTENT.Id, contentId)
					.SetValue(c => c.InfocMapId, source.MapId)
					.SetValue(c => c.InfocMapName, source.MapName)
					.SetValue(c => c.Coverage, source.CoveragePercent)
					.SetValue(c => c.UpperLeftX, source.CoverageUpperLeftX)
					.SetValue(c => c.UpperLeftY, source.CoverageUpperLeftY)
					.SetValue(c => c.LowerRightX, source.CoverageLowerRightX)
					.SetValue(c => c.LowerRightY, source.CoverageLowerRightY)
				;

			var pk = calcDbScope.Executor.Execute<IProjectMapContentSource_PK>(insertQuery);
			return pk.Id;
		}

		

		private DM.SourceMapData[] FindSourceMaps(IDataLayerScope infoDbScope, DM.ProjectMapData data, ProjectMapType mapType)
		{
			var sourceMapQuery = _infocenterDataLayer.GetBuilder<IC.IMap>()
				.From()
				.Select(
					c => c.Id,
					c => c.MapName,
					c => c.UpperLeftX,
					c => c.UpperLeftY,
					c => c.LowerRightX,
					c => c.LowerRightY,
					c => c.AxisXNumber,
					c => c.AxisXStep,
					c => c.AxisYNumber,
					c => c.AxisYStep,
					c => c.StepDataSize,
					c => c.CreatedDate,
					c => c.SectorsCount)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)IC.MapStatusCode.Available)
				.Where(c => c.TypeCode, ConditionOperator.Equal, (byte)mapType) // одного типа
				.Where(c => c.Projection, ConditionOperator.Equal, data.Projection) // одной проэкции
				.Where(c => c.StepUnit, ConditionOperator.Equal, data.StepUnit) // одной единицы измерения 
				.OrderByDesc(c => c.CreatedDate);

			return infoDbScope.Executor.ExecuteAndFetch(sourceMapQuery, reader =>
			{
				var sourceMaps = new List<DM.SourceMapData>();
				while (reader.Read())
				{
					var sourceMap = new DM.SourceMapData()
					{
						AxisXNumber = reader.GetValue(c => c.AxisXNumber),
						AxisXStep = reader.GetValue(c => c.AxisXStep),
						AxisYNumber = reader.GetValue(c => c.AxisXNumber),
						AxisYStep = reader.GetValue(c => c.AxisXNumber),
						UpperLeftX = reader.GetValue(c => c.UpperLeftX),
						UpperLeftY = reader.GetValue(c => c.UpperLeftY),
						LowerRightX = reader.GetValue(c => c.LowerRightX),
						LowerRightY = reader.GetValue(c => c.LowerRightY),
						MapId = reader.GetValue(c => c.Id),
						StepDataSize = reader.GetValue(c => c.StepDataSize),
						MapName = reader.GetValue(c => c.MapName),
						SectorsCount = reader.GetValue(c => c.SectorsCount)
					};

					if (sourceMap.IntersectWith(data))
					{
						sourceMaps.Add(sourceMap);
					}
				}
				return sourceMaps
					.OrderBy(c => c.Priority)
					//.ThenByDescending(c => c.CoveragePercent)
					.ThenByDescending(c => c.MapId)
					.ToArray(); 
			});
	
		}

		private void CheckAxis(DM.ProjectMapData data)
		{
			if (data.AxisXNumber <= 0)
			{
				throw new InvalidOperationException("Incorrect value of AxisXNumber");
			}
			if (data.AxisYNumber <= 0)
			{
				throw new InvalidOperationException("Incorrect value of AxisYNumber");
			}
			if (data.AxisXStep <= 0)
			{
				throw new InvalidOperationException("Incorrect value of AxisXStep");
			}
			if (data.AxisYStep <= 0)
			{
				throw new InvalidOperationException("Incorrect value of AxisYStep");
			}
		}
		private void CheckThresholdMaxSteps(int stepsNumber)
		{
			var thresholdMaxSteps = _config.ThresholdMaxSteps ?? 15000 * 15000;
			if (stepsNumber > thresholdMaxSteps)
			{
				throw new InvalidOperationException(
					$"Violation of the threshold value of the total number of steps. The threshold value is {thresholdMaxSteps}. The project map total number of steps is {stepsNumber}.");
			}
		}

		private  void CheckProjection(string projectionName)
		{
			if (string.IsNullOrEmpty(projectionName))
			{
				throw new InvalidOperationException("Undefined a projection");
			}

			if (projectionName.Length >= 4)
			{
				if ("4UTN".Equals(projectionName.SubString(4)))
				{
					return;
				}
			}

			throw new InvalidOperationException($"Unsupported the projection '{projectionName}'");
		}

		private void CheckStepUnit(string stepUnit)
		{
			if (string.IsNullOrEmpty(stepUnit))
			{
				throw new InvalidOperationException("Undefined a step unit");
			}

			if (stepUnit.Length == 1)
			{
				if ("M".Equals(stepUnit))
				{
					return;
				}
			}

			throw new InvalidOperationException($"Unsupported the step unit '{stepUnit}'");
		}

		private void CleanProjectMapContext(IDataLayerScope dbScope, long projectMapId)
		{
			var sourceDeleteQuery = _calcServerDataLayer.GetBuilder<IProjectMapContentSource>()
				.Delete()
				.Where(c => c.CONTENT.MAP.Id, ConditionOperator.Equal, projectMapId);
			var contentDeleteQuery = _calcServerDataLayer.GetBuilder<IProjectMapContent>()
				.Delete()
				.Where(c => c.MAP.Id, ConditionOperator.Equal, projectMapId);

			var sourceDelCount = dbScope.Executor.Execute(sourceDeleteQuery);
			var contentDelCount = dbScope.Executor.Execute(contentDeleteQuery);
		}

		private bool ChangeProjectMapStatus(IDataLayerScope dbScope, long projectMapId, ProjectMapStatusCode newStatus, string statusNote = null)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)newStatus)
				.SetValue(c => c.StatusName, newStatus.ToString())
				.SetValue(c => c.StatusNote, statusNote)
				.Where(c => c.Id, ConditionOperator.Equal, projectMapId);

			return dbScope.Executor.Execute(query) > 0;
		}
		private bool ChangeProjectMapStatus(IDataLayerScope dbScope, long projectMapId, ProjectMapStatusCode newStatus, ProjectMapStatusCode oldStatus , string statusNote = null)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)newStatus)
				.SetValue(c => c.StatusName, newStatus.ToString())
				.SetValue(c => c.StatusNote, statusNote)
				.Where(c => c.Id, ConditionOperator.Equal, projectMapId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)oldStatus);

			return dbScope.Executor.Execute(query) > 0;
		}

		private DM.ProjectMapData LoadProjectMapData(IDataLayerScope dbScope, long projectMapId)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.From()
				.Select(
					c => c.OwnerAxisXNumber,
					c => c.OwnerAxisXStep,
					c => c.OwnerAxisYNumber,
					c => c.OwnerAxisYStep,
					c => c.OwnerUpperLeftX,
					c => c.OwnerUpperLeftY,
					c => c.Projection,
					c => c.StepUnit)
				.Where(c => c.Id, ConditionOperator.Equal, projectMapId);

			return dbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				DM.ProjectMapData map = null;
				while (reader.Read())
				{
					map = new DM.ProjectMapData()
					{
						ProjectMapId = projectMapId,
						OwnerAxisXNumber = reader.GetValue(c => c.OwnerAxisXNumber),
						OwnerAxisXStep = reader.GetValue(c => c.OwnerAxisXStep),
						OwnerAxisYNumber = reader.GetValue(c => c.OwnerAxisXNumber),
						OwnerAxisYStep = reader.GetValue(c => c.OwnerAxisXNumber),
						OwnerUpperLeftX = reader.GetValue(c => c.OwnerUpperLeftX),
						OwnerUpperLeftY = reader.GetValue(c => c.OwnerUpperLeftY),
						Projection = reader.GetValue(c => c.Projection),
						StepUnit = reader.GetValue(c => c.StepUnit),
					};

				}
				return map;
			});
		}

		private bool SaveProjectMapData(IDataLayerScope dbScope, DM.ProjectMapData projectMap)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.Update()
				.SetValue(c => c.AxisXNumber, projectMap.AxisXNumber)
				.SetValue(c => c.AxisXStep, projectMap.AxisXStep)
				.SetValue(c => c.AxisYNumber, projectMap.AxisYNumber)
				.SetValue(c => c.AxisYStep, projectMap.AxisYStep)
				.SetValue(c => c.UpperLeftX, projectMap.UpperLeftX)
				.SetValue(c => c.UpperLeftY, projectMap.UpperLeftY)
				.SetValue(c => c.LowerRightX, projectMap.LowerRightX)
				.SetValue(c => c.LowerRightY, projectMap.LowerRightY)
				.Where(c => c.Id, ConditionOperator.Equal, projectMap.ProjectMapId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)ProjectMapStatusCode.Processing);

			return dbScope.Executor.Execute(query) > 0;
		}
	}
}
