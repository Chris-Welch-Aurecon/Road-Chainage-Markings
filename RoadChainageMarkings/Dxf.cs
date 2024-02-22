using netDxf;
using Objects.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RoadChainageMarkings.Geometry;

namespace RoadChainageMarkings
{

	internal static class DxfBuilder
	{

		internal static void SaveToDxf(this IEnumerable<Polyline> polylines, double division, string filepath)
		{
			var dxf = new DxfDocument();

			foreach (var polyline in polylines)
			{
				var frames = polyline.FramesAtDistance(division);

				var lines = frames.ConvertAll(x => x.CreateLine(1.2));
				var texts = frames.ConvertAll(x => x.CreateText());
				dxf.Entities.Add(lines);
				dxf.Entities.Add(texts);
			}

			dxf.DrawingVariables.InsUnits = netDxf.Units.DrawingUnits.Meters;

			dxf.Save(filepath);
		}


		internal static netDxf.Entities.Line CreateLine(this Geometry.PointDirection pd, double length)
		{
			var (x, y, z) = (pd.Point.x, pd.Point.y, pd.Point.z);
			var (xv, yv) = (pd.Vector.y, -pd.Vector.x);

			var line = new netDxf.Entities.Line(new Vector3(
				x: x + xv * length,
				y: y + yv * length,
				z: z),
				new Vector3(
				x: x - xv * length,
				y: y - yv * length,
				z: z))
			{
				Color = new AciColor(1, 1, 1)
			};

			return line;
		}

		internal static netDxf.Entities.Text CreateText(this PointDirection pd)
		{
			var (x, y, z, t, u, v, w) = (pd.Point.x, pd.Point.y, pd.Point.z, pd.Distance, pd.Vector.x, pd.Vector.y, pd.Vector.z);

			var textEntity = new netDxf.Entities.Text(t.ToString(), new Vector3(x, y, z), height: 1.2)
			{
				Color = new AciColor(1, 1, 1),
				Alignment = netDxf.Entities.TextAlignment.BottomCenter,
				Normal = new Vector3(u, v, w)
			};

			return textEntity;
		}
	}

}
