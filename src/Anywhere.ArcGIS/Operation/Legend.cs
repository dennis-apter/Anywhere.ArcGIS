using System;
using System.Runtime.Serialization;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Operation
{
    public class Legend : QueryBase
    {
        public Legend(IEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint, beforeRequest, afterRequest)
        { }

        public Legend(string endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint, beforeRequest, afterRequest)
        { }
    }

    [DataContract]
    public class LegendResponse : PortalResponse
    {
        [DataMember(Name = "layers")]
        public LayerLegendInfo[] Layers { get; set; }
    }

    [DataContract]
    public sealed class LayerLegendInfo
    {
        [DataMember(Name = "layerId")]
        public int LayerId { get; set; }

        [DataMember(Name = "layerName")]
        public string LayerName { get; set; }

        [DataMember(Name = "layerType")]
        public string LayerType { get; set; }

        [DataMember(Name = "maxScale", EmitDefaultValue = false)]
        public double? MaxScale { get; set; }

        [DataMember(Name = "minScale", EmitDefaultValue = false)]
        public double? MinScale { get; set; }

        [DataMember(Name = "legend")]
        public LegendItemInfo[] Legend { get; set; }
    }

    [DataContract]
    public sealed class LegendItemInfo
    {
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "imageData")]
        public string ImageData { get; set; }

        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "height", EmitDefaultValue = false)]
        public int? Height { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "width", EmitDefaultValue = false)]
        public int? Width { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "values")]
        public object[] Values { get; set; }
    }
}
