using Atdi.Icsm.Plugins.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WpfContols.WinForm
{
	class MainView : ViewBase
	{
		public MainView()
		{
			this.LocalDataAdapter = new RecordDataAdapter();
			this.LocalDataAdapter.Refresh(1000);
		}

		public override void Dispose()
		{
			;
			;
		}


		public RecordDataAdapter LocalDataAdapter
		{
			get; set;

		}
	}


	public enum RecordStatus
	{
		Status0 = 0,
		Status1,
		Status2,
		Status3,
		Status4,
		Status5
	}
	public class Record
	{
		public int Id { get; set; }
		public Guid Guid { get; set; }
		public string Title { get; set; }
		public RecordStatus Status { get; set; }
		public DateTimeOffset Created { get; set; }
		public float ValueAsFloat { get; set; }
		public double ValueAsDouble { get; set; }
		public bool ValueAsBool { get; set; }
		public string Note { get; set; }
	}

	public class RecordDataAdapter : EntityDataAdapter<Record>
	{
		private Random r = new Random();
		protected override Record ReadData(int index)
		{
			return new Record
			{
				Id = index,
				Created = DateTimeOffset.Now,
				Guid = Guid.NewGuid(),
				Status = (RecordStatus) r.Next(0, 5),
				Title = $"Title #{index}",
				Note = $"The some record of data {index}.",
				ValueAsBool = Convert.ToBoolean(r.Next(-1, 1)),
				ValueAsFloat = Convert.ToSingle(r.NextDouble()),
				ValueAsDouble = r.NextDouble()
			};
		}
	}
}
