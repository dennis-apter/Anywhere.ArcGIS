using System.Runtime.Serialization;
using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.Serialization;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.GeoJson
{
    [JsonConverter(typeof(GeoJsonConverter))]
    public interface IGeoJsonGeometry
    {
        IGeometry ToSpatial();
    }

    [DataContract]
    public enum GeoJsonGeometryType
    {
        Unknown,
        [EnumMember(Value = "Point")] Point,
        [EnumMember(Value = "Polygon")] Polygon,
        [EnumMember(Value = "LineString")] LineString,
        [EnumMember(Value = "MultiPoint")] MultiPoint,
        [EnumMember(Value = "MultiLineString")] MultiLineString,
        [EnumMember(Value = "MultiPolygon")] MultiPolygon,
        [EnumMember(Value = "GeometryCollection")] GeometryCollection,
    }
}
