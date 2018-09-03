using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace XICSM.Atdi.Icsm.Plugins.WebQuery
{
   public class CLocaliz
   {
      static Localization valLocal = null;
      static CLocaliz()
      {
         valLocal = new Localization(Environment.CurrentDirectory, "XICSM_WebQuery");
      }
      //===================================================
      // Переводим фразу "Please wait"
      //===================================================
      public static string PleaseWait { get { return TxT("Please wait"); } }
      //===================================================
      // Переводим фразу "Warning"
      //===================================================
      public static string Warning { get { return TxT("Warning"); } }
      //===================================================
      // Переводим фразу "Error"
      //===================================================
      public static string Error { get { return TxT("Error"); } }
      public static string Question { get { return TxT("Question"); } }
      //===================================================
      // Переводим фразу "Information"
      //===================================================
      public static string Information { get { return TxT("Information"); } }
      //===================================================
      // Переводим фразу "Saving"
      //===================================================
      public static string Saving { get { return TxT("Saving..."); } }
      //===================================================
      // Переводим фразу "Loading"
      //===================================================
      public static string Loading { get { return TxT("Loading..."); } }
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
