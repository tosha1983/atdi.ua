using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Atdi.AppServer.AppService.WebQueryDataDriver.XMLLibrary;
using Atdi.AppServer.AppService.WebQueryDataDriver;


namespace Atdi.AppServer.AppService.WebQueryDataDriver.Logs
{
    //=================================================================================================================================================================================================
   /// <summary>
   /// Тип сообщения
   /// </summary>
   public enum ELogsType
   {
      Info,
      Warning,
      Error
   }
   //=================================================================================================================================================================================================
   /// <summary>
   /// Кто создал сообщение
   /// </summary>
   public enum ELogsWhat
   {
      Unknown = 0,
      DataDriver = 1,
      WorkFlow = 2,
      WebViewer = 3,
      Loader_Sponsor_cs = 4,
      Loader_PluginLoader_cs = 5,
      Loader_PluginHost_cs = 6,
      Loader_ManageInstance_cs = 7,
      Direct_ManageDirect_cs = 8,
      ServerApp_UserControls = 9,
      ServerApp = 10,
      DataDriver_ConnectDB = 11,
      DataDriver_ParseIRP = 12,
      DataDriver_Object_Manager = 13,
      HealthCareCalculationView = 14,
      RVIS = 15
   }

   [Serializable]
   public class CLogs
   {
       
       //public static string CurrentDir = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\DataDriver\\";
      //=================================================================================================================================================================================================
      /// <summary>
      /// Writes an error message from Exception
      /// </summary>
      /// <param name="who">Who writes the message</param>
      /// <param name="ex">Exception</param>
      public static void WriteError(ELogsWhat who, Exception ex)
      {
          WriteError(who, ex, true);
      }
      //=================================================================================================================================================================================================
      /// <summary>
      /// Writes an error message from Exception
      /// </summary>
      /// <param name="who">Who writes the message</param>
      /// <param name="ex">Exception</param>
      /// <param name="isSilent">Тихий режим</param>
      public static void WriteError(ELogsWhat who, Exception ex, bool isSilent)
      {
          string mess = string.Format("[ ELogsType:{0}; ELogsWhat:{1};  {2} Source:{3}; TargetSite:{4}; StackTrace:{5}; DateTime:{6} ]", ELogsType.Error.ToString(), who.ToString(), ex.Message, ex.Source, ex.TargetSite, ex.StackTrace, DateTime.Now.ToString());
         WriteLogTxt(mess);
         if (!isSilent)
             System.Windows.Forms.MessageBox.Show(mess, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
      }
      //=================================================================================================================================================================================================
      /// <summary>
      /// Writes an error message
      /// </summary>
      /// <param name="who">Who writes the message</param>
      /// <param name="message">A message</param>
      public static void WriteError(ELogsWhat who, string message)
      {
         WriteError(who, message, true);
      }
      //=================================================================================================================================================================================================
      /// <summary>
      /// Writes an error message
      /// </summary>
      /// <param name="who">Who writes the message</param>
      /// <param name="message">A message</param>
      /// <param name="isSilent">TRUE - тихий режим</param>
      public static void WriteError(ELogsWhat who, string message, bool isSilent)
      {
          string mess = string.Format("[ ELogsType:{0}; ELogsWhat:{1}; Message:{2}; DateTime:{3} ]", ELogsType.Error.ToString(), who.ToString(), message, DateTime.Now.ToString());
          WriteLogTxt(mess);
          if (!isSilent)
              System.Windows.Forms.MessageBox.Show(mess, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
      }
      //=================================================================================================================================================================================================
      /// <summary>
      /// Writes a warning message
      /// </summary>
      /// <param name="who">Who writes the message</param>
      /// <param name="message">A message</param>
      public static void WriteWarning(ELogsWhat who, string message)
      {
          string mess = string.Format("[ ELogsType:{0}; ELogsWhat:{1}; Message:{2}; DateTime:{3} ]", ELogsType.Warning.ToString(), who.ToString(), message, DateTime.Now.ToString());
         WriteLogTxt(mess);
      }
      //=================================================================================================================================================================================================
      /// <summary>
      /// Writes an info message
      /// </summary>
      /// <param name="who">Who writes the message</param>
      /// <param name="message">A message</param>
      public static void WriteInfo(ELogsWhat who, string message, string User, string TableName)
      {
          WriteInfo(who, message, User, TableName, true);
      }

      //=================================================================================================================================================================================================
      /// <summary>
      /// Writes LOG in TXT files
      /// </summary>
      /// <param name="message">A message</param>
      public static void WriteLogTxt(string message)
      {
          using (StreamWriter fs = new StreamWriter(XMLLibrary.BaseXMLDirect.pathtoapp + @"\LogDataDriver.txt", true))
          {
              fs.WriteLine(message);
          }

         
      }
      //=================================================================================================================================================================================================
      /// <summary>
      /// Writes an info message
      /// </summary>
      /// <param name="who">Who writes the message</param>
      /// <param name="message">A message</param>
      /// <param name="isSilent">TRUE - Тихий режим</param>
      public static void WriteInfo(ELogsWhat who, string message,string User, string TableName, bool isSilent)
      {
          string mess = string.Format("[ ELogsType:{0}; ELogsWhat:{1}; Message:{2}; DateTime:{3} ]", ELogsType.Info.ToString(), who.ToString(), message, DateTime.Now.ToString());
          ConnectDB cn = new ConnectDB();
          cn.WriteLogICSMBase(who, message, User, TableName);
          WriteLogTxt(mess);
          if(!isSilent)
              System.Windows.Forms.MessageBox.Show(mess, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
      }

   }
}
