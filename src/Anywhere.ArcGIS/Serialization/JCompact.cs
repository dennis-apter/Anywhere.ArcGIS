using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serialization
{
    /// <summary>
    /// Utility for work with compact representation of ESRI geometric data structures.
    /// </summary>
    public static class JCompact
    {
        private sealed class CompactWriter : JsonTextWriter
        {
            private readonly StringWriter _textWriter;

            internal CompactWriter(StringWriter textWriter)
                : base(textWriter)
            {
                _textWriter = textWriter;
            }

            public bool IsEmpty => _textWriter.GetStringBuilder().Length == 0;
        }

        public static bool IsCompact(this JsonWriter self)
        {
            return self is CompactWriter w && w.IsEmpty;
        }

        public static string From(object o, JsonSerializer serializer = null)
        {
            if (o == null)
            {
                return null;
            }

            var s = o as string;
            if (s != null)
            {
                return s;
            }

            var builder = new StringBuilder(256);
            using (var writer = new CompactWriter(new StringWriter(builder, CultureInfo.InvariantCulture)))
            {
                (serializer ?? JsonSerializer.CreateDefault()).Serialize(writer, o);
            }

            var type = o.GetType();
            if (!type.IsArray && builder[0] == '[')
            {
                builder.Remove(0, 1);
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.ToString();
        }

        public static JToken Object(object o)
        {
            if (o == null)
            {
                return null;
            }

            var s = o.ToString();
            if (s == string.Empty)
            {
                return null;
            }

            switch (s[0])
            {
                // JSON
                case '{':
                case '[':
                case '"':
                case '\'':
                    return JToken.Parse(s);
                // Plain value
                default:
                    // TODO Интерпретировать строки в формате (key:value[;])+ как словарь { key: value }
                    if (s.Length < 3 || !s.Contains(','))
                    {
                        return Value(s);
                    }

                    var array = new JArray();
                    foreach (var part in s.Split(','))
                    {
                        if (part.Length == 0)
                        {
                            array.Add(null);
                        }
                        else
                        {
                            array.Add(Value(part));
                        }
                    }

                    return array;
            }
        }

        public static T To<T>(object o)
        {
            return Object(o).ToObject<T>();
        }

        public static T To<T>(object o, JsonSerializer serializer)
        {
            return Object(o).ToObject<T>(serializer);
        }

        private static JToken Value(string s)
        {
            switch (s)
            {
                case "null":
                    return null;
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    if (char.IsDigit(s[0]))
                    {
                        return JToken.Parse(s);
                    }
                    else if (1 < s.Length && (s[0] == '-' || s[0] == '+') && char.IsDigit(s[1]))
                    {
                        return JToken.Parse(s);
                    }

                    return s;
            }
        }

        public static bool TryGetInt32(this JsonReader reader, out int value)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                value = (int) (long) reader.Value;
            }
            else if (reader.TokenType == JsonToken.Float)
            {
                value = (int) (double) reader.Value;
            }
            else
            {
                value = 0;
                return false;
            }

            return true;
        }

        public static bool TryGetDouble(this JsonReader reader, out double value)
        {
            if (reader.TokenType == JsonToken.Float)
            {
                value = (double) reader.Value;
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                value = (long) reader.Value;
            }
            else
            {
                value = double.NaN;
                return false;
            }

            return true;
        }
    }
}