using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Anywhere.ArcGIS.Logging;

namespace Anywhere.ArcGIS
{
    public static class HttpClientFactory
    {
        public static Func<HttpClient> Get { get; set; }

        public static Func<IWebProxy, HttpClient> GetSecureProxyConnection { get; set; }

        public static Func<HttpClient> GetWindowsIntegrated { get; set; }

        public static Func<string, string, string, HttpClient> GetWindowsNamedUser { get; set; }

        static HttpClientFactory()
        {
            var timeout = TimeSpan.FromMinutes(10);
            var log = LogProvider.GetLogger(typeof(HttpClientFactory));

            Get = (() =>
            {
                var httpClientHandler = new HttpClientHandler();
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

                httpClientHandler.PreAuthenticate = true;

                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    log.Debug($"Sender: {sender}, cert: {cert}, chain: {chain}, sslPolicyErrors: {sslPolicyErrors}.");
                    return true;
                };

                var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = timeout
                };

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsonp"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                return httpClient;
            });

            GetSecureProxyConnection = (IWebProxy wp) =>
            {
                var httpClientHandler = new HttpClientHandler();
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

                if (wp != null)
                {
                    httpClientHandler.Proxy = wp;
                    httpClientHandler.PreAuthenticate = true;
                    httpClientHandler.UseDefaultCredentials = false;
                }

                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    log.Debug($"Sender: {sender}, cert: {cert}, chain: {chain}, sslPolicyErrors: {sslPolicyErrors}.");
                    return true;
                };

                var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = timeout
                };

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsonp"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                return httpClient;
            };

            GetWindowsIntegrated = (() =>
            {
                var httpClientHandler = new HttpClientHandler();
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

                httpClientHandler.PreAuthenticate = true;

                // TODO : check this works httpClientHandler.UseDefaultCredentials = true; is the current context which
                // would be the app pool user
                httpClientHandler.Credentials = CredentialCache.DefaultNetworkCredentials;

                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    log.Debug($"Sender: {sender}, cert: {cert}, chain: {chain}, sslPolicyErrors: {sslPolicyErrors}.");
                    return true;
                };

                var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = timeout
                };

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsonp"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                return httpClient;
            });

            GetWindowsNamedUser = ((username, password, domain) =>
            {
                var httpClientHandler = new HttpClientHandler();
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

                httpClientHandler.PreAuthenticate = true;
                httpClientHandler.UseDefaultCredentials = false;

                // check for domain in username
                httpClientHandler.Credentials = string.IsNullOrWhiteSpace(domain)
                    ? new NetworkCredential(username, password)
                    : new NetworkCredential(username, password, domain);

                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    log.Debug($"Sender: {sender}, cert: {cert}, chain: {chain}, sslPolicyErrors: {sslPolicyErrors}.");
                    return true;
                };

                var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = timeout
                };

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsonp"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                return httpClient;
            });
        }
    }
}