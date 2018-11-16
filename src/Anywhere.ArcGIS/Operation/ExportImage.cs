using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Operation
{
    public class ExportImage : ArcGISServerOperation
    {
        public ExportImage(IEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint, beforeRequest, afterRequest)
        { }

        public ExportImage(string endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint, beforeRequest, afterRequest)
        { }

        [DataMember(Name = "bbox")]
        public Extent BoundingBox { get; set; }

        [DataMember(Name = "bboxSr")]
        public SpatialReference BoundingBoxSpatialReference { get; set; }

        [DataMember(Name = "imageSr")]
        public SpatialReference ImageSpatialReference { get; set; }

        [DataMember(Name = "dpi")]
        public double Dpi { get; set; }

        [DataMember(Name = "size")]
        public Size Size { get; set; }

        [DataMember(Name = "transparent")]
        public bool Transparent { get; set; }

        [IgnoreDataMember]
        public LayerIdsMethod? LayerIdsMethod { get; set; }

        /// <summary>
        ///  The layers to perform the export operation on.
        /// </summary>
        [IgnoreDataMember]
        public List<int> LayerIdsToExport { get; set; }

        [DataMember(Name = "layers")]
        public string LayerIdsToExportValue
        {
            get
            {
                if (LayerIdsToExport != null && LayerIdsToExport.Count > 0)
                {
                    var method = LayerIdsMethod.HasValue 
                        ? LayerIdsMethod.Value.ToString().ToLowerInvariant() + ":"
                        : string.Empty;

                    return method + string.Join(",", LayerIdsToExport);
                }

                return null;
            }
            set
            {
                LayerIdsMethod = null;
                LayerIdsToExport = null;

                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Trim();
                    var index = value.IndexOf(':');
                    if (index > 0)
                    {
                        var methodValue = value.Substring(0, index);
                        if (Enum.TryParse(methodValue, true, out LayerIdsMethod method))
                        {
                            LayerIdsMethod = method;
                        }

                        value = value.Substring(index + 1);
                    }

                    var parts = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        LayerIdsToExport = new List<int>(parts.Length);
                        foreach (var part in parts)
                        {
                            if (int.TryParse(part, out var id))
                            {
                                LayerIdsToExport.Add(id);
                            }
                        }
                    }
                }
            }
        }

        [IgnoreDataMember]
        public IDictionary<int, string> LayerDefinitions { get; set; }

        [DataMember(Name = "layerDefs")]
        public string LayerDefinitionsValue
        {
            get
            {
                if (LayerDefinitions == null || LayerDefinitions.Count == 0)
                {
                    return null;
                }

                var builder = new StringBuilder();
                foreach (var definition in LayerDefinitions)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(";");
                    }

                    builder.Append(definition.Key).Append(":").Append(definition.Value);
                }

                return builder.ToString();
            }
            set
            {
                LayerDefinitions = null;

                if (!string.IsNullOrEmpty(value))
                {
                    if (value.StartsWith("{"))
                    {
                        // JSON Syntax: { "<layerId1>" : "<layerDef1>" , "<layerId2>" : "<layerDef2>" }

                        LayerDefinitions = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
                    }
                    else
                    {
                        // Simple Syntax: <layerId1>:<layerDef1>;<layerId2>:<layerDef2>

                        var pairs = value.Split(';');
                        if (0 < pairs.Length)
                        {
                            LayerDefinitions = new Dictionary<int, string>(pairs.Length);
                            foreach (var pair in pairs)
                            {
                                var index = pair.IndexOf(':');
                                if (index > 0)
                                {
                                    var key = pair.Substring(0, index);
                                    if (int.TryParse(key, out var id))
                                    {
                                        var def = pair.Substring(index + 1);
                                        LayerDefinitions.Add(id, def);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///This option was added at 10.1 
        ///Description: Use this parameter to export a map image at a specific scale, with the map centered around the center of the specified bounding box (bbox). 
        /// </summary>
        [DataMember(Name = "mapScale")]
        public double? MapScale { get; set; }

        [DataMember(Name = "datumTransformations")]
        public string DatumTransformations { get; set; }

        [DataMember(Name = "mapRangeValues")]
        public string MapRangeValues { get; set; }

        [DataMember(Name = "layerRangeValues")]
        public string LayerRangeValues { get; set; }

        [DataMember(Name = "layerParameterValues")]
        public string LayerParameterValues { get; set; }
    }

    public enum LayerIdsMethod
    {
        Show,
        Hide,
        Include,
        Exclude,
    }
}
