using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.CalcServer.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	/// <summary>
	/// Пример реализации итерации
	/// Объект создается один раз, т.е. его состояние глобальное
	/// это нужно учитывать при работе с БД если будет в таком необходимость
	/// т.е. открытваь всегда новое соединение в рамках вызова.  
	/// </summary>
	class CoverageProfileIteration : IIterationHandler<CoverageProfileData, VoidResult>
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Заказываем у контейнера нужные сервисы
		/// </summary>
		public CoverageProfileIteration(ILogger logger)
		{
			_logger = logger;
		}

		public VoidResult Run(ITaskContext taskContext, CoverageProfileData data)
		{

			//for (var i = 0; i < data.Points.Length; i++)
			//{
			//	var target = data.Points[i];

			//	var xDelta = target.X - data.Point.X;
			//	var yDelta = target.Y - data.Point.X;
			//	var k = (float)yDelta / (float)xDelta;

			//	var xPointIndex = data.Point.X  - target.X

			//}
			// обязательный возврат результата
			return VoidResult.Instance;
		}

		private static int Calculate(CoverageProfileData.Coordinate target, CoverageProfileData.Coordinate location, double axisXStep, double axisYStep,
			CoverageProfileData.Coordinate point, byte[] profile, byte[] buffer, CoverageProfileData.Axis axisX, int dataSize)
		{
			var count = 0;
			var xTargetIndex = (int)Math.Ceiling((target.X - location.X) / axisXStep) - 1;
			var yTargetIndex = (int)Math.Ceiling((target.Y - location.Y) / axisYStep) - 1;

			var xPointIndex = (int)Math.Ceiling((point.X - location.X) / axisXStep) - 1;
			var yPointIndex = (int)Math.Ceiling((point.Y - location.Y) / axisYStep) - 1;


			var xCurrentIndex = xPointIndex;
			var yCurrentIndex = yPointIndex;


			var xDelta = target.X - point.X;
			var yDelta = target.Y - point.Y;

			// бредовый случай, но все же одна точка профиля есть
			if (point.X == target.X && point.Y == target.Y)
			{
				var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
				profile[count++] = buffer[index];
				if (dataSize == 2)
				{
					profile[count++] = buffer[index + 1];
				}
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
							var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
							profile[count++] = buffer[index];
							if (dataSize == 2)
							{
								profile[count++] = buffer[index + 1];
							}
						}
					}
					else
					{
						for (xCurrentIndex = xPointIndex, yCurrentIndex = yPointIndex;
							xCurrentIndex <= xTargetIndex;
							xCurrentIndex++, yCurrentIndex--)
						{
							var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
							profile[count++] = buffer[index];
							if (dataSize == 2)
							{
								profile[count++] = buffer[index + 1];
							}
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
							var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
							profile[count++] = buffer[index];
							if (dataSize == 2)
							{
								profile[count++] = buffer[index + 1];
							}
						}
					}
					else
					{
						for (xCurrentIndex = xPointIndex, yCurrentIndex = yPointIndex;
							xCurrentIndex >= xTargetIndex;
							xCurrentIndex--, yCurrentIndex--)
						{
							var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
							profile[count++] = buffer[index];
							if (dataSize == 2)
							{
								profile[count++] = buffer[index + 1];
							}
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
						var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						profile[count++] = buffer[index];
						if (dataSize == 2)
						{
							profile[count++] = buffer[index + 1];
						}
					}
				}
				else
				{
					for (yCurrentIndex = yPointIndex; yCurrentIndex >= yTargetIndex; yCurrentIndex--)
					{
						var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						profile[count++] = buffer[index];
						if (dataSize == 2)
						{
							profile[count++] = buffer[index + 1];
						}
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
						var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						profile[count++] = buffer[index];
						if (dataSize == 2)
						{
							profile[count++] = buffer[index + 1];
						}
					}
				}
				else
				{
					for (xCurrentIndex = xPointIndex; xCurrentIndex >= xTargetIndex; xCurrentIndex--)
					{
						var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						profile[count++] = buffer[index];
						if (dataSize == 2)
						{
							profile[count++] = buffer[index + 1];
						}
					}
				}
			}
			else
			{
				// будет математика
				var k = (double)yDelta / (double)xDelta;
				// берем первое значение дпо опорной точке
				profile[count++] = buffer[yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize];
				if (dataSize == 2)
				{
					profile[count++] = buffer[yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize + 1];
				}

				if (target.Y > point.Y && target.X > point.X)
				{
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var a = ((yCurrentIndex + 1) - ((xCurrentIndex + 1) * k)) > 0;
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
						var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						profile[count++] = buffer[index];
						if (dataSize == 2)
						{
							profile[count++] = buffer[index + 1];
						}
					}
				}
				else if (target.Y < point.Y && target.X < point.X)
				{
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var a = (yCurrentIndex - 1) < ((xCurrentIndex - 1) * k);
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
						var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						profile[count++] = buffer[index];
						if (dataSize == 2)
						{
							profile[count++] = buffer[index + 1];
						}
					}
				}
				else if (target.Y > point.Y && target.X < point.X)
				{
					var d = (target.X - location.X) % axisXStep;
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var a = (yCurrentIndex + 1) > ((xCurrentIndex - d) * k);
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
						var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						profile[count++] = buffer[index];
						if (dataSize == 2)
						{
							profile[count++] = buffer[index + 1];
						}
					}
				}
				else if (target.Y < point.Y && target.X > point.X)
				{
					var d = (target.X - location.X) % axisXStep;
					while (xCurrentIndex != xTargetIndex || yCurrentIndex != yTargetIndex)
					{
						var a = (yCurrentIndex - 1) > ((xCurrentIndex - d) * k);
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
						var index = yCurrentIndex * axisX.Number * dataSize + xCurrentIndex * dataSize;
						profile[count++] = buffer[index];
						if (dataSize == 2)
						{
							profile[count++] = buffer[index + 1];
						}
					}
				}
			}

			return count;
		}
	}
}
