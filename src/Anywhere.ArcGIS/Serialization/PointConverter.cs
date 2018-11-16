using System;
using System.Linq;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serialization
{
    public sealed class PointConverter : SpatialConverterBase
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Point).IsAssignableFrom(objectType);
        }

        private static bool TryReadCompact(JsonReader reader, out IGeometry spatial)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                spatial = null;
                return true;
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                if (reader.Read() && reader.TryGetDouble(out var x))
                {
                    if (reader.Read() && reader.TryGetDouble(out var y))
                    {
                        if (reader.Read() && reader.TryGetDouble(out var z))
                        {
                            if (reader.Read() && reader.TryGetDouble(out var m))
                            {
                                if (reader.Read() && reader.TokenType == JsonToken.EndArray)
                                {
                                    spatial = new Point(x, y, z, m);
                                    return true;
                                }
                            }
                            else if (reader.TokenType == JsonToken.EndArray)
                            {
                                spatial = new Point(x, y, z);
                                return true;
                            }
                        }
                        else if (reader.TokenType == JsonToken.EndArray)
                        {
                            spatial = new Point(x, y);
                            return true;
                        }
                    }
                }
            }

            spatial = null;
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (TryReadCompact(reader, out var spatial))
            {
                return spatial;
            }

            var o = JObject.Load(reader);
            var spatialReference = ReadSpatialReference(o);

            if (TryReadPoint(o, spatialReference, out var point))
            {
                return point;
            }

            return null;
        }
    }
}
