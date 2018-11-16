using System.Collections.Generic;

namespace Anywhere.ArcGIS.Common
{
    public interface IFeature  : IFeatureAttributes
    {
        IGeometry Geometry { get; }
    }

    public interface IFeatureAttributes
    {
        /// <summary>
        /// A JSON object that contains a dictionary of name-value pairs.
        /// The names are the feature field names.
        /// The values are the field values and they can be any of the standard JSON types - string, number and boolean.
        /// </summary>
        /// <remarks>Date values are encoded as numbers. The number represents the number of milliseconds since epoch (January 1, 1970) in UTC.</remarks>
        IDictionary<string, object> Attributes { get; }
    }
}
