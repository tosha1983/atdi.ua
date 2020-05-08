using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TR = System.Threading;
using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace XICSM.ICSControlClient
{
    public static class PluginHelper
    {
        public const string MessageBoxCaption = "ISC Control Client";
        public static void ShowMessageValueMustBeInTheRange(string fieldName, string from, string to)
        {
            MessageBox.Show($"{Properties.Resources.Message_TheValue} '{fieldName}' {Properties.Resources.Message_MustBeInTheRange} {Properties.Resources.From} {from} {Properties.Resources.To} {to}!", MessageBoxCaption);
        }
        public static void ShowMessageValueShouldBeGreatOfThe(string fieldFrom, string fieldTo)
        {
            MessageBox.Show($"{Properties.Resources.Message_TheValue} '{fieldTo}' {Properties.Resources.Message_ShouldBeGreatOfThe} '{fieldFrom}'!", MessageBoxCaption);
        }
        //public static WebApiEndpoint GetEndpoint()
        //{
        //    return new WebApiEndpoint(new Uri("http://10.1.1.195:15020/"), "/appserver/v1");
        //}
        public static double? ConvertStringToDouble(string s, bool isShowMessage = false)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            char systemSeparator = TR.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (!s.Contains(","))
                    result = double.Parse(s, CultureInfo.InvariantCulture);
                else
                    result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
            }
            catch
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        if (isShowMessage)
                            MessageBox.Show("Wrong double format");

                        return null;
                    }
                }
            }
            return result;
        }
        public static string GetRestApiEndPoint()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string endpointUrls = appSettings["SdrnServerRestEndpoint"];

            if (string.IsNullOrEmpty(endpointUrls))
            {
                MessageBox.Show("Undefined value for SdrnServerRestEndpoint in file ICSM3.exe.config.");
                return null;
            }
            return endpointUrls;
        }
        public static string GetDataContext()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string dataContext = appSettings["DataContext"];

            if (string.IsNullOrEmpty(dataContext))
            {
                MessageBox.Show("Undefined value for DataContext in file ICSM3.exe.config.");
                return null;
            }
            return dataContext;
        }
        public static string GetWebAPIBaseAddress()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string dataWebAPIBaseAddress = appSettings["WebAPIBaseAddress"];

            if (string.IsNullOrEmpty(dataWebAPIBaseAddress))
            {
                MessageBox.Show("Undefined value for WebAPIBaseAddress in file ICSM3.exe.config.");
                return null;
            }
            return dataWebAPIBaseAddress;
        }

        public static string GetWebAPIUrl()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string dataWebAPIUrl = appSettings["WebAPIUrl"];

            if (string.IsNullOrEmpty(dataWebAPIUrl))
            {
                MessageBox.Show("Undefined value for WebAPIUrl in file ICSM3.exe.config.");
                return null;
            }
            return dataWebAPIUrl;
        }


        public static long GetMaxCountRecordInPage()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string dataMaxCountRecordInPage = appSettings["MaxCountRecordInPage"];
            if (string.IsNullOrEmpty(dataMaxCountRecordInPage))
            {
                MessageBox.Show("Undefined value for MaxCountRecordInPage in file ICSM3.exe.config.");
                return 0;
            }
            return Convert.ToInt64(dataMaxCountRecordInPage);
        }

        public static long GetMaxCountRecordInCsvFile()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string dataMaxCountRecordInCsvFile = appSettings["MaxCountRecordInCsvFile"];

            if (string.IsNullOrEmpty(dataMaxCountRecordInCsvFile))
            {
                MessageBox.Show("Undefined value for MaxCountRecordInCsvFile in file ICSM3.exe.config.");
                return 0;
            }
            return Convert.ToInt64(dataMaxCountRecordInCsvFile);
        }
        public static string GetFullTaskStatus(string state)
        {
            string statusFull = "";
            switch (state)
            {
                case "N":
                    statusFull = Properties.Resources.State_N;
                    break;
                case "C":
                    statusFull = Properties.Resources.State_C;
                    break;
                case "F":
                    statusFull = Properties.Resources.State_F;
                    break;
                case "A":
                    statusFull = Properties.Resources.State_A;
                    break;
                case "S":
                    statusFull = Properties.Resources.State_S;
                    break;
                default:
                    break;
            }
            return statusFull;
        }
    }
}
