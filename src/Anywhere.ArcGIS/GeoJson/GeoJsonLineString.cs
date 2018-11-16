using System;
using System.Collections.Generic;
using System.Linq;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;
using Coordinates = System.Collections.Generic.List<double[]>;

namespace Anywhere.ArcGIS.GeoJson
{
    public class GeoJsonLineString : GeoJsonGeometry
    {
        public GeoJsonLineString()
        {
        }

        public GeoJsonLineString(IEnumerable<double[]> coordinates)
        {
            Coordinates = coordinates?.ToList();
        }

        public override GeoJsonGeometryType Type => GeoJsonGeometryType.LineString;

        public Coordinates Coordinates { get; set; }

        public override IGeometry ToSpatial()
        {
            if (Coordinates == null)
            {
                return null;
            }

            return new Polyline
            {
                Paths = new List<Path>
                {
                    new Path(Coordinates.Select(c => new Point(c)))
                }
            };
        }

        public override void Accept(IGeoJsonVisitor visitor)
        {
            visitor.Visit(this);
        }

        public static GeoJsonLineString Parse(string value)
        {
            return JsonConvert.DeserializeObject<GeoJsonLineString>(value);
        }
    }
}
