using System;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serialization
{
    public sealed class SizeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Size);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var sr = value as Size;
            if (sr == null)
            {
                writer.WriteNull();
                return;
            }

            if (writer.IsCompact())
            {
                writer.WriteStartArray();
                writer.WriteValue(sr.Width);
                writer.WriteValue(sr.Height);
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteStartObject();

                writer.WritePropertyName("w");
                writer.WriteValue(sr.Width);

                writer.WritePropertyName("h");
                writer.WriteValue(sr.Height);

                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                int w;
                if (reader.Read() && reader.TryGetInt32(out w))
                {
                    int h;
                    if (reader.Read() && reader.TryGetInt32(out h))
                    {
                        if (reader.Read() && reader.TokenType == JsonToken.EndArray)
                        {
                            return new Size(w, h);
                        }
                    }
                    else if (reader.TokenType == JsonToken.EndArray)
                    {
                        return new Size(w);
                    }
                }
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                var o = JObject.Load(reader);

                JToken w, h;
                if (o.TryGetValue("w", out w) &&
                    o.TryGetValue("h", out h))
                {
                    return new Size
                    {
                        Width = w.Value<int>(),
                        Height = h.Value<int>(),
                    };
                }
            }
            else
            {
                int s;
                if (reader.TryGetInt32(out s))
                {
                    return new Size(s);
                }
            }

            return null;
        }
    }
}