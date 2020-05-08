using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Diagnostics;
using ICSM;
using MD = Atdi.Icsm.Plugins.SdrnCalcServerClient.Metadata;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient
{
    public class Logger
    {
        public enum RecordType
        {
            Info,
            Warning,
            Error,
            Exception,
            StartedTrace,
            StopedTrace
        }
        public class Record
        {
            public DateTime Created;
            public RecordType Type;
            public string Process;
            public string Message;
            public string Data;
            public long Duration;
        }

        public class TraceScope : IDisposable
        {
            private Stopwatch _timer;
            private string _process;
            private string _message;
            private Expression<Func<object>>[] _data;

            public TraceScope(string process, string message, params Expression<Func<object>>[] data)
            {
                this._process = process;
                this._message = message;
                this._data = data;

                this._timer = new Stopwatch();
                Logger.WriteStartedTrace(process, message, data);
                this._timer.Start();
            }

            #region IDisposable Support
            private bool disposedValue = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        this._timer.Stop();
                        Logger.WriteStopedTrace(this._process, this._message, this._timer.ElapsedMilliseconds, this._data);
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

        public static void WriteExeption(string process, Exception e, bool showToUser = false)
        {
            var record = new Record
            {
                Type = RecordType.Exception,
                Created = DateTime.Now,
                Process = process,
                Message = e.Message,
                Data = e.ToString()
            };

            WriteToDb(record);

            if (showToUser)
            {
                System.Windows.Forms.MessageBox.Show($"Exeption: {e}", process, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
        }

        public static void WriteError(string process, string message, bool showToUser = false)
        {
            var record = new Record
            {
                Type = RecordType.Error,
                Created = DateTime.Now,
                Process = process,
                Message = message
            };

            WriteToDb(record);

            if (showToUser)
            {
                System.Windows.Forms.MessageBox.Show(message, process, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void WriteInfo(string process, string message, bool showToUser = false)
        {
            var record = new Record
            {
                Type = RecordType.Info,
                Created = DateTime.Now,
                Process = process,
                Message = message
            };

            WriteToDb(record);

            if (showToUser)
            {
                System.Windows.Forms.MessageBox.Show(message, process, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }

        public static void WriteWarning(string process, string message, bool showToUser = false)
        {
            var record = new Record
            {
                Type = RecordType.Warning,
                Created = DateTime.Now,
                Process = process,
                Message = message
            };

            WriteToDb(record);

            if (showToUser)
            {
                System.Windows.Forms.MessageBox.Show(message, process, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        public static void WriteStartedTrace(string process, string message, params Expression<Func<object>>[] data)
        {
            var record = new Record
            {
                Type = RecordType.StartedTrace,
                Created = DateTime.Now,
                Process = process,
                Message = message
            };

            WriteToDb(record);
        }

        public static void WriteStopedTrace(string process, string message, long duration, params Expression<Func<object>>[] data)
        {
            var record = new Record
            {
                Type = RecordType.StopedTrace,
                Created = DateTime.Now,
                Process = process,
                Message = message,
                Duration = duration
            };

            WriteToDb(record);
        }

        public static TraceScope StartTrace(string process, string message, params Expression<Func<object>>[] data)
        {
            return new TraceScope(process, message, data);
        }

        public static void WriteToDb(Record record)
        {
            try
            {
                var logsRs = new IMRecordset(MD.SysLogs.TableName, IMRecordset.Mode.ReadWrite);
                using (var logsRsScope = logsRs.OpenForAdd(
                        MD.SysLogs.Fields.Id,
                        MD.SysLogs.Fields.Event,
                        MD.SysLogs.Fields.TableName,
                        MD.SysLogs.Fields.Count,
                        MD.SysLogs.Fields.Info,
                        MD.SysLogs.Fields.Who,
                        MD.SysLogs.Fields.When
                    ))
                {
                    var currentTime = DateTime.Now;
                    var currentUser = IM.ConnectedUser();

                    logsRs.AddNew(MD.SysLogs.TableName, MD.SysLogs.Fields.Id);
                    logsRs.Put(MD.SysLogs.Fields.Event, record.Type.ToString());
                    logsRs.Put(MD.SysLogs.Fields.TableName, MD.SysLogs.TableName);
                    logsRs.Put(MD.SysLogs.Fields.Count, 1);
                    logsRs.Put(MD.SysLogs.Fields.Info, $"{record.Process}: {record.Message}");
                    logsRs.Put(MD.SysLogs.Fields.Who, currentUser);
                    logsRs.Put(MD.SysLogs.Fields.When, currentTime);
                    logsRs.Update();

                    if (record.Duration > 0)
                    {
                        logsRs.AddNew(MD.SysLogs.TableName, MD.SysLogs.Fields.Id);
                        logsRs.Put(MD.SysLogs.Fields.Event, record.Type.ToString());
                        logsRs.Put(MD.SysLogs.Fields.TableName, MD.SysLogs.TableName);
                        logsRs.Put(MD.SysLogs.Fields.Count, 1);
                        logsRs.Put(MD.SysLogs.Fields.Info, $"{record.Process}: Duration {record.Duration}ms");
                        logsRs.Put(MD.SysLogs.Fields.Who, currentUser);
                        logsRs.Put(MD.SysLogs.Fields.When, currentTime);
                        logsRs.Update();
                    }

                    if (!string.IsNullOrEmpty(record.Data))
                    {
                        logsRs.AddNew(MD.SysLogs.TableName, MD.SysLogs.Fields.Id);
                        logsRs.Put(MD.SysLogs.Fields.Event, record.Type.ToString());
                        logsRs.Put(MD.SysLogs.Fields.TableName, MD.SysLogs.TableName);
                        logsRs.Put(MD.SysLogs.Fields.Count, 1);
                        logsRs.Put(MD.SysLogs.Fields.Info, $"{record.Process}: {record.Data}");
                        logsRs.Put(MD.SysLogs.Fields.Who, currentUser);
                        logsRs.Put(MD.SysLogs.Fields.When, currentTime);
                        logsRs.Update();
                    }
                }


            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString(), "Logger.WriteToDb", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

        }
    }
}
