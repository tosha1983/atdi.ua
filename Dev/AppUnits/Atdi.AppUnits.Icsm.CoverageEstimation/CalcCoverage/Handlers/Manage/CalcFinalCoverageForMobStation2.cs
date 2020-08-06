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
using Atdi.AppUnits.Icsm.CoverageEstimation.Localization;

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
        private CLocaliz _localiz { get; set; }

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
                    throw new InvalidOperationException(CLocaliz.TxT("Config file is null"));
                }


                //Загрузка конфигурационного файла
                var loadConfig = dataConfig;

                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, CLocaliz.TxT($"The start procedure for opening the page for the section 'GroupsMobStationConfig2'"));

                var gdalCalc = new GdalCalc(this._logger);

                if (loadConfig.BlockStationsConfig.MobStation2Config == null)
                {
                    this._logger.Warning(Contexts.CalcCoverages, string.Format(CLocaliz.TxT(Events.StartIterationNumber.ToString()), iterationNumber), CLocaliz.TxT(Exceptions.GroupsMobStation2ConfigBlockIsEmpty));
                    return;
                }
                // Проверка/создание списка поддиректорий, соответствующих перечню значений Province
                gdalCalc.CheckOutTIFFFilesDirectorysForMobStation2(loadConfig);
                
                this._logger.Info(Contexts.CalcCoverages, string.Format(CLocaliz.TxT(Events.StartIterationNumber.ToString()), iterationNumber));

                if (loadConfig.BlockStationsConfig.MobStation2Config == null)
                {
                    throw new InvalidOperationException(CLocaliz.TxT(Exceptions.CodeOperatorAndStatusConfigBlockIsEmpty));
                }
                if (loadConfig.BlockStationsConfig.MobStation2Config.Length == 0)
                {
                    throw new InvalidOperationException(CLocaliz.TxT(Exceptions.CountCodeOperatorAndStatusConfigBlocksLengthZero));
                }

                // цикл по перечню стандартов, провинций, операторов 
                for (int k = 0; k < loadConfig.BlockStationsConfig.MobStation2Config.Length; k++)
                {
                    // получить очередной блок содержащий данные по стандарту, провинциям, операторам 
                    var codeOperatorAndStatusesConfig = loadConfig.BlockStationsConfig.MobStation2Config[k];

                    if (codeOperatorAndStatusesConfig == null)
                    {
                        throw new InvalidOperationException(CLocaliz.TxT(Exceptions.BlockCodeOperatorAndStatusConfigIsNull));
                    }

                    if (codeOperatorAndStatusesConfig.FreqConfig.provincesConfig == null)
                    {
                        throw new InvalidOperationException(CLocaliz.TxT(Exceptions.Block_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigIsNull));
                    }

                    if (codeOperatorAndStatusesConfig.FreqConfig.provincesConfig.Length == 0)
                    {
                        throw new InvalidOperationException(CLocaliz.TxT(Exceptions.CountBlock_CodeOperatorAndStatusConfig_FreqConfig_provincesConfigEqualZero));
                    }

                    for (int l = 0; l < codeOperatorAndStatusesConfig.FreqConfig.provincesConfig.Length; l++)
                    {
                        var provincesConfig = codeOperatorAndStatusesConfig.FreqConfig.provincesConfig[l];
                        if (!string.IsNullOrEmpty(provincesConfig.ICSTelecomProjectFile))
                        {
                            // Очистка бланка
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, CLocaliz.TxT($"Started the cleaning procedure for the form file ") +  provincesConfig.BlankTIFFFile);
                            gdalCalc.ClearBlank(loadConfig, provincesConfig.BlankTIFFFile);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Blank file cleanup procedure")} {provincesConfig.BlankTIFFFile} {CLocaliz.TxT("successfully completed")}");

                            //получить директорию текущего проекта ICS Telecom
                            var ICSTelecomProjectDir = Path.GetDirectoryName(provincesConfig.ICSTelecomProjectFile);
                            //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                            //gdalCalc.ClearTempFiles(loadConfig);
                            //удаление файлов TIF, TFW с директории проекта ICS Telecom (ICSTelecomProjectDir)
                            var ICSTelecomEwxFileDir = Path.GetDirectoryName(provincesConfig.NameEwxFile);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The started procedure for clearing the directory of the ICS Telecom project")} '{ICSTelecomEwxFileDir}'");
                            gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for clearing the directory of the ICS Telecom project")} '{ICSTelecomEwxFileDir}' {CLocaliz.TxT("successfully completed")}");

                            // формирование объекта Condition для отправки запроса в WebQuery
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{ CLocaliz.TxT("The procedure of forming the Condition object for further generation of SQL - query is started")}");
                            var condition = new CreateConditionForMobStation2(codeOperatorAndStatusesConfig, provincesConfig.Name, this._logger);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure of forming the Condition object for further generation of SQL - query was successfully completed")}");
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
                                    this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Pass operation 'CreateEWX' for FreqConfig=")} '{freqConfigValues}'  CurrICSTelecomEwxFileDir = '{ICSTelecomEwxFileDir}'");
                                    Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Pass operation 'CreateEWX' for FreqConfig=")} '{freqConfigValues}'  CurrICSTelecomEwxFileDir = '{ICSTelecomEwxFileDir}'");
                                    continue;
                                }
                            }

                            // запись параметров текущей операции в отдельный файл
                            this._checkOperation.Save(operationCreateEwx);
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("A new operation has been added to the log file")} {Operation.CreateEWX}");

                            // копирование перечня станций в EWX- файл, который расположен в текущей директории проекта ICS Telecom (ICSTelecomEwxFileDir)

                            var nameEwxFile = provincesConfig.NameEwxFile;
                            if (File.Exists(nameEwxFile))
                            {
                                File.Delete(nameEwxFile);
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Clear file")} '{nameEwxFile}' {CLocaliz.TxT("successfully completed")}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Clear file")} '{nameEwxFile}' {CLocaliz.TxT("successfully completed")}");
                            }
                            

                            this._logger.Info(Contexts.CalcCoverages, CLocaliz.TxT("The procedure of forming the list of stations on the basis of which the set of ewx - files will be created is begun"));
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, CLocaliz.TxT("The procedure of forming the list of stations on the basis of which the set of ewx - files will be created is begun"));
                            var copyStationsToEwxFile = new CopyMobStationToEwxFile(condition.GetCondition(), TableNameStations, this._dataLayer, this._logger);
                            var ewx = copyStationsToEwxFile.Copy(loadConfig, nameEwxFile, this._logger);
                            this._logger.Info(Contexts.CalcCoverages, CLocaliz.TxT("The procedure of forming the list of stations for generating ewx - files was completed successfully"));
                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, CLocaliz.TxT("The procedure of forming the list of stations for generating ewx - files was completed successfully"));
                            if (ewx.Length == 0)
                            {
                                
                                //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("The directory cleaning procedure has started")} {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The directory cleaning procedure has started")} {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}");
                                gdalCalc.ClearTempFiles(loadConfig);
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {dataConfig.DirectoryConfig.TempTIFFFilesDirectory} {CLocaliz.TxT("successfully completed")}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {dataConfig.DirectoryConfig.TempTIFFFilesDirectory} {CLocaliz.TxT("successfully completed")}");



                                //удаление файлов TIF, TFW с директории проекта ICS Telecom ()
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {ICSTelecomEwxFileDir}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {ICSTelecomEwxFileDir}");
                                gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {ICSTelecomEwxFileDir} {CLocaliz.TxT("successfully completed")}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {ICSTelecomEwxFileDir}  {CLocaliz.TxT("successfully completed")}");


                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for deleting the log file of the last operation has started")}  '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}'");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for deleting the log file of the last operation has started")}  '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}'");
                                this._checkOperation.DeleteProtocolFile();
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for deleting the log file of the last operation")}  '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}' {CLocaliz.TxT("successfully completed")}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for deleting the log file of the last operation")}  '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}' {CLocaliz.TxT("successfully completed")}");

                                throw new Exception(string.Format(CLocaliz.TxT(Exceptions.ErrorCopyStationsIntoEwxFile2), freqConfigValues, provincesConfig.Name));
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
                                        this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for creating an ewx file has started")}  '{createfileEwx}'");
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for creating an ewx file has started")}  '{createfileEwx}'");
                                        var createFileEwx = new CreateFileEwx(loadConfig, this._logger);
                                        createFileEwx.CreateFile(createfileEwx, ewx[h]);
                                        this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Procedure for creating an ewx file")} '{createfileEwx}' {CLocaliz.TxT("successfully completed")}");
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Procedure for creating an ewx file")} '{createfileEwx}'  {CLocaliz.TxT("successfully completed")}");
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

                                            this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Running the ICS Telecom process to calculate coverage (file")} '{nameEwxFile}')");
                                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Running the ICS Telecom process to calculate coverage (file")} '{nameEwxFile}')");
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
                                            this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("ICS Telecom has successfully completed the calculation of the coverage for the file")}  '{nameEwxFile}'");
                                            Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("ICS Telecom has successfully completed the calculation of the coverage for the file")}  '{nameEwxFile}'");
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
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("A new operation has been added to the log file")} {Operation.CreateTempTifFiles}");

                                        //Подготовка временных графических файлов (TIF), которые представляют собой результат операции объединения содержимого файла бланка и отдельно взятого файла покрытия,
                                        // который был получен на этапе обработки ICS Telecom
                                        // Полученные графические файлы записываются во временную директорию dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                        //var isSuccessCreateTempFiles = gdalCalc.StartProcessConcatBlankWithStation(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                        //var isSuccessCreateTempFiles = gdalCalc.StartProcessConcatBlankWithStation(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                        this._logger.Info(Contexts.CalcCoverages, CLocaliz.TxT("The procedure of preparation of temporary graphic files has begun"));
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, CLocaliz.TxT("The procedure of preparation of temporary graphic files has begun"));
                                        var isSuccessCreateTempFiles = gdalCalc.StartProcessConcatBlankWithStation(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile, filesOuttempEwxFilesDirectory[h]);
                                        if (isSuccessCreateTempFiles == false)
                                        {
                                            throw new InvalidOperationException(string.Format(CLocaliz.TxT(Exceptions.OccurredWhilePreparingTemporaryImageTIF2), freqConfigValues));
                                        }
                                        this._logger.Info(Contexts.CalcCoverages, CLocaliz.TxT("The procedure for preparing temporary image files has been successfully completed"));
                                        Utils.LogInfo(loadConfig, Contexts.CalcCoverages, CLocaliz.TxT("The procedure for preparing temporary image files has been successfully completed"));

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
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("A new operation has been added to the log file")} {Operation.CreateFinalCoverage}");

                                // На основании сформрованных на предыдщум шаге граческих файлах, формируем один итоговый файл, представляющий собой результат расчета суммарного покрытия 
                                // Результирующее покрытие записывается в директорию provincesConfig.OutTIFFFilesDirectory 
                                var nameProvince = provincesConfig.Name;
                                var provCode = Utils.GetProvincesCode(dataConfig, nameProvince);
                                var codeOperatorConfig = Utils.GetOperatorConfig(provincesConfig.CodeOperatorConfig);
                                var fileName = Utils.GetOutFileNameForMobStation2(dataConfig, provCode, freqConfigValues, codeOperatorConfig) + ".TIF";
                                var finalCoverageTIFFile = provincesConfig.OutTIFFFilesDirectory + fileName;
                                var tempPathfinalCoverageTIFFile = System.IO.Path.GetTempPath() + fileName;
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for generating the output coverage file has started")}  '{finalCoverageTIFFile}'");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for generating the output coverage file has started")}  '{finalCoverageTIFFile}'");
                                var isSuccessCreateOutCoverageFile = gdalCalc.Run(loadConfig, System.IO.Path.GetTempPath(), fileName, provincesConfig.BlankTIFFFile);
                                if (isSuccessCreateOutCoverageFile == false)
                                {
                                    throw new InvalidOperationException(string.Format(CLocaliz.TxT(Exceptions.FinalCoverageFileTifNotWritenIntoPath2), finalCoverageTIFFile, freqConfigValues));
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
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for generating the output coverage file")} '{finalCoverageTIFFile}'  {CLocaliz.TxT("successfully completed")}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for generating the output coverage file")} '{finalCoverageTIFFile}'  {CLocaliz.TxT("successfully completed")}");

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
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}");
                                gdalCalc.ClearTempFiles(loadConfig);
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {dataConfig.DirectoryConfig.TempTIFFFilesDirectory} {CLocaliz.TxT("successfully completed")}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {dataConfig.DirectoryConfig.TempTIFFFilesDirectory}  {CLocaliz.TxT("successfully completed")}");


                                //удаление файлов TIF, TFW с директории проекта ICS Telecom ()
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {ICSTelecomEwxFileDir}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {ICSTelecomEwxFileDir}");
                                gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {ICSTelecomEwxFileDir} {CLocaliz.TxT("successfully completed")}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("Directory cleaning procedure")} {ICSTelecomEwxFileDir}  {CLocaliz.TxT("successfully completed")}");


                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for deleting the log file of the last operation has started")} '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}'");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for deleting the log file of the last operation has started")} '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}'");
                                this._checkOperation.DeleteProtocolFile();
                                this._logger.Info(Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for deleting the log file of the last operation has started")} '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}' {CLocaliz.TxT("successfully completed")}");
                                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, $"{CLocaliz.TxT("The procedure for deleting the log file of the last operation has started")} '{this._appServerComponentConfig.ProtocolOperationFileNameForMobStation2}' {CLocaliz.TxT("successfully completed")}");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException(CLocaliz.TxT(Exceptions.ICSTelecomProjectFileIsNullOrEmpty));
                        }
                    }
                }
                //после очередной итерации
                // очистка итоговых  списка поддиректорий, соответствующих перечню значений Province
                //gdalCalc.ClearOutTIFFFilesDirectory(loadConfig);
                this._logger.Info(Contexts.CalcCoverages, string.Format(CLocaliz.TxT(Events.EndIterationNumber.ToString()), iterationNumber));

                Utils.LogInfo(loadConfig, Contexts.CalcCoverages, CLocaliz.TxT("Coverage calculation procedure for 'GroupsMobStationConfig' section completed successfully"));
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
                Utils.LogException(dataConfig, Contexts.CalcCoverages, e);
            }
        }
    }
}
