using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
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
				data.Result, area.AxisY.Number);

			//if (data.CheckReverse)
			//{
			//	var tempPoint = data.Point;
			//	data.Point = data.Target;
			//	data.Target = tempPoint;

			//	var forwardIndexers = new Indexer[count];
			//	Array.Copy(data.Result, 0, forwardIndexers, 0, count);

			//	var reverseCount = Calculate(data.Point, data.Target, area.LowerLeft, area.AxisX.Step, area.AxisY.Step,
			//		data.Result, area.AxisY.Number);


			//	var countErrors = 0;
			//	var xErrors = 0;
			//	var yErrors = 0;

				
			//	if (count != reverseCount)
			//	{
			//		++countErrors;
			//		//System.Diagnostics.Debug.WriteLine($"CHECK REVERSE PROFILE: Point={tempPoint}, Target={data.Point}");
			//		//System.Diagnostics.Debug.WriteLine($"CHECK REVERSE PROFILE: There was a mismatch. ForwardCount={count}, reverseCount={reverseCount}");
			//	}
			//	else
			//	{
			//		for (int i = 0; i < count; i++)
			//		{
			//			var forward = forwardIndexers[i];
			//			var reverse = data.Result[count - i - 1];
			//			if (forward.XIndex != reverse.XIndex)
			//			{
			//				//System.Diagnostics.Debug.WriteLine($"CHECK REVERSE PROFILE: Point={tempPoint}, Target={data.Point}");
			//				//System.Diagnostics.Debug.WriteLine($"CHECK REVERSE PROFILE: There was a mismatch. ForwardXIndex={forward.XIndex}, ReverseXIndex={reverse.XIndex}");
			//				++xErrors;
			//			}
			//			if (forward.YIndex != reverse.YIndex)
			//			{
			//				//System.Diagnostics.Debug.WriteLine($"CHECK REVERSE PROFILE: Point={tempPoint}, Target={data.Point}");
			//				//System.Diagnostics.Debug.WriteLine($"CHECK REVERSE PROFILE: There was a mismatch. ForwardYIndex={forward.YIndex}, ReverseYIndex={reverse.YIndex}");
			//				++yErrors;
			//			}
			//		}
			//	}

				
			//	Array.Copy(forwardIndexers, 0, data.Result, 0, count);
			//	tempPoint = data.Point;
			//	data.Point = data.Target;
			//	data.Target = tempPoint;

			//	if (countErrors > 0 || xErrors > 0 || yErrors > 0)
			//	{
			//		data.HasError = true;
			//		var rule = "Rule 0";
			//		if (data.Target.Y > data.Point.Y && data.Target.X > data.Point.X)
			//		{
			//			rule = "Rule 1";
			//		}
			//		else if (data.Target.Y < data.Point.Y && data.Target.X < data.Point.X)
			//		{
			//			rule = "Rule 2";
			//		}
			//		else if (data.Target.Y > data.Point.Y && data.Target.X < data.Point.X)
			//		{
			//			rule = "Rule 3";
			//		}
			//		else if (data.Target.Y < data.Point.Y && data.Target.X > data.Point.X)
			//		{
			//			rule = "Rule 4";
			//		}
			//		System.Diagnostics.Debug.WriteLine($"CHECK MISMATCH REVERSE PROFILE: Rule={rule}, Point={tempPoint}, Target={data.Point}, XCount={xErrors}, YCount={yErrors}, ForwardCount={count}, Count{countErrors}, ReverseCount={reverseCount}");
			//	}
			//	else
			//	{
			//		data.HasError = false;
			//	}
			//}
			return count;
		}

		private static int Calculate(
			AtdiCoordinate point,
			AtdiCoordinate target,
			AtdiCoordinate location, 
			decimal axisXStep, 
			decimal axisYStep,
			ProfileIndexer[] indexers,
			int axisYNumber)
		{
			var count = 0;
			var xTargetIndex = (int)Math.Floor((target.X - location.X ) / axisXStep);
			var yTargetIndex = (int)Math.Floor((target.Y - location.Y) / axisYStep);

			var xPointIndex = (int)Math.Floor((point.X - location.X) / axisXStep);
			var yPointIndex = (int)Math.Floor((point.Y - location.Y) / axisYStep);

			var xCurrentIndex = xPointIndex;
			var yCurrentIndex = yPointIndex;

			var xDelta = target.X - point.X;
			var yDelta = target.Y - point.Y;

			// уменьшаем на одну операцию
			axisYNumber -= 1;

			// бредовый случай, но все же одна точка профиля есть
			if (point.X == target.X && point.Y == target.Y)
			{
				indexers[count++] = new ProfileIndexer()
				{
					XIndex = xCurrentIndex,
					YIndex = axisYNumber - yCurrentIndex 
				};
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
							indexers[count++] = new ProfileIndexer()
							{
								XIndex = xCurrentIndex,
								YIndex = axisYNumber - yCurrentIndex
							};
						}
					}
					else
					{
						for (xCurrentIndex = xPointIndex, yCurrentIndex = yPointIndex;
							xCurrentIndex <= xTargetIndex;
							xCurrentIndex++, yCurrentIndex--)
						{
							indexers[count++] = new ProfileIndexer()
							{
								XIndex = xCurrentIndex,
								YIndex = axisYNumber - yCurrentIndex 
							};
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
							indexers[count++] = new ProfileIndexer()
							{
								XIndex = xCurrentIndex,
								YIndex = axisYNumber - yCurrentIndex
							};
						}
					}
					else
					{
						for (xCurrentIndex = xPointIndex, yCurrentIndex = yPointIndex;
							xCurrentIndex >= xTargetIndex;
							xCurrentIndex--, yCurrentIndex--)
						{
							indexers[count++] = new ProfileIndexer()
							{
								XIndex = xCurrentIndex,
								YIndex = axisYNumber - yCurrentIndex
							};
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
						indexers[count++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
					}
				}
				else
				{
					for (yCurrentIndex = yPointIndex; yCurrentIndex >= yTargetIndex; yCurrentIndex--)
					{
						indexers[count++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
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
						indexers[count++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
					}
				}
				else
				{
					for (xCurrentIndex = xPointIndex; xCurrentIndex >= xTargetIndex; xCurrentIndex--)
					{
						indexers[count++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
					}
				}
			}
			else
			{
				// будет математика
				//var k = Math.Abs((double)yDelta / (double)xDelta);

				var xAbsDelta = Math.Abs(xDelta);
				var yAbsDelta = Math.Abs(yDelta);

				// берем первое значение для опорной точки

				indexers[count++] = new ProfileIndexer()
				{
					XIndex = xCurrentIndex,
					YIndex = axisYNumber - yCurrentIndex
				};

				if (target.Y > point.Y && target.X > point.X)
				{
					var xd = (point.X - location.X) / axisXStep - 1;
					var yd = (point.Y - location.Y) / axisYStep - 1;
					
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var left =  (yCurrentIndex - yd) * xAbsDelta;
						var right = (xCurrentIndex - xd) * yAbsDelta;
						//var a = ((yCurrentIndex + 1 - yd) * xAbsDelta > ((xCurrentIndex + 1 - xd) * yAbsDelta));
						if (left > right)
						{
							++xCurrentIndex;
						}
						else //if (left <= right)
						{
							++yCurrentIndex;
						}

						if (xCurrentIndex > xTargetIndex || yCurrentIndex > yTargetIndex)
						{
							System.Diagnostics.Debug.WriteLine($"Something went wrong: Rule=1, Point=[{point}], Target=[{target}], xPointIndex={xPointIndex}, xCurrentIndex={xCurrentIndex}, xTargetIndex={xTargetIndex}, yPointIndex={yPointIndex}, yCurrentIndex={yCurrentIndex}, yTargetIndex={yTargetIndex}");
							break;
							//throw new InvalidOperationException($"Something went wrong: xCurrentIndex = {xCurrentIndex}, xTargetIndex = {xTargetIndex}, yCurrentIndex = {yCurrentIndex}, yTargetIndex = {yTargetIndex}");
						}

						indexers[count++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
					}
				}
				else if (target.Y < point.Y && target.X < point.X)
				{
					var xd = (target.X - location.X) / axisXStep;
					var yd = (target.Y - location.Y) / axisYStep;

					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var left = (yCurrentIndex - yd) * xAbsDelta;
						var right = (xCurrentIndex - xd) * yAbsDelta;
						//var a = (yCurrentIndex - yd ) * xAbsDelta <= ((xCurrentIndex - xd) * yAbsDelta);
						if (left <= right)
						{
							--xCurrentIndex;
						}
						else //if (left >= right)
						{
							--yCurrentIndex;
						}

						if (xCurrentIndex < xTargetIndex || yCurrentIndex < yTargetIndex)
						{
							System.Diagnostics.Debug.WriteLine($"Something went wrong: Rule=2, Point=[{point}], Target=[{target}], xPointIndex={xPointIndex}, xCurrentIndex={xCurrentIndex}, xTargetIndex={xTargetIndex}, yPointIndex={yPointIndex}, yCurrentIndex={yCurrentIndex}, yTargetIndex={yTargetIndex}");
							break;
						}

						indexers[count++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
					}
				}
				else if (target.Y > point.Y && target.X < point.X)
				{
					var xd =  (point.X - location.X) / axisXStep;
					var yd = (point.Y - location.Y) / axisYStep - 1;
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var left = (yCurrentIndex - yd) * xAbsDelta;
						var right = (xd - xCurrentIndex) * yAbsDelta;

						//var a = (yCurrentIndex + 1 - yd) * xAbsDelta > ((xd - xCurrentIndex) * yAbsDelta);
						if (left > right)
						{
							--xCurrentIndex;
						}
						else //if (left <= right)
						{
							++yCurrentIndex;
						}

						if (xCurrentIndex < xTargetIndex || yCurrentIndex > yTargetIndex)
						{
							System.Diagnostics.Debug.WriteLine($"Something went wrong: Rule=3, Point=[{point}], Target=[{target}], xPointIndex={xPointIndex}, xCurrentIndex={xCurrentIndex}, xTargetIndex={xTargetIndex}, yPointIndex={yPointIndex}, yCurrentIndex={yCurrentIndex}, yTargetIndex={yTargetIndex}");
							break;
						}

						indexers[count++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
					}
				}
				else if (target.Y < point.Y && target.X > point.X)
				{
					var xd = (target.X - location.X) / axisXStep - 1;
					var yd = (target.Y - location.Y) / axisYStep;

					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var left = (yCurrentIndex - yd) * xAbsDelta;
						var right = (xd - xCurrentIndex) * yAbsDelta;
						//var a = (yCurrentIndex - yd) * xAbsDelta <= ((xd - xCurrentIndex - 1) * yAbsDelta);
						if (left <= right)
						{
							++xCurrentIndex;
						}
						else //if (left >= right)
						{
							--yCurrentIndex;
						}

						if (xCurrentIndex > xTargetIndex || yCurrentIndex < yTargetIndex)
						{
							System.Diagnostics.Debug.WriteLine($"Something went wrong: Rule=4, Point=[{point}], Target=[{target}], xPointIndex={xPointIndex}, xCurrentIndex={xCurrentIndex}, xTargetIndex={xTargetIndex}, yPointIndex={yPointIndex}, yCurrentIndex={yCurrentIndex}, yTargetIndex={yTargetIndex}");
							break;
							//throw new InvalidOperationException($"Something went wrong: xCurrentIndex = {xCurrentIndex}, xTargetIndex = {xTargetIndex}, yCurrentIndex = {yCurrentIndex}, yTargetIndex = {yTargetIndex}");
						}

						indexers[count++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
					}
				}
				else
				{
					System.Diagnostics.Debug.WriteLine($"Something went wrong. This is Unknown rule");
				}
			}

			return count;
		}
	}
}
