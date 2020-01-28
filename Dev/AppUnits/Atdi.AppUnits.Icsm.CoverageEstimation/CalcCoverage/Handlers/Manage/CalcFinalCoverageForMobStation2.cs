using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;
using Atdi.AppUnits.Icsm.Hooks;
using Atdi.AppUnits.Icsm.CoverageEstimation.Utilities;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public  class CalcFinalCoverageForMobStation2 :  ICalcFinalCoverage
    {
        private AppServerComponentConfig _appServerComponentConfig { get; set; }
        private ILogger _logger { get; set; }
        private IDataLayer<IcsmDataOrm> _dataLayer { get; set; }
        private CheckOperation _checkOperation { get; set; }
        private const string TableNameStations = "MOB_STATION2";
        private const string TableNameSaveOutCoverage = "XWEBCOVERAGE";


        public CalcFinalCoverageForMobStation2(AppServerComponentConfig appServerComponentConfig, IDataLayer<IcsmDataOrm> dataLayer, ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._appServerComponentConfig = appServerComponentConfig;
            this._checkOperation = new CheckOperation(appServerComponentConfig.ProtocolOperationFileNameForMobStation2);
        }


        public void Run(DataConfig dataConfig, long iterationNumber)
        {
            try
            {
                if (dataConfig==null)
                {
                    throw new InvalidOperationException("Config file is null");
                }


                //Загрузка конфигурационного файла
                var loadConfig = dataConfig;

                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру розрахунку покриття для секції 'GroupsMobStation2Config'");

                var gdalCalc = new GdalCalc(this._logger);
                

                // Проверка/создание списка поддиректорий, соответствующих перечню значений Province
                gdalCalc.CheckOutTIFFFilesDirectorysForMobStation2(loadConfig);
                
                this._logger.Info(Contexts.CalcCoverages, string.Format(Events.StartIterationNumber.ToString(), iterationNumber));

                if (loadConfig.BlockStationsConfig.MobStation2Config == null)
                {
                    throw new InvalidOperationException(Exceptions.CodeOperatorAndStatusConfigBlockIsEmpty);
                }
                if (loadConfig.BlockStationsConfig.MobStation2Config.Length == 0)
                {
                    throw new InvalidOperationException(Exceptions.CountCodeOperatorAndStatusConfigBlocksLengthZero);
                }

                // цикл по перечню стандартов, провинций, операторов 
                for (int k = 0; k < loadConfig.BlockStationsConfig.MobStation2Config.Length; k++)
                {
                    // получить очередной блок содержащий данные по стандарту, провинциям, операторам 
                    var codeOperatorAndStatusesConfig = loadConfig.BlockStationsConfig.MobStation2Config[k];

                    if (codeOperatorAndStatusesConfig == null)
                    {
                        throw new InvalidOperationException(Exceptions.BlockCodeOperatorAndStatusConfigIsNull);
                    }

                    if (codeOperatorAndStatusesConfig.FreqConfig.provincesConfig == null)
                    {
                        throw new InvalidOperationException(Exceptions.Block_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigIsNull);
                    }

                    if (codeOperatorAndStatusesConfig.FreqConfig.provincesConfig.Length == 0)
                    {
                        throw new InvalidOperationException(Exceptions.CountBlock_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigEqualZero);
                    }

                    for (int l = 0; l < codeOperatorAndStatusesConfig.FreqConfig.provincesConfig.Length; l++)
                    {
                        var provincesConfig = codeOperatorAndStatusesConfig.FreqConfig.provincesConfig[l];
                        if (!string.IsNullOrEmpty(provincesConfig.ICSTelecomProjectFile))
                        {
                            // Очистка бланка
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру очистки файлу бланка {provincesConfig.BlankTIFFFile}");
                            gdalCalc.ClearBlank(loadConfig, provincesConfig.BlankTIFFFile);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру очистки файлу бланка {provincesConfig.BlankTIFFFile} успішно завершено");

                            //получить директорию текущего проекта ICS Telecom
                            var ICSTelecomProjectDir = Path.GetDirectoryName(provincesConfig.ICSTelecomProjectFile);
                            //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                            //gdalCalc.ClearTempFiles(loadConfig);
                            //удаление файлов TIF, TFW с директории проекта ICS Telecom (ICSTelecomProjectDir)
                            var ICSTelecomEwxFileDir = Path.GetDirectoryName(provincesConfig.NameEwxFile);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру очистки директорії розміщення проекту ICS Telecom '{ICSTelecomEwxFileDir}'");
                            gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру очистки директорії розміщення проекту ICS Telecom '{ICSTelecomEwxFileDir}'  успішно завершено");

                            // формирование объекта Condition для отправки запроса в WebQuery
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру формування об'єкту Condition для подальшої генерації SQL - запиту");
                            var condition = new CreateConditionForMobStation2(codeOperatorAndStatusesConfig, provincesConfig.Name, this._logger);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру формування об'єкту Condition для  подальшої генерації SQL - запиту успішно виконано");
                            var freqConfigValues = codeOperatorAndStatusesConfig.FreqConfig.Values == null ? "" : codeOperatorAndStatusesConfig.FreqConfig.Values;


                            var operationCreateEwx = new CurrentOperation()
                            {
                                CurrICSTelecomProjectDir = ICSTelecomEwxFileDir,
                                Operation = Operation.CreateEWX,
                                Freqs = freqConfigValues,
                                Status = false
                            };

                            if (this._checkOperation.isNotNullFailedOperation() != null)
                            {
                                if (this._checkOperation.isFindOperation(operationCreateEwx) == false)
                                {
                                    this._logger.Info(Contexts.CalcCoverages, $"Pass operation 'CreateEWX' for FreqConfig='{freqConfigValues}' and CurrICSTelecomEwxFileDir = '{ICSTelecomEwxFileDir}'");
                                    Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Пропуск операції 'CreateEWX' для FreqConfig='{freqConfigValues}' та CurrICSTelecomEwxFileDir = '{ICSTelecomEwxFileDir}'");
                                    continue;
                                }
                            }

                            // запись параметров текущей операции в отдельный файл
                            this._checkOperation.Save(operationCreateEwx);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"До файлу протоколу додано нову операцію {Operation.CreateEWX}");

                            // копирование перечня станций в EWX- файл, который расположен в текущей директории проекта ICS Telecom (ICSTelecomEwxFileDir)

                            var nameEwxFile = provincesConfig.NameEwxFile;
                            if (File.Exists(nameEwxFile))
                            {
                                File.Delete(nameEwxFile);
                                this._logger.Info(Contexts.CalcCoverages, $"Очистку файла '{nameEwxFile}' успішно виконано");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Очистку файла '{nameEwxFile}' успішно виконано");
                            }
                            

                            this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру формування переліку станцій, на основі яких буде створено набір ewx - файлів");
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру формування переліку станцій, на основі яких буде створено набір ewx - файлів");
                            var copyStationsToEwxFile = new CopyMobStationToEwxFile(condition.GetCondition(), TableNameStations, this._dataLayer, this._logger);
                            var ewx = copyStationsToEwxFile.Copy(loadConfig, nameEwxFile, this._logger);
                            this._logger.Info(Contexts.CalcCoverages, $"Процедуру формування переліку станцій для генерації ewx - файлів завершено успішно");
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру формування переліку станцій для генерації ewx - файлів завершено успішно");
                            if (ewx.Length == 0)
                            {
                                
                                //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру очищення директорії {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру очищення директорії {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}");
                                gdalCalc.ClearTempFiles(loadConfig);
                                this._logger.Info(Contexts.CalcCoverages, $"Процедуру очищення директорії {dataConfig.DirectoryConfig.TempTIFFFilesDirectory} успішно завершено");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру очищення директорії {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}  успішно завершено");



                                //удаление файлов TIF, TFW с директории проекта ICS Telecom ()
                                this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру очищення директорії {ICSTelecomEwxFileDir}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру очищення директорії {ICSTelecomEwxFileDir}");
                                gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                this._logger.Info(Contexts.CalcCoverages, $"Процедуру очищення директорії {ICSTelecomEwxFileDir} успішно завершено");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру очищення директорії {ICSTelecomEwxFileDir}  успішно завершено");


                                this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру видалення файлу протоколу останньої операції '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}'");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру видалення файлу протоколу останньої операції '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}'");
                                this._checkOperation.DeleteProtocolFile();
                                this._logger.Info(Contexts.CalcCoverages, $"Процедуру видалення файлу протоколу останньої операції '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}' успішно завершено");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру видалення файлу протоколу останньої операції '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}' успішно завершено");

                                throw new Exception(string.Format(Exceptions.ErrorCopyStationsIntoEwxFile2, freqConfigValues, provincesConfig.Name));
                            }
                            else
                            {
                                var tempEwxFilesDirectory = dataConfig.DirectoryConfig.TempEwxFilesDirectory;
                                var filesOuttempEwxFilesDirectory = Directory.GetFiles(tempEwxFilesDirectory);
                                if (filesOuttempEwxFilesDirectory.Length == 0)
                                {
                                    for (int h = 0; h < ewx.Length; h++)
                                    {
                                        var createfileEwx = tempEwxFilesDirectory + $"\\Ewx_{h}.ewx";
                                        this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру створення ewx - файлу '{createfileEwx}'");
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру створення ewx - файлу '{createfileEwx}'");
                                        var createFileEwx = new CreateFileEwx(this._logger);
                                        createFileEwx.CreateFile(createfileEwx, ewx[h]);
                                        this._logger.Info(Contexts.CalcCoverages, $"Процедура створення ewx - файлу '{createfileEwx}'  успішно завершена");
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедура створення ewx - файлу '{createfileEwx}'  успішно завершена");
                                    }
                                }

                                filesOuttempEwxFilesDirectory = Directory.GetFiles(tempEwxFilesDirectory);
                                if (filesOuttempEwxFilesDirectory.Length > 0)
                                {

                                    for (int h = 0; h < filesOuttempEwxFilesDirectory.Length; h++)
                                    {
                                        if (File.Exists(nameEwxFile))
                                        {
                                            File.Delete(nameEwxFile);
                                        }

                                        if (File.Exists(filesOuttempEwxFilesDirectory[h]))
                                        {
                                            File.Copy(filesOuttempEwxFilesDirectory[h], nameEwxFile);
                                        }


                                        // цикл по списку команд конфигурационного файла
                                        for (int i = 0; i < loadConfig.DirectoryConfig.CommandsConfig.CommandsConfigs.Length; i++)
                                        {
                                            // Получить данные по очередной команде
                                            var commandConfig = loadConfig.DirectoryConfig.CommandsConfig.CommandsConfigs[i];
                                            // переменная для хранения аргументоа комманды
                                            var stringBuilder = new List<string>();
                                            // первый аргумент - всегда имя проекта ICS Telecom
                                            stringBuilder.Add(" " + provincesConfig.ICSTelecomProjectFile);
                                            for (int j = 0; j < commandConfig.ArgumentsConfig.ArgumentsConfigs.Length; j++)
                                            {
                                                // получение очередного аргумента комманды
                                                var argumentsConfig = commandConfig.ArgumentsConfig.ArgumentsConfigs[j];
                                                stringBuilder.Add(argumentsConfig.Value);
                                            }
                                            // Запись перечня аргументов в processStartInfo для последующего вызова
                                            var processStartInfo = new System.Diagnostics.ProcessStartInfo();
                                            processStartInfo.Arguments = string.Join(" ", stringBuilder);
                                            processStartInfo.FileName = loadConfig.DirectoryConfig.BinICSTelecomDirectory + @"\" + commandConfig.NameFile;
                                            processStartInfo.UseShellExecute = false;
                                            processStartInfo.CreateNoWindow = true;
                                            processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                            // Запуск процесса со списком сформированных раннее аргументов комманды

                                            this._logger.Info(Contexts.CalcCoverages, $"Запуск процесу ICS Telecom для розрахунку покриття (файл '{nameEwxFile}')");
                                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Запуск процесу ICS Telecom для розрахунку покриття (файл '{nameEwxFile}')");
                                            var value = System.Diagnostics.Process.Start(processStartInfo);
                                            // запуск процедуры подмены функции WinAPI BitBlt на "пустышку" 
                                            var processName = System.IO.Path.GetFileNameWithoutExtension(processStartInfo.FileName);
                                            var processICSTelecom = System.Diagnostics.Process.GetProcessesByName(processName);
                                            if ((processICSTelecom != null) && (processICSTelecom.Length > 0))
                                            {
                                                if (!string.IsNullOrEmpty(this._appServerComponentConfig.HookBitBltWinAPIFunctionInjectDll))
                                                {
                                                    Injector.Inject(processICSTelecom[0].Id, this._appServerComponentConfig.HookBitBltWinAPIFunctionInjectDll);
                                                }
                                            }
                                            // ожидаем завершения работы процесса ICS Telecom
                                            value.WaitForExit();
                                            this._logger.Info(Contexts.CalcCoverages, $"ICS Telecom успішно завершив розрахунок покриття для файлу '{nameEwxFile}'");
                                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"ICS Telecom успішно завершив розрахунок покриття для файлу '{nameEwxFile}'");
                                        }



                                        // проверка протокола
                                        var operationCreateTempTifFiles = new CurrentOperation()
                                        {
                                            CurrICSTelecomProjectDir = ICSTelecomEwxFileDir,
                                            Operation = Operation.CreateTempTifFiles,
                                            Freqs = freqConfigValues,
                                            NameProvince = provincesConfig.Name,
                                            Status = false
                                        };
                                        // запись параметров текущей операции в отдельный файл
                                        this._checkOperation.Save(operationCreateTempTifFiles);
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"До файлу протоколу додано нову операцію {Operation.CreateTempTifFiles}");

                                        //Подготовка временных графических файлов (TIF), которые представляют собой результат операции объединения содержимого файла бланка и отдельно взятого файла покрытия,
                                        // который был получен на этапе обработки ICS Telecom
                                        // Полученные графические файлы записываются во временную директорию dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                        //var isSuccessCreateTempFiles = gdalCalc.StartProcessConcatBlankWithStation(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                        //var isSuccessCreateTempFiles = gdalCalc.StartProcessConcatBlankWithStation(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                        this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру підготовки тимчасових графічних файлів");
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру підготовки тимчасових графічних файлів");
                                        var isSuccessCreateTempFiles = gdalCalc.StartProcessConcatBlankWithStation(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile, filesOuttempEwxFilesDirectory[h]);
                                        if (isSuccessCreateTempFiles == false)
                                        {
                                            throw new InvalidOperationException(string.Format(Exceptions.OccurredWhilePreparingTemporaryImageTIF2, freqConfigValues));
                                        }
                                        this._logger.Info(Contexts.CalcCoverages, $"Процедуру підготовки тимчасових графічних файлів успішно завершено");
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру підготовки тимчасових графічних файлів успішно завершено");

                                        //удаление файлов TIF, TFW с директории проекта ICS Telecom ()
                                        gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);

                                        if (File.Exists(filesOuttempEwxFilesDirectory[h]))
                                        {
                                            File.Delete(filesOuttempEwxFilesDirectory[h]);
                                        }
                                    }
                                }


                                // проверка протокола
                                var operationCreateFinalCoverage = new CurrentOperation()
                                {
                                    CurrICSTelecomProjectDir = ICSTelecomEwxFileDir,
                                    Operation = Operation.CreateFinalCoverage,
                                    Freqs = freqConfigValues,
                                    NameProvince = provincesConfig.Name,
                                    Status = false
                                };
                                // запись параметров текущей операции в отдельный файл
                                this._checkOperation.Save(operationCreateFinalCoverage);
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"До файлу протоколу додано нову операцію {Operation.CreateFinalCoverage}");

                                // На основании сформрованных на предыдщум шаге граческих файлах, формируем один итоговый файл, представляющий собой результат расчета суммарного покрытия 
                                // Результирующее покрытие записывается в директорию provincesConfig.OutTIFFFilesDirectory 
                                var nameProvince = provincesConfig.Name.Replace(",", "-").Replace(".", "-");
                                var provCode = Utils.GetProvincesCode(dataConfig, nameProvince);
                                var codeOperatorConfig = Utils.GetOperatorConfig(provincesConfig.CodeOperatorConfig);
                                var fileName = Utils.GetOutFileNameForMobStation2(dataConfig, provCode, freqConfigValues, codeOperatorConfig) + ".TIF";
                                var finalCoverageTIFFile = provincesConfig.OutTIFFFilesDirectory + fileName;
                                var tempPathfinalCoverageTIFFile = System.IO.Path.GetTempPath() + fileName;
                                this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру генерації вихідного файлу '{finalCoverageTIFFile}' покриття");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру генерації вихідного файлу '{finalCoverageTIFFile}' покриття");
                                var isSuccessCreateOutCoverageFile = gdalCalc.Run(loadConfig, System.IO.Path.GetTempPath(), fileName, provincesConfig.BlankTIFFFile);
                                if (isSuccessCreateOutCoverageFile == false)
                                {
                                    throw new InvalidOperationException(string.Format(Exceptions.FinalCoverageFileTifNotWritenIntoPath2, finalCoverageTIFFile, freqConfigValues));
                                }
                                else
                                {
                                    if (System.IO.File.Exists(finalCoverageTIFFile))
                                    {
                                        System.IO.File.Delete(finalCoverageTIFFile);
                                    }

                                    if (System.IO.File.Exists(tempPathfinalCoverageTIFFile))
                                    {
                                        System.IO.File.Copy(tempPathfinalCoverageTIFFile, finalCoverageTIFFile);
                                        System.IO.File.Delete(tempPathfinalCoverageTIFFile);
                                    }
                                }
                                this._logger.Info(Contexts.CalcCoverages, $"Процедуру генерації вихідного файлу '{finalCoverageTIFFile}' покриття успішно завершено");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру генерації вихідного файлу '{finalCoverageTIFFile}' покриття успішно завершено");

                                /*
                                if (this._appServerComponentConfig.IsSaveFinalCoverageToDB)
                                {
                                    var operationSaveFinalCoverageToDB =
                                    new CurrentOperation()
                                    {
                                        CurrICSTelecomProjectDir = ICSTelecomEwxFileDir,
                                        Operation = Operation.SaveFinalCoverageToDB,
                                        Freqs = freqConfigValues,
                                        NameProvince = provincesConfig.Name,
                                        Status = false
                                    };
                                    // запись параметров текущей операции в отдельный файл
                                    this._checkOperation.Save(operationSaveFinalCoverageToDB);

                                    //Передача полученного суммарного покрытия в виде двоичного файла данных в специальную таблицу (XWEBCOVERAGE) БД ICS Manager
                                    //var fileFinalCoverage = provincesConfig.OutTIFFFilesDirectory + @"\" + codeOperatorAndStatusesConfig.StandardConfig.Name + ".TIF";
                                    var saveResultCalcCoverageIntoDB = new SaveResultCalcCoverageIntoDB(TableNameSaveOutCoverage, this._dataLayer, finalCoverageTIFFile, this._logger);
                                    if (saveResultCalcCoverageIntoDB.SaveImageToBlob(nameProvince) == false)
                                    {
                                        throw new InvalidOperationException(string.Format(Exceptions.FinalCoverageFileTifNotWritenIntoDB, finalCoverageTIFFile));
                                    }
                                }
                                */

                                //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру очищення директорії {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру очищення директорії {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}");
                                gdalCalc.ClearTempFiles(loadConfig);
                                this._logger.Info(Contexts.CalcCoverages, $"Процедуру очищення директорії {dataConfig.DirectoryConfig.TempTIFFFilesDirectory} успішно завершено");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру очищення директорії {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}  успішно завершено");


                                //удаление файлов TIF, TFW с директории проекта ICS Telecom ()
                                this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру очищення директорії {ICSTelecomEwxFileDir}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру очищення директорії {ICSTelecomEwxFileDir}");
                                gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                this._logger.Info(Contexts.CalcCoverages, $"Процедуру очищення директорії {ICSTelecomEwxFileDir} успішно завершено");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру очищення директорії {ICSTelecomEwxFileDir}  успішно завершено");


                                this._logger.Info(Contexts.CalcCoverages, $"Розпочато процедуру видалення файлу протоколу останньої операції '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}'");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Розпочато процедуру видалення файлу протоколу останньої операції '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}'");
                                this._checkOperation.DeleteProtocolFile();
                                this._logger.Info(Contexts.CalcCoverages, $"Процедуру видалення файлу протоколу останньої операції '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}' успішно завершено");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру видалення файлу протоколу останньої операції '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}' успішно завершено");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException(Exceptions.ICSTelecomProjectFileIsNullOrEmpty);
                        }
                    }
                }
                //после очередной итерации
                // очистка итоговых  списка поддиректорий, соответствующих перечню значений Province
                //gdalCalc.ClearOutTIFFFilesDirectory(loadConfig);
                this._logger.Info(Contexts.CalcCoverages, string.Format(Events.EndIterationNumber.ToString(), iterationNumber));

                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"Процедуру розрахунку покриття для секції 'GroupsMobStation2Config'успішно завершено");
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
                Utils.LogException(dataConfig, Contexts.CalcCoverages, e);
            }
        }
    }
}
