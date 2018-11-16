using Anywhere.ArcGIS.GeoJson;

namespace Anywhere.ArcGIS.Common
{
    public abstract class SpatialGeometry : Geometry, IGeometry
    {
        protected SpatialGeometry()
        {
        }

        protected SpatialGeometry(SpatialReference spatialReference)
        {
            SpatialReference = spatialReference;
        }

        public SpatialReference SpatialReference { get; set; }

        void IGeometry.Accept(ISpatialVisitor visitor) => AcceptImpl(visitor);
        protected abstract void AcceptImpl(ISpatialVisitor visitor);

        public abstract Point GetCenter();
        public abstract Point GetPole();
        public abstract IGeoJsonGeometry ToGeoJson();
    }
}
