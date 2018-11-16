using System;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.GeoJson
{
    public class GeoJsonPoint : GeoJsonGeometry, IEquatable<GeoJsonPoint>
    {
        private double[] _coordinates = new double[2];

        public GeoJsonPoint()
        {
        }

        public GeoJsonPoint(double[] coordinates)
        {
            Coordinates = coordinates;
        }

        public override GeoJsonGeometryType Type => GeoJsonGeometryType.Point;

        public double[] Coordinates
        {
            get => _coordinates;
            set
            {
                if (value == null || value.Length < 2 || value.Length > 4)
                {
                    throw new FormatException();
                }

                _coordinates = value;
            }
        }

        public double X
        {
            get => Coordinates[0];
            set => Coordinates[0] = value;
        }

        public double Y
        {
            get => Coordinates[1];
            set => Coordinates[1] = value;
        }

        public double? Z
        {
            get => Coordinates.Length > 2 ? Coordinates[2] : (double?) null;
            set
            {
                if (value.HasValue)
                {
                    Coordinates[2] = value.Value;
                }
                else
                {
                    Array.Resize(ref _coordinates, 2);
                }
            }
        }

        public double? M
        {
            get => Coordinates.Length > 3 ? Coordinates[3] : (double?) null;
            set
            {
                if (value.HasValue)
                {
                    Coordinates[3] = value.Value;
                }
                else
                {
                    Array.Resize(ref _coordinates, 3);
                }
            }
        }

        public override IGeometry ToSpatial()
        {
            if (Coordinates == null ||
                Coordinates.Length < 2 ||
                Coordinates.Length > 4)
            {
                return null;
            }

            return new Point(Coordinates);
        }

        public override void Accept(IGeoJsonVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string ToDataString()
        {
            return Coordinates.Join();
        }

        public static GeoJsonPoint Parse(string value, PointStyles style = PointStyles.Default)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException();
            }

            value = value.Trim();
            if (value.StartsWith("{"))
            {
                return JsonConvert.DeserializeObject<GeoJsonPoint>(value);
            }

            var coordinates = Point.ParseCoordinates(value, style);
            return new GeoJsonPoint(coordinates);
        }

        public bool Equals(GeoJsonPoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.Equals(Type, other.Type) ||
                _coordinates.Length != other._coordinates.Length)
            {
                return false;
            }

            for (int i = 0; i < _coordinates.Length; i++)
            {
                if (!_coordinates[i].Equals(other._coordinates[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GeoJsonPoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_coordinates != null ? _coordinates.GetHashCode() : 0) * 397) ^
                       (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }
}
