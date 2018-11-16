using System;
using System.Collections.Generic;
using Anywhere.ArcGIS.Serialization;

namespace Anywhere.ArcGIS.Common
{
    public abstract class Geometry : ICloneable
    {
        public abstract GeometryType Type { get; }

        public abstract Extent GetExtent();
        public abstract IEnumerable<Point> GetBoundingPoints();

        protected abstract object CloneImpl();
        object ICloneable.Clone() => CloneImpl();

        public override string ToString() => JCompact.From(this);
    }
}
