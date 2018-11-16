using System;

namespace Anywhere.ArcGIS.Common
{
    public class RootServerEndpoint : IEndpoint
    {
        /// <summary>
        /// Create an IEndpoint for the path
        /// </summary>
        /// <param name="path"></param>
        public RootServerEndpoint(string path)
        {
            RelativeUrl = path;
        }

        public string RelativeUrl { get; private set; }

        public string BuildAbsoluteUrl(string rootUrl)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            return !RelativeUrl.Contains(rootUrl.Substring(6)) && !RelativeUrl.Contains(rootUrl.Substring(6))
                ? rootUrl.Trim('/') + "/" + RelativeUrl
                : RelativeUrl;
        }
    }
}
