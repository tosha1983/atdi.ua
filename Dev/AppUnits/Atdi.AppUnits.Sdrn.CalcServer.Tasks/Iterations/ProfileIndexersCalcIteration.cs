using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.CalcServer.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	/// <summary>
	/// Пример реализации итерации
	/// Объект создается один раз, т.е. его состояние глобальное
	/// это нужно учитывать при работе с БД если будет в таком необходимость
	/// т.е. открытваь всегда новое соединение в рамках вызова.  
	/// </summary>
	public class ProfileIndexersCalcIteration : IIterationHandler<ProfileIndexersCalcData, int>
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Заказываем у контейнера нужные сервисы
		/// </summary>
		public ProfileIndexersCalcIteration(ILogger logger)
		{
			_logger = logger;
		}

		public int Run(ITaskContext taskContext, ProfileIndexersCalcData data)
		{
			var area = data.Area;
			var count = Calculate(data.Point, data.Target, area.LowerLeft, area.AxisX.Step, area.AxisY.Step,
				data.Result, area.AxisX.Number, area.AxisY.Number);
			return count;
		}

		private static int Calculate(
			Coordinate point, 
			Coordinate target, 
			Coordinate location, 
			double axisXStep, 
			double axisYStep,
			Indexer[] indexers, 
			int axisXNumber,
			int axisYNumber)
		{
			var count = 0;
			var xTargetIndex = (int)Math.Ceiling((target.X - location.X ) / axisXStep) - 1;
			var yTargetIndex = (int)Math.Ceiling((target.Y - location.Y) / axisYStep) - 1;

			var xPointIndex = (int)Math.Ceiling((point.X - location.X) / axisXStep) - 1;
			var yPointIndex = (int)Math.Ceiling((point.Y - location.Y) / axisYStep) - 1;

			//var __XIndex = (int)Math.Ceiling((x - this.UpperLeftX + 1) / (double)this.AxisXStep) - 1,
			//YIndex = this.AxisYNumber - ((int)Math.Ceiling((y - (this.UpperLeftY - (this.AxisYNumber * this.AxisYStep)) + 1) / (double)this.AxisYStep))

			var xCurrentIndex = xPointIndex;
			var yCurrentIndex = yPointIndex;


			var xDelta = target.X - point.X;
			var yDelta = target.Y - point.Y;

			// бредовый случай, но все же одна точка профиля есть
			if (point.X == target.X && point.Y == target.Y)
			{
				indexers[count++] = new Indexer()
				{
					XIndex = xCurrentIndex,
					YIndex = axisYNumber - yCurrentIndex - 1
				};
				//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
				//profile[count++] = buffer[index];
				//if (dataSize == 2)
				//{
				//	profile[count++] = buffer[index + 1];
				//}
			}
			// особый случай, по диаганаль, ровный цыкл
			else if (xDelta == yDelta)
			{
				// тут 4 направления
				if (xPointIndex < xTargetIndex)
				{
					if (yPointIndex < yTargetIndex)
					{
						for (xCurrentIndex = xPointIndex, yCurrentIndex = yPointIndex;
							xCurrentIndex <= xTargetIndex;
							xCurrentIndex++, yCurrentIndex++)
						{
							indexers[count++] = new Indexer()
							{
								XIndex = xCurrentIndex,
								YIndex = axisYNumber - yCurrentIndex - 1
							};
							//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
							//profile[count++] = buffer[index];
							//if (dataSize == 2)
							//{
							//	profile[count++] = buffer[index + 1];
							//}
						}
					}
					else
					{
						for (xCurrentIndex = xPointIndex, yCurrentIndex = yPointIndex;
							xCurrentIndex <= xTargetIndex;
							xCurrentIndex++, yCurrentIndex--)
						{
							indexers[count++] = new Indexer()
							{
								XIndex = xCurrentIndex,
								YIndex = axisYNumber - yCurrentIndex - 1
							};
							//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
							//profile[count++] = buffer[index];
							//if (dataSize == 2)
							//{
							//	profile[count++] = buffer[index + 1];
							//}
						}
					}
				}
				else
				{
					if (yPointIndex < yTargetIndex)
					{
						for (xCurrentIndex = xPointIndex, yCurrentIndex = yPointIndex;
							xCurrentIndex >= xTargetIndex;
							xCurrentIndex--, yCurrentIndex++)
						{
							indexers[count++] = new Indexer()
							{
								XIndex = xCurrentIndex,
								YIndex = axisYNumber - yCurrentIndex - 1
							};
							//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
							//profile[count++] = buffer[index];
							//if (dataSize == 2)
							//{
							//	profile[count++] = buffer[index + 1];
							//}
						}
					}
					else
					{
						for (xCurrentIndex = xPointIndex, yCurrentIndex = yPointIndex;
							xCurrentIndex >= xTargetIndex;
							xCurrentIndex--, yCurrentIndex--)
						{
							indexers[count++] = new Indexer()
							{
								XIndex = xCurrentIndex,
								YIndex = axisYNumber - yCurrentIndex - 1
							};

							//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
							//profile[count++] = buffer[index];
							//if (dataSize == 2)
							//{
							//	profile[count++] = buffer[index + 1];
							//}
						}
					}
				}
			}
			// по оси Х
			else if (point.X == target.X)
			{
				// тут два направления
				if (yPointIndex < yTargetIndex)
				{
					for (yCurrentIndex = yPointIndex; yCurrentIndex <= yTargetIndex; yCurrentIndex++)
					{
						indexers[count++] = new Indexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex - 1
						};

						//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						//profile[count++] = buffer[index];
						//if (dataSize == 2)
						//{
						//	profile[count++] = buffer[index + 1];
						//}
					}
				}
				else
				{
					for (yCurrentIndex = yPointIndex; yCurrentIndex >= yTargetIndex; yCurrentIndex--)
					{
						indexers[count++] = new Indexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex - 1
						};

						//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						//profile[count++] = buffer[index];
						//if (dataSize == 2)
						//{
						//	profile[count++] = buffer[index + 1];
						//}
					}
				}
			}
			// по оси У
			else if (point.Y == target.Y)
			{
				// тут два направления
				if (xPointIndex < xTargetIndex)
				{
					for (xCurrentIndex = xPointIndex; xCurrentIndex <= xTargetIndex; xCurrentIndex++)
					{
						indexers[count++] = new Indexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex - 1
						};

						//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						//profile[count++] = buffer[index];
						//if (dataSize == 2)
						//{
						//	profile[count++] = buffer[index + 1];
						//}
					}
				}
				else
				{
					for (xCurrentIndex = xPointIndex; xCurrentIndex >= xTargetIndex; xCurrentIndex--)
					{
						indexers[count++] = new Indexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex - 1
						};

						//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						//profile[count++] = buffer[index];
						//if (dataSize == 2)
						//{
						//	profile[count++] = buffer[index + 1];
						//}
					}
				}
			}
			else
			{
				// будет математика
				var k = Math.Abs((double)yDelta / (double)xDelta);
				// берем первое значение для опорной точки

				indexers[count++] = new Indexer()
				{
					XIndex = xCurrentIndex,
					YIndex = axisYNumber - yCurrentIndex - 1
				};

				//profile[count++] = buffer[yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize];
				//if (dataSize == 2)
				//{
				//	profile[count++] = buffer[yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize + 1];
				//}

				if (target.Y > point.Y && target.X > point.X)
				{
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var a = ((yCurrentIndex + 1) > ((xCurrentIndex + 1) * k));
						if (a)
						{
							++xCurrentIndex;
						}
						else
						{
							++yCurrentIndex;
						}

						if (xCurrentIndex > xTargetIndex || yCurrentIndex > yTargetIndex)
						{
							throw new InvalidOperationException($"Something went wrong: xCurrentIndex = {xCurrentIndex}, xTargetIndex = {xTargetIndex}, yCurrentIndex = {yCurrentIndex}, yTargetIndex = {yTargetIndex}");
						}

						//++count;
						// тут брать очередное значение из буфера
						indexers[count++] = new Indexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex - 1
						};

						//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						//profile[count++] = buffer[index];
						//if (dataSize == 2)
						//{
						//	profile[count++] = buffer[index + 1];
						//}
					}
				}
				else if (target.Y < point.Y && target.X < point.X)
				{
					var xd = (target.X - location.X) % axisXStep + xTargetIndex;
					var yd = (target.Y - location.Y) % axisYStep + yTargetIndex;
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var a = (yCurrentIndex - 1 - yd ) < ((xCurrentIndex - 1 - xd) * k);
						if (a)
						{
							--xCurrentIndex;
						}
						else
						{
							--yCurrentIndex;
						}

						if (xCurrentIndex < xTargetIndex || yCurrentIndex < yTargetIndex)
						{
							throw new InvalidOperationException($"Something went wrong: xCurrentIndex = {xCurrentIndex}, xTargetIndex = {xTargetIndex}, yCurrentIndex = {yCurrentIndex}, yTargetIndex = {yTargetIndex}");
						}

						//++count;
						// тут брать очередное значение из буфера
						indexers[count++] = new Indexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex - 1
						};

						//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						//profile[count++] = buffer[index];
						//if (dataSize == 2)
						//{
						//	profile[count++] = buffer[index + 1];
						//}
					}
				}
				else if (target.Y > point.Y && target.X < point.X)
				{
					var xd = (target.X - location.X) % axisXStep + xTargetIndex;
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var a = (yCurrentIndex + 1) > ((xCurrentIndex - 1 - xd) * k);
						if (a)
						{
							--xCurrentIndex;
						}
						else
						{
							++yCurrentIndex;
						}

						if (xCurrentIndex < xTargetIndex || yCurrentIndex > yTargetIndex)
						{
							throw new InvalidOperationException($"Something went wrong: xCurrentIndex = {xCurrentIndex}, xTargetIndex = {xTargetIndex}, yCurrentIndex = {yCurrentIndex}, yTargetIndex = {yTargetIndex}");
						}

						//++count;
						// тут брать очередное значение из буфера
						indexers[count++] = new Indexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex - 1
						};

						//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						//profile[count++] = buffer[index];
						//if (dataSize == 2)
						//{
						//	profile[count++] = buffer[index + 1];
						//}
					}
				}
				else if (target.Y < point.Y && target.X > point.X)
				{
					var yd = (target.Y - location.Y) % axisYStep + yTargetIndex;
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var a = (yCurrentIndex - 1 - yd) < ((xCurrentIndex + 1) * k);
						if (a)
						{
							++xCurrentIndex;
						}
						else
						{
							--yCurrentIndex;
						}

						if (xCurrentIndex > xTargetIndex || yCurrentIndex < yTargetIndex)
						{
							throw new InvalidOperationException($"Something went wrong: xCurrentIndex = {xCurrentIndex}, xTargetIndex = {xTargetIndex}, yCurrentIndex = {yCurrentIndex}, yTargetIndex = {yTargetIndex}");
						}

						//++count;
						// тут брать очередное значение из буфера
						indexers[count++] = new Indexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex - 1
						};

						//var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						//profile[count++] = buffer[index];
						//if (dataSize == 2)
						//{
						//	profile[count++] = buffer[index + 1];
						//}
					}
				}
			}

			return count;
		}
	}
}
