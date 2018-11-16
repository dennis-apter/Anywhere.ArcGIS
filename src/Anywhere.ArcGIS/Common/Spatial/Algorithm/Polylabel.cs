using System;
using System.Collections.Generic;
using System.Linq;
using Anywhere.ArcGIS.Extensions;

namespace Anywhere.ArcGIS.Common.Algorithm
{
    /// <summary>
    /// A fast algorithm for finding the pole of inaccessibility of a polygon.
    /// (Ported from C++ https://github.com/mapbox/polylabel)
    /// </summary>
    public static class Polylabel
    {
        private static Cell GetCentroidCell(Polygon polygon)
        {
            var area = 0d;
            var c = new Point(0, 0);
            var ring = polygon.Rings[0].Points;

            for (int i = 0, len = ring.Count, j = len - 1; i < len; j = i++)
            {
                var a = ring[i];
                var b = ring[j];
                var f = a.X * b.Y - b.X * a.Y;
                c.X += (a.X + b.X) * f;
                c.Y += (a.Y + b.Y) * f;
                area += f * 3;
            }

            return new Cell(area.IsZero() ? ring[0] : c / area, 0, polygon);
        }

        public static Point Find(Polygon polygon, double precision = 1)
        {
            // find the bounding box of the outer ring
            var extent = polygon.Rings[0].GetExtent();

            var sx = extent.XMax - extent.XMin;
            var sy = extent.YMax - extent.YMin;
            var size = new Point(sx, sy);

            var cellSize = Math.Min(sx, sy);
            var h = cellSize / 2d;

            var cellQueue = new CellQueue();
            if (cellSize < 0.1)
            {
                return extent.Min;
            }

            // cover polygon with initial cells
            for (var x = extent.XMin; x < extent.XMax; x += cellSize)
            {
                for (var y = extent.YMin; y < extent.XMax; y += cellSize)
                {
                    cellQueue.Push(new Cell(new Point(x + h, y + h), h, polygon));
                }
            }

            // take centroid as the first best guess
            var bestCell = GetCentroidCell(polygon);

            // special case for rectangular polygons
            var bboxCell = new Cell(extent.Min + size / 2d, 0, polygon);
            if (bboxCell.Distance > bestCell.Distance)
            {
                bestCell = bboxCell;
            }

            //var numProbes = cellQueue.Count;
            while (cellQueue.Count > 0)
            {
                // pick the most promising cell from the queue
                var cell = cellQueue.Pop();

                // update the best cell if we found a better one
                if (cell.Distance > bestCell.Distance)
                {
                    bestCell = cell;
                }

                // do not drill down further if there's no chance of a better solution
                if (cell.Maximum - bestCell.Distance <= precision) continue;

                // split the cell into four cells
                h = cell.Heigth / 2;
                cellQueue.Push(new Cell(new Point(cell.Center.X - h, cell.Center.Y - h), h, polygon));
                cellQueue.Push(new Cell(new Point(cell.Center.X + h, cell.Center.Y - h), h, polygon));
                cellQueue.Push(new Cell(new Point(cell.Center.X - h, cell.Center.Y + h), h, polygon));
                cellQueue.Push(new Cell(new Point(cell.Center.X + h, cell.Center.Y + h), h, polygon));
                //numProbes += 4;
            }

            return bestCell.Center;
        }

        private class Cell
        {
            public readonly Point Center;
            public readonly double Heigth;
            public readonly double Distance;
            public readonly double Maximum;

            public Cell(Point center, double heigth, Polygon polygon)
            {
                Center = center;
                Heigth = heigth;
                Distance = PointToPolygonDist(center, polygon);
                Maximum = Distance + heigth * Math.Sqrt(2);
            }

            private double PointToPolygonDist(Point point, Polygon polygon)
            {
                bool inside = false;
                var minDistSq = double.PositiveInfinity;

                foreach (var r in polygon.Rings)
                {
                    var ring = r.Points;
                    for (int i = 0, len = ring.Count, j = len - 1; i < len; j = i++)
                    {
                        var a = ring[i];
                        var b = ring[j];

                        if ((a.Y > point.Y) != (b.Y > point.Y) &&
                            (point.X < (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y) + a.X)) inside = !inside;

                        minDistSq = Math.Min(minDistSq, GetSegDistSq(point, a, b));
                    }
                }

                return (inside ? 1 : -1) * Math.Sqrt(minDistSq);
            }

            private double GetSegDistSq(Point p, Point a, Point b)
            {
                var x = a.X;
                var y = a.Y;
                var dx = b.X - x;
                var dy = b.Y - y;

                if (dx != 0 || dy != 0)
                {
                    var t = ((p.X - x) * dx + (p.Y - y) * dy) / (dx * dx + dy * dy);

                    if (t > 1)
                    {
                        x = b.X;
                        y = b.Y;
                    }
                    else if (t > 0)
                    {
                        x += dx * t;
                        y += dy * t;
                    }
                }

                dx = p.X - x;
                dy = p.Y - y;

                return dx * dx + dy * dy;
            }
        }

        /// <summary>
        /// a priority queue of cells in order of their "potential" (max distance to polygon)
        /// </summary>
        private class CellQueue : SortedList<double, Cell>
        {
            public CellQueue()
                : base(Comparer<double>.Default)
            {
            }

            public void Push(Cell cell)
            {
                if (!ContainsKey(cell.Maximum))
                {
                    Add(cell.Maximum, cell);
                }
            }

            public Cell Pop()
            {
                var pair = this.First();
                Remove(pair.Key);
                return pair.Value;
            }
        }
    }
}
