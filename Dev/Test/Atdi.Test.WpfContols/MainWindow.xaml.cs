using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Atdi.Test.WpfContols
{
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

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		
		public MainWindow()
		{
			InitializeComponent();
			gridMain.ItemsSource = GenerateRecords(100);
		}

		private static List<Record> GenerateRecords(int count)
		{
			var result = new List<Record>(count);
			var r = new Random();
			for (int i = 1; i <= count; i++)
			{
				result.Add(new Record
				{
					Id = i,
					Created = DateTimeOffset.Now,
					Guid = Guid.NewGuid(),
					Status = (RecordStatus)r.Next(0, 5),
					Title =  $"Title #{i}",
					Note = $"The some record of data {i}.",
					ValueAsBool = Convert.ToBoolean(r.Next(-1, 1)),
					ValueAsFloat = Convert.ToSingle(r.NextDouble()),
					ValueAsDouble = r.NextDouble()
				});
			}
			return result;
		}
	}
}
