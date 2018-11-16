using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serializers
{
    using ArcGIS;
    using Operation;
    using System.Collections.Generic;

    public class JsonDotNetSerializer : ISerializer
    {
        static ISerializer _serializer = null;

        public static ISerializer Create(JsonSerializerSettings settings = null)
        {
            if (_serializer == null)
            {
                _serializer = new JsonDotNetSerializer(settings);
            }
            return _serializer;
        }

        readonly JsonSerializerSettings _settings;

        public JsonDotNetSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                TypeNameHandling = TypeNameHandling.None
            };
        }

        public Dictionary<string, string> AsDictionary<T>(T objectToConvert) where T : CommonParameters
        {
            var jsonSerializer = JsonSerializer.CreateDefault(_settings);
            var obj = JObject.FromObject(objectToConvert, jsonSerializer);
            var result = new Dictionary<string, string>();
            foreach (var item in obj)
            {
                var value = item.Value.ToString(Formatting.None).Trim('\"');
                result.Add(item.Key, value);
            }

            return result;
        }

        public T AsPortalResponse<T>(string dataToConvert) where T : IPortalResponse
        {
            return JsonConvert.DeserializeObject<T>(dataToConvert, _settings);
        }

        public IPortalResponse AsPortalResponse(Type responseType, string dataToConvert)
        {
            var response = JsonConvert.DeserializeObject(dataToConvert, responseType, _settings);
            return (IPortalResponse) response;
        }
    }
}
