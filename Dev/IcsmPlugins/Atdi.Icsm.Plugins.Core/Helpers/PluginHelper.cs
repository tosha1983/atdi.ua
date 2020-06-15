using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;
using TR = System.Threading;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using System.Windows.Forms;

namespace Atdi.Icsm.Plugins.Core
{
    public static class PluginHelper
    {
	    //private static System.Windows.Threading.Dispatcher UiDispatcher;
	    
	    public static void RunUiContext(Action action)
	    {
		    System.Windows.Application.Current.Dispatcher.Invoke(action);
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
        
    }
}
