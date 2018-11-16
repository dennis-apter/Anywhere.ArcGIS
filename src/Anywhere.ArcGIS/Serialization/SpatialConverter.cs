using System;
using System.Linq;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serialization
{
    public sealed class SpatialConverter : SpatialConverterBase
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IGeometry).IsAssignableFrom(objectType);
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
                if (reader.Read())
                {
                    double x, x2, y, y2;
                    double? z = null, z2 = null, m = null, m2 = null;
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        if (reader.Read() && reader.TryGetDouble(out x))
                        {
                            if (reader.Read() && reader.TryGetDouble(out y))
                            {
                                int i = 0;
                                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                                {
                                    double d;
                                    if (reader.TryGetDouble(out d))
                                    {
                                        if (i == 0) z = d;
                                        else if (i == 1) m = d;
                                    }
                                    i++;
                                }

                                if (reader.Read() && reader.TokenType == JsonToken.StartArray)
                                {
                                    if (reader.Read() && reader.TryGetDouble(out x2))
                                    {
                                        if (reader.Read() && reader.TryGetDouble(out y2))
                                        {
                                            i = 0;
                                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                                            {
                                                double d;
                                                if (reader.TryGetDouble(out d))
                                                {
                                                    if (i == 0) z2 = d;
                                                    else if (i == 1) m2 = d;
                                                }
                                                i++;
                                            }

                                            if (reader.Read() && reader.TokenType == JsonToken.EndArray)
                                            {
                                                spatial = new Extent(x, y, x2, y2, null)
                                                {
                                                    ZMin = z,
                                                    ZMax = z2,
                                                    MMin = m,
                                                    MMax = m2
                                                };

                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (reader.TryGetDouble(out x))
                        {
                            if (reader.Read() && reader.TryGetDouble(out y))
                            {
                                if (reader.Read() && reader.TryGetDouble(out x2))
                                {
                                    if (reader.Read() && reader.TryGetDouble(out y2))
                                    {
                                        if (reader.Read() && reader.TokenType == JsonToken.EndArray)
                                        {
                                            spatial = new Extent(x, y, x2, y2, null);
                                            return true;
                                        }
                                    }
                                }
                                else if (reader.TokenType == JsonToken.EndArray)
                                {
                                    spatial = new Point(x, y, null);
                                    return true;
                                }
                            }
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

            if (TryReadPoint(o, spatialReference, out var point)) return point;
            if (TryReadEnvelope(o, spatialReference, out var envelope)) return envelope;

            // Polyline
            {
                if (o.TryGetValue("paths", out var token))
                {
                    var paths = (JArray)token;
                    var polyline = new Polyline(spatialReference)
                    {
                        Paths = paths.Select(p => new Path(
                            p.Select(c => new Point(c.First.Value<double>(), c.Last.Value<double>())))).ToList()
                    };

                    return polyline;
                }
            }
            // Polygon
            {
                if (o.TryGetValue("rings", out var token))
                {
                    var rings = (JArray)token;
                    var polygon = new Polygon(spatialReference)
                    {
                        Rings = rings.Select(p => new Ring(
                            p.Select(c => new Point(c.First.Value<double>(), c.Last.Value<double>(), null)))).ToList()
                    };

                    return polygon;
                }
            }
            // Multipoint
            {
                if (o.TryGetValue("points", out var token))
                {
                    var points = (JArray)token;
                    var multiPoint = new MultiPoint(spatialReference)
                    {
                        Points = points.Select(c => new Point(c.First.Value<double>(), c.Last.Value<double>(), null)).ToList()
                    };

                    return multiPoint;
                }
            }

            return null;
        }
    }
}
