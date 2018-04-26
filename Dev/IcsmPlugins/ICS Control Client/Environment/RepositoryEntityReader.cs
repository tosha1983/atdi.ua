using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;

namespace XICSM.ICSControlClient.Environment
{
    public class RepositoryEntityReader<T> : IDisposable
        where T : class, IRepositoryEntity, IRepositoryReadedEntity, new()
    {
        private readonly OpenedRecordsetScope _scope;
        private readonly IMRecordset _source;
        private T _current;
        public RepositoryEntityReader(IMRecordset source)
        {
            this._source = source;
            this._scope = new OpenedRecordsetScope(source);
        }

        public T GetEntity()
        {
            return this._current;
        }

        public bool Read()
        {
            if (this._source.IsEOF())
            {
                return false;
            }

            if (this._current != null)
            {
                this._source.MoveNext();

                if (this._source.IsEOF())
                {
                    return false;
                }
            }

            this._current = new T();
            this._current.LoadFromRecordset(this._source);

            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._scope.Dispose();
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
