using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Anywhere.ArcGIS.GeoJson;
using Anywhere.ArcGIS.Serialization;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Common
{
    [JsonConverter(typeof(ExtentConverter))]
    public class Extent : SpatialGeometry, IEquatable<Extent>
    {
        public static readonly Extent Empty = new Extent(0, 0, 0, 0, null);

        public Extent()
        {
        }

        public Extent(SpatialReference spatialReference)
            : base(spatialReference)
        {
        }

        public Extent(double xmin,
            double ymin,
            double xmax,
            double ymax, SpatialReference spatialReference)
            : base(spatialReference)
        {
            XMin = xmin;
            XMax = xmax;
            YMin = ymin;
            YMax = ymax;
        }

        public Extent(double xmin,
            double ymin,
            double zmin,
            double xmax,
            double ymax,
            double zmax, SpatialReference spatialReference)
            : this(xmin, ymin, xmax, ymax, spatialReference)
        {
            ZMin = zmin;
            ZMax = zmax;
        }

        public Extent(double xmin,
            double ymin,
            double zmin,
            double mmin,
            double xmax,
            double ymax,
            double zmax,
            double mmax, SpatialReference spatialReference)
            : this(xmin, ymin, zmin, xmax, ymax, zmax, spatialReference)
        {
            MMin = mmin;
            MMax = mmax;
        }

        public Extent(double[] coordinates, SpatialReference spatialReference = null)
            : base(spatialReference)
        {
            switch (coordinates.Length)
            {
                case 8:
                    XMin = coordinates[0];
                    YMin = coordinates[1];
                    ZMin = coordinates[2];
                    MMin = coordinates[3];
                    XMax = coordinates[4];
                    YMax = coordinates[5];
                    ZMax = coordinates[6];
                    MMax = coordinates[7];
                    break;
                case 6:
                    XMin = coordinates[0];
                    YMin = coordinates[1];
                    ZMin = coordinates[2];
                    XMax = coordinates[3];
                    YMax = coordinates[4];
                    ZMax = coordinates[5];
                    break;
                case 4:
                    XMin = coordinates[0];
                    YMin = coordinates[1];
                    XMax = coordinates[2];
                    YMax = coordinates[3];
                    break;
                default:
                    throw new FormatException();
            }
        }

        public override IEnumerable<Point> GetBoundingPoints()
        {
            if (ZMin.HasValue)
            {
                yield return new Point(XMin, YMin, ZMin.Value, MMin.Value, SpatialReference);
                yield return new Point(XMax, YMax, ZMax.Value, MMax.Value, SpatialReference);
            }
            else if (MMin.HasValue)
            {
                yield return new Point(XMin, YMin, ZMin.Value, SpatialReference);
                yield return new Point(XMax, YMax, ZMax.Value, SpatialReference);
            }
            else
            {
                yield return new Point(XMin, YMin, SpatialReference);
                yield return new Point(XMax, YMax, SpatialReference);
            }
        }

        protected override void AcceptImpl(ISpatialVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override GeometryType Type => GeometryType.Envelope;

        public double XMin { get; set; }
        public double YMin { get; set; }
        public double? ZMin { get; set; }
        public double? MMin { get; set; }
        public double XMax { get; set; }
        public double YMax { get; set; }
        public double? ZMax { get; set; }
        public double? MMax { get; set; }

        public Point Min
        {
            get
            {
                if (ZMin.HasValue && MMin.HasValue)
                    return new Point(XMin, YMin, ZMin.Value, MMin.Value, SpatialReference);
                if (ZMin.HasValue)
                    return new Point(XMin, YMin, ZMin.Value, SpatialReference);
                else
                    return new Point(XMin, YMin, SpatialReference);
            }
        }

        public Point Max
        {
            get
            {
                if (ZMax.HasValue && MMax.HasValue)
                    return new Point(XMax, YMax, ZMax.Value, MMax.Value, SpatialReference);
                if (ZMax.HasValue)
                    return new Point(XMax, YMax, ZMax.Value, SpatialReference);
                else
                    return new Point(XMax, YMax, SpatialReference);
            }
        }

        /// <summary>
        /// Gets the height of the Envelope (along Y-axis).
        /// </summary>
        public double Height => YMax - YMin;

        /// <summary>
        /// Gets the width of this Envelope (along X-axis).
        /// </summary>
        [DataMember]
        public double Width => XMax - XMin;

        /// <summary>
        /// Gets the depth of this Envelope (along Z-axis).
        /// </summary>
        public double? Depth => ZMax - ZMin;

        /// <summary>
        /// Gets the distance of this Envelope (along M-axis).
        /// </summary>
        public double? Distance => MMax - MMin;

        public bool IsEmpty => Math.Abs(XMax - XMin) <= double.Epsilon || Math.Abs(YMax - YMin) <= double.Epsilon;

        public override Extent GetExtent()
        {
            return this;
        }

        public override Point GetCenter()
        {
            return new Point(SpatialReference)
            {
                X = (XMin + XMax) / 2d,
                Y = (YMin + YMax) / 2d
            };
        }

        public override Point GetPole()
        {
            return GetCenter();
        }

        public double[] ToCoordinates()
        {
            if (ZMin.HasValue && MMin.HasValue && ZMax.HasValue && MMax.HasValue)
            {
                return new[]
                {
                    XMin,
                    YMin,
                    ZMin.Value,
                    MMin.Value,
                    XMax,
                    YMax,
                    ZMax.Value,
                    MMax.Value,
                };
            }
            else if (ZMin.HasValue && ZMax.HasValue)
            {
                return new[]
                {
                    XMin,
                    YMin,
                    ZMin.Value,
                    XMax,
                    YMax,
                    ZMax.Value,
                };
            }
            else
            {
                return new[]
                {
                    XMin,
                    YMin,
                    XMax,
                    YMax,
                };
            }
        }

        public Extent Union(Extent other)
        {
            if (other == null) other = this;
            if (SpatialReference != null && !SpatialReference.Equals(other.SpatialReference))
                throw new ArgumentException("Spatial references must match for union operation.");

            var result = new Extent(SpatialReference ?? other.SpatialReference);

            if (double.IsNaN(XMin))
                result.XMin = other.XMin;
            else if (!double.IsNaN(other.XMin))
                result.XMin = Math.Min(other.XMin, XMin);
            else
                result.XMin = XMin;

            if (double.IsNaN(XMax))
                result.XMax = other.XMax;
            else if (!double.IsNaN(other.XMax))
                result.XMax = Math.Max(other.XMax, XMax);
            else
                result.XMax = XMax;

            if (double.IsNaN(YMin))
                result.YMin = other.YMin;
            else if (!double.IsNaN(other.YMin))
                result.YMin = Math.Min(other.YMin, YMin);
            else
                result.YMin = YMin;

            if (double.IsNaN(YMax))
                result.YMax = other.YMax;
            else if (!double.IsNaN(other.YMax))
                result.YMax = Math.Max(other.YMax, YMax);
            else
                result.YMax = YMax;

            return result;
        }

        public bool Equals(Extent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(SpatialReference, other?.SpatialReference) &&
                   XMin.Equals(other.XMin) &&
                   XMax.Equals(other.XMax) &&
                   YMin.Equals(other.YMin) &&
                   YMax.Equals(other.YMax);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (SpatialReference != null ? SpatialReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ XMin.GetHashCode();
                hashCode = (hashCode * 397) ^ XMax.GetHashCode();
                hashCode = (hashCode * 397) ^ YMin.GetHashCode();
                hashCode = (hashCode * 397) ^ YMax.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Extent)obj);
        }

        public override IGeoJsonGeometry ToGeoJson()
        {
            return new GeoJsonPolygon
            {
                Coordinates = new List<List<double[]>>
                {
                    new List<double[]>
                    {
                        new []{XMin, YMin},
                        new []{XMax, YMin},
                        new []{XMax, YMax},
                        new []{XMin, YMax},
                        new []{XMin, YMin}
                    }
                }
            };
        }

        protected override object CloneImpl() => Clone();

        public Extent Clone()
        {
            return new Extent(SpatialReference?.Clone())
            {
                XMax = XMax,
                XMin = XMin,
                YMax = YMax,
                YMin = YMin
            };
        }

        public override string ToString()
        {
            return $"{GetType().Name}; XMin = {XMin}; YMin = {YMin}; XMax = {XMax}; YMax = {YMax};";
        }

        public static Extent Parse(string value, object spatialReference = null)
        {
            return Parse(value, PointStyles.Default, spatialReference);
        }

        public static Extent Parse(string value, PointStyles style, object spatialReference = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException();
            }

            value = value.Trim();
            if (value.StartsWith("{") || value.StartsWith("["))
            {
                return JsonConvert.DeserializeObject<Extent>(value);
            }

            int dimensions = 4;
            if ((style & PointStyles.AllowZ) != 0 && (style & PointStyles.AllowM) != 0)
            {
                dimensions = 8;
            }
            else if ((style & PointStyles.AllowZ) != 0)
            {
                dimensions = 6;
            }
            else if ((style & PointStyles.AllowM) != 0)
            {
                // TODO [PointParseStyle.AllowM]
            }

            var parts = value.Split(',');
            if (dimensions < parts.Length || parts.Length < 4 || parts.Length % 2 != 0)
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

            return new Extent(coordinates, SpatialReference.FromObject(spatialReference));
        }

        public string ToDataString()
        {
            return ToCoordinates().Join();
        }
    }
}
