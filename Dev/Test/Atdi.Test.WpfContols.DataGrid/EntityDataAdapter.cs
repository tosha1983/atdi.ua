using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Icsm.Plugins.Core
{
	public abstract class EntityDataAdapter<TModel> : IEnumerable, IEnumerator, INotifyCollectionChanged
		where TModel : new()
	{

		private TModel[] _data;
		private int _currentIndex;


		protected EntityDataAdapter()
		{
			this._data = new TModel[] { };
			this._currentIndex = -1;
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public IEnumerator GetEnumerator()
		{
			return this;
		}

		public object Current
		{
			get
			{
				if (this._currentIndex == -1 || this._data == null || this._currentIndex >= this._data.Length)
				{
					return null;
				}

				var row = this._data[this._currentIndex];
				if (row == null)
				{
					row = this.GetRow(this._currentIndex);
					this._data[this._currentIndex] = row;
				}

				return row;
			}
		}

		public bool MoveNext()
		{
			if (this._data != null && this._data.Length > (this._currentIndex + 1))
			{
				++this._currentIndex;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			this._currentIndex = -1;
		}

		private TModel GetRow(int index)
		{
			
			return this.ReadData(index);
		}

		public void Refresh(int count)
		{
			try
			{
				this._data = new TModel[count];
				this._currentIndex = -1;
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			
			catch (Exception e)
			{
				throw;
			}
		}

	
		protected abstract TModel ReadData(int index);
	}
}

