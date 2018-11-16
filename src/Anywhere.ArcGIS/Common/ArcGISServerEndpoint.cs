using System;

namespace Anywhere.ArcGIS.Common
{
    /// <summary>
    /// Represents an ArcGIS Server REST endpoint
    /// </summary>
    public class ArcGISServerEndpoint : IEndpoint
    {
        /// <summary>
        /// Creates a new ArcGIS Server REST endpoint representation
        /// </summary>
        /// <param name="relativePath">Path of the endpoint relative to the root url of the ArcGIS Server</param>
        public ArcGISServerEndpoint(string relativePath)
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

            if (RelativeUrl.IndexOf("rest/services/", StringComparison.OrdinalIgnoreCase) > -1)
            {
                RelativeUrl = RelativeUrl.Substring(RelativeUrl.LastIndexOf("rest/services/", StringComparison.OrdinalIgnoreCase));
            }

            RelativeUrl = RelativeUrl.Replace("rest/services/", "");
            RelativeUrl = "rest/services/" + RelativeUrl.Trim('/');
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