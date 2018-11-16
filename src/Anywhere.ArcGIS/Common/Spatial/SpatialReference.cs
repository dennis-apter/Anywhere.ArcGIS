using System;
using System.Runtime.Serialization;
using Anywhere.ArcGIS.Serialization;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Common
{
    /// <summary>
    /// Spatial reference used for operations. If WKT is set then other properties are nulled
    /// </summary>
    [JsonConverter(typeof(SpatialReferenceConverter))]
    public class SpatialReference : IEquatable<SpatialReference>, ICloneable, ISerializable
    {
        public SpatialReference() { }

        public SpatialReference(int wkid, int? latestWkid = null)
        {
            Wkid = wkid;
            LatestWkid = latestWkid;
        }

        public SpatialReference(string wkt)
        {
            Wkt = wkt;
        }

        private SpatialReference(SerializationInfo info, StreamingContext ctx)
        {
            foreach (var entry in info)
            {
                switch (entry.Name)
                {
                    case "wkt":
                        Wkt = info.GetString(entry.Name);
                        return;
                    case "wkid":
                        Wkid = info.GetInt32(entry.Name);
                        break;
                    case "latestWkid":
                        LatestWkid = info.GetInt32(entry.Name);
                        break;
                    case "vcsWkid":
                        VcsWkid = info.GetInt32(entry.Name);
                        break;
                    case "latestVcsWkid":
                        LatestVcsWkid = info.GetInt32(entry.Name);
                        break;
                }
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (string.IsNullOrEmpty(_wkt))
            {
                if (Wkid.HasValue) info.AddValue("wkid", Wkid.Value);
                if (LatestWkid.HasValue) info.AddValue("latestWkid", LatestWkid.Value);
                if (VcsWkid.HasValue) info.AddValue("vcsWkid", VcsWkid.Value);
                if (LatestVcsWkid.HasValue) info.AddValue("latestVcsWkid", LatestVcsWkid.Value);
            }
            else
            {
                info.AddValue("wkt", Wkt);
            }
        }

        /// <summary>
        /// World Geodetic System 1984 (WGS84)
        /// </summary>
        public static readonly SpatialReference WGS84 = new SpatialReference(4326, 4326);

        /// <summary>
        /// WGS 1984 Web Mercator (Auxiliary Sphere)
        /// </summary>
        public static readonly SpatialReference WebMercator = new SpatialReference(102100, 3857);

        public int? Wkid { get; set; }

        public int? LatestWkid { get; set; }

        public int? VcsWkid { get; set; }

        public int? LatestVcsWkid { get; set; }

        string _wkt;

        [DataMember(Name = "wkt")]
        public string Wkt
        {
            get => _wkt;
            set
            {
                _wkt = value;
                Wkid = null;
                LatestWkid = null;
                VcsWkid = null;
                LatestVcsWkid = null;
            }
        }

        public static bool operator ==(SpatialReference left, SpatialReference right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return true;
            if (ReferenceEquals(null, left)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(SpatialReference left, SpatialReference right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right)) return false;
            if (ReferenceEquals(null, left)) return true;
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SpatialReference)obj);
        }

        public bool Equals(SpatialReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (string.IsNullOrWhiteSpace(Wkt))
                ? (Wkid == other.Wkid || LatestWkid == other.LatestWkid) && (VcsWkid == other.VcsWkid || LatestVcsWkid == other.LatestVcsWkid)
                : string.Equals(Wkt, other.Wkt, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Wkid != null ? Wkid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LatestWkid != null ? LatestWkid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (VcsWkid != null ? VcsWkid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LatestVcsWkid != null ? LatestVcsWkid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Wkt != null ? Wkt.GetHashCode() : 0);
                return hashCode;
            }
        }

        public SpatialReference Clone()
        {
            return new SpatialReference
            {
                LatestVcsWkid = LatestVcsWkid,
                LatestWkid = LatestWkid,
                VcsWkid = VcsWkid,
                Wkid = Wkid,
                Wkt = Wkt
            };
        }

        object ICloneable.Clone() => Clone();

        public override string ToString()
        {
            return Wkid.HasValue
                ? $"{GetType().Name}; Wkid = {Wkid};"
                : $"{GetType().Name}; Wkt = {Wkt};";
        }

        public static SpatialReference FromObject(object value)
        {
            if (null != value)
            {
                if (value is SpatialReference sr)
                {
                    return sr;
                }
                else if (value is string wkt)
                {
                    wkt = wkt.Trim();
                    if (wkt != string.Empty)
                    {
                        return new SpatialReference(wkt);
                    }
                }
                else if (value is int wkid)
                {
                    if (wkid != 0)
                    {
                        return new SpatialReference(wkid);
                    }
                }
            }

            return null;
        }

        public static SpatialReference Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException();
            }

            value = value.Trim();
            if (value.StartsWith("{"))
            {
                return JsonConvert.DeserializeObject<SpatialReference>(value);
            }

            if (int.TryParse(value, out var wkid))
            {
                return new SpatialReference(wkid);
            }

            return new SpatialReference(value);
        }
    }
}
