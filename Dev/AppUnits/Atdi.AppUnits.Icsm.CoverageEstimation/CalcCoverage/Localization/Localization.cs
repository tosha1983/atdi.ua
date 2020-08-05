using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Localization
{
   public class Localization
   {
      private static Hashtable HashTransl;
      private string folderPath;
      private string filePrefix;
      //===================================================
      // [in]
      // string prefixFile - префикс файла
      // string language   - язык локализации
      public Localization(string path, string prefixFile)
      {
         if(path == "") folderPath = ".";
         else           folderPath = path;
         filePrefix = prefixFile;

       }
        //===================================================
        // Перевод строки
        //===================================================
        private void LoadLanguage()
        {
            HashTransl = new Hashtable();
            string fileName = folderPath + "\\" + filePrefix + ".txt";
            if (File.Exists(fileName))
            {
                try
                {
                    string strTransl = null;
                    string[] lines = File.ReadAllLines(fileName);
                    foreach (string line in lines)
                    {
                        string SpaceTrimStr = line.TrimStart(' ');
                        if (SpaceTrimStr.StartsWith(">"))
                        {
                            string strTmp = SpaceTrimStr.TrimStart(' ', '>');
                            if (strTmp.StartsWith("\"") && strTmp.EndsWith("\"") && (strTransl != null))
                            {
                                HashTransl[strTransl] = strTmp.Trim('"');
                                strTransl = null;   //Будем искать новый перевод
                            }
                        }
                        else if (SpaceTrimStr.StartsWith("\"") && SpaceTrimStr.EndsWith("\""))
                            strTransl = SpaceTrimStr.Trim('"');
                    }
                }
                catch { }
            }
        }
      //===================================================
      // Перевод строки
      //===================================================
      public string Translate(string trunslString)
      {
         LoadLanguage();
         if (HashTransl.ContainsKey(trunslString))
         {
            string retStr = (string)HashTransl[trunslString];
            return retStr;
         }
         return trunslString;
      }
     
   }
}
