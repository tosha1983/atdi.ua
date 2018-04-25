using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Atdi.AppServer.AppService.WebQueryDataDriver.LocalizationLib
{
   [Serializable]
   public class TLocaliz
   {
      static LocalizationLib.Localization valLocal = null;
      static TLocaliz()
      {
          valLocal = new LocalizationLib.Localization(Environment.CurrentDirectory, "XICSM_Monitoring");
      }
      //===================================================
      // Переводим фразу
      //===================================================
      public static string TxT(string trunsl)
      {
         if(valLocal != null)
            return valLocal.Translate(trunsl);
         return trunsl;
      }
      //===================================================
      // Переводим форму
      //===================================================
      public static void TxT(Form Formtrunsl)
      {
         if (valLocal != null)
            valLocal.Translate(Formtrunsl);
      }
      //===================================================
      // Переводим форму
      //===================================================
      public static List<string> Lenguages()
      {
         return valLocal.ListLanguage;
      }
   }
}
