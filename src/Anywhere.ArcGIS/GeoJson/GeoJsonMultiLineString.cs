using System.Collections.Generic;
using System.Linq;
using Anywhere.ArcGIS.Common;

namespace Anywhere.ArcGIS.GeoJson
{
    public class GeoJsonMultiLineString : GeoJsonGeometry
    {
        public GeoJsonMultiLineString()
        {
        }

        public GeoJsonMultiLineString(List<List<double[]>> coordinates)
        {
            Coordinates = coordinates;
        }

        public override GeoJsonGeometryType Type => GeoJsonGeometryType.MultiLineString;

        public List<List<double[]>> Coordinates { get; set; }

        public override IGeometry ToSpatial()
        {
            if (Coordinates == null)
            {
                return null;
            }

            return new Polyline
            {
                Paths = Coordinates.Select(r => new Path(r.Select(p => new Point(p)))).ToList()
            };
        }

        public override void Accept(IGeoJsonVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
