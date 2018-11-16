using System;
using Anywhere.ArcGIS.GeoJson;
using Anywhere.ArcGIS.Serialization;
using Newtonsoft.Json;

namespace Anywhere.ArcGIS.Common
{
    /// <summary>
    /// The ArcGIS Server REST API supports the following five geometry types:
    /// Points,
    /// Multipoints,
    /// Polylines,
    /// Polygons,
    /// Envelopes
    /// </summary>
    /// <remarks>Starting at ArcGIS Server 10.1, geometries containing m and z values are supported</remarks>
    [JsonConverter(typeof(SpatialConverter))]
    public interface IGeometry : ICloneable
    {
        /// <summary>
        /// The spatial reference can be defined using a well-known ID (wkid) or well-known text (wkt)
        /// </summary>
        SpatialReference SpatialReference { get; set; }

        GeometryType Type { get; }

        /// <summary>
        /// Calculates the minimum bounding extent for the geometry
        /// </summary>
        /// <returns>Extent that can contain the geometry</returns>
        Extent GetExtent();

        /// <summary>
        /// Calculates the center of the minimum bounding extent for the geometry
        /// </summary>
        /// <returns>The value for the center of the extent for the geometry</returns>
        Point GetCenter();

        /// <summary>
        /// Calculates the pole of inaccessibility of a geometry
        /// </summary>
        /// <returns>The pole of inaccessibility</returns>
        Point GetPole();

        /// <summary>
        /// Converts the geometry to its GeoJSON representation
        /// </summary>
        /// <returns>The corresponding GeoJSON for the geometry</returns>
        IGeoJsonGeometry ToGeoJson();

        void Accept(ISpatialVisitor visitor);
    }
}
