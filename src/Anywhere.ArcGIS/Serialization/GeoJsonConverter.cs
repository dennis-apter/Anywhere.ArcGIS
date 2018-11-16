using System;
using System.Collections.Generic;
using Anywhere.ArcGIS.GeoJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serialization
{
    public sealed class GeoJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(GeoJsonGeometry).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var geometry = value as GeoJsonGeometry;
            if (geometry == null)
            {
                writer.WriteNull();
                return;
            }

            geometry.Accept(new GeoJsonEmitter(writer));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            const string coordinates = "coordinates";

            var o = JObject.Load(reader);
            var type = o.GetValue("type").ToObject<GeoJsonGeometryType>();
            switch (type)
            {
                case GeoJsonGeometryType.Point: return new GeoJsonPoint(ReadArray1(o.GetValue(coordinates)));
                case GeoJsonGeometryType.LineString: return new GeoJsonLineString(ReadArray2(o.GetValue(coordinates)));
                case GeoJsonGeometryType.Polygon: return new GeoJsonPolygon(ReadArray3(o.GetValue(coordinates)));
                case GeoJsonGeometryType.MultiPoint: return new GeoJsonMultiPoint(ReadArray2(o.GetValue(coordinates)));
                case GeoJsonGeometryType.MultiLineString: return new GeoJsonMultiLineString(ReadArray3(o.GetValue(coordinates)));
                case GeoJsonGeometryType.MultiPolygon: return new GeoJsonMultiPolygon(ReadArray4(o.GetValue(coordinates)));
                case GeoJsonGeometryType.GeometryCollection: return new GeoJsonGeometryCollection(o.Value<IEnumerable<IGeoJsonGeometry>>("geometries"));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<List<List<double[]>>> ReadArray4(JToken token)
        {
            if (token.Type == JTokenType.Array)
            {
                var result = new List<List<List<double[]>>>();
                var array = (JArray)token;
                for (int i = 0; i < array.Count; i++)
                {
                    var points = ReadArray3(array[i]);
                    if (points != null)
                    {
                        result.Add(points);
                    }
                }

                return result;
            }

            return null;
        }

        private List<List<double[]>> ReadArray3(JToken token)
        {
            if (token.Type == JTokenType.Array)
            {
                var result = new List<List<double[]>>();
                var array = (JArray)token;
                for (int i = 0; i < array.Count; i++)
                {
                    var points = ReadArray2(array[i]);
                    if (points != null)
                    {
                        result.Add(points);
                    }
                }

                return result;
            }

            return null;
        }

        private List<double[]> ReadArray2(JToken token)
        {
            if (token.Type == JTokenType.Array)
            {
                var result = new List<double[]>();
                var array = (JArray)token;
                for (int i = 0; i < array.Count; i++)
                {
                    var points = ReadArray1(array[i]);
                    if (points != null)
                    {
                        result.Add(points);
                    }
                }

                return result;
            }

            return null;
        }

        private double[] ReadArray1(JToken token)
        {
            if (token.Type == JTokenType.Array)
            {
                var array = (JArray)token;
                var result = new double[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    result[i] = array[i].Value<double>();
                }

                return result;
            }

            return null;
        }
    }
}
