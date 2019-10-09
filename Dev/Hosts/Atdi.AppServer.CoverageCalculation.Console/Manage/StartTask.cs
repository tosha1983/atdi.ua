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
using Atdi.DataModels.CoverageCalculation;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels;
using System.ServiceModel;
using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels.Identity;
using Atdi.AppServer.CoverageCalculation;
using Atdi.Platform.Logging;

namespace Atdi.WebQuery.CoverageCalculation
{
    public  class StartTask
    {
        private  ILogger _logger { get; set; }

        public StartTask(ILogger logger)
        {
            this._logger = logger;
        }

        public void Run()
        {
            try
            {
                //Загрузка конфигурационного файла
                var loadConfig = Config.Load();
                var gdalCalc = new GdalCalc(this._logger);
                // Проверка/создание списка поддиректорий, соответствующих перечню значений Province
                gdalCalc.CheckOutTIFFFilesDirectorys(loadConfig);
                // инициализация доступа к WebQuery
                var initWebQuery = new InitWebQuery(loadConfig, this._logger);
                int index = 1;
                while (true)
                {
                    this._logger.Info(Contexts.CalcCoverages, string.Format(Events.StartIterationNumber.ToString(), index));

                    if (loadConfig.CodeOperatorAndStatusesConfig==null)
                    {
                        throw new InvalidOperationException(Exceptions.CodeOperatorAndStatusConfigBlockIsEmpty);
                    }
                    if (loadConfig.CodeOperatorAndStatusesConfig.Length == 0)
                    {
                        throw new InvalidOperationException(Exceptions.CountCodeOperatorAndStatusConfigBlocksLengthZero);
                    }

                    // цикл по перечню стандартов, провинций, операторов 
                    for (int k = 0; k < loadConfig.CodeOperatorAndStatusesConfig.Length; k++)
                    {
                        // получить очередной блок содержащий данные по стандарту, провинциям, операторам 
                        var codeOperatorAndStatusesConfig = loadConfig.CodeOperatorAndStatusesConfig[k];

                        if (codeOperatorAndStatusesConfig==null)
                        {
                            throw new InvalidOperationException(Exceptions.BlockCodeOperatorAndStatusConfigIsNull);
                        }

                        // формирование объекта Condition для отправки запроса в WebQuery
                        var condition = new CreateCondition(codeOperatorAndStatusesConfig, this._logger);
                        var loadStationsFromWebQuery = new LoadStations(initWebQuery.WebQuery, initWebQuery.UserToken, initWebQuery.QueryTokenStationsCalcCoverage, condition.GetCondition(), this._logger);
                        // Получение перечня станций
                        var queryResult = loadStationsFromWebQuery.GetStationsFromWebQuery();

                        if (queryResult.Data.Dataset.RowCount == 0)
                        {
                            throw new InvalidOperationException(Exceptions.ResultRequestWebQueryEmptyRecordset);
                        }
                        if (codeOperatorAndStatusesConfig.StandardConfig.provincesConfig==null)
                        {
                            throw new InvalidOperationException(Exceptions.Block_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigIsNull);
                        }
                        if (codeOperatorAndStatusesConfig.StandardConfig.provincesConfig.Length == 0)
                        {
                            throw new InvalidOperationException(Exceptions.CountBlock_CodeOperatorAndStatusConfig_StandardConfig_provincesConfigEqualZero);
                        }

                        var processStartInfo = new System.Diagnostics.ProcessStartInfo();
                        for (int l = 0; l < codeOperatorAndStatusesConfig.StandardConfig.provincesConfig.Length; l++)
                        {
                            var provincesConfig = codeOperatorAndStatusesConfig.StandardConfig.provincesConfig[l];
                            if (!string.IsNullOrEmpty(provincesConfig.ICSTelecomProjectFile))
                            {
                                //получить директорию текущего проекта ICS Telecom
                                var ICSTelecomProjectDir = System.IO.Path.GetDirectoryName(provincesConfig.ICSTelecomProjectFile);
                                //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                gdalCalc.ClearTempFiles(loadConfig);
                                //удаление файлов TIF, TFW с директории проекта ICS Telecom (ICSTelecomProjectDir)
                                gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomProjectDir);
                                // копирование перечня станций в EWX- файл, который расположен в текущей директории проекта ICS Telecom (ICSTelecomProjectDir)
                                var isSuccessCopyStations =  CopyStationsToEwxFile.Copy(queryResult, loadConfig, ICSTelecomProjectDir, this._logger);
                                if (isSuccessCopyStations==false)
                                {
                                    throw new InvalidOperationException(Exceptions.ErrorCopyStationsIntoEwxFile);
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
                                    processStartInfo.Arguments = string.Join(" ", stringBuilder);
                                    processStartInfo.FileName = loadConfig.DirectoryConfig.BinICSTelecomDirectory + @"\" + commandConfig.NameFile;
                                    processStartInfo.UseShellExecute = false;
                                    // Запуск процесса со списком сформированных раннее аргументов комманды
                                    //var value = System.Diagnostics.Process.Start(processStartInfo);
                                    //value.WaitForExit();
                                }

                                //Подготовка временных графических файлов (TIF), которые представляют собой результат операции объединения содержимого файла бланка и отдельно взятого файла покрытия,
                                // который был получен на этапе обработки ICS Telecom
                                // Полученные графические файлы записываются во временную директорию dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                var isSuccessCreateTempFiles = gdalCalc.SaveRecalcTIFFFile(loadConfig, ICSTelecomProjectDir, loadConfig.DirectoryConfig.BlankTIFFFile);
                                if (isSuccessCreateTempFiles == false)
                                {
                                    throw new InvalidOperationException(Exceptions.OccurredWhilePreparingTemporaryImageTIF);
                                }

                                // На основании сформрованных на предыдщум шаге граческих файлах, формируем один итоговый файл, представляющий собой результат расчета суммарного покрытия 
                                // Результирующее покрытие записывается в директорию provincesConfig.OutTIFFFilesDirectory 
                                var finalCoverageTIFFile = provincesConfig.OutTIFFFilesDirectory + codeOperatorAndStatusesConfig.StandardConfig.Name + ".TIF";
                                var isSuccessCreateOutCoverageFile = gdalCalc.Run(loadConfig, provincesConfig.OutTIFFFilesDirectory, codeOperatorAndStatusesConfig.StandardConfig.Name + ".TIF");
                                if (isSuccessCreateOutCoverageFile == false)
                                {
                                    throw new InvalidOperationException(string.Format(Exceptions.FinalCoverageFileTifNotWritenIntoPath, finalCoverageTIFFile));
                                }


                                //Передача полученного суммарного покрытия в виде двоичного файла данных в специальную таблицу (XWEBCOVERAGE) БД ICS Manager
                                var fileFinalCoverage = provincesConfig.OutTIFFFilesDirectory + @"\" + codeOperatorAndStatusesConfig.StandardConfig.Name + ".TIF";
                                var saveResultCalcCoverageIntoDB = new SaveResultCalcCoverageIntoDB(initWebQuery.WebQuery, initWebQuery.UserToken, initWebQuery.QueryTokenResultCalcCoverage, fileFinalCoverage, this._logger);
                                if (saveResultCalcCoverageIntoDB.SaveImageToBlob(provincesConfig.Name) == false)
                                {
                                    throw new InvalidOperationException(string.Format(Exceptions.FinalCoverageFileTifNotWritenIntoDB, fileFinalCoverage));
                                }

                                //очистка временных файлов с директории dataConfig.DirectoryConfig.TempTIFFFilesDirectory
                                gdalCalc.ClearTempFiles(loadConfig);
                                //удаление файлов TIF, TFW с директории проекта ICS Telecom ()
                                gdalCalc.ClearResultFilesICSTelecomProject(loadConfig, ICSTelecomProjectDir);
                            }
                            else
                            {
                                throw new InvalidOperationException(Exceptions.ICSTelecomProjectFileIsNullOrEmpty);
                            }
                        }
                    }
                    //после очередной итерации
                    // очистка итоговых  списка поддиректорий, соответствующих перечню значений Province
                    gdalCalc.ClearOutTIFFFilesDirectory(loadConfig);
                    this._logger.Info(Contexts.CalcCoverages, string.Format(Events.EndIterationNumber.ToString(), index));
                    index++;
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
        }
    }
}
