using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Anywhere.ArcGIS.GeoJson;

namespace Anywhere.ArcGIS.Common
{
    [DataContract]
    public class Polyline : SpatialGeometry, IEquatable<Polyline>
    {
        public Polyline()
        {
        }

        public Polyline(SpatialReference spatialReference)
            : base(spatialReference)
        {
        }

        public Polyline(SpatialReference spatialReference, IEnumerable<Path> paths)
            : base(spatialReference)
        {
            if (paths != null)
            {
                Paths = paths.ToList();
            }
        }

        public override GeometryType Type => GeometryType.Polyline;

        public bool HasM { get; set; }
        public bool HasZ { get; set; }
        public List<Path> Paths { get; set; }

        public override Point GetCenter()
        {
            return GetExtent().GetCenter();
        }

        public override Point GetPole()
        {
            throw new NotImplementedException();
        }

        public override Extent GetExtent()
        {
            return GetBoundingPoints().GetExtent();
        }

        public override IEnumerable<Point> GetBoundingPoints()
        {
            foreach (var path in Paths)
            {
                foreach (var point in path.GetBoundingPoints())
                {
                    yield return point;
                }
            }
        }

        protected override void AcceptImpl(ISpatialVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override object CloneImpl() => Clone();

        public Polyline Clone()
        {
            return new Polyline(SpatialReference?.Clone())
            {
                HasM = HasM,
                HasZ = HasZ,
                Paths = Paths?.Select(p => p.Clone()).ToList()
            };
        }

        public bool Equals(Polyline other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SpatialReference, other.SpatialReference) && HasM.Equals(other.HasM) && HasZ.Equals(other.HasZ) && Equals(Paths, other.Paths);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HasM.GetHashCode();
                hashCode = (hashCode * 397) ^ HasZ.GetHashCode();
                hashCode = (hashCode * 397) ^ (Paths != null ? Paths.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Polyline)obj);
        }

        public override IGeoJsonGeometry ToGeoJson()
        {
            if (Paths == null || !Paths.Any())
            {
                return null;
            }

            var coordinates = new List<double[]>();
            foreach (var path in Paths)
            {
                if (path.Points != null)
                {
                    foreach (var point in path.Points)
                    {
                        coordinates.Add(point.ToCoordinates());
                    }
                }
            }

            if (coordinates.Count == 0)
            {
                return null;
            }

            return new GeoJsonLineString(coordinates);
        }

        public override string ToString()
        {
            return $"{GetType().Name}; Paths[{Paths.Count}];";
        }
    }
}