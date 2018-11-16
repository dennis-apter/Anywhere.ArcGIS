namespace Anywhere.ArcGIS.Common
{
    public interface ISpatialVisitor
    {
        void Visit(MultiPoint multipoint);
        void Visit(Point point);
        void Visit(Extent extent);
        void Visit(Polyline polyline);
        void Visit(Polygon polygon);
        void Visit(SpatialReference spatialReference);
    }
}
