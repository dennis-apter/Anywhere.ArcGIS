using System.Collections.Generic;
using System.Runtime.Serialization;
using Anywhere.ArcGIS.Common;

namespace Anywhere.ArcGIS.GeoJson
{
    [DataContract]
    public class GeoJsonFeature<TGeometry> : IFeatureAttributes
        where TGeometry : IGeoJsonGeometry
    {
        [DataMember(Name = "id")]
        public object Id { get; set; }

        [DataMember(Name = "properties")]
        public IDictionary<string, object> Properties { get; set; }

        IDictionary<string, object> IFeatureAttributes.Attributes => Properties;

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "bbox")]
        public double[] BoundingBox { get; set; }

        [DataMember(Name = "geometry")]
        public TGeometry Geometry { get; set; }
    }
}
