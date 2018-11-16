using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Serialization
{
    internal class SpatialEmitter : ISpatialVisitor
    {
        private readonly JsonWriter _writer;

        public SpatialEmitter(JsonWriter writer)
        {
            _writer = writer;
        }

        public void Visit(MultiPoint multipoint)
        {
            _writer.WriteStartObject();

            if (multipoint.HasM)
            {
                _writer.WritePropertyName("hasM");
                _writer.WriteValue(true);
            }

            if (multipoint.HasZ)
            {
                _writer.WritePropertyName("hasZ");
                _writer.WriteValue(true);
            }

            _writer.WritePropertyName("points");
            _writer.WriteStartArray();
            foreach (var p in multipoint.Points)
            {
                _writer.WriteStartArray();
                _writer.WriteValue(p.X);
                _writer.WriteValue(p.Y);
                //TODO разобраться с ситуацией, когда возвращается M, но не Z. Третьим компонентом становится M, или пишется 0 в третьем компоненте, а M - в четвертом?
                if (p.Z.HasValue)
                {
                    _writer.WriteValue(p.Z);
                }

                if (p.M.HasValue)
                {
                    _writer.WriteValue(p.M);
                }

                _writer.WriteEndArray();
            }

            _writer.WriteEndArray();

            WriteSpatialReference(multipoint);

            _writer.WriteEndObject();
        }

        public void Visit(Point point)
        {
            _writer.WriteStartObject();

            _writer.WritePropertyName("x");
            _writer.WriteValue(point.X);
            _writer.WritePropertyName("y");
            _writer.WriteValue(point.Y);
            if (point.Z.HasValue)
            {
                _writer.WritePropertyName("z");
                _writer.WriteValue(point.Z);
            }

            if (point.M.HasValue)
            {
                _writer.WritePropertyName("m");
                _writer.WriteValue(point.M);
            }

            WriteSpatialReference(point);

            _writer.WriteEndObject();
        }

        public void Visit(Extent extent)
        {
            _writer.WriteStartObject();

            _writer.WritePropertyName("xmin");
            _writer.WriteValue(extent.XMin);
            _writer.WritePropertyName("xmax");
            _writer.WriteValue(extent.XMax);

            _writer.WritePropertyName("ymin");
            _writer.WriteValue(extent.YMin);
            _writer.WritePropertyName("ymax");
            _writer.WriteValue(extent.YMax);
            if (extent.MMax.HasValue)
            {
                _writer.WritePropertyName("mmax");
                _writer.WriteValue(extent.MMax);
            }

            if (extent.MMin.HasValue)
            {
                _writer.WritePropertyName("mmin");
                _writer.WriteValue(extent.MMin);
            }

            if (extent.ZMax.HasValue)
            {
                _writer.WritePropertyName("zmax");
                _writer.WriteValue(extent.ZMax);
            }

            if (extent.ZMin.HasValue)
            {
                _writer.WritePropertyName("zmin");
                _writer.WriteValue(extent.ZMin);
            }

            WriteSpatialReference(extent);

            _writer.WriteEndObject();
        }

        public void Visit(Polyline polyline)
        {
            _writer.WriteStartObject();

            if (polyline.HasM)
            {
                _writer.WritePropertyName("hasM");
                _writer.WriteValue(true);
            }

            if (polyline.HasZ)
            {
                _writer.WritePropertyName("hasZ");
                _writer.WriteValue(true);
            }

            _writer.WritePropertyName("paths");
            _writer.WriteStartArray();
            foreach (var ring in polyline.Paths)
            {
                _writer.WriteStartArray();
                foreach (var p in ring.Points)
                {
                    _writer.WriteStartArray();
                    _writer.WriteValue(p.X);
                    _writer.WriteValue(p.Y);
                    //TODO разобраться с ситуацией, когда возвращается M, но не Z. Третьим компонентом становится M, или пишется 0 в третьем компоненте, а M - в четвертом?
                    if (p.Z.HasValue)
                    {
                        _writer.WriteValue(p.Z);
                    }

                    if (p.M.HasValue)
                    {
                        _writer.WriteValue(p.M);
                    }

                    _writer.WriteEndArray();
                }

                _writer.WriteEndArray();
            }

            _writer.WriteEndArray();

            WriteSpatialReference(polyline);

            _writer.WriteEndObject();
        }

        public void Visit(Polygon polygon)
        {
            _writer.WriteStartObject();

            if (polygon.HasM)
            {
                _writer.WritePropertyName("hasM");
                _writer.WriteValue(true);
            }

            if (polygon.HasZ)
            {
                _writer.WritePropertyName("hasZ");
                _writer.WriteValue(true);
            }

            _writer.WritePropertyName("rings");
            _writer.WriteStartArray();
            foreach (var ring in polygon.Rings)
            {
                _writer.WriteStartArray();
                foreach (var p in ring.Points)
                {
                    _writer.WriteStartArray();
                    _writer.WriteValue(p.X);
                    _writer.WriteValue(p.Y);
                    //TODO разобраться с ситуацией, когда возвращается M, но не Z. Третьим компонентом становится M, или пишется 0 в третьем компоненте, а M - в четвертом?
                    if (p.Z.HasValue)
                    {
                        _writer.WriteValue(p.Z);
                    }

                    if (p.M.HasValue)
                    {
                        _writer.WriteValue(p.M);
                    }

                    _writer.WriteEndArray();
                }

                _writer.WriteEndArray();
            }

            _writer.WriteEndArray();

            WriteSpatialReference(polygon);

            _writer.WriteEndObject();
        }

        public void Visit(SpatialReference spatialReference)
        {
            _writer.WriteStartObject();

            if (spatialReference.Wkid != 0)
            {
                _writer.WritePropertyName("wkid");
                _writer.WriteValue(spatialReference.Wkid);
            }
            else
            {
                _writer.WritePropertyName("wkt");
                _writer.WriteValue(spatialReference.Wkt);
            }

            _writer.WriteEndObject();
        }

        private void WriteSpatialReference(IGeometry element)
        {
            if (element.SpatialReference != null)
            {
                _writer.WritePropertyName("spatialReference");
                Visit(element.SpatialReference);
            }
        }
    }
}