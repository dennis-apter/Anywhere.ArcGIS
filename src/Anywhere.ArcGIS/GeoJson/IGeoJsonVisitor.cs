namespace Anywhere.ArcGIS.GeoJson
{
    public interface IGeoJsonVisitor
    {
        void Visit(GeoJsonPoint point);
        void Visit(GeoJsonLineString lineString);
        void Visit(GeoJsonPolygon polygon);
        void Visit(GeoJsonMultiPoint multiPoint);
        void Visit(GeoJsonMultiLineString multiLineString);
        void Visit(GeoJsonMultiPolygon multiPolygon);
        void Visit(GeoJsonGeometryCollection geometryCollection);
    }
}