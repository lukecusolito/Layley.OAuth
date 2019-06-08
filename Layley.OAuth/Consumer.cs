using Layley.OAuth.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Layley.OAuth
{
    public class Consumer
    {
        #region Properties
        public string ConsumerKey { get; }
        public string ConsumerSecret { get; }
        public string AccessToken { get; }
        public string AccessTokenSecret { get; }
        internal Uri CallbackUri { get; }
        #endregion

        #region Fields
        private readonly HMACSHA1 sigHasher;
        private readonly DateTime epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region Constructor
        public Consumer(string consumerKey, string consumerSecret, string accessToken = null, string accessTokenSecret = null)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            AccessToken = accessToken;
            AccessTokenSecret = accessTokenSecret;

            sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes($"{consumerSecret}&{accessTokenSecret}"));
        }

        internal Consumer(string consumerKey, string consumerSecret, string callbackUrl) : this(consumerKey, consumerSecret)
        {
            CallbackUri = new Uri(callbackUrl);
        }
        #endregion

        #region Public Methods
        public AuthenticationHeaderValue GetOAuthHeader(Uri uri, HttpMethod httpMethod)
        {
            return new AuthenticationHeaderValue("OAuth", GetOAuthHeaderValue(uri, httpMethod));
        }

        public string GetOAuthHeaderValue(Uri uri, HttpMethod httpMethod)
        {

            var timestamp = (int)((DateTime.UtcNow - epochUtc).TotalSeconds);
            string oauth_nonce = Guid.NewGuid().ToString("N");

            Dictionary<string, string> queryParameters;

            if (httpMethod == HttpMethod.Get)
                queryParameters = uri.GetQueryParameters();
            else
                queryParameters = new Dictionary<string, string>();

            queryParameters.Add("oauth_consumer_key", ConsumerKey);
            queryParameters.Add("oauth_signature_method", "HMAC-SHA1");
            queryParameters.Add("oauth_timestamp", timestamp.ToString());
            queryParameters.Add("oauth_nonce", oauth_nonce);
            queryParameters.Add("oauth_version", "1.0");

            if (CallbackUri != null)
                queryParameters.Add("oauth_callback", CallbackUri.AbsoluteUri);
            else
                queryParameters.Add("oauth_token", AccessToken);

            queryParameters.Add("oauth_signature", GenerateOAuthSignature(uri, httpMethod, queryParameters));

            return GenerateOAuthHeader(queryParameters);
        }
        #endregion

        #region Private Methods
        private string GenerateOAuthSignature(Uri uri, HttpMethod httpMethod, Dictionary<string, string> parameters)
        {
            var sigString = string.Join(
                "&",
                parameters
                    .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")
                    .OrderBy(x => x, StringComparer.Ordinal)
            );

            var fullSigData =
                $"{httpMethod.Method}&{Uri.EscapeDataString(uri.Path())}&{Uri.EscapeDataString(sigString)}";

            return Convert.ToBase64String(sigHasher.ComputeHash(Encoding.ASCII.GetBytes(fullSigData)));
        }

        private string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return string.Join(
                ", ",
                data
                    .Where(kvp => kvp.Key.StartsWith("oauth_"))
                    .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}=\"{Uri.EscapeDataString(kvp.Value)}\"")
                    .OrderBy(s => s, StringComparer.Ordinal)
            );
        }
        #endregion
    }
}
