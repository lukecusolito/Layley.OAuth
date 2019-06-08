using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Layley.OAuth.Extensions
{
    internal static class UriExtensions
    {
        internal static Uri AddQuery(this Uri uri, string name, string value)
        {
            string newUrl = uri.OriginalString;

            if (newUrl.EndsWith("&") || newUrl.EndsWith("?"))
                newUrl = $"{newUrl}{name}={value}";
            else if (newUrl.Contains("?"))
                newUrl = $"{newUrl}&{name}={value}";
            else
                newUrl = $"{newUrl}?{name}={value}";

            return new Uri(newUrl);
        }

        internal static string Path(this Uri uri) =>
            uri.GetLeftPart(UriPartial.Path);

        internal static Dictionary<string, string> GetQueryParameters(this Uri uri)
        {
            if (!string.IsNullOrWhiteSpace(uri.Query))
            {
                var nvc = HttpUtility.ParseQueryString(uri.Query);
                return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
            }

            return new Dictionary<string, string>();
        }
    }
}
