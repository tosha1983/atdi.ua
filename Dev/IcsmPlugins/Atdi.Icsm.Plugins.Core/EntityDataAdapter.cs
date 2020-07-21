using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.Core
{
	public abstract class EntityDataAdapter<TEntity, TModel> : IEnumerable, IEnumerator, INotifyCollectionChanged
		where TModel : new()
	{
		private readonly WebApiDataLayer _dataLayer;
		private readonly ILogger _logger;
		private TModel[] _data;
		private int _currentIndex;
		private IDataReader<TEntity> _reader;

		protected EntityDataAdapter(WebApiDataLayer dataLayer, ILogger logger)
		{
			_dataLayer = dataLayer;
			_logger = logger;
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
			if (_reader == null)
			{
				throw new InvalidOperationException("Undefined current date reader");
			}

			if (!_reader.MoveTo(index))
			{
				// тут могут быть разные сценарии, напримр загрузка следующей страницы
				// но пока падаем
				throw new InvalidOperationException($"Index #{index} out of range for current date reader");
			}

			return this.ReadData(_reader, index);
		}

		public void Refresh()
		{
			try
			{
				var query = _dataLayer.GetBuilder<TEntity>().Read();
				this.PrepareQuery(query);
				this._reader = _dataLayer.Executor.ExecuteReader(query);
				this._data = new TModel[_reader.Count];
				this._currentIndex = -1;
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
            catch (EntityOrmWebApiException e)
            {
                if (e.ServerException != null)
                {
                    _logger.Exception((EventContext)"EntityDataAdapter", (EventCategory)"Refresh", new Exception($"ExceptionMessage: {e.ServerException.ExceptionMessage}; ExceptionType: {e.ServerException.ExceptionType}; InnerException: {e.ServerException.InnerException}; Message: {e.ServerException.Message}!"));
                }
            }
			catch (Exception e)
			{
				_logger.Exception((EventContext)"EntityDataAdapter", (EventCategory)"Refresh", e);
				throw;
			}
		}

		protected abstract void PrepareQuery(IReadQuery<TEntity> query);

		protected abstract TModel ReadData(IDataReader<TEntity> reader, int index);
	}
}
