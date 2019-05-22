using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient
{
    public static class IMMainMenuExtentions
    {
        public static string BuildToolName(this IMMainMenu mainMenu, string parentToolName, string toolName)
        {
            return string.Concat(parentToolName, "\\", toolName);
        }

        public static void InsertItem(this IMMainMenu mainMenu, string parentToolName, string toolName, Action action)
        {
            mainMenu.InsertItem(mainMenu.BuildToolName(parentToolName, toolName), () => action(), MD.Tours.TableName);
        }

        public static void SetLocation(this IMMainMenu mainMenu)
        {
            mainMenu.SetInsertLocation(mainMenu.BuildToolName(PluginMetadata.Menu.BeforeTool, ""), IMMainMenu.InsertLocation.Before);
        }
    }

    public static class IMRecordsetExtentions
    {
        public static void Select(this IMRecordset recordset, params string[] fields)
        {
            recordset.Select(string.Join(",", fields));
        }

        public static void AddNew(this IMRecordset recordset, string tableNmae, string fieldName)
        {
            recordset.AddNew();
            recordset.PutNextId(tableNmae, fieldName);
        }

        public static void PutNextId(this IMRecordset recordset, string tableNmae, string fieldName)
        {
            recordset.Put(fieldName, IM.AllocID(tableNmae, 1, -1));
        }

        public static OpenedRecordsetScope OpenForAdd(this IMRecordset recordset, params string[] fields)
        {
            recordset.Select(string.Join(",", fields));
            recordset.SetWhere(fields[0], IMRecordset.Operation.Eq, -1);
            return recordset.OpenWithScope();
        }

        public static OpenedRecordsetScope OpenWithScope(this IMRecordset recordset)
        {
            return new OpenedRecordsetScope(recordset);
        }
    }

    public static class IMQueryMenuNodeExtentions
    {
        public static void AddContextMenuToolForEachRecords(this List<IMQueryMenuNode> nodes, string caption, IMQueryMenuNode.Handler handler)
        {
            nodes.Add(
                    new IMQueryMenuNode(
                        caption, null,
                        handler,
                        IMQueryMenuNode.ExecMode.EachRecord)
                );
        }

        public static void AddContextMenuToolForSelectionOfRecords(this List<IMQueryMenuNode> nodes, string caption, IMQueryMenuNode.Handler handler)
        {
            nodes.Add(
                    new IMQueryMenuNode(
                        caption, null,
                        handler,
                        IMQueryMenuNode.ExecMode.SelectionOfRecords)
                );
        }

        public static bool ExecuteContextMenuAction(this IMQueryMenuNode.Context context, string process, Func<int, bool> action)
        {
            try
            {
                using (var scope = Logger.StartTrace(process, "Execute", () => context.TableId, () => context.TableName))
                {
                    return action(context.TableId);
                }
            }
            catch (Exception e)
            {
                Logger.WriteExeption(process, e);
                return false;
            }
        }

        public static bool ExecuteContextMenuAction(this IMQueryMenuNode.Context context, string process, Func<IMDBList, bool> action)
        {
            try
            {
                using (var scope = Logger.StartTrace(process, "Execute", () => context.TableId, () => context.TableName))
                {
                    return action(context.DataList);
                }
            }
            catch (Exception e)
            {
                Logger.WriteExeption(process, e);
                return false;
            }
        }
    }

    public static class OthesExtentions
    {
        public static int TryToInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return IM.NullI;
            }
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return IM.NullI;
        }

        public static double? ToNull (this double value)
        {
            if (value == IM.NullD)
            {
                return null;
            }

            return value;
        }

        public static double? ToNull(this double? value)
        {
            if (value == null || value == IM.NullD)
            {
                return null;
            }

            return value;
        }

        public static int? ToNull(this int value)
        {
            if (value == IM.NullI)
            {
                return null;
            }

            return value;
        }

        public static int? ToNull(this int? value)
        {
            if (value == null || value == IM.NullI)
            {
                return null;
            }

            return value;
        }

        public static DateTime? ToNull(this DateTime value)
        {
            if (value == IM.NullT)
            {
                return null;
            }

            return value;
        }

        public static DateTime ? ToNull(this DateTime? value)
        {
            if (value == null || value == IM.NullT)
            {
                return null;
            }

            return value;
        }

        public static bool LessOrEqual(this double value, double target)
        {
            if (value <= target)
            {
                return true;
            }

            var delate = Math.Abs(value - target);

            return delate <= 0.0000001;
        }
        public static bool ExistsItems<T>(this List<T> list)
        {
            return list != null && list.Count > 0; 
        }

        public static bool IsNull(this DateTime value)
        {
            return value == IM.NullT;
        }
    }


    public class OpenedRecordsetScope : IDisposable
    {
        private IMRecordset _recordset;

        public OpenedRecordsetScope(IMRecordset recordset)
        {
            this._recordset = recordset;

            if (!recordset.IsOpen())
            {
                recordset.Open();
            }
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
                    {
                        this._recordset.Close();
                    }
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
