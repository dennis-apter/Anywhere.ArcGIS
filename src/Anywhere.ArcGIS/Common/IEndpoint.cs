namespace Anywhere.ArcGIS.Common
{
    /// <summary>
    /// Represents a REST endpoint
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// Relative url of the resource
        /// </summary>
        string RelativeUrl { get; }

        /// <summary>
        /// Check the url is complete (ignore the scheme)
        /// </summary>
        /// <param name="rootUrl"></param>
        /// <returns></returns>
        string BuildAbsoluteUrl(string rootUrl);
    }
}
