﻿using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Test.Api.Sdrn.CalcServer.Client.DataModels;
using Atdi.Test.Api.Sdrn.CalcServer.Client.DTO;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.Test.Api.Sdrn.CalcServer.Client.Tasks;
using DM = Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.Infocenter.Entities.Stations;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client
{
	class Program
	{
		private static readonly string ClientInstance = "Atdi.Test.Api.Sdrn.CalcServer.Client: " + Guid.NewGuid().ToString();
		static HttpClient client = new HttpClient();

		static void Main(string[] args)
		{
			Console.WriteLine($"Press any key to start SDRN Calculation Server Client (AK) ...");
			Console.ReadLine();

			//CheckOrmBug();

			//TestWebApiOrm();
			//RunPointFieldStrengthCalcTask();


			client.BaseAddress = new Uri("http://localhost:15070/");
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));


			////LoadContextStation(10);

			////var projectId = CreateProject();



			////var stationId = CreateContextStation(clientContextId, "Station #0001", "CS: 000001128236263565");
			////Console.ReadLine();

			////var mapId = CreateProjectMap(
			////		projectId,
			////		"Main",
			////		"Львов: тестирования расчета профелей 2",
			////		new MapCoordinate
			////		{
			////			X = 276328,
			////			Y = 5532476
			////		},
			////		new MapAxis
			////		{
			////			Number = 4122,
			////			Step = 5
			////		},
			////		new MapAxis
			////		{
			////			Number = 3340,
			////			Step = 5
			////		}
			////	);

			////ChangeProjectMapStatus(mapId, 1, "Pending", "The status of waiting for map processing");
			////Console.WriteLine($"Created Project ID #{projectId}, Map Id #{mapId}. Press any key to change status");

			////Console.WriteLine($"Wait maps preparing ... Press any key to continue");
			////Console.ReadLine();

			//var projectId = 2;
			//var clientContextId = CreateClientContext(projectId);
			//var taskId = CreateCoverageProfilesCalcTask(projectId, @"C:\Temp\Maps\Profiles\Station.csv");
			//ChangeCalcTaskStatus(taskId, 2, "Available", "The status of available task for calculation");
			//var resultId = CreateCalcResult(clientContextId, taskId);

			//Console.WriteLine($"Created Client Context ID #{clientContextId}.");
			//Console.WriteLine($"Created Calc Task with ID #{taskId}.");
			//Console.WriteLine($"Created result task with ID #{taskId}. Press any key to change status");

			//Console.WriteLine($"Press any key to start calculation");
			//Console.ReadLine();

			//ChangeCalcResultStatus(resultId, 1, "Pending", "The status of waiting calculation result");

			//Console.WriteLine($"Wait calculation ... Press any key to show result");
			//Console.ReadLine();

			//var mapFile = LoadProjectMapContent(2);
			var sector1 = MapService.LoadMapSectorContent(43);
			var sector2 = MapService.LoadMapSectorContent(44);
			var sector3 = MapService.LoadMapSectorContent(45);
			var sector4 = MapService.LoadMapSectorContent(46);
			var sector5 = MapService.LoadMapSectorContent(47);
			var sector6 = MapService.LoadMapSectorContent(48);
			var sector7 = MapService.LoadMapSectorContent(49);
			var sector8 = MapService.LoadMapSectorContent(50);
			var sector9 = MapService.LoadMapSectorContent(51);
			MapMaker.Make(sector1, @"C:\Temp\Maps\Out", true );
			MapMaker.Make(sector2, @"C:\Temp\Maps\Out", true);
			MapMaker.Make(sector3, @"C:\Temp\Maps\Out", true);
			MapMaker.Make(sector4, @"C:\Temp\Maps\Out", true);
			MapMaker.Make(sector5, @"C:\Temp\Maps\Out", true);
			MapMaker.Make(sector6, @"C:\Temp\Maps\Out", true);
			MapMaker.Make(sector7, @"C:\Temp\Maps\Out", true);
			MapMaker.Make(sector8, @"C:\Temp\Maps\Out", true);
			MapMaker.Make(sector9, @"C:\Temp\Maps\Out", true);

			Console.ReadLine();
		}

		static void CheckOrmBug()
		{
			try
			{
				//var endpoint = new WebApiEndpoint(new Uri("http://localhost:15075/"), "/appserver/v1");
				//var dataContext = new WebApiDataContext("SDRN_Infocenter_DB");

				var endpoint = new WebApiEndpoint(new Uri("http://localhost:15070/"), "/appserver/v1");
				var dataContext = new WebApiDataContext("SDRN_CalcServer_DB");

				var dataLayer = new WebApiDataLayer(endpoint, dataContext);
				//var d = new DateTime();
				//var d2 = DateTime.SpecifyKind(d, DateTimeKind.Unspecified);
				//var query = dataLayer.GetBuilder<IIntegrationLog>()
				//	.Create();
				//	query.SetValue( c => c.CreatedDate, d)
				//	;

					var queryArgs = dataLayer.GetBuilder<IStationCalibrationArgs>()
						.Create()
						.SetValue(c => c.TASK.Id, 1000);
					dataLayer.Executor.Execute(queryArgs);

				//var reader = dataLayer.Executor.ExecuteReader(query);
				//if (reader.Read())
				//{
				//	var v1 = reader.GetValue(c => c.GsidCount);
				//	var v2 = reader.GetValue(c => c.StandardStats);
				//	var v3 = reader.GetValueAs<DriveTestStandardStats[]>(c => c.StandardStats);
				//}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				//throw;
			}
			
		}

		static void RunPointFieldStrengthCalcTask()
		{
			var endpoint = new WebApiEndpoint(new Uri("http://localhost:15070/"), "/appserver/v1");
			var dataContext = new WebApiDataContext("SDRN_CalcServer_DB");

			var dataLayer = new WebApiDataLayer();

			PointFieldStrengthCalcTask.Run(dataLayer, dataLayer.GetExecutor(endpoint, dataContext));
		}

		static void RunPointMyCalcTask()
		{
			var endpoint = new WebApiEndpoint(new Uri("http://localhost:15070/"), "/appserver/v1");
			var dataContext = new WebApiDataContext("SDRN_CalcServer_DB");

			var dataLayer = new WebApiDataLayer();

			PointFieldStrengthCalcTask.Run(dataLayer, dataLayer.GetExecutor(endpoint, dataContext));
		}

		static void TestWebApiOrm()
		{
			try
			{
				var endpoint = new WebApiEndpoint(new Uri("http://localhost:15070/"), "/appserver/v1");
				var dataContext = new WebApiDataContext("SDRN_CalcServer_DB");

				var dataLayer = new WebApiDataLayer(endpoint, dataContext);

				var webQuery = dataLayer.GetBuilder<DM.IProject>()
					.Read()
					.Select(c => c.Id)
					.Distinct()
					.OrderByDesc(c => c.Id)
					.Paginate(5, 5);

				var result = dataLayer.Executor.ExecuteAndRead(webQuery, reader =>
				{
					return reader.GetValue(c => c.Id);
				});

				for (int i = 0; i < result.Length; i++)
				{
					Console.WriteLine($" {i:d3} = {result[i]}");
				}
			}
			catch (EntityOrmWebApiException e)
			{
				Console.WriteLine(e);

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				
			}
			Console.ReadLine();
		}
		static long CreateProject()
		{
			var request = new DTO.RecordCreateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "Project",
				Fields = new string[]
				{
					"Name",
					"Note",
					"OwnerInstance",
					"OwnerProjectId",
					"StatusCode",
					"StatusName",
					"StatusNote",
					"Projection"
				},
				Values = new object []
				{
					$"Project: test map processing",
					"The project of the test map processing",
					Program.ClientInstance,
					Guid.NewGuid().ToString(),
					0,
					"Created",
					null,
					"4UTN35",
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
			// return project ID.
			return result.PrimaryKey.Id;
		}

		static long CreateProjectMap(long projectId, string mapName, string mapNote, MapCoordinate upperLeft, MapAxis xAxis, MapAxis yAxis)
		{
			var request = new DTO.RecordCreateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "ProjectMap",
				Fields = new string[]
				{
					"PROJECT.Id",
					"MapName",
					"MapNote",
					"OwnerInstance",
					"OwnerMapId",
					"StatusCode",
					"StatusName",
					"StatusNote",
					"StepUnit",
					"OwnerAxisXNumber",
					"OwnerAxisXStep",
					"OwnerAxisYNumber",
					"OwnerAxisYStep",
					"OwnerUpperLeftX",
					"OwnerUpperLeftY"
				},
				Values = new object[]
				{
					projectId,
					mapName,
					mapNote,
					Program.ClientInstance,
					Guid.NewGuid().ToString(),
					0,
					"Created",
					null,
					"M",
					xAxis.Number,
					xAxis.Step,
					yAxis.Number,
					yAxis.Step,
					upperLeft.X,
					upperLeft.Y
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
			// return project ID.
			return result.PrimaryKey.Id;
		}

		static long ChangeProjectMapStatus(long projectMapId, int statusCode, string statusName, string statusNote)
		{
			var request = new DTO.RecordUpdateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "ProjectMap",
				PrimaryKey = projectMapId.ToString(),
				Fields = new string[]
				{
					"StatusCode",
					"StatusName",
					"StatusNote"
				},
				Values = new object[]
				{
					statusCode,
					statusName,
					statusNote
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/update", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
			// return project ID.
			return 1;
		}


		static MapFile LoadProjectMapContent(long contentId)
		{
			var endpoint = new WebApiEndpoint(new Uri("http://localhost:15070/"), "/appserver/v1");
			var dataContext = new WebApiDataContext("SDRN_CalcServer_DB");

			var dataLayer = new WebApiDataLayer(endpoint, dataContext);

			var webQuery = dataLayer.GetBuilder<DM.IProjectMapContent>()
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


		static long CreateClientContext(long projectId)
		{
			var request = new DTO.RecordCreateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "ClientContext",
				Fields = new string[]
				{
					"PROJECT.Id",
					"OwnerInstance",
					"OwnerContextId"
				},
				Values = new object[]
				{
					projectId,
					Program.ClientInstance,
					Guid.NewGuid().ToString(),
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
			// return project ID.
			return result.PrimaryKey.Id;
		}

		static long CreateContextStation(long contextId, string name, string callSign)
		{
			var r = new Random();
			var request = new DTO.RecordCreateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "ContextStation",
				Fields = new string[]
				{
					"CONTEXT.Id",
					"Name",
					"CallSign",
					"TypeCode",
					"TypeName",

					"SITE.Longitude_DEC",
					"SITE.Latitude_DEC",
					"SITE.Altitude_m",

					"TRANSMITTER.Freq_MHz",
					"TRANSMITTER.BW_kHz",
					"TRANSMITTER.Loss_dB",
					"TRANSMITTER.MaxPower_dBm",
					"TRANSMITTER.PolarizingCode",
					"TRANSMITTER.PolarizingName",

					"RECEIVER.Freq_MHz",
					"RECEIVER.BW_kHz",
					"RECEIVER.Loss_dB",
					"RECEIVER.KTBF_dBm",
					"RECEIVER.Threshold_dBm",
					"RECEIVER.PolarizingCode",
					"RECEIVER.PolarizingName",

					"ANTENNA.Gain_dB",
					"ANTENNA.Tilt_deg",
					"ANTENNA.Azimuth_deg",
					"ANTENNA.XPD_dB",
					"ANTENNA.ItuPatternCode",
					"ANTENNA.ItuPatternName"
				},
				Values = new object[]
				{
					contextId,
					name,
					callSign,
					1,
					"A",

					r.NextDouble(),  // SITE.Longitude_DEC
					r.NextDouble(),  // SITE.Latitude_DEC
					r.NextDouble(),  // SITE.Altitude_m

					r.NextDouble(),  // TRANSMITTER.Freq_MHz
					r.NextDouble(),  // TRANSMITTER.BW_kHz
					r.NextDouble(),  // TRANSMITTER.Loss_dB
					r.NextDouble(),  // TRANSMITTER.MaxPower_dBm
					1,   //TRANSMITTER.PolarizingCode
					"V", //TRANSMITTER.PolarizingName

					r.NextDouble(),  // RECEIVER.Freq_MHz
					r.NextDouble(),  // RECEIVER.BW_kHz
					r.NextDouble(),  // RECEIVER.Loss_dB
					r.NextDouble(),  // RECEIVER.KTBF_dBm
					r.NextDouble(),  // RECEIVER.Threshold_dBm
					1,   //RECEIVER.PolarizingCode
					"H", //RECEIVER.PolarizingName

					r.NextDouble(),  // ANTENNA.Gain_dB
					r.NextDouble(),  // ANTENNA.Tilt_deg
					r.NextDouble(),  // ANTENNA.Azimuth_deg
					r.NextDouble(),  // ANTENNA.XPD_dB

					4,   //ANTENNA.ItuPatternCode
					"ITU1213"  //ANTENNA.ItuPatternName
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();

			CreateStationPattern(result.PrimaryKey.Id, "H", "H");
			CreateStationPattern(result.PrimaryKey.Id, "H", "V");
			CreateStationPattern(result.PrimaryKey.Id, "V", "H");
			CreateStationPattern(result.PrimaryKey.Id, "V", "V");

			// return project ID.
			return result.PrimaryKey.Id;
		}

		static object CreateStationPattern(long stationId,  string antennaPlane, string wavePlane)
		{
			var request = new DTO.RecordCreateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "ContextStationPattern",
				Fields = new string[]
				{
					"StationId",
					"AntennaPlane",
					"WavePlane",
					"Loss_dB",
					"Angle_deg"
				},
				Values = new object[]
				{
					stationId,
					antennaPlane,
					wavePlane,
					new float[]{1,2,3},
					new float[]{5,6,7},
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<ObjectPkRecordCreateResult>().GetAwaiter().GetResult();
			// return project ID.
			return result.PrimaryKey;
		}




		static void LoadContextStation(long stationId)
		{
			var r = new Random();
			var request = new DTO.RecordReadRequest()
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "ContextStation",
				PrimaryKey = stationId.ToString(),
				Select = new string[]
				{
					"Id",
					"CONTEXT.Id",
					"Name",
					"CallSign",
					"TypeCode",
					"TypeName",

					"SITE.Longitude_DEC",
					"SITE.Latitude_DEC",
					"SITE.Altitude_m",

					"TRANSMITTER.Freq_MHz",
					"TRANSMITTER.BW_kHz",
					"TRANSMITTER.Loss_dB",
					"TRANSMITTER.MaxPower_dBm",
					"TRANSMITTER.PolarizingCode",
					"TRANSMITTER.PolarizingName",

					"RECEIVER.Freq_MHz",
					"RECEIVER.BW_kHz",
					"RECEIVER.Loss_dB",
					"RECEIVER.KTBF_dBm",
					"RECEIVER.Threshold_dBm",
					"RECEIVER.PolarizingCode",
					"RECEIVER.PolarizingName",

					"ANTENNA.Gain_dB",
					"ANTENNA.Tilt_deg",
					"ANTENNA.Azimuth_deg",
					"ANTENNA.XPD_dB",
					"ANTENNA.ItuPatternCode",
					"ANTENNA.ItuPatternName",


					"ANTENNA.HH_PATTERN.Loss_dB",
					"ANTENNA.HH_PATTERN.Angle_deg",

					"ANTENNA.HV_PATTERN.Loss_dB",
					"ANTENNA.HV_PATTERN.Angle_deg",

					"ANTENNA.VH_PATTERN.Loss_dB",
					"ANTENNA.VH_PATTERN.Angle_deg",

					"ANTENNA.VV_PATTERN.Loss_dB",
					"ANTENNA.VV_PATTERN.Angle_deg"
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<RecordResult>().GetAwaiter().GetResult();

			

			// return project ID.
			return ;
		}


		static long CreateCoverageProfilesCalcTask(long projectId)
		{
			try
			{
				var request = new DTO.RecordCreateRequest
				{
					Context = "SDRN_CalcServer_DB",
					Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
					Entity = "CoverageProfilesCalcTask",
					Fields = new string[]
					{
						"TypeCode",
						"TypeName",
						"StatusCode",
						"StatusName",
						"StatusNote",
						"OwnerInstance",
						"OwnerTaskId",
						"PROJECT.Id",
						"MapName",
						"ModeCode",
						"ModeName",
						"PointsX",
						"PointsY",
						"ResultPath"
					},
					Values = new object[]
					{
						1,
						"CoverageProfilesCalc",
						0,
						"Created",
						null,
						Program.ClientInstance,
						Guid.NewGuid().ToString(),
						projectId,
						"Main",
						0,
						"InPairs",
						new int[] {  289460,  287135,  287135,  289460,    284115,  290540,  290540,  284115,   288580,  287555,  287555,  288580},
						new int[] { 5529044, 5525564, 5525564, 5529044,   5526779, 5525594, 5525594, 5526779,  5524024, 5524844, 5524844, 5524024},
						@"C:\Temp\Maps\Profiles"
					}
				};

				var response = client.PostAsJsonAsync(
					"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

				response.EnsureSuccessStatusCode();

				var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
				// return task ID.
				return result.PrimaryKey.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
		}

		static long CreateCoverageProfilesCalcTask(long projectId, string path)
		{
			try
			{
				var csv = File.ReadAllLines(path);
				var xArray = new int[csv.Length - 1];
				var yArray = new int[csv.Length - 1];

				for (int i = 1; i < csv.Length; i++)
				{
					var row = csv[i];
					var parts = row.Split(';');
					xArray[i - 1] = (int)Math.Round(double.Parse(parts[0]));
					yArray[i - 1] = (int)Math.Round(double.Parse(parts[1]));
				}

				var request = new DTO.RecordCreateRequest
				{
					Context = "SDRN_CalcServer_DB",
					Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
					Entity = "CoverageProfilesCalcTask",
					Fields = new string[]
					{
						"TypeCode",
						"TypeName",
						"StatusCode",
						"StatusName",
						"StatusNote",
						"OwnerInstance",
						"OwnerTaskId",
						"PROJECT.Id",
						"MapName",
						"ModeCode",
						"ModeName",
						"PointsX",
						"PointsY",
						"ResultPath"
					},
					Values = new object[]
					{
						1,
						"CoverageProfilesCalc",
						0,
						"Created",
						null,
						Program.ClientInstance,
						Guid.NewGuid().ToString(),
						projectId,
						"Main",
						2,
						"AllWithAll",
						xArray, // new int[] {  289460,  287135,  287135,  289460,    284115,  290540,  290540,  284115,   288580,  287555,  287555,  288580},
						yArray, // new int[] { 5529044, 5525564, 5525564, 5529044,   5526779, 5525594, 5525594, 5526779,  5524024, 5524844, 5524844, 5524024},
						@"C:\Temp\Maps\Profiles"
					}
				};

				var response = client.PostAsJsonAsync(
					"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

				response.EnsureSuccessStatusCode();

				var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
				// return task ID.
				return result.PrimaryKey.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}

		static long ChangeCalcTaskStatus(long taskId, int statusCode, string statusName, string statusNote)
		{
			var request = new DTO.RecordUpdateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "CalcTask",
				PrimaryKey = taskId.ToString(),
				Fields = new string[]
				{
					"StatusCode",
					"StatusName",
					"StatusNote"
				},
				Values = new object[]
				{
					statusCode,
					statusName,
					statusNote
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/update", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
			// return project ID.
			return result.Count;
		}


		static long CreateCalcResult(long contextId, long taskId)
		{
			var request = new DTO.RecordCreateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "CalcResult",
				Fields = new string[]
				{
					"StatusCode",
					"StatusName",
					"StatusNote",
					"CallerInstance",
					"CallerResultId",
					"CONTEXT.Id",
					"TASK.Id"
				},
				Values = new object[]
				{
					0,
					"Created",
					null,
					Program.ClientInstance,
					Guid.NewGuid().ToString(),
					contextId,
					taskId
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
			// return task ID.
			return result.PrimaryKey.Id;
		}

		static long ChangeCalcResultStatus(long resultId, int statusCode, string statusName, string statusNote)
		{
			var request = new DTO.RecordUpdateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "CalcResult",
				PrimaryKey = resultId.ToString(),
				Fields = new string[]
				{
					"StatusCode",
					"StatusName",
					"StatusNote"
				},
				Values = new object[]
				{
					statusCode,
					statusName,
					statusNote
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/update", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
			// return project ID.
			return result.Count;
		}
	}
}
