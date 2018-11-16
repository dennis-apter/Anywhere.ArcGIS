using System;

namespace Anywhere.ArcGIS.Common
{
    public class ArcGISOnlineEndpoint : IEndpoint
    {
        /// <summary>
        /// Creates a new ArcGIS Online or Portal REST endpoint representation
        /// </summary>
        /// <param name="relativePath">Path of the endpoint relative to the root url of ArcGIS Online / Portal</param>
        public ArcGISOnlineEndpoint(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath), "relativePath is null.");
            }

            if (!Uri.TryCreate(relativePath, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                throw new InvalidOperationException("Not a valid relative url " + relativePath);
            }

            if (uri.IsAbsoluteUri)
            {
                RelativeUrl = uri.AbsolutePath.Trim('/') + "/";
            }
            else
            {
                RelativeUrl = uri.OriginalString.Trim('/') + "/";
            }

            if (RelativeUrl.IndexOf("sharing/rest/", StringComparison.OrdinalIgnoreCase) > -1)
            {
                RelativeUrl = RelativeUrl.Substring(RelativeUrl.LastIndexOf("sharing/rest/", StringComparison.OrdinalIgnoreCase));
            }

            RelativeUrl = RelativeUrl.Replace("sharing/rest/", "");
            RelativeUrl = "sharing/rest/" + RelativeUrl.Trim('/');
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