using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal sealed class CheckPoint : ICheckPoint
	{
		private readonly bool _isReadonly;
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly IDataLayerScope _calcDbScope;

		public CheckPoint(long id, string name, bool isReadonly, IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer, IDataLayerScope calcDbScope)
		{
			this._calcServerDataLayer = calcServerDataLayer;
			this._calcDbScope = calcDbScope;
			this.Id = id;
			this.Name = name;
			this._isReadonly = isReadonly;
		}

		public long Id {get;}
		public string Name { get; }

		public void Commit()
		{
			if (_isReadonly)
			{
				return;
			}
			this.SetToAvailable();
		}

		public void Dispose()
		{
			_calcDbScope.Dispose();
		}

		public T RestoreData<T>(string context)
		{
			var selQuery = _calcServerDataLayer.GetBuilder<ICalcCheckPointData>()
				.From()
				.Select(c => c.DataJson)
				.Where(c => c.CHECKPOINT.Id, ConditionOperator.Equal, this.Id)
				.Where(c =>c.DataContext, ConditionOperator.Equal, context);

			var data = _calcDbScope.Executor.ExecuteAndFetch(selQuery, reader =>
			{
				if (!reader.Read())
				{
					return default(T);
				}

				return reader.GetValueAs<T>(c => c.DataJson);
			});

			return data;
		}

		public void SaveData<T>(string context, T data)
		{
			if (_isReadonly)
			{
				return;
			}

			var insQuery = _calcServerDataLayer.GetBuilder<ICalcCheckPointData>()
				.Insert()
				.SetValue(c => c.CHECKPOINT.Id, this.Id)
				.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
				.SetValue(c => c.DataContext, context)
				.SetValueAsJson(c => c.DataJson, data);

			var count = _calcDbScope.Executor.Execute(insQuery);
			if (count == 0)
			{
				throw new InvalidOperationException($"New CheckPoint not found by ID #{this.Id} ");
			}
		}

		private void SetToAvailable()
		{
			var updQuery = _calcServerDataLayer.GetBuilder<ICalcCheckPoint>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)CalcCheckPointStatusCode.Available)
				.SetValue(c => c.StatusName, CalcCheckPointStatusCode.Available.ToString())

				.Where(c => c.Id, ConditionOperator.Equal, this.Id)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcCheckPointStatusCode.Created);

			var count = _calcDbScope.Executor.Execute(updQuery);

			if (count == 0)
			{
				throw new InvalidOperationException($"New CheckPoint not found by ID #{this.Id} ");
			}
		}
	}
}
