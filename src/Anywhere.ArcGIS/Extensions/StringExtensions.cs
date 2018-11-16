﻿using System.Globalization;
using System.Text;

namespace Anywhere.ArcGIS
{
    using Common;
    using System;

    public static class StringExtensions
    {
        public static ArcGISServerEndpoint AsEndpoint(this string relativeUrl)
        {
            return new ArcGISServerEndpoint(relativeUrl);
        }

        public static ArcGISServerAdminEndpoint AsAdminEndpoint(this string relativeUrl)
        {
            return new ArcGISServerAdminEndpoint(relativeUrl);
        }

        public static ArcGISOnlineEndpoint AsArcGISOnlineEndpoint(this string relativeUrl)
        {
            return new ArcGISOnlineEndpoint(relativeUrl);
        }

        public static AbsoluteEndpoint AsAbsoluteEndpoint(this string url)
        {
            return new AbsoluteEndpoint(url);
        }

        public static string AsRootUrl(this string rootUrl)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl), "rootUrl is null.");
            }

            rootUrl = rootUrl.TrimEnd('/');
            if (rootUrl.IndexOf("/rest/admin/services", StringComparison.OrdinalIgnoreCase) > -1)
            {
                rootUrl = rootUrl.Substring(0, rootUrl.IndexOf("/rest/admin/services", StringComparison.OrdinalIgnoreCase));
            }

            if (rootUrl.IndexOf("/rest/services", StringComparison.OrdinalIgnoreCase) > -1)
            {
                rootUrl = rootUrl.Substring(0, rootUrl.IndexOf("/rest/services", StringComparison.OrdinalIgnoreCase));
            }

            if (rootUrl.IndexOf("/admin", StringComparison.OrdinalIgnoreCase) > -1)
            {
                rootUrl = rootUrl.Substring(0, rootUrl.IndexOf("/admin", StringComparison.OrdinalIgnoreCase));
            }

            if (rootUrl.IndexOf("/tokens", StringComparison.OrdinalIgnoreCase) > -1)
            {
                rootUrl = rootUrl.Substring(0, rootUrl.IndexOf("/tokens", StringComparison.OrdinalIgnoreCase));
            }

            return rootUrl.Replace("/rest/services", "") + "/";
        }

        public static string UrlEncode(this string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 65520)
            {
                // this will get sent to POST anyway so don't bother escaping
                return text;
            }
            else
            {
                var t = text.Trim('\"');
                return Uri.EscapeDataString(t);
            }
        }

        /// <summary>
        /// Converts a hex-encoded string to the corresponding byte array.
        /// </summary>
        /// <param name="hex">Hex-encoded string</param>
        /// <returns>Byte representation of the hex-encoded input</returns>
        public static byte[] HexToBytes(this string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;

            int length = hex.Length;

            if (length % 2 != 0)
            {
                length += 1;
                hex = "0" + hex;
            }

            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        internal static string Join<T>(this T[] array, string separator = ",", string format = null, IFormatProvider provider = null)
            where T: IFormattable
        {
            if (provider == null)
            {
                provider = CultureInfo.InvariantCulture;
            }

            var sb = new StringBuilder();
            foreach (var d in array)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(d.ToString(format, provider));
            }

            return sb.ToString();
        }
    }
}
