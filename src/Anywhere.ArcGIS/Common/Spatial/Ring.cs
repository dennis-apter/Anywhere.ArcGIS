using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.Common
{
    [DataContract]
    public sealed class Ring : Geometry
    {
        public Ring()
        {
        }

        public Ring(IEnumerable<Point> points)
        {
            Points = new List<Point>(points);
        }

        [DataMember(Name = "points")]
        public List<Point> Points { get; set; }

        public override Extent GetExtent()
        {
            return Points.GetExtent();
        }

        public override IEnumerable<Point> GetBoundingPoints()
        {
            return Points;
        }

        public override GeometryType Type => GeometryType.Ring;

        protected override object CloneImpl() => Clone();

        public Ring Clone() => new Ring
        {
            Points = Points?.Select(p=>p.Clone()).ToList()
        };
    }
}
