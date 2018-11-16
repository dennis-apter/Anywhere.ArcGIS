using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.GeoJson
{
    [DataContract]
    public class GeoJsonFeatureCollection<TGeometry> where TGeometry : IGeoJsonGeometry
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "bbox")]
        public double[] BoundingBox { get; set; }

        [DataMember(Name = "features")]
        public List<GeoJsonFeature<TGeometry>> Features { get; set; }

        [DataMember(Name = "crs")]
        public Crs CoordinateReferenceSystem { get; set; }
    }
}
