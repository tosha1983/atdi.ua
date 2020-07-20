using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;

namespace Atdi.Icsm.Plugins.Core
{
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
