using System;
using Anywhere.ArcGIS.Common;

namespace Anywhere.ArcGIS.GeoJson
{
    public abstract class GeoJsonGeometry : IGeoJsonGeometry
    {
        public abstract GeoJsonGeometryType Type { get; }

        [Obsolete("Use " + nameof(ToSpatial) + " instead")]
        public IGeometry ToGeometry() => ToSpatial();

        public abstract IGeometry ToSpatial();

        public abstract void Accept(IGeoJsonVisitor visitor);
    }
}
