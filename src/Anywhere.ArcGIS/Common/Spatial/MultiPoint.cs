using System;
using System.Collections.Generic;
using System.Linq;
using Anywhere.ArcGIS.GeoJson;

namespace Anywhere.ArcGIS.Common
{
    public class MultiPoint : SpatialGeometry, IEquatable<MultiPoint>
    {
        public MultiPoint()
        {
        }

        public MultiPoint(SpatialReference spatialReference)
            : base(spatialReference)
        {
        }

        public MultiPoint(SpatialReference spatialReference, IEnumerable<Point> points)
            : base(spatialReference)
        {
            if (points != null)
            {
                Points = points.ToList();
            }
        }

        public override Extent GetExtent()
        {
            return Points.GetExtent();
        }

        public override IEnumerable<Point> GetBoundingPoints()
        {
            return Points;
        }

        protected override void AcceptImpl(ISpatialVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override GeometryType Type => GeometryType.Multipoint;

        public bool HasM { get; set; }
        public bool HasZ { get; set; }
        public List<Point> Points { get; set; }

        public override Point GetCenter()
        {
            return GetExtent().GetCenter();
        }

        public override Point GetPole()
        {
            throw new NotImplementedException();
        }

        public bool Equals(MultiPoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SpatialReference, other.SpatialReference) && HasM.Equals(other.HasM) && HasZ.Equals(other.HasZ) && Equals(Points, other.Points);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HasM.GetHashCode();
                hashCode = (hashCode * 397) ^ HasZ.GetHashCode();
                hashCode = (hashCode * 397) ^ (Points != null ? Points.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MultiPoint)obj);
        }

        public override IGeoJsonGeometry ToGeoJson()
        {
            return new GeoJsonLineString(Points.ToCoordinates());
        }

        public MultiPoint Clone()
        {
            return new MultiPoint(SpatialReference?.Clone())
            {
                HasM = HasM,
                HasZ = HasZ,
                Points = Points.Select(p => p.Clone()).ToList()
            };
        }

        protected override object CloneImpl() => Clone();
    }
}
