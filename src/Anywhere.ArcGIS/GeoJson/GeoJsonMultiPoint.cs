using System;
using System.Collections.Generic;
using System.Linq;
using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.GeoJson
{
    public class GeoJsonMultiPoint : GeoJsonGeometry
    {
        public GeoJsonMultiPoint()
        {
        }

        public GeoJsonMultiPoint(List<double[]> coordinates)
        {
            Coordinates = coordinates;
        }

        public override GeoJsonGeometryType Type => GeoJsonGeometryType.MultiPoint;

        public List<double[]> Coordinates { get; set; }

        public override IGeometry ToSpatial()
        {
            if (Coordinates == null)
            {
                return null;
            }

            return new MultiPoint
            {
                Points = Coordinates.Select(c => new Point(c)).ToList()
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