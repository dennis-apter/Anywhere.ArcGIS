using System.Collections.Generic;
using System.Linq;

namespace Anywhere.ArcGIS.Common
{
    public sealed class Path : Geometry
    {
        public Path()
        {
        }

        public Path(IEnumerable<Point> points)
        {
            Points = new List<Point>(points);
        }

        public List<Point> Points { get; set; }

        public override Extent GetExtent()
        {
            return Points.GetExtent();
        }

        public override IEnumerable<Point> GetBoundingPoints()
        {
            return Points;
        }

        public override GeometryType Type => GeometryType.Path;

        protected override object CloneImpl() => Clone();

        public Path Clone()
        {
            return new Path
            {
                Points = Points?.Select(p => p.Clone()).ToList()
            };
        }
    }
}
