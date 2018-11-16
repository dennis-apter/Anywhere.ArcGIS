using System.Collections.Generic;
using System.Linq;
using Anywhere.ArcGIS.Common;
using Coordinates = System.Collections.Generic.List<System.Collections.Generic.List<double[]>>;

namespace Anywhere.ArcGIS.GeoJson
{
    public class GeoJsonPolygon : GeoJsonGeometry
    {
        public GeoJsonPolygon()
        {
        }

        public GeoJsonPolygon(IEnumerable<IEnumerable<double[]>> coordinates)
        {
            Coordinates = coordinates?.Select(c => c?.ToList()).ToList();
        }

        public override GeoJsonGeometryType Type => GeoJsonGeometryType.Polygon;

        public Coordinates Coordinates { get; set; }

        public override IGeometry ToSpatial()
        {
            if (Coordinates == null)
            {
                return null;
            }

            return new Polygon
            {
                Rings = Coordinates.Select(r => new Ring(r.Select(p => new Point(p)))).ToList()
            };
        }

        public override void Accept(IGeoJsonVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
