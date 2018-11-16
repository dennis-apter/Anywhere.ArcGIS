using System;
using System.Linq;
using Anywhere.ArcGIS.Common;
using Coordinates = System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<double[]>>>;

namespace Anywhere.ArcGIS.GeoJson
{
    public class GeoJsonMultiPolygon : GeoJsonGeometry
    {
        public override GeoJsonGeometryType Type => GeoJsonGeometryType.MultiPolygon;

        public GeoJsonMultiPolygon() { }

        public GeoJsonMultiPolygon(Coordinates coordinates)
        {
            Coordinates = coordinates;
        }

        public Coordinates Coordinates { get; set; }

        public override IGeometry ToSpatial()
        {
            // TODO Convert to Bag?

            if (Coordinates == null) return null;

            if (Coordinates.Count > 1)
            {
                var coordinates = Coordinates[0];
                var result = new Polygon
                {
                    Rings = coordinates.Select(r => new Ring(r.Select(c => new Point(c)))).ToList()
                };

                return result;
            }

            return null;
        }

        public override void Accept(IGeoJsonVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
