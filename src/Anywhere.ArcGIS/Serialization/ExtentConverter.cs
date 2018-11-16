using System;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serialization
{
    public sealed class ExtentConverter : SpatialConverterBase
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Extent).IsAssignableFrom(objectType);
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
                    double n1, n3, n2, n4;
                    double? z = null, z2 = null, m = null, m2 = null;
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        if (reader.Read() && reader.TryGetDouble(out n1))
                        {
                            if (reader.Read() && reader.TryGetDouble(out n2))
                            {
                                int i = 0;
                                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                                {
                                    if (reader.TryGetDouble(out var d))
                                    {
                                        if (i == 0) z = d;
                                        else if (i == 1) m = d;
                                    }
                                    i++;
                                }

                                if (reader.Read() && reader.TokenType == JsonToken.StartArray)
                                {
                                    if (reader.Read() && reader.TryGetDouble(out n3))
                                    {
                                        if (reader.Read() && reader.TryGetDouble(out n4))
                                        {
                                            i = 0;
                                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                                            {
                                                if (reader.TryGetDouble(out var d))
                                                {
                                                    if (i == 0) z2 = d;
                                                    else if (i == 1) m2 = d;
                                                }
                                                i++;
                                            }

                                            if (reader.Read() && reader.TokenType == JsonToken.EndArray)
                                            {
                                                spatial = new Extent(n1, n2, n3, n4, null)
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
                        if (reader.TryGetDouble(out n1))
                        {
                            if (reader.Read() && reader.TryGetDouble(out n2))
                            {
                                if (reader.Read() && reader.TryGetDouble(out n3))
                                {
                                    if (reader.Read() && reader.TryGetDouble(out n4))
                                    {
                                        if (reader.Read())
                                        {
                                            if (reader.TokenType == JsonToken.EndArray)
                                            {
                                                spatial = new Extent(n1, n2, n3, n4, null);
                                                return true;
                                            }
                                            else if (reader.TryGetDouble(out var n5))
                                            {
                                                // Z
                                                if (reader.Read() && reader.TryGetDouble(out var n6))
                                                {
                                                    if (reader.Read())
                                                    {
                                                        if (reader.TokenType == JsonToken.EndArray)
                                                        {
                                                            spatial = new Extent(n1, n2, n3, n4, n5, n6, null);
                                                            return true;
                                                        }
                                                        else if (reader.TryGetDouble(out var n7))
                                                        {
                                                            // M
                                                            if (reader.Read() && reader.TryGetDouble(out var n8))
                                                            {
                                                                if (reader.Read())
                                                                {
                                                                    if (reader.TokenType == JsonToken.EndArray)
                                                                    {
                                                                        spatial = new Extent(n1, n2, n3, n4, n5, n6, n7, n8, null);
                                                                        return true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
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

            if (TryReadEnvelope(o, spatialReference, out var envelope)) return envelope;

            return null;
        }
    }
}
