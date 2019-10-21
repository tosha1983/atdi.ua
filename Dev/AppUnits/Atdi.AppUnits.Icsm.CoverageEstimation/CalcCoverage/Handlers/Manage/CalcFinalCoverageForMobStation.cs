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


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public  class CalcFinalCoverageForMobStation :  ICalcFinalCoverage
    {
        private AppServerComponentConfig _appServerComponentConfig { get; set; }
        private ILogger _logger { get; set; }
        private IDataLayer<IcsmDataOrm> _dataLayer { get; set; }
        private CheckOperation _checkOperation { get; set; }
        private const string TableNameStations = "MOB_STATION";
        private const string TableNameSaveOutCoverage = "XWEBCOVERAGE";


        public CalcFinalCoverageForMobStation(AppServerComponentConfig appServerComponentConfig, IDataLayer<IcsmDataOrm> dataLayer, ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._appServerComponentConfig = appServerComponentConfig;
            this._checkOperation = new CheckOperation(appServerComponentConfig.ProtocolOperationFileNameForMobStation);
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
                var gdalCalc = new GdalCalc(this._logger);
                

                // Проверка/создание списка поддиректорий, соответствующих перечню значений Province
                gdalCalc.CheckOutTIFFFilesDirectorysForMobStation(loadConfig);

                this._logger.Info(Contexts.CalcCoverages, string.Format(Events.StartIterationNumber.ToString(), iterationNumber));

                if (loadConfig.BlockStationsConfig.MobStationConfig == null)
                {
                    throw new InvalidOperationException(Exceptions.CodeOperatorAndStatusConfigBlockIsEmpty);
                }
                if (loadConfig.BlockStationsConfig.MobStationConfig.Length == 0)
                {
                    throw new InvalidOperationException(Exceptions.CountCodeOperatorAndStatusConfigBlocksLengthZero);
                }

                // цикл по перечню стандартов, провинций, операторов 
                for (int k = 0; k < loadConfig.BlockStationsConfig.MobStationConfig.Length; k++)
                {
                    // получить очередной блок содержащий данные по стандарту, провинциям, операторам 
                    var codeOperatorAndStatusesConfig = loadConfig.BlockStationsConfig.MobStationConfig[k];

                    if (codeOperatorAndStatusesConfig == null)
                    {
                        throw new InvalidOperationException(Exceptions.BlockCodeOperatorAndStatusConfigIsNull);
                    }

                    if (codeOperatorAndStatusesConfig.StandardConfig.provincesConfig == null)
                    {
                        throw new InvalidOperationException(Exceptions.Block_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigIsNull);
                    }

                    if (codeOperatorAndStatusesConfig.StandardConfig.provincesConfig.Length == 0)
                    {
                        throw new InvalidOperationException(Exceptions.CountBlock_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigEqualZero);
                    }

                    for (int l = 0; l < codeOperatorAndStatusesConfig.StandardConfig.provincesConfig.Length; l++)
                    {
                        var provincesConfig = codeOperatorAndStatusesConfig.StandardConfig.provincesConfig[l];
                        if (!string.IsNullOrEmpty(provincesConfig.ICSTelecomProjectFile))
                        {
                            // Очистка бланка
                            gdalCalc.ClearBlank(loadConfig, provincesConfig.BlankTIFFFile);

                            //получить директорию текущего проекта ICS Telecom
                            var ICSTelecomProjectDir = Path.GetDirectoryName(provincesConfig.ICSTelecomProjectFile);
                            //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                            gdalCalc.ClearTempFiles(loadConfig);
                            //удаление файлов TIF, TFW с директории проекта ICS Telecom (ICSTelecomProjectDir)
                            var ICSTelecomEwxFileDir = Path.GetDirectoryName(provincesConfig.NameEwxFile);
                            gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                            

                            // формирование объекта Condition для отправки запроса в WebQuery
                            var condition = new CreateConditionForMobStation(codeOperatorAndStatusesConfig, provincesConfig.Name, this._logger);
                            
                            var operationCreateEwx = new CurrentOperation()
                            {
                                CurrICSTelecomProjectDir = ICSTelecomEwxFileDir,
                                Operation = Operation.CreateEWX,
                                Standard = codeOperatorAndStatusesConfig.StandardConfig.Name,
                                Status = false
                            };

                            if (this._checkOperation.isNotNullFailedOperation()!=null)
                            {
                                if (this._checkOperation.isFindOperation(operationCreateEwx) == false)
                                {
                                    this._logger.Info(Contexts.CalcCoverages, $"Pass operation 'CreateEWX' for Standard='{codeOperatorAndStatusesConfig.StandardConfig.Name}' and CurrICSTelecomEwxFileDir = '{ICSTelecomEwxFileDir}'");
                                    continue;
                                }
                            }

                            // запись параметров текущей операции в отдельный файл
                            this._checkOperation.Save(operationCreateEwx);

                            // копирование перечня станций в EWX- файл, который расположен в текущей директории проекта ICS Telecom (ICSTelecomEwxFileDir)

                            var nameEwxFile = provincesConfig.NameEwxFile;
                            var copyStationsToEwxFile = new CopyMobStationToEwxFile(condition.GetCondition(), TableNameStations, this._dataLayer, this._logger);
                            var isSuccessCopyStations = copyStationsToEwxFile.Copy(loadConfig, nameEwxFile, this._logger);
                            if (isSuccessCopyStations == false)
                            {
                                throw new InvalidOperationException(string.Format(Exceptions.ErrorCopyStationsIntoEwxFile, codeOperatorAndStatusesConfig.StandardConfig.Name));
                            }
                            else
                            {
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
                                }



                                // проверка протокола
                                var operationCreateTempTifFiles = new CurrentOperation()
                                {
                                    CurrICSTelecomProjectDir = ICSTelecomEwxFileDir,
                                    Operation = Operation.CreateTempTifFiles,
                                    Standard = codeOperatorAndStatusesConfig.StandardConfig.Name,
                                    NameProvince = provincesConfig.Name,
                                    Status = false
                                };
                                // запись параметров текущей операции в отдельный файл
                                this._checkOperation.Save(operationCreateTempTifFiles);

                                //Подготовка временных графических файлов (TIF), которые представляют собой результат операции объединения содержимого файла бланка и отдельно взятого файла покрытия,
                                // который был получен на этапе обработки ICS Telecom
                                // Полученные графические файлы записываются во временную директорию dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                var isSuccessCreateTempFiles = gdalCalc.StartProcessConcatBlankWithStation(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                //var isSuccessCreateTempFiles = gdalCalc.SaveRecalcTIFFFile(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                if (isSuccessCreateTempFiles == false)
                                {
                                    throw new InvalidOperationException(string.Format(Exceptions.OccurredWhilePreparingTemporaryImageTIF, codeOperatorAndStatusesConfig.StandardConfig.Name));
                                }



                                // проверка протокола
                                var operationCreateFinalCoverage = new CurrentOperation()
                                {
                                    CurrICSTelecomProjectDir = ICSTelecomEwxFileDir,
                                    Operation = Operation.CreateFinalCoverage,
                                    Standard = codeOperatorAndStatusesConfig.StandardConfig.Name,
                                    NameProvince = provincesConfig.Name,
                                    Status = false
                                };
                                // запись параметров текущей операции в отдельный файл
                                this._checkOperation.Save(operationCreateFinalCoverage);

                                // На основании сформрованных на предыдщум шаге граческих файлах, формируем один итоговый файл, представляющий собой результат расчета суммарного покрытия 
                                // Результирующее покрытие записывается в директорию provincesConfig.OutTIFFFilesDirectory 
                                var nameProvince = provincesConfig.Name.Replace(",", "_").Replace(".", "_");
                                var finalCoverageTIFFile = provincesConfig.OutTIFFFilesDirectory + Transliteration.TransliteSpecial(codeOperatorAndStatusesConfig.StandardConfig.Name) + "_"+ Transliteration.TransliteSpecial(nameProvince) + ".TIF";
                                var tempPathfinalCoverageTIFFile = System.IO.Path.GetTempPath() + Transliteration.TransliteSpecial(codeOperatorAndStatusesConfig.StandardConfig.Name) + "_" + Transliteration.TransliteSpecial(nameProvince) + ".TIF";
                                var isSuccessCreateOutCoverageFile = gdalCalc.Run(loadConfig, System.IO.Path.GetTempPath(), codeOperatorAndStatusesConfig.StandardConfig.Name + "_" + Transliteration.TransliteSpecial(nameProvince) + ".TIF", provincesConfig.BlankTIFFFile);
                                if (isSuccessCreateOutCoverageFile == false)
                                {
                                    throw new InvalidOperationException(string.Format(Exceptions.FinalCoverageFileTifNotWritenIntoPath, finalCoverageTIFFile, codeOperatorAndStatusesConfig.StandardConfig.Name));
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


                                if (this._appServerComponentConfig.IsSaveFinalCoverageToDB)
                                {
                                    var operationSaveFinalCoverageToDB =
                                    new CurrentOperation()
                                    {
                                        CurrICSTelecomProjectDir = ICSTelecomEwxFileDir,
                                        Operation = Operation.SaveFinalCoverageToDB,
                                        Standard = codeOperatorAndStatusesConfig.StandardConfig.Name,
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

                                //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                gdalCalc.ClearTempFiles(loadConfig);
                                //удаление файлов TIF, TFW с директории проекта ICS Telecom ()
                                gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomEwxFileDir, provincesConfig.BlankTIFFFile);
                                this._checkOperation.DeleteProtocolFile();
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
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
        }
    }
}
