using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.Environment.Wpf
{
    public interface IWpfDataAdapter<TSource, TData> : IEnumerable, IEnumerator, INotifyCollectionChanged
    {
        TSource[] Source { get; set; }
    }

    public abstract class WpfDataAdapter<TSource, TData, TAdapter> : IWpfDataAdapter<TSource, TData>
        where TData : class, new()
        where TAdapter : WpfDataAdapter<TSource, TData, TAdapter>, new()

    {
        private TSource[] _source;
        private TData[] _data;
        private Func<TSource, TData> _mapper;
        private int _currentIndex;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public WpfDataAdapter()
        {
            this._data = new TData[] { };
            this._currentIndex = -1;
            this._mapper = this.GetMapper();
        }

        public TSource[] Source
        {
            get
            {
                return this._source;
            }

            set
            {
                this._currentIndex = -1;
                this._source = value;
                if (this._source != null)
                {
                    this._data = new TData[this._source.Length];
                }
                else
                {
                    this._data = new TData[] { };
                }

                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected abstract Func<TSource, TData> GetMapper();

        private TData this[int index]
        {
            get
            {
                if (this._data[index] == null)
                {
                    this._data[index] = this._mapper(this._source[index]);
                }
                return this._data[index];
            }
        }

        object IEnumerator.Current => this[this._currentIndex];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        bool IEnumerator.MoveNext()
        {
            if (this._source != null && this._source.Length > (this._currentIndex + 1))
            {
                ++this._currentIndex;
                return true;
            }
            return false;
        }

        void IEnumerator.Reset()
        {
            this._currentIndex = -1;
        }

        public static TAdapter Create(TSource[] source)
        {
            var adapter = new TAdapter();
            adapter.Source = source;
            return adapter;
        }
    }
}
