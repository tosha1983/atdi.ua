using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TR = System.Threading;

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
    }
}
