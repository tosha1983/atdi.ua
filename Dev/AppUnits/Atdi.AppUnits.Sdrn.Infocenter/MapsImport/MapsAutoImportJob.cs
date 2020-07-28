using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.Infocenter.DataModels;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.Infocenter;
using ES = Atdi.DataModels.Sdrn.Infocenter.Entities;
//using BR = Atdi.DataModels.Brific.Entities;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using Newtonsoft.Json;
using Atdi.DataModels.Sdrn.DeepServices.Gis.Maps;
using JSN = Atdi.DataModels.Sdrn.DeepServices.Gis.Maps.Json;

namespace Atdi.AppUnits.Sdrn.Infocenter
{
	internal class MapsAutoImportJob : IJobExecutor
	{
		private const int AtdiMapHeaderSize = 1010;
		private const int ContentPartSize = 1024 * 64;

		private readonly AppServerComponentConfig _config;
		//private readonly IDataLayer<EntityDataOrm<BR.BrificEntityOrmContext>> _brificDataLayer;
		private readonly ILogger _logger;
		private readonly IDataLayer<EntityDataOrm> _dataLayer;

		private readonly int _contentSizeLimit = 25 * 1024 * 1024;

		public MapsAutoImportJob(
			AppServerComponentConfig config,
			//IDataLayer<EntityDataOrm<BR.BrificEntityOrmContext>> brificDataLayer,
			IDataLayer<EntityDataOrm> dataLayer, 
			ILogger logger)
		{
			_config = config;
			//_brificDataLayer = brificDataLayer;
			_dataLayer = dataLayer;
			_logger = logger;
		}

//		private void TestBrific()
//		{
//			using (var dbScope = this._brificDataLayer.CreateScope<BrificDataContext>())
//			{
//				var query = this._brificDataLayer.GetBuilder<BR.Ifmtv_fdg>().From()
//					.Select(c => c.terrakey)
//					.Select(c => c.d_updated)
//					.Select(c => c.fdg_status)
//					.Select(c => c.fdg_type)
//					.Select(c => c.is_favorbl)
//					.Select(c => c.src_fdg);

//				var result = dbScope.Executor.ExecuteAndFetch(query, reader =>
//				{
//					var count = 0;
//					while (reader.Read())
//					{
//						++count;
//						var terrakey = reader.GetValue(c => c.terrakey);
//;
//					}

//					return count;
//				});


//			}
//		}

		public JobExecutionResult Execute(JobExecutionContext context)
		{
			//TestBrific();

			// сканируем каталог
			var folderName = this._config.AutoImportMapsFolder;
			if (string.IsNullOrEmpty(folderName))
			{
				_logger.Warning(Contexts.ThisComponent, Categories.MapsImport, "Undefined map directory in configuration");
				return JobExecutionResult.Canceled;
			}

			if (!Directory.Exists(folderName))
			{
				_logger.Warning(Contexts.ThisComponent, Categories.MapsImport, "Invalid map directory specified in configuration");
				return JobExecutionResult.Canceled;
			}

			// находим файлы
			var fileNames = Directory.EnumerateFiles(folderName, "*.*", SearchOption.TopDirectoryOnly)
				.Where(s => s.EndsWith(".geo", StringComparison.OrdinalIgnoreCase) 
			             || s.EndsWith(".blg", StringComparison.OrdinalIgnoreCase) 
			             || s.EndsWith(".sol", StringComparison.OrdinalIgnoreCase)
			             ).ToArray();
			//Directory.GetFiles(folderName, "*.geo,*.blg,*.sol", SearchOption.TopDirectoryOnly);

			if (fileNames != null && fileNames.Length > 0)
			{
				using (var dbScope = this._dataLayer.CreateScope<InfocenterDataContext>())
				{
					// импортируем
					foreach (var fileName in fileNames)
					{
						dbScope.BeginTran();
						try
						{
							var mapId = this.ImportMapFromFile(fileName, dbScope);

							// перемещаем в каталог обработки
							var movingFolder = Path.Combine(folderName, "Processed");
							if (!Directory.Exists(movingFolder))
							{
								Directory.CreateDirectory(movingFolder);
							}
							
							File.Move(fileName, Path.Combine(movingFolder, $"map_{mapId:D10}_" + Path.GetFileName(fileName)));

							if (".sol".Equals(Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase))
							{
								var clutterFileName = fileName + ".json";
								if (File.Exists(clutterFileName))
								{
									ImportClutterJson(clutterFileName, mapId, dbScope);
									File.Move(clutterFileName, Path.Combine(movingFolder, $"map_{mapId:D10}_" + Path.GetFileName(clutterFileName)));
								}
							}
							dbScope.Commit();
						}
						catch (Exception e)
						{
							_logger.Exception(Contexts.ThisComponent, Categories.MapsImport, e);
							dbScope.Rollback();
						}

					}
				}
			}


			// сканируем на факт наличия дефолтного описания клатеров
			var defaultCluttersDescFileName = Path.Combine(folderName, "DefaultCluttersDesc.json");
			if (File.Exists(defaultCluttersDescFileName))
			{
				using (var dbScope = this._dataLayer.CreateScope<InfocenterDataContext>())
				{
					dbScope.BeginTran();
					try
					{
						// перемещаем в каталог обработки
						var movingFolder = Path.Combine(folderName, "Processed");
						if (!Directory.Exists(movingFolder))
						{
							Directory.CreateDirectory(movingFolder);
						}

						var descId = ImportClutterJson(defaultCluttersDescFileName, null, dbScope);
						File.Move(defaultCluttersDescFileName, Path.Combine(movingFolder, $"cdesc_{descId:D10}_" + Path.GetFileName(defaultCluttersDescFileName)));
						dbScope.Commit();
					}
					catch (Exception e)
					{
						_logger.Exception(Contexts.ThisComponent, Categories.ClutterImport, e);
						dbScope.Rollback();
					}
				}
			}
			return JobExecutionResult.Completed;
		}

		private long ImportClutterJson(string path, long? mapId, IDataLayerScope dbScope)
		{
			try
			{
				var cluttersDesc = JsonConvert.DeserializeObject<JSN.CluttersDesc>(File.ReadAllText(path));

				var descQuery = _dataLayer.GetBuilder<ES.ICluttersDesc>()
					.Insert()
					.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
					.SetValue(c => c.Name, cluttersDesc.Name)
					.SetValue(c => c.Note, cluttersDesc.Note)
					;
				if (mapId.HasValue)
				{
					descQuery.SetValue(c => c.MAP.Id, mapId.Value);
				}
				var descPk = dbScope.Executor.Execute<ES.ICluttersDesc_PK>(descQuery);

				for (var i = 0; i < cluttersDesc.Clutters.Length; i++)
				{
					var clutter = cluttersDesc.Clutters[i];
					var clutterQuery = _dataLayer.GetBuilder<ES.ICluttersDescClutter>()
							.Insert()
							.SetValue(c => c.CLUTTERS_DESC.Id, descPk.Id)
							.SetValue(c => c.Code, clutter.Code)
							.SetValue(c => c.Name, clutter.Name)
							.SetValue(c => c.Note, clutter.Note)
							.SetValue(c => c.Height_m, clutter.Height_m)
						;
					dbScope.Executor.Execute(clutterQuery);
				}
				for (var i = 0; i < cluttersDesc.Frequencies.Length; i++)
				{
					var freq = cluttersDesc.Frequencies[i];
					var freqQuery = _dataLayer.GetBuilder<ES.ICluttersDescFreq>()
							.Insert()
							.SetValue(c => c.CLUTTERS_DESC.Id, descPk.Id)
							.SetValue(c => c.Freq_MHz, freq.Freq_MHz)
							.SetValue(c => c.Note, freq.Note)
						;
					var freqPk = dbScope.Executor.Execute<ES.ICluttersDescFreq_PK>(freqQuery);
					for (var j = 0; j < freq.Clutters.Length; j++)
					{
						var clutter = freq.Clutters[j];
						var clutterQuery = _dataLayer.GetBuilder<ES.ICluttersDescFreqClutter>()
								.Insert()
								.SetValue(c => c.FREQ.Id, freqPk.Id)
								.SetValue(c => c.Code, clutter.Code)
								.SetValue(c => c.Note, clutter.Note)
								.SetValue(c => c.FlatLoss_dB, (float?)clutter.FlatLoss_dB)
								.SetValue(c => c.LinearLoss_dBkm, (float?)clutter.LinearLoss_dBkm)
								.SetValue(c => c.Reflection, (float?)clutter.Reflection)
							;
						dbScope.Executor.Execute(clutterQuery);
					}
				}
				return descPk.Id;
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.ClutterImport, e, this);
				_logger.Error(Contexts.ThisComponent, Categories.ClutterImport, $"An error occurred while importing the clutter description from the file {path} (Map ID #{mapId})");
				throw;
			}
			
		}

		private long ImportMapFromFile(string path, IDataLayerScope dbScope)
		{
			using (var stream = File.OpenRead(path))
			{
				var mapFile = this.DecodeMapFile(stream, Path.GetFileName(path));

				// сохраняем методаданые о карте
				var mapQuery = _dataLayer.GetBuilder<ES.IMap>()
					.Insert()
					.SetValue(c => c.FileName, mapFile.FileName)
					.SetValue(c => c.FileSize, (int)stream.Length)
					.SetValue(c => c.ContentSource, "File")
					.SetValue(c => c.ContentSize, mapFile.ContentRealSize)
					.SetValue(c => c.StatusCode, (byte)1)
					.SetValue(c => c.StatusName, "Created")
					.SetValue(c => c.MapNote, mapFile.Info)
					.SetValue(c => c.TypeCode, (byte)mapFile.MapType)
					.SetValue(c => c.TypeName, mapFile.MapType.ToString())
					.SetValue(c => c.Projection, mapFile.Projection)
					.SetValue(c => c.StepUnit, mapFile.StepUnit)
					.SetValue(c => c.StepDataType, mapFile.StepDataType.Name)
					.SetValue(c => c.StepDataSize, mapFile.StepDataSize)
					.SetValue(c => c.AxisXNumber, mapFile.AxisX.Number)
					.SetValue(c => c.AxisXStep, mapFile.AxisX.Step)
					.SetValue(c => c.AxisYNumber, mapFile.AxisY.Number)
					.SetValue(c => c.AxisYStep, mapFile.AxisY.Step)
					.SetValue(c => c.UpperLeftX, mapFile.Coordinates.UpperLeft.X)
					.SetValue(c => c.UpperLeftY, mapFile.Coordinates.UpperLeft.Y)
					.SetValue(c => c.UpperRightX, mapFile.Coordinates.UpperRight.X)
					.SetValue(c => c.UpperRightY, mapFile.Coordinates.UpperRight.Y)
					.SetValue(c => c.LowerLeftX, mapFile.Coordinates.LowerLeft.X)
					.SetValue(c => c.LowerLeftY, mapFile.Coordinates.LowerLeft.Y)
					.SetValue(c => c.LowerRightX, mapFile.Coordinates.LowerRight.X)
					.SetValue(c => c.LowerRightY, mapFile.Coordinates.LowerRight.Y)
					.SetValue(c => c.MapName, mapFile.ToString())
					;

				var mapPk = dbScope.Executor.Execute<ES.IMap_PK>(mapQuery);


				// дробим ее на сегменты по 25 мегабайт

				
				if (mapFile.ContentRealSize <= this._contentSizeLimit)
				{
					var section = new AtdiMapSector()
					{
						MapId = mapPk.Id,
						AxisXIndex = 1,
						AxisYIndex = 1,
						AxisXNumber = mapFile.AxisX.Number,
						AxisYNumber = mapFile.AxisY.Number,
						Coordinates = mapFile.Coordinates,
						Content = new AtdiMapContent()
						{
							Type = typeof(byte[]).AssemblyQualifiedName
						}
					};
					var buffer = new byte[mapFile.StepRealNumber];
					stream.Position = AtdiMapHeaderSize;
					var realNumber = stream.Read(buffer, 0, buffer.Length);
					if (realNumber != buffer.Length)
					{
						throw new InvalidOperationException("Something went wrong");
					}

					section.Content.Data = buffer;
					section.Content.Size = buffer.Length;

					this.SaveSectorToDb(dbScope, section);
					this.SaveMapStatisticsAndMakeAvailable(dbScope, mapPk.Id, 1, 1);
				}
				else
				{
					var stepsLimit = this._contentSizeLimit / mapFile.StepDataSize;

					var a = (decimal)mapFile.AxisX.Number / (decimal) mapFile.AxisY.Number;

					var yNumberPerStep = (int)Math.Sqrt((double)(stepsLimit / a));
					var xNumberPerStep = (int) (a * (decimal)yNumberPerStep);

					if (yNumberPerStep * xNumberPerStep > stepsLimit)
					{
						throw new InvalidOperationException("Something went wrong");
					}
					var xk = mapFile.AxisX.Number / xNumberPerStep;
					var yk = mapFile.AxisY.Number / yNumberPerStep;

					// Расчет коэфициента дробления на сегменты
					//var k = (int)Math.Round((decimal)mapFile.StepRealNumber / ( (decimal)stepsLimit), MidpointRounding.AwayFromZero);

					//var xNumberPerStep = mapFile.AxisX.Number / k;
					//var yNumberPerStep = mapFile.AxisY.Number / k;

					// фактически матрица сегментов
					var xSectorSteps = new int[xk + 1];
					var ySectorSteps = new int[yk + 1];

					// фикисруем общее кол-во шагов по X и Y
					var xRest = mapFile.AxisX.Number;
					var yRest = mapFile.AxisY.Number;

					var xSectorCount = 0;
					var ySectorCount = 0;
					// заполняем матрицу
					for (int i = 0; i < xk; i++)
					{
						xSectorSteps[i] = xNumberPerStep;
						//ySectorSteps[i] = yNumberPerStep;
						xRest -= xNumberPerStep;
						//yRest -= yNumberPerStep;
						++xSectorCount;
						//++ySectorCount;
					}
					for (int i = 0; i < yk; i++)
					{
						//xSectorSteps[i] = xNumberPerStep;
						ySectorSteps[i] = yNumberPerStep;
						//xRest -= xNumberPerStep;
						yRest -= yNumberPerStep;
						//++xSectorCount;
						++ySectorCount;
					}

					// возможно остаток еще можно прибавить в конечный сегмент 
					if (xRest > 0 &&
					    yRest > 0 &&
					    ((xRest * yRest) + (xNumberPerStep * yNumberPerStep)) <= stepsLimit)
					{
						xSectorSteps[xk - 1] = xNumberPerStep + xRest;
						xRest = 0;

						ySectorSteps[yk - 1] = yNumberPerStep + yRest;
						yRest = 0;
					}

					// если еще есть остаток  - это все идет в "крайний" сегмент
					if (xRest > 0)
					{
						xSectorSteps[xk] = xRest;
						++xSectorCount;
					}

					if (yRest > 0)
					{
						ySectorSteps[yk] = yRest;
						++ySectorCount;
					}

					// по Y
					var upperleftX = mapFile.Coordinates.UpperLeft.X;
					var upperleftY = mapFile.Coordinates.UpperLeft.Y;
					var filePosition = AtdiMapHeaderSize;
					var xFileOffset = 0;
					var yFileOffset = 0;
					for (int ySegmentIndex = 0; ySegmentIndex < ySectorCount; ySegmentIndex++)
					{
						for (int xSegmentIndex = 0; xSegmentIndex < xSectorCount; xSegmentIndex++)
						{
							var buffer = new byte[xSectorSteps[xSegmentIndex] * ySectorSteps[ySegmentIndex] * mapFile.StepDataSize];
							var sector = new AtdiMapSector()
							{
								AxisXIndex = xSegmentIndex + 1,
								AxisYIndex = ySegmentIndex + 1,
								AxisXNumber = xSectorSteps[xSegmentIndex],
								AxisYNumber = ySectorSteps[ySegmentIndex],
								MapId = mapPk.Id,
								Content = new AtdiMapContent()
								{
									Type = typeof(byte[]).AssemblyQualifiedName,
									Data = buffer,
									Size = buffer.Length
								},
								Coordinates = new AtdiMapCoordinates()
								{
									UpperLeft = new AtdiMapCoordinate()
									{
										X = upperleftX,
										Y = upperleftY
									},
									UpperRight = new AtdiMapCoordinate()
									{
										X = upperleftX + xSectorSteps[xSegmentIndex] * mapFile.AxisX.Step,
										Y = upperleftY
									},
									LowerLeft = new AtdiMapCoordinate()
									{
										X = upperleftX,
										Y = upperleftY - ySectorSteps[ySegmentIndex] * mapFile.AxisY.Step
									},
									LowerRight = new AtdiMapCoordinate()
									{
										X = upperleftX + xSectorSteps[xSegmentIndex] * mapFile.AxisX.Step,
										Y = upperleftY - ySectorSteps[ySegmentIndex] * mapFile.AxisY.Step
									}
								}
							};
							//  смещаем координаты по X
							upperleftX += xSectorSteps[xSegmentIndex] * mapFile.AxisX.Step;

							// тут загрузка буфера
							
							var len = 0;
							for (int rowIndex = 0; rowIndex < ySectorSteps[ySegmentIndex]; rowIndex++)
							{
								stream.Position = filePosition + yFileOffset + xFileOffset + rowIndex * mapFile.AxisX.Number * mapFile.StepDataSize;
								len += stream.Read(buffer, len, xSectorSteps[xSegmentIndex] * mapFile.StepDataSize);
							}

							// смещаемся в буфере
							xFileOffset += xSectorSteps[xSegmentIndex] * mapFile.StepDataSize;

							// сохраняем сегмент
							this.SaveSectorToDb(dbScope, sector);
						}

						// возвращаем X  в начальную позицию и смещаем координаты по Y
						upperleftX = mapFile.Coordinates.UpperLeft.X;
						upperleftY -= ySectorSteps[ySegmentIndex] * mapFile.AxisY.Step;

						// смещаемся в буфере
						//yFileOffset += (ySegmentIndex + 1) * mapFile.AxisX.Number * mapFile.StepDataSize * ySectorSteps[ySegmentIndex];
						yFileOffset +=  mapFile.AxisX.Number * mapFile.StepDataSize * ySectorSteps[ySegmentIndex];
					}

					this.SaveMapStatisticsAndMakeAvailable(dbScope, mapPk.Id, xSectorCount, ySectorCount);
				}
				return mapPk.Id;
			}
		}

		private void SaveSectorToDb(IDataLayerScope dbScope, AtdiMapSector sector)
		{
			var compressedData = Compressor.Compress(sector.Content.Data);

			var mapQuery = _dataLayer.GetBuilder<ES.IMapSector>()
					.Insert()
					.SetValue(c => c.MAP.Id, sector.MapId)
					.SetValue(c => c.AxisXNumber, sector.AxisXNumber)
					.SetValue(c => c.AxisYNumber, sector.AxisYNumber)
					.SetValue(c => c.ContentSize, sector.Content.Size)
					.SetValue(c => c.ContentType, sector.Content.Type)
					.SetValue(c => c.ContentEncoding, "compressed")
					.SetValue(c => c.Content, compressedData)
					.SetValue(c => c.UpperLeftX, sector.Coordinates.UpperLeft.X)
					.SetValue(c => c.UpperLeftY, sector.Coordinates.UpperLeft.Y)
					.SetValue(c => c.UpperRightX, sector.Coordinates.UpperRight.X)
					.SetValue(c => c.UpperRightY, sector.Coordinates.UpperRight.Y)
					.SetValue(c => c.LowerLeftX, sector.Coordinates.LowerLeft.X)
					.SetValue(c => c.LowerLeftY, sector.Coordinates.LowerLeft.Y)
					.SetValue(c => c.LowerRightX, sector.Coordinates.LowerRight.X)
					.SetValue(c => c.LowerRightY, sector.Coordinates.LowerRight.Y)
					.SetValue(c => c.AxisXIndex, sector.AxisXIndex)
					.SetValue(c => c.AxisYIndex, sector.AxisYIndex)
					.SetValue(c => c.SectorName, sector.ToString())
				;

			var sectionPk = dbScope.Executor.Execute<ES.IMapSector_PK>(mapQuery);
		}

		private AtdiMapFile DecodeMapFile(FileStream stream, string fileName)
		{
			

			var buffer = new byte[AtdiMapHeaderSize];
			var realLength = stream.Read(buffer, 0, buffer.Length);
			if (realLength != buffer.Length)
			{
				throw new InvalidOperationException($"Invalid map file '{fileName}' size. Expected minimum of {buffer.Length} bytes.");
			}

			var map = DecodeMap(buffer, fileName);
			//map.Min = int.MaxValue;
			//map.Max = int.MinValue;

			var length = 0;
			var content = new byte[ContentPartSize];
			realLength = content.Length;
			//map.Statistics = new int[256];
			while (realLength == content.Length)
			{
				realLength = stream.Read(content, 0, content.Length);
				length += realLength;

				//for (int i = 0; i < realLength; i++)
				//{
				//	var val = content[i];
				//	if (map.Max < val)
				//	{
				//		map.Max = val;
				//	}

				//	if (map.Min > val)
				//	{
				//		map.Min = val;
				//	}

				//	map.Statistics[val] = map.Statistics[val] + 1;
				//}
			}

			map.ContentRealSize = length;
			if (map.ContentRealSize != map.ContentCalcSize)
			{
				throw new InvalidOperationException($"Invalid map file '{fileName}'. Expected content size of {map.ContentCalcSize} bytes.");
			}
			if (map.StepRealNumber != map.StepCalcNumber)
			{
				throw new InvalidOperationException($"Invalid map file '{fileName}'. Expected number of {map.StepCalcNumber} steps.");
			}
			return map;
		}

		private static AtdiMapFile DecodeMap(byte[] body, string fileName)
		{
			var mapType = DefineMapType(fileName);
			var map = new AtdiMapFile()
			{
				FileName = fileName,
				MapType = mapType,
				StepDataType = DefineMapStepDataType(mapType)
			};
			const uint CoordinatesUpperLeftXOffset = 160;
			const uint CoordinatesUpperLeftYOffset = 175;
			const uint CoordinatesUpperRightXOffset = 190;
			const uint CoordinatesUpperRightYOffset = 205;

			const uint CoordinatesLowerLeftXOffset = 220;
			const uint CoordinatesLowerLeftYOffset = 235;
			const uint CoordinatesLowerRightXOffset = 250;
			const uint CoordinatesLowerRightYOffset = 265;

			const uint CoordinatesSize = 15;

			// required properties
			map.Coordinates.UpperLeft.X = DecodeInt(body, CoordinatesUpperLeftXOffset, CoordinatesSize);
			map.Coordinates.UpperLeft.Y = DecodeInt(body, CoordinatesUpperLeftYOffset, CoordinatesSize);

			map.AxisX.Step = DecodeInt(body, 280, 10);
			map.AxisY.Step = DecodeInt(body, 290, 10);

			map.AxisX.Number = DecodeInt(body, 320, 10);
			map.AxisY.Number = DecodeInt(body, 330, 10);

			map.Projection = DecodeProjectionString(body, 340, 8);

			// not required properties

			// UpperRight
			var notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesUpperRightXOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				map.Coordinates.UpperRight.X = notRequiredIntValue.Value;
			}
			else
			{
				map.Coordinates.UpperRight.X = map.Coordinates.UpperLeft.X + map.AxisX.Step * map.AxisX.Number;
			}
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesUpperRightYOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				map.Coordinates.UpperRight.Y = notRequiredIntValue.Value;
			}
			else
			{
				map.Coordinates.UpperRight.Y = map.Coordinates.UpperLeft.Y;
			}

			// LowerLeft
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesLowerLeftXOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				map.Coordinates.LowerLeft.X = notRequiredIntValue.Value;
			}
			else
			{
				map.Coordinates.LowerLeft.X = map.Coordinates.UpperLeft.X;
			}
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesLowerLeftYOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				map.Coordinates.LowerLeft.Y = notRequiredIntValue.Value;
			}
			else
			{
				map.Coordinates.LowerLeft.Y = map.Coordinates.UpperLeft.Y - map.AxisY.Step * map.AxisY.Number;
			}

			// LowerRight
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesLowerRightXOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				map.Coordinates.LowerRight.X = notRequiredIntValue.Value;
			}
			else
			{
				map.Coordinates.LowerRight.X = map.Coordinates.UpperRight.X;
			}
			notRequiredIntValue = DecodeIntAsNotRequired(body, CoordinatesLowerRightYOffset, CoordinatesSize);
			if (notRequiredIntValue.HasValue)
			{
				map.Coordinates.LowerRight.Y = notRequiredIntValue.Value;
			}
			else
			{
				map.Coordinates.LowerRight.Y = map.Coordinates.LowerLeft.Y;
			}
				

			

			map.StepUnit = DecodeString(body, 300, 1);
			// без указания единици измерения карты считаем ее "в метрах"
			if (string.IsNullOrEmpty(map.StepUnit))
			{
				map.StepUnit = "M";
			}

			
			map.Info = DecodeString(body, 0, 160);
			//map.Min = DecodeInt(body, 380, 10);
			//map.Max = DecodeInt(body, 390, 10);

			// валидация
			ValidateMap(map);

			// Карту нужно сдивнуть в лево и вверх на step/2
			var xOffset = map.AxisX.Step / 2;
			var yOffset = map.AxisY.Step / 2;

			map.Coordinates.UpperLeft.X -= xOffset;
			map.Coordinates.UpperRight.X -= xOffset;
			map.Coordinates.LowerLeft.X -= xOffset;
			map.Coordinates.LowerRight.X -= xOffset;
			map.Coordinates.UpperLeft.Y += yOffset;
			map.Coordinates.UpperRight.Y += yOffset;
			map.Coordinates.LowerLeft.Y += yOffset;
			map.Coordinates.LowerRight.Y += yOffset;

			// повторная валидация после сдвига
			ValidateMap(map);

			return map;
		}

		private static void ValidateMap(AtdiMapFile map)
		{
			if (map.Coordinates.UpperLeft.Y != map.Coordinates.UpperRight.Y)
			{
				throw new InvalidDataException("Incorrect coordinates for the upper axis y");
			}

			if (map.Coordinates.LowerLeft.Y != map.Coordinates.LowerRight.Y)
			{
				throw new InvalidDataException("Incorrect coordinates for the lower axis y");
			}

			if (map.Coordinates.UpperLeft.X != map.Coordinates.LowerLeft.X)
			{
				throw new InvalidDataException("Incorrect coordinates for the left axis X");
			}

			if (map.Coordinates.UpperRight.X != map.Coordinates.LowerRight.X)
			{
				throw new InvalidDataException("Incorrect coordinates for the right axis X");
			}


			if (map.Coordinates.UpperLeft.X > map.Coordinates.UpperRight.X)
			{
				throw new InvalidDataException("Incorrect coordinates for the upper axis X");
			}

			if (map.Coordinates.LowerLeft.X > map.Coordinates.LowerRight.X)
			{
				throw new InvalidDataException("Incorrect coordinates for the lower axis X");
			}

			if (map.Coordinates.UpperLeft.Y < map.Coordinates.LowerLeft.Y)
			{
				throw new InvalidDataException("Incorrect coordinates for the left axis y");
			}

			if (map.Coordinates.UpperRight.Y < map.Coordinates.LowerRight.Y)
			{
				throw new InvalidDataException("Incorrect coordinates for the right axis y");
			}

			if (map.AxisX.Size != map.Coordinates.AxisX)
			{
				throw new InvalidDataException("Incorrect X-coordinates or X-axis properties");
			}

			if (map.AxisY.Size != map.Coordinates.AxisY)
			{
				throw new InvalidDataException("Incorrect Y-coordinates or Y-axis properties");
			}
		}


        //private static float DecodeFloat(byte[] source, uint offset, uint size)
        //{
        //	var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        //	return float.Parse(statement);
        //}
        //private static int DecodeInt(byte[] source, uint offset, uint size)
        //{
        //	var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        //	return Convert.ToInt32((decimal.Parse(statement)));
        //}
        private static float DecodeFloat(byte[] source, uint offset, uint size)
        {
            var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            try
            {
                return float.Parse(statement);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot decode source as float: '{statement}', NumberDecimalSeparator='{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}'", e);
            }

        }
        private static int DecodeInt(byte[] source, uint offset, uint size)
        {
            var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            try
            {
                return Convert.ToInt32((decimal.Parse(statement)));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot decode source as int: '{statement}', NumberDecimalSeparator='{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}'", e);
            }
        }

        private static int? DecodeIntAsNotRequired(byte[] source, uint offset, uint size)
        {
	        var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
	        try
	        {
		        if (string.IsNullOrEmpty(statement))
		        {
			        return null;
		        }
		        return Convert.ToInt32((decimal.Parse(statement)));
	        }
	        catch (Exception e)
	        {
		        throw new InvalidOperationException($"Cannot decode source as int: '{statement}', NumberDecimalSeparator='{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}'", e);
	        }
        }

		private static string DecodeString(byte[] source, uint offset, uint size)
		{
			var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size).Replace("\0", "");
			return statement;
		}

		private static string DecodeProjectionString(byte[] source, uint offset, uint size)
		{
			var statement = Encoding.ASCII.GetString(source, (int)offset, (int)size); //.Replace("\0", "");
			var result = new StringBuilder();
			foreach (var c in statement)
			{
				if (c == '\0')
				{
					return result.ToString();
				}

				result.Append(c);
			}
			return result.ToString();
		}

		private static Type DefineMapStepDataType(AtdiMapType mapType)
		{
			if (mapType == AtdiMapType.Clutter || mapType == AtdiMapType.Building)
			{
				return typeof(byte);
			}
			else if (mapType == AtdiMapType.Relief)
			{
				return typeof(short);
			}
			throw new InvalidOperationException($"Unsupported the map type '{mapType}'");
		}

		private static AtdiMapType DefineMapType(string fileName)
		{
			var ext = Path.GetExtension(fileName);
			if (".geo".Equals(ext, StringComparison.OrdinalIgnoreCase))
			{
				return AtdiMapType.Relief;
			}
			if (".sol".Equals(ext, StringComparison.OrdinalIgnoreCase))
			{
				return AtdiMapType.Clutter;
			}
			if (".blg".Equals(ext, StringComparison.OrdinalIgnoreCase))
			{
				return AtdiMapType.Building;
			}
			throw new InvalidOperationException($"Unsupported file extension '{ext}'");
		}

		private bool SaveMapStatisticsAndMakeAvailable(IDataLayerScope dbScope, long mapId, int sectorsXCount, int sectorsYCount)
		{
			var mapQuery = _dataLayer.GetBuilder<ES.IMap>()
					.Update()
					.SetValue(c => c.SectorsCount, sectorsXCount * sectorsYCount)
					.SetValue(c => c.SectorsXCount, sectorsXCount)
					.SetValue(c => c.SectorsYCount, sectorsYCount)
					.SetValue(c => c.StatusCode, (byte)ES.MapStatusCode.Available)
					.SetValue(c => c.StatusName, "Available")
					.Where(c => c.Id, ConditionOperator.Equal, mapId)
				;

			return dbScope.Executor.Execute(mapQuery) > 0;
		}
	}
}
