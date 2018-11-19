using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Anywhere.ArcGIS.Logging;

namespace Anywhere.ArcGIS
{
    public static class HttpClientFactory
    {
        private static readonly ILog Log = LogProvider.GetLogger(typeof(HttpClientFactory));

        public static Func<HttpClientHandler> CreateHttpClientHandler { get; set; } = () => new HttpClientHandler();

        public static Func<HttpClientHandler, HttpClient> CreateHttpClient { get; set; } = httpClientHandler => new HttpClient(httpClientHandler);

        public static Action<HttpClientHandler> InitHttpClientHandler { get; set; } =
            httpClientHandler =>
            {
                if (httpClientHandler.SupportsAutomaticDecompression)
                {
                    httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }

                if (httpClientHandler.SupportsProxy)
                {
                    httpClientHandler.UseProxy = true;
                }

                if (httpClientHandler.SupportsRedirectConfiguration)
                {
                    httpClientHandler.AllowAutoRedirect = true;
                }

                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    Log.Debug($"Sender: {sender}, cert: {cert}, chain: {chain}, sslPolicyErrors: {sslPolicyErrors}.");
                    return true;
                };
            };

        public static Action<HttpClient> InitHttpClient { get; set; } =
            httpClient =>
            {
                httpClient.Timeout = TimeSpan.FromMinutes(10);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsonp"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            };

        public static Func<HttpClientHandler> GetHttpClientHandler { get; set; } = () =>
        {
            var httpClientHandler = CreateHttpClientHandler();
            InitHttpClientHandler(httpClientHandler);
            return httpClientHandler;
        };

        public static Func<HttpClient> Get { get; set; } =
            () =>
            {
                var httpClientHandler = GetHttpClientHandler();

                httpClientHandler.PreAuthenticate = true;

                var httpClient = CreateHttpClient(httpClientHandler);
                InitHttpClient(httpClient);
                return httpClient;
            };

        public static Func<IWebProxy, HttpClient> GetSecureProxyConnection { get; set; } =
            wp =>
            {
                var httpClientHandler = GetHttpClientHandler();

                if (wp != null)
                {
                    httpClientHandler.Proxy = wp;
                    httpClientHandler.PreAuthenticate = true;
                    httpClientHandler.UseDefaultCredentials = false;
                }

                var httpClient = new HttpClient(httpClientHandler);
                InitHttpClient(httpClient);
                return httpClient;
            };

        public static Func<HttpClient> GetWindowsIntegrated { get; set; } =
            () =>
            {
                var httpClientHandler = GetHttpClientHandler();

                httpClientHandler.PreAuthenticate = true;

                // TODO : check this works httpClientHandler.UseDefaultCredentials = true; is the current context which
                // would be the app pool user
                httpClientHandler.Credentials = CredentialCache.DefaultNetworkCredentials;

                var httpClient = new HttpClient(httpClientHandler);
                InitHttpClient(httpClient);
                return httpClient;
            };

        public static Func<string, string, string, HttpClient> GetWindowsNamedUser { get; set; } =
            (username, password, domain) =>
            {
                var httpClientHandler = GetHttpClientHandler();

                httpClientHandler.PreAuthenticate = true;
                httpClientHandler.UseDefaultCredentials = false;

                // check for domain in username
                httpClientHandler.Credentials = string.IsNullOrWhiteSpace(domain)
                    ? new NetworkCredential(username, password)
                    : new NetworkCredential(username, password, domain);

                var httpClient = new HttpClient(httpClientHandler);
                InitHttpClient(httpClient);
                return httpClient;
            };
    }
}
