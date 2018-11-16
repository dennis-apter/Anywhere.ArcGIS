namespace Anywhere.ArcGIS.Common
{
    public class AbsoluteEndpoint : IEndpoint
    {
        /// <summary>
        /// Create an IEndpoint for the path
        /// </summary>
        /// <param name="path"></param>
        public AbsoluteEndpoint(string path)
        {
            RelativeUrl = path;
        }

        public string RelativeUrl { get; private set; }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            return RelativeUrl;
        }
    }
}