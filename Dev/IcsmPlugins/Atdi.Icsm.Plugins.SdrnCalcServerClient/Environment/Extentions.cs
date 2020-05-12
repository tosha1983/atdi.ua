using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Metadata;
using ICSM;


namespace Atdi.Icsm.Plugins.SdrnCalcServerClient
{
    public static class IMMainMenuExtentions
    {
        public static string BuildToolName(this IMMainMenu mainMenu, string parentToolName, string toolName)
        {
            return string.Concat(parentToolName, "\\", toolName);
        }
        public static void InsertItem(this IMMainMenu mainMenu, string parentToolName, string toolName, Action action)
        {
            mainMenu.InsertItem(mainMenu.BuildToolName(parentToolName, toolName), () => action(), SysLogs.TableName);
        }
        public static void SetLocation(this IMMainMenu mainMenu)
        {
            mainMenu.SetInsertLocation(mainMenu.BuildToolName(PluginMetadata.Menu.BeforeTool, ""), IMMainMenu.InsertLocation.Before);
        }
    }
    public class OpenedRecordsetScope : IDisposable
    {
        private IMRecordset _recordset;

        public OpenedRecordsetScope(IMRecordset recordset)
        {
            this._recordset = recordset;

            if (!recordset.IsOpen())
                recordset.Open();
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this._recordset.IsOpen())
                        this._recordset.Close();
                    this._recordset.Destroy();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
