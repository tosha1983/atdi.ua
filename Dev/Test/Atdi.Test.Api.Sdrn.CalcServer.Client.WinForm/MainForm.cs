using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.WinForm
{
	public partial class MainForm : Form
	{
		private ResultProfile _profile;

		public MainForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				var body = File.ReadAllText(openFileDialog.FileName);
				_profile = JsonConvert.DeserializeObject<ResultProfile>(body);

				

				var graphics = this.CreateGraphics();

				DrawProfile(graphics, _profile);
			}
		}

		private void DrawProfile(Graphics graphics, ResultProfile profile)
		{
			Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
			

			var xStart = 10;
			var yStart = 10;

			//var xWidth = 1000;
			var yWidth = 400;

			yStart = yWidth + 200;

			//graphics.DrawLine(pen, xStart, yStart, xStart + xWidth, yStart);
			//graphics.DrawLine(pen, xStart, yStart - yWidth, xStart + xWidth, yStart - yWidth);

			//graphics.DrawLine(pen, xStart, yStart, xStart, yStart - yWidth);
			//graphics.DrawLine(pen, xStart + xWidth, yStart, xStart + xWidth, yStart - yWidth);

			Pen pen2 = new Pen(Color.CadetBlue);
			Pen pen3 = new Pen(Color.LightSlateGray);
			for (int i = 0; i < profile.Records.Length; i++)
			{
				var record = profile.Records[i];

				

				graphics.DrawLine(pen3, xStart + i, yStart, xStart + i, yStart - record.Relief );

				graphics.DrawLine(pen2, xStart + i, yStart - record.Relief, xStart + i, yStart - (record.Relief + record.Building));
			}
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			if (_profile != null)
			{
				DrawProfile(e.Graphics, _profile);
			}
		}
	}

	public struct Indexer
	{
		public int XIndex;
		public int YIndex;

		public override string ToString()
		{
			return $"[{YIndex:D4}:{XIndex:D4}]";
		}
	}

	public struct Coordinate
	{
		public int X;
		public int Y;

		public override string ToString()
		{
			return $"({X},{Y})";
		}
	}

	public class ResultProfile
	{
		public Coordinate Point;
		public Coordinate Target;

		public ResultProfileRecord[] Records { get; set; }

		public int Count { get; set; }
	}

	public struct ResultProfileRecord
	{
		public int Num;
		public int Index;
		public Indexer Indexer;
		public short Relief;
		public byte Clutter;
		public byte Building;
	};
}
