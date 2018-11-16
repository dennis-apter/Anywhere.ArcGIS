using System;
using System.Collections.Generic;
using System.Linq;
using Anywhere.ArcGIS.Common.Algorithm;
using Anywhere.ArcGIS.GeoJson;

namespace Anywhere.ArcGIS.Common
{
    public class Polygon : SpatialGeometry, IEquatable<Polygon>
    {
        public Polygon()
        {
        }

        public Polygon(SpatialReference spatialReference) 
            : base(spatialReference)
        {
        }

        public Polygon(SpatialReference spatialReference, IEnumerable<Ring> rings)
            : base(spatialReference)
        {
            if (rings != null)
            {
                Rings = rings.ToList();
            }
        }

        public override IEnumerable<Point> GetBoundingPoints()
        {
            foreach (var ring in Rings)
            {
                foreach (var point in ring.GetBoundingPoints())
                {
                    yield return point;
                }
            }
        }

        protected override void AcceptImpl(ISpatialVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override GeometryType Type => GeometryType.Polygon;

        public bool HasM { get; set; }
        public bool HasZ { get; set; }
        public List<Ring> Rings { get; set; }

        public override Extent GetExtent()
        {
            return GetBoundingPoints().GetExtent();
        }

        public override Point GetCenter()
        {
            return GetExtent().GetCenter();
        }

        /// <summary>
        /// Returns pole of inaccessibility, the most distant internal point from the polygon outline (not to be confused with centroid).
        /// </summary>
        /// <returns></returns>
        public override Point GetPole()
        {
            return Polylabel.Find(this);
        }

        public bool Equals(Polygon other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SpatialReference, other.SpatialReference) && HasM.Equals(other.HasM) && HasZ.Equals(other.HasZ) && Equals(Rings, other.Rings);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HasM.GetHashCode();
                hashCode = (hashCode * 397) ^ HasZ.GetHashCode();
                hashCode = (hashCode * 397) ^ (Rings != null ? Rings.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Polygon)obj);
        }

        public override IGeoJsonGeometry ToGeoJson()
        {
            return new GeoJsonPolygon(Rings?.Select(r => r.Points?.ToCoordinates()));
        }

        protected override object CloneImpl() => Clone();

        public Polygon Clone()
        {
            return new Polygon(SpatialReference?.Clone())
            {
                HasM = HasM,
                HasZ = HasZ,
                Rings = Rings?.Select(r => r.Clone()).ToList()
            };
        }

        public override string ToString()
        {
            return $"{GetType().Name}; Rings[{Rings.Count}];";
        }
    }
}
