using System;
using System.Collections.Generic;
using System.Linq;
using Anywhere.ArcGIS.Common;

namespace Anywhere.ArcGIS.GeoJson
{
    public class GeoJsonGeometryCollection : GeoJsonGeometry
    {
        public GeoJsonGeometryCollection() { }

        public GeoJsonGeometryCollection(IEnumerable<IGeoJsonGeometry> geometries)
        {
            Geometries = geometries?.ToList();
        }

        public List<IGeoJsonGeometry> Geometries { get; set; }

        public override GeoJsonGeometryType Type => GeoJsonGeometryType.GeometryCollection;

        public override IGeometry ToSpatial()
        {
            throw new NotImplementedException();
        }

        public override void Accept(IGeoJsonVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
