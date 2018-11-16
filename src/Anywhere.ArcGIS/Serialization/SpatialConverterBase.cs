using Anywhere.ArcGIS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anywhere.ArcGIS.Serialization
{
    public abstract class SpatialConverterBase : JsonConverter
    {
        protected SpatialReference ReadSpatialReference(JObject o)
        {
            if (o.TryGetValue("spatialReference", out var sr))
            {
                if (sr is JObject sro)
                {
                    return sro.ToObject<SpatialReference>();
                }
                else if (sr.Type == JTokenType.String)
                {
                    return new SpatialReference(sr.Value<string>());
                }
                else if (sr.Type == JTokenType.Integer)
                {
                    return new SpatialReference(sr.Value<int>());
                }
            }

            return null;
        }

        protected bool TryReadPoint(JObject o, SpatialReference spatialReference, out Point point)
        {
            if (o.TryGetValue("x", out var x))
            {
                if (o.TryGetValue("y", out var y))
                {
                    if (o.TryGetValue("z", out var z))
                    {
                        if (o.TryGetValue("m", out var m))
                        {
                            point = new Point(
                                x.Value<double>(),
                                y.Value<double>(),
                                z.Value<double>(),
                                m.Value<double>(),
                                spatialReference);
                        }
                        else
                        {
                            point = new Point(
                                x.Value<double>(),
                                y.Value<double>(),
                                z.Value<double>(),
                                spatialReference);
                        }
                    }
                    else
                    {
                        point = new Point(
                            x.Value<double>(), 
                            y.Value<double>(),
                            spatialReference);
                    }

                    return true;
                }
            }

            point = null;
            return false;
        }

        protected bool TryReadEnvelope(JObject o, SpatialReference spatialReference, out Extent extent)
        {
            if (o.TryGetValue("xmin", out var xmin))
            {
                if (o.TryGetValue("xmax", out var xmax) &&
                    o.TryGetValue("ymin", out var ymin) &&
                    o.TryGetValue("ymax", out var ymax))
                {
                    if (o.TryGetValue("zmax", out var zmax) &&
                        o.TryGetValue("zmin", out var zmin))
                    {
                        if (o.TryGetValue("mmax", out var mmax) &&
                            o.TryGetValue("mmin", out var mmin))
                        {
                            extent = new Extent(xmin.Value<double>(),
                                ymin.Value<double>(),
                                zmin.Value<double>(),
                                mmin.Value<double>(),
                                xmax.Value<double>(),
                                ymax.Value<double>(),
                                zmax.Value<double>(),
                                mmax.Value<double>(), spatialReference);
                        }
                        else
                        {
                            extent = new Extent(xmin.Value<double>(),
                                ymin.Value<double>(),
                                zmin.Value<double>(),
                                xmax.Value<double>(),
                                ymax.Value<double>(),
                                zmax.Value<double>(), spatialReference);
                        }
                    }
                    else
                    {
                        extent = new Extent(xmin.Value<double>(),
                            ymin.Value<double>(),
                            xmax.Value<double>(),
                            ymax.Value<double>(), spatialReference);
                    }

                    return true;
                }
            }

            extent = null;
            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var spatial = value as IGeometry;
            if (spatial == null)
            {
                writer.WriteNull();
                return;
            }

            spatial.Accept(new SpatialEmitter(writer));
        }
    }
}