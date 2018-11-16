using System;
using System.Collections.Generic;
using System.Globalization;
using Anywhere.ArcGIS.GeoJson;
using Anywhere.ArcGIS.Serialization;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Common
{
    [JsonConverter(typeof(PointConverter))]
    public class Point : SpatialGeometry, IEquatable<Point>
    {
        public Point() { }

        public Point(SpatialReference spatialReference)
            : base(spatialReference)
        {
        }

        public Point(double x, double y, SpatialReference spatialReference = null)
            : base(spatialReference)
        {
            X = x;
            Y = y;
        }

        public Point(double x, double y, double z, SpatialReference spatialReference = null)
            : this(x, y, spatialReference)
        {
            Z = z;
        }

        public Point(double x, double y, double z, double m, SpatialReference spatialReference = null)
            : this(x, y, z, spatialReference)
        {
            M = m;
        }

        public Point(double[] coordinates, SpatialReference spatialReference = null)
            : this(coordinates[0], coordinates[1], spatialReference)
        {
            if (coordinates.Length > 3)
            {
                Z = coordinates[2];
                M = coordinates[3];
            }
            else if (coordinates.Length > 2)
            {
                Z = coordinates[2];
            }
        }

        public override GeometryType Type => GeometryType.Point;

        public double X { get; set; }
        public double Y { get; set; }
        public double? Z { get; set; }
        public double? M { get; set; }

        public override Point GetCenter()
        {
            return Clone();
        }

        public override Point GetPole()
        {
            return GetCenter();
        }

        public override Extent GetExtent()
        {
            if (M.HasValue && Z.HasValue)
            {
                return new Extent(X, Y, Z.Value, M.Value, X, Y, Z.Value, M.Value, SpatialReference);
            }
            else if (Z.HasValue)
            {
                return new Extent(X, Y, Z.Value, X, Y, Z.Value, SpatialReference);
            }
            else
            {
                return new Extent(X, Y, X, Y, SpatialReference);
            }
        }

        public override IEnumerable<Point> GetBoundingPoints()
        {
            yield return this;
        }

        protected override void AcceptImpl(ISpatialVisitor visitor)
        {
            visitor.Visit(this);
        }

        public bool Equals(Point other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && M.Equals(other.M) && Equals(SpatialReference, other.SpatialReference);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ M.GetHashCode();
                hashCode = (hashCode * 397) ^ (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Point)obj);
        }

        public override IGeoJsonGeometry ToGeoJson()
        {
            return new GeoJsonPoint
            {
                Coordinates = ToCoordinates()
            };
        }

        protected override object CloneImpl() => Clone();

        public Point Clone()
        {
            return new Point(SpatialReference?.Clone())
            {
                X = X,
                Y = Y,
                M = M,
                Z = Z
            };
        }

        public double[] ToCoordinates()
        {
            if (Z.HasValue && M.HasValue)
            {
                return new[] { X, Y, Z.Value, M.Value };
            }
            else if (Z.HasValue)
            {
                return new[] { X, Y, Z.Value };
            }

            return new[] { X, Y };
        }

        public override string ToString()
        {
            return $"{GetType().Name}; X = {X}; Y = {Y};";
        }

        public static Point Parse(string value, object spatialReference = null)
        {
            return Parse(value, PointStyles.Default, spatialReference);
        }

        public static Point Parse(string value, PointStyles style, object spatialReference = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException();
            }

            value = value.Trim();
            if (value.StartsWith("{") || value.StartsWith("["))
            {
                return JsonConvert.DeserializeObject<Point>(value);
            }

            var coordinates = ParseCoordinates(value, style);
            return new Point(coordinates, SpatialReference.FromObject(spatialReference));
        }

        internal static double[] ParseCoordinates(string value, PointStyles style)
        {
            int dimensions = 2;
            if ((style & PointStyles.AllowZ) != 0 && (style & PointStyles.AllowM) != 0)
            {
                dimensions = 4;
            }
            else if ((style & PointStyles.AllowZ) != 0)
            {
                dimensions = 3;
            }
            else if ((style & PointStyles.AllowM) != 0)
            {
                // TODO [PointParseStyle.AllowM]
            }

            var parts = value.Split(',');
            if (dimensions < parts.Length || parts.Length < 2)
            {
                throw new FormatException();
            }

            var coordinates = new double[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                if (double.TryParse(
                    parts[i],
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var c))
                {
                    coordinates[i] = c;
                }
                else
                {
                    throw new FormatException();
                }
            }

            return coordinates;
        }

        public string ToDataString()
        {
            return ToCoordinates().Join();
        }

        public static Point operator /(Point point, double divider)
        {
            if (point.Z.HasValue && point.M.HasValue)
            {
                return new Point(point.X / divider, point.Y / divider, point.Z.Value / divider, point.M.Value / divider, point.SpatialReference);
            }
            else if (point.Z.HasValue)
            {
                return new Point(point.X / divider, point.Y / divider, point.Z.Value / divider, point.SpatialReference);
            }

            return new Point(point.X / divider, point.Y / divider, point.SpatialReference);
        }

        public static Point operator *(Point point, double multiplier)
        {
            if (point.Z.HasValue && point.M.HasValue)
            {
                return new Point(point.X * multiplier, point.Y * multiplier, point.Z.Value * multiplier, point.M.Value * multiplier, point.SpatialReference);
            }
            else if (point.Z.HasValue)
            {
                return new Point(point.X * multiplier, point.Y * multiplier, point.Z.Value * multiplier, point.SpatialReference);
            }

            return new Point(point.X * multiplier, point.Y * multiplier, point.SpatialReference);
        }

        public static Point operator +(Point l, Point r)
        {
            if (l.Z.HasValue && l.M.HasValue)
            {
                return new Point(l.X + r.X, l.Y + r.Y, l.Z.Value + r.Z ?? 0d, l.M.Value + r.M ?? 0d, l.SpatialReference);
            }
            else if (l.Z.HasValue)
            {
                return new Point(l.X + r.X, l.Y + r.Y, l.Z.Value + r.Z ?? 0d, l.SpatialReference);
            }

            return new Point(l.X + r.X, l.Y + r.Y, l.SpatialReference);
        }

        public static Point operator -(Point l, Point r)
        {
            if (l.Z.HasValue && l.M.HasValue)
            {
                return new Point(l.X - r.X, l.Y - r.Y, l.Z.Value - r.Z ?? 0d, l.M.Value - r.M ?? 0d, l.SpatialReference);
            }
            else if (l.Z.HasValue)
            {
                return new Point(l.X - r.X, l.Y - r.Y, l.Z.Value - r.Z ?? 0d, l.SpatialReference);
            }

            return new Point(l.X - r.X, l.Y - r.Y, l.SpatialReference);
        }

        public static Point operator +(Point l, Size r)
        {
            return new Point(l.X + r.Width, l.Y + r.Height, l.SpatialReference);
        }

        public static Point operator -(Point l, Size r)
        {
            return new Point(l.X - r.Width, l.Y - r.Height, l.SpatialReference);
        }
    }

    [Flags]
    public enum PointStyles
    {
        Default = 0,
        AllowZ = 2,
        AllowM = 4,
        Any = AllowZ | AllowM
    }
}
