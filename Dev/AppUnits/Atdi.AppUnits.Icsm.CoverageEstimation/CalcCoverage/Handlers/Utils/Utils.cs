using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;
using System.Linq;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Utilities
{
    public class Utils
    {
        public static void LogInfo(DataConfig dataConfig, EventContext context, EventText eventText)
        {
            if (!string.IsNullOrEmpty(dataConfig.DirectoryConfig.SpecifiedLogFile))
            {
                System.IO.File.AppendAllText(dataConfig.DirectoryConfig.SpecifiedLogFile,  DateTime.Now.ToString() + "."+ DateTime.Now.Millisecond + " Inf " + context.Name + " " + eventText.Text + Environment.NewLine);
            }
        }

        public static void LogException(DataConfig dataConfig, EventContext context, Exception e)
        {
            if (!string.IsNullOrEmpty(dataConfig.DirectoryConfig.SpecifiedLogFile))
            {
                System.IO.File.AppendAllText(dataConfig.DirectoryConfig.SpecifiedLogFile, DateTime.Now.ToString() + "." + DateTime.Now.Millisecond + " Exception " + context.Name + " " + e.Message + " " + e.StackTrace + Environment.NewLine);
            }
        }

        public static void LogError(DataConfig dataConfig, EventContext context, EventText eventText)
        {
            if (!string.IsNullOrEmpty(dataConfig.DirectoryConfig.SpecifiedLogFile))
            {
                System.IO.File.AppendAllText(dataConfig.DirectoryConfig.SpecifiedLogFile, DateTime.Now.ToString() + "." + DateTime.Now.Millisecond + " Err " + context.Name + " " + eventText.Text + Environment.NewLine);
            }
        }

        

        /// <summary>
        /// Метод, который возвращает по перечню областей их кодовые значения
        /// </summary>
        /// <param name="dataConfig"></param>
        /// <param name="Province"></param>
        /// <returns></returns>
        public static string GetProvincesCode(DataConfig dataConfig, string Province)
        {
            var resultOutProvinceCode = Province;
            var loadConfig = dataConfig;
            if (loadConfig.ProvinceCodeConfig != null)
            {
                for (int i = 0; i < loadConfig.ProvinceCodeConfig.Length; i++)
                {
                    var provCodeConfig = loadConfig.ProvinceCodeConfig[i];
                    resultOutProvinceCode = resultOutProvinceCode.Replace(provCodeConfig.NameProvince, provCodeConfig.Code);
                }
            }
            return resultOutProvinceCode;
        }
        /// <summary>
        /// Метод, выполняющий проверку по входящему перечню областей - все ли области Украины были указаны
        /// </summary>
        /// <param name="dataConfig"></param>
        /// <param name="Province"></param>
        /// <returns></returns>
        public static bool isAllProvinces(DataConfig dataConfig, string Province)
        {
            bool isSuccess = false;
            int cnt = 0;
            var loadConfig = dataConfig;
            var tempProvArr = Province.Split(new char[] { ',', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if ((loadConfig.ProvinceCodeConfig != null) && (tempProvArr!=null))
            {
                var lst = tempProvArr.ToList();
                for (int i = 0; i < loadConfig.ProvinceCodeConfig.Length; i++)
                {
                    var provCodeConfig = loadConfig.ProvinceCodeConfig[i];
                    if (tempProvArr.Length>0)
                    {
                        var temp = lst.Find(x => x.Contains(provCodeConfig.Code));
                        if (!string.IsNullOrEmpty(temp))
                        {
                            cnt++;
                        }
                    }
                }
                if (cnt == loadConfig.ProvinceCodeConfig.Length)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }
        /// <summary>
        /// Метод, возвращающий перечень наименований операторов
        /// </summary>
        /// <param name="codeOperatorConfig"></param>
        /// <returns></returns>
        public static string GetOperatorConfig(CodeOperatorConfig[] codeOperatorConfig)
        {
            var resultOutOperators = "";
            if (codeOperatorConfig != null)
            {
                for (int i = 0; i < codeOperatorConfig.Length; i++)
                {
                    var operatorConfig = codeOperatorConfig[i];
                    resultOutOperators += operatorConfig.Name + "-";
                }
            }
            return resultOutOperators;
        }

        /// <summary>
        /// Метод, возвращающий наименование выходного файла на основе заданного шаблона
        /// для (MobStation)
        /// </summary>
        /// <param name="dataConfig"></param>
        /// <param name="Province"></param>
        /// <param name="Standard"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static string GetOutFileNameForMobStation(DataConfig dataConfig, string Province, string Standard, string Operator)
        {
            const string ProvinceTemplate = "Province";
            const string StandardTemplate = "Standard";
            const string OperatorTemplate = "Operator";
            const string DateTemplate = "Date";
            var resultOutFileName = "";
            var loadConfig = dataConfig;
            var templateFile = loadConfig.DirectoryConfig.TemplateOutputFileNameForMobStation;
            string prov = isAllProvinces(dataConfig, Province) ? "UKR" : Province;
            string oper = string.IsNullOrEmpty(Operator) ? "ALL" : Operator;
            resultOutFileName = templateFile.Replace(ProvinceTemplate, prov).Replace(StandardTemplate, Standard).Replace(OperatorTemplate, oper).Replace(DateTemplate, DateTime.Now.ToString("dd_MM_yyyy"));
            return resultOutFileName;
        }

        /// <summary>
        /// Метод, возвращающий наименование выходного файла на основе заданного шаблона
        /// для (MobStation2)
        /// </summary>
        /// <param name="dataConfig"></param>
        /// <param name="Province"></param>
        /// <param name="Freq"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static string GetOutFileNameForMobStation2(DataConfig dataConfig, string Province, string Freq, string Operator)
        {
            const string ProvinceTemplate = "Province";
            const string FreqTemplate = "Freq";
            const string OperatorTemplate = "Operator";
            const string DateTemplate = "Date";
            var resultOutFileName = "";
            var loadConfig = dataConfig;
            var templateFile = loadConfig.DirectoryConfig.TemplateOutputFileNameForMobStation2;
            string prov = isAllProvinces(dataConfig, Province) ? "UKR" : Province;
            string oper = string.IsNullOrEmpty(Operator) ? "ALL" : Operator;
            resultOutFileName = templateFile.Replace(ProvinceTemplate, prov).Replace(FreqTemplate, Freq.Replace(";","-")).Replace(OperatorTemplate, oper).Replace(DateTemplate, DateTime.Now.ToString("dd_MM_yyyy"));
            return resultOutFileName;
        }

    }
}
