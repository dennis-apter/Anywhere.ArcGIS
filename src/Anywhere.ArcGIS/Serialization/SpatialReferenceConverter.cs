using System;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serialization
{
    public class SpatialReferenceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string) || objectType == typeof(SpatialReference);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sr = value as SpatialReference;
            if (sr == null)
            {
                writer.WriteNull();
                return;
            }

            if (writer.IsCompact() && string.IsNullOrEmpty(sr.Wkt))
            {
                writer.WriteValue(sr.Wkid);
            }
            else
            {
                writer.WriteStartObject();

                if (string.IsNullOrEmpty(sr.Wkt))
                {
                    writer.WritePropertyName("wkid");
                    writer.WriteValue(sr.Wkid);

                    if (sr.LatestWkid.HasValue)
                    {
                        writer.WritePropertyName("latestWkid");
                        writer.WriteValue(sr.LatestWkid.Value);
                    }

                    if (sr.VcsWkid.HasValue)
                    {
                        writer.WritePropertyName("vcsWkid");
                        writer.WriteValue(sr.VcsWkid.Value);
                    }

                    if (sr.LatestVcsWkid.HasValue)
                    {
                        writer.WritePropertyName("latestVcsWkid");
                        writer.WriteValue(sr.LatestVcsWkid.Value);
                    }
                }
                else
                {
                    writer.WritePropertyName("wkt");
                    writer.WriteValue(sr.Wkt);
                }

                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new SpatialReference((int)(long)reader.Value);
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return new SpatialReference((string)reader.Value);
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                var o = JObject.Load(reader);
                if (o.TryGetValue("wkt", out var wkt))
                {
                    return new SpatialReference(wkt.Value<string>());
                }

                if (o.TryGetValue("wkid", out var wkid))
                {
                    return new SpatialReference(wkid.Value<int>())
                    {
                        LatestWkid = o.Value<int?>("latestWkid"),
                        VcsWkid = o.Value<int?>("vcsWkid"),
                        LatestVcsWkid = o.Value<int?>("latestVcsWkid")
                    };
                }
            }

            return null;
        }
    }
}