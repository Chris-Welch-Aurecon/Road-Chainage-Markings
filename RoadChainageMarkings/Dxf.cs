using Objects.Geometry;
using Objects.Other;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RoadChainageMarkings.Geometry;

namespace RoadChainageMarkings
{

	internal static class ModelBuilder
	{

		internal static Base CreateNewModel(this IEnumerable<Polyline> polylines, double division, string filepath)
		{
			var @base = new Base();	

			var alignments = new List<Base>();

			foreach (var polyline in polylines)
			{
				var alignment = new Base();

				var frames = polyline.FramesAtDistance(division);

				var lines = frames.ConvertAll(x => x.CreateLine(1.2));
				//var texts = frames.ConvertAll(x => x.CreateText());
				alignment[nameof(lines)] = lines;
				//alignment[nameof(texts)] = texts;

				alignments.Add(alignment);

			}

			@base[nameof(alignments)] = alignments;

			return @base;
		}


		internal static Line CreateLine(this Geometry.PointDirection pd, double length)
		{
			var (x, y, z) = (pd.Point.x, pd.Point.y, pd.Point.z);
			var (xv, yv) = (pd.Vector.y, -pd.Vector.x);

			var line = new Line(new Point(
				x: x + xv * length,
				y: y + yv * length,
				z: z),
				new Point(
				x: x - xv * length,
				y: y - yv * length,
				z: z))
			{
				
			};

			return line;
		}

		internal static Text CreateText(this PointDirection pd)
		{
			throw new NotImplementedException();

			//var (x, y, z, t, u, v, w) = (pd.Point.x, pd.Point.y, pd.Point.z, pd.Distance, pd.Vector.x, pd.Vector.y, pd.Vector.z);
			//
			//var textEntity = new Text()
			//{
			//	f
			//}
			//	
			//	
			//	(t.ToString(), new Vector3(x, y, z), height: 1.2)
			//{
			//	Color = new AciColor(1, 1, 1),
			//	Alignment = netDxf.Entities.TextAlignment.BottomCenter,
			//	Normal = new Vector3(u, v, w)
			//};
			//
			//return textEntity;
		}
	}

}
