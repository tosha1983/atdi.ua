using Atdi.AppUnits.Sdrn.Infocenter.Integration.FilesImport;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.DataModels.Sdrn.Infocenter.Entities.Stations;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.Stations
{
	internal class CsvReader : IDisposable
	{
		private readonly string _fileName;
		private readonly char _delimiter;
		private readonly StreamReader _reader;
		private readonly string[] _fields;
		private readonly Dictionary<string, int> _indexes;
		private string[] _values;

		public CsvReader(string fileName, char delimiter)
		{
			_fileName = fileName;
			_delimiter = delimiter;
			_reader = File.OpenText(_fileName);
			var headerLine = _reader.ReadLine();
			if (headerLine != null)
			{
				this._fields = headerLine.Split(_delimiter);
				_indexes = new Dictionary<string, int>(_fields.Length);
				for (var i = 0; i < this._fields.Length; i++)
				{
					_indexes[_fields[i]] = i;
				}
			}
			
		}

		public string[] Fields => _fields.ToArray();

		public bool Contains(string[] fields)
		{
			foreach (var field in fields)
			{
				if (!_indexes.ContainsKey(field))
				{
					return false;
				}
			}

			return true;
		}

		public bool Contains(string field)
		{
			return _indexes.ContainsKey(field);
		}

		public string GetValue(string field)
		{
			return _values[_indexes[field]];
		}

		public string GetValue(int index)
		{
			return _values[index];
		}

		public bool Read()
		{
			if (_fields == null)
			{
				return false;
			}

			var line = _reader.ReadLine();
			if (line == null)
			{
				_values = null;
				return false;
			}

			_values = line.Split(_delimiter);
			return true;
		}

		public void Dispose()
		{
			_reader.Close();
		}

		
	}
	internal class GlobalIdentityPipelineHandler : IPipelineHandler<ImportFileInfo, ImportFileResult>
	{
		private readonly IIntegrationService _integrationService;
		private readonly IDataLayer<EntityDataOrm> _dataLayer;
		private readonly ILogger _logger;
		private const string LicenseGsid = "LicenceGSID";
		private const string RealGsid = "RealGSID";
		private const string Standard = "Standard";
		private const string Region = "Region";


		public GlobalIdentityPipelineHandler(IIntegrationService integrationService, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
		{
			_integrationService = integrationService;
			_dataLayer = dataLayer;
			_logger = logger;
		}

		public ImportFileResult Handle(ImportFileInfo data, IPipelineContext<ImportFileInfo, ImportFileResult> context)
		{
			if (!"GlobalIdentity.csv".Equals(data.Name, StringComparison.OrdinalIgnoreCase))
			{
				return context.GoAhead(data);
			}

			var result = new ImportFileResult
			{
				Status = ImportFileResultStatus.Refused
			};

			try
			{
				using (var reader = new CsvReader(data.Path, ';'))
				{
					var error = new StringBuilder();

					CheckField(reader, LicenseGsid, error);
					CheckField(reader, RealGsid, error);
					CheckField(reader, Standard, error);
					CheckField(reader, Region, error);

					if (error.Length > 0)
					{
						result.Status = ImportFileResultStatus.Refused;
						result.Message = error.ToString();
						_logger.Error(Contexts.ThisComponent, "ImportFile", "Incorrect structure in the file with name 'GlobalIdentity.csv'");

						return result;
					}

					var token = _integrationService.Start("CSV-file", "GlobalIdentity");

					//  local statistics
					var row = 0;
					var importedCount = 0;
					var insertedCount = 0;
					var updatedCount = 0;
					var errorCount = 0;
					try
					{
						using (var dbScope = this._dataLayer.CreateScope<InfocenterDataContext>())
						{
							while (reader.Read())
							{
								++row;
								try
								{
									var licenseGsid = reader.GetValue(LicenseGsid);
									var realGsid = reader.GetValue(RealGsid);
									var standard = reader.GetValue(Standard);
									var region = reader.GetValue(Region);

									if (string.IsNullOrEmpty(licenseGsid))
									{
										throw new InvalidOperationException($"Undefined field value with name '{LicenseGsid}'");
									}
									if (string.IsNullOrEmpty(realGsid))
									{
										throw new InvalidOperationException($"Undefined field value with name '{RealGsid}'");
									}
									if (string.IsNullOrEmpty(standard))
									{
										throw new InvalidOperationException($"Undefined field value with name '{Standard}'");
									}

									var findQuery = _dataLayer.GetBuilder<IGlobalIdentity>()
										.From()
										.Select(c => c.LicenseGsid)
										.Where(c => c.LicenseGsid, ConditionOperator.Equal, licenseGsid)
										.Where(c => c.RegionCode, ConditionOperator.Equal, region)
										.Where(c => c.Standard, ConditionOperator.Equal, standard);

									var count = dbScope.Executor.Execute(findQuery);
									if (count == 0)
									{
										var insQuery = _dataLayer.GetBuilder<IGlobalIdentity>()
											.Insert()
											.SetValue(c => c.LicenseGsid, licenseGsid)
											.SetValue(c => c.RegionCode, region)
											.SetValue(c => c.Standard, standard)
											.SetValue(c => c.RealGsid, realGsid)
											.SetValue(c => c.CreatedDate, DateTimeOffset.Now);
										count = dbScope.Executor.Execute(insQuery);
										++insertedCount;
									}
									else
									{
										var updQuery = _dataLayer.GetBuilder<IGlobalIdentity>()
											.Update()
											.SetValue(c => c.RealGsid, realGsid)
											.Where(c => c.LicenseGsid, ConditionOperator.Equal, licenseGsid)
											.Where(c => c.RegionCode, ConditionOperator.Equal, region)
											.Where(c => c.Standard, ConditionOperator.Equal, standard);
										count = dbScope.Executor.Execute(updQuery);
										++updatedCount;
									}

									++importedCount;

								}
								catch (Exception e)
								{
									_logger.Exception(Contexts.ThisComponent, Categories.CsvImport, $"Error importing row #{row}", e, this);
									error.AppendLine($"Error importing row #{row}: {e.Message}");
									++errorCount;
								}
							}
						}
					}
					catch (Exception e)
					{
						_logger.Exception(Contexts.ThisComponent, Categories.CsvImport, e, this);
						_integrationService.Finish(token, IntegrationStatusCode.Aborted, e.Message, null);
						result.Status = ImportFileResultStatus.Refused;
						result.Message = e.ToString();
						return result;
					}

					var total =
						$"File='{data.Name}', Rows={row}, Imported={importedCount}, Inserted={insertedCount}, Updated={updatedCount}, Errors={errorCount}";

					var statusNote = "CSV import completed successfully";

					if (errorCount > 0)
					{
						statusNote = "CSV import did not complete completely";
					}
					_integrationService.Finish(token, IntegrationStatusCode.Done, statusNote, total);

					result.FileName = $"gi_{token:D10}_{data.Name}";
					result.Status = ImportFileResultStatus.Processed;
					return result;
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.CsvImport, e, this);
				result.Status = ImportFileResultStatus.Refused;
				result.Message = e.ToString();
				return result;
			}
			
		}

		private static void CheckField(CsvReader reader, string field, StringBuilder error)
		{
			if (!reader.Contains(field))
			{
				error.AppendLine($"Column with name '{field}' not found");
			}
		}
	}
}
