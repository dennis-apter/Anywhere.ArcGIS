using System.Runtime.Serialization;

namespace Anywhere.ArcGIS.GeoJson
{
    [DataContract]
    public class CrsProperties
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "code")]
        public int Wkid { get; set; }
    }
}