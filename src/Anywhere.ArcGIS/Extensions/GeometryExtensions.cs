using Anywhere.ArcGIS.Common;
using System.Collections.Generic;

namespace Anywhere.ArcGIS
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// Convert a <see cref="Point"/> into a double[] so that it can be added to a <see cref="GeoJsonCoordinateList"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns>Array with 2 values</returns>
        public static double[] ToPointCollectionEntry(this Point point)
        {
            return new[] {point.X, point.Y};
        }

        /// <summary>
        /// Convert a collection of points into a <see cref="Ring"/> that can be used as paths in <see cref="Polyline"/> types
        /// or rings in <see cref="Polygon"/> types
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<List<double[]>> ToPointCollectionList(this IEnumerable<Point> points)
        {
            var result = new List<List<double[]>>();
            var pointCollection = new List<double[]>();
            foreach (var point in points)
            {
                pointCollection.Add(point.ToPointCollectionEntry());
            }

            result.Add(pointCollection);
            return result;
        }


        public static IEnumerable<double[]> ToCoordinates(this IEnumerable<Point> points)
        {
            if (points == null)
            {
                yield break;
            }

            foreach (var point in points)
            {
                yield return point.ToCoordinates();
            }
        }

        public static Extent GetExtent(this IEnumerable<Point> points)
        {
            using (IEnumerator<Point> en = points.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    var c = en.Current;
                    double x1, x2;
                    x1 = x2 = c.X;
                    double y1, y2;
                    y1 = y2 = c.Y;
                    double? z1, z2;
                    z1 = z2 = c.Z;
                    double? m1, m2;
                    m1 = m2 = c.M;

                    if (z1.HasValue && m2.HasValue)
                    {
                        while (en.MoveNext())
                        {
                            var p = en.Current;
                            if (p == null)
                            {
                                continue;
                            }

                            if (p.X < x1) x1 = p.X;
                            if (p.X > x2) x2 = p.X;

                            if (p.Y < y1) y1 = p.Y;
                            if (p.Y > y2) y2 = p.Y;

                            if (p.Z < z1) z1 = p.Z;
                            if (p.Z > z2) z2 = p.Z;

                            if (p.M < m1) m1 = p.M;
                            if (p.M > m2) m2 = p.M;
                        }

                        return new Extent(x1, y1, z1.Value, m1.Value, x2, y2, z2.Value, m2.Value, c.SpatialReference);
                    }
                    else if (z1.HasValue)
                    {
                        while (en.MoveNext())
                        {
                            var p = en.Current;
                            if (p == null)
                            {
                                continue;
                            }

                            if (p.X < x1) x1 = p.X;
                            if (p.X > x2) x2 = p.X;

                            if (p.Y < y1) y1 = p.Y;
                            if (p.Y > y2) y2 = p.Y;

                            if (p.Z < z1) z1 = p.Z;
                            if (p.Z > z2) z2 = p.Z;
                        }

                        return new Extent(x1, y1, z1.Value, x2, y2, z2.Value, c.SpatialReference);
                    }
                    else
                    {
                        while (en.MoveNext())
                        {
                            var p = en.Current;
                            if (p == null)
                            {
                                continue;
                            }

                            if (p.X < x1) x1 = p.X;
                            if (p.X > x2) x2 = p.X;

                            if (p.Y < y1) y1 = p.Y;
                            if (p.Y > y2) y2 = p.Y;
                        }

                        return new Extent(x1, y1, x2, y2, c.SpatialReference);
                    }
                }
            }

            return Extent.Empty;
        }
    }
}
