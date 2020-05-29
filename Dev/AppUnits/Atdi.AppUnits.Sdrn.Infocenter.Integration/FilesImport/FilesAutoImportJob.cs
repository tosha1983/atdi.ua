using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.FilesImport
{
	internal class FilesAutoImportJob : IJobExecutor
	{
		private readonly IPipelineSite _pipelineSite;
		private readonly IPipeline<ImportFileInfo, ImportFileResult> _filesImportPipeline;
		private readonly AppServerComponentConfig _config;
		private readonly ILogger _logger;

		public FilesAutoImportJob(IPipelineSite pipelineSite, AppServerComponentConfig config, ILogger logger)
		{
			_pipelineSite = pipelineSite;
			_filesImportPipeline = _pipelineSite.GetByName<ImportFileInfo, ImportFileResult>(Pipelines.FilesImport);
			_config = config;
			_logger = logger;
		}

		public JobExecutionResult Execute(JobExecutionContext context)
		{
			// сканируем каталог
			var folderName = this._config.AutoImportFilesFolder;
			if (string.IsNullOrEmpty(folderName))
			{
				_logger.Warning(Contexts.ThisComponent, Categories.MapsImport, "Undefined files directory in configuration");
				return JobExecutionResult.Canceled;
			}

			if (!Directory.Exists(folderName))
			{
				_logger.Warning(Contexts.ThisComponent, Categories.MapsImport, "Invalid files directory specified in configuration");
				return JobExecutionResult.Canceled;
			}

			var processedFolder = Path.Combine(folderName, "Processed");
			if (!Directory.Exists(processedFolder))
			{
				Directory.CreateDirectory(processedFolder);
			}
			var notProcessedFolder = Path.Combine(folderName, "NotProcessed");
			if (!Directory.Exists(notProcessedFolder))
			{
				Directory.CreateDirectory(notProcessedFolder);
			}
			var refusedFolder = Path.Combine(folderName, "Refused");
			if (!Directory.Exists(refusedFolder))
			{
				Directory.CreateDirectory(refusedFolder);
			}

			var fileNames = Directory.GetFiles(folderName, "*.*", SearchOption.TopDirectoryOnly);
			if (fileNames != null && fileNames.Length > 0)
			{
				foreach (var path in fileNames)
				{
					var fileInfo = new ImportFileInfo
					{
						Path = path,
						Extension = Path.GetExtension(path),
						Name = Path.GetFileName(path)
					};

					var result = _filesImportPipeline.Execute(fileInfo);

					var movingFolder = processedFolder;
					if (result == null || result.Status == ImportFileResultStatus.NotProcessed)
					{
						movingFolder = notProcessedFolder;
					}
					else if (result.Status == ImportFileResultStatus.Refused)
					{
						movingFolder = refusedFolder;
					}
					var newFileName = result?.FileName;
					if (string.IsNullOrEmpty(newFileName))
					{
						newFileName = $"{DateTime.Now.Ticks:D20}_{fileInfo.Name}";
					}

					File.Move(path, Path.Combine(movingFolder, Path.GetFileName(newFileName)));

					if (!string.IsNullOrEmpty(result?.Message))
					{
						newFileName = $"{newFileName}.info.txt";
						File.WriteAllText(newFileName, result.Message);
					}
				}
			}

			return JobExecutionResult.Completed;
		}
	}
}
