using Anywhere.ArcGIS.GeoJson;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Serialization
{
    public class GeoJsonEmitter : IGeoJsonVisitor
    {
        private readonly JsonWriter _writer;

        public GeoJsonEmitter(JsonWriter writer)
        {
            _writer = writer;
        }

        private void WriteStartObject(GeoJsonGeometryType type)
        {
            _writer.WriteStartObject();

            _writer.WritePropertyName("type");
            _writer.WriteValue(type.ToString());
        }

        public void Visit(GeoJsonPoint point)
        {
            WriteStartObject(GeoJsonGeometryType.Point);
            _writer.WritePropertyName("coordinates");
            _writer.WriteValue(point.Coordinates);
            _writer.WriteEndObject();
        }

        public void Visit(GeoJsonLineString lineString)
        {
            WriteStartObject(GeoJsonGeometryType.LineString);
            _writer.WritePropertyName("coordinates");
            _writer.WriteValue(lineString.Coordinates);
            _writer.WriteEndObject();
        }

        public void Visit(GeoJsonPolygon polygon)
        {
            WriteStartObject(GeoJsonGeometryType.Polygon);

            _writer.WritePropertyName("coordinates");
            _writer.WriteValue(polygon.Coordinates);
            _writer.WriteEndObject();
        }

        public void Visit(GeoJsonMultiPoint multiPoint)
        {
            WriteStartObject(GeoJsonGeometryType.MultiPoint);
            _writer.WritePropertyName("coordinates");
            _writer.WriteValue(multiPoint.Coordinates);
            _writer.WriteEndObject();
        }

        public void Visit(GeoJsonMultiLineString multiLineString)
        {
            WriteStartObject(GeoJsonGeometryType.MultiLineString);
            _writer.WritePropertyName("coordinates");
            _writer.WriteValue(multiLineString.Coordinates);
            _writer.WriteEndObject();
        }

        public void Visit(GeoJsonMultiPolygon multiPolygon)
        {
            WriteStartObject(GeoJsonGeometryType.MultiPolygon);
            _writer.WritePropertyName("coordinates");
            _writer.WriteValue(multiPolygon.Coordinates);
            _writer.WriteEndObject();
        }

        public void Visit(GeoJsonGeometryCollection geometryCollection)
        {
            WriteStartObject(GeoJsonGeometryType.GeometryCollection);
            _writer.WritePropertyName("geometries");
            _writer.WriteValue(geometryCollection.Geometries);
            _writer.WriteEndObject();
        }
    }
}
