using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis.MapService;

namespace Atdi.AppUnits.Sdrn.DeepServices.Gis
{
	public class MapService : IMapService
	{
		public void CalcProfileIndexers(in CalcProfileIndexersArgs args, ref CalcProfileIndexersResult result)
		{
			result.IndexerCount = MapService.CalcProfileIndexers(
					in args.Point,
					in args.Target,
					in args.Location,
					in args.AxisXStep,
					in args.AxisXStep,
					result.Indexers,
					args.AxisYNumber,
					result.StartPosition
				);
		}

		private static int CalcProfileIndexers(
			in AtdiCoordinate point,
			in AtdiCoordinate target,
			in AtdiCoordinate location,
			in decimal axisXStep,
			in decimal axisYStep,
			ProfileIndexer[] indexers,
			int axisYNumber,
			int startPosition)
		{
			var indexerPosition = startPosition;
			var xTargetIndex = (int)Math.Floor((target.X - location.X) / axisXStep);
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
				indexers[indexerPosition++] = new ProfileIndexer()
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
							indexers[indexerPosition++] = new ProfileIndexer()
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
							indexers[indexerPosition++] = new ProfileIndexer()
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
							indexers[indexerPosition++] = new ProfileIndexer()
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
							indexers[indexerPosition++] = new ProfileIndexer()
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
						indexers[indexerPosition++] = new ProfileIndexer()
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
						indexers[indexerPosition++] = new ProfileIndexer()
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
						indexers[indexerPosition++] = new ProfileIndexer()
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
						indexers[indexerPosition++] = new ProfileIndexer()
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

				indexers[indexerPosition++] = new ProfileIndexer()
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
						var left = (yCurrentIndex - yd) * xAbsDelta;
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

						indexers[indexerPosition++] = new ProfileIndexer()
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

						indexers[indexerPosition++] = new ProfileIndexer()
						{
							XIndex = xCurrentIndex,
							YIndex = axisYNumber - yCurrentIndex
						};
					}
				}
				else if (target.Y > point.Y && target.X < point.X)
				{
					var xd = (point.X - location.X) / axisXStep;
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

						indexers[indexerPosition++] = new ProfileIndexer()
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

						indexers[indexerPosition++] = new ProfileIndexer()
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

			return indexerPosition - startPosition;
		}
	}
}
