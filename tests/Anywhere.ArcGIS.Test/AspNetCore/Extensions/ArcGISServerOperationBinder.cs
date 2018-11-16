using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.Operation;
using Anywhere.ArcGIS.Serialization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Anywhere.ArcGIS.Test.AspNetCore.Extensions
{
    public sealed class ArcGISServerOperationBinder : IModelBinder
    {
        private static readonly FormOptions DefaultFormOptions = new FormOptions();
        private readonly JsonSerializer _serializer;
        private readonly IDictionary<Type, JsonObjectContract> _contracts;

        public ArcGISServerOperationBinder(Type modelType)
        {
            _serializer = JsonSerializer.Create(/*TODO Apply JSON settings from configuration*/);
            _contracts = ResolveContracts(modelType, _serializer);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var resolver = new DynamicContractResolver(_contracts);

            JObject json;
            var request = bindingContext.HttpContext.Request;
            if (request.HasFormContentType)
            {
                json = new JObject();

                if (request.Form != null)
                {
                    foreach (string key in request.Form.Keys)
                    {
                        if (resolver.TryParse(key, request.Form[key], out var token))
                        {
                            json[key] = token;
                        }
                    }
                }
            }
            else if (MultipartRequestHelper.IsMultipartContentType(request.ContentType))
            {
                json = new JObject();
                var boundary = MultipartRequestHelper.GetBoundary(
                    MediaTypeHeaderValue.Parse(request.ContentType),
                    DefaultFormOptions.MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, request.Body);

                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var d))
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(d))
                        {
                            // TODO Read Body
                            // await section.Body.CopyToAsync(targetStream);
                        }
                        else if (MultipartRequestHelper.HasFormDataContentDisposition(d))
                        {
                            // Content-Disposition: form-data; name="key" 
                            // 
                            // value 

                            // Do not limit the key name length here because the 
                            // multipart headers length limit is already in effect. 
                            var key = HeaderUtilities.RemoveQuotes(d.Name);
                            var encoding = GetEncoding(section.ContentType);
                            using (var streamReader = new StreamReader(
                                section.Body,
                                encoding,
                                true,
                                1024,
                                true))
                            {
                                // The value length limit is enforced by MultipartBodyLengthLimit 
                                var value = await streamReader.ReadToEndAsync();
                                if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                                {
                                    value = string.Empty;
                                }

                                // TODO
                            }
                        }

                        // TODO
                    }

                    // Drains any remaining section body that has not been consumed and 
                    // reads the headers for the next section. 
                    section = await reader.ReadNextSectionAsync();
                }
            }
            else
            {
                var encoding = GetEncoding(request.ContentType);
                using (var streamReader = new StreamReader(
                    request.Body,
                    encoding,
                    true,
                    1024,
                    true))
                {
                    // The value length limit is enforced by MultipartBodyLengthLimit 
                    var body = await streamReader.ReadToEndAsync();
                    if (string.Equals(body, "undefined", StringComparison.OrdinalIgnoreCase))
                    {
                        body = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(body))
                    {
                        try
                        {
                            json = JObject.Parse(body);
                        }
                        catch
                        {
                            json = new JObject();
                            foreach (var s in body.Split('&'))
                            {
                                if (string.IsNullOrWhiteSpace(s)) continue;
                                var arr = s.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr != null && arr.Length == 2)
                                {
                                    if (resolver.TryParse(arr[0], arr[1], out var token))
                                        json[arr[0]] = token;
                                }
                            }
                        }
                    }
                    else
                    {
                        json = new JObject();
                    }
                }
            }

            foreach (var pair in bindingContext.ActionContext.RouteData.Values)
            {
                if (resolver.TryParse(pair.Key, pair.Value, out var token))
                {
                    json[pair.Key] = token;
                }
            }

            foreach (var pair in request.Query)
            {
                if (string.IsNullOrEmpty(pair.Value))
                    continue;

                var value = pair.Value.Count == 1
                    ? (object)pair.Value[0]
                    : pair.Value;

                if (resolver.TryParse(pair.Key, value, out var token))
                {
                    json[pair.Key] = token;
                }
            }

            var result = resolver.Create(request.Path, json, _serializer);
            bindingContext.Result = ModelBindingResult.Success(result);
        }

        private static IDictionary<Type, JsonObjectContract> ResolveContracts(Type type, JsonSerializer serializer)
        {
            var result = new Dictionary<Type, JsonObjectContract>();
            void AddContract(Type t)
            {
                var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(t);
                result.Add(t, contract);
            }

            if (type == typeof(Query))
            {
                AddContract(typeof(Query));
                AddContract(typeof(QueryForIds));
                AddContract(typeof(QueryForCount));
                AddContract(typeof(QueryForExtent));
            }
            else
            {
                AddContract(type);
            }

            return result;
        }

        private class DynamicContractResolver
        {
            //private static readonly ArcGISServerEndpoint EmptyEndpoint = new ArcGISServerEndpoint("/");

            private JsonObjectContract _resultContract;
            private readonly IDictionary<Type, JsonObjectContract> _contracts;

            public DynamicContractResolver(IDictionary<Type, JsonObjectContract> contracts)
            {
                _contracts = contracts;
            }

            /// <summary>
            /// Разбор значений в формате application/x-www-form-urlencoded
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="token"></param>
            /// <returns></returns>
            public bool TryParse(string key, object value, out JToken token)
            {
                token = null;
                JsonProperty property = null;

                if (_resultContract != null)
                {
                    property = _resultContract.Properties.GetClosestMatchProperty(key);
                }

                if (property == null)
                {
                    foreach (var pair in _contracts)
                    {
                        property = pair.Value.Properties.GetClosestMatchProperty(key);
                        if (property != null)
                        {
                            _resultContract = pair.Value;
                            break;
                        }
                    }

                    if (property == null)
                    {
                        return false;
                    }
                }

                if (property.PropertyType == typeof(string) ||
                    property.PropertyType.IsPrimitive ||
                    property.PropertyType.IsEnum)
                {
                    token = JToken.FromObject(value);
                }
                else
                {
                    token = JCompact.Object(value);

                    if (property.PropertyType.IsArray &&
                        token.Type != JTokenType.Array)
                    {
                        token = new JArray(token);
                    }
                }

                return true;
            }

            public ArcGISServerOperation Create(string path, JObject obj, JsonSerializer serializer)
            {
                if (_resultContract == null)
                {
                    return null;
                }

                try
                {
                    var endpoint = new ArcGISServerEndpoint(path);
                    var result = (ArcGISServerOperation)Activator.CreateInstance(
                        _resultContract.CreatedType, new object[] { endpoint, null, null });

                    using (var jtokenReader = new JTokenReader(obj))
                    {
                        serializer.Populate(jtokenReader, result);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    //
                }

                return null;
            }
        }

        private static Encoding GetEncoding(string contentType)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(contentType, out var mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases. 
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }
}
