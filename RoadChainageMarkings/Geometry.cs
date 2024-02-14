using Objects.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadChainageMarkings
{
	internal static class Geometry
	{

		internal record PointDirection(Point Point, Vector Vector, double Distance)
		{

		}

		internal static Point Interpolate(this Point startPoint, Point endPoint, double factor, string units)
		{
			double interpolatedX = startPoint.x + (endPoint.x - startPoint.x) * factor;
			double interpolatedY = startPoint.y + (endPoint.y - startPoint.y) * factor;
			double interpolatedZ = startPoint.z + (endPoint.z - startPoint.z) * factor;

			return new Point(interpolatedX, interpolatedY, interpolatedZ, units);
		}

		internal static List<PointDirection> FramesAtDistance(this Polyline p, double distance)
		{
			var points = p.GetPoints();

			var pairwise = points.Zip(points.Skip(1));

			var distanceUntilNextPoint = distance;
			var totalDistance = 0.0;

			var frames = new List<PointDirection>();

			foreach (var (a, b) in pairwise)
			{
				var d = a.DistanceTo(b);

				if (d == 0.0) continue;

				if (distanceUntilNextPoint <= d)
				{
					// need to fix this so multiple divisions can happen on a segment

					var intermediatePoint = a.Interpolate(b, distanceUntilNextPoint / d, p.units);
					frames.Add(new(intermediatePoint, new Vector(b.x - a.x, b.y - a.y, b.z - a.z, p.units), distanceUntilNextPoint));
					distanceUntilNextPoint = distance;
				}
				else
				{
					distanceUntilNextPoint -= d;
				}

				totalDistance += distanceUntilNextPoint;
			}

			return frames;
		}
	}
}
