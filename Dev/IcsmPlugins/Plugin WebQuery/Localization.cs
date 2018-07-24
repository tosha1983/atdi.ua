using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using Microsoft.Win32;

namespace XICSM.WebQuery
{
   public class Localization
   {
      private List<string> listLanguage;
      public List<string> ListLanguage
      {
         get { return this.listLanguage;}
      }

      private Hashtable HashTransl;
      private bool tryLoadLanguage;
      private string folderPath;
      private string filePrefix;
      //===================================================
      // Конструктор
      // [in]
      // string prefixFile - префикс файла
      //===================================================
      public Localization(string path, string prefixFile) : this(path, prefixFile, "")
      {}
      //===================================================
      // [in]
      // string prefixFile - префикс файла
      // string language   - язык локализации
      public Localization(string path, string prefixFile, string language)
      {
         if(path == "") folderPath = ".";
         else           folderPath = path;
         filePrefix = prefixFile;
         tryLoadLanguage = false;
         listLanguage = new List<string>();
         if (language != "")
            listLanguage.Add(language);
         // Попрбуем загрузить другие языки ICSM
         RegistryKey readKey = Registry.CurrentUser.OpenSubKey("Software\\ATDI\\ICSM\\LANGUAGES");
         if (readKey != null)
         {
            try
            {
               // Загружаем список языков, поддерживаемых ICSM
               string loadLanguage = (string)readKey.GetValue("Order");
               if (loadLanguage != null)
               {
                  string[] splitLanguage = loadLanguage.Split(new Char[] { '.' });
                        foreach (string lng in splitLanguage)
                            if (lng != "")
                            {
                                listLanguage.Add(lng);
                                return;
                            }
               }
            }
            finally
            {
               readKey.Close();
            }
         }
            // Если языки не загружены, то будем пробовать загрузить язык текущей локализации ОС
            //?? Пока не знаю как определить текущий язык ОС
            //listLanguage.Add("ENG");
        }
      //===================================================
      // Перевод строки
      //===================================================
      private void LoadLanguage()
      {
            HashTransl = new Hashtable();
            //if(tryLoadLanguage)
            //   return;
            //tryLoadLanguage = true;
            // Пытаемся загрузить язык
            listLanguage = new List<string>();
            listLanguage.Clear();
            // Попрбуем загрузить другие языки ICSM
            RegistryKey readKey = Registry.CurrentUser.OpenSubKey("Software\\ATDI\\ICSM\\LANGUAGES");
            if (readKey != null)
            {
                try
                {
                    // Загружаем список языков, поддерживаемых ICSM
                    string loadLanguage = (string)readKey.GetValue("Order");
                    if (loadLanguage != null)
                    {
                        string[] splitLanguage = loadLanguage.Split(new Char[] { '.' });
                        foreach (string lng in splitLanguage)
                            if (lng != "")
                            {
                                listLanguage.Add(lng);
                                break;
                            }
                    }
                }
                finally
                {
                    readKey.Close();
                }
            }

        
         foreach(string lng in listLanguage)
         {
            string fileName = folderPath + "\\" + filePrefix + "_" + lng + ".txt";
            if(File.Exists(fileName))
            {
               try
               {
                  string strTransl = null;
                  string [] lines = File.ReadAllLines(fileName);
                  foreach(string line in lines)
                  {
                     string SpaceTrimStr = line.TrimStart(' ');
                     if(SpaceTrimStr.StartsWith(">"))
                     {
                        string strTmp = SpaceTrimStr.TrimStart(' ', '>');
                        if(strTmp.StartsWith("\"") && strTmp.EndsWith("\"") && (strTransl!=null))
                        {
                           HashTransl[strTransl] = strTmp.Trim('"');
                           strTransl = null;   //Будем искать новый перевод
                        }
                     }
                     else if(SpaceTrimStr.StartsWith("\"") && SpaceTrimStr.EndsWith("\""))
                        strTransl = SpaceTrimStr.Trim('"');
                  }
                  break; //Файл с переводом нашли, вываливаемся.
               }
               catch {}
            }
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
      //===================================================
      // Перевод елементов формы
      //===================================================
      public void Translate(Form trunslForm)
      {
         trunslForm.Text = Translate(trunslForm.Text);
         foreach (Control cntrl in trunslForm.Controls)
            EnumControls(cntrl);
      }
      //===================================================
      // Рекурсивный перевод всех строк формы
      //===================================================
      private void EnumControls(Control trunslControl)
      {
         if (trunslControl == null)
            return;
         
         // Перевод контрола
         if (trunslControl is Label
             || trunslControl is GroupBox
             || trunslControl is Button
             || trunslControl is CheckBox
             || trunslControl is RadioButton
            )
            trunslControl.Text = Translate(trunslControl.Text);
         
         // ComboBox
         if (trunslControl is ComboBox)
         {
            ComboBox cmbBox = trunslControl as ComboBox;
            for(int ind = 0; ind < cmbBox.Items.Count; ind++)
               if(cmbBox.Items[ind] is string)
               {
                  try
                  {
                     string strObj = cmbBox.Items[ind] as string;
                     cmbBox.Items[ind] = Translate(strObj);
                  }
                  catch { }
               }
         }

         // ListView
         if (trunslControl is ListView)
         {
            ListView lstView = trunslControl as ListView;
            for(int ind = 0; ind < lstView.Columns.Count; ind++)
            {
               try
               {
                  string strObj = lstView.Columns[ind].Text as string;
                  lstView.Columns[ind].Text = Translate(strObj);
               }
               catch { }
            }
         }

         // Menu

         foreach (Control cntrl in trunslControl.Controls)
            EnumControls(cntrl);
      }
   }
}
