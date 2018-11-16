using Anywhere.ArcGIS.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Operation
{
    /// <summary>
    /// Find request operation
    /// </summary>
    [DataContract]
    public class Find : ArcGISServerOperation
    {
        public Find(string relativeUrl, Action beforeRequest = null, Action afterRequest = null)
            : this(relativeUrl.AsEndpoint(), beforeRequest, afterRequest)
        { }

        /// <summary>
        /// Represents a request for a find against a service resource
        /// </summary>
        /// <param name="endpoint">Resource to apply the query against</param>
        public Find(ArcGISServerEndpoint endpoint, Action beforeRequest = null, Action afterRequest = null)
            : base(endpoint.RelativeUrl.Trim('/') + "/" + Operations.Find, beforeRequest, afterRequest)
        {
            Contains = true;
            ReturnGeometry = true;
            ReturnZ = true;
        }

        /// <summary>
        /// The search string. This is the text that is searched across the layers and fields the user specifies.
        /// </summary>
        [DataMember(Name = "searchText")]
        public string SearchText { get; set; }

        /// <summary>
        /// If false, the operation searches for an exact match of the searchText string. An exact match is case sensitive.
        /// Otherwise, it searches for a value that contains the searchText provided. This search is not case sensitive.
        /// The default is true.
        /// </summary>
        [DataMember(Name = "contains")]
        public bool Contains { get; set; }

        /// <summary>
        /// If false, the operation searches for an exact match of the SearchText string. An exact match is case sensitive.
        /// Otherwise, it searches for a value that contains the searchText provided. This search is not case sensitive
        /// </summary>
        /// <remarks>Default is true</remarks>
        [IgnoreDataMember]
        [Obsolete("Use Contains property instead")]
        public bool FuzzySearch { get => Contains; set => Contains = value; }

        /// <summary>
        /// If true, the resultset includes the geometry associated with each result.
        /// </summary>
        /// <remarks>Default is true</remarks>
        [DataMember(Name = "returnGeometry")]
        public bool ReturnGeometry { get; set; }

        /// <summary>
        /// The well-known ID of the spatial reference of the output geometries.
        /// </summary>
        [IgnoreDataMember]
        [Obsolete("Use OutputSpatialReference instead")]
        public int? OutputSpatialReferenceValue
        {
            get { return OutputSpatialReference?.Wkid; }
            set { OutputSpatialReference = value.HasValue ? new SpatialReference(value.Value) : null; }
        }

        /// <summary>
        /// The well-known ID of the spatial reference of the output geometries.
        /// </summary>
        [DataMember(Name = "sr")]
        public SpatialReference OutputSpatialReference { get; set; }

        /// <summary>
        ///  The names of the fields to search. The fields are specified as a comma-separated list of field names.
        /// If this parameter is not specified, all fields are searched.
        /// </summary>
        [IgnoreDataMember]
        public List<string> SearchFields { get; set; }

        /// <summary>
        /// <para>Syntax: searchFields=&lt;fieldName1&gt;,&lt;fieldName2&gt;</para>
        /// <para>Where fieldName1, fieldName2 are the field names returned by the layer resource.</para>
        /// <para>Example: <example>searchFields=AREANAME,SUB_REGION</example></para>
        /// </summary>
        [DataMember(Name = "searchFields")]
        public string SearchFieldsValue
        {
            get { return SearchFields == null ? string.Empty : string.Join(",", SearchFields); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var parts = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        SearchFields = new List<string>(parts);
                        return;
                    }
                }

                SearchFields = null;
            }
        }

        /// <summary>
        ///  The layers to perform the find operation on.
        /// </summary>
        [IgnoreDataMember]
        public List<int> LayerIdsToSearch { get; set; }

        [DataMember(Name = "layers")]
        public string LayerIdsToSearchValue
        {
            get { return LayerIdsToSearch == null ? string.Empty : string.Join(",", LayerIdsToSearch); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var parts = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        LayerIdsToSearch = new List<int>(parts.Length);
                        foreach (var part in parts)
                        {
                            if (int.TryParse(part, out var id))
                            {
                                LayerIdsToSearch.Add(id);
                            }
                        }

                        return;
                    }
                }

                LayerIdsToSearch = null;
            }
        }

        /// <summary>
        /// <para>New in 10.0.</para>
        /// <para>
        /// Allows you to filter the features of individual layers in the exported map by specifying definition expressions for those layers.
        /// Definition expression for a layer that is published with the service will be always honored.
        /// </para>
        /// </summary>
        [IgnoreDataMember]
        public IDictionary<int, string> LayerDefinitions { get; set; }

        /// <summary>
        /// <para>Syntax: { "&lt;layerId1&gt;" : "&lt;layerDef1&gt;" , "&lt;layerId2&gt;" : "&lt;layerDef2&gt;" }</para>
        /// <para></para>
        /// </summary>
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
        /// This option can be used to specify the maximum allowable offset to be used for generalizing geometries returned by the find operation.
        /// </summary>
        [DataMember(Name = "maxAllowableOffset")]
        public int? MaxAllowableOffset { get; set; }

        /// <summary>
        /// This option can be used to specify the number of decimal places in the response geometries returned by the find operation.
        /// This applies to X and Y values only (not m or z values).
        /// </summary>
        [DataMember(Name = "geometryPrecision")]
        public int? GeometryPrecision { get; set; }

        /// <summary>
        /// If true, Z values will be included in the results if the features have Z values. Otherwise, Z values are not returned.
        /// </summary>
        /// <remarks>Default is true. This parameter only applies if returnGeometry=true.</remarks>
        [DataMember(Name = "returnZ")]
        public bool ReturnZ { get; set; }

        /// <summary>
        /// If true, M values will be included in the results if the features have M values. Otherwise, M values are not returned.
        /// </summary>
        /// <remarks>Default is false. This parameter only applies if returnGeometry=true.</remarks>
        [DataMember(Name = "returnM")]
        public bool ReturnM { get; set; }

        /// <summary>
        /// Switch map layers to point to an alternate geodabase version.
        /// </summary>
        [DataMember(Name = "gdbVersion")]
        public string GeodatabaseVersion { get; set; }

        /// <summary>
        /// If true, the values in the result will not be formatted i.e. numbers will returned as is and dates will be returned as epoch values.
        /// This option was added at 10.5.
        /// </summary>
        [DataMember(Name = "returnUnformattedValues")]
        public bool ReturnUnformattedValues { get; set; }

        /// <summary>
        /// If true, field names will be returned instead of field aliases.
        /// This option was added at 10.5.
        /// </summary>
        [DataMember(Name = "returnFieldName")]
        public bool ReturnFieldNames { get; set; }

        /// <summary>
        /// <para>This option was added at 10.6.1.</para>
        /// <para>
        /// Features from the historic moment to identify.
        /// This parameter applies only if the layer is archiving enabled and the supportsQueryWithHistoricMoment property is set to true.
        /// This property is provided in the layer resource.
        /// </para>
        /// </summary>
        [DataMember(Name = "historicMoment")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? HistoricMoment { get; set; }
    }

    [DataContract]
    public class FindResponse : PortalResponse
    {
        [DataMember(Name = "results")]
        public FindResult[] Results { get; set; }
    }

    [DataContract]
    public class FindResult : IFeatureAttributes
    {
        [DataMember(Name = "layerId")]
        public int LayerId { get; set; }

        [DataMember(Name = "layerName")]
        public string LayerName { get; set; }

        [DataMember(Name = "displayFieldName")]
        public string DisplayFieldName { get; set; }

        [DataMember(Name = "foundFieldName")]
        public string FoundFieldName { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "attributes")]
        public IDictionary<string, object> Attributes { get; set; }

        [DataMember(Name = "geometryType")]
        public string GeometryType { get; set; }

        [DataMember(Name = "geometry")]
        public object Geometry { get; set; }
    }
}
