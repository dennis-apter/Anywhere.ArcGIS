using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.GeoJson
{
    [DataContract]
    public class Crs
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "properties")]
        public CrsProperties Properties { get; set; }
    }
}